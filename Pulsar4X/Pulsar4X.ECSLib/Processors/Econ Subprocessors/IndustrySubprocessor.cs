using System;
using System.Collections.Generic;
using System.Linq;

namespace Pulsar4X.ECSLib
{
    public class IndustrySubprocessor
    {
        private readonly Game _game;
        private readonly MiningSubprocessor _miningSubprocessor;
        internal DateTime NextHaltTime { get; private set; } = DateTime.MaxValue;

        internal IndustrySubprocessor(Game game)
        {
            _game = game;
            _miningSubprocessor = new MiningSubprocessor(game);
        }

        internal void Process(StarSystem system)
        {
            List<Entity> industialEntities = system.SystemManager.GetAllEntitiesWithDataBlob<IndustryDB>();

            foreach (Entity entity in industialEntities)
            {
                var industrialEntity = new IndustrialEntity(entity);
                UpdateIndustryDB(industrialEntity);
                
                // TODO: Research
                // TODO: ProcessTerraforming(entity, industryDB);
                // TODO: JP stabilization
                // TODO: Salvage
                _miningSubprocessor.ProcessMining(industrialEntity);
                if (_miningSubprocessor.NextHaltTime < NextHaltTime)
                {
                    NextHaltTime = _miningSubprocessor.NextHaltTime;
                }

                ProcessJobs(industrialEntity, IndustryType.Refining);
                ProcessJobs(industrialEntity, IndustryType.ComponentConstruction);
                ProcessJobs(industrialEntity, IndustryType.OrdnanceConstruction);
                ProcessJobs(industrialEntity, IndustryType.FighterConstruction);
                ProcessJobs(industrialEntity, IndustryType.ShipConstruction);
                ProcessJobs(industrialEntity, IndustryType.InstallationConstruction);
            }
        }

        internal static void UpdateIndustryDB(IndustrialEntity industrialEntity)
        {
            industrialEntity.IndustryDB.industryRates = GetIndustrialRates(industrialEntity);
        }

        private static Dictionary<IndustryType, float> GetIndustrialRates(IndustrialEntity industrialEntity)
        {
            var components = industrialEntity.Entity.GetDataBlob<ComponentInstancesDB>();

            // Get the combined-type capacities.
            var industryCapacity = new Dictionary<IndustryType, float>();

            foreach (KeyValuePair<Entity, List<ComponentInstance>> specificInstance in components.specificInstances)
            {
                Entity componentDefinition = specificInstance.Key;

                var industryAbilityDB = componentDefinition.GetDataBlob<IndustryAbilityDB>();
                if (industryAbilityDB == null)
                {
                    continue;
                }

                int functionalInstances = specificInstance.Value.Count(instance => instance.IsEnabled);

                foreach (KeyValuePair<IndustryType, int> constructionPoint in industryAbilityDB.industryRates)
                {
                    industryCapacity.SafeValueAdd(constructionPoint.Key, constructionPoint.Value * functionalInstances);
                }
            }

            // Get the single-type capacities.
            var industialRates = new Dictionary<IndustryType, float>();
            foreach (KeyValuePair<IndustryType, float> pair in industryCapacity)
            {
                IndustryType type = pair.Key;

                foreach (IndustryType value in Enum.GetValues(typeof(IndustryType)))
                {
                    if ((value & type) == value)
                    {
                        industialRates.SafeValueAdd(value, pair.Value);
                    }
                }
            }
            
            return ApplyIndustryBonuses(industrialEntity, industialRates); ;
        }

        internal static Dictionary<IndustryType, float> ApplyIndustryBonuses(IndustrialEntity industrialEntity, Dictionary<IndustryType, float> industialRates)
        {
            var applicableBonuses = new List<BonusesDB>();

            // Apply Faction bonuses.
            Entity ownerFaction = industrialEntity.OwnedDB.EntityOwner;
            var factionBonusesDB = ownerFaction.GetDataBlob<BonusesDB>();

            if (factionBonusesDB != null)
            {
                applicableBonuses.Add(factionBonusesDB);
            }

            // Apply Plantary/System Bonuses.
            Entity parentEntity = industrialEntity.MatedToDB?.Parent;
            var parentBonusesDB = parentEntity?.GetDataBlob<BonusesDB>();

            if (parentBonusesDB != null)
            {
                applicableBonuses.Add(parentBonusesDB);
            }

            // Apply Ship Captain/Colony Govenor Bonuses.
            if (industrialEntity.MatedToDB != null)
            {
                foreach (Entity childEntity in industrialEntity.MatedToDB.Children)
                {
                    // Check if the child is valid for applying bonuses
                    // Currently only Leaders are valid.
                    var childLeaderDB = childEntity.GetDataBlob<LeaderDB>();
                    if (childLeaderDB == null || childLeaderDB.AssignedTo != industrialEntity.Entity)
                    {
                        continue;
                    }

                    // Child is valid, add their bonuses to the childBonuses list.
                    var childBonusDB = childEntity.GetDataBlob<BonusesDB>();
                    if (childBonusDB != null)
                    {
                        applicableBonuses.Add(childBonusDB);
                    }
                    break;
                }
            }

            // TODO: Sector Command
            // TODO: Fleet Command

            // Now that the BonusDB's have been assembled, loop through and apply their bonuses.
            foreach (BonusesDB BonusesDB in applicableBonuses)
            {
                
                foreach (KeyValuePair<IndustryType, float> industialRate in industialRates )
                {
                    IndustryType industryType = industialRate.Key;
                    
                    float currentBonus;
                    if (!BonusesDB.industrialBonuses.TryGetValue(industryType, out currentBonus))
                    {
                        currentBonus = 1;
                    }
                    
                    industialRates[industryType] = industialRate.Value * currentBonus;
                }
            }

            return industialRates;
        }

        public static float GetIndustrialMultiplier(Game game, Guid itemGuid, IndustryDB industryDB)
        {
            float industryMultiplier;
            if (industryDB.industryMultipliers.TryGetValue(itemGuid, out industryMultiplier))
            {
                return industryMultiplier;
            }
            if (industryDB.industryMultipliers.TryGetValue(Guid.Empty, out industryMultiplier))
            {
                return industryMultiplier;
            }
            return 1;
        }

        private float ProcessJob(IndustrialEntity industrialEntity, Dictionary<CargoDefinition, float> materialRequirements, IndustryJob industryJob)
        {
            // Get annual production for this industry type.
            float annualProduction = industrialEntity.IndustryDB.industryRates[industryJob.IndustryType];

            // Apply the specific-item bonus
            float maxProduction = annualProduction * GetIndustrialMultiplier(_game, industryJob.ItemGuid, industrialEntity.IndustryDB);
            annualProduction = maxProduction * industryJob.PercentToUtilize;

            // Get the production points for this tick.
            double tickProduction = annualProduction * (_game.Settings.EconomyCycleTime.TotalDays / 365);

            if (tickProduction <= 0)
            {
                return 0;
            }

            // Get the total number of items to produce
            double numberToProduce = Math.Min(Math.Floor(tickProduction / industryJob.BPPerItem), industryJob.NumberOrdered);
            bool productionIsRestricted = false;

            // Check if we have enough raw materials to perform the job.
            foreach (KeyValuePair<CargoDefinition, float> materialRequirement in materialRequirements)
            {
                double materialCarried;
                if (!industrialEntity.CargoDB.cargoCarried.TryGetValue(materialRequirement.Key, out materialCarried))
                {
                    materialCarried = 0;
                }

                double haveMaterialsFor = materialCarried / materialRequirement.Value;
                if (haveMaterialsFor >= numberToProduce)
                {
                    continue;
                }

                // Not enough materials. Check if we can pull from our host.
                if (industrialEntity.IndustryDB.CanPullFromHost)
                {
                    Entity parent = industrialEntity.MatedToDB?.Parent;
                    var parentCargo = parent?.GetDataBlob<CargoDB>();
                    double parentMaterialCarried;
                    if (parentCargo == null || !parentCargo.cargoCarried.TryGetValue(materialRequirement.Key, out parentMaterialCarried))
                    {
                        parentMaterialCarried = 0;
                    }

                    haveMaterialsFor = Math.Floor((materialCarried + parentMaterialCarried) / materialRequirement.Value);
                }

                if (haveMaterialsFor < numberToProduce)
                {
                    // Still not enough materials.
                    numberToProduce = haveMaterialsFor;
                    productionIsRestricted = true;
                }
            }

            if (productionIsRestricted)
            {
                var notEnoughVespeneGas = new Event(_game.CurrentDateTime, "Material shortage in production.", EventType.MaterialShortage, null, industrialEntity.Entity);
                _game.EventLog.AddEvent(notEnoughVespeneGas);
            }

            // Check if we have free cargospace to store the output.
            CargoDefinition outputCargoDef = CargoHelper.GetCargoDefinition(_game, industryJob.ItemGuid);
            double haveSpaceFor = Math.Floor(CargoHelper.GetFreeCargoSpace(industrialEntity.CargoDB, outputCargoDef.Type) / outputCargoDef.Weight);

            if (haveSpaceFor < Math.Ceiling(numberToProduce))
            {
                numberToProduce = Math.Floor(haveSpaceFor);
                var notEnoughVespeneGas = new Event(_game.CurrentDateTime, "Production halted due to lack of cargo space.", EventType.CargoFull, null, industrialEntity.Entity);
                _game.EventLog.AddEvent(notEnoughVespeneGas);
            }

            if (numberToProduce == 0)
            {
                return 0;
            }

            // Remove the required materials from the entity.
            foreach (KeyValuePair<CargoDefinition, float> materialRequirement in materialRequirements)
            {
                industrialEntity.CargoDB.cargoCarried[materialRequirement.Key] -= materialRequirement.Value * numberToProduce;

                if (industrialEntity.CargoDB.cargoCarried[materialRequirement.Key] < 0)
                {
                    industrialEntity.MatedToDB.Parent.GetDataBlob<CargoDB>().cargoCarried[materialRequirement.Key] += industrialEntity.CargoDB.cargoCarried[materialRequirement.Key];
                    industrialEntity.CargoDB.cargoCarried[materialRequirement.Key] = 0;
                }

            }

            // Update the job
            int totalCompleted = UpdateIndustryJob(industrialEntity, industryJob, annualProduction, numberToProduce);

            // Add completed products
            DeliverProducts(industrialEntity, industryJob, outputCargoDef, totalCompleted);

            // Return the percentage of produciton used for this job.
            return maxProduction / (float)(numberToProduce * industryJob.BPPerItem);
        }

        private int UpdateIndustryJob(IndustrialEntity industrialEntity, IndustryJob industryJob, float annualProduction, double numberToProduce)
        {
            // Apply partial construction
            int numberCompleted = (int)Math.Floor(numberToProduce);

            // numberToProduce is not an int. We may have produced 1.5 this run. We completed 1. leftOver = 0.5
            double leftOver = numberToProduce - numberCompleted;
            // Apply that 0.5 leftOver to our PartialBPApplied.
            industryJob.PartialBPApplied += (float)(industryJob.BPPerItem * leftOver);
            
            // PartialBPApplied is cumulative from the last run, if we had 0.7, and add 0.5, we now have 1.2; make it 0.2 and add one to numberCompled.
            while (industryJob.PartialBPApplied >= industryJob.BPPerItem)
            {
                // Done in a loop so if BPPerItem is changed (SD change, or something) you complete the right number no matter what.
                industryJob.PartialBPApplied -= industryJob.BPPerItem;
                numberCompleted++;
            }

            industryJob.NumberCompleted += numberCompleted;

            if (industryJob.NumberCompleted == industryJob.NumberOrdered)
            {
                if (industryJob.AutoRepeat)
                {
                    industryJob.NumberCompleted = 0;
                }
                else
                {
                    var completedEvent = new Event(_game.CurrentDateTime, "Production job completed.", EventType.ProductionCompleted, null, industrialEntity.Entity);
                    _game.EventLog.AddEvent(completedEvent);
                    industrialEntity.IndustryDB.industryJobs[industryJob.IndustryType].Remove(industryJob);
                }
            }
            double totalRemainingBP = industryJob.BPPerItem * (industryJob.NumberCompleted - industryJob.NumberOrdered) - industryJob.PartialBPApplied;
            double remainingDays = totalRemainingBP / annualProduction / 365;
            TimeSpan remainingTime = TimeSpan.FromDays(remainingDays);

            industryJob.ProjectedCompletion = _game.CurrentDateTime + remainingTime;
            if (industryJob.ProjectedCompletion < NextHaltTime)
            {
                NextHaltTime = industryJob.ProjectedCompletion;
            }
            return numberCompleted;
        }

        private void DeliverProducts(IndustrialEntity industrialEntity, IndustryJob industryJob, CargoDefinition outputCargoDefinition, int numberCompleted)
        {
            // TODO: Properly deliver non-cargo items
            switch (industryJob.IndustryType)
            {
                case IndustryType.InstallationConstruction:
                    var componentInstancesDB = industrialEntity.Entity.GetDataBlob<ComponentInstancesDB>();
                    if (componentInstancesDB != null)
                    {
                        // Add it to the entity's installationDB
                        Entity installationDesign = _game.GlobalManager.GetLocalEntityByGuid(industryJob.ItemGuid);

                        var newInstallation = new ComponentInstance(installationDesign);

                        if (componentInstancesDB.specificInstances.ContainsKey(installationDesign))
                        {
                            componentInstancesDB.specificInstances[installationDesign].Add(newInstallation);
                        }
                        else
                        {
                            componentInstancesDB.specificInstances.Add(installationDesign, new List<ComponentInstance> { newInstallation });
                        }
                    }
                    else
                    {
                        // Throw it in the cargo.
                        industrialEntity.CargoDB.cargoCarried.SafeValueAdd(outputCargoDefinition, numberCompleted);
                    }
                    break;
                case IndustryType.FighterConstruction:
                    // TODO: Spawn a fighter at my location, find default location/taskgroup for them, and mate it to them.
                    // Probably do this in another function.
                    throw new NotImplementedException("Delivery of Fighters not implemented.");
                    break;
                case IndustryType.ShipConstruction:
                    // TODO: Spawn a ship at my location, find a default taskgroup for it, and add them to it.
                    // Probably do this in another function.
                    throw new NotImplementedException("Delivery of ships not implemented");
                    break;
                default:
                    // Throw everything else in the cargo hold.
                    industrialEntity.CargoDB.cargoCarried.SafeValueAdd(outputCargoDefinition, numberCompleted);
                    break;
            }
        }



        private void ProcessJobs(IndustrialEntity industrialEntity, IndustryType industryType)
        {
            float percentUtilized = 0;

            LinkedList<IndustryJob> industryJobs = industrialEntity.IndustryDB.industryJobs[industryType];
            LinkedListNode<IndustryJob> currentNode = industryJobs.First;

            while (percentUtilized < 1 && currentNode != null)
            {
                IndustryJob industryJob = currentNode.Value;

                float jobIndustryToUtilize = industryJob.PercentToUtilize;

                if (jobIndustryToUtilize > 1 - percentUtilized)
                {
                    industryJob.PercentToUtilize = 1 - percentUtilized;
                }
                
                var materialRequirements = new Dictionary<CargoDefinition, float>();
                foreach (KeyValuePair<Guid, float> rawMineralCost in industryJob.materialsRequiredPerItem)
                {
                    materialRequirements.Add(CargoHelper.GetCargoDefinition(_game, rawMineralCost.Key), rawMineralCost.Value);
                }

                percentUtilized += ProcessJob(industrialEntity, materialRequirements, industryJob);
                industryJob.PercentToUtilize = jobIndustryToUtilize;

                currentNode = currentNode.Next;
            }
        }

        #region Public Interactions

        [PublicAPI]
        public static void AddPendingJob(Entity entity, IndustryJob job)
        {
            var industryDB = entity?.GetDataBlob<IndustryDB>();

            if (industryDB == null)
            {
                throw new ArgumentException("Provided entity is not capable of performing industry.", nameof(entity));
            }

            industryDB.industryJobs[job.IndustryType].AddLast(job);
        }

        [PublicAPI]
        public static void RemoveJob(Entity entity, IndustryJob job)
        {
            var industryDB = entity?.GetDataBlob<IndustryDB>();

            if (industryDB == null)
            {
                throw new ArgumentException("Provided entity is not capable of performing industry.", nameof(entity));
            }

            if (!industryDB.industryJobs[job.IndustryType].Remove(job))
            {
                throw new ArgumentException("Provided job not found on entity");
            }
        }

        [PublicAPI]
        public static void ReorderJob(Entity entity, IndustryJob job, int newIndex)
        {
            var industryDB = entity?.GetDataBlob<IndustryDB>();

            if (industryDB == null)
            {
                throw new ArgumentException("Provided entity is not capable of performing industry.", nameof(entity));
            }

            LinkedList<IndustryJob> industryJobs = industryDB.industryJobs[job.IndustryType];

            if (newIndex < 0 || newIndex > industryJobs.Count)
            {
                throw new IndexOutOfRangeException($"{nameof(newIndex)} is out of the range of the LinkedList.");
            }

            if (!industryJobs.Remove(job))
            {
                throw new ArgumentException("Provided job not found on entity");
            }

            LinkedListNode<IndustryJob> currentNode = industryJobs.First;
            for (int i = 0; i < industryJobs.Count; i++)
            {
                if (currentNode == null)
                {
                    industryJobs.AddLast(job);
                    break;
                }

                if (i == newIndex)
                {
                    industryJobs.AddBefore(currentNode, job);
                    break;
                }
                currentNode = currentNode.Next;
            }
        }

        #endregion

    }
}

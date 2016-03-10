using System;
using System.Collections.Generic;
using System.Linq;

namespace Pulsar4X.ECSLib.IndustryProcessors
{
    /// <summary>
    /// Helper class that pulls the DB's from the EntityManager ONCE for multiple uses.
    /// </summary>
    internal class IndustrialEntity
    {
        internal readonly Entity Entity;
        internal readonly CargoDB CargoDB;
        internal readonly IndustryDB IndustryDB;
        internal readonly MatedToDB MatedToDB;
        internal readonly OwnedDB OwnedDB;

        public IndustrialEntity(Entity entity)
        {
            Entity = entity;
            CargoDB = entity.GetDataBlob<CargoDB>();
            IndustryDB = entity.GetDataBlob<IndustryDB>();
            MatedToDB = entity.GetDataBlob<MatedToDB>();
            OwnedDB = entity.GetDataBlob<OwnedDB>();
        }
    }

    public class IndustrySubprocessor
    {
        private readonly Game _game;
        private readonly MiningSubprocessor _miningSubprocessor;

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
                IndustrialEntity industrialEntity = new IndustrialEntity(entity);
                UpdateIndustryDB(industrialEntity);

                // ProcessTerraforming(entity, industryDB);
                // JP stabilization
                // Research
                // Salvage
                _miningSubprocessor.ProcessMining(industrialEntity);
                ProcessJobs(industrialEntity, IndustryType.Refining);
                ProcessJobs(industrialEntity, IndustryType.ComponentConstruction);
                ProcessJobs(industrialEntity, IndustryType.OrdnanceConstruction);
                ProcessJobs(industrialEntity, IndustryType.FighterConstruction);
                ProcessJobs(industrialEntity, IndustryType.ShipConstruction);
                ProcessJobs(industrialEntity, IndustryType.InstallationConstruction);
            }
        }

        private static void UpdateIndustryDB(IndustrialEntity industrialEntity)
        {
            industrialEntity.IndustryDB.industryRates = GetIndustrialRates(industrialEntity);
        }

        private static Dictionary<IndustryType, float> GetIndustrialRates(IndustrialEntity industrialEntity)
        {
            var components = industrialEntity.Entity.GetDataBlob<ComponentInstancesDB>();

            // Get the combined-type capacities.
            var industryCapacity = new Dictionary<IndustryType, float>();

            foreach (KeyValuePair<Entity, List<ComponentInstance>> specificInstance in components.SpecificInstances)
            {
                Entity componentDefinition = specificInstance.Key;

                var industryAbilityDB = componentDefinition.GetDataBlob<IndustryAbilityDB>();
                if (industryAbilityDB != null)
                {
                    int functionalInstances = specificInstance.Value.Count(instance => instance.IsEnabled);

                    foreach (KeyValuePair<IndustryType, int> constructionPoint in industryAbilityDB.industryRates)
                    {
                        industryCapacity.SafeValueAdd(constructionPoint.Key, constructionPoint.Value * functionalInstances);
                    }
                }
            }

            // Get the single-type capacities.
            var industialRates = new Dictionary<IndustryType, float>();
            foreach (KeyValuePair<IndustryType, float> pair in industryCapacity)
            {
                IndustryType type = pair.Key;

                foreach (IndustryType value in Enum.GetValues(typeof(IndustryType)))
                {
                    if ((value & type) != 0)
                    {
                        industialRates.SafeValueAdd(value, pair.Value);
                    }
                }
            }
            
            Entity ownerFaction = industrialEntity.OwnedDB.EntityOwner;
            var factionBonusesDB = ownerFaction.GetDataBlob<EntityBonusesDB>();
            
            Entity parentEntity = industrialEntity.MatedToDB?.Parent;
            var parentBonusesDB = parentEntity?.GetDataBlob<EntityBonusesDB>();

            var childBonuses = new List<EntityBonusesDB>();
            if (industrialEntity.MatedToDB != null)
            {
                // Check for children that are actually present at this location.
                foreach (Entity childEntity in industrialEntity.MatedToDB.Children)
                {
                    // Check if the child is valid for applying bonuses
                    // Currently only leaders are valid.
                    var childLeaderDB = childEntity.GetDataBlob<LeaderDB>();
                    if (childLeaderDB == null || childLeaderDB.AssignedTo != industrialEntity.Entity)
                    {
                        continue;
                    }

                    // Child is valid, add their bonuses to the childBonuses list.
                    var childBonusDB = childEntity.GetDataBlob<EntityBonusesDB>();
                    if (childBonusDB != null)
                    {
                        childBonuses.Add(childBonusDB);
                    }
                }
            }

            foreach (KeyValuePair<IndustryType, float> industialRate in industialRates)
            {
                var industryType = industialRate.Key;

                float factionBonus;
                if (factionBonusesDB == null || !factionBonusesDB.industrialBonuses.TryGetValue(industryType, out factionBonus))
                {
                    factionBonus = 1;
                }

                float parentBonus;
                if (parentBonusesDB == null || !parentBonusesDB.industrialBonuses.TryGetValue(industryType, out parentBonus))
                {
                    parentBonus = 1;
                }

                float childrenBonuses = 1;
                foreach (EntityBonusesDB entityBonusesDB in childBonuses)
                {
                    float childBonus;
                    if (!entityBonusesDB.industrialBonuses.TryGetValue(industryType, out childBonus))
                    {
                        childBonus = 1;
                    }
                    childrenBonuses = childrenBonuses * childBonus;
                }

                industialRates[industryType] = industialRate.Value * factionBonus * parentBonus * childrenBonuses;
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
            float annualProduction = industrialEntity.IndustryDB.industryRates[industryJob.IndustryType];
            float industrialMultiplier = GetIndustrialMultiplier(_game, industryJob.ItemGuid, industrialEntity.IndustryDB);
            float maxProduction = annualProduction * (float)(_game.Settings.EconomyCycleTime.TotalDays / 365) * industrialMultiplier;
            double tickProduction = maxProduction * industryJob.PercentToUtilize;

            if (tickProduction <= 0)
            {
                return 0;
            }

            double numberToRefine = Math.Min(Math.Floor(tickProduction / industryJob.BPPerItem), industryJob.NumberOrdered);

            bool refiningIsRestricted = false;

            // Check if we have enough raw materials to perform the job.
            foreach (KeyValuePair<CargoDefinition, float> materialRequirement in materialRequirements)
            {
                double materialCarried;
                if (!industrialEntity.CargoDB.cargoCarried.TryGetValue(materialRequirement.Key, out materialCarried))
                {
                    materialCarried = 0;
                }

                double haveMaterialsFor = Math.Floor(materialCarried / materialRequirement.Value);
                if (haveMaterialsFor < numberToRefine)
                {
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

                    if (haveMaterialsFor < numberToRefine)
                    {
                        // Still not enough materials.
                        numberToRefine = haveMaterialsFor;
                        refiningIsRestricted = true;
                    }
                }
            }

            if (refiningIsRestricted)
            {
                var notEnoughVespeneGas = new Event(_game.CurrentDateTime, "Material shortage in production.", EventType.MaterialShortage, null, industrialEntity.Entity);
                _game.EventLog.AddEvent(notEnoughVespeneGas);
            }

            // Check if we have free cargospace to store the output.
            CargoDefinition outputCargoDef = CargoHelper.GetCargoDefinition(_game, industryJob.ItemGuid);
            double freeSpace = CargoHelper.GetFreeCargoSpace(industrialEntity.CargoDB, outputCargoDef.Type);
            double haveSpaceFor = Math.Floor(freeSpace / outputCargoDef.Weight);

            if (haveSpaceFor < Math.Ceiling(numberToRefine))
            {
                var notEnoughVespeneGas = new Event(_game.CurrentDateTime, "Production halted due to lack of cargo space.", EventType.CargoFull, null, industrialEntity.Entity);
                _game.EventLog.AddEvent(notEnoughVespeneGas);
            }

            if (numberToRefine == 0)
            {
                return 0;
            }

            // Remove the required materials from the entity.
            foreach (KeyValuePair<CargoDefinition, float> materialRequirement in materialRequirements)
            {

                industrialEntity.CargoDB.cargoCarried[materialRequirement.Key] -= materialRequirement.Value * numberToRefine;

                if (industrialEntity.CargoDB.cargoCarried[materialRequirement.Key] < 0)
                {
                    industrialEntity.MatedToDB.Parent.GetDataBlob<CargoDB>().cargoCarried[materialRequirement.Key] += industrialEntity.CargoDB.cargoCarried[materialRequirement.Key];
                    industrialEntity.CargoDB.cargoCarried[materialRequirement.Key] = 0;
                }

            }

            // Apply partial construction
            int numberCompleted = (int)Math.Floor(numberToRefine);

            double leftOver = numberToRefine - numberCompleted;
            industryJob.BPToNextCompletion += industryJob.BPPerItem - (float)(industryJob.BPPerItem * leftOver);

            if (industryJob.BPToNextCompletion >= industryJob.BPPerItem)
            {
                industryJob.BPToNextCompletion -= industryJob.BPPerItem;
                numberCompleted++;
            }

            // Update the job
            industryJob.NumberCompleted += numberCompleted;

            if (industryJob.NumberCompleted == industryJob.NumberOrdered)
            {
                var completedEvent = new Event(_game.CurrentDateTime, "Production job completed.", EventType.ProductionCompleted, null, industrialEntity.Entity);
                _game.EventLog.AddEvent(completedEvent);
            }

            // Add completed products
            industrialEntity.CargoDB.cargoCarried.SafeValueAdd(outputCargoDef, numberCompleted);
            
            // Return the percentage of produciton used for this job.
            return maxProduction / (float)(numberToRefine * industryJob.BPPerItem);
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
    }
}

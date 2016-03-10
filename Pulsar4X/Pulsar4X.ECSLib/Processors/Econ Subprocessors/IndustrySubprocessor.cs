using System;
using System.Collections.Generic;
using System.Linq;

namespace Pulsar4X.ECSLib
{
    public class IndustrySubprocessor
    {
        /// <summary>
        /// Helper class that pulls the DB's from the EntityManager for multiple uses.
        /// </summary>
        private class IndustryEntity
        {
            internal readonly Entity Entity;
            internal readonly IndustryDB IndustryDB;
            internal readonly MatedToDB MatedToDB;
            internal readonly CargoDB CargoDB;

            public IndustryEntity(Entity entity)
            {
                Entity = entity;
                IndustryDB = entity.GetDataBlob<IndustryDB>();
                MatedToDB = entity.GetDataBlob<MatedToDB>();
                CargoDB = entity.GetDataBlob<CargoDB>();
            }
        }

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
                IndustryDB industryDB = UpdateIndustryDB(entity);

                // ProcessTerraforming(entity, industryDB);
                // JP stabilization
                // Research
                // Salvage
                _miningSubprocessor.ProcessMining(entity, industryDB);
                ProcessJobs(entity, industryDB, IndustryType.RefiningRate);
                ProcessJobs(entity, industryDB, IndustryType.ComponentConstruction);
                ProcessJobs(entity, industryDB, IndustryType.OrdnanceConstruction);
                ProcessJobs(entity, industryDB, IndustryType.FighterConstruction);
                ProcessJobs(entity, industryDB, IndustryType.ShipConstruction);
                ProcessJobs(entity, industryDB, IndustryType.InstallationConstruction);
            }
        }

        public static IndustryDB UpdateIndustryDB(Entity entity)
        {
            var industryDB = entity.GetDataBlob<IndustryDB>();
            industryDB.industryRates = GetIndustrialRates(entity);

            return industryDB;
        }

        public static Dictionary<IndustryType, float> GetIndustrialRates(Entity entity)
        {
            var components = entity.GetDataBlob<ComponentInstancesDB>();

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

            // TODO: Apply tech/faction/planet/leader bonuses
             
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

        private float ProcessJob(IndustryEntity indEntity, Dictionary<CargoDefinition, float> materialRequirements, IndustryJob industryJob)
        {
            float annualProduction = indEntity.IndustryDB.industryRates[industryJob.IndustryType];
            float industrialMultiplier = GetIndustrialMultiplier(_game, industryJob.ItemGuid, indEntity.IndustryDB);
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
                if (!indEntity.CargoDB.cargoCarried.TryGetValue(materialRequirement.Key, out materialCarried))
                {
                    materialCarried = 0;
                }

                double haveMaterialsFor = Math.Floor(materialCarried / materialRequirement.Value);
                if (haveMaterialsFor < numberToRefine)
                {
                    // Not enough materials. Check if we can pull from our host.
                    if (indEntity.IndustryDB.CanPullFromHost)
                    {
                        Entity parent = indEntity.MatedToDB?.Parent;
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
                var notEnoughVespeneGas = new Event(_game.CurrentDateTime, "Material shortage in production.", EventType.MaterialShortage, null, indEntity.Entity);
                _game.EventLog.AddEvent(notEnoughVespeneGas);
            }

            // Check if we have free cargospace to store the output.
            CargoDefinition outputCargoDef = CargoHelper.GetCargoDefinition(_game, industryJob.ItemGuid);
            float freeSpace = CargoHelper.GetFreeCargoSpace(indEntity.CargoDB, outputCargoDef.Type);
            double haveSpaceFor = Math.Floor(freeSpace / outputCargoDef.Weight);

            if (haveSpaceFor < Math.Ceiling(numberToRefine))
            {
                var notEnoughVespeneGas = new Event(_game.CurrentDateTime, "Production halted due to lack of cargo space.", EventType.CargoFull, null, indEntity.Entity);
                _game.EventLog.AddEvent(notEnoughVespeneGas);
            }

            if (numberToRefine == 0)
            {
                return 0;
            }

            // Remove the required materials from the entity.
            foreach (KeyValuePair<CargoDefinition, float> materialRequirement in materialRequirements)
            {

                indEntity.CargoDB.cargoCarried[materialRequirement.Key] -= materialRequirement.Value * numberToRefine;

                if (indEntity.CargoDB.cargoCarried[materialRequirement.Key] < 0)
                {
                    indEntity.MatedToDB.Parent.GetDataBlob<CargoDB>().cargoCarried[materialRequirement.Key] += indEntity.CargoDB.cargoCarried[materialRequirement.Key];
                    indEntity.CargoDB.cargoCarried[materialRequirement.Key] = 0;
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
                var completedEvent = new Event(_game.CurrentDateTime, "Production job completed.", EventType.ProductionCompleted, null, indEntity.Entity);
                _game.EventLog.AddEvent(completedEvent);
            }

            // Add completed products
            indEntity.CargoDB.cargoCarried.SafeValueAdd(outputCargoDef, numberCompleted);
            
            // Return the percentage of produciton used for this job.
            return maxProduction / (float)(numberToRefine * industryJob.BPPerItem);
        }

        private void ProcessJobs(Entity entity, IndustryDB industryDB, IndustryType industryType)
        {
            var indEntity = new IndustryEntity(entity);

            float percentUtilized = 0;

            LinkedList<IndustryJob> industryJobs = industryDB.industryJobs[industryType];
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

                percentUtilized += ProcessJob(indEntity, materialRequirements, industryJob);
                industryJob.PercentToUtilize = jobIndustryToUtilize;

                currentNode = currentNode.Next;
            }
        }
    }
}

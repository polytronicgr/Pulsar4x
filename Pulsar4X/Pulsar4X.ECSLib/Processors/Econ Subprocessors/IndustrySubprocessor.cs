using System;
using System.Collections.Generic;
using System.Linq;
using Pulsar4X.ECSLib.DataBlobs;

namespace Pulsar4X.ECSLib
{
    public class IndustrySubprocessor
    {
        internal static void Process(Game game, StarSystem system)
        {
            List<Entity> industialEntities = system.SystemManager.GetAllEntitiesWithDataBlob<IndustryDB>();

            foreach (Entity entity in industialEntities)
            {
                IndustryDB industryDB = UpdateIndustryDB(entity);

                //ProcessTerraforming(entity, industryDB);
                ProcessMining(entity, industryDB);
                ProcessRefining(entity, industryDB);
                ProcessConstruction(entity, industryDB);
                ProcessResearch(entity, industryDB);
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

                    foreach (KeyValuePair<IndustryType, int> constructionPoint in industryAbilityDB.constructionPoints)
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

        private static void ProcessTerraforming(Entity entity, IndustryDB industryDB)
        {
            throw new NotImplementedException();
        }

        private static void ProcessMining(Entity entity, IndustryDB industryDB)
        {
            Entity mineableEntity = entity.GetDataBlob<MatedToDB>().Parent;
            var parentSystemBodyDB = mineableEntity?.GetDataBlob<SystemBodyDB>();
            var entityCargoDB = entity.GetDataBlob<CargoDB>();
            var remainingCapacity = entityCargoDB.HasUnlimitedCapacity ? double.MaxValue : entityCargoDB.cargoCapacity[CargoType.General];

            foreach (KeyValuePair<Guid, float> carriedCargo in entityCargoDB.cargoCarried)
            {
                
            }

            if (parentSystemBodyDB == null)
            {
                // Entity not mated to an entity that can be mined.
                return;
            }

            foreach (KeyValuePair<Guid, MineralDepositInfo> mineralDepositInfo in parentSystemBodyDB.Minerals)
            {
                var depositInfo = mineralDepositInfo.Value;



            }

            throw new NotImplementedException();
        }

        private static void ProcessRefining(Entity entity, IndustryDB industryDB)
        {
            throw new NotImplementedException();
        }

        private static void ProcessConstruction(Entity entity, IndustryDB industryDB)
        {
            throw new NotImplementedException();
        }

        private static void ProcessResearch(Entity entity, IndustryDB industryDB)
        {
            throw new NotImplementedException();
        }
    }
}

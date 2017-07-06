using System;
using System.Collections.Generic;
using System.Linq;

namespace Pulsar4X.ECSLib
{
    internal static class MineProcessor
    {
        internal static void MineResources(Entity colonyEntity)
        {
            Dictionary<Guid, int> mineRates = colonyEntity.GetDataBlob<ColonyMinesDB>().MineingRate;
            Dictionary<Guid,MineralDepositInfo> planetMinerals = colonyEntity.GetDataBlob<ColonyInfoDB>().PlanetEntity.GetDataBlob<SystemBodyInfoDB>().Minerals;
            //Dictionary<Guid, int> colonyMineralStockpile = colonyEntity.GetDataBlob<ColonyInfoDB>().MineralStockpile;
            CargoStorageDB stockpile = colonyEntity.GetDataBlob<CargoStorageDB>();
            float mineBonuses = 1;//colonyEntity.GetDataBlob<ColonyBonusesDB>().GetBonus(AbilityType.Mine);
            foreach (var kvp in mineRates)
            {
                var typeID = stockpile.StaticData.GetICargoable(kvp.Key).CargoTypeID;
                double accessability = planetMinerals[kvp.Key].Accessibility;
                double actualRate = kvp.Value * mineBonuses * accessability;
                int mineralsMined = (int)Math.Min(actualRate, planetMinerals[kvp.Key].Amount);
                long capacity = stockpile.StorageByType[typeID].Capacity;
                if (capacity > 0)
                {
                    //colonyMineralStockpile.SafeValueAdd<Guid>(kvp.Key, mineralsMined);
                    CargoStorageHelpers.AddItemToCargo(stockpile, kvp.Key, (uint)mineralsMined);
                    MineralDepositInfo mineralDeposit = planetMinerals[kvp.Key];
                    int newAmount = mineralDeposit.Amount -= mineralsMined;

                    accessability = Math.Pow((float)mineralDeposit.Amount / mineralDeposit.HalfOriginalAmount, 3) * mineralDeposit.Accessibility;
                    double newAccess = GMath.Clamp(accessability, 0.1, mineralDeposit.Accessibility);

                    mineralDeposit.Amount = newAmount;
                    mineralDeposit.Accessibility = newAccess;
                }
            }
        }

        /// <summary>
        /// Called by the ReCalcProcessor.
        /// </summary>
        /// <param name="colonyEntity"></param>
        internal static void CalcMaxRate(Entity colonyEntity)
        {

            Dictionary<Guid,int> rates = new Dictionary<Guid, int>();
            var instancesDB = colonyEntity.GetDataBlob<ComponentInstancesDB>();
            List<KeyValuePair<Entity, PrIwObsList<Entity>>> mineEntities = instancesDB.SpecificInstances.GetInternalDictionary().Where(item => item.Key.HasDataBlob<MineResourcesAtbDB>()).ToList();
            foreach (var mineComponentDesignList in mineEntities)
            {
                foreach (var mineInstance in mineComponentDesignList.Value)
                {
                    //todo check if it's damaged, check if it's enabled, check if there's enough workers here to.
                    foreach (var item in mineComponentDesignList.Key.GetDataBlob<MineResourcesAtbDB>().ResourcesPerEconTick)
                    {
                        rates.SafeValueAdd(item.Key, item.Value);
                    }                    
                }
            }
            colonyEntity.GetDataBlob<ColonyMinesDB>().MineingRate = rates;
        }
    }
}
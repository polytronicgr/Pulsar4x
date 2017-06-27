﻿using System;
using System.Collections.Generic;

namespace Pulsar4X.ECSLib
{
    //This processor takes an entity and recalcs for each of the datablobs in that entity.
    //as an ability is added to the game, it's recalc processor should be linked here.
    internal static class ReCalcProcessor
    {
        [ThreadStatic]
        private static Entity CurrentEntity;
        internal static Dictionary<Type, Delegate> TypeProcessorMap = new Dictionary<Type, Delegate>
            {
                { typeof(ShipInfoDB), new Action<ShipInfoDB>(processor => {ShipAndColonyInfoProcessor.ReCalculateShipTonnaageAndHTK(CurrentEntity); }) },
                { typeof(ColonyMinesDB), new Action<ColonyMinesDB>(processor => { MineProcessor.CalcMaxRate(CurrentEntity);}) },
                { typeof(PropulsionDB), new Action<PropulsionDB>(processor => { PropulsionCalcs.CalcMaxSpeed(CurrentEntity); }) },
                { typeof(ColonyRefiningDB), new Action<ColonyRefiningDB>(processor => { RefiningProcessor.ReCalcRefiningRate(CurrentEntity); }) },
                { typeof(ColonyConstructionDB), new Action<ColonyConstructionDB>(processor => { ConstructionProcessor.ReCalcConstructionRate(CurrentEntity); }) },
                { typeof(ColonyLifeSupportDB), new Action<ColonyLifeSupportDB>(processor => {PopulationProcessor.ReCalcMaxPopulation(CurrentEntity); }) },
                { typeof(BeamWeaponsDB), new Action<BeamWeaponsDB>(processor => {WeaponProcessor.RecalcBeamWeapons(CurrentEntity); }) },
                { typeof(CargoStorageDB), new Action<CargoStorageDB>(processor => {CargoStorageHelpers.ReCalcCapacity(CurrentEntity); }) },

            };

        

        internal static void ReCalcAbilities(Entity entity)
        {
             
            //lock (CurrentEntity) 
            //{
                CurrentEntity = entity;
                foreach (var datablob in entity.DataBlobs)
                {
                    var t = datablob.GetType();
                    if (TypeProcessorMap.ContainsKey(t))
                        TypeProcessorMap[t].DynamicInvoke(datablob); // invoke appropriate delegate  
                }                
            //}
        }
    }



}
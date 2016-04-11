using System;
using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public enum CargoType
    {
        None = 0,
        General, // Used for general freight. Trade Goods, Minerals, Refined Minerals, etc.
        Troops,
        Ordnance,
        Colonists,
        Fuel,
    }
    
    public class CargoDefinition
    {
        public Guid ItemGuid { get; internal set; }
        public CargoType Type { get; internal set; }
        public float Weight { get; internal set; }
        public IndustryType IndustryType { get; internal set;}
        public CargoDefinition(Entity entity)
        {
            Type = CargoType.General;
            if (entity.HasDataBlob<ComponentDB>())
            {
                var componentInfo = entity.GetDataBlob<ComponentDB>();
                Weight = componentInfo.SizeInTons * 1000;
                IndustryType = IndustryType.ComponentConstruction;
            }
            //TODO check if entity is figher, Installation, Ship etc.
                  
            
        }
        public CargoDefinition(MineralSD mineral)
        {
            ItemGuid = mineral.ID;
            Weight = mineral.Weight;
            Type = mineral.CargoType;
            IndustryType = IndustryType.Mining;
        }
        public CargoDefinition(RefinedMaterialSD material)
        {
            ItemGuid = material.ID;
            Weight = material.Weight;
            Type = material.CargoType;
            IndustryType = IndustryType.Refining;
        }       
    }

    public static class CargoHelper
    {
        public static double GetFreeCargoSpace(CargoDB cargoDB, CargoType cargoType)
        {
            if (cargoDB.HasUnlimitedCapacity)
            {
                return float.MaxValue;
            }

            double freeSpace = cargoDB.cargoCapacity[cargoType];

            foreach (KeyValuePair<CargoDefinition, double> carriedCargo in cargoDB.cargoCarried)
            {
                var cargoDef = carriedCargo.Key;

                if (cargoDef.Type == cargoType)
                {
                    freeSpace -= carriedCargo.Value;
                }
            }

            return freeSpace;
        }

        public static CargoDefinition GetCargoDefinition(Game game, Guid cargoGuid)
        {
            CargoDefinition cargoDef;
            if (!TryGetSDCargoDefinition(game, cargoGuid, out cargoDef))
            {
                if (!TryGetComponentCargoDefintion(game, cargoGuid, out cargoDef))
                {
                    throw new GuidNotFoundException(cargoGuid);
                }
            }

            return cargoDef;
        }

        private static bool TryGetComponentCargoDefintion(Game game, Guid cargoGuid, out CargoDefinition cargoDef)
        {
            Entity entity;
            cargoDef = null;
            if (!game.GlobalManager.FindEntityByGuid(cargoGuid, out entity))
            {
                return false;
            }

            // Cargo is a component.
            cargoDef = new CargoDefinition(entity);
            return false;
        }

        private static bool TryGetSDCargoDefinition(Game game, Guid cargoGuid, out CargoDefinition cargoDef)
        {
            object cargo = game.StaticData.FindDataObjectUsingID(cargoGuid);
            cargoDef = null;
            
            if (cargo == null)
            {
                return false;
            }

            dynamic cargoDynamic = cargo;

            cargoDef = cargoDynamic.Cargo;
            return true;
        }
    }
}

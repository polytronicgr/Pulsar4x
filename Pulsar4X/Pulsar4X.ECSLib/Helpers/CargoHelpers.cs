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
        public CargoDefinition(Game game, Entity entity)
        {
            Type = CargoType.General;
            if (entity.HasDataBlob<ComponentDB>())
            {
                var componentInfo = entity.GetDataBlob<ComponentDB>();
                Weight = componentInfo.SizeInTons * 1000;
                IndustryType = IndustryType.ComponentConstruction;
            }
            //TODO check if entity is figher, Installation, Ship etc, and add the industry type.
            game.CargoDefinitions.Add(ItemGuid, this);

        }
        public CargoDefinition(Game game, MineralSD mineral)
        {
            ItemGuid = mineral.ID;
            Weight = mineral.Weight;
            Type = mineral.CargoType;
            IndustryType = IndustryType.Mining;
            game.CargoDefinitions.Add(ItemGuid, this);
        }
        public CargoDefinition(Game game, RefinedMaterialSD material)
        {
            ItemGuid = material.ID;
            Weight = material.Weight;
            Type = material.CargoType;
            IndustryType = IndustryType.Refining;
            game.CargoDefinitions.Add(ItemGuid, this);
        }       
    }

    public static class CargoHelper
    {
        public static double GetFreeCargoSpace(Game game, CargoDB cargoDB, CargoType cargoType)
        {
            if (cargoDB.HasUnlimitedCapacity)
            {
                return float.MaxValue;
            }

            double freeSpace = cargoDB.cargoCapacity[cargoType];

            foreach (KeyValuePair<Guid, double> carriedCargo in cargoDB.cargoCarried)
            {
                CargoDefinition cargoDef = GetCargoDefinition(game, cargoDB, carriedCargo.Key);

                if (cargoDef.Type == cargoType)
                {
                    freeSpace -= carriedCargo.Value;
                }
            }

            return freeSpace;
        }

        /// <summary>
        /// returns the cargo definition for a cargoDB and itemDef
        /// will throw a key not found exception if the cargoDB does not contain the item.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cargo"></param>
        /// <param name="id">guid of the item</param>
        /// <returns></returns>
        public static CargoDefinition GetCargoDefinition(Game game, CargoDB cargo, Guid id)
        {
            return game.CargoDefinitions[id];
        }

        public static Dictionary<CargoDefinition, double> GetMineralCargoDefs(Game game, CargoDB cargo)
        {
            foreach (var item in cargo.cargoCarried)
            {
                if(game.StaticData.Minerals.Contains)
            }
        }

        /// <summary>
        /// returns the cargo def or creates a new one if the exsisting one does not exsist in the global dictionary. 
        /// will throw an exception if it can't find the cargoableItemID in the game.CargoDefinitions or if it can't find the object as an entity or a mineral or refined material. 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cargoableItemID"></param>
        /// <returns></returns>
        internal static CargoDefinition FindOrCreateCargoDefinition(Game game, Guid cargoableItemID)
        {
            CargoDefinition cargoDef;
            if (!game.CargoDefinitions.ContainsKey(cargoableItemID))
            {
                if (TryGetComponentCargoDefintion(game, cargoableItemID, out cargoDef)) { }
                else if(TryGetSDCargoDefinition(game, cargoableItemID, out cargoDef)) { }
                else
                    throw new GuidNotFoundException(cargoableItemID);
            }
            else
                cargoDef = game.CargoDefinitions[cargoableItemID];

            return cargoDef;
        }

        //public static CargoDefinition GetCargoDefinition(Game game, Guid cargoGuid)
        //{
        //    CargoDefinition cargoDef;
        //    if (!TryGetSDCargoDefinition(game, cargoGuid, out cargoDef))
        //    {
        //        if (!TryGetComponentCargoDefintion(game, cargoGuid, out cargoDef))
        //        {
        //            throw new GuidNotFoundException(cargoGuid);
        //        }
        //    }

        //    return cargoDef;
        //}

        private static bool TryGetComponentCargoDefintion(Game game, Guid cargoGuid, out CargoDefinition cargoDef)
        {
            Entity entity;
            cargoDef = null;
            if (!game.GlobalManager.FindEntityByGuid(cargoGuid, out entity))
            {
                return false;
            }

            // Cargo is a component.
            cargoDef = new CargoDefinition(game, entity);
            return false;
        }

        /// <summary>
        /// TODO: This can be removed if cargoDefs are created on static data load. 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cargoGuid"></param>
        /// <param name="cargoDef"></param>
        /// <returns></returns>
        private static bool TryGetSDCargoDefinition(Game game, Guid cargoGuid, out CargoDefinition cargoDef)
        {
            object cargo = game.StaticData.FindDataObjectUsingID(cargoGuid);
            cargoDef = null;

            if (cargo is MineralSD)
                cargoDef = new CargoDefinition(game, (MineralSD)cargo);
            else if (cargo is RefinedMaterialSD)
                cargoDef = new CargoDefinition(game, (RefinedMaterialSD)cargo);
            else
                return false;
            return true;
        }
    }
}

#region Copyright/License
/* 
 *Copyright© 2017 Daniel Phelps
    This file is part of Pulsar4x.

    Pulsar4x is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pulsar4x is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;


namespace Pulsar4X.ECSLib
{

    public static class StorageSpaceProcessor
    {
        /// <summary>
        /// returns the amount of items for a given item guid.
        /// </summary>
        /// <param name="fromCargo"></param>
        /// <param name="itemID">a min or mat ID</param>
        /// <returns></returns>
        public static long GetAmountOf(CargoStorageDB fromCargo, Guid itemID)
        {
            Guid cargoTypeID = fromCargo.ItemToTypeMap[itemID];
            ICargoable cargo = fromCargo.OwningEntity.Manager.Game.StaticData.GetICargoable(itemID);
            long returnValue = 0;
            if (fromCargo.MinsAndMatsByCargoType.ContainsKey(cargoTypeID))
            {
                if (fromCargo.MinsAndMatsByCargoType[cargoTypeID].ContainsKey(cargo))
                {
                    returnValue = fromCargo.MinsAndMatsByCargoType[cargoTypeID][cargo];
                }
            }
            return returnValue;
        }

        /// <summary>
        /// a list of entities stored of a given cargotype - does not remove entites. 
        /// </summary>
        /// <param name="fromCargo"></param>
        /// <param name="typeID">cargo type guid</param>
        /// <returns>new list of Entites or an empty list</returns>
        public static List<Entity> GetEntitesOfCargoType(CargoStorageDB fromCargo, Guid typeID)
        {
            var entityList = new List<Entity>();
            if (fromCargo.StoredEntities.ContainsKey(typeID))
            {
                foreach (var kvp in fromCargo.StoredEntities[typeID])
                {
                    entityList.AddRange(kvp.Value);
                }
            }
            return entityList;
        }

        /// <summary>
        /// checks if a given cargoDB has a specific entity.
        /// </summary>
        /// <param name="cargo"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool HasSpecificEntity(CargoStorageDB cargo, Entity entity)
        {
            Entity designEntity = entity.GetDataBlob<DesignInfoDB>().DesignEntity;
            Guid cargoType = entity.GetDataBlob<CargoAbleTypeDB>().CargoTypeID;
            if (cargo.StoredEntities.ContainsKey(cargoType))
                if (cargo.StoredEntities[cargoType].ContainsKey(designEntity))
                    if (cargo.StoredEntities[cargoType][designEntity].Contains(entity))
                        return true;
            return false;
        }

        /// <summary>
        /// Removes the given entity from a given cargo storage. 
        /// </summary>
        /// <param name="cargo"></param>
        /// <param name="entity"></param>
        /// <returns>true if successfull</returns>
        public static bool RemoveSpecificEntity(CargoStorageDB cargo, Entity entity)
        {
            Entity designEntity = entity.GetDataBlob<DesignInfoDB>().DesignEntity;
            Guid cargoType = entity.GetDataBlob<CargoAbleTypeDB>().CargoTypeID;
            if (cargo.StoredEntities.ContainsKey(cargoType))
                if (cargo.StoredEntities[cargoType].ContainsKey(designEntity))
                    return cargo.StoredEntities[cargoType][designEntity].Remove(entity);
            return false;
        }

        /// <summary>
        /// a Dictionary of resources stored of a given cargotype
        /// </summary>
        /// <param name="typeID">cargo type guid</param>
        /// <returns>new dictionary of resources or an empty dictionary</returns>
        public static Dictionary<ICargoable, long> GetResourcesOfCargoType(CargoStorageDB fromCargo, Guid typeID)
        {
            if (fromCargo.MinsAndMatsByCargoType.ContainsKey(typeID))
                return new Dictionary<ICargoable, long>(fromCargo.MinsAndMatsByCargoType[typeID]);
            return new Dictionary<ICargoable, long>();
        }

        /// <summary>
        /// Adds a value to the dictionary, if the item does not exsist, it will get added to the dictionary.
        /// </summary>
        /// <param name="item">the guid of the item to add</param>
        /// <param name="value">the amount of the item to add</param>
        private static void AddValue(CargoStorageDB toCargo, ICargoable item, long value)
        {
            Guid cargoTypeID = toCargo.ItemToTypeMap[item.ID];
            if (!toCargo.MinsAndMatsByCargoType.ContainsKey(cargoTypeID))
            {
                toCargo.MinsAndMatsByCargoType.Add(cargoTypeID, new ObservableDictionary<ICargoable, long>());
            }
            if (!toCargo.MinsAndMatsByCargoType[cargoTypeID].ContainsKey(item))
            {
                toCargo.MinsAndMatsByCargoType[cargoTypeID].Add(item, value);                                                        
            }
            else
                toCargo.MinsAndMatsByCargoType[cargoTypeID][item] += value;
        }

        internal static void AddItemToCargo(CargoStorageDB toCargo, Guid itemID, long amount)
        {
            ICargoable item = (ICargoable)toCargo.OwningEntity.Manager.Game.StaticData.FindDataObjectUsingID(itemID);
            long remainingWeightCapacity = RemainingCapacity(toCargo, item.CargoTypeID);
            long remainingNumCapacity = (long)(remainingWeightCapacity / item.Mass);
            float amountWeight = amount / item.Mass;
            if (remainingNumCapacity >= amount)
                AddValue(toCargo, item, amount);
            else
                AddValue(toCargo, item, remainingNumCapacity);
        }

        /// <summary>
        /// Checks storage capacity and stores either the amount or the amount that toCargo is capable of storing.
        /// </summary>
        /// <param name="toCargo"></param>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        internal static void AddItemToCargo(CargoStorageDB toCargo, ICargoable item, int amount)
        {
            long remainingWeightCapacity = RemainingCapacity(toCargo, item.CargoTypeID);
            int remainingNumCapacity = (int)(remainingWeightCapacity / item.Mass);
            float amountWeight = amount * item.Mass;
            if (remainingNumCapacity >= amount)
                AddValue(toCargo, item, amount);
            else
                AddValue(toCargo, item, remainingNumCapacity);
        }

        /// <summary>
        /// checks the toCargo and stores the item if there is enough space.
        /// </summary>
        /// <param name="toCargo"></param>
        /// <param name="entity"></param>
        /// <param name=""></param>
        internal static void AddItemToCargo(CargoStorageDB toCargo, Entity entity)
        {
            Entity designEntity = entity.GetDataBlob<DesignInfoDB>().DesignEntity;
            ICargoable cargoTypeDB = designEntity.GetDataBlob<CargoAbleTypeDB>();
            float amountWeight = cargoTypeDB.Mass;
            long remainingWeightCapacity = RemainingCapacity(toCargo, cargoTypeDB.CargoTypeID);
            int remainingNumCapacity = (int)(remainingWeightCapacity / amountWeight);

            if (remainingNumCapacity >= 1)
                AddToCargo(toCargo, entity, cargoTypeDB);
        }

        /// <summary>
        /// Will remove the item from the dictionary if subtracting the value causes the dictionary value to be 0.
        /// </summary>
        /// <param name="fromCargo"></param>
        /// <param name="itemID">the guid of the item to subtract</param>
        /// <param name="value">the amount of the item to subtract</param>
        /// <returns>the amount succesfully taken from the dictionary(will not remove more than what the dictionary contains)</returns>
        internal static long SubtractValue(CargoStorageDB fromCargo, Guid itemID, long value)
        {
            Guid cargoTypeID = fromCargo.ItemToTypeMap[itemID];
            ICargoable cargoItem = fromCargo.OwningEntity.Manager.Game.StaticData.GetICargoable(itemID);
            long returnValue = 0;
            if (fromCargo.MinsAndMatsByCargoType.ContainsKey(cargoTypeID))
                if (fromCargo.MinsAndMatsByCargoType[cargoTypeID].ContainsKey(cargoItem))
                {
                    if (fromCargo.MinsAndMatsByCargoType[cargoTypeID][cargoItem] >= value)
                    {
                        fromCargo.MinsAndMatsByCargoType[cargoTypeID][cargoItem] -= value;
                        returnValue = value;
                    }
                    else
                    {
                        returnValue = fromCargo.MinsAndMatsByCargoType[cargoTypeID][cargoItem];
                        fromCargo.MinsAndMatsByCargoType[cargoTypeID].Remove(cargoItem);
                    }
                }
            return returnValue;
        }

        /// <summary>
        /// Checks storage capacity and transferes either the amount or the amount that toCargo is capable of storing.
        /// </summary>
        /// <param name="fromCargo"></param>
        /// <param name="toCargo"></param>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        internal static void TransferCargo(CargoStorageDB fromCargo, CargoStorageDB toCargo, ICargoable item, int amount)
        {
            Guid cargoTypeID = item.CargoTypeID;
            float itemWeight = item.Mass;
            Guid itemID = item.ID;

            long remainingWeightCapacity = RemainingCapacity(toCargo, cargoTypeID);
            long remainingNumCapacity = (long)(remainingWeightCapacity / itemWeight);
            float amountWeight = amount * itemWeight;
            if (remainingNumCapacity >= amount)
            {
                //AddToCargo(toCargo, item, amount);
                //fromCargo.MinsAndMatsByCargoType[cargoTypeID][itemID] -= amount;
                long amountRemoved = SubtractValue(fromCargo, itemID, amount);
                AddValue(toCargo, item, amountRemoved);


            }
            else
            {
                //AddToCargo(toCargo, item, remainingNumCapacity);
                //fromCargo.MinsAndMatsByCargoType[cargoTypeID][itemID] -= remainingNumCapacity;
                long amountRemoved = SubtractValue(fromCargo, itemID, remainingNumCapacity);
                AddValue(toCargo, item, amountRemoved);
            }
        }

        /// <summary>
        /// must be mins or mats
        /// </summary>
        /// <param name="fromCargo"></param>
        /// <param name="amounts">must be mins or mats</param>
        internal static void RemoveResources(CargoStorageDB fromCargo, Dictionary<Guid, int> amounts)
        {
            foreach (var item in amounts)
            {
                SubtractValue(fromCargo, item.Key, item.Value);
            }
        }

        /// <summary>
        /// checks the toCargo and transferes the item if there is enough space.
        /// </summary>
        /// <param name="fromCargo"></param>
        /// <param name="toCargo"></param>
        /// <param name="entityItem"></param>
        internal static void TransferEntity(CargoStorageDB fromCargo, CargoStorageDB toCargo, Entity entityItem)
        {
            CargoAbleTypeDB cargotypedb = entityItem.GetDataBlob<CargoAbleTypeDB>();
            Guid cargoTypeID = cargotypedb.CargoTypeID;
            float itemWeight = cargotypedb.Mass;
            Guid itemID = cargotypedb.ID;

            long remainingWeightCapacity = RemainingCapacity(toCargo, cargoTypeID);
            long remainingNumCapacity = (long)(remainingWeightCapacity / itemWeight);
            if (remainingNumCapacity >= 1)
            {
                if (fromCargo.StoredEntities[cargoTypeID].Remove(entityItem))
                    AddToCargo(toCargo, entityItem, cargotypedb);
            }
        }

        private static void AddToCargo(CargoStorageDB toCargo, Entity entityItem, ICargoable cargotypedb)
        {
            if (!entityItem.HasDataBlob<ComponentInstanceInfoDB>())
                new Exception("entityItem does not contain ComponentInstanceInfoDB, it must be an componentInstance type entity");
            Entity design = entityItem.GetDataBlob<ComponentInstanceInfoDB>().DesignEntity;
            if (!toCargo.StoredEntities.ContainsKey(cargotypedb.CargoTypeID))
                toCargo.StoredEntities.Add(cargotypedb.CargoTypeID, new ObservableDictionary<Entity, ObservableCollection<Entity>>());
            if (!toCargo.StoredEntities[cargotypedb.CargoTypeID].ContainsKey(design))
                toCargo.StoredEntities[cargotypedb.CargoTypeID].Add(design, new ObservableCollection<Entity>());
            toCargo.StoredEntities[cargotypedb.CargoTypeID][design].Add(entityItem);
        }




        public static long RemainingCapacity(CargoStorageDB cargo, Guid typeID)
        {
            long capacity = cargo.CargoCapacity[typeID];
            long storedWeight = NetWeight(cargo, typeID);
            return capacity - storedWeight;
        }

        public static long NetWeight(CargoStorageDB cargo, Guid typeID)
        {            
            long net = 0;
            if (cargo.MinsAndMatsByCargoType.ContainsKey(typeID))
                net = StoredWeight(cargo.MinsAndMatsByCargoType, typeID);
            else if (cargo.StoredEntities.ContainsKey(typeID))
                net = StoredWeight(cargo.StoredEntities, typeID);
            return net;
        }

        private static long StoredWeight(ObservableDictionary<Guid, ObservableDictionary<ICargoable, long>> dict, Guid typeID)
    {
            long storedWeight = 0;
            foreach (var amount in dict[typeID].Values.ToArray())
            {
                storedWeight += amount;
            }
            return storedWeight;
        }

        private static long StoredWeight(ObservableDictionary<Guid, ObservableDictionary<Entity, ObservableCollection<Entity>>> dict, Guid typeID)
        {
            double storedWeight = 0;
            foreach (var itemType in dict[typeID])
            {
                foreach (var designInstanceKVP in itemType.Value)
                {
                    storedWeight += designInstanceKVP.GetDataBlob<MassVolumeDB>().Mass;
                }

            }
            return (int)Math.Round(storedWeight, MidpointRounding.AwayFromZero);
        }

        public static bool HasReqiredItems(CargoStorageDB stockpile, Dictionary<Guid, int> costs)
        {
            if (costs == null)
                return true;
            else
            {
                foreach (var costitem in costs)
                {
                    if (costitem.Value >= GetAmountOf(stockpile, costitem.Key))
                        return false;
                }
            }
            return true;
        }

        internal static void ReCalcCapacity(Entity parentEntity)
        {
            CargoStorageDB storageDB = parentEntity.GetDataBlob<CargoStorageDB>();
            ObservableDictionary<Guid, long> totalSpace = storageDB.CargoCapacity;

            List<KeyValuePair<Entity, ObservableCollection<Entity>>> StorageComponents = parentEntity.GetDataBlob<ComponentInstancesDB>().SpecificInstances.Where(item => item.Key.HasDataBlob<CargoStorageAtbDB>()).ToList();
            foreach (var kvp in StorageComponents)
            {
                Entity componentDesign = kvp.Key;
                Guid cargoTypeID = componentDesign.GetDataBlob<CargoStorageAtbDB>().CargoTypeGuid;
                long alowableSpace = 0;
                foreach (Entity specificComponent in kvp.Value)
                {
                    var healthPercent = specificComponent.GetDataBlob<ComponentInstanceInfoDB>().HealthPercent();
                    if (healthPercent > 0.75)
                        alowableSpace = componentDesign.GetDataBlob<CargoStorageAtbDB>().StorageCapacity;
                }
                if (!totalSpace.ContainsKey(cargoTypeID))
                {
                    totalSpace.Add(cargoTypeID, alowableSpace);
                    if (!storageDB.MinsAndMatsByCargoType.ContainsKey(cargoTypeID))
                        storageDB.MinsAndMatsByCargoType.Add(cargoTypeID, new ObservableDictionary<ICargoable, long>());                                       
                }
                else if (totalSpace[cargoTypeID] != alowableSpace)
                {
                    totalSpace[cargoTypeID] = alowableSpace;
                    if (RemainingCapacity(storageDB, cargoTypeID) < 0)
                    { //todo: we've lost cargo capacity, and we're carrying more than we have storage for, drop random cargo
                    }
                }
            }
        }
    }

    public class CargoOrderProcessor : IActionableProcessor
    {

        public void ProcessAction(DateTime toDate, BaseAction action)
        {
            ProcessAction(toDate, (CargoAction)action);
        }
        
        private void ProcessAction(DateTime toDate, CargoAction action)
        {
            action.Status = "In Progress ";
            TimeSpan deltaTime = toDate - action.ThisStorage.LastRunDate;
            

            CargoStorageDB cargoFrom = action.CargoFrom;
            CargoStorageDB cargoTo = action.CargoTo;
            MessagePump messagePump = action.ThisEntity.Manager.Game.MessagePump;
            
            double tonsThisDeltaT = action.ThisStorage.OrderTransferRate * deltaTime.TotalSeconds / 3600;
            tonsThisDeltaT += action.ThisStorage.PartAmount;
            action.ThisStorage.PartAmount = tonsThisDeltaT - Math.Floor(tonsThisDeltaT);
            int amountThisMove = Math.Max((int)tonsThisDeltaT, 0);
            action.ThisStorage.AmountToTransfer -= amountThisMove;

            StorageSpaceProcessor.TransferCargo(cargoFrom, cargoTo, action.ThisStorage.OrderTransferItem, amountThisMove);

            if (action.ThisStorage.AmountToTransfer == 0)
            {
                //action.ThisStorage.PercentComplete.Percent = 1.0f;
                action.IsFinished = true;

            }
            else
            {
                if (action.ThisEntity.Manager.ManagerSubpulses.SystemLocalDateTime >= action.EstTimeComplete)
                {
                    OrderProcessor.SetNextInterupt(EstDateTime(action, action.ThisStorage), action);
                }
            }
        }

        /// <summary>
        /// Sets an Entity interupt at the datetime the cargo transfer should complete.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cargoStorageDB"></param>
        internal static DateTime EstDateTime(CargoAction action, CargoStorageDB cargoStorageDB)
        {
            cargoStorageDB.OrderTransferRate = (int)(action.CargoFrom.TransferRate + action.CargoTo.TransferRate * 0.5);
            TimeSpan timeToComplete = TimeSpan.FromHours((float)cargoStorageDB.AmountToTransfer / cargoStorageDB.OrderTransferRate);
            return action.ThisEntity.Manager.ManagerSubpulses.SystemLocalDateTime + timeToComplete;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using Pulsar4X.ECSLib.DataSubscription;


namespace Pulsar4X.ECSLib
{

    public static class CargoStorageHelpers
    {        
        internal static void ReCalcCapacity(Entity parentEntity)
        {
            CargoStorageDB storageDB = parentEntity.GetDataBlob<CargoStorageDB>();
            

            List<KeyValuePair<Entity, PrIwObsList<Entity>>> StorageComponents = parentEntity.GetDataBlob<ComponentInstancesDB>().SpecificInstances.GetInternalDictionary().Where(item => item.Key.HasDataBlob<CargoStorageAtbDB>()).ToList();
            foreach (var kvp in StorageComponents)
            {
                Entity componentDesign = kvp.Key;
                Guid cargoTypeID = componentDesign.GetDataBlob<CargoStorageAtbDB>().CargoTypeGuid;
                long alowableCapacity = 0;
                foreach (Entity specificComponent in kvp.Value) //get the total allowed capacity
                {
                    var healthPercent = specificComponent.GetDataBlob<ComponentInstanceInfoDB>().HealthPercent();
                    if (healthPercent > 0.75)
                        alowableCapacity = componentDesign.GetDataBlob<CargoStorageAtbDB>().StorageCapacity;
                }
                if (!storageDB.StorageByType.ContainsKey(cargoTypeID))
                {
                    //storage doesn't have the capability to store this type at all yet, so add it. 
                    storageDB.StorageByType.Add(cargoTypeID, new CargoStorageTypeData()
                    {
                        Capacity = alowableCapacity,
                        FreeCapacity = alowableCapacity
                        
                    });                                           
                }
                else if (storageDB.StorageByType[cargoTypeID].Capacity != alowableCapacity)
                {
                    long difference = alowableCapacity - storageDB.StorageByType[cargoTypeID].Capacity;
                    long freeCapacity = storageDB.StorageByType[cargoTypeID].FreeCapacity;
                    storageDB.StorageByType[cargoTypeID].Capacity = alowableCapacity;
                    if (storageDB.HasSubscribers)
                    {
                        CargoDataChange change = new CargoDataChange()
                        {
                            ChangeType = CargoDataChange.CargoChangeTypes.CapacityChange,
                            Amount = (uint)alowableCapacity,
                            TypGuid = cargoTypeID
                        };
                        storageDB.Changes.Add(change);
                    }
                    
                    storageDB.StorageByType[cargoTypeID].FreeCapacity += difference;
                    if (freeCapacity < freeCapacity + difference)
                    {                        
                        //todo: we've lost cargo capacity, and we're carrying more than we have storage for, drop random cargo
                    }
                }
            }
        }
        

        internal static void AddItemToCargo(CargoStorageDB toCargo, Guid cargoableItemID, uint amount)
        {
            ICargoable cargoableItem = toCargo.StaticData.GetICargoable(cargoableItemID);
            AddItemToCargo(toCargo, cargoableItem, amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toCargo"></param>
        /// <param name="cargoableItem"></param>
        /// <param name="amount"></param>
        internal static void AddItemToCargo(CargoStorageDB toCargo, ICargoable cargoableItem, uint amount)
        {
            float massPerItem = cargoableItem.Mass;

            Guid cargoTypeID = cargoableItem.CargoTypeID;
            
            uint amountToMove = Math.Min(AmountMovable(toCargo, cargoableItem), amount);
            long massToMove = (long)massPerItem * amountToMove;

            
            toCargo.StorageByType[cargoTypeID].FreeCapacity -= massToMove;
            if(!toCargo.StorageByType[cargoTypeID].StoredByItemID.ContainsKey(cargoableItem.ID))
                toCargo.StorageByType[cargoTypeID].StoredByItemID.Add(cargoableItem.ID, 0);
            toCargo.StorageByType[cargoTypeID].StoredByItemID[cargoableItem.ID] += amountToMove;
            if (toCargo.HasSubscribers)
            {
                CargoDataChange change = new CargoDataChange()
                {
                    ChangeType = CargoDataChange.CargoChangeTypes.AddToCargo,
                    Amount = amountToMove,
                    ItemID = cargoableItem.ID,
                    TypGuid = cargoTypeID
                };
                toCargo.Changes.Add(change);
            }

            if (cargoableItem is CargoAbleTypeDB)
            {
                CargoAbleTypeDB db = (CargoAbleTypeDB)cargoableItem;
                toCargo.StorageByType[cargoTypeID].StoredEntities.Add(db.OwningEntity.Guid, db.OwningEntity);
            }

        }


        internal static void RemoveItemFromCargo(CargoStorageDB fromCargo, Guid cargoableItemID, uint amount)
        {
            ICargoable cargoableItem = fromCargo.StaticData.GetICargoable(cargoableItemID);
            RemoveItemFromCargo(fromCargo, cargoableItem, amount);
        }

        internal static void RemoveItemFromCargo(CargoStorageDB fromCargo, ICargoable cargoableItem, uint amount)
        {
            float massPerItem = cargoableItem.Mass;
            Guid cargoTypeID = cargoableItem.CargoTypeID;

            uint amountInStore = fromCargo.StorageByType[cargoTypeID].StoredByItemID[cargoableItem.ID];

            uint amountToMove = Math.Min(amountInStore, amount);
            long massToMove = (long)(massPerItem * amountToMove);
            
            

            fromCargo.StorageByType[cargoTypeID].FreeCapacity += massToMove;
            fromCargo.StorageByType[cargoTypeID].StoredByItemID[cargoableItem.ID] -= amountToMove;
            if (fromCargo.HasSubscribers)
            {
                CargoDataChange change = new CargoDataChange()
                {
                    ChangeType = CargoDataChange.CargoChangeTypes.RemoveFromCargo,
                    Amount = amountToMove,
                    ItemID = cargoableItem.ID,
                    TypGuid = cargoTypeID
                };
                fromCargo.Changes.Add(change);
            }
        }

        internal static uint AmountMovable(CargoStorageDB cargoTo, ICargoable item)
        {
            float massPerItem = item.Mass;
            long freeCapacity = cargoTo.StorageByType[item.CargoTypeID].FreeCapacity;           
            return (uint)(freeCapacity / massPerItem);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromCargo"></param>
        /// <param name="toCargo"></param>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        internal static void TransferCargo(CargoStorageDB fromCargo, CargoStorageDB toCargo, ICargoable item, uint amount)
        {
            uint maxAddable = AmountMovable(toCargo, item);
            uint maxremovable = fromCargo.StorageByType[item.CargoTypeID].StoredByItemID[item.ID];
            uint amountToMove = Math.Min(maxremovable, amount);
            amountToMove = Math.Min(amountToMove, maxAddable);
            RemoveItemFromCargo(fromCargo, item, amountToMove);
            AddItemToCargo(toCargo, item, amountToMove);
        }

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
            if (fromCargo.StorageByType.ContainsKey(cargoTypeID))
            {
                if (fromCargo.StorageByType[cargoTypeID].StoredByItemID.ContainsKey(cargo.ID))
                {
                    returnValue = fromCargo.StorageByType[cargoTypeID].StoredByItemID[cargo.ID];
                }
            }
            return returnValue;
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
            if (cargo.StorageByType[cargoType].StoredEntities.ContainsKey(cargoType))
                return true;
            return false;
        }
        
        public static bool HasReqiredItems(CargoStorageDB stockpile, Dictionary<Guid, int> costs)
        {
            if (costs != null)
            {
                foreach (var costitem in costs)
                {
                    if (costitem.Value >= GetAmountOf(stockpile, costitem.Key))
                        return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Will remove the item from the store if subtracting the value causes the dictionary value to be 0.
        /// </summary>
        /// <param name="fromCargo"></param>
        /// <param name="itemID">the guid of the item to subtract</param>
        /// <param name="value">the amount of the item to subtract</param>
        /// <returns>the amount succesfully taken from the store(will not remove more than what the store contains)</returns>
        internal static long SubtractValue(CargoStorageDB fromCargo, Guid itemID, uint value)
        {
            Guid cargoTypeID = fromCargo.ItemToTypeMap[itemID];
            ICargoable cargoItem = fromCargo.StaticData.GetICargoable(itemID);
            long amountRemoved = 0;
            if (fromCargo.StorageByType.ContainsKey(cargoTypeID))
                if (fromCargo.StorageByType[cargoTypeID].StoredByItemID.ContainsKey(itemID))
                {
                    if (fromCargo.StorageByType[cargoTypeID].StoredByItemID[itemID] > value)
                    {
                        fromCargo.StorageByType[cargoTypeID].StoredByItemID[itemID] -= value;
                        amountRemoved = value;
                    }
                    else
                    {
                        amountRemoved = fromCargo.StorageByType[cargoTypeID].StoredByItemID[itemID];
                        fromCargo.StorageByType[cargoTypeID].StoredByItemID.Remove(itemID);
                    }


                    fromCargo.StorageByType[cargoTypeID].FreeCapacity += amountRemoved;
                    if (fromCargo.HasSubscribers)
                    {
                        CargoDataChange change = new CargoDataChange()
                        {
                            ChangeType = CargoDataChange.CargoChangeTypes.RemoveFromCargo,
                            Amount = (uint)amountRemoved,
                            ItemID = itemID,
                            TypGuid = cargoTypeID
                        };
                        fromCargo.Changes.Add(change);
                    }
                }            
            return amountRemoved;
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
                SubtractValue(fromCargo, item.Key, (uint)item.Value);
            }
        }
    }

    public class CargoActionProcessor : IActionableProcessor
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
            MessagePumpServer messagePump = action.ThisEntity.Manager.Game.MessagePump;
            
            double tonsThisDeltaT = action.ThisStorage.OrderTransferRate * deltaTime.TotalSeconds / 3600;
            tonsThisDeltaT += action.ThisStorage.PartAmount;
            action.ThisStorage.PartAmount = tonsThisDeltaT - Math.Floor(tonsThisDeltaT);
            int amountThisMove = Math.Max((int)tonsThisDeltaT, 0);
            action.ThisStorage.AmountToTransfer -= amountThisMove;

            CargoStorageHelpers.TransferCargo(cargoFrom, cargoTo, action.ThisStorage.OrderTransferItem, (uint)amountThisMove);

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

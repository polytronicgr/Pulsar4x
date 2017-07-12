using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib
{

    /// <summary>
    /// Contains info on a ships cargo capicity.
        /// Needs to store: Total Capacities for each CargoTypeSD.ID that the parent entity is capable of storing.
    /// Items Stored: this is both static data and entities.
    /// entities need to reference specific items or it'd be possible to put damaged components into cargo and take them out new.  
    /// Either capacity used or capacity remaining. 
    /// </summary>
    public class CargoStorageDB : SubscribableDatablob
    {
        
        //public bool HasSubscribers { get; set; }
        //public List<DatablobChange> Changes { get; } = new List<DatablobChange>();

        public Dictionary<Guid, CargoStorageTypeData> StorageByType { get; private set; } = new Dictionary<Guid, CargoStorageTypeData>();

        /// <summary>
        /// in tones per hour?
        /// </summary>
        [JsonProperty]
        public int TransferRate { get; internal set; } = 10;

        [JsonIgnore] //don't store this in the savegame, we'll re-reference this OnDeserialised
        internal Dictionary<Guid, Guid> ItemToTypeMap;

        [JsonIgnore] //don't store this in the savegame, we'll re-reference this OnDeserialised
        internal StaticDataStore StaticData;
        

        [OnDeserialized]
        private void Deserialized(StreamingContext context)
        {            
            var game = (Game)context.Context;
            ItemToTypeMap = game.StaticData.StorageTypeMap;
            StaticData = game.StaticData;            
        }

        public CargoStorageDB()
        {
        }

        public CargoStorageDB(StaticDataStore staticDataStore)
        {
            StaticData = staticDataStore;
            ItemToTypeMap = staticDataStore.StorageTypeMap;
        }

        public CargoStorageDB(CargoStorageDB cargoDB)
        {
            //CargoCapicities = new PrIwObsDict<Guid, long>(cargoDB.CargoCapicities);
            //MinsAndMatsByCargoType = new PrIwObsDict<Guid, PrIwObsDict<Guid, long>>(cargoDB.MinsAndMatsByCargoType);
            //StoredEntities = new PrIwObsDict<Guid, PrIwObsDict<Entity, PrIwObsList<Entity>>>(cargoDB.StoredEntities);

            StorageByType = new Dictionary<Guid, CargoStorageTypeData>(cargoDB.StorageByType);        
            
            ItemToTypeMap = cargoDB.ItemToTypeMap; //note that this is not 'new', the dictionary referenced here is static and should be the same dictionary throughout the game.
        
            AmountToTransfer = cargoDB.AmountToTransfer;
            PartAmount = cargoDB.PartAmount;
            OrderTransferRate = cargoDB.OrderTransferRate;
            LastRunDate = cargoDB.LastRunDate;
            StaticData = cargoDB.StaticData;

        }



        /// <summary>
        /// gives the cargoType of a given itemID
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public CargoTypeSD CargoType(Guid itemID)
        {
            return StaticData.CargoTypes[ItemToTypeMap[itemID]];
        }

        public Guid CargoTypeID(Guid itemID)
        {
            return ItemToTypeMap[itemID];
        }

        public override object Clone()
        {
            return new CargoStorageDB(this);
        }
        
        [JsonProperty]
        internal CargoAction CurrentAction { get; set; }

        [JsonProperty]
        public int AmountToTransfer { get; internal set; }
        [JsonProperty]
        public double PartAmount { get; internal set; }
        [JsonProperty]
        public int OrderTransferRate { get; internal set; } //an average of the transfer rates of the two entites.

        [JsonProperty] private Guid _orderTranferItemGuid;
        
        internal Guid OrderTransferItemGuid {
            get { return _orderTranferItemGuid; }
            set
            {
                _orderTranferItemGuid = value;  
                OrderTransferItem = StaticData.GetICargoable(value);
            }
        }

        [JsonIgnore]
        internal ICargoable OrderTransferItem { get; set; }

        [JsonProperty]
        public DateTime LastRunDate { get; internal set; }
    }

    public class CargoStorageTypeData
    {
        /// <summary>
        /// by Weight
        /// </summary>
        public long Capacity { get; set; } = 0;

        /// <summary>
        /// By Weight
        /// </summary>
        public long FreeCapacity { get; set; } = 0;
        /// <summary>
        /// Key is ICargoable.ID
        /// </summary>
        public Dictionary<Guid, uint> StoredByItemID { get; } = new Dictionary<Guid, uint>();
        public Dictionary<Guid, Entity> StoredEntities { get; } = new Dictionary<Guid, Entity>();
    }



    public class CargoDataChange : DatablobChange
    {
        public enum CargoChangeTypes
        {
            AddToCargo,
            RemoveFromCargo,
            CapacityChange,
            TransferRateChange
        }
        public CargoChangeTypes ChangeType;
        public Guid TypGuid;
        public Guid ItemID;      
        public uint Amount;
        
    }
    

}
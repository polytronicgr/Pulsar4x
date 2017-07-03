using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Pulsar4X.ECSLib
{

    /// <summary>
    /// Contains info on a ships cargo capicity.
    /// Needs to store: Total Capacities for each CargoTypeSD.ID that the parent entity is capable of storing.
    /// Items Stored: this is both static data and entities.
    /// entities need to reference specific items or it'd be possible to put damaged components into cargo and take them out new.  
    /// Either capacity used or capacity remaining. 
    /// </summary>
    public class CargoStorageDB : BaseDataBlob
    {
        [JsonProperty]
        public PrIwObsDict<Guid, long> CargoCapicities { get; private set; } = new PrIwObsDict<Guid, long>();
        
        //[JsonProperty]
        //public Dictionary<Guid, long> UsedCapicities { get; private set; } = new Dictionary<Guid,long>();

        [JsonProperty]
        public PrIwObsDict<Guid, PrIwObsDict<Entity, PrIwObsList<Entity>>> StoredEntities { get; private set; } = new PrIwObsDict<Guid, PrIwObsDict<Entity, PrIwObsList<Entity>>>();
        
        /// <summary>
        /// Key is CargoTypeSD.ID inner key is ICargoable.ID
        /// </summary>
        [JsonProperty] 
        public PrIwObsDict<Guid, PrIwObsDict<Guid, long>> MinsAndMatsByCargoType { get; private set;} = new PrIwObsDict<Guid, PrIwObsDict<Guid, long>>();

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
            CargoCapicities = new PrIwObsDict<Guid, long>(cargoDB.CargoCapicities);
            MinsAndMatsByCargoType = new PrIwObsDict<Guid, PrIwObsDict<Guid, long>>(cargoDB.MinsAndMatsByCargoType);
            StoredEntities = new PrIwObsDict<Guid, PrIwObsDict<Entity, PrIwObsList<Entity>>>(cargoDB.StoredEntities);
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
}
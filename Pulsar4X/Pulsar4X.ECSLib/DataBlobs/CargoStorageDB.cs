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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Pulsar4X.ECSLib
{

    /// <summary>
    /// Contains info on a ships cargo capicity.
    /// </summary>
    public class CargoStorageDB : BaseDataBlob
    {
        [JsonProperty]
        public ObservableDictionary<Guid, long> CargoCapicity { get; private set; } = new ObservableDictionary<Guid, long>();

        [JsonProperty]
        public ObservableDictionary<Guid, ObservableDictionary<Entity, ObservableCollection<Entity>>> StoredEntities { get; private set; } = new ObservableDictionary<Guid, ObservableDictionary<Entity, ObservableCollection<Entity>>>();
        
        [JsonProperty] //TODO maybe change the ICargoable key to a GUID.
        public ObservableDictionary<Guid, ObservableDictionary<ICargoable, long>> MinsAndMatsByCargoType { get; private set;}= new ObservableDictionary<Guid, ObservableDictionary<ICargoable, long>>();

        /// <summary>
        /// in tones per hour?
        /// </summary>
        [JsonProperty]
        public int TransferRate { get; internal set; } = 10;

        [JsonIgnore] //don't store this in the savegame, we'll re-reference this OnDeserialised
        internal Dictionary<Guid, Guid> ItemToTypeMap;

        [JsonIgnore] //don't store this in the savegame, we'll re-reference this OnDeserialised
        private StaticDataStore _staticData;
        

        [OnDeserialized]
        private void Deserialized(StreamingContext context)
        {            
            var game = (Game)context.Context;
            ItemToTypeMap = game.StaticData.StorageTypeMap;
            _staticData = game.StaticData;            
        }

        public CargoStorageDB()
        {
            CargoCapicity.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(CargoCapicity), args);
            StoredEntities.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(StoredEntities), args);
            MinsAndMatsByCargoType.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(MinsAndMatsByCargoType), args);
        }

        public CargoStorageDB(StaticDataStore staticDataStore) : this()
        {
            _staticData = staticDataStore;
            ItemToTypeMap = staticDataStore.StorageTypeMap;
        }

        public CargoStorageDB(CargoStorageDB cargoDB) : this()
        {
            CargoCapicity.Merge(cargoDB.CargoCapicity);
            MinsAndMatsByCargoType.Merge(cargoDB.MinsAndMatsByCargoType);
            StoredEntities.Merge(cargoDB.StoredEntities);
            ItemToTypeMap = cargoDB.ItemToTypeMap; //note that this is not 'new', the dictionary referenced here is static and should be the same dictionary throughout the game.
        
            AmountToTransfer = cargoDB.AmountToTransfer;
            PartAmount = cargoDB.PartAmount;
            OrderTransferRate = cargoDB.OrderTransferRate;
            LastRunDate = cargoDB.LastRunDate;
            _staticData = cargoDB._staticData;

        }



        /// <summary>
        /// gives the cargoType of a given itemID
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public CargoTypeSD CargoType(Guid itemID)
        {
            return _staticData.CargoTypes[ItemToTypeMap[itemID]];
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
                OrderTransferItem = _staticData.GetICargoable(value);
            }
        }

        [JsonIgnore]
        internal ICargoable OrderTransferItem { get; set; }

        [JsonProperty]
        public DateTime LastRunDate { get; internal set; }
    }
}
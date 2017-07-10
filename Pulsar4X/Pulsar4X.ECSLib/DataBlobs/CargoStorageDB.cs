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
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Contains info on a ships cargo capicity.
    /// </summary>
    public class CargoStorageDB : BaseDataBlob
    {
        #region Fields
        private int _amountToTransfer;
        private ObservableDictionary<Guid, long> _cargoCapacity;
        private CargoAction _currentAction;
        private DateTime _lastRunDate;
        private ObservableDictionary<Guid, ObservableDictionary<ICargoable, long>> _minsAndMatsByCargoType;

        [JsonProperty]
        private Guid _orderTranferItemGuid;

        private ICargoable _orderTransferItem;
        private int _orderTransferRate;
        private double _partAmount;

        [JsonIgnore] //don't store this in the savegame, we'll re-reference this OnDeserialised
        private StaticDataStore _staticData;

        private ObservableDictionary<Guid, ObservableDictionary<Entity, ObservableCollection<Entity>>> _storedEntities;
        private int _transferRate = 10;

        [JsonIgnore] //don't store this in the savegame, we'll re-reference this OnDeserialised
        public Dictionary<Guid, Guid> ItemToTypeMap;
        #endregion

        #region Properties
        [JsonProperty]
        public ObservableDictionary<Guid, long> CargoCapacity
        {
            get { return _cargoCapacity; }
            set
            {
                SetField(ref _cargoCapacity, value);
                CargoCapacity.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(CargoCapacity), args);
            }
        }

        [JsonProperty]
        public ObservableDictionary<Guid, ObservableDictionary<Entity, ObservableCollection<Entity>>> StoredEntities
        {
            get { return _storedEntities; }
            set
            {
                SetField(ref _storedEntities, value);
                StoredEntities.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(StoredEntities), args);
            }
        }

        [JsonProperty] //TODO maybe change the ICargoable key to a GUID.
        public ObservableDictionary<Guid, ObservableDictionary<ICargoable, long>> MinsAndMatsByCargoType
        {
            get { return _minsAndMatsByCargoType; }
            set
            {
                _minsAndMatsByCargoType = value;
                MinsAndMatsByCargoType.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(MinsAndMatsByCargoType), args);
            }
        }

        /// <summary>
        /// in tones per hour?
        /// </summary>
        [JsonProperty]
        public int TransferRate { get { return _transferRate; } set { SetField(ref _transferRate, value); } }

        [JsonProperty]
        public CargoAction CurrentAction { get { return _currentAction; } set { SetField(ref _currentAction, value); } }

        [JsonProperty]
        public int AmountToTransfer { get { return _amountToTransfer; } set { SetField(ref _amountToTransfer, value); } }

        [JsonProperty]
        public double PartAmount { get { return _partAmount; } set { SetField(ref _partAmount, value); } }

        [JsonProperty]
        public int OrderTransferRate { get { return _orderTransferRate; } set { SetField(ref _orderTransferRate, value); } } //an average of the transfer rates of the two entites.

        public Guid OrderTransferItemGuid
        {
            get { return _orderTranferItemGuid; }
            set
            {
                SetField(ref _orderTranferItemGuid, value);
                OrderTransferItem = _staticData.GetICargoable(value);
            }
        }

        [JsonIgnore]
        public ICargoable OrderTransferItem { get { return _orderTransferItem; } set { SetField(ref _orderTransferItem, value); } }

        [JsonProperty]
        public DateTime LastRunDate { get { return _lastRunDate; } set { SetField(ref _lastRunDate, value); } }
        #endregion

        #region Constructors
        public CargoStorageDB()
        {
            CargoCapacity = new ObservableDictionary<Guid, long>();
            StoredEntities = new ObservableDictionary<Guid, ObservableDictionary<Entity, ObservableCollection<Entity>>>();
            MinsAndMatsByCargoType = new ObservableDictionary<Guid, ObservableDictionary<ICargoable, long>>();
            ;
        }

        public CargoStorageDB(StaticDataStore staticDataStore) : this()
        {
            _staticData = staticDataStore;
            ItemToTypeMap = staticDataStore.StorageTypeMap;
        }

        public CargoStorageDB(CargoStorageDB cargoDB) : this()
        {
            CargoCapacity.Merge(cargoDB.CargoCapacity);
            MinsAndMatsByCargoType.Merge(cargoDB.MinsAndMatsByCargoType);
            StoredEntities.Merge(cargoDB.StoredEntities);
            ItemToTypeMap = cargoDB.ItemToTypeMap; //note that this is not 'new', the dictionary referenced here is static and should be the same dictionary throughout the game.

            AmountToTransfer = cargoDB.AmountToTransfer;
            PartAmount = cargoDB.PartAmount;
            OrderTransferRate = cargoDB.OrderTransferRate;
            LastRunDate = cargoDB.LastRunDate;
            _staticData = cargoDB._staticData;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new CargoStorageDB(this);
        #endregion

        #region Public Methods
        /// <summary>
        /// gives the cargoType of a given itemID
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public CargoTypeSD CargoType(Guid itemID) => _staticData.CargoTypes[ItemToTypeMap[itemID]];

        public Guid CargoTypeID(Guid itemID) => ItemToTypeMap[itemID];
        #endregion

        #region Private Methods
        [OnDeserialized]
        private void Deserialized(StreamingContext context)
        {
            var game = (Game)context.Context;
            ItemToTypeMap = game.StaticData.StorageTypeMap;
            _staticData = game.StaticData;
        }
        #endregion
    }
}
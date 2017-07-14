#region Copyright/License
// Copyright© 2017 Daniel Phelps
//     This file is part of Pulsar4x.
// 
//     Pulsar4x is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Pulsar4x is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pulsar4X.ECSLib;

namespace Pulsar4X.ViewModel
{
    public class CargoStorageVM : ViewModelBase
    {
        #region Fields
        private CargoStorageDB _storageDB;
        private StaticDataStore _dataStore;
        private readonly GameVM _gameVM;
        #endregion

        #region Properties
        public ObservableCollection<CargoStorageByTypeVM> CargoStore { get; } = new ObservableCollection<CargoStorageByTypeVM>();
        #endregion

        #region Constructors
        public CargoStorageVM(GameVM gameVM)
        {
            _gameVM = gameVM;
            _dataStore = _gameVM.Game.StaticData;
        }
        #endregion

        #region Public Methods
        public void Initialise(Entity entity)
        {
            _storageDB = entity.GetDataBlob<CargoStorageDB>();
            foreach (KeyValuePair<Guid, long> item in _storageDB.CargoCapacity)
            {
                var storeType = new CargoStorageByTypeVM(_gameVM);
                storeType.Initalise(_storageDB, item.Key);
                CargoStore.Add(storeType);
            }
            _storageDB.CargoCapacity.CollectionChanged += _storageDB_CollectionChanged;
            _storageDB.StoredEntities.CollectionChanged += StoredEntities_CollectionChanged;
        }
        #endregion

        #region EventHandlers
        private void StoredEntities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }

        private void _storageDB_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var storeType = new CargoStorageByTypeVM(_gameVM);
                var kvp = (KeyValuePair<Guid, object>)e.NewItems[0];
                storeType.Initalise(_storageDB, kvp.Key);
                CargoStore.Add(storeType);
            }
        }
        #endregion
    }

    public class CargoStorageByTypeVM : INotifyPropertyChanged
    {
        #region Fields
        private CargoStorageDB _storageDB;
        private readonly StaticDataStore _dataStore;
        private readonly GameVM _gameVM;
        private string _typeName;
        #endregion

        #region Properties
        public Guid TypeID { get; private set; }
        public string TypeName { get; set; }
        public long MaxWeight => _storageDB?.CargoCapacity[TypeID] ?? 0;
        public float NetWeight => StorageSpaceProcessor.NetWeight(_storageDB, TypeID);
        public float RemainingWeight => StorageSpaceProcessor.RemainingCapacity(_storageDB, TypeID);

        public string HeaderText
        {
            get => _typeName;
            set
            {
                _typeName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CargoItemVM> TypeStore { get; } = new ObservableCollection<CargoItemVM>();
        public ObservableCollection<ComponentSpecificDesignVM> DesignStore { get; } = new ObservableCollection<ComponentSpecificDesignVM>();

        public bool HasComponents
        {
            get
            {
                if (DesignStore.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Constructors
        public CargoStorageByTypeVM(GameVM gameVM)
        {
            _gameVM = gameVM;
            _dataStore = _gameVM.Game.StaticData;
        }
        #endregion

        #region Public Methods
        public void Initalise(CargoStorageDB storageDB, Guid storageTypeID)
        {
            _storageDB = storageDB;
            TypeID = storageTypeID;

            CargoTypeSD cargoType = _dataStore.CargoTypes[TypeID];
            TypeName = cargoType.Name;
            foreach (KeyValuePair<ICargoable, long> itemKVP in StorageSpaceProcessor.GetResourcesOfCargoType(storageDB, TypeID))
            {
                var cargoItem = new CargoItemVM(_gameVM, _storageDB, itemKVP.Key);
                TypeStore.Add(cargoItem);
            }
            if (_storageDB.StoredEntities.ContainsKey(TypeID))
            {
                InitEntities();
            }

            HeaderText = cargoType.Name + ": " + NetWeight + " of " + MaxWeight + " used, " + RemainingWeight + " remaining";
            _storageDB.OwningEntity.Manager.ManagerSubpulses.SystemDateChangedEvent += ManagerSubpulses_SystemDateChangedEvent;
            _storageDB.MinsAndMatsByCargoType[TypeID].CollectionChanged += _storageDB_CollectionChanged;
            _storageDB.StoredEntities.CollectionChanged += StoredEntities_CollectionChanged;
        }
        #endregion

        #region Other Members
        private void InitEntities()
        {
            foreach (KeyValuePair<Entity, ObservableCollection<Entity>> item in _storageDB.StoredEntities[TypeID])
            {
                var design = new ComponentSpecificDesignVM(item.Key, item.Value);
                DesignStore.Add(design);
            }
            _storageDB.StoredEntities[TypeID].CollectionChanged += CargoStorageByTypeVM_CollectionChanged;
            OnPropertyChanged(nameof(HasComponents));
        }

        private void OnItemAdded(KeyValuePair<ICargoable, long> newItem)
        {
            if (newItem.Key.CargoTypeID == TypeID)
            {
                var cargoItem = new CargoItemVM(_gameVM, _storageDB, newItem.Key);
                TypeStore.Add(cargoItem);
            }
        }

        private void OnItemRemoved(ICargoable removedItem)
        { //is there a better way to do this?
            foreach (CargoItemVM item in TypeStore.ToArray())
            {
                if (item.ItemID == removedItem.ID)
                {
                    TypeStore.Remove(item);
                    break;
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        #endregion

        #region EventHandlers
        private void StoredEntities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (KeyValuePair<Guid, ObservableDictionary<Entity, ObservableCollection<Entity>>> newitem in e.NewItems)
                {
                    if (TypeID == newitem.Key)
                    {
                        InitEntities();
                    }
                }
            }
        }

        private void CargoStorageByTypeVM_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    var kvp = (KeyValuePair<Entity, ObservableCollection<Entity>>)item;
                    var design = new ComponentSpecificDesignVM(kvp.Key, kvp.Value);
                    DesignStore.Add(design);
                }
            }
            else
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (object item in e.OldItems)
                    {
                        var key = (Entity)item;
                        foreach (ComponentSpecificDesignVM vmitem in DesignStore.ToArray())
                        {
                            if (vmitem.EntityID == key.Guid)
                            {
                                DesignStore.Remove(vmitem);
                                break;
                            }
                        }
                    }
                }
            }


            OnPropertyChanged(nameof(HasComponents));
        }

        private void _storageDB_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    OnItemAdded((KeyValuePair<ICargoable, long>)item);
                }
            }
            else
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (object item in e.OldItems)
                    {
                        OnItemRemoved((ICargoable)item);
                    }
                }
            }
        }

        private void ManagerSubpulses_SystemDateChangedEvent(DateTime newDate)
        {
            HeaderText = TypeName + ": " + NetWeight + " of " + MaxWeight + " used, " + RemainingWeight + " remaining";
            OnPropertyChanged(nameof(MaxWeight));
            OnPropertyChanged(nameof(NetWeight));
            OnPropertyChanged(nameof(RemainingWeight));
            //OnPropertyChanged(nameof(TypeStore));
        }
        #endregion

        #region Interfaces
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class CargoItemVM : INotifyPropertyChanged
    {
        #region Fields
        private readonly CargoStorageDB _storageDB;
        #endregion

        #region Properties
        internal Guid ItemID { get; }
        public string ItemName { get; set; }
        public string ItemTypeName { get; set; }
        public long Amount => StorageSpaceProcessor.GetAmountOf(_storageDB, ItemID);
        public float ItemWeight { get; set; }
        public float TotalWeight => ItemWeight * Amount;
        #endregion

        #region Constructors
        public CargoItemVM(GameVM gameVM, CargoStorageDB storageDB, ICargoable item)
        {
            ItemID = item.ID;
            ItemName = item.Name;
            _storageDB = storageDB;
            ItemWeight = item.Mass;
            _storageDB.OwningEntity.Manager.ManagerSubpulses.SystemDateChangedEvent += ManagerSubpulses_SystemDateChangedEvent;
            if (item is MineralSD)
            {
                ItemTypeName = "Raw Mineral";
            }
            else
            {
                if (item is ProcessedMaterialSD)
                {
                    ItemTypeName = "Processed Material";
                }
                else
                {
                    if (item is CargoAbleTypeDB)
                    {
                        var itemdb = (CargoAbleTypeDB)item;
                        Entity itemEntity = itemdb.OwningEntity;
                        ItemTypeName = itemEntity.GetDataBlob<ComponentInstanceInfoDB>()?.DesignEntity.GetDataBlob<NameDB>().GetName(itemEntity.GetDataBlob<OwnedDB>().ObjectOwner) ?? "Unknown Construct Type";
                    }
                }
            }
        }
        #endregion

        #region Other Members
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        #endregion

        #region EventHandlers
        private void ManagerSubpulses_SystemDateChangedEvent(DateTime newDate) { OnPropertyChanged(nameof(Amount)); }
        #endregion

        #region Interfaces
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulsar4X.ECSLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ViewModel
{
    public class CargoStorageVM : ViewModelBase, IHandleMessage
    {
        private CargoStorageDB _cargoData;
        private StaticDataStore _dataStore;
        private GameVM _gameVM;
        private Guid _entityGuid;
        public  ObservableCollection<CargoStorageByTypeVM> CargoStore { get; } = new ObservableCollection<CargoStorageByTypeVM>();
        private Dictionary<Guid, CargoStorageByTypeVM> CargoDictionary { get; }= new Dictionary<Guid, CargoStorageByTypeVM>();
        public CargoStorageVM(GameVM gameVM)
        {
            _gameVM = gameVM;
            _dataStore = _gameVM.Game.StaticData;
        }
        public void Initialise(Entity entity)
        {
            _entityGuid = entity.Guid;
            SubscriptionRequestMessage<CargoStorageDB> subreq = new SubscriptionRequestMessage<CargoStorageDB>()
            {
                ConnectionID = Guid.Empty, 
                EntityGuid = entity.Guid, 
            };
            _gameVM.IncomingMessageHandler.Subscribe(subreq, this);   
            
        }

        public void Update(BaseToClientMessage message)
        {
            if (message is DatablobChangedMessage)
            {
                var dataMessage = (DatablobChangedMessage)message;
                foreach (CargoDataChange change in dataMessage.Changes.OfType<CargoDataChange>())
                {
                    switch (change.ChangeType)
                    {
                        case CargoDataChange.CargoChangeTypes.AmountChange:
                            AmountChange(change);
                            break;
                        case CargoDataChange.CargoChangeTypes.CapacityChange: break;
                        case CargoDataChange.CargoChangeTypes.TransferRateChange: break;
                    }
                }
            }
            else if(message is DatablobDataMessage<CargoStorageDB>)
            {
                var datamessage = (DatablobDataMessage<CargoStorageDB>)message;
                CargoStorageDB datablob = (CargoStorageDB)datamessage.DataBlob;
                foreach (var kvp in datablob.StorageByType)
                {
                    if (!CargoDictionary.ContainsKey(kvp.Key))
                    {
                        CargoDictionary.Add(kvp.Key, new CargoStorageByTypeVM(_gameVM, kvp.Key, kvp.Value));
                        CargoStore.Add(CargoDictionary[kvp.Key]);
                    }
                }                
            }
        }

        public void AmountChange(CargoDataChange change)
        {
            Guid storageType = change.TypeGuid;

            if (CargoDictionary.ContainsKey(storageType))
                CargoDictionary[storageType].AmountChange(change);
            else
            {
                DataRequestMessage<CargoStorageDB> datarequest = new DataRequestMessage<CargoStorageDB>(_gameVM.ConnectionID, _entityGuid);
                _gameVM.Game.MessagePump.EnqueueIncomingMessage(datarequest);
            }
        }  
    }



    public class CargoStorageByTypeVM : INotifyPropertyChanged
    {
        private CargoStorageTypeData _storageData;
        private StaticDataStore _dataStore;
        public Guid TypeID { get; private set; }
        private GameVM _gameVM;
        public string TypeName { get; set; }
        public long MaxWeight { get { return _storageData?.Capacity ?? 0; } }
        public float NetWeight { get { return MaxWeight - _storageData?.FreeCapacity ?? 0; } }
        public float RemainingWeight { get { return _storageData?.FreeCapacity ?? 0; } }
        private string _typeName;
        public string HeaderText { get { return _typeName; } set { _typeName = value; OnPropertyChanged(); } }
        public ObservableCollection<CargoItemVM> TypeStore { get; } = new ObservableCollection<CargoItemVM>();
        private Dictionary<Guid, CargoItemVM> TypeDictionary { get; } = new Dictionary<Guid, CargoItemVM>();
        public ObservableCollection<ComponentSpecificDesignVM> DesignStore { get; } = new ObservableCollection<ComponentSpecificDesignVM>();
        public bool HasComponents { get { if (DesignStore.Count > 0) return true; else return false; } }
        
        public CargoStorageByTypeVM(GameVM gameVM)
        {
            _gameVM = gameVM;
            _dataStore = _gameVM.Game.StaticData;
        }

        public CargoStorageByTypeVM(GameVM gameVM, Guid typeID, CargoStorageTypeData typeData) : this(gameVM)
        {
            Initalise(typeID, typeData);
        }

        public void Initalise(Guid storageTypeID, CargoStorageTypeData storageData)
        {
            _storageData = storageData;
            TypeID = storageTypeID;

            CargoTypeSD cargoType = _dataStore.CargoTypes[TypeID];
            TypeName = cargoType.Name;
            foreach (var item in _storageData.StoredByItemID)
            {                             
                CargoItemVM cargoItem = new CargoItemVM(_gameVM, _storageData, _dataStore.GetICargoable(item.Key));
                TypeStore.Add(cargoItem);
            }
            if (_storageData.StoredEntities.ContainsKey(TypeID))
            {
                InitEntities();
            }

            HeaderText = cargoType.Name + ": " + NetWeight.ToString() + " of " + MaxWeight.ToString() + " used, " + RemainingWeight.ToString() + " remaining";
            //_storageData.OwningEntity.Manager.ManagerSubpulses.SystemDateChangedEvent += ManagerSubpulses_SystemDateChangedEvent;
            //_storageData.MinsAndMatsByCargoType[TypeID].CollectionChanged += _storageDB_CollectionChanged;
            //_storageData.StoredEntities.CollectionChanged += StoredEntities_CollectionChanged;
        }

        internal void AmountChange(CargoDataChange change)
        {
            if(TypeDictionary.ContainsKey(change.ItemID))
                TypeDictionary[change.ItemID].UpdateAmount(change);
            else
            {
                TypeDictionary.Add(change.ItemID, new CargoItemVM(_gameVM, _storageData, _dataStore.GetICargoable(change.ItemID)));
                TypeStore.Add(TypeDictionary[change.ItemID]);
            }
        }
        
        private void InitEntities()
        {
            foreach (var item in _storageData.StoredEntities)
            {
                //ComponentSpecificDesignVM design = new ComponentSpecificDesignVM(item.Key, item.Value);
                //DesignStore.Add(design);
            }
            //_storageData.StoredEntities[TypeID].CollectionChanged += CargoStorageByTypeVM_CollectionChanged;
            OnPropertyChanged(nameof(HasComponents));
        }

        private void StoredEntities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (KeyValuePair<Guid, PrIwObsDict<Entity, PrIwObsList<Entity>>> newitem in e.NewItems)
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
                foreach (var item in e.NewItems)
                {
                    KeyValuePair<Entity, PrIwObsList<Entity>> kvp = (KeyValuePair<Entity, PrIwObsList<Entity>>)item;
                    ComponentSpecificDesignVM design = new ComponentSpecificDesignVM(kvp.Key, kvp.Value);
                    DesignStore.Add(design);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    Entity key = (Entity)item;
                    foreach (var vmitem in DesignStore.ToArray())
                    {
                        if (vmitem.EntityID == key.Guid)
                        {
                            DesignStore.Remove(vmitem);
                            break;
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
                foreach (var item in e.NewItems)
                {
                    OnItemAdded((KeyValuePair<ICargoable, long>)item);
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {

                foreach (var item in e.OldItems)
                {
                    OnItemRemoved((ICargoable)item);
                }
            }
        }

        private void OnItemAdded(KeyValuePair<ICargoable, long> newItem)
        {
            if (newItem.Key.CargoTypeID == TypeID)
            {
                CargoItemVM cargoItem = new CargoItemVM(_gameVM, _storageData, newItem.Key);
                TypeStore.Add(cargoItem);
            }
        }

        private void OnItemRemoved(ICargoable removedItem)
        { //is there a better way to do this?
            foreach (var item in TypeStore.ToArray())
            {
                if (item.ItemID == removedItem.ID)
                {
                    TypeStore.Remove(item);
                    break;
                }
            }    
        }

        private void ManagerSubpulses_SystemDateChangedEvent(DateTime newDate)
        {
            HeaderText = TypeName + ": " + NetWeight.ToString() + " of " + MaxWeight.ToString() + " used, " + RemainingWeight.ToString() + " remaining";
            OnPropertyChanged(nameof(MaxWeight));
            OnPropertyChanged(nameof(NetWeight));
            OnPropertyChanged(nameof(RemainingWeight));
            //OnPropertyChanged(nameof(TypeStore));
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CargoItemVM : INotifyPropertyChanged
    {
        CargoStorageTypeData _storageData;
        internal Guid ItemID { get; private set; }
        internal Guid TypeID { get; private set; }
        public string ItemName { get; set; }
        public string ItemTypeName { get; set; }
        public long Amount { get { return _storageData.StoredByItemID[ItemID]; } }
        public float ItemWeight { get; set; } = 0;
        public float TotalWeight { get { return (ItemWeight * Amount); } } 

        public CargoItemVM(GameVM gameVM, CargoStorageTypeData storageData, ICargoable item)
        {
            ItemID = item.ID;
            TypeID = item.CargoTypeID;
            ItemName = item.Name;
            _storageData = storageData;
            ItemWeight = item.Mass;
            //_storageData.OwningEntity.Manager.ManagerSubpulses.SystemDateChangedEvent += ManagerSubpulses_SystemDateChangedEvent;
            if (item is MineralSD)
                ItemTypeName = "Raw Mineral";
            else if (item is ProcessedMaterialSD)
                ItemTypeName = "Processed Material";
            else if (item is CargoAbleTypeDB)
            {
                CargoAbleTypeDB itemdb = (CargoAbleTypeDB)item;
                Entity itemEntity = itemdb.OwningEntity;
                ItemTypeName = itemEntity.GetDataBlob<ComponentInstanceInfoDB>()?.
                    DesignEntity.GetDataBlob<NameDB>().GetName(itemEntity.GetDataBlob<OwnedDB>().
                    ObjectOwner) ?? "Unknown Construct Type";
            }
        }

        internal void UpdateAmount(CargoDataChange change)
        {
            _storageData.StoredByItemID[change.ItemID] = change.Amount;
            OnPropertyChanged(nameof(Amount));
        }

        private void ManagerSubpulses_SystemDateChangedEvent(DateTime newDate)
        {
            OnPropertyChanged(nameof(Amount));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

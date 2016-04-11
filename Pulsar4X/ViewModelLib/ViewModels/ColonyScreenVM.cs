using Pulsar4X.ECSLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;


namespace Pulsar4X.ViewModel
{
    public class ColonyScreenVM : IViewModel
    {
        private Entity _colonyEntity;
        private ColonyDB Colony { get { return _colonyEntity.GetDataBlob<ColonyDB>(); } }
        private Entity FactionEntity { get { return _colonyEntity.GetDataBlob<OwnedDB>().ObjectOwner; } }
        private Dictionary<Guid, MineralSD> _mineralDictionary;

        private ObservableCollection<FacilityVM> _facilities;
        public ObservableCollection<FacilityVM> Facilities
        {
            get { return _facilities; }
        }

        private Dictionary<string, long> _species;
        public Dictionary<string, long> Species { get { return _species; } }

        public PlanetMineralDepositVM PlanetMineralDepositVM { get; set; }
        public RawMineralStockpileVM RawMineralStockpileVM { get; set; }
        public RefinedMatsStockpileVM RefinedMatsStockpileVM { get; set; }
        public CargoVM ColonyCargo { get; set; }

        public string ColonyName
        {
            get { return _colonyEntity.GetDataBlob<NameDB>().GetName(FactionEntity); }
            set
            {
                _colonyEntity.GetDataBlob<NameDB>().SetName(FactionEntity, value);
                OnPropertyChanged();
            }
        }


        public ColonyScreenVM(GameVM gameVM, Entity colonyEntity, StaticDataStore staticData)
        {

            gameVM.DateChangedEvent += GameVM_DateChangedEvent;
            _colonyEntity = colonyEntity;
            _facilities = new ObservableCollection<FacilityVM>();


            ColonyCargo = new CargoVM(gameVM, colonyEntity);
            //foreach (var installation in colonyEntity.GetDataBlob<InstallationsDB>().Installations)
            {
                //Facilities.Add(new FacilityVM(installation.Key, Colony));
            }
            _species = new Dictionary<string, long>();

            foreach (var kvp in Colony.Population)
            {
                string name = kvp.Key.GetDataBlob<NameDB>().DefaultName;

                _species.Add(name, kvp.Value);
            }

            _mineralDictionary = new Dictionary<Guid, MineralSD>();
            foreach (var mineral in staticData.Minerals)
            {
                _mineralDictionary.Add(mineral.ID, mineral);
            }


            PlanetMineralDepositVM = new PlanetMineralDepositVM(staticData, _colonyEntity.GetDataBlob<ColonyDB>().PlanetEntity);

            RawMineralStockpileVM = new RawMineralStockpileVM(staticData, _colonyEntity);

            RefinedMatsStockpileVM = new RefinedMatsStockpileVM(staticData, _colonyEntity);
        }

        private void GameVM_DateChangedEvent(DateTime oldDate, DateTime newDate)
        {
            Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void Refresh(bool partialRefresh = false)
        {
            //PlanetMineralDepositVM.Refresh();
            RawMineralStockpileVM.Refresh();
            RefinedMatsStockpileVM.Refresh();
            foreach (var facilityvm in Facilities)
            {
                facilityvm.Refresh();
            }
        }
    }

    
    public class PlanetMineralDepositVM : IViewModel
    {
        private Entity _planetEntity;
        private SystemBodyDB systemBodyInfo { get { return _planetEntity.GetDataBlob<SystemBodyDB>(); } }
        private Dictionary<Guid, MineralSD> _mineralDictionary;

        private readonly ObservableDictionary<Guid, PlanetMineralInfoVM> _mineralDeposits = new ObservableDictionary<Guid, PlanetMineralInfoVM>();
        public ObservableDictionary<Guid, PlanetMineralInfoVM> MineralDeposits
        {
            get { return _mineralDeposits; }
        }



        public PlanetMineralDepositVM(StaticDataStore staticData, Entity planetEntity)
        {
            _mineralDictionary = new Dictionary<Guid, MineralSD>();
            foreach (var mineral in staticData.Minerals)
            {
                _mineralDictionary.Add(mineral.ID, mineral);
            }
            _planetEntity = planetEntity;
            Initialise();
        }

        private void Initialise()
        {
            var minerals = systemBodyInfo.Minerals;
            _mineralDeposits.Clear();
            foreach (var kvp in minerals)
            {
                MineralSD mineral = _mineralDictionary[kvp.Key];
                if(!_mineralDeposits.ContainsKey(kvp.Key))
                    _mineralDeposits.Add(kvp.Key, new PlanetMineralInfoVM(mineral.Name, kvp.Value));
            }
            OnPropertyChanged(nameof(MineralDeposits));

        }




        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void Refresh(bool partialRefresh = false)
        {
            if (systemBodyInfo.Minerals.Count != MineralDeposits.Count)
                Initialise();
            else
                foreach (var mineralvm in MineralDeposits.Values)
                {
                    mineralvm.Refresh();
                }
            OnPropertyChanged();
        }
    }

    public class PlanetMineralInfoVM : IViewModel
    {
        private MineralDepositInfo _mineralDepositInfo;

        public string Mineral { get; private set; }
        //public int Amount { get { return _mineralDepositInfo.Amount; } }
        public double Accessability { get { return _mineralDepositInfo.Accessibility; } }

        public PlanetMineralInfoVM(string name, MineralDepositInfo deposit)
        {
            Mineral = name;
            _mineralDepositInfo = deposit;           
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Refresh(bool partialRefresh = false)
        {
            if (PropertyChanged != null)
            {
                //PropertyChanged(this, new PropertyChangedEventArgs(nameof(Amount)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Accessability)));

            }
        }
    }


    public class RawMineralStockpileVM : IViewModel
    {
        private Entity _colonyEntity;
        private ColonyDB Colony { get { return _colonyEntity.GetDataBlob<ColonyDB>(); } }
        private CargoDB _cargoDB; 


        private Dictionary<CargoDefinition, double> MineralDictionary { get { return _cargoDB.CargoCarried.Where(kvp => _mineralGuids.Contains(kvp.Key.ItemGuid)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);  } }



        private StaticDataStore _staticData;
        List<Guid> _mineralGuids; 
        private readonly ObservableDictionary<Guid, RawMineralInfoVM> _mineralStockpile = new ObservableDictionary<Guid, RawMineralInfoVM>();
        public ObservableDictionary<Guid, RawMineralInfoVM> MineralStockpile
        {
            get { return _mineralStockpile; }
        }

        public RawMineralStockpileVM(StaticDataStore staticData, Entity colonyEntity)
        {

            _mineralGuids = staticData.Minerals.Select(mineralSD => mineralSD.ID).ToList();

            _colonyEntity = colonyEntity;
            _cargoDB = colonyEntity.GetDataBlob<CargoDB>();
            Initialise();
        }

        private void Initialise()
        {

            //var rawMinerals = Colony.MineralStockpile;
            var entityCargo = _colonyEntity.GetDataBlob<CargoDB>().CargoCarried;
            var rawMinerals = entityCargo.Where(kvp => _mineralGuids.Contains(kvp.Key.ItemGuid)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            _mineralStockpile.Clear();

            foreach (var kvp in rawMinerals)
            {
                //MineralSD mineral = _mineralDictionary[kvp.Key];
                //if(!MineralStockpile.ContainsKey(kvp.Key))
                //    _mineralStockpile.Add(kvp.Key, new RawMineralInfoVM(kvp.Key, mineral.Name, Colony));             
            }
            OnPropertyChanged(nameof(MineralStockpile));
            
            

            

            
           
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void Refresh(bool partialRefresh = false)
        {
            
            if (_cargoDB.CargoCarried.Count != MineralStockpile.Count)
                Initialise();
            else
                foreach (var mineral in MineralStockpile)
                {
                    mineral.Value.Refresh();
                    
                }
            OnPropertyChanged();
            
        }
    }
    public class RawMineralInfoVM : IViewModel
    {
        private ColonyDB _colony;
        private Guid _guid;
        public string Mineral { get; private set; }
        public int Amount { get { return 0;}} //_colony.MineralStockpile[_guid]; } }

        public RawMineralInfoVM(Guid guid, string name, ColonyDB colony)
        {
            _guid = guid;
            Mineral = name;
            _colony = colony;           
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Refresh(bool partialRefresh = false)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Amount)));
            }
        }
    }

    public class RefinedMatsStockpileVM : IViewModel
    {

        private Entity _colonyEntity;
        private ColonyDB Colony { get { return _colonyEntity.GetDataBlob<ColonyDB>(); } }
        private Dictionary<Guid, RefinedMaterialSD> _materialsDictionary;


        private readonly ObservableDictionary<Guid, RefinedMatInfoVM> _materialStockpile = new ObservableDictionary<Guid, RefinedMatInfoVM>();
        public ObservableDictionary<Guid, RefinedMatInfoVM> MaterialStockpile
        {
            get { return _materialStockpile; }
        }


        public RefinedMatsStockpileVM(StaticDataStore staticData, Entity colonyEntity)
        {
            _materialsDictionary = staticData.RefinedMaterials;
            _colonyEntity = colonyEntity;
            Initialise();
        }

        private void Initialise()
        {
            /*
            var mats = _colonyEntity.GetDataBlob<ColonyDB>().RefinedStockpile;            
            foreach (var kvp in mats)
            {
                RefinedMaterialSD mat = _materialsDictionary[kvp.Key];
                if(!_materialStockpile.ContainsKey(kvp.Key))
                    _materialStockpile.Add(kvp.Key, new RefinedMatInfoVM(kvp.Key, mat.Name, Colony));
                OnPropertyChanged();
            }
           */
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void Refresh(bool partialRefresh = false)
        {
            /*
            if (Colony.MineralStockpile.Count != MaterialStockpile.Count)
                Initialise();
            else
                foreach (var item in MaterialStockpile)
                {
                    item.Value.Refresh();
                }
            OnPropertyChanged();
            */
        }
    }
    public class RefinedMatInfoVM : IViewModel
    {
        private ColonyDB _colony;
        private Guid _guid;
        public string Material { get; private set; }
        public int Amount { get { return 0;}}//_colony.RefinedStockpile[_guid]; } }

        public RefinedMatInfoVM(Guid guid, string name, ColonyDB colony)
        {
            _guid = guid;
            Material = name;
            _colony = colony;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Refresh(bool partialRefresh = false)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Amount)));
            }
        }
    }


    public class FacilityVM : IViewModel
    {
        private Entity _facilityEntity;
        private ColonyDB _colony;

        public string Name { get { return _facilityEntity.GetDataBlob<NameDB>().DefaultName; } }
        public int Count {get { return 0;}} //_colony.Installations[_facilityEntity];}}
        public int WorkersRequired { get { return _facilityEntity.GetDataBlob<ComponentDB>().CrewRequrements * Count; } }

        public FacilityVM()
        {
        }

        public FacilityVM(Entity facilityEntity, ColonyDB colony)
        {
            _facilityEntity = facilityEntity;
            _colony = colony;
            Refresh();

        }
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void Refresh(bool partialRefresh = false)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Count"));
        }
    }
}

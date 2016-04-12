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

        private ObservableCollection<FacilityVM> _facilities;
        public ObservableCollection<FacilityVM> Facilities
        {
            get { return _facilities; }
        }

        private Dictionary<string, long> _species;
        public Dictionary<string, long> Species { get { return _species; } }

        public PlanetMineralDepositVM PlanetMineralDepositVM { get; set; }

        public CargoVM ColonyCargo { get; set; }

        public IndustrialEntityVM ColonyIndustry { get; set; }

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
            ColonyIndustry = new IndustrialEntityVM(colonyEntity);

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



            PlanetMineralDepositVM = new PlanetMineralDepositVM(staticData, _colonyEntity.GetDataBlob<MatedToDB>().Parent);

        }

        private void GameVM_DateChangedEvent(DateTime oldDate, DateTime newDate)
        {
            Refresh();
            ColonyCargo.OnRefresh();
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
            PlanetMineralDepositVM.Refresh();
            
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
        private StaticDataStore _staticData;
        private readonly ObservableDictionary<Guid, PlanetMineralInfoVM> _mineralDeposits = new ObservableDictionary<Guid, PlanetMineralInfoVM>();
        public ObservableDictionary<Guid, PlanetMineralInfoVM> MineralDeposits {get { return _mineralDeposits; }}


        public PlanetMineralDepositVM(StaticDataStore staticData, Entity planetEntity)
        {
            _planetEntity = planetEntity;
            _staticData = staticData;
            Initialise();
        }

        private void Initialise()
        {
            var minerals = systemBodyInfo.Minerals;
            _mineralDeposits.Clear();
            foreach (var kvp in minerals)
            {
                MineralSD mineral = _staticData.Minerals[kvp.Key];
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
        public float Amount { get { return _mineralDepositInfo.Amount; } }
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
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Amount)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Accessability)));

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

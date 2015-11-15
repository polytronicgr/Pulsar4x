using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Pulsar4X.ECSLib;
using Pulsar4X.Networking;

namespace Pulsar4X.ViewModel
{
    public class NewGameOptionsVM : IViewModel
    {
        private GameVM _gameVM;
        
        public string GmPassword { get; set; }
        
        public bool CreatePlayerFaction { get; set; }
        public string FactionName { get; set; }
        public string FactionPassword { get; set; }
        public bool DefaultStart { get; set; }

        public int NumberOfSystems { get; set; }

        public bool NetworkHost { get; set; }
        public int HostPortNum { get; set; }
        
        private NetworkHost NetHost { get { return _gameVM.NetworkModule as NetworkHost; } }
        
        public ObservableCollection<string> ServerMessagesX { get; private set; }
        
        
        public NewGameOptionsVM()
        {
            //CreatePlayerFaction = true;
            //DefaultStart = true;
            //FactionName = "United Earth Federation";
            //FactionPassword = "FPnotImplemented";
            //GmPassword = "GMPWnotImplemented";
            //NumberOfSystems = 50;
            //NetworkHost = true;
            //HostPortNum = 28888;
        }

        public NewGameOptionsVM(GameVM gameVM)
        {

            CreatePlayerFaction = true;
            DefaultStart = true;
            FactionName = "United Earth Federation";
            FactionPassword = "";
            GmPassword = "";
            NumberOfSystems = 50;
            NetworkHost = true;
            HostPortNum = 28888;

            _gameVM = gameVM;
            NetworkHost netHost = new NetworkHost(28888);
            gameVM.NetworkModule = netHost;
            ServerMessagesX = NetHost.Messages;
            
        }

        //public static NewGameOptionsVM Create(GameVM gameVM)
        //{
        //    NewGameOptionsVM optionsVM = new NewGameOptionsVM();
        //    optionsVM._gameVM = gameVM;
        //    NetworkHost netHost = new NetworkHost(gameVM, 28888);
        //    gameVM.NetworkModule = netHost;
        //    optionsVM.ServerMessages = netHost.Messages;
        //    return optionsVM;
        //}

        public void CreateGame()
        {
            _gameVM.CreateGame(this);
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
            ServerMessagesX.Add("testAdd");
            //throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pulsar4x.Networking;
using Pulsar4X.ECSLib;


namespace Pulsar4X.ViewModel
{
    public class NetworkClientConnectVM : IViewModel
    {
        private GameVM _gameVM { get; set; }
        public string HostAddress { get; set; }
        public int HostPortNum { get; set; }
        private NetworkClient NetClient { get { return _gameVM.NetworkModule as NetworkClient; } }
        public string GameName { get; set; }

        public ObservableCollection<String> ServerMessages { get; set; }
            public ObservableCollection<string> Factions { get; set; }

        public NetworkClientConnectVM()
        {  
         
            HostAddress = "localhost";
            HostPortNum = 28888;
            GameName = "Not Connected";
            Factions = new ObservableCollection<string>();
            
        }

        public NetworkClientConnectVM(GameVM gameVM) : this()
        {
            _gameVM = gameVM; 
            NetworkClient netClient = new NetworkClient(_gameVM, HostAddress, HostPortNum);
            _gameVM.NetworkModule = netClient; 
            ServerMessages = _gameVM.NetworkMessages;

        }

        public void OnConnect()
        {

            NetworkClient netClient = new NetworkClient(_gameVM, HostAddress, HostPortNum);
            _gameVM.NetworkModule = netClient;
            netClient.ClientConnect();
            //netClient.SendFactionListRequest();
        }

        private ICommand _connectButton;
        public ICommand ConnectButton
        {
            get
            {
                return _connectButton ?? (_connectButton = new CommandHandler(OnConnect, true));
            }
        }

        private ICommand _getFactions;
        public ICommand GetFactions
        {
            get
            {
                if (_gameVM != null)
                    return _getFactions ?? (_getFactions = new CommandHandler(NetClient.SendFactionListRequest, NetClient.IsConnectedToServer) );
                return _getFactions ?? (_getFactions = new CommandHandler(null, false));
            }
            set { OnPropertyChanged();}
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
            throw new NotImplementedException();
        }    
        

    }
}

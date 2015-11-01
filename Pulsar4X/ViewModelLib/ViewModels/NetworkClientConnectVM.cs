using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pulsar4x.Networking;


namespace Pulsar4X.ViewModel
{
    public class NetworkClientConnectVM : IViewModel
    {
        private GameVM _gameVM { get; set; }
        public string HostAddress { get; set; }
        public int HostPortNum { get; set; }

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
            netClient.SendFactionListRequest();
        }

        private ICommand _connectButton;
        public ICommand ConnectButton
        {
            get
            {
                return _connectButton ?? (_connectButton = new CommandHandler(OnConnect, true));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Refresh(bool partialRefresh = false)
        {
            throw new NotImplementedException();
        }    
        

    }
}

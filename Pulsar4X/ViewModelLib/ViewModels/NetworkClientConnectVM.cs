using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pulsar4x.Networking;
using Pulsar4X.ECSLib;


namespace Pulsar4X.ViewModel
{
    public class NetworkClientConnectVM : IViewModel
    {
        private GameVM _gameVM;

        public string HostAddress { get; set; }
        public int HostPortNum { get; set; }

        private NetworkClient NetClient { get { return _gameVM.NetworkModule as NetworkClient; } }
        
        public string GameName { get; private set; }
        public ObservableCollection<string> ServerMessages { get; private set; }
        public ObservableCollection<FactionItem> Factions { get; private set;  }

        public NetworkClientConnectVM()
        {           
        }

        public NetworkClientConnectVM(GameVM gameVM)
        {
            _gameVM = gameVM; 
            NetworkClient netClient = new NetworkClient(_gameVM, "localhost", 28888);            
            _gameVM.NetworkModule = netClient;
            HostAddress = NetClient.HostAddress;
            HostPortNum = NetClient.PortNum;

            ServerMessages = NetClient.Messages;
            Factions = NetClient.Factions;
            GetFactions = GetFactions;
            
        }

        public void OnConnect()
        {
            NetClient.ClientConnect();
            GetFactions = GetFactions;
            NetClient.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
        }

        private void UpdateFactions()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("Factions"));
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
                    return _getFactions ?? (_getFactions = new CommandHandler(NetClient.SendFactionListRequest, true)); //NetClient.IsConnectedToServer) );
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

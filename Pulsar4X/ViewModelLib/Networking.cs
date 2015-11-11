using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Threading;
using Lidgren.Network;
using Pulsar4X.ECSLib;
using Pulsar4X.ViewModel;

namespace Pulsar4x.Networking
{
    public class NetworkBase
    {
        protected Game _game_ { get { return _gameVM_.Game; }}
        protected GameVM _gameVM_ { get; set; }
        protected const int SecureChannel = 31;
        private readonly ObservableCollection<string> _messages;
        public ObservableCollection<string> Messages { get {return _messages;} }

        public int PortNum { get; set; }
        public NetPeer NetPeerObject { get; set; }

        public NetworkBase()
        {
            _messages = new ObservableCollection<string>();
            _messages.Add("Start of Messages");
        }

        protected void StartListning()
        {            
            NetPeerObject.RegisterReceivedCallback(new SendOrPostCallback(GotMessage)); 
        }

        /// <summary>
        /// This gets called when triggered by an event regestered in StartListning()
        /// </summary>
        /// <param name="peer">this is NetPeerObject so just using that</param>
        public void GotMessage(object peer)
        {
            NetIncomingMessage message; 
            
            while ((message = NetPeerObject.ReadMessage()) != null)
            {
                if (message.SequenceChannel == SecureChannel)
                {
                    DecryptedReceve(message);
                }
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        // handle custom messages
                        Messages.Add("Data Message from: " + message.SenderConnection.RemoteUniqueIdentifier);
                        HandleIncomingDataMessage(message.SenderConnection, message);
                        break;

                    case NetIncomingMessageType.DiscoveryRequest:
                        HandleDiscoveryRequest(message);
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        HandleDiscoveryResponce(message);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        // handle connection status messages
                        Messages.Add("New status: " + message.SenderConnection.Status + " (Reason: " + message.ReadString() + ")");
                        ConnectionStatusChanged(message);
                        break;                   
                    case NetIncomingMessageType.DebugMessage:
                        // handle debug messages
                        // (only received when compiled in DEBUG mode)
                        Messages.Add("Debug Msg: " + message.ReadString());
                        break;

                    /* .. */
                    default:
                        Messages.Add(("unhandled message with type: " + message.MessageType));
                        break;
                }
            }
        }

        /// <summary>
        /// TODO implement proper private/public key. is that even possible to do transperantly?
        /// </summary>
        /// <param name="recever"></param>
        /// <param name="message"></param>
        protected NetOutgoingMessage Encrypt(NetOutgoingMessage message)
        {
            NetEncryption algo = new NetXtea(NetPeerObject, "SharedKey45B635DF-649B-4C10-B110-439CE1784C59");
            message.Encrypt(algo);
            return message;
        }


        protected NetIncomingMessage DecryptedReceve(NetIncomingMessage message)
        {
            NetEncryption algo = new NetXtea(NetPeerObject, "SharedKey45B635DF-649B-4C10-B110-439CE1784C59");
            message.Decrypt(algo);
            return message;
        }

        protected virtual void HandleDiscoveryRequest(NetIncomingMessage message)
        {
        }
        protected virtual void HandleDiscoveryResponce(NetIncomingMessage message)
        {
        }
        protected virtual void ConnectionStatusChanged(NetIncomingMessage message)
        {
        }
        protected void HandleIncomingDataMessage(NetConnection sender, NetIncomingMessage message)
        {
            DataMessageType messageType = (DataMessageType)message.ReadByte();
            switch (messageType)
            {
                    case DataMessageType.FactionDataRequest:
                        HandleFactionDataRequest(message);
                        break;
                    case DataMessageType.GameData:
                        HandleGameDataMessage(message);
                        break;
                    case DataMessageType.TickInfo:
                        HandleTickInfo(message);
                        break;
                    case DataMessageType.EntityData:
                        HandleEntityData(message);
                        break;
            }
        }

        /// <summary>
        /// server only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        protected virtual void HandleFactionDataRequest(NetIncomingMessage message)
        {
        }


        /// <summary>
        /// this one should be client only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        protected virtual void HandleEntityData(NetIncomingMessage message)
        {
        }

        /// <summary>
        /// client only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        protected virtual void HandleTickInfo(NetIncomingMessage message)
        {
        }



        /// <summary>
        /// client only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        protected virtual void HandleGameDataMessage(NetIncomingMessage message)
        {
        }


        /// <summary>
        /// use this for sending an non basic object type.
        /// </summary>
        /// <param name="dataMessage"></param>
        /// <returns></returns>
        protected NetOutgoingMessage SerialiseDataMessage(DataMessage dataMessage)
        {
            //turn the dataMessage into a stream.
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            binFormatter.Serialize(mStream, dataMessage);
            NetOutgoingMessage sendMsg = NetPeerObject.CreateMessage();
            sendMsg.Write(mStream.ToArray()); //send the stream as an byte array. 
            return sendMsg;
        }

        protected void HandleIncomingStringMessage(NetConnection sender, DataMessage dataMessage)
        {
            Messages.Add(sender.RemoteUniqueIdentifier + " Sent: " + (string)dataMessage.DataObject);
        }

    }

    public class NetworkHost : NetworkBase
    {
        private Dictionary<NetConnection, Guid> _connectedFactions { get; set; }
        private Dictionary<Guid, List<NetConnection>> _factionConnections { get; set; } 
               
        public NetServer NetServerObject { get { return (NetServer)NetPeerObject; }}

        

        public NetworkHost(GameVM gameVM, int portNum)
        {
            _gameVM_ = gameVM;
            PortNum = portNum;
        }

        public void ServerStart()
        {
            var config = new NetPeerConfiguration("Pulsar4X") { Port = PortNum };
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            
            NetPeerObject = new NetServer(config);
            NetPeerObject.Start();
            _connectedFactions = new Dictionary<NetConnection, Guid>();
            _factionConnections = new Dictionary<Guid, List<NetConnection>>();
            StartListning();
            _game_.TickEvent += OnTickEvent;
        }

        private void OnTickEvent(DateTime currentTime, int delta)
        {
            IList<NetConnection> connections = _connectedFactions.Keys.ToList();
            NetOutgoingMessage sendMsg = NetServerObject.CreateMessage();
            sendMsg.Write((byte)DataMessageType.TickInfo);
            sendMsg.Write(currentTime.ToBinary());
            sendMsg.Write(delta);

            NetServerObject.SendMessage(sendMsg, connections, NetDeliveryMethod.ReliableOrdered, 0);

        }

        protected override void HandleDiscoveryRequest(NetIncomingMessage message)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => Messages.Add("RX DiscoveryRequest " + message.SenderEndPoint)));
            
            Messages.Add("RX DiscoveryRequest " + message.SenderEndPoint);
            NetOutgoingMessage response = NetServerObject.CreateMessage();
            response.Write(_game_.GameName);
            response.Write(_game_.CurrentDateTime.ToBinary());
            
            NetServerObject.SendDiscoveryResponse(response, message.SenderEndPoint);
        }

        protected override void HandleFactionDataRequest(NetIncomingMessage message)
        {
            NetConnection sender = message.SenderConnection;
            string name = message.ReadString();
            string pass = message.ReadString();
            List<Entity> factions = _game_.GlobalManager.GetAllEntitiesWithDataBlob<FactionInfoDB>();

            Entity faction = factions.Find(item => item.GetDataBlob<NameDB>().DefaultName == name);

            if (AuthProcessor.Validate(faction, pass))
            {
                if (_connectedFactions.ContainsKey(sender))                
                    _connectedFactions[sender] = faction.Guid;                
                else 
                    _connectedFactions.Add(sender, faction.Guid);
                if(!_factionConnections.ContainsKey(faction.Guid))
                    _factionConnections.Add(faction.Guid, new List<NetConnection>());
                _factionConnections[faction.Guid].Add(sender);
                SendFactionData(sender, faction);
            }
        }

        protected override void ConnectionStatusChanged(NetIncomingMessage message)
        {
            switch (message.SenderConnection.Status)
            {
                case NetConnectionStatus.Connected:
                    break;
                case NetConnectionStatus.Disconnected:                
                    if (_connectedFactions.ContainsKey(message.SenderConnection))
                    {
                        Guid factionGuid = _connectedFactions[message.SenderConnection];
                        _factionConnections[factionGuid].Remove(message.SenderConnection);
                        _connectedFactions.Remove(message.SenderConnection);
                    }                
                break;
            }
        }

        private void SetSendMessages()
        {
            
            //set events in the processors? or move this into the ecslib...
        }

        private void SendFactionList(NetConnection recipient)
        {
            //list of factions: 
            
            List<Entity> factions = _game_.GlobalManager.GetAllEntitiesWithDataBlob<FactionInfoDB>();
            //we don't want to send the whole entitys, just a dictionary of guid ID and the string name. 
            //Dictionary<Guid,string> factionGuidNames = factions.ToDictionary(faction => faction.Guid, faction => faction.GetDataBlob<NameDB>().DefaultName);

            List<FactionItem> factionItems = new List<FactionItem>();
            foreach (var faction in factions)
            {
                FactionItem factionItem = new FactionItem();
                factionItem.Name = faction.GetDataBlob<NameDB>().DefaultName;
                factionItem.ID = faction.Guid;
                factionItems.Add(factionItem);
            }

            DataMessage dataMessage = new DataMessage { DataMessageType = DataMessageType.FactionDictionary, DataObject = factionItems };

            NetOutgoingMessage sendMsg = SerialiseDataMessage(dataMessage);
            
            Messages.Add("TX Faction List to " + recipient.RemoteUniqueIdentifier);
            NetServerObject.SendMessage(sendMsg, recipient, NetDeliveryMethod.ReliableOrdered);
        }



        private void SendFactionData(NetConnection recipient, Entity factionEntity)
        { 

            var mStream = new MemoryStream();
            SaveGame.Save(factionEntity, mStream);
            byte[] entityByteArray = mStream.ToArray();

            int len = entityByteArray.Length;
            NetOutgoingMessage sendMsg = NetPeerObject.CreateMessage();
            sendMsg.Write((byte)DataMessageType.EntityData);
            sendMsg.Write(factionEntity.Guid.ToByteArray());
            sendMsg.Write(len);           
            sendMsg.Write(entityByteArray);
            


            NetServerObject.SendMessage(sendMsg, recipient, NetDeliveryMethod.ReliableOrdered);
            

        }
    }

    public enum DataMessageType : byte
    {
        StringMessage,
        FactionDictionary,
        GameData,
        EntityData,        
        DataBlobData,
        DataBlobPropertyUpdate,
        FactionDataRequest,

        TickInfo
    }

    /*
     * 
     * GameData, (string)gameName, (long)currentDate
     * EntityData, (Byte[])Guid, (Byte[])memeoryStream.
     * 
     * 
     * FactionDataRequest, (string)factionName, (string)password
     * TickInfo, (long)fromDate, (long)Delta
     * 
     */

    [Serializable]
    public class DataMessage
    {
        public DataMessageType DataMessageType { get; set; }
        public object DataObject { get; set; }

        public Guid EntityGuid { get; set; }
        public string PropertyName { get; set; }//actualy can I look at how wpf does this?
    }

    /// <summary>
    /// a short struct which holds string name and guid of a faction. 
    /// </summary>
    [Serializable]
    public struct FactionItem
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
    }

    public class NetworkClient : NetworkBase
    {

        private NetClient NetClientObject { get { return (NetClient)NetPeerObject; } }
        public string HostAddress { get; set; }
        private bool _isConnectedToServer;
        public bool IsConnectedToServer { get { return _isConnectedToServer; } set { _isConnectedToServer = value; OnPropertyChanged(); } }
        public string ConnectedToGameName { get; private set; }
        public DateTime ConnectedToDateTime { get; private set; }
        //private Dictionary<Guid, string> _factions; 
        public ObservableCollection<FactionItem> Factions { get; set; }
        
        public NetworkClient(GameVM gameVM, string hostAddress, int portNum)
        {
            _gameVM_ = gameVM;
            PortNum = portNum;
            HostAddress = hostAddress;
            IsConnectedToServer = false;
            Factions = new ObservableCollection<FactionItem>();
        }

        public void ClientConnect()
        {
            var config = new NetPeerConfiguration("Pulsar4X");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            NetPeerObject = new NetClient(config);
            NetPeerObject.Start();
            NetClientObject.DiscoverLocalPeers(PortNum);
            NetPeerObject.Connect(host: HostAddress, port: PortNum);
            StartListning();
            
        }


        protected override void HandleDiscoveryResponce(NetIncomingMessage message)
        {
            ConnectedToGameName = message.ReadString();
            ConnectedToDateTime = new DateTime(message.ReadInt64());
            Messages.Add("Found Server: " + message.SenderEndPoint + "Name Is: " + ConnectedToGameName);            
        }

        protected override void HandleGameDataMessage(NetIncomingMessage message)
        {
            ConnectedToGameName = message.ReadString();
            ConnectedToDateTime = new DateTime(message.ReadInt64());
        }


        protected override void HandleTickInfo(NetIncomingMessage message)
        {
            ConnectedToDateTime = new DateTime(message.ReadInt64());
            int delta = message.ReadInt32();
            delta += (int)(ConnectedToDateTime - _game_.CurrentDateTime).TotalSeconds;
            _game_.AdvanceTime(delta);
        }

        protected override void HandleEntityData(NetIncomingMessage message)
        {
            
            Guid entityID = new Guid(message.ReadBytes(16));            
            int len = message.ReadInt32();
            byte[] data = message.ReadBytes(len);

            if (_game_ == null || _game_.GameName != ConnectedToGameName) //TODO handle if connecting to a game when in the middle of a singleplayer game. (ie prompt save)
            {
                _gameVM_.Game = Game.NewGame(ConnectedToGameName, ConnectedToDateTime, 0, null, false);
            }
 
            var mStream = new MemoryStream(data);
            Entity entity = SaveGame.ImportEntity(_game_.GlobalManager, entityID, mStream);
        }

        protected override void ConnectionStatusChanged(NetIncomingMessage message)
        {
            switch (message.SenderConnection.Status)
            {
                case  NetConnectionStatus.Connected:               
                    IsConnectedToServer = true;               
                    break;
                case NetConnectionStatus.Disconnected:
                    IsConnectedToServer = false;
                    break;
            }
        }
             

        public void SendFactionDataRequest(string faction, string password)
        {

            NetOutgoingMessage sendMsg = NetPeerObject.CreateMessage();
            sendMsg.Write((byte)DataMessageType.FactionDataRequest);
            sendMsg.Write(faction);
            sendMsg.Write(password);
            Encrypt(sendMsg);//sequence channel 31 is expected to be encrypted by the recever. see NetworkBase GotMessage()
            NetClientObject.SendMessage(sendMsg, NetClientObject.ServerConnection, NetDeliveryMethod.ReliableOrdered, SecureChannel); 
        }

        public void ReceveFactionList(DataMessage dataMessage)
        {
            Factions.Clear();
            
            foreach (var factionItem in (List<FactionItem>)dataMessage.DataObject)
            {
                Factions.Add(factionItem);
            }
        }

        public void SendFactionListRequest()
        {            
            DataMessage dataMessage = new DataMessage();
            dataMessage.DataMessageType = DataMessageType.FactionDictionary;

            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            binFormatter.Serialize(mStream, dataMessage);


            NetOutgoingMessage sendMsg = NetClientObject.CreateMessage();
            sendMsg.Write(mStream.ToArray()); //send the stream as an byte array. 
            NetClientObject.SendMessage(sendMsg, NetClientObject.ServerConnection, NetDeliveryMethod.ReliableOrdered);
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
    }
}

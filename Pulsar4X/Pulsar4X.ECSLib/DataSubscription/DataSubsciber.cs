using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Pulsar4X.ECSLib;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib.DataSubscription
{
    /// <summary>
    /// Need one of these per UI connection. 
    /// </summary>
    public class DataSubsciber
    {
        private readonly Guid _connectionID;
        private readonly Game _game;

        private Dictionary<Guid, HashSet<string>> SubscribedDataCodes { get; } = new Dictionary<Guid, HashSet<string>>();

        public DataSubsciber(Game game, Guid connectionID)
        {

            _game = game;
            _connectionID = connectionID;
        }

        internal void Subscribe(Guid entityGuid, string dataCode)
        {
            Entity entity;
            if (_game.GlobalManager.FindEntityByGuid(entityGuid, out entity))
            {
                if (!SubscribedDataCodes.ContainsKey(entityGuid))
                    SubscribedDataCodes.Add(entityGuid, new HashSet<string>()); 
                SubscribedDataCodes[entityGuid].Add(dataCode);
            }
        }


        internal void TriggerIfSubscribed(Guid entityGuid, UIData uiData)
        {
            if (IsSubscribedTo(entityGuid, uiData.GetDataCode))
            {            
                _game.MessagePump.EnqueueOutgoingMessage(_connectionID, uiData);
            }
        }

        internal bool IsSubscribedTo(Guid entityGuid, string dataCode)
        {
            if (SubscribedDataCodes.ContainsKey(entityGuid) && SubscribedDataCodes[entityGuid].Contains(dataCode))
                return true;
            else          
                return false;
        }

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};

        public static string SerializeDataBlob(BaseDataBlob db) { return JsonConvert.SerializeObject(db, Settings); }

        public static BaseDataBlob DeserializeDataBlob(string jsonString) { return JsonConvert.DeserializeObject<BaseDataBlob>(jsonString, Settings); }
    }

 
    public class SubscriptionRequestMessage : BaseToServerMessage 
    {
        public Guid EntityGuid { get; set; }
        public string DataCode { get; set; }
        
        [JsonConstructor]
        public SubscriptionRequestMessage() { }

        public static SubscriptionRequestMessage NewMessage(Guid connectionID, Guid factionID, Guid entityGuid, string dataCode)
        {
            return new SubscriptionRequestMessage()
            {
                ConnectionID = connectionID,
                FactionGuid = factionID,
                EntityGuid = entityGuid,
                DataCode = dataCode,
            };
        }

        internal override void HandleMessage(Game game)
        {            
            game.MessagePump.DataSubscibers[ConnectionID].Subscribe(EntityGuid, DataCode);
        }
    }

    public abstract class UIData : BaseToClientMessage
    {
        
    }
}



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

        private Dictionary<Guid, HashSet<Type>> SubscribedDataCodes { get; } = new Dictionary<Guid, HashSet<Type>>();

        public DataSubsciber(Game game, Guid connectionID)
        {

            _game = game;
            _connectionID = connectionID;
        }

        internal void Subscribe<T>(Guid entityGuid) where T: BaseToClientMessage
        {
            Entity entity;
            if (_game.GlobalManager.FindEntityByGuid(entityGuid, out entity))
            {
                if (!SubscribedDataCodes.ContainsKey(entityGuid))
                    SubscribedDataCodes.Add(entityGuid, new HashSet<Type>()); 
                SubscribedDataCodes[entityGuid].Add(typeof(T));
            }
        }


        internal void TriggerIfSubscribed<T>(Guid entityGuid, UIData uiData) where T : BaseToClientMessage
        {
            if (IsSubscribedTo<T>(entityGuid))
            {            
                _game.MessagePump.EnqueueOutgoingMessage(_connectionID, uiData);
            }
        }

        internal bool IsSubscribedTo<T>(Guid entityGuid) where T : BaseToClientMessage
        {
            if (SubscribedDataCodes.ContainsKey(entityGuid) && SubscribedDataCodes[entityGuid].Contains(typeof(T)))
                return true;
            else          
                return false;
        }

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};

        public static string SerializeDataBlob(BaseDataBlob db) { return JsonConvert.SerializeObject(db, Settings); }

        public static BaseDataBlob DeserializeDataBlob(string jsonString) { return JsonConvert.DeserializeObject<BaseDataBlob>(jsonString, Settings); }
    }

 
    public class SubscriptionRequestMessage<T> : BaseToServerMessage where T: BaseToClientMessage
    {
        public Guid EntityGuid { get; set; }

        
        [JsonConstructor]
        public SubscriptionRequestMessage() { }

        public static SubscriptionRequestMessage<T> NewMessage(Guid connectionID, Guid factionID, Guid entityGuid)
        {
            return new SubscriptionRequestMessage<T>()
            {
                ConnectionID = connectionID,
                FactionGuid = factionID,
                EntityGuid = entityGuid,
            };
        }

        internal override void HandleMessage(Game game)
        {            
            game.MessagePump.DataSubscibers[ConnectionID].Subscribe<T>(EntityGuid);
        }
    }

    public abstract class UIData : BaseToClientMessage
    {
        
    }
}



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

        private Dictionary<Guid, HashSet<string>> SubscribedDatablobs { get; } = new Dictionary<Guid, HashSet<string>>();

        public DataSubsciber(Game game, Guid connectionID)
        {

            _game = game;
            _connectionID = connectionID;
        }

        internal void Subscribe<T>(Guid entityGuid)
            where T : UIData
        {
            Entity entity;
            if (_game.GlobalManager.FindEntityByGuid(entityGuid, out entity))
            {
                if (!SubscribedDatablobs.ContainsKey(entityGuid))
                    SubscribedDatablobs.Add(entityGuid, new HashSet<string>()); 
                SubscribedDatablobs[entityGuid].Add(typeof(T).ToString());
            }
        }


        internal void TriggerIfSubscribed<T>(Guid entityGuid, UIData uiData)
            where T : UIData
        {
            if (IsSubscribedTo<T>(entityGuid))
            {
                

                //T datablob = entity.GetDataBlob<T>();
                //string stringblob = SerializeDataBlob(datablob);
                //T uiData = UIData.CreateNew(_game, entity);
                //UIDataBlobUpdateMessage message = new UIDataBlobUpdateMessage(uiData);
                //_game.MessagePump.EnqueueOutgoingMessage(_connectionID, message);
                _game.MessagePump.EnqueueOutgoingMessage(_connectionID, uiData);
            }
        }

        internal bool IsSubscribedTo<T>(Guid entityGuid)
            where T : UIData
        {
            if (SubscribedDatablobs.ContainsKey(entityGuid) && SubscribedDatablobs[entityGuid].Contains(typeof(T).ToString()))
                return true;
            else
                return false;
        }

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};

        public static string SerializeDataBlob(BaseDataBlob db) { return JsonConvert.SerializeObject(db, Settings); }

        public static BaseDataBlob DeserializeDataBlob(string jsonString) { return JsonConvert.DeserializeObject<BaseDataBlob>(jsonString, Settings); }
    }

 
    public class SubscriptionRequestMessage<T> : BaseToServerMessage where T : UIData
    {
        public Guid EntityGuid { get; set; }

        public SubscriptionRequestMessage() { }

        public static SubscriptionRequestMessage<T> newMessage(Guid connectionID, Guid factionID, Guid entityGuid)
        {
            return new SubscriptionRequestMessage<T>()
            {
                ConnectionID = connectionID,
                FactionGuid = factionID,
                EntityGuid = entityGuid                
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



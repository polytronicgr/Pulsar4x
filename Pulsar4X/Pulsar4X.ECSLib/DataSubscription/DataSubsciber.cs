using System;
using System.Collections.Generic;
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
            where T : BaseDataBlob
        {
            Entity entity;
            if (_game.GlobalManager.FindEntityByGuid(entityGuid, out entity))
            {
                if (!SubscribedDatablobs.ContainsKey(entityGuid))
                    SubscribedDatablobs.Add(entityGuid, new HashSet<string>());
                SubscribedDatablobs[entityGuid].Add(nameof(T));
            }
        }


        internal void TriggerIfSubscribed<T>(Guid entityGuid)
            where T : BaseDataBlob
        {
            if (IsSubscribedTo<T>(entityGuid))
            {
                Entity entity;
                _game.GlobalManager.FindEntityByGuid(entityGuid, out entity);
                T datablob = entity.GetDataBlob<T>();
                //string stringblob = SerializeDataBlob(datablob);
                UIDataBlobUpdateMessage message = new UIDataBlobUpdateMessage(datablob, "");
                _game.MessagePump.EnqueueOutgoingMessage(_connectionID, message);
            }
        }

        private bool IsSubscribedTo<T>(Guid entityGuid)
            where T : BaseDataBlob
        {
            if (SubscribedDatablobs.ContainsKey(entityGuid) && SubscribedDatablobs[entityGuid].Contains(nameof(T)))
                return true;
            else
                return false;
        }

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};

        public static string SerializeDataBlob(BaseDataBlob db) { return JsonConvert.SerializeObject(db, Settings); }

        public static BaseDataBlob DeserializeDataBlob(string jsonString) { return JsonConvert.DeserializeObject<BaseDataBlob>(jsonString, Settings); }
    }

}



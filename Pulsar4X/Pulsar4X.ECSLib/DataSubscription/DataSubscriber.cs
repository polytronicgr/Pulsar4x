using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib.DataSubscription
{
    public class DataSubscriber
    {
        private readonly Guid _connectionID;
        private readonly Game _game;

        private Dictionary<Guid, HashSet<Type>> SubscribedDatablobsByEntity { get; } = new Dictionary<Guid, HashSet<Type>>();

        public DataSubscriber(Game game, Guid connectionID)
        {

            _game = game;
            _connectionID = connectionID;
        }


        public void Subscribe<T>(Guid entityGuid)
            where T : SubscribableDatablob
        {
            Entity entity;
            if (_game.GlobalManager.FindEntityByGuid(entityGuid, out entity))
            {
                if (entity.HasDataBlob<T>())
                {
                    if (!SubscribedDatablobsByEntity.ContainsKey(entityGuid))
                        SubscribedDatablobsByEntity.Add(entityGuid, new HashSet<Type>());
                    SubscribedDatablobsByEntity[entityGuid].Add(typeof(T));
                    SubscribedEntityDB subscribedEntity = new SubscribedEntityDB();
                    if (!entity.HasDataBlob<SubscribedEntityDB>())
                    { 
                        entity.SetDataBlob<SubscribedEntityDB>(subscribedEntity);
                        subscribedEntity.SubscribableDatablobs.Add(entity.GetDataBlob<T>());
                    }
                    SubscribableDatablob db = entity.GetDataBlob<T>();
                    db.HasSubscribers = true;
                }
            }
        }

        public void UnsubscribeToDatablob<T>(Guid entityGuid)
            where T : SubscribableDatablob
        {
            Entity entity;
            if (_game.GlobalManager.FindEntityByGuid(entityGuid, out entity))
            {

                if (SubscribedDatablobsByEntity.ContainsKey(entityGuid))
                    SubscribedDatablobsByEntity[entityGuid].Remove(typeof(T));

                //TODO check no other connections are subscribed to this datablob and remove the datablob from the entity. 
            }
        }

        public static List<DatablobChangedMessage> ChangedEntityMessages(EntityManager manager)
        {
            var changedEntityMessages = new List<DatablobChangedMessage>();

            var entites = manager.GetAllEntitiesWithDataBlob<SubscribedEntityDB>();
            List<BaseDataBlob> subscribedDatablobs = entites.Select(entity => entity.GetDataBlob<SubscribedEntityDB>()).Cast<BaseDataBlob>().ToList();

            foreach (SubscribedEntityDB subscribedEntityDB in subscribedDatablobs)
            {
                foreach (var datablob in subscribedEntityDB.SubscribableDatablobs)
                {
                    DatablobChangedMessage changedMessage = new DatablobChangedMessage(datablob.OwningEntity.Guid, datablob.Changes, datablob.GetType().ToString());
                    datablob.Changes.Clear();
                    changedEntityMessages.Add(changedMessage);
                }
            }
            return changedEntityMessages;
        }

        
        /// <summary>
        /// checks if subscribed to the entity and enqueues message if so. 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messagePump"></param>
        public void SendDatablobChangesIfSubscribedToEntity(DatablobChangedMessage message, MessagePumpServer messagePump)
        {
            if( SubscribedDatablobsByEntity.ContainsKey(message.EntityGuid))
                messagePump.EnqueueOutgoingMessage(_connectionID, message);       
        }

        public bool IsSubscribedTo<T>(Guid entityGuid)
        {
            if (SubscribedDatablobsByEntity.ContainsKey(entityGuid))
                return (SubscribedDatablobsByEntity[entityGuid].Contains(typeof(T)));
            return false;
        }

    }

    public class DatablobChangedMessage : BaseToClientMessage 
    {
        public List<DatablobChange> Changes;
        public Guid EntityGuid;
        public override string ResponseCode { get; } 

        public DatablobChangedMessage(Guid entityID, List<DatablobChange> changes, string responseCode) 
        {
            Changes = changes;
            EntityGuid = entityID;
            ResponseCode = responseCode;
        }

        
    }


    public abstract class SubscribableDatablob : BaseDataBlob
    {
        private bool _hasSubscribers;

        
        internal bool HasSubscribers {
            get { return _hasSubscribers; }
            set
            {
                if (_hasSubscribers != value)
                {
                    _hasSubscribers = value;
                    if (!_hasSubscribers)
                        Changes = null; //this should reduce the memory size of the datablob when it's not needed...
                    else
                    {
                        Changes = new List<DatablobChange>();
                    }
                }
            }
        }
        internal List<DatablobChange> Changes { get; private set; }
    }

    public abstract class DatablobChange
    {
        
    }

    public class SubscribedEntityDB : BaseDataBlob
    {
        [JsonIgnore]
        internal List<SubscribableDatablob> SubscribableDatablobs { get; } = new List<SubscribableDatablob>();
        
        public override object Clone() { throw new NotImplementedException(); }
    }
}
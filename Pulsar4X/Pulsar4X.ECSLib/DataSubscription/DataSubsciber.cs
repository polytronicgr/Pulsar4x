using System;
using System.Collections.Generic;

namespace Pulsar4X.ECSLib.DataSubscription
{
    /// <summary>
    /// Need one of these per UI connection. 
    /// </summary>
    public class DataSubsciber
    {
        private Guid UISubscriberID;
        private Entity _factionEntity;

        private readonly Game _game;
        //private List<Entity> SubscribedEntites { get; } = new List<Entity>();
        private Dictionary<Guid,HashSet<string>> SubscribedDatablobs { get; } = new Dictionary<Guid, HashSet<string>>();

        public DataSubsciber(Game game, Guid uiSubscriber) 
        { 
            _game = game;
            UISubscriberID = uiSubscriber;
        }

        /*
        internal void Subscibe(Guid entityGuid)
        {
            Entity entity;
            if (_game.GlobalManager.FindEntityByGuid(entityGuid, out entity))
                SubscribedEntites.Add(entity);
        }*/

        internal void Subscribe<T>(Guid entityGuid)
            where T : BaseDataBlob
        {
            Entity entity;
            if (_game.GlobalManager.FindEntityByGuid(entityGuid, out entity))
            {
                if(!SubscribedDatablobs.ContainsKey(entityGuid))
                    SubscribedDatablobs.Add(entityGuid, new HashSet<string>());
                SubscribedDatablobs[entityGuid].Add(nameof(T));
            }
        }

        /*
        internal void TriggerIfSubscribed(Guid entityGuid)
        {
            
        }*/
        
        internal void TriggerIfSubscribed<T>(Guid entityGuid)
            where T : BaseDataBlob
        {
            if (IsSubscribedTo<T>(entityGuid))
            {
                //TODO: construct message;
                //TODO: send message to the ui connection that this is created for.
            }
        }

        private bool IsSubscribedTo<T>(Guid entityGuid)
            where T : BaseDataBlob
        {
            if(SubscribedDatablobs.ContainsKey(entityGuid) && SubscribedDatablobs[entityGuid].Contains(nameof(T)))                
                return true;
            else        
                return false;
        }
    }

    public class UIConnections
    {
        Dictionary<Guid, UIConnection> _connections = new Dictionary<Guid, UIConnection>();

        public void CreateConnection()
        {
            var newConnection = new UIConnection();
        }

        public void NotifyConnectionsOfDatablobChanges<T>(Guid entityGuid) where T : BaseDataBlob
        {
            foreach (var item in _connections.Values)
            {
                item._dataSubsciber.TriggerIfSubscribed<T>(entityGuid);
            }
        }
    }

    public class UIConnection
    {
        private Guid _connectionID;
        internal DataSubsciber _dataSubsciber;
        private OutGoingMessageQueue OutGoingMessageQueue { get; } = new OutGoingMessageQueue();
    }
}
#region Copyright/License
// Copyright© 2017 Daniel Phelps
//     This file is part of Pulsar4x.
// 
//     Pulsar4x is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Pulsar4x is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using static Pulsar4X.ECSLib.EntityChangeEvent;

namespace Pulsar4X.ECSLib
{
    internal class EntityChangeProcessor
    {
        #region Fields
        private readonly Dictionary<Guid, List<EntityChangeEvent>> _entityChanges = new Dictionary<Guid, List<EntityChangeEvent>>();
        private readonly List<EntityChangeEvent> _entityEvents = new List<EntityChangeEvent>();

        /// <summary>
        /// </summary>
        /// Key == EntityID
        /// Value == List of ConnectionID's subscribed.
        private readonly Dictionary<Guid, List<Guid>> _subscriptions = new Dictionary<Guid, List<Guid>>();
        #endregion

        #region Internal Methods
        internal void ClearEvents()
        {
            _entityChanges.Clear();
            _entityEvents.Clear();
        }

        internal void UnsubscribeToEntity(Guid entityGuid, Guid connectionID)
        {
            if (entityGuid == Guid.Empty)
            {
                throw new ArgumentException("Cannot subscribe to an invalid entity.", nameof(entityGuid));
            }

            if (!HasSubscribers(entityGuid))
            {
                return;
            }

            _subscriptions[entityGuid].Remove(connectionID);
            if (_subscriptions[entityGuid].Count == 0)
            {
                _subscriptions.Remove(entityGuid);
            }
        }

        [NotNull]
        [Pure]
        internal List<Guid> GetSubscribers([NotNull] Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return GetSubscribers(entity.Guid);
        }

        [NotNull]
        [Pure]
        internal List<Guid> GetSubscribers(Guid entityGuid)
        {
            var subscribers = new List<Guid>();

            if (HasSubscribers(entityGuid))
            {
                subscribers.AddRange(_subscriptions[entityGuid]);
            }

            return subscribers;
        }

        [Pure]
        internal bool HasSubscribers(Entity entity)
        {
            return entity != null && HasSubscribers(entity.Guid);
        }

        [Pure]
        internal bool HasSubscribers(Guid entityGuid)
        {
            if (!_subscriptions.ContainsKey(entityGuid))
            {
                return false;
            }

            List<Guid> subList = _subscriptions[entityGuid];
            if (subList.Count != 0)
            {
                return true;
            }
            _subscriptions.Remove(entityGuid);
            return false;
        }

        internal void SubscribeToEntity([NotNull] Entity entity, Guid connectionID)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            SubscribeToEntity(entity.Guid, connectionID);
        }

        internal void SubscribeToEntity(Guid entityGuid, Guid connectionID)
        {
            if (entityGuid == Guid.Empty)
            {
                throw new ArgumentException("Cannot subscribe to an invalid entity.", nameof(entityGuid));
            }

            if (HasSubscribers(entityGuid))
            {
                _subscriptions[entityGuid].Add(connectionID);
            }
            else
            {
                _subscriptions.Add(entityGuid,
                                   new List<Guid>
                                   {
                                       connectionID
                                   });
            }
        }

        internal void UnsubscribeToEntity([NotNull] Entity entity, Guid connectionID)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            UnsubscribeToEntity(entity.Guid, connectionID);
        }

        internal void Initialize(IEnumerable<StarSystem> systems)
        {
            foreach (StarSystem system in systems)
            {
                AddSystem(system);
            }
        }

        internal void Process(Game game)
        {
            foreach (var(entityGuid, changes) in _entityChanges)
            {
                foreach (Guid subscriber in GetSubscribers(entityGuid))
                {
                    game.MessagePump.EnqueueOutgoingMessage(subscriber, SerializationManager.Export(game, changes));
                }
            }
        }

        internal void AddSystem(StarSystem system)
        {
            system.SystemManager.EntityCreated += EntityCreated;
            system.SystemManager.EntityMoved += EntityMoved;

            foreach (Entity entity in system.SystemManager.Entities)
            {
                AddEntity(entity);
            }
        }

        internal void RemoveSystem(StarSystem system)
        {
            system.SystemManager.EntityCreated -= EntityCreated;
            system.SystemManager.EntityMoved -= EntityMoved;
        }
        #endregion

        #region Other Members
        private void AddEntity(Entity entity)
        {
            entity.EntityDestroyed += Entity_EntityDestroyed;
            entity.DataBlobSet += Entity_DataBlobSet;
            entity.DataBlobRemoving += Entity_DataBlobRemoving;

            foreach (BaseDataBlob dataBlob in entity.DataBlobs)
            {
                dataBlob.PropertyChanged += DataBlob_PropertyChanged;
                dataBlob.SubCollectionChanged += DataBlob_SubCollectionChanged;
            }
        }

        private void AddChangeEvent(EntityChangeEvent changeEvent)
        {
            if (_entityChanges.ContainsKey(changeEvent.EntityGuid))
            {
                _entityChanges[changeEvent.EntityGuid].Add(changeEvent);
            }
            else
            {
                _entityChanges.Add(changeEvent.EntityGuid,
                                   new List<EntityChangeEvent>
                                   {
                                       changeEvent
                                   });
            }
            _entityEvents.Add(changeEvent);
        }
        #endregion

        #region EventHandlers
        private void EntityCreated(object sender, EntityEventArgs args)
        {
            var entityManager = (EntityManager)sender;
            Entity entity = entityManager.GetLocalEntityByGuid(args.EntityGuid);

            AddEntity(entity);
        }

        private void EntityMoved(object sender, EntityEventArgs args)
        {
            //var entityManager = (EntityManager)sender;
            AddChangeEvent(new EntityChangeEvent(args.Type, args.EntityGuid));
        }

        private void Entity_EntityDestroyed(object sender, EntityEventArgs args)
        {
            //var entity = (Entity)sender;
            AddChangeEvent(new EntityChangeEvent(args.Type, args.EntityGuid));
        }

        private void Entity_DataBlobRemoving(object sender, EntityEventArgs args)
        {
            var entity = (Entity)sender;
            var dataBlob = entity.GetDataBlob<BaseDataBlob>(args.DataBlobTypeIndex);

            dataBlob.PropertyChanged -= DataBlob_PropertyChanged;
            dataBlob.SubCollectionChanged -= DataBlob_SubCollectionChanged;

            AddChangeEvent(new EntityChangeEvent(args.Type, args.EntityGuid, null, args.DataBlobTypeIndex));
        }

        private void Entity_DataBlobSet(object sender, EntityEventArgs args)
        {
            var entity = (Entity)sender;
            var dataBlob = entity.GetDataBlob<BaseDataBlob>(args.DataBlobTypeIndex);

            dataBlob.PropertyChanged += DataBlob_PropertyChanged;
            dataBlob.SubCollectionChanged += DataBlob_SubCollectionChanged;

            AddChangeEvent(new EntityChangeEvent(args.Type, args.EntityGuid, null, args.DataBlobTypeIndex));
        }

        private void DataBlob_SubCollectionChanged(object sender, SubCollectionChangedEventArgs subCollectionChangedEventArgs)
        {
            var dataBlob = (BaseDataBlob)sender;

            var changeEvent = new EntityChangeEvent(EntityChangeType.EntityDataBlobSubCollectionChanged, dataBlob.OwningEntity.Guid, subCollectionChangedEventArgs.CollectionPropertyName, subCollectionChangedEventArgs);
            AddChangeEvent(changeEvent);
        }

        private void DataBlob_PropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var dataBlob = (BaseDataBlob)sender;
            object newValue = dataBlob.GetType().GetProperty(propertyChangedEventArgs.PropertyName).GetValue(dataBlob);

            var changeEvent = new EntityChangeEvent(EntityChangeType.EntityDataBlobPropertyChanged, dataBlob.OwningEntity.Guid, propertyChangedEventArgs.PropertyName, newValue);
            AddChangeEvent(changeEvent);
        }
        #endregion
    }
}
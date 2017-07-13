#region Copyright/License
/* 
 *Copyright© 2017 Daniel Phelps
    This file is part of Pulsar4x.

    Pulsar4x is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pulsar4x is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using static Pulsar4X.ECSLib.EntityChangeEvent;

namespace Pulsar4X.ECSLib
{
    [DebuggerDisplay("{" + nameof(PropertyName) + "}")]
    public class EntityChangeEvent
    {
        #region Types
        public enum EntityChangeType
        {
            EntityCreated, // Entity was freshly created.
            EntityDestroyed, // Entity has been destroyed and is no longer valid.
            EntityMovedIn, // Entity moved into an EntityManager from another.
            EntityMovedOut, // Entity moved out of an EntityManager to another.
            EntityDataBlobSet, // Entity has a new DataBlob set.
            EntityDataBlobRemoved, // Entity had a DataBlob removed.
            EntityDataBlobPropertyChanged, // DataBlob on an entity has changed data.
            EntityDataBlobSubCollectionChanged // DataBlob on an entity had a SubCollection change.
        }
        #endregion

        #region Fields
        public object Data;
        public Guid EntityGuid;
        public string PropertyName;

        public EntityChangeType Type;
        #endregion

        #region Constructors
        public EntityChangeEvent(EntityChangeType type, Guid entityGuid, string propertyName = null, object data = null)
        {
            Type = type;
            EntityGuid = entityGuid;
            PropertyName = propertyName;
            Data = data;
        }
        #endregion
    }

    internal class EntityChangeProcessor
    {
        #region Fields
        private readonly Dictionary<Guid, List<EntityChangeEvent>> _entityChanges = new Dictionary<Guid, List<EntityChangeEvent>>();
        private readonly List<EntityChangeEvent> _entityEvents = new List<EntityChangeEvent>();
        private readonly EntitySubscriptionManager _subscriptionManager = new EntitySubscriptionManager();
        #endregion
        
        #region Internal Methods
        internal void Initialize(IEnumerable<StarSystem> systems)
        {
            foreach (StarSystem system in systems)
            {
                AddSystem(system);
            }
        }

        internal void Process()
        {
            foreach (var(entityGuid, changes) in _entityChanges)
            {
                foreach (Guid subscriber in _subscriptionManager.GetSubscribers(entityGuid))
                {
                    // TODO: Use Actual MessagePump
                    MessagePump mp;
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
                _entityChanges.Add(changeEvent.EntityGuid, new List<EntityChangeEvent>{changeEvent});
            }
            _entityEvents.Add(changeEvent);
        }

        public void ClearEvents()
        {
            _entityChanges.Clear();
            _entityEvents.Clear();
        }

        #region EntityEventHandlers
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
        #endregion EntityEventHandlers
    }
}
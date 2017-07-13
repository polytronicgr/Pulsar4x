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

namespace Pulsar4X.ECSLib
{
    internal class EntitySubscriptionManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// Key == EntityID
        /// Value == List of ConnectionID's subscribed.
        private readonly Dictionary<Guid, List<Guid>> _subscriptions = new Dictionary<Guid, List<Guid>>();

        [NotNull]
        [Pure]
        internal List<Guid> GetSubscribers([NotNull]Entity entity)
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

        internal bool HasSubscribers(Entity entity) => entity != null && HasSubscribers(entity.Guid);
        internal bool HasSubscribers(Guid entityGuid)
        {
            if (_subscriptions.ContainsKey(entityGuid))
            {
                List<Guid> subList = _subscriptions[entityGuid];
                if (subList.Count != 0)
                {
                    return true;
                }
                _subscriptions.Remove(entityGuid);
                return false;
            }
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
                _subscriptions.Add(entityGuid, new List<Guid>{ connectionID });
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
        private void UnsubscribeToEntity(Guid entityGuid, Guid connectionID)
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
    }
}

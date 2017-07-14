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
using System.Diagnostics;

namespace Pulsar4X.ECSLib
{
    [DebuggerDisplay("{" + nameof(PropertyName) + "}")]
    public class EntityChangeEvent
    {
        #region Required Types
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
}
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

using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class OwnedDB : BaseDataBlob
    {
        #region Fields
        private Entity _entityOwner = Entity.InvalidEntity;
        private Entity _objectOwner = Entity.InvalidEntity;
        #endregion

        #region Properties
        [JsonProperty]
        public Entity EntityOwner
        {
            get { return _entityOwner; }
            set
            {
                SetField(ref _entityOwner, value);
                ;
            }
        }

        [JsonProperty]
        public Entity ObjectOwner
        {
            get { return _objectOwner; }
            set
            {
                SetField(ref _objectOwner, value);
                ;
            }
        }
        #endregion

        #region Constructors
        // Json Constructor
        public OwnedDB() { }

        public OwnedDB(Entity ownerFaction) : this(ownerFaction, ownerFaction) { }

        public OwnedDB(Entity entityOwner, Entity objectOwner)
        {
            EntityOwner = entityOwner;
            ObjectOwner = objectOwner;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new OwnedDB(EntityOwner, ObjectOwner);
        #endregion
    }
}
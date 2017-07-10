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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class FactionSystemKenDB : BaseDataBlob
    {
        #region Fields
        private ObservableDictionary<Entity, List<Entity>> _entitiesKnownByFaction;
        #endregion

        #region Properties
        [JsonProperty]
        public ObservableDictionary<Entity, List<Entity>> EntitiesKnownByFaction
        {
            get { return _entitiesKnownByFaction; }
            set
            {
                SetField(ref _entitiesKnownByFaction, value);
                EntitiesKnownByFaction.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(EntitiesKnownByFaction), args);
            }
        }
        #endregion

        #region Constructors
        public FactionSystemKenDB() { EntitiesKnownByFaction = new ObservableDictionary<Entity, List<Entity>>(); }

        public FactionSystemKenDB(FactionSystemKenDB db) { EntitiesKnownByFaction = new ObservableDictionary<Entity, List<Entity>>(db.EntitiesKnownByFaction); }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new FactionSystemKenDB(this);
        #endregion
    }
}
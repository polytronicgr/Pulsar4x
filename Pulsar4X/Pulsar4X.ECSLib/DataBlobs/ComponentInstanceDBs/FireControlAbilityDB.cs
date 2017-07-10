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

namespace Pulsar4X.ECSLib
{
    public class FireControlInstanceAbilityDB : BaseDataBlob
    {
        #region Fields
        private List<Entity> _assignedWeapons = new List<Entity>();
        private bool _isEngaging;
        private Entity _target;
        #endregion

        #region Properties
        public Entity Target { get { return _target; } set { SetField(ref _target, value); } }

        public List<Entity> AssignedWeapons { get { return _assignedWeapons; } set { SetField(ref _assignedWeapons, value); } }

        public bool IsEngaging { get { return _isEngaging; } set { SetField(ref _isEngaging, value); } }
        #endregion

        #region Constructors
        public FireControlInstanceAbilityDB() { }

        public FireControlInstanceAbilityDB(FireControlInstanceAbilityDB db)
        {
            Target = db.Target;
            AssignedWeapons = new List<Entity>(db.AssignedWeapons);
            IsEngaging = db.IsEngaging;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new FireControlInstanceAbilityDB(this);
        #endregion
    }
}
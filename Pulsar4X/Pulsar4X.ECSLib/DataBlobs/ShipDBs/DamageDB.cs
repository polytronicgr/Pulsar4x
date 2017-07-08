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
namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Information on damage done to a ship and its Damage Control
    /// </summary>
    public class DamageDB : BaseDataBlob
    {
        private int _damageControlRating;

        public int DamageControlRating { get { return _damageControlRating; } internal set { SetField(ref _damageControlRating, value); } }

        public DamageDB()
        {
        }

        public DamageDB(DamageDB damageDB)
        {
            DamageControlRating = damageDB.DamageControlRating;
        }

        public override object Clone()
        {
            return new DamageDB(this);
        }
    }
}
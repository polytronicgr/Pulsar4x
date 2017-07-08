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
    /// Contains info on the Ships missile weapons, including Fire control and Magazine storage.
    /// </summary>
    public class MissileWeaponsDB : BaseDataBlob
    {
        private int _maximumMagazineCapicity;
        private int _usedMagazineCapicity;

        public int MaximumMagazineCapicity { get { return _maximumMagazineCapicity; } set { SetField(ref _maximumMagazineCapicity, value); } } // in MSP
        public int UsedMagazineCapicity { get { return _usedMagazineCapicity; } set { SetField(ref _usedMagazineCapicity, value); } }

        public MissileWeaponsDB()
        {
        }

        public MissileWeaponsDB(MissileWeaponsDB missleWeaponDB)
        {
            MaximumMagazineCapicity = missleWeaponDB.MaximumMagazineCapicity;
            UsedMagazineCapicity = missleWeaponDB.UsedMagazineCapicity;
        }

        public override object Clone()
        {
            return new MissileWeaponsDB(this);
        }
    }
}
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
    /// Contains info on the ships ability to transort troups.
    /// </summary>
    public class TroopTransportDB : BaseDataBlob
    {
        private int _normalCapicity;
        private int _cryoCapicity;
        private int _normalCombatDropCapicity;
        private int _cryoCombatDropCapicity;

        public int TotalCapicity => CryoCapicity + NormalCapicity;

        public int NormalCapicity { get { return _normalCapicity; } set { SetField(ref _normalCapicity, value); } } // from normal Troop Transport Bays
        public int CryoCapicity { get { return _cryoCapicity; } set { SetField(ref _cryoCapicity, value); } } // From Cryo Troop Transport Bays

        public int TotalCombatDropCapicity => NormalCombatDropCapicity + CryoCombatDropCapicity;

        public int NormalCombatDropCapicity { get { return _normalCombatDropCapicity; } set { SetField(ref _normalCombatDropCapicity, value); } }
        public int CryoCombatDropCapicity { get { return _cryoCombatDropCapicity; } set { SetField(ref _cryoCombatDropCapicity, value); } }

        public TroopTransportDB()
        {
        }

        public TroopTransportDB(TroopTransportDB troopTransportDB)
        {
            NormalCapicity = troopTransportDB.NormalCapicity;
            CryoCapicity = troopTransportDB.CryoCapicity;
            NormalCombatDropCapicity = troopTransportDB.NormalCombatDropCapicity;
            CryoCombatDropCapicity = troopTransportDB.CryoCombatDropCapicity;
        }

        public override object Clone()
        {
            return new TroopTransportDB(this);
        }
    }
}
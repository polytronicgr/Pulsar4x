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
    /// Contains info on a ships beam weapons, including Firecontrol and turret(s).
    /// </summary>
    public class BeamWeaponsDB : BaseDataBlob
    {
        #region Fields
        private int _maxDamage;
        private int _maxRange;
        private int _maxTrackingSpeed;
        private int _numBeamWeapons;
        private int _numFireControls;
        private int _totalDamage;
        #endregion

        #region Properties
        public int NumFireControls { get { return _numFireControls; } set { SetField(ref _numFireControls, value); } }
        public int NumBeamWeapons { get { return _numBeamWeapons; } set { SetField(ref _numBeamWeapons, value); } }
        public int TotalDamage { get { return _totalDamage; } set { SetField(ref _totalDamage, value); } }
        public int MaxDamage { get { return _maxDamage; } set { SetField(ref _maxDamage, value); } }
        public int MaxRange { get { return _maxRange; } set { SetField(ref _maxRange, value); } }
        public int MaxTrackingSpeed { get { return _maxTrackingSpeed; } set { SetField(ref _maxTrackingSpeed, value); } }
        #endregion

        #region Constructors
        public BeamWeaponsDB() { }

        public BeamWeaponsDB(BeamWeaponsDB beamWeaponsDB) { }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new BeamWeaponsDB(this);
        #endregion
    }
}
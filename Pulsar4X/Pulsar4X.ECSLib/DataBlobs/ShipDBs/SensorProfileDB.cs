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
    /// Info on how this ship looks to sensors.
    /// </summary>
    public class SensorProfileDB : BaseDataBlob
    {
        #region Fields
        private int _emSignature;
        private int _thermalSignature;
        private int _totalCrossSection;
        #endregion

        #region Properties
        public int TotalCrossSection { get { return _totalCrossSection; } set { SetField(ref _totalCrossSection, value); } }
        public int ThermalSignature { get { return _thermalSignature; } set { SetField(ref _thermalSignature, value); } }
        public int EMSignature { get { return _emSignature; } set { SetField(ref _emSignature, value); } }
        #endregion

        #region Constructors
        public SensorProfileDB() { }

        public SensorProfileDB(SensorProfileDB sensorProfileDB)
        {
            TotalCrossSection = sensorProfileDB.TotalCrossSection;
            ThermalSignature = sensorProfileDB.ThermalSignature;
            EMSignature = sensorProfileDB.EMSignature;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new SensorProfileDB(this);
        #endregion
    }
}
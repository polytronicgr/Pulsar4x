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
    /// Contains info on a ships maintance supplies/failure rate/etc.
    /// </summary>
    public class MaintenanceDB : BaseDataBlob
    {
        #region Fields
        private double _annualFailureRate;
        private int _currentMSP;
        private double _incrementalFailureRate;
        private int _maintenanceStorageCapicity;
        private int _maximumRepairCost;
        #endregion

        #region Properties
        public int MaintenanceStorageCapicity { get { return _maintenanceStorageCapicity; } set { SetField(ref _maintenanceStorageCapicity, value); } }
        public int CurrentMSP { get { return _currentMSP; } set { SetField(ref _currentMSP, value); } }

        public int MaximumRepairCost { get { return _maximumRepairCost; } set { SetField(ref _maximumRepairCost, value); } }
        public double AnnualFailureRate { get { return _annualFailureRate; } set { SetField(ref _annualFailureRate, value); } }
        public double IncrementalFailureRate { get { return _incrementalFailureRate; } set { SetField(ref _incrementalFailureRate, value); } }
        #endregion

        #region Constructors
        public MaintenanceDB() { }

        public MaintenanceDB(MaintenanceDB maintenanceDB)
        {
            MaintenanceStorageCapicity = maintenanceDB.MaintenanceStorageCapicity;
            CurrentMSP = maintenanceDB.CurrentMSP;
            MaximumRepairCost = maintenanceDB.MaximumRepairCost;
            AnnualFailureRate = maintenanceDB.AnnualFailureRate;
            IncrementalFailureRate = maintenanceDB.IncrementalFailureRate;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new MaintenanceDB(this);
        #endregion
    }
}
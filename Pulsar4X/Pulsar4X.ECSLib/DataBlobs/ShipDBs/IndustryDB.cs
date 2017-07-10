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
    /// Contains info on the industral capabilities of a ship.
    /// </summary>
    public class IndustryDB : BaseDataBlob
    {
        #region Fields
        private double _fuelHarvestingRate;
        private double _jumpGateConstructionRate;
        private double _miningRate;
        private double _salvageRate;
        private double _terraformingRate;
        #endregion

        #region Properties
        public double MiningRate { get { return _miningRate; } set { SetField(ref _miningRate, value); } }
        public double FuelHarvestingRate { get { return _fuelHarvestingRate; } set { SetField(ref _fuelHarvestingRate, value); } }
        public double SalvageRate { get { return _salvageRate; } set { SetField(ref _salvageRate, value); } }
        public double TerraformingRate { get { return _terraformingRate; } set { SetField(ref _terraformingRate, value); } }
        public double JumpGateConstructionRate { get { return _jumpGateConstructionRate; } set { SetField(ref _jumpGateConstructionRate, value); } }
        #endregion

        #region Constructors
        public IndustryDB() { }

        public IndustryDB(IndustryDB indusrtyDB)
        {
            MiningRate = indusrtyDB.MiningRate;
            FuelHarvestingRate = indusrtyDB.FuelHarvestingRate;
            SalvageRate = indusrtyDB.SalvageRate;
            TerraformingRate = indusrtyDB.TerraformingRate;
            JumpGateConstructionRate = indusrtyDB.JumpGateConstructionRate;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new IndustryDB(this);
        #endregion
    }
}
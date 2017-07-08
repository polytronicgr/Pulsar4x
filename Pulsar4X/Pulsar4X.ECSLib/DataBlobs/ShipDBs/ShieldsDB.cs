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
    /// Info on the shields in a ship.
    /// </summary>
    public class ShieldsDB : BaseDataBlob
    {
        private int _maximumShieldStrength;
        private int _currentShieldStrength;
        private int _rechargeRate;

        public int MaximumShieldStrength { get { return _maximumShieldStrength; } set { SetField(ref _maximumShieldStrength, value); } }
        public int CurrentShieldStrength { get { return _currentShieldStrength; } set { SetField(ref _currentShieldStrength, value); } }
        public int RechargeRate { get { return _rechargeRate; } set { SetField(ref _rechargeRate, value); } }

        public ShieldsDB()
        {
        }

        public ShieldsDB(ShieldsDB shieldsDB)
        {
            MaximumShieldStrength = shieldsDB.MaximumShieldStrength;
            CurrentShieldStrength = shieldsDB.CurrentShieldStrength;
            RechargeRate = shieldsDB.RechargeRate;
        }

        public override object Clone()
        {
            return new ShieldsDB(this);
        }
    }
}
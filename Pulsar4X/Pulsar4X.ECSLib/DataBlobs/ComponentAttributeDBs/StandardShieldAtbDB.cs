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

using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class StandardShieldAtbDB : BaseDataBlob
    {
        #region Fields
        private int _shieldHP;
        private int _shieldRechargeRate;
        #endregion

        #region Properties
        [JsonProperty]
        public int ShieldHP { get { return _shieldHP; } set { SetField(ref _shieldHP, value); } }

        [JsonProperty]
        public int ShieldRechargeRate { get { return _shieldRechargeRate; } set { SetField(ref _shieldRechargeRate, value); } }
        #endregion

        #region Constructors
        public StandardShieldAtbDB() { }

        public StandardShieldAtbDB(double shieldHP, double shieldRechargeRate) : this((int)shieldHP, (int)shieldRechargeRate) { }

        public StandardShieldAtbDB(int shieldHP, int shieldRechargeRate)
        {
            ShieldHP = shieldHP;
            ShieldRechargeRate = shieldRechargeRate;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new StandardShieldAtbDB(ShieldHP, ShieldRechargeRate);
        #endregion
    }
}
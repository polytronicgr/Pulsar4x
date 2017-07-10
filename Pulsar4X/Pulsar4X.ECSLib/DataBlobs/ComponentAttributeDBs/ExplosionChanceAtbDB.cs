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
    public class ExplosionChanceAtbDB : BaseDataBlob
    {
        #region Fields
        private float _explosionChance;
        private float _explosionDamage;
        #endregion

        #region Properties
        /// <summary>
        /// Chance this component will cause a secondary explosion when hit.
        /// </summary>
        [JsonProperty]
        public float ExplosionChance { get { return _explosionChance; } set { SetField(ref _explosionChance, value); } }

        [JsonProperty]
        public float ExplosionDamage { get { return _explosionDamage; } set { SetField(ref _explosionDamage, value); } }
        #endregion

        #region Constructors
        public ExplosionChanceAtbDB(double explosionChance, double explosionDamage) : this((float)explosionChance, (float)explosionDamage) { }

        [JsonConstructor]
        public ExplosionChanceAtbDB(float explosionChance = 0, float explosionDamage = 0)
        {
            ExplosionChance = explosionChance;
            ExplosionDamage = explosionDamage;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ExplosionChanceAtbDB(ExplosionChance, ExplosionDamage);
        #endregion
    }
}
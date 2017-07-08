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
using System.CodeDom;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public enum BeamWeaponType
    {
        Invalid = 0,
        Gauss,
        HighPoweredMicrowave,
        Laser,
        Meson,
        ParticleBeam,
        PlasmaCarronade,
        Railgun,
    }

    public class BeamWeaponAtbDB : BaseDataBlob
    {
        private int _maxRange;
        private int _baseDamage;
        private float _accuracyMultiplier;
        private float _powerRequired;
        private float _powerRechargeRate;
        private int _shotsPerVolley;
        private BeamWeaponType _weaponType;

        /// <summary>
        /// Max range of this weapon. Measured in KM.
        /// </summary>
        [JsonProperty]
        public int MaxRange { get { return _maxRange; } internal set { SetField(ref _maxRange, value); } }

        /// <summary>
        /// Damage of this weapon at point blank range - drops off over longer distances
        /// </summary>
        [JsonProperty]
        public int BaseDamage { get { return _baseDamage; } internal set { SetField(ref _baseDamage, value); } }

        /// <summary>
        /// AccuracyPenalty. ChanceToHit = (Speed/Range/other penalties) * AccuracyMultiplier
        /// </summary>
        [JsonProperty]
        public float AccuracyMultiplier { get { return _accuracyMultiplier; } internal set { SetField(ref _accuracyMultiplier, value); } }

        /// <summary>
        /// Power required for this weapon to fire.
        /// </summary>
        [JsonProperty]
        public float PowerRequired { get { return _powerRequired; } internal set { SetField(ref _powerRequired, value); } }

        /// <summary>
        /// Power this weapon can charge per second.
        /// </summary>
        [JsonProperty]
        public float PowerRechargeRate { get { return _powerRechargeRate; } internal set { SetField(ref _powerRechargeRate, value); } }

        /// <summary>
        /// Number of shots fired in a single volley.
        /// </summary>
        [JsonProperty]
        public int ShotsPerVolley { get { return _shotsPerVolley; } internal set { SetField(ref _shotsPerVolley, value); } }

        /// <summary>
        /// Type of weapon this beam weapon is.
        /// </summary>
        [JsonProperty]
        public BeamWeaponType WeaponType { get { return _weaponType; } internal set { SetField(ref _weaponType, value); } }

        [JsonConstructor]
        internal BeamWeaponAtbDB() { }

        public BeamWeaponAtbDB(double maxRange, double damageAtMaxRange, double accuracyMultiplier, double powerRequired, double powerRechargeRate, double shotsPerVolley, BeamWeaponType weaponType) 
            : this((int) maxRange, (int) damageAtMaxRange, (float) accuracyMultiplier, (float) powerRequired, (float) powerRechargeRate, (int) shotsPerVolley, weaponType) { }

        public BeamWeaponAtbDB(int maxRange, int baseDamage, float accuracyMultiplier, float powerRequired, float powerRechargeRate, int shotsPerVolley, BeamWeaponType weaponType)
        {
            MaxRange = maxRange;
            BaseDamage = baseDamage;
            AccuracyMultiplier = accuracyMultiplier;
            PowerRequired = powerRequired;
            PowerRechargeRate = powerRechargeRate;
            ShotsPerVolley = shotsPerVolley;
            WeaponType = weaponType;
        }

        public override object Clone() => new BeamWeaponAtbDB(MaxRange, BaseDamage, AccuracyMultiplier, PowerRequired, PowerRechargeRate, ShotsPerVolley, WeaponType);
    }
}
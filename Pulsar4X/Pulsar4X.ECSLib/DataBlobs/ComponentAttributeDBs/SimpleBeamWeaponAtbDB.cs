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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsar4X.ECSLib
{
    public class SimpleBeamWeaponAtbDB : BaseDataBlob
    {
        private double _maxRange;
        private double _damageAmount;
        private double _reloadRate;

        [JsonProperty]
        public double MaxRange { get { return _maxRange; } internal set { SetField(ref _maxRange, value); } }

        [JsonProperty]
        public double DamageAmount { get { return _damageAmount; } internal set { SetField(ref _damageAmount, value); } }

        [JsonProperty]
        public double ReloadRate { get { return _reloadRate; } internal set { SetField(ref _reloadRate, value); } }

        public SimpleBeamWeaponAtbDB() { }

        public SimpleBeamWeaponAtbDB(double maxRange, double damageAmount, double reloadRate)
        {
            MaxRange = maxRange;
            DamageAmount = damageAmount;
            ReloadRate = reloadRate;
        }

        public SimpleBeamWeaponAtbDB(SimpleBeamWeaponAtbDB db)
        {
            MaxRange = db.MaxRange;
            DamageAmount = db.DamageAmount;
            ReloadRate = db.ReloadRate;
        }

        public override object Clone()
        {
            return new SimpleBeamWeaponAtbDB(this);
        }
    }
}

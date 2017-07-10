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
    public class LifeSupportAtbDB : BaseDataBlob
    {
        #region Fields
        private int _lifeSupportCapacity;
        #endregion

        #region Properties
        [JsonProperty]
        public int LifeSupportCapacity { get { return _lifeSupportCapacity; } set { SetField(ref _lifeSupportCapacity, value); } }
        #endregion

        #region Constructors
        public LifeSupportAtbDB() { }

        public LifeSupportAtbDB(double lifeSupportCapacity) : this((int)lifeSupportCapacity) { }

        public LifeSupportAtbDB(int lifeSupportCapacity) { LifeSupportCapacity = lifeSupportCapacity; }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new LifeSupportAtbDB(LifeSupportCapacity);
        #endregion
    }
}
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
    //
    //@summary Provides the ability to support a number of colonists in a population.  Dependent on the colony cost.
    //
    public class PopulationSupportAtbDB : BaseDataBlob
    {
        private int _populationCapacity;

        [JsonProperty]
        // Population capacity at 1.0 colony cost
        // Infrastructure = 10000
        public int PopulationCapacity { get { return _populationCapacity; } internal set { SetField(ref _populationCapacity, value); } }

        public PopulationSupportAtbDB() { }

        public PopulationSupportAtbDB(double popSupportCapacity) : this((int)popSupportCapacity) { }

        public PopulationSupportAtbDB(int popSupportCapacity)
        {
            PopulationCapacity = popSupportCapacity;
        }

        public override object Clone()
        {
            return new PopulationSupportAtbDB(PopulationCapacity);
        }
    }
}

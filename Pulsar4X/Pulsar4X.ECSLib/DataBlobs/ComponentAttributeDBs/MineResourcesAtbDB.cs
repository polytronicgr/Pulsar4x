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
using System;
using System.Collections.Generic;

namespace Pulsar4X.ECSLib
{
    public class MineResourcesAtbDB : BaseDataBlob
    {
        private Dictionary<Guid, int> _resourcesPerEconTick;

        public Dictionary<Guid, int> ResourcesPerEconTick { get { return _resourcesPerEconTick; } internal set { SetField(ref _resourcesPerEconTick, value); } }

        public MineResourcesAtbDB() { }

        /// <summary>
        /// Component factory constructor.
        /// </summary>
        /// <param name="resources">values will be cast to ints!</param>
        public MineResourcesAtbDB(Dictionary<Guid,double> resources)
        {
            ResourcesPerEconTick = new Dictionary<Guid, int>();
            foreach (var kvp in resources)
            {
                ResourcesPerEconTick.Add(kvp.Key,(int)kvp.Value);
            }
        }

        public MineResourcesAtbDB(MineResourcesAtbDB db)
        {
            ResourcesPerEconTick = db.ResourcesPerEconTick;
        }

        public override object Clone()
        {
            return new MineResourcesAtbDB(this);
        }
    }
}
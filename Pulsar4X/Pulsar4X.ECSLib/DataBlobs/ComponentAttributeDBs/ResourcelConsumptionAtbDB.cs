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
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class ResourceConsumptionAtbDB : BaseDataBlob
    {
        #region Properties
        [JsonProperty]
        public ObservableDictionary<Guid, int> MaxUsage { get; set; } = new ObservableDictionary<Guid, int>();
        [JsonProperty]
        public ObservableDictionary<Guid, int> MinUsage { get; set; } = new ObservableDictionary<Guid, int>();
        #endregion

        #region Constructors
        public ResourceConsumptionAtbDB()
        {
            MaxUsage.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(MaxUsage), args);
            MinUsage.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(MinUsage), args);
        }

        public ResourceConsumptionAtbDB(Guid resourcetype, double maxUsage, double minUsage) : this()
        {
            MaxUsage.Add(resourcetype, (int)maxUsage);
            MinUsage.Add(resourcetype, (int)minUsage);
        }

        public ResourceConsumptionAtbDB(Dictionary<Guid, double> maxUsage, Dictionary<Guid, double> minUsage)
        {
            foreach (KeyValuePair<Guid, double> kvp in maxUsage)
            {
                MaxUsage.Add(kvp.Key, (int)kvp.Value);
            }
            foreach (KeyValuePair<Guid, double> kvp in minUsage)
            {
                MinUsage.Add(kvp.Key, (int)kvp.Value);
            }
        }

        public ResourceConsumptionAtbDB(IDictionary<Guid, int> maxUsage, IDictionary<Guid, int> minUsage) : this()
        {
            MaxUsage.AddRange(maxUsage);
            MinUsage.AddRange(minUsage);
        }

        public ResourceConsumptionAtbDB(ResourceConsumptionAtbDB db) : this(db.MaxUsage, db.MinUsage) { }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ResourceConsumptionAtbDB(this);
        #endregion
    }
}
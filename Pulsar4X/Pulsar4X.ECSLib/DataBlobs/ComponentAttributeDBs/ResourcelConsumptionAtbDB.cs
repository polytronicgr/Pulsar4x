using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pulsar4X.ECSLib
{
    public class ResourceConsumptionAtbDB : BaseDataBlob
    {
        [JsonProperty]
        public ObservableDictionary<Guid, int> MaxUsage { get; internal set; } = new ObservableDictionary<Guid, int>();
        [JsonProperty]
        public ObservableDictionary<Guid, int> MinUsage { get; internal set; } = new ObservableDictionary<Guid, int>();

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
            foreach (var kvp in maxUsage)
            {
                MaxUsage.Add(kvp.Key, (int)kvp.Value);
            }
            foreach (var kvp in minUsage)
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

        public override object Clone()
        {
            return new ResourceConsumptionAtbDB(this);
        }
    }
}
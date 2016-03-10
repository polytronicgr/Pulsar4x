using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Contains the total industrial capacity of this entity, and its current jobs.
    /// </summary>
    public class IndustryDB : BaseDataBlob
    {
        [JsonProperty]
        internal Dictionary<IndustryType, float> industryRates { get; set; } = new Dictionary<IndustryType, float>();
        public IReadOnlyDictionary<IndustryType, float> IndustryRates => industryRates;

        [JsonProperty]
        internal Dictionary<Guid, float> industryMultipliers { get; set; } = new Dictionary<Guid, float>();
        public IReadOnlyDictionary<Guid, float> IndustryMultipliers => industryMultipliers;

        [JsonProperty]
        internal Dictionary<IndustryType, LinkedList<IndustryJob>> industryJobs { get; set; } = new Dictionary<IndustryType, LinkedList<IndustryJob>>();
        // ReSharper disable once SuspiciousTypeConversion.Global
        public IReadOnlyDictionary<IndustryType, IReadOnlyCollection<IndustryJob>> IndustryJobs => (IReadOnlyDictionary<IndustryType, IReadOnlyCollection<IndustryJob>>)industryJobs;

        [JsonProperty]
        public bool CanPullFromHost { get; internal set; }

        public IndustryDB()
        {
            foreach (IndustryType industryType in Enum.GetValues(typeof(IndustryType)))
            {
                industryJobs.Add(industryType, new LinkedList<IndustryJob>());
            }
        }
        
        public override object Clone()
        {
            return new IndustryDB();
        }
    }
}
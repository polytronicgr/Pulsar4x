using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Contains the total industrial capacity of this entity, and its current jobs.
    /// </summary>
    public class IndustryDB : BaseDataBlob
    {
        [JsonProperty]
        internal Dictionary<IndustryType, float> industryRates { get; set; }
        public ReadOnlyDictionary<IndustryType, float> IndustryRates { get; set; }

        [JsonProperty]
        internal List<IndustryJob> industryJobs { get; set; } = new List<IndustryJob>();
        public ReadOnlyCollection<IndustryJob> IndustryJobs => new ReadOnlyCollection<IndustryJob>(industryJobs);  

        public override object Clone()
        {
            return new IndustryDB();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// A single ability can provide multiple types of CP's. Some may even overlap. 
    /// For example, you can have a component that provides 5 Installations CP's, and provides 2 Installations | Ships CP's.
    /// Final result will be 7 Installation CP's, and 2 Ship CP's.
    /// </summary>
    public class IndustryAbilityDB : BaseDataBlob
    {
        [JsonProperty]
        internal Dictionary<IndustryType, int> industryRates { get; set; } = new Dictionary<IndustryType, int>();
        public ReadOnlyDictionary<IndustryType, int> IndustryRates => new ReadOnlyDictionary<IndustryType, int>(industryRates);

        [JsonProperty]
        internal Dictionary<Guid, float> industryMultipliers { get; set; } = new Dictionary<Guid, float>();
        public ReadOnlyDictionary<Guid, float> IndustryMultipliers => new ReadOnlyDictionary<Guid, float>(industryMultipliers);

        [JsonProperty]
        public bool CanPullFromHost { get; internal set; }

        /// <summary>
        /// Casting constructor. Casts from double to int.
        /// </summary>
        public IndustryAbilityDB(IDictionary<IndustryType, double> industryRates, IDictionary<Guid, float> industryMultipliers = null, bool canPullFromHost = false) 
            : this(industryRates.ToDictionary(industryRate => industryRate.Key, industryRate => (int)industryRate.Value), industryMultipliers, canPullFromHost) { }
        
        [JsonConstructor]
        public IndustryAbilityDB(IDictionary<IndustryType, int> industryRates = null, IDictionary<Guid, float> industryMultipliers = null, bool canPullFromHost = false)
        {
            if (industryRates != null)
            {
                this.industryRates = new Dictionary<IndustryType, int>(industryRates);
            }
            if (industryMultipliers != null)
            {
                this.industryMultipliers = new Dictionary<Guid, float>(industryMultipliers);
            }
            CanPullFromHost = canPullFromHost;
        }

        public override object Clone()
        {
            return new IndustryAbilityDB(IndustryRates, IndustryMultipliers);
        }

        /// <summary>
        /// Adds up all construstion points this ability provides for a given type.
        /// </summary>
        public int GetIndustryRate(IndustryType type)
        {
            int totalConstructionPoints = 0;
            foreach (KeyValuePair<IndustryType, int> keyValuePair in industryRates)
            {
                IndustryType entryType = keyValuePair.Key;
                int cp = keyValuePair.Value;

                if ((entryType & type) != 0)
                {
                    totalConstructionPoints += cp;
                }
            }
            return totalConstructionPoints;
        }
    }
}
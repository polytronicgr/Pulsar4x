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
        internal Dictionary<IndustryType, int> constructionPoints { get; set; } = new Dictionary<IndustryType, int>();
        public ReadOnlyDictionary<IndustryType, int> ConstructionPoints => new ReadOnlyDictionary<IndustryType, int>(constructionPoints);

        /// <summary>
        /// Casting constructor. Casts from double to int.
        /// </summary>
        /// <param name="constructionPoints"></param>
        public IndustryAbilityDB(IDictionary<IndustryType, double> constructionPoints) 
            : this(constructionPoints.ToDictionary(constructionPoint => constructionPoint.Key, constructionPoint => (int)constructionPoint.Value)) { }
        
        [JsonConstructor]
        public IndustryAbilityDB(IDictionary<IndustryType, int> constructionPoints = null)
        {
            if (constructionPoints != null)
            {
                this.constructionPoints = new Dictionary<IndustryType, int>(constructionPoints);
            }
        }

        public override object Clone()
        {
            return new IndustryAbilityDB(ConstructionPoints);
        }

        /// <summary>
        /// Adds up all construstion points this ability provides for a given type.
        /// </summary>
        public int GetConstructionPoints(IndustryType type)
        {
            int totalConstructionPoints = 0;
            foreach (KeyValuePair<IndustryType, int> keyValuePair in constructionPoints)
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
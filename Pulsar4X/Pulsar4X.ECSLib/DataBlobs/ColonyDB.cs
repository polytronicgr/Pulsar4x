using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pulsar4X.ECSLib
{
    public class ColonyDB : BaseDataBlob
    {
        /// <summary>
        /// Species Entity and amount
        /// </summary>
        [JsonProperty]
        internal Dictionary<Entity, long> population { get; set; }
        public IReadOnlyDictionary<Entity, long> Population => population;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="population">Species and population number</param>
        [JsonConstructor]
        public ColonyDB(IDictionary<Entity, long> population = null)
        {
            this.population = population == null ? new Dictionary<Entity, long>() : new Dictionary<Entity, long>(population);
        }

        public ColonyDB(Entity species, long populationCount) : this(new Dictionary<Entity, long> {{species, populationCount}}) { }

        public override object Clone()
        {
            return new ColonyDB(population);
        }
    }
}

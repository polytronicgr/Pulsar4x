using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pulsar4X.ECSLib
{
    public class ColonyInfoDB : BaseDataBlob
    {
        /// <summary>
        /// Species Entity and amount
        /// </summary>
        [JsonProperty]
        internal Dictionary<Entity, long> population { get; set; } = new Dictionary<Entity, long>();
        public ReadOnlyDictionary<Entity, long> Population => new ReadOnlyDictionary<Entity, long>(population);

        /// <summary>
        /// Raw Mined minerals. Mines push here, Refinary pulls from here, Construction pulls from here.
        /// </summary>
        [JsonProperty]
        internal Dictionary<Guid, int> mineralStockpile { get; set; } = new Dictionary<Guid, int>();
        public ReadOnlyDictionary<Guid, int> MineralStockpile => new ReadOnlyDictionary<Guid, int>(mineralStockpile);

        /// <summary>
        /// Refined Fuel, or refined minerals if the modder so desires.
        /// Refinary pushes here, Construction pulls from here.
        /// </summary>
        [JsonProperty]
        internal Dictionary<Guid, int> refinedStockpile { get; set; } = new Dictionary<Guid, int>();
        public ReadOnlyDictionary<Guid, int> RefinedStockpile => new ReadOnlyDictionary<Guid, int>(refinedStockpile);

        /// <summary>
        /// constructed parts stockpile.
        /// Construction pulls and pushes from here.
        /// </summary>
        [JsonProperty]
        internal Dictionary<Guid, int> componentStockpile { get; set; } = new Dictionary<Guid, int>();
        public ReadOnlyDictionary<Guid, int> ComponentStockpile => new ReadOnlyDictionary<Guid, int>(componentStockpile);

        /// <summary>
        /// Construction pushes here.
        /// </summary>
        [JsonProperty]
        internal Dictionary<Guid, float> ordnanceStockpile { get; set; } = new Dictionary<Guid, float>();
        public ReadOnlyDictionary<Guid, float> OrdnanceStockpile => new ReadOnlyDictionary<Guid, float>(ordnanceStockpile);

        [JsonProperty]
        public Dictionary<Entity, int> Installations { get; set; } = new Dictionary<Entity, int>();

        [JsonProperty]
        public List<Entity> Scientists { get; set; } = new List<Entity>();

        public ColonyInfoDB() { }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="popCount">Species and population number</param>
        /// <param name="planet"> the planet entity this colony is on</param>
        public ColonyInfoDB(IDictionary<Entity, long> popCount, Entity planet)
        {
            population = new Dictionary<Entity, long>(popCount);
            
            mineralStockpile =  new Dictionary<Guid, int>();
            refinedStockpile = new Dictionary<Guid, int>();
            componentStockpile = new Dictionary<Guid, int>();
            ordnanceStockpile = new Dictionary<Guid, float>();
            Installations = new Dictionary<Entity, int>();
            Scientists = new List<Entity>();
        }

        public ColonyInfoDB(Entity species, long populationCount, Entity planet):this(
            new Dictionary<Entity, long> {{species, populationCount}},
            planet)
        {
        }

        public ColonyInfoDB(ColonyInfoDB colonyInfoDB)
        {
            population = new Dictionary<Entity, long>(colonyInfoDB.Population);
            mineralStockpile = new Dictionary<Guid, int>(colonyInfoDB.mineralStockpile);
            refinedStockpile = new Dictionary<Guid, int>(colonyInfoDB.refinedStockpile);
            componentStockpile = new Dictionary<Guid, int>(colonyInfoDB.componentStockpile);
            ordnanceStockpile = new Dictionary<Guid, float>(colonyInfoDB.ordnanceStockpile);
            Installations = new Dictionary<Entity, int>(colonyInfoDB.Installations);
            Scientists = new List<Entity>(colonyInfoDB.Scientists);
        }

        public override object Clone()
        {
            return new ColonyInfoDB(this);
        }
    }
}

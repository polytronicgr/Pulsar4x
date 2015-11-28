using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class FactionInfoDB : BaseDataBlob
    {
        /// <summary>
        /// The different species that this faction has in its control.
        /// </summary>
        [PublicAPI, JsonProperty]
        public List<Entity> Species { get; internal set; }

        [PublicAPI, JsonProperty]
        public List<Guid> KnownSystems { get; internal set; }

        /// <summary>
        /// Other factions that this faction has had contact with. possibly this should be an contact type of entity/blob?
        /// </summary>
        [PublicAPI, JsonProperty]
        public List<Entity> KnownFactions { get; internal set; }


        [PublicAPI, JsonProperty]
        public List<Entity> Colonies { get; internal set; }

        [PublicAPI, JsonProperty]
        public List<Entity> ShipClasses { get; internal set; }

        [PublicAPI, JsonProperty]
        public JDictionary<Guid, Entity> ComponentDesigns { get; internal set; }
        


        public FactionInfoDB()
            : this(new List<Entity>(), new List<Guid>(), new List<Entity>(), new List<Entity>() )
        {

        }

        public FactionInfoDB(
            List<Entity> species,
            List<Guid> knownSystems,
            List<Entity> colonies,
            List<Entity> shipClasses)
        {
            Species = species;
            KnownSystems = knownSystems;
            Colonies = colonies;
            ShipClasses = shipClasses;
            KnownFactions = new List<Entity>();
            ComponentDesigns = new JDictionary<Guid, Entity>();
        }
        

        public FactionInfoDB(FactionInfoDB factionDB)
        {
            Species = new List<Entity>(factionDB.Species);
            KnownSystems = new List<Guid>(factionDB.KnownSystems);
            KnownFactions = new List<Entity>(factionDB.KnownFactions);
            Colonies = new List<Entity>(factionDB.Colonies);
            ShipClasses = new List<Entity>(factionDB.ShipClasses);
            ComponentDesigns = new JDictionary<Guid, Entity>(factionDB.ComponentDesigns);
        }

        public override object Clone()
        {
            return new FactionInfoDB(this);
        }
    }
}
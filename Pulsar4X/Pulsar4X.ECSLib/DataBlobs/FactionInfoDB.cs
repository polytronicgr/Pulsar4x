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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Pulsar4X.ECSLib
{
    public class FactionInfoDB : BaseDataBlob
    {
        [PublicAPI]
        [JsonProperty]
        public List<Entity> Species { get; internal set; }

        [PublicAPI]
        [JsonProperty]
        public List<Guid> KnownSystems { get; internal set; }

        [PublicAPI]
        public ReadOnlyDictionary<Guid, List<Entity>> KnownJumpPoints => new ReadOnlyDictionary<Guid, List<Entity>>(InternalKnownJumpPoints);
        [JsonProperty]
        internal Dictionary<Guid, List<Entity>> InternalKnownJumpPoints = new Dictionary<Guid, List<Entity>>();

        [PublicAPI]
        [JsonProperty]
        public List<Entity> KnownFactions { get; internal set; }


        [PublicAPI]
        [JsonProperty]
        public List<Entity> Colonies { get; internal set; }

        [PublicAPI]
        [JsonProperty]
        public List<Entity> ShipClasses { get; internal set; }

        [PublicAPI]
        public ReadOnlyDictionary<Guid, Entity> ComponentDesigns => new ReadOnlyDictionary<Guid, Entity>(InternalComponentDesigns);
        [JsonProperty]
        internal Dictionary<Guid, Entity> InternalComponentDesigns = new Dictionary<Guid, Entity>();


        [PublicAPI]
        public ReadOnlyDictionary<Guid, Entity> MissileDesigns => new ReadOnlyDictionary<Guid, Entity>(InternalMissileDesigns);
        [JsonProperty]
        internal Dictionary<Guid, Entity> InternalMissileDesigns = new Dictionary<Guid, Entity>();

        public FactionInfoDB() : this(new List<Entity>(), new List<Guid>(), new List<Entity>(), new List<Entity>() ) { }

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
            InternalComponentDesigns = new Dictionary<Guid, Entity>();
        }
        

        public FactionInfoDB(FactionInfoDB factionDB)
        {
            Species = new List<Entity>(factionDB.Species);
            KnownSystems = new List<Guid>(factionDB.KnownSystems);
            KnownFactions = new List<Entity>(factionDB.KnownFactions);
            Colonies = new List<Entity>(factionDB.Colonies);
            ShipClasses = new List<Entity>(factionDB.ShipClasses);

        }

        public override object Clone()
        {
            return new FactionInfoDB(this);
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            ((Game)context.Context).PostLoad += (sender, args) => { };
        }
    }
}
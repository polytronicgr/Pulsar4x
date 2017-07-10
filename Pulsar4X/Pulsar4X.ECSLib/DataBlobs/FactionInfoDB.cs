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
        public ObservableDictionary<Guid, ObservableCollection<Entity>> KnownJumpPoints => new ObservableDictionary<Guid, ObservableCollection<Entity>>();

        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<Guid, Entity> ComponentDesigns => new ObservableDictionary<Guid, Entity>();

        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<Guid, Entity> MissileDesigns => new ObservableDictionary<Guid, Entity>();

        [PublicAPI]
        [JsonProperty]
        public ObservableCollection<Entity> Species { get; internal set; } = new ObservableCollection<Entity>();

        [PublicAPI]
        [JsonProperty]
        public ObservableCollection<Guid> KnownSystems { get; internal set; } = new ObservableCollection<Guid>();
        
        [PublicAPI]
        [JsonProperty]
        public ObservableCollection<Entity> KnownFactions { get; internal set; } = new ObservableCollection<Entity>();

        [PublicAPI]
        [JsonProperty]
        public ObservableCollection<Entity> Colonies { get; internal set; } = new ObservableCollection<Entity>();

        [PublicAPI]
        [JsonProperty]
        public ObservableCollection<Entity> ShipClasses { get; internal set; } = new ObservableCollection<Entity>();

        public FactionInfoDB()
        {
            KnownJumpPoints.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(KnownJumpPoints), args);
            ComponentDesigns.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ComponentDesigns), args);
            MissileDesigns.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(MissileDesigns), args);
            Species.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(Species), args);
            KnownSystems.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(KnownSystems), args);
            KnownFactions.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(KnownFactions), args);
            Colonies.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(Colonies), args);
            ShipClasses.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ShipClasses), args);
        }
        

        public FactionInfoDB(FactionInfoDB factionDB) : this()
        {
            KnownJumpPoints.Merge(factionDB.KnownJumpPoints);
            ComponentDesigns.Merge(factionDB.ComponentDesigns);
            MissileDesigns.Merge(factionDB.MissileDesigns);

            foreach (Entity value in factionDB.Species)
            {
                Species.Add(value);
            }
            foreach (Guid value in factionDB.KnownSystems)
            {
                KnownSystems.Add(value);
            }
            foreach (Entity value in factionDB.KnownFactions)
            {
                KnownFactions.Add(value);
            }
            foreach (Entity value in factionDB.Colonies)
            {
                Colonies.Add(value);
            }
            foreach (Entity value in factionDB.ShipClasses)
            {
                ShipClasses.Add(value);
            }
        }

        public override object Clone()
        {
            return new FactionInfoDB(this);
        }
    }
}
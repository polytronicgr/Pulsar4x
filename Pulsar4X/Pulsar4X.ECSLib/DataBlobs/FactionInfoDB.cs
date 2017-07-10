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

using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class FactionInfoDB : BaseDataBlob
    {
        #region Fields
        private ObservableCollection<Entity> _colonies;
        private ObservableDictionary<Guid, Entity> _componentDesigns;
        private ObservableCollection<Entity> _knownFactions;
        private ObservableDictionary<Guid, ObservableCollection<Entity>> _knownJumpPoints;
        private ObservableCollection<Guid> _knownSystems;
        private ObservableDictionary<Guid, Entity> _missileDesigns;
        private ObservableCollection<Entity> _shipClasses;
        private ObservableCollection<Entity> _species;
        #endregion

        #region Properties
        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<Guid, ObservableCollection<Entity>> KnownJumpPoints
        {
            get { return _knownJumpPoints; }
            set
            {
                SetField(ref _knownJumpPoints, value);
                KnownJumpPoints.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(_knownJumpPoints), args);
            }
        }

        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<Guid, Entity> ComponentDesigns
        {
            get { return _componentDesigns; }
            set
            {
                SetField(ref _componentDesigns, value);
                ComponentDesigns.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ComponentDesigns), args);
            }
        }

        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<Guid, Entity> MissileDesigns
        {
            get { return _missileDesigns; }
            set
            {
                SetField(ref _missileDesigns, value);
                MissileDesigns.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(MissileDesigns), args);
            }
        }

        [PublicAPI]
        [JsonProperty]
        public ObservableCollection<Entity> Species
        {
            get { return _species; }
            set
            {
                SetField(ref _species, value);
                Species.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(Species), args);
            }
        }

        [PublicAPI]
        [JsonProperty]
        public ObservableCollection<Guid> KnownSystems
        {
            get { return _knownSystems; }
            set
            {
                SetField(ref _knownSystems, value);
                KnownSystems.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(KnownSystems), args);
            }
        }

        [PublicAPI]
        [JsonProperty]
        public ObservableCollection<Entity> KnownFactions
        {
            get { return _knownFactions; }
            set
            {
                SetField(ref _knownFactions, value);
                KnownFactions.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(KnownFactions), args);
            }
        }

        [PublicAPI]
        [JsonProperty]
        public ObservableCollection<Entity> Colonies
        {
            get { return _colonies; }
            set
            {
                SetField(ref _colonies, value);
                Colonies.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(Colonies), args);
            }
        }

        [PublicAPI]
        [JsonProperty]
        public ObservableCollection<Entity> ShipClasses
        {
            get { return _shipClasses; }
            set
            {
                SetField(ref _shipClasses, value);
                ShipClasses.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ShipClasses), args);
            }
        }
        #endregion

        #region Constructors
        public FactionInfoDB()
        {
            KnownJumpPoints = new ObservableDictionary<Guid, ObservableCollection<Entity>>();
            ComponentDesigns = new ObservableDictionary<Guid, Entity>();
            MissileDesigns = new ObservableDictionary<Guid, Entity>();
            Species = new ObservableCollection<Entity>();
            KnownSystems = new ObservableCollection<Guid>();
            KnownFactions = new ObservableCollection<Entity>();
            Colonies = new ObservableCollection<Entity>();
            ShipClasses = new ObservableCollection<Entity>();
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
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new FactionInfoDB(this);
        #endregion
    }
}
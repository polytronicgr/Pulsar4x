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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class ColonyInfoDB : BaseDataBlob
    {
        #region Fields
        private ObservableDictionary<Entity, double> _colonyComponentDictionary;
        private ObservableDictionary<Guid, int> _componentStockpile;
        private ObservableCollection<Entity> _fighterStockpile;
        private ObservableDictionary<Guid, float> _ordinanceStockpile;
        private Entity _planetEntity = Entity.InvalidEntity;
        private ObservableDictionary<Entity, long> _population = new ObservableDictionary<Entity, long>();
        private ObservableCollection<Entity> _scientists;
        #endregion

        #region Properties
        /// <summary>
        /// the parent planet
        /// </summary>
        [JsonProperty]
        public Entity PlanetEntity { get { return _planetEntity; } set { SetField(ref _planetEntity, value); } }

        /// <summary>
        /// Species Entity and amount
        /// </summary>
        [JsonProperty]
        public ObservableDictionary<Entity, long> Population
        {
            get { return _population; }
            set
            {
                SetField(ref _population, value);
                Population.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(Population), args);
            }
        }


        /// <summary>
        /// constructed parts stockpile.
        /// Construction pulls and pushes from here.
        /// </summary>
        [JsonProperty]
        public ObservableDictionary<Guid, int> ComponentStockpile
        {
            get { return _componentStockpile; }
            set
            {
                SetField(ref _componentStockpile, value);
                ComponentStockpile.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ComponentStockpile), args);
            }
        }

        /// <summary>
        /// Construction pushes here.
        /// </summary>
        [JsonProperty]
        public ObservableDictionary<Guid, float> OrdinanceStockpile
        {
            get { return _ordinanceStockpile; }
            set
            {
                SetField(ref _ordinanceStockpile, value);
                OrdinanceStockpile.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(OrdinanceStockpile), args);
            }
        }

        /// <summary>
        /// Construction *adds* to this list. damaged and partialy constructed fighters will go here too, but shouldnt launch.
        /// </summary>
        [JsonProperty]
        public ObservableCollection<Entity> FighterStockpile
        {
            get { return _fighterStockpile; }
            set
            {
                SetField(ref _fighterStockpile, value);
                FighterStockpile.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(FighterStockpile), args);
            }
        }

        [JsonProperty]
        public ObservableCollection<Entity> Scientists
        {
            get { return _scientists; }
            set
            {
                SetField(ref _scientists, value);
                Scientists.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(Scientists), args);
            }
        }

        /// <summary>
        /// Installation list for damage calculations. Colony installations are considered components.
        /// </summary>
        public ObservableDictionary<Entity, double> ColonyComponentDictionary
        {
            get { return _colonyComponentDictionary; }
            set
            {
                SetField(ref _colonyComponentDictionary, value);
                ColonyComponentDictionary.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ColonyComponentDictionary), args);
            }
        }
        #endregion

        #region Constructors
        public ColonyInfoDB()
        {
            Population = new ObservableDictionary<Entity, long>();
            ComponentStockpile = new ObservableDictionary<Guid, int>();
            OrdinanceStockpile = new ObservableDictionary<Guid, float>();
            FighterStockpile = new ObservableCollection<Entity>();
            Scientists = new ObservableCollection<Entity>();
            ColonyComponentDictionary = new ObservableDictionary<Entity, double>();
        }

        /// <summary>
        /// </summary>
        /// <param name="popCount">Species and population number</param>
        /// <param name="planet"> the planet entity this colony is on</param>
        public ColonyInfoDB(IDictionary<Entity, long> popCount, Entity planet) : this()
        {
            Population.Merge(popCount);
            PlanetEntity = planet;
        }

        public ColonyInfoDB(Entity species, long populationCount, Entity planet) : this(new ObservableDictionary<Entity, long>
                                                                                        {
                                                                                            {species, populationCount}
                                                                                        },
                                                                                        planet) { }

        public ColonyInfoDB(ColonyInfoDB colonyInfoDB) : this()
        {
            Population.Merge(colonyInfoDB.Population);
            PlanetEntity = colonyInfoDB.PlanetEntity;
            ComponentStockpile.Merge(colonyInfoDB.ComponentStockpile);
            OrdinanceStockpile.Merge(colonyInfoDB.OrdinanceStockpile);
            foreach (Entity entity in colonyInfoDB.FighterStockpile)
            {
                FighterStockpile.Add(entity);
            }
            foreach (Entity scientist in colonyInfoDB.Scientists)
            {
                Scientists.Add(scientist);
            }
            ColonyComponentDictionary.Merge(colonyInfoDB.ColonyComponentDictionary);
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ColonyInfoDB(this);
        #endregion
    }
}
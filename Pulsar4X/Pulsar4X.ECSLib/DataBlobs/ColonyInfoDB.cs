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

namespace Pulsar4X.ECSLib
{
    public class ColonyInfoDB : BaseDataBlob
    {
        private Entity _planetEntity = Entity.InvalidEntity;
        /// <summary>
        /// Species Entity and amount
        /// </summary>
        [JsonProperty]
        public ObservableDictionary<Entity, long> Population { get; internal set; } = new ObservableDictionary<Entity, long>();


        /// <summary>
        /// constructed parts stockpile.
        /// Construction pulls and pushes from here.
        /// </summary>
        [JsonProperty]
        public ObservableDictionary<Guid, int> ComponentStockpile { get; internal set; } = new ObservableDictionary<Guid, int>();

        /// <summary>
        /// Construction pushes here.
        /// </summary>
        [JsonProperty]
        public ObservableDictionary<Guid, float> OrdinanceStockpile { get; internal set; } = new ObservableDictionary<Guid, float>();

        /// <summary>
        /// Construction *adds* to this list. damaged and partialy constructed fighters will go here too, but shouldnt launch.
        /// </summary>
        [JsonProperty]
        public ObservableCollection<Entity> FighterStockpile { get; internal set; } = new ObservableCollection<Entity>();

        /// <summary>
        /// the parent planet
        /// </summary>
        [JsonProperty]
        public Entity PlanetEntity { get { return _planetEntity; } internal set { SetField(ref _planetEntity, value); } }

        [JsonProperty]
        public ObservableCollection<Entity> Scientists { get; internal set; } = new ObservableCollection<Entity>();

        /// <summary>
        /// Installation list for damage calculations. Colony installations are considered components.
        /// </summary>
        public ObservableDictionary<Entity, double> ColonyComponentDictionary { get; set; } = new ObservableDictionary<Entity, double>();

        public ColonyInfoDB()
        {
            Population.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(Population), args);
            ComponentStockpile.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ComponentStockpile), args);
            OrdinanceStockpile.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(OrdinanceStockpile), args);
            FighterStockpile.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(FighterStockpile), args);
            Scientists.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(Scientists), args);
            ColonyComponentDictionary.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ColonyComponentDictionary), args);
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="popCount">Species and population number</param>
        /// <param name="planet"> the planet entity this colony is on</param>
        public ColonyInfoDB(IDictionary<Entity, long> popCount, Entity planet) : this()
        {
            Population.Merge(popCount);
            PlanetEntity = planet;
        }

        public ColonyInfoDB(Entity species, long populationCount, Entity planet) : this(new ObservableDictionary<Entity, long> {{species, populationCount}}, planet) { }

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

        public override object Clone()
        {
            return new ColonyInfoDB(this);
        }
    }
}

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

namespace Pulsar4X.ECSLib
{
    public class ColonyInfoDB : BaseDataBlob
    {
        private Entity _planetEntity = Entity.InvalidEntity;
        /// <summary>
        /// Species Entity and amount
        /// </summary>
        [JsonProperty]
        public Dictionary<Entity, long> Population { get; internal set; } = new Dictionary<Entity, long>();


        /// <summary>
        /// constructed parts stockpile.
        /// Construction pulls and pushes from here.
        /// </summary>
        [JsonProperty]
        public Dictionary<Guid, int> ComponentStockpile { get; internal set; } = new Dictionary<Guid, int>();

        /// <summary>
        /// Construction pushes here.
        /// </summary>
        [JsonProperty]
        public Dictionary<Guid, float> OrdinanceStockpile { get; internal set; } = new Dictionary<Guid, float>();

        /// <summary>
        /// Construction *adds* to this list. damaged and partialy constructed fighters will go here too, but shouldnt launch.
        /// </summary>
        [JsonProperty]
        public List<Entity> FighterStockpile { get; internal set; } = new List<Entity>();

        /// <summary>
        /// the parent planet
        /// </summary>
        [JsonProperty]
        public Entity PlanetEntity { get { return _planetEntity; } internal set { SetField(ref _planetEntity, value); } }

        [JsonProperty]
        public List<Entity> Scientists { get; internal set; } = new List<Entity>();

        /// <summary>
        /// Installation list for damage calculations. Colony installations are considered components.
        /// </summary>
        public Dictionary<Entity, double> ColonyComponentDictionary { get; set; }

        public ColonyInfoDB() { }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="popCount">Species and population number</param>
        /// <param name="planet"> the planet entity this colony is on</param>
        public ColonyInfoDB(Dictionary<Entity, long> popCount, Entity planet)
        {
            Population = popCount;
            PlanetEntity = planet;
            
            ComponentStockpile = new Dictionary<Guid, int>();
            OrdinanceStockpile = new Dictionary<Guid, float>();
            FighterStockpile = new List<Entity>();
            Scientists = new List<Entity>();

            ColonyComponentDictionary = new Dictionary<Entity, double>();
        }

        public ColonyInfoDB(Entity species, long populationCount, Entity planet):this(
            new Dictionary<Entity, long> {{species, populationCount}},
            planet)
        {
        }

        public ColonyInfoDB(ColonyInfoDB colonyInfoDB)
        {
            Population = new Dictionary<Entity, long>(colonyInfoDB.Population);
            PlanetEntity = colonyInfoDB.PlanetEntity;
            ComponentStockpile = new Dictionary<Guid, int>(colonyInfoDB.ComponentStockpile);
            OrdinanceStockpile = new Dictionary<Guid, float>(colonyInfoDB.OrdinanceStockpile);
            FighterStockpile = new List<Entity>(colonyInfoDB.FighterStockpile);            
            Scientists = new List<Entity>(colonyInfoDB.Scientists);
            ColonyComponentDictionary = new Dictionary<Entity, double>(colonyInfoDB.ColonyComponentDictionary);
        }

        public override object Clone()
        {
            return new ColonyInfoDB(this);
        }
    }
}

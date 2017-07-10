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
    public class FactionTechDB : BaseDataBlob
    {
        /// <summary>
        /// dictionary of technolagy levels that have been fully researched.
        /// techs will be added to this dictionary or incremeted by the processor once research is complete.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<Guid, int> ResearchedTechs { get; internal set; } = new ObservableDictionary<Guid, int>();

        /// <summary>
        /// dictionary of technologies that are available to research, or are being researched. 
        /// techs will get added to this dict as they become available by the processor.
        /// the int is how much research has been compleated on this tech.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<TechSD, int> ResearchableTechs { get; internal set; } = new ObservableDictionary<TechSD, int>();

        /// <summary>
        /// a list of techs not yet meeting the requirements to research
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<TechSD, int> UnavailableTechs { get; internal set; } = new ObservableDictionary<TechSD, int>();

        [PublicAPI]
        [JsonProperty]
        public int ResearchPoints { get; internal set; }

        /// <summary>
        /// Constructor for datablob, this should only be used when a new faction is created.
        /// </summary>
        /// <param name="alltechs">a list of all possible techs in game</param>
        public FactionTechDB(List<TechSD> alltechs)
        {
            UnavailableTechs = new ObservableDictionary<TechSD, int>();
            foreach (var techSD in alltechs)
            {             
                UnavailableTechs.Add(techSD,0);
            }

            ResearchPoints = 0;
        }

        public FactionTechDB(FactionTechDB techDB) : this()
        {
            UnavailableTechs.Merge(techDB.UnavailableTechs);
            ResearchedTechs.Merge(techDB.ResearchedTechs);
            ResearchableTechs.Merge(techDB.ResearchableTechs);
            ResearchPoints = techDB.ResearchPoints;
        }

        public FactionTechDB()
        {
            UnavailableTechs.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(UnavailableTechs), args);
            ResearchedTechs.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ResearchedTechs), args);
            ResearchableTechs.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ResearchableTechs), args);
        }

        /// <summary>
        /// returns the level that this faction has researched for a given TechSD
        /// </summary>
        /// <param name="techSD"></param>
        /// <returns></returns>
        [PublicAPI]
        public int LevelforTech(TechSD techSD)
        {
            if (ResearchedTechs.ContainsKey(techSD.ID))
                return ResearchedTechs[techSD.ID];
            else
                return 0;
        }

        public override object Clone()
        {
            return new FactionTechDB(this);
        }
    }
}

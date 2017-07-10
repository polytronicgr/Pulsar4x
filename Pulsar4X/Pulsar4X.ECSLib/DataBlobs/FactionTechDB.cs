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
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class FactionTechDB : BaseDataBlob
    {
        #region Fields
        private ObservableDictionary<TechSD, int> _researchableTechs;
        private ObservableDictionary<Guid, int> _researchedTechs;
        private int _researchPoints;
        private ObservableDictionary<TechSD, int> _unavailableTechs;
        #endregion

        #region Properties
        /// <summary>
        /// dictionary of technolagy levels that have been fully researched.
        /// techs will be added to this dictionary or incremeted by the processor once research is complete.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<Guid, int> ResearchedTechs
        {
            get { return _researchedTechs; }
            set
            {
                SetField(ref _researchedTechs, value);

                ResearchedTechs.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ResearchedTechs), args);
            }
        }

        /// <summary>
        /// dictionary of technologies that are available to research, or are being researched.
        /// techs will get added to this dict as they become available by the processor.
        /// the int is how much research has been compleated on this tech.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<TechSD, int> ResearchableTechs
        {
            get { return _researchableTechs; }
            set
            {
                SetField(ref _researchableTechs, value);
                ResearchableTechs.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ResearchableTechs), args);
            }
        }

        /// <summary>
        /// a list of techs not yet meeting the requirements to research
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<TechSD, int> UnavailableTechs
        {
            get { return _unavailableTechs; }
            set
            {
                SetField(ref _unavailableTechs, value);
                UnavailableTechs.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(UnavailableTechs), args);
            }
        }

        [PublicAPI]
        [JsonProperty]
        public int ResearchPoints { get { return _researchPoints; } set { SetField(ref _researchPoints, value); } }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for datablob, this should only be used when a new faction is created.
        /// </summary>
        /// <param name="alltechs">a list of all possible techs in game</param>
        public FactionTechDB(List<TechSD> alltechs) : this()
        {
            foreach (TechSD techSD in alltechs)
            {
                UnavailableTechs.Add(techSD, 0);
            }
        }

        public FactionTechDB(FactionTechDB techDB)
        {
            UnavailableTechs = new ObservableDictionary<TechSD, int>(techDB.UnavailableTechs);
            ResearchedTechs = new ObservableDictionary<Guid, int>(techDB.ResearchedTechs);
            ResearchableTechs = new ObservableDictionary<TechSD, int>(techDB.ResearchableTechs);
            ResearchPoints = techDB.ResearchPoints;
        }

        public FactionTechDB()
        {
            ResearchedTechs = new ObservableDictionary<Guid, int>();
            ResearchableTechs = new ObservableDictionary<TechSD, int>();
            UnavailableTechs = new ObservableDictionary<TechSD, int>();
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new FactionTechDB(this);
        #endregion

        #region Public Methods
        /// <summary>
        /// returns the level that this faction has researched for a given TechSD
        /// </summary>
        /// <param name="techSD"></param>
        /// <returns></returns>
        [PublicAPI]
        public int LevelforTech(TechSD techSD)
        {
            if (ResearchedTechs.ContainsKey(techSD.ID))
            {
                return ResearchedTechs[techSD.ID];
            }
            return 0;
        }
        #endregion
    }
}
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
    /// <summary>
    /// ScientistDB defines an entity as being a scientist, capable of research.
    /// </summary>
    /// <remarks>
    /// TODO: Gameplay Review
    /// ScientistDB and CommanderDB should probably be merged. Surely we can figure out a way
    /// for a single DB to cover all Leader functions?
    /// </remarks>
    public class ScientistDB : BaseDataBlob
    {
        private byte _maxLabs;
        private byte _assignedLabs;
        /// <summary>
        /// Bonuses that this scentist imparts.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public Dictionary<ResearchCategories, float> Bonuses { get; internal set; }

        /// <summary>
        /// Max number of labs this scientist can manage.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public byte MaxLabs { get { return _maxLabs; } internal set { SetField(ref _maxLabs, value);; } }

        /// <summary>
        /// Current number of labs assigned to this scientist.
        /// </summary>
        [JsonProperty]
        public byte AssignedLabs { get { return _assignedLabs; } internal set { SetField(ref _assignedLabs, value);; } }

        /// <summary>
        /// Queue of projects currently being worked on by this scientist.
        /// </summary>
        /// <remarks>
        /// TODO: Pre-release Review
        /// Why is ProjectQueue not a queue?
        /// </remarks>
        public List<Guid> ProjectQueue { get; internal set; }
//        public List<Guid> ProjectQueue { get; internal set; } 

        public ScientistDB() { }

        public ScientistDB(Dictionary<ResearchCategories,float> bonuses, byte maxLabs )
        {
            Bonuses = bonuses;
            MaxLabs = maxLabs;
            AssignedLabs = 0;
            ProjectQueue = new List<Guid>();
        }

        public ScientistDB(ScientistDB dB)
        {
            Bonuses = new Dictionary<ResearchCategories, float>(dB.Bonuses);
            MaxLabs = dB.MaxLabs;
            AssignedLabs = dB.AssignedLabs;
            ProjectQueue = dB.ProjectQueue;
        }

        public override object Clone() => new ScientistDB(this);
    }
}
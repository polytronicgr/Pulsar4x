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

namespace Pulsar4X.ECSLib
{
    public class RuinsDB : BaseDataBlob
    {
        private uint _ruinCount;
        private int _ruinTechLevel;
        private RSize _ruinSize;
        private RQuality _ruinQuality;

        /// <summary>
        /// Ruins size descriptors
        /// </summary>
        public enum RSize : byte
        {
            NoRuins,
            Outpost,
            Settlement,
            Colony,
            City,
            Count
        }

        /// <summary>
        /// Ruins Quality descriptors
        /// </summary>
        public enum RQuality : byte
        {
            Destroyed,
            Ruined,
            PartiallyIntact,
            Intact,
            MultipleIntact,
            Count
        }

        /// <summary>
        /// How many ruins are on this world. or something.
        /// </summary>
        [JsonProperty]
        public uint RuinCount { get { return _ruinCount; } internal set { SetField(ref _ruinCount, value);; } }

        /// <summary>
        /// What kinds of things should be found in this ruin? including sophistication of killbots?
        /// </summary>
        [JsonProperty]
        public int RuinTechLevel { get { return _ruinTechLevel; } internal set { SetField(ref _ruinTechLevel, value);; } }

        /// <summary>
        /// How big are these ruins?
        /// </summary>
        [JsonProperty]
        public RSize RuinSize { get { return _ruinSize; } internal set { SetField(ref _ruinSize, value);; } }

        /// <summary>
        /// What shape are these ruins in?
        /// </summary>
        [JsonProperty]
        public RQuality RuinQuality { get { return _ruinQuality; } internal set { SetField(ref _ruinQuality, value);; } }

        /// <summary>
        /// Empty constructor for RuinsDataBlob.
        /// </summary>
        public RuinsDB()
        {
            RuinCount = 0;
            RuinTechLevel = 0;
            RuinSize = RSize.Count;
            RuinQuality = RQuality.Count;
        }

        /// <summary>
        /// Constructor for RuinsDataBlob.
        /// </summary>
        /// <param name="ruinCount"></param>
        /// <param name="ruinTechLevel"> What kinds of things should be found in this ruin? including sophistication of killbots?</param>
        /// <param name="ruinSize">How big are these ruins?</param>
        /// <param name="ruinQuality"> What shape are these ruins in?</param>
        public RuinsDB(uint ruinCount, int ruinTechLevel, RSize ruinSize, RQuality ruinQuality)
        {
            RuinCount = ruinCount;
            RuinTechLevel = ruinTechLevel;
            RuinSize = ruinSize;
            RuinQuality = ruinQuality;
        }
        
        public override object Clone()
        {
            return new RuinsDB(RuinCount, RuinTechLevel, RuinSize, RuinQuality);
        }
    }
}

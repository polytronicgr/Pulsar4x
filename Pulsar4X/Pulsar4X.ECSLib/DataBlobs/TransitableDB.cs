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
    /// <summary>
    /// TransitableDB defines an entity as capable of being used as a jump point.
    /// </summary>
    public class TransitableDB : BaseDataBlob
    {
        #region Fields
        private Entity _destination;
        private bool _isStabilized;
        #endregion

        #region Properties
        /// <summary>
        /// Destination that this jump point goes to.
        /// </summary>
        [JsonProperty]
        public Entity Destination
        {
            get { return _destination; }
            set
            {
                SetField(ref _destination, value);
                ;
            }
        }

        /// <summary>
        /// Determination if this jump point has a "gate" on it.
        /// </summary>
        /// <remarks>
        /// TODO: Gameplay Review
        /// We might want to use a TransitType enum, to allow different types of FTL using the same type of DB
        /// </remarks>
        [JsonProperty]
        public bool IsStabilized
        {
            get { return _isStabilized; }
            set
            {
                SetField(ref _isStabilized, value);
                ;
            }
        }
        #endregion

        #region Constructors
        public TransitableDB() { }

        public TransitableDB(Entity destination) : this(destination, false) { }

        public TransitableDB(Entity destination, bool isStabilized)
        {
            Destination = destination;
            IsStabilized = isStabilized;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new TransitableDB(Destination, IsStabilized);
        #endregion
    }
}
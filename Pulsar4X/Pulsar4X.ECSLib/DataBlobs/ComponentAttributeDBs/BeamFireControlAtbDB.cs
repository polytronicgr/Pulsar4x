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
    public class BeamFireControlAtbDB : BaseDataBlob
    {
        #region Fields
        private bool _finalFireOnly;
        private int _range;
        private int _trackingSpeed;
        #endregion

        #region Properties
        /// <summary>
        /// Max range of this Beam Fire Control
        /// </summary>
        [JsonProperty]
        public int Range { get { return _range; } set { SetField(ref _range, value); } }

        /// <summary>
        /// Tracking Speed of this Beam Fire Control
        /// </summary>
        [JsonProperty]
        public int TrackingSpeed { get { return _trackingSpeed; } set { SetField(ref _trackingSpeed, value); } }

        /// <summary>
        /// Determines if this Beam Fire Control is only capable of FinalDefensiveFire (Like CIWS)
        /// </summary>
        [JsonProperty]
        public bool FinalFireOnly { get { return _finalFireOnly; } set { SetField(ref _finalFireOnly, value); } }
        #endregion

        #region Constructors
        public BeamFireControlAtbDB(double range, double trackingSpeed) : this((int)range, (int)trackingSpeed) { }

        [JsonConstructor]
        public BeamFireControlAtbDB(int range = 0, int trackingSpeed = 0, bool finalFireOnly = false)
        {
            Range = range;
            TrackingSpeed = trackingSpeed;
            FinalFireOnly = finalFireOnly;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new BeamFireControlAtbDB(Range, TrackingSpeed, FinalFireOnly);
        #endregion
    }
}
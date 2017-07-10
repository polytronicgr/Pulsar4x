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
    public class ActiveSensorAtbDB : BaseDataBlob
    {
        #region Fields
        private int _emSensitivity;
        private int _gravSensorStrength;
        private bool _isSearchSensor;
        private int _resolution;
        #endregion

        #region Properties
        [JsonProperty]
        public int GravSensorStrength { get { return _gravSensorStrength; } set { SetField(ref _gravSensorStrength, value); } }

        [JsonProperty]
        public int EMSensitivity { get { return _emSensitivity; } set { SetField(ref _emSensitivity, value); } }

        [JsonProperty]
        public int Resolution { get { return _resolution; } set { SetField(ref _resolution, value); } }

        [JsonProperty]
        public bool IsSearchSensor { get { return _isSearchSensor; } set { SetField(ref _isSearchSensor, value); } }

        public bool IsTrackingSensor => !IsSearchSensor;
        #endregion

        #region Constructors
        [JsonConstructor]
        public ActiveSensorAtbDB(int gravStrength = 0, int emSensitivity = 0, int resolution = 0, bool isSearchSensor = true)
        {
            GravSensorStrength = gravStrength;
            EMSensitivity = emSensitivity;
            Resolution = resolution;
            IsSearchSensor = isSearchSensor;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ActiveSensorAtbDB(GravSensorStrength, EMSensitivity, Resolution, IsSearchSensor);
        #endregion
    }

    public class ActiveSensorStateInfo
    {
        #region Properties
        public Entity Target { get; set; } = Entity.InvalidEntity;
        #endregion
    }
}
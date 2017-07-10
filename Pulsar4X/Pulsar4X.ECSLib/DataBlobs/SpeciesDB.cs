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
    /// SpeciesDB defines an entity as being a Species.
    /// </summary>
    public class SpeciesDB : BaseDataBlob
    {
        #region Fields
        private double _baseGravity;
        private double _basePressure;
        private double _baseTemperature;
        private double _maximumGravityConstraint;
        private double _maximumPressureConstraint;
        private double _maximumTemperatureConstraint;
        private double _minimumGravityConstraint;
        private double _minimumPressureConstraint;
        private double _minimumTemperatureConstraint;
        private double _temperatureToleranceRange;
        #endregion

        #region Properties
        /// <summary>
        /// The ideal gravity for this species.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double BaseGravity
        {
            get { return _baseGravity; }
            set
            {
                SetField(ref _baseGravity, value);
                ;
            }
        }

        /// <summary>
        /// The minimum gravity the species can tolerate
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double MinimumGravityConstraint { get { return _minimumGravityConstraint; } set { SetField(ref _minimumGravityConstraint, value); } }

        /// <summary>
        /// The maximum gravity the species can tolerate
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double MaximumGravityConstraint
        {
            get { return _maximumGravityConstraint; }
            set
            {
                SetField(ref _maximumGravityConstraint, value);
                ;
            }
        }

        /// <summary>
        /// The ideal atmospheric pressure for the species
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double BasePressure
        {
            get { return _basePressure; }
            set
            {
                SetField(ref _basePressure, value);
                ;
            }
        }

        /// <summary>
        /// The minimum atmospheric pressure the species can tolerate
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double MinimumPressureConstraint
        {
            get { return _minimumPressureConstraint; }
            set
            {
                SetField(ref _minimumPressureConstraint, value);
                ;
            }
        }

        /// <summary>
        /// The maximum atmospheric pressure the species can tolerate
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double MaximumPressureConstraint
        {
            get { return _maximumPressureConstraint; }
            set
            {
                SetField(ref _maximumPressureConstraint, value);
                ;
            }
        }

        /// <summary>
        /// The ideal temperature for the species
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double BaseTemperature
        {
            get { return _baseTemperature; }
            set
            {
                SetField(ref _baseTemperature, value);
                ;
            }
        }

        /// <summary>
        /// The minimum temperature the species can tolerate
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double MinimumTemperatureConstraint
        {
            get { return _minimumTemperatureConstraint; }
            set
            {
                SetField(ref _minimumTemperatureConstraint, value);
                ;
            }
        }

        /// <summary>
        /// The maximum temperature the species can tolerate
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double MaximumTemperatureConstraint
        {
            get { return _maximumTemperatureConstraint; }
            set
            {
                SetField(ref _maximumTemperatureConstraint, value);
                ;
            }
        }

        /// <summary>
        /// Range of temperatures this species can withstand?
        /// </summary>
        /// <remarks>
        /// TODO: Gameplay Review
        /// We should either have BaseTemperature + ToleranceRange, or Min/Max temps, but not both.
        /// Min/Max works best with aurora mechanics.
        /// </remarks>
        [PublicAPI]
        [JsonProperty]
        public double TemperatureToleranceRange
        {
            get { return _temperatureToleranceRange; }
            set
            {
                SetField(ref _temperatureToleranceRange, value);
                ;
            }
        }
        #endregion

        #region Constructors
        public SpeciesDB() { }

        public SpeciesDB(double baseGravity, double minGravity, double maxGravity, double basePressure, double minPressure, double maxPressure, double baseTemp, double minTemp, double maxTemp)
        {
            BaseGravity = baseGravity;
            MinimumGravityConstraint = minGravity;
            MaximumGravityConstraint = maxGravity;
            BasePressure = basePressure;
            MinimumPressureConstraint = minPressure;
            MaximumPressureConstraint = maxPressure;
            BaseTemperature = baseTemp;
            MinimumTemperatureConstraint = minTemp;
            MaximumTemperatureConstraint = maxTemp;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new SpeciesDB(BaseGravity, MinimumGravityConstraint, MaximumGravityConstraint, BasePressure, MinimumPressureConstraint, MaximumPressureConstraint, BaseTemperature, MinimumTemperatureConstraint, MaximumTemperatureConstraint);
        #endregion
    }
}
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
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public enum SpectralType : byte
    {
        O,
        B,
        A,
        F,
        G,
        K,
        M,
        D,
        C
    }

    public enum LuminosityClass : byte
    {
        O, // Hypergiants
        Ia, // Luminous Supergiants
        Iab, // Intermediate Supergiants
        Ib, // Less Luminous Supergiants
        II, // Bright Giants
        III, // Giants
        IV, // Subgiants
        V, // Main-Sequence (like our sun)
        sd, // Subdwarfs
        D // White Dwarfs
    }

    public class StarInfoDB : BaseDataBlob
    {
        #region Fields
        private double _age;
        private string _class;
        private double _luminosity;
        private LuminosityClass _luminosityClass;
        private ushort _spectralSubDivision;
        private SpectralType _spectralType;
        private double _temperature;
        #endregion

        #region Properties
        /// <summary>
        /// Age of this star. Fluff.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double Age
        {
            get { return _age; }
            set
            {
                SetField(ref _age, value);
                ;
            }
        }

        /// <summary>
        /// Effective ("Photosphere") temperature in Degrees C.
        /// Affects habitable zone.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double Temperature
        {
            get { return _temperature; }
            set
            {
                SetField(ref _temperature, value);
                ;
            }
        }

        /// <summary>
        /// Luminosity of this star. Fluff.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double Luminosity
        {
            get { return _luminosity; }
            set
            {
                SetField(ref _luminosity, value);
                ;
            }
        }

        /// <summary>
        /// Star class. Mostly fluff (affects SystemGeneration).
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public string Class
        {
            get { return _class; }
            set
            {
                SetField(ref _class, value);
                ;
            }
        }

        /// <summary>
        /// Main Type. Mostly fluff (affects SystemGeneration).
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public SpectralType SpectralType
        {
            get { return _spectralType; }
            set
            {
                SetField(ref _spectralType, value);
                ;
            }
        }

        /// <summary>
        /// Subtype.  Mostly fluff (affects SystemGeneration).
        /// number from  0 (hottest) to 9 (coolest)
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public ushort SpectralSubDivision
        {
            get { return _spectralSubDivision; }
            set
            {
                SetField(ref _spectralSubDivision, value);
                ;
            }
        }

        /// <summary>
        /// LuminosityClass. Fluff.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public LuminosityClass LuminosityClass
        {
            get { return _luminosityClass; }
            set
            {
                SetField(ref _luminosityClass, value);
                ;
            }
        }

        /// <summary>
        /// Calculates and sets the Habitable Zone of this star based on it Luminosity.
        /// calculated according to this site: http://www.planetarybiology.com/calculating_habitable_zone.html
        /// </summary>
        [PublicAPI]
        public double EcoSphereRadius => (MinHabitableRadius + MaxHabitableRadius) / 2;

        /// <summary>
        /// Minimum edge of the Habitable Zone (in AU)
        /// </summary>
        [PublicAPI]
        public double MinHabitableRadius => Math.Sqrt(Luminosity / 1.1);

        /// <summary>
        /// Maximum edge of the Habitable Zone (in AU)
        /// </summary>
        [PublicAPI]
        public double MaxHabitableRadius => Math.Sqrt(Luminosity / 0.53);
        #endregion

        #region Constructors
        public StarInfoDB() { }

        public StarInfoDB(StarInfoDB starInfoDB)
        {
            Age = starInfoDB.Age;
            Temperature = starInfoDB.Temperature;
            Luminosity = starInfoDB.Luminosity;
            Class = starInfoDB.Class;

            SpectralType = starInfoDB.SpectralType;
            SpectralSubDivision = starInfoDB.SpectralSubDivision;
            LuminosityClass = starInfoDB.LuminosityClass;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new StarInfoDB(this);
        #endregion
    }
}
using Newtonsoft.Json;
using System;

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
        O,          // Hypergiants
        Ia,         // Luminous Supergiants
        Iab,        // Intermediate Supergiants
        Ib,         // Less Luminous Supergiants
        II,         // Bright Giants
        III,        // Giants
        IV,         // Subgiants
        V,          // Main-Sequence (like our sun)
        sd,         // Subdwarfs
        D,          // White Dwarfs
    }

    public class StarDB : BaseDataBlob
    {
        
        [JsonProperty]
        public double Age { get; internal set; }

        // Effective ("Photosphere") temperature in Degrees C.
        
        [JsonProperty]
        public double Temperature { get; internal set; }

        
        [JsonProperty]
        public double Luminosity { get; internal set; }

        
        [JsonProperty]
        public string Class { get; internal set; }


        
        [JsonProperty]
        public SpectralType SpectralType { get; internal set; }

        // number from  0 (hottest) to 9 (coolest)
        
        [JsonProperty]
        public ushort SpectralSubDivision { get; internal set; }

        
        [JsonProperty]
        public LuminosityClass LuminosityClass { get; internal set; }

        /// <summary>
        /// Calculates and sets the Habitable Zone of this star based on it Luminosity.
        /// calculated according to this site: http://www.planetarybiology.com/calculating_habitable_zone.html
        /// </summary>
        
        public double EcoSphereRadius => (MinHabitableRadius + MaxHabitableRadius) / 2;

        // Average Habitable Radius, in AU.
        
        public double MinHabitableRadius => Math.Sqrt(Luminosity / 1.1);

        // in au
        
        public double MaxHabitableRadius => Math.Sqrt(Luminosity / 0.53);

        // in au

        public StarDB()
        {
            
        }

        public StarDB(StarDB StarDB)
        {
            Age = StarDB.Age;
            Temperature = StarDB.Temperature;
            Luminosity = StarDB.Luminosity;
            Class = StarDB.Class;

            SpectralType = StarDB.SpectralType;
            SpectralSubDivision = StarDB.SpectralSubDivision;
            LuminosityClass = StarDB.LuminosityClass;

        }

        public override object Clone()
        {
            return new StarDB(this);
        }
    }
}

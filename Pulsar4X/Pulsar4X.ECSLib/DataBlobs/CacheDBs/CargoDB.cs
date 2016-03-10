using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Contains info on a ships cargo capicity.
    /// </summary>
    public class CargoDB : BaseDataBlob
    {
        [JsonProperty]
        internal Dictionary<CargoDefinition, double> cargoCarried { get; set; } = new Dictionary<CargoDefinition, double>();
        public ReadOnlyDictionary<CargoDefinition, double> CargoCarried => new ReadOnlyDictionary<CargoDefinition, double>(cargoCarried);
        
        [JsonProperty]
        internal Dictionary<CargoType, double> cargoCapacity { get; set; } = new Dictionary<CargoType, double>();
        public ReadOnlyDictionary<CargoType, double> CargoCapacity => new ReadOnlyDictionary<CargoType, double>(cargoCapacity);

        [JsonProperty]
        public bool HasUnlimitedCapacity { get; internal set; }

        public CargoDB()
        {
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public double GetFreeCargoSpace(Game game, CargoType cargoType)
        {
            if (HasUnlimitedCapacity)
            {
                return double.MaxValue;
            }

            double freeSpace = cargoCapacity[cargoType];

            foreach (KeyValuePair<CargoDefinition, double> carriedCargo in cargoCarried)
            {
                CargoDefinition cargoDef = carriedCargo.Key;

                if (cargoDef.Type == cargoType)
                {
                    freeSpace -= carriedCargo.Value;
                }
            }

            return freeSpace;
        }
    }
}
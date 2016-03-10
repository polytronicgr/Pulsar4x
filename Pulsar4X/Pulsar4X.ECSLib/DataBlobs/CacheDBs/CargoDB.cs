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
        public IReadOnlyDictionary<CargoDefinition, double> CargoCarried => cargoCarried;
        
        [JsonProperty]
        internal Dictionary<CargoType, double> cargoCapacity { get; set; } = new Dictionary<CargoType, double>();
        public IReadOnlyDictionary<CargoType, double> CargoCapacity =>cargoCapacity;

        [JsonProperty]
        public bool HasUnlimitedCapacity { get; internal set; }
        
        public override object Clone()
        {
            return new CargoDB {cargoCarried = cargoCarried, cargoCapacity = cargoCapacity, HasUnlimitedCapacity = HasUnlimitedCapacity};
        }
    }
}
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
        internal Dictionary<Guid, double> cargoCarried { get; set; } = new Dictionary<Guid, double>();
        public IReadOnlyDictionary<Guid, double> CargoCarried => cargoCarried;
        
        [JsonProperty]
        internal Dictionary<CargoType, double> cargoCapacity { get; set; } = new Dictionary<CargoType, double>();
        public IReadOnlyDictionary<CargoType, double> CargoCapacity =>cargoCapacity;

        [JsonProperty]
        public bool HasUnlimitedCapacity { get; internal set; }

        [JsonConstructor]
        public CargoDB(bool unlimitedCapacity = false)
        {
            HasUnlimitedCapacity = unlimitedCapacity;
        }

        public override object Clone()
        {
            return new CargoDB {cargoCarried = cargoCarried, cargoCapacity = cargoCapacity, HasUnlimitedCapacity = HasUnlimitedCapacity};
        }
    }
}
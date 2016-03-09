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
        internal Dictionary<Guid, float> cargoCarried { get; set; } = new Dictionary<Guid, float>();
        public ReadOnlyDictionary<Guid, float> CargoCarried => new ReadOnlyDictionary<Guid, float>(cargoCarried);

        public Dictionary<CargoType, float> cargoCapacityUsed = new Dictionary<CargoType, float>();

        [JsonProperty]
        internal Dictionary<CargoType, float> cargoCapacity { get; set; } = new Dictionary<CargoType, float>();
        public ReadOnlyDictionary<CargoType, float> CargoCapacity => new ReadOnlyDictionary<CargoType, float>(cargoCapacity);

        [JsonProperty]
        public bool HasUnlimitedCapacity { get; internal set; }

        public CargoDB()
        {
        }

        public override object Clone()
        {
            return new CargoDB(this);
        }

        public void StoreCargo(Guid cargoGuid, float amount)
        {
            
        }

        private CargoDefinition LookupCargo(Game game, Guid cargoGuid)
        {
            Entity entity;
            CargoDefinition cargoDefinition = new CargoDefinition();

            if (game.GlobalManager.FindEntityByGuid(cargoGuid, out entity))
            {
                // Cargo is a component.
                var componentInfo = entity.GetDataBlob<ComponentInfoDB>();
                cargoDefinition.Type = CargoType.General;
                cargoDefinition.Weight = componentInfo.SizeInTons * 1000;
            }

            return cargoDefinition;
        }
    }
}
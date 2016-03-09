using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class CargoStorageAbilityDB : BaseDataBlob
    {
        /// <summary>
        /// Storage Capacity of this module.
        /// </summary>
        [JsonProperty]
        public int StorageCapacity { get; internal set; }

        [JsonProperty]
        public float LoadingSpeedMultiplier { get; internal set; }

        public CargoStorageAbilityDB(double storageCapacity, double loadingSpeedMultiplier) : this((int)storageCapacity, (float)loadingSpeedMultiplier) { }

        [JsonConstructor]
        public CargoStorageAbilityDB(int storageCapacity = 0, float loadingSpeedMultiplier = 0)
        {
            StorageCapacity = storageCapacity;
            LoadingSpeedMultiplier = loadingSpeedMultiplier;
        }

        public override object Clone()
        {
            return new CargoStorageAbilityDB(StorageCapacity);
        }
    }
}

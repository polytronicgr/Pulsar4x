using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class FuelStorageAbilityDB : BaseDataBlob
    {
        [JsonProperty]
        public int StorageCapacity { get; internal set; }

        [JsonConstructor]
        public FuelStorageAbilityDB(double storageCapacity = 0)
        {
            StorageCapacity = (int)storageCapacity;
        }

        public override object Clone()
        {
            return new FuelStorageAbilityDB(StorageCapacity);
        }
    }
}
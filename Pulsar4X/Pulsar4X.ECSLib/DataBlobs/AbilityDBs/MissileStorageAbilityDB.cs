using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class MissileStorageAbilityDB : BaseDataBlob
    {
        [JsonProperty]
        public int StorageCapacity { get; internal set; }

        public MissileStorageAbilityDB(double storageCapacity) : this((int)storageCapacity) { }

        [JsonConstructor]
        public MissileStorageAbilityDB(int storageCapacity = 0)
        {
            StorageCapacity = storageCapacity;
        }

        public override object Clone()
        {
            return new MissileStorageAbilityDB(StorageCapacity);
        }
    }
}

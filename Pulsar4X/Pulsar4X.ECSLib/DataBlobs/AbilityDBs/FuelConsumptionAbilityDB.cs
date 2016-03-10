using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class FuelConsumptionAbilityDB : BaseDataBlob
    {
        [JsonProperty]
        public double FuelUsePerHour { get; internal set; }
        
        [JsonConstructor]
        public FuelConsumptionAbilityDB(double fuelUsagePerHour = 0)
        {
            FuelUsePerHour = fuelUsagePerHour;
        }

        public override object Clone()
        {
            return new FuelConsumptionAbilityDB(FuelUsePerHour);
        }
    }
}
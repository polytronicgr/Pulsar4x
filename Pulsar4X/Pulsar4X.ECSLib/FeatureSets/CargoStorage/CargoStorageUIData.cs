using System.Collections.Generic;
using Newtonsoft.Json;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib
{
    public class CargoStorageUIData : UIData
    {
        [JsonProperty]
        public List<CargoTypeAmount> Capacities = new List<CargoTypeAmount>();

        [JsonConstructor]
        public CargoStorageUIData() { }

        public CargoStorageUIData(StaticDataStore staticData, CargoStorageDB db)
        {
            foreach (var kvp in db.CargoCapicity)
            {
                string cargoTypeName = staticData.CargoTypes[kvp.Key].Name;
                Capacities.Add(new CargoTypeAmount(){TypeName = cargoTypeName, Amount = kvp.Value});
            }
        }

        public struct CargoTypeAmount
        {
            public string TypeName;
            public long Amount;
        }
    }
}
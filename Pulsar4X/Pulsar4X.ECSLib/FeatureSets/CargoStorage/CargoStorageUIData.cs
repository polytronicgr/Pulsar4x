using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib
{
    public class CargoStorageUIData : UIData
    {
        public static string DataCode = "CargoData";

        public override string GetDataCode{get { return DataCode; }}

        [JsonProperty]
        public List<CargoTypeAmount> Capacities = new List<CargoTypeAmount>();

        
        
        [JsonConstructor]
        public CargoStorageUIData() { }

        public CargoStorageUIData(StaticDataStore staticData, CargoStorageDB db)
        {
            foreach (var kvp in db.CargoCapacity)
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
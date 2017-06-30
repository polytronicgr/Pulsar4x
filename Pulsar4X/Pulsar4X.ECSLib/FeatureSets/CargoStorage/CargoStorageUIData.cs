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
        public List<CargoTypeAmount> TotalCapacities = new List<CargoTypeAmount>();

        [JsonProperty]
        public List<CargoTypeAmount> UsedCapacities = new List<CargoTypeAmount>();

        
        
        public List<CargoByType> CargoByTypes = new List<CargoByType>();
        
        [JsonConstructor]
        public CargoStorageUIData() { }

        public CargoStorageUIData(StaticDataStore staticData, CargoStorageDB db)
        {
            foreach (var kvp in db.CargoCapicities)
            {
                string cargoTypeName = staticData.CargoTypes[kvp.Key].Name;
                TotalCapacities.Add(new CargoTypeAmount(){TypeName = cargoTypeName, Amount = kvp.Value});
            }
            
            foreach (var kvp in db.UsedCapicities)
            {
                string cargoTypeName = staticData.CargoTypes[kvp.Key].Name;
                UsedCapacities.Add(new CargoTypeAmount(){TypeName = cargoTypeName, Amount = kvp.Value});
            }

            foreach (var kvp in db.MinsAndMatsByCargoType)
            {
                string cargoTypeName = staticData.CargoTypes[kvp.Key].Name;
                foreach (var kvp2 in kvp.Value)
                {
                    //kvp2.Key.ItemTypeName
                }
                //CargoByTypes.Add(new CargoByType(){TypeName = cargoTypeName, TotalWeight = kvp.Value});
            }
        }

        public struct CargoTypeAmount
        {
            public string TypeName;
            public long Amount;
        }

        public struct CargoByType
        {
            public string TypeName;
            public long TotalWeight;
        }
    }
}
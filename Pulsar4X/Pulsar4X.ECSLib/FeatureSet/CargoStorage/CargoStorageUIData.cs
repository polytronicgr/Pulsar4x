using System.Collections.Generic;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib
{
    public class CargoStorageUIData : UIData
    {
        public List<CargoTypeAmount> Capacities = new List<CargoTypeAmount>();


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
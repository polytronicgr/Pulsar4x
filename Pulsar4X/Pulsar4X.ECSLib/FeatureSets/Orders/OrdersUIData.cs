using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib
{
    public class OrdersUIData : UIData
    {
        public static string DataCode = "OrderData";
        public override string GetDataCode { get { return DataCode; } }

        [JsonProperty]
        public List<OrderUIData> OrderUIDatas = new List<OrderUIData>();
        
        public OrdersUIData() { }

        public OrdersUIData(OrderableDB db)
        {
            foreach (var action in db.ActionQueue)
            {                               
                var orderData = new OrderUIData()
                {
                    Name = action.Name,
                    Status = action.Status                  
                };
                if (action.HasTargetEntity)
                {
                    orderData.TargetName = action.TargetEntity.GetDataBlob<NameDB>().GetName(action.FactionEntity); 
                }
                
                OrderUIDatas.Add(orderData);
            }           
        }
        
        public struct OrderUIData
        {
            public string Name;
            public string Status;
            public string TargetName;
        }

    }
}
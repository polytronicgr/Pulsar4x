using System.Collections.Generic;
using Newtonsoft.Json;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// this datablob allows an entity to be orderable. 
    /// </summary>
    public class OrderableDB : SubscribableDatablob
    {        
        [JsonProperty]
        public List<BaseAction> ActionQueue { get; } = new List<BaseAction>();
        
        public OrderableDB()
        {
        }

        public OrderableDB(OrderableDB db)
        {
            ActionQueue = new List<BaseAction>(db.ActionQueue);
        }

        public override object Clone()
        {
            return new OrderableDB(this);
        }
    }
}
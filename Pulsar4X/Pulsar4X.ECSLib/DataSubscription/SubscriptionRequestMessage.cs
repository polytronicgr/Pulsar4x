using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib.DataSubscription
{
    public class SubscriptionRequestMessage<T> : BaseToServerMessage where T: SubscribableDatablob
    {
        public Guid EntityGuid { get; set; }
        public override string ResponseCode { get; set; } = typeof(T).ToString();

        [JsonConstructor]
        public SubscriptionRequestMessage() { }

        public static SubscriptionRequestMessage<T> NewMessage(Guid connectionID, Guid factionID, Guid entityGuid)
        {
            return new SubscriptionRequestMessage<T>()
            {
                ConnectionID = connectionID,
                FactionGuid = factionID,
                EntityGuid = entityGuid,
                
            };
        }

        

        internal override void HandleMessage(Game game)
        {            
            game.MessagePump.DataSubscibers[ConnectionID].Subscribe<T>(EntityGuid);
        }
    }
}
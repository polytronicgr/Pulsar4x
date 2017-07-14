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

    public class DataRequestMessage<T> : BaseToServerMessage
        where T : SubscribableDatablob
    {
        public Guid EntityGuid { get; set; }
        public override string ResponseCode { get; set; } = typeof(T).ToString();

        public DataRequestMessage(Guid connectionID, Guid entityGuid)
        {
            EntityGuid = entityGuid;
            ConnectionID = connectionID;
        }
        
        internal override void HandleMessage(Game game)
        {
            Entity entity;
            if(game.GlobalManager.FindEntityByGuid(EntityGuid, out entity))
            {
                SubscribableDatablob dataBlob;
                if (entity.HasDataBlob<T>())
                {
                    dataBlob = entity.GetDataBlob<T>();
                    
                    game.MessagePump.EnqueueOutgoingMessage(ConnectionID,  new DatablobDataMessage<T>(dataBlob) );
                }
            }
        }
    }

    public class DatablobDataMessage<T> : BaseToClientMessage
        where T : SubscribableDatablob
    {
        public override string ResponseCode { get; } = typeof(T).ToString();
        public BaseDataBlob DataBlob { get; set; }

        public DatablobDataMessage(BaseDataBlob dataBlob) { DataBlob = dataBlob; }
    }
}
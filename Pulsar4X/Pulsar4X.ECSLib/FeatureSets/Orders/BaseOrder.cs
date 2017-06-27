using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseOrder : BaseToServerMessage
    {        
        [JsonProperty]
        public Guid EntityGuid { get; set; }
        //[JsonProperty]
        //public Guid FactionGuid { get; set; }
        [JsonProperty]
        public Guid TargetEntityGuid { get; internal set; }
        [JsonProperty]
        internal bool HasTargetEntity { get; } = false;

        protected BaseOrder() { }

        protected BaseOrder(Guid orderEntity, Guid faction)
        {
            FactionGuid = faction;
            EntityGuid = orderEntity;
        }


        protected BaseOrder(Guid orderEntity, Guid faction, Guid targetEntity) : this(orderEntity, faction)
        {
            TargetEntityGuid = targetEntity;
            HasTargetEntity = true;
        }

        /// <summary>
        /// Creates an Action 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="order"></param>
        internal abstract BaseAction CreateAction(Game game, BaseOrder order);

        internal override void HandleMessage(Game game)
        {
            
            Entity entity;
            if (game.GlobalManager.FindEntityByGuid(EntityGuid, out entity))
            {
                entity.Manager.OrderQueue.Enqueue(this);
            }   
        }

        /// <summary>
        /// returns true if the required entites are found and the orderedEntity is owned by the factionEntity
        /// I realy dislike this style of function however, and I may rewrite it. (bool with out)
        /// </summary>
        /// <param name="game"></param>
        /// <param name="order"></param>
        /// <param name="orderEntities"></param>
        /// <returns></returns>
        internal static bool GetOrderEntities(Game game, BaseOrder order, out OrderEntities orderEntities)
        {
            orderEntities = new OrderEntities();
            if (!game.GlobalManager.FindEntityByGuid(order.EntityGuid, out orderEntities.ThisEntity))
                return false;
            if (!orderEntities.ThisEntity.HasDataBlob<OrderableDB>())
                return false;
            if (!game.GlobalManager.FindEntityByGuid(order.FactionGuid, out orderEntities.FactionEntity))
                return false;
            if (order.HasTargetEntity)
            {
                if (!game.GlobalManager.FindEntityByGuid(order.TargetEntityGuid, out orderEntities.TargetEntity))
                    return false;
            }
            if (orderEntities.ThisEntity.GetDataBlob<OwnedDB>().EntityOwner != orderEntities.FactionEntity)
                return false;
            
            return true;
        }
    }

    internal struct OrderEntities
    {
        internal Entity ThisEntity;
        internal Entity FactionEntity;
        internal Entity TargetEntity;
    }
}
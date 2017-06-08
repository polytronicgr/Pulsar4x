using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class EnableDisableComponentOrder:BaseOrder
    {
        public enum OrderType
        {
            Enable,
            Disable
        }
        [JsonProperty]
        public OrderType Ordertype { get; set; }
        
        public EnableDisableComponentOrder(Guid entity, Guid faction, OrderType orderType) 
            : base(entity, faction)
        {
            Ordertype = orderType;    
        }

        internal BaseAction CreateAction(Game game, EnableDisableComponentOrder order)
        {
            OrderEntities orderEntities;
            if (GetOrderEntities(game, order, out orderEntities))
            {
                
                if (order.Ordertype == OrderType.Enable)
                {
                    return new EnableComponent(this, orderEntities); 
                }
                else
                {
                    return new DisableComponent(this, orderEntities); 
                }                                   
            }
            else throw new Exception("Failed Create " + order.Ordertype + "Component Action");
        }

        internal override BaseAction CreateAction(Game game, BaseOrder order) 
        { return CreateAction(game, (EnableDisableComponentOrder)order); }
    }
    
    /// <summary>
    /// This will enable a component/facility. ie turn on a mine, active sensor etc, 
    /// it is non blocking and shouldn't normaly be blocked by other actions. 
    /// keeping enable/disable seperate instead of just a single toggle should help with laggy network race conditions.
    /// </summary>
    internal class EnableComponent : BaseAction
    {
        public EnableComponent(BaseOrder order, OrderEntities orderEntities) : base(3, false, order, orderEntities.ThisEntity, orderEntities.FactionEntity)
        {
            orderEntities.ThisEntity.GetDataBlob<ComponentInstanceInfoDB>().IsEnabled = true;            
            OrderableProcessor = new DoNothingAction();
            IsFinished = true;
        }
    }
    
    /// <summary>
    /// This will disable a component/facility. ie turn off a mine, disable an active sensor etc, 
    /// it is non blocking and shouldn't normaly be blocked by other actions. 
    /// keeping enable/disable seperate instead of just a single toggle should help with laggy network race conditions.
    /// </summary>
    internal class DisableComponent : BaseAction
    {
        public DisableComponent(BaseOrder order, OrderEntities orderEntities) : base(3, false, order, orderEntities.ThisEntity, orderEntities.FactionEntity)
        {
            orderEntities.ThisEntity.GetDataBlob<ComponentInstanceInfoDB>().IsEnabled = true;            
            OrderableProcessor = new DoNothingAction();
            IsFinished = true;
        }
    }
}
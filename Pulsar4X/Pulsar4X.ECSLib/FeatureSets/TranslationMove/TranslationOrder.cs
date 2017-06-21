using System;

namespace Pulsar4X.ECSLib
{
    public class TranslationOrder : BaseOrder
    {        
        public enum HelmOrderTypeEnum
        {
            InterceptTarget, //move to the target, other orders should initiate this first if they're not close enough.
            MatchTarget, //close and orbit the targets parent if orbiting, else close and match the targets speed.
            OrbitTarget, //orbit the target
        }
        public HelmOrderTypeEnum OrderType { get; set; }
        public double StandOffDistance { get; set; }
        
        /// <summary>
        /// Creation of a new order. 
        /// </summary>
        /// <param name="faction">The Faction this order is coming from</param>
        /// <param name="entity">The entity this order is for</param>
        /// <param name="target">The Target Entity</param>
        /// <param name="orderType"></param>
        /// <param name="standoff">How close to the target entity we should get</param>
        public TranslationOrder(Guid faction, Guid entity, Guid target, HelmOrderTypeEnum orderType, double standoff)
            : base(faction, entity, target)
        {
            OrderType = orderType;
            StandOffDistance = standoff;
        }

        internal TranslationAction CreateAction(Game game, TranslationOrder order)
        {
            OrderEntities orderEntities;
            if (GetOrderEntities(game, order, out orderEntities))
            {
                return new TranslationAction(this, orderEntities, order.StandOffDistance);                         
            }
            //TODO: log don't throw, it's possible an entity could be destroyed by the time this happens.
            throw new Exception("couldn't find all required entites to create TranslationAction from TranslationOrder");
        }

        internal override BaseAction CreateAction(Game game, BaseOrder order)
        {
            return CreateAction(game, (TranslationOrder)order);
        }
    }
}
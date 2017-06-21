namespace Pulsar4X.ECSLib
{
    internal class  TranslationAction : BaseAction
    {
        internal TranslationOrder.HelmOrderTypeEnum HelmOrderType { get; set; }
        internal double StandOffDistance { get; set; } //this is the distance we want to Match or orbit at.
        internal PropulsionDB ThisPropulsionDB { get; set; }
        
        public TranslationAction(TranslationOrder order, OrderEntities orderEntities, double standoff) : 
            base(1, true, order, orderEntities.ThisEntity, orderEntities.FactionEntity, orderEntities.TargetEntity)
        {
            Name = "Move to " + TargetEntity.GetDataBlob<NameDB>().DefaultName;
            Status = "Waiting";
            OrderableProcessor = new TranslationActionProcessor();
            StandOffDistance = standoff;
            HelmOrderType = order.OrderType;
            ThisPropulsionDB = ThisEntity.GetDataBlob<PropulsionDB>();
        }
    }
}
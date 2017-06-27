namespace Pulsar4X.ECSLib
{
    internal class CargoAction : BaseAction
    {
        internal CargoStorageDB CargoFrom { get; set; }
        internal CargoStorageDB CargoTo { get; set; }
        internal CargoStorageDB ThisStorage { get; set; }
        public CargoAction(CargoOrder order, OrderEntities orderEntities, int amount) : 
            base(1, true, order, orderEntities.ThisEntity, orderEntities.FactionEntity, orderEntities.TargetEntity)
        {
            //set the orderableProcessor for cargoAction. 
            OrderableProcessor = new CargoActionProcessor();

            if (order.CargoOrderType == CargoOrder.CargoOrderTypes.LoadCargo)
                Name = "Cargo Transfer: Load from ";
            else
                Name = "Cargo Transfer: Unload To ";                       
            Name += TargetEntity.GetDataBlob<NameDB>().DefaultName;

            Status = "Waiting";
            //set local variables for cargoAction

            ThisStorage = ThisEntity.GetDataBlob<CargoStorageDB>();            
            switch (order.CargoOrderType)
            {
                case CargoOrder.CargoOrderTypes.LoadCargo:
                    CargoFrom = this.TargetEntity.GetDataBlob<CargoStorageDB>();
                    CargoTo = this.ThisEntity.GetDataBlob<CargoStorageDB>();
                    break;
                case CargoOrder.CargoOrderTypes.UnloadCargo:
                    CargoTo = this.TargetEntity.GetDataBlob<CargoStorageDB>();
                    CargoFrom = this.ThisEntity.GetDataBlob<CargoStorageDB>();
                    break;
            }
            
            ThisStorage.CurrentAction = this;
            ThisStorage.LastRunDate = ThisEntity.Manager.ManagerSubpulses.SystemLocalDateTime;
            
            ThisStorage.AmountToTransfer = amount;

            ThisStorage.OrderTransferItemGuid = order.CargoItemGuid;      
            OrderProcessor.SetNextInterupt(CargoActionProcessor.EstDateTime(this, ThisStorage), this);
        }
    }
}
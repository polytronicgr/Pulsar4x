using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class CargoOrder : BaseOrder
    {   
        public enum CargoOrderTypes
        {
            LoadCargo,
            UnloadCargo,
        }    
        [JsonProperty]
        public CargoOrderTypes CargoOrderType;
        [JsonProperty]
        public Guid CargoItemGuid;
        [JsonProperty]
        public int Amount;

        public CargoOrder(Guid entity, Guid faction, Guid target, CargoOrderTypes orderType, Guid cargoItemID, int amount) 
            : base(entity, faction, target)
        {
            CargoOrderType = orderType;
            CargoItemGuid = cargoItemID;
            Amount = amount;       
        }

        /// <summary>
        /// Creates a new CargoAction and sets it to the orderableEntity. 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cargoOrder"></param>
        internal CargoAction CreateAction(Game game, CargoOrder cargoOrder)
        {
            OrderEntities orderEntities;
            if (GetOrderEntities(game, cargoOrder, out orderEntities))
            {
                return new CargoAction(this, orderEntities, cargoOrder.Amount);                         
            }
            //TODO: log don't throw, it's possible an entity could be destroyed by the time this happens.
            throw new Exception("couldn't find all required entites to create cargoAction from cargoOrder");
        }

        internal override BaseAction CreateAction(Game game, BaseOrder order)
        {
            return CreateAction(game, (CargoOrder)order);
        }
    }
}
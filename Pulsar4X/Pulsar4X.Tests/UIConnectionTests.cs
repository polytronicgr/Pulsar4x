using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pulsar4X.ECSLib;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.Tests
{
    [TestFixture]
    public class UIConnectionTests
    {

        TestGame _testGame;
        private MineralSD _duraniumSD;
        
        private DateTime _currentDateTime {get { return _testGame.Game.CurrentDateTime; }}

        private BaseOrder _cargoOrder;
        
        [SetUp]
        public void Init()
        {
            _testGame = new TestGame(1);
            _duraniumSD = NameLookup.TryGetMineralSD(_testGame.Game, "Duranium");
            OrderableDB orderable = new OrderableDB();
            TestingUtilities.ColonyFacilitys(_testGame, _testGame.EarthColony);
            _testGame.EarthColony.Manager.SetDataBlob(_testGame.DefaultShip.ID, orderable);
            StorageSpaceProcessor.AddItemToCargo(_testGame.EarthColony.GetDataBlob<CargoStorageDB>(), _duraniumSD, 10000); 
            
            _cargoOrder = new CargoOrder(_testGame.DefaultShip.Guid, _testGame.HumanFaction.Guid, 
                                         _testGame.EarthColony.Guid, CargoOrderTypes.LoadCargo, 
                                         _duraniumSD.ID, 100);
        }
        

        [Test]
        public void SubscribeToDatablob()
        {

            Guid conectionID = new Guid();
            AuthenticationToken auth = new AuthenticationToken(_testGame.Game.SpaceMaster, "");
            
            _testGame.Game.MessagePump.UIConnections.Connections[Guid.Empty].DataSubsciber.Subscribe<CargoStorageDB>(_testGame.DefaultShip.Guid);


            BaseAction action = _cargoOrder.CreateAction(_testGame.Game, _cargoOrder);
            Assert.NotNull(action.OrderableProcessor);

            _testGame.EarthColony.Manager.OrderQueue.Enqueue(_cargoOrder);
            OrderProcessor.ProcessManagerOrders(_testGame.EarthColony.Manager);
            Assert.True(_testGame.DefaultShip.GetDataBlob<OrderableDB>().ActionQueue[0] is CargoAction);

            string message;
            Assert.True(_testGame.Game.MessagePump.LocalOutgoingQueue.TryPeekOutgoingMessage(out message));
            
        }


    }
}
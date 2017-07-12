using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using NUnit.Framework;
using Pulsar4X.ECSLib;
using Pulsar4X.ECSLib.DataSubscription;
using Pulsar4X.ViewModel;


namespace Pulsar4X.Tests
{
    [Description("Ship Entity Tests")]
    internal class OrderTests
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
            CargoStorageHelpers.AddItemToCargo(_testGame.EarthColony.GetDataBlob<CargoStorageDB>(), _duraniumSD, 10000); 
            
            _cargoOrder = new CargoOrder(_testGame.DefaultShip.Guid, _testGame.HumanFaction.Guid, 
                                                   _testGame.EarthColony.Guid, CargoOrder.CargoOrderTypes.LoadCargo, 
                                                   _duraniumSD.ID, 100);
        }

        [Test]
        public void TestCargoOrder()
        {
           
            
            BaseAction action = _cargoOrder.CreateAction(_testGame.Game, _cargoOrder);
            Assert.NotNull(action.OrderableProcessor);
            
            //enqueue it to the manager for now since the messagepump is still wip. 
            _testGame.EarthColony.Manager.OrderQueue.Enqueue(_cargoOrder); 
            OrderProcessor.ProcessManagerOrders(_testGame.EarthColony.Manager);
            Assert.True(_testGame.DefaultShip.GetDataBlob<OrderableDB>().ActionQueue[0] is CargoAction);

        }

        [Test]
        public void TestCargoMove()
        {
            //_testGame.GameSettings.EnableMultiThreading = false;
            EntityManager entityManager = _testGame.EarthColony.Manager;

            CargoStorageDB cargoStorageDB = _testGame.DefaultShip.GetDataBlob<CargoStorageDB>();
            
            Entity entity;
            Assert.True(_testGame.Game.GlobalManager.FindEntityByGuid(_testGame.DefaultShip.Guid, out entity));       
            
            _testGame.EarthColony.Manager.OrderQueue.Enqueue(_cargoOrder);             

            OrderProcessor.ProcessManagerOrders(_testGame.EarthColony.Manager);

            CargoAction cargoAction = (CargoAction)_testGame.DefaultShip.GetDataBlob<OrderableDB>().ActionQueue[0];
            DateTime eta = cargoAction.EstTimeComplete;
            DateTime nextStep = entityManager.ManagerSubpulses.EntityDictionary.ElementAt(0).Key;
            Assert.AreEqual(nextStep, eta, "check if eta & nextstep are equal");


            
            TimeSpan timeToTake = eta - _currentDateTime;
                                    
            Assert.Greater(nextStep, _currentDateTime, "nextStep should be greater than current datetime");

            long spaceAvailible = cargoStorageDB.StorageByType[_duraniumSD.CargoTypeID].Capacity;

            _testGame.Game.GameLoop.Ticklength = timeToTake;
            _testGame.Game.GameLoop.TimeStep();
            
            Assert.AreEqual(_currentDateTime, eta);
            long amountInShip = CargoStorageHelpers.GetAmountOf(cargoStorageDB, _duraniumSD.ID);   
            long amountOnColony = CargoStorageHelpers.GetAmountOf(_testGame.EarthColony.GetDataBlob<CargoStorageDB>(), _duraniumSD.ID); 
            long spaceRemaining = cargoStorageDB.StorageByType[_duraniumSD.CargoTypeID].FreeCapacity;
            Assert.AreEqual(100, amountInShip, "ship has " + amountInShip.ToString() + " Duranium");
            Assert.AreEqual(9900, amountOnColony, "colony should have duranium removed");

            Assert.AreEqual(0, _testGame.DefaultShip.GetDataBlob<OrderableDB>().ActionQueue.Count, "action should have been removed from queue");

        }

        [Test]
        public void TestOrderSeralisation()
        {            
            string seralisedOrder = ObjectSerializer.SerializeObject(_cargoOrder);
            BaseOrder deserailisedOrder = ObjectSerializer.DeserializeObject<BaseOrder>(seralisedOrder);
            Assert.True(deserailisedOrder is CargoOrder);
        }
        
        
        [Test]
        public void TestOrderViaMessagePump()
        {
            _testGame.GameSettings.EnableMultiThreading = false;
            ClientMessageHandler incommingMessageHandler = new ClientMessageHandler(_testGame.Game.MessagePump);
            FakeVM fakeVM = new FakeVM();
            SubscriptionRequestMessage<OrderableDB> subreq = new SubscriptionRequestMessage<OrderableDB>()
            {
                ConnectionID = Guid.Empty, 
                EntityGuid = _testGame.DefaultShip.Guid,             
            };

            incommingMessageHandler.Subscribe(subreq, fakeVM);            
            
            _testGame.Game.MessagePump.EnqueueIncomingMessage(_cargoOrder);
            _testGame.Game.GameLoop.Ticklength = TimeSpan.FromHours(1);
            BaseToClientMessage message;
            while (!_testGame.Game.MessagePump.TryPeekOutgoingMessage(Guid.Empty, out message))
            {
                _testGame.Game.GameLoop.TimeStep();
                OrderProcessor.ProcessManagerOrders(_testGame.EarthColony.Manager);
                OrderProcessor.ProcessActionList(_testGame.Game.CurrentDateTime, _testGame.EarthColony.Manager);
            }
            
            incommingMessageHandler.Read();
            Assert.IsTrue(fakeVM.OrderStatus == "In Progress ", "Wrong Order Status");
            Assert.IsTrue(fakeVM.Name == "Cargo Transfer: Load from Earth Colony", "Wrong Order Name");
        }
        
        public class FakeVM : IHandleMessage
        {
            public string OrderStatus { get; set; }
            public string Name { get; set; }
            public void Update(BaseToClientMessage message)
            {
                DatablobChangedMessage changeMessage = (DatablobChangedMessage)message;
                foreach (OrderableDataChange change in changeMessage.Changes)
                {   
                    OrderStatus = change.Status;
                    Name = change.Name;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pulsar4X.ECSLib;
using Pulsar4X.ECSLib.DataSubscription;
using Pulsar4X.ViewModel;

namespace Pulsar4X.Tests
{
    [TestFixture]
    public class DataSubscriptionTests
    {

        TestGame _testGame;
        private MineralSD _duraniumSD;

        private DateTime _currentDateTime
        {
            get { return _testGame.Game.CurrentDateTime; }
        }

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

            _cargoOrder = new CargoOrder(_testGame.DefaultShip.Guid, _testGame.HumanFaction.Guid, _testGame.EarthColony.Guid, CargoOrder.CargoOrderTypes.LoadCargo, _duraniumSD.ID, 100);
        }


        [Test]
        public void SubscribeToUIData()
        {

            Guid conectionID = new Guid();
            AuthenticationToken auth = new AuthenticationToken(_testGame.Game.SpaceMaster, "");



            _testGame.Game.MessagePump.DataSubscibers[Guid.Empty].Subscribe<CargoStorageUIData>(_testGame.DefaultShip.Guid);
            //_testGame.Game.MessagePump.DataSubscibers[Guid.Empty].Subscribe<OrderableDB>(_testGame.DefaultShip.Guid);

            Assert.True(_testGame.Game.MessagePump.DataSubscibers[Guid.Empty].IsSubscribedTo<CargoStorageUIData>(_testGame.DefaultShip.Guid), "not subscribed");
            Assert.True(_testGame.Game.MessagePump.AreAnySubscribers<CargoStorageUIData>(_testGame.DefaultShip.Guid), "No subscribers");
            BaseAction action = _cargoOrder.CreateAction(_testGame.Game, _cargoOrder);
            Assert.NotNull(action.OrderableProcessor);

            _testGame.EarthColony.Manager.OrderQueue.Enqueue(_cargoOrder);
            OrderProcessor.ProcessManagerOrders(_testGame.EarthColony.Manager);
            Assert.True(_testGame.DefaultShip.GetDataBlob<OrderableDB>().ActionQueue[0] is CargoAction, "No action in ship orders");
            _testGame.Game.GameLoop.Ticklength = TimeSpan.FromSeconds(10);
            _testGame.Game.GameLoop.TimeStep();
            OrderProcessor.ProcessActionList(_testGame.Game.CurrentDateTime, _testGame.EarthColony.Manager);


            BaseToClientMessage clientMessage;
            Assert.True(_testGame.Game.MessagePump.TryDequeueOutgoingMessage(Guid.Empty, out clientMessage), "Message not in pump");
            Assert.True(clientMessage is CargoStorageUIData);
        }

        [Test]
        public void SubscribeViaMessage()
        {
            SubscriptionRequestMessage<CargoStorageUIData> subreq = new SubscriptionRequestMessage<CargoStorageUIData>() {ConnectionID = Guid.Empty, EntityGuid = _testGame.DefaultShip.Guid};

            _testGame.Game.MessagePump.EnqueueIncomingMessage(ObjectSerializer.SerializeObject(subreq));
            _testGame.Game.GameLoop.TimeStep();
            Assert.True(_testGame.Game.MessagePump.AreAnySubscribers<CargoStorageUIData>(_testGame.DefaultShip.Guid));
        }

        [Test]
        public void TestClientHandler()
        {
            ClientMessageHandler incommingMessageHandler = new ClientMessageHandler(_testGame.Game.MessagePump);
            FakeVM fakeVM = new FakeVM();
            SubscriptionRequestMessage<CargoStorageUIData> subreq = new SubscriptionRequestMessage<CargoStorageUIData>()
            {
                ConnectionID = Guid.Empty, 
                EntityGuid = _testGame.DefaultShip.Guid,             
            };

            incommingMessageHandler.Subscribe(subreq, fakeVM);            
            
            _testGame.Game.MessagePump.EnqueueIncomingMessage(_cargoOrder);
            
            BaseToClientMessage message;
            while (!_testGame.Game.MessagePump.TryPeekOutgoingMessage(Guid.Empty, out message))
            {
                _testGame.Game.GameLoop.TimeStep();
            }
            
            incommingMessageHandler.Read();
            Assert.IsTrue(fakeVM.IsHandled);
        }
        
        public class FakeVM : IHandleMessage
        {
            public bool IsHandled { get; set; } = false;
            public void Update(BaseToClientMessage message) { IsHandled = true; }
        }
    }
}
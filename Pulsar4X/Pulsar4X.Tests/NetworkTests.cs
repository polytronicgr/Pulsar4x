using System;
using System.Collections.Generic;
using Pulsar4X.ECSLib;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Pulsar4X.Networking;


namespace Pulsar4X.Tests
{
    using NUnit.Framework;

    [TestFixture, Description("Tests network related stuff")]
    class NetworkTests
    {
        private Game _gameHost;
        private Game _gameClient;
        private readonly DateTime testTime = DateTime.Now;
        private Entity _humanFaction;
        private NetworkHost _nethost;
        private NetworkClient _netClient;
        [SetUp]
        public void Init()
        {
            //sets SyncronisationContext since the networking uses this (UI threads typicaly have this already)
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            _gameHost = Game.NewGame("Unit Test Game", testTime, 1);         
            _nethost = new NetworkHost(28888);
            //add human faction, sol, etc.:
            _humanFaction = DefaultStartFactory.DefaultHumans(_gameHost, "New Terran Utopian Empire");
            //Entity humanSpecies = _humanFaction.GetDataBlob<FactionInfoDB>().Species[0];
            _nethost.Game = _gameHost;
            _nethost.ServerStart();
            
            _gameClient = Game.NewGame("Unit Test Game", testTime, 0, null, false);
            _netClient = new NetworkClient("localhost", 28888);
            _netClient.Game = _gameClient;

            
            _netClient.ClientConnect();
        }

        [TearDown]
        public void Cleanup()
        {
            _gameHost = null;
            _gameClient = null;
            _humanFaction = null;
            _nethost.Shutdown("");
            _netClient.Shutdown("");
            _nethost = null;
            _netClient = null;
        }

        [Test]
        public void TestFactionEntitySerialisation()
        {
            var mStream = new MemoryStream();

            SaveGame.ExportEntity(_humanFaction, mStream);

            byte[] entityByteArray = mStream.ToArray();
            var mStream2 = new MemoryStream(entityByteArray);

            mStream2.Position = 0;
            var sr = new StreamReader(mStream2);
            var myStr = Encoding.ASCII.GetString(mStream2.ToArray());
            mStream2.Position = 0;


            Entity testEntity = SaveGame.ImportEntity(_gameClient, _gameClient.GlobalManager, mStream2);

            Assert.IsTrue(testEntity.HasDataBlob<NameDB>());
            Assert.AreEqual(_humanFaction.DataBlobs.Count, testEntity.DataBlobs.Count);
        }


        [Test]
        public void TestSystemSerialisation()
        {
            
            var mStream = new MemoryStream();

            SaveGame.ExportStarSystem(_gameHost.Systems[0], mStream);

            byte[] entityByteArray = mStream.ToArray();
            var mStream2 = new MemoryStream(entityByteArray);

            mStream2.Position = 0;
            var sr = new StreamReader(mStream2);
            var myStr = Encoding.ASCII.GetString(mStream2.ToArray());
            mStream2.Position = 0;

            int syscount = _gameClient.Systems.Count;

            StarSystem testSystem = SaveGame.ImportStarSystem(_gameClient, mStream2);

            Assert.IsTrue(testSystem.NameDB.DefaultName == _gameHost.Systems[0].NameDB.DefaultName);
            Assert.AreEqual(testSystem.SystemManager.Entities.Count, _gameHost.Systems[0].SystemManager.Entities.Count);
            Assert.IsTrue(_gameClient.Systems.Count >= syscount);
        }

        //[Test]
        //public void TestTurnProcessing()
        //{
        //    _netClient.SendFactionDataRequest("New Terran Utopian Empire", "");


        //}

    }
}

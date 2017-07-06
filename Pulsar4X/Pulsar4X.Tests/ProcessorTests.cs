using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using NUnit.Framework;
using Pulsar4X.ECSLib;

namespace Pulsar4X.Tests
{
    [TestFixture, Description("Processor Tests")]
    class ProcessorTests
    {
        private Game _game;
        EntityManager _entityManager;
        private Entity _faction;
        private Entity _colonyEntity;
        private Entity _earth;
        private MineralSD _duraniumSD;
        TestGame _testGame;

        private DateTime _currentDateTime
        {
            get { return _testGame.Game.CurrentDateTime; }
        }

        private CargoStorageDB _cargoStorageDB;
        private Guid DuraniumID => _duraniumSD.ID;
        private Guid DuraniumCargoTypeID => _duraniumSD.CargoTypeID;
        
        
        [SetUp]
        public void Init()
        {
            _testGame = new TestGame(1);
            _duraniumSD = NameLookup.TryGetMineralSD(_testGame.Game, "Duranium");

            TestingUtilities.ColonyFacilitys(_testGame, _testGame.EarthColony);

            _cargoStorageDB = _testGame.EarthColony.GetDataBlob<CargoStorageDB>();

            _colonyEntity = _testGame.EarthColony;
            _faction = _testGame.HumanFaction;
            _game = _testGame.Game;
            _entityManager = _colonyEntity.Manager;
            _earth = _testGame.Earth;
        }

        [Test]
        public void TestMining()
        {
            Assert.True(_earth.GetDataBlob<SystemBodyInfoDB>().Minerals.ContainsKey(DuraniumID), "Planet doesn't have minerals");
            Assert.True(_colonyEntity.GetDataBlob<ColonyMinesDB>().MineingRate[DuraniumID] > 0, "Colony is incapable of mining");
            Assert.True(_colonyEntity.GetDataBlob<CargoStorageDB>().StorageByType[DuraniumCargoTypeID].FreeCapacity > 0, "Colony can't store minerals");
            MineProcessor.MineResources(_colonyEntity);
            
            Assert.True(_colonyEntity.GetDataBlob<CargoStorageDB>().StorageByType[DuraniumCargoTypeID].StoredByItemID[DuraniumID] > 0, "Should have Duranum in cargo");
        }

    }
}





//        [TearDown]
//        public void Cleanup()
//        {
//            _game = null;
//            _entityManager = null;
//            _faction = null;
//            _colonyEntity = null;
//            StaticDataManager.ClearAllData();
//        }

//        [Test]
//        public void TestMineing()
//        {
//            //first with no population;
//            Entity colonyEntity = _faction.GetDataBlob<FactionDB>().Colonies[0];
//            InstallationsDB installations = colonyEntity.GetDataBlob<InstallationsDB>();
//            Dictionary<Guid, float> mineralstockpile = colonyEntity.GetDataBlob<ColonyInfoDB>().MineralStockpile;
//            Dictionary<Guid, float> mineralstockpilePreMined = new Dictionary<Guid, float>(mineralstockpile);


//            InstallationProcessor.Employment(colonyEntity); //do employment check;
//            InstallationProcessor.Mine(_faction, colonyEntity); //run mines

//            Assert.AreEqual(mineralstockpile[_corundiumSD.ID], 0);
//            Assert.AreEqual(mineralstockpile[_duraniumSD.ID], 0);


//            ColonyInfoDB colonyInfo = colonyEntity.GetDataBlob<ColonyInfoDB>();
//            Dictionary<Entity, long> pop = colonyInfo.Population;
//            var species = pop.Keys.ToList();
//            colonyInfo.Population[species[0]] = 5; //5mil pop

//            InstallationProcessor.Employment(colonyEntity); //do employment check;
//            InstallationProcessor.Mine(_faction, colonyEntity); //run mines

//            Assert.AreNotEqual(mineralstockpile[_corundiumSD.ID], 10);
//            Assert.AreNotEqual(mineralstockpile[_duraniumSD.ID], 5);

//        }

//        [Test]
//        public void TestConstruction()
//        {
//            ColonyInfoDB colonyInfo = _colonyEntity.GetDataBlob<ColonyInfoDB>();
//            Guid itemConstructing = new Guid();//just a random guid for now.
//            double ablityPointsThisColony = 100;
//            List<ConstructionJob> jobList = new List<ConstructionJob>();

//            Dictionary<Guid,float> stockpileOut = new Dictionary<Guid, float>();

//            PercentValue priority = new PercentValue {Percent = 1};
//            Dictionary<Guid,int> jobRawMaterials = new Dictionary<Guid, int>();
//            jobRawMaterials.Add(_duraniumSD.ID, 5000); //500 per item
//            jobRawMaterials.Add(_corundiumSD.ID, 70); //7 per item
//            ConstructionJob newJob = new ConstructionJob 
//            {
//                Type = itemConstructing,  
//                ItemsRemaining = 10, 
//                PriorityPercent = priority,
//                RawMaterialsRemaining = jobRawMaterials,
//                BuildPointsRemaining = 1000,
//                BuildPointsPerItem = 100
//            };
//            jobList.Add(newJob);

//            colonyInfo.MineralStockpile.Add(_duraniumSD.ID, 2250); //not enough of this should get 4.5  total installations. 
//            colonyInfo.MineralStockpile.Add(_corundiumSD.ID, 100); //enough of this
//            stockpileOut.Add(itemConstructing,0);


//            //firstpass 
//            InstallationProcessor.GenericConstructionJobs(0, jobList, colonyInfo, stockpileOut);
//            Assert.AreEqual(0, stockpileOut[itemConstructing], "Should not have constructed anything due to no buildpoints");
//            Assert.AreEqual(2250, colonyInfo.MineralStockpile[_duraniumSD.ID], "Mineral Usage Incorrect");
//            Assert.AreEqual(100, colonyInfo.MineralStockpile[_corundiumSD.ID], "Mineral Usage Incorrect");

//            //todo: fix floating point math.

//            //secondPass
//            InstallationProcessor.GenericConstructionJobs(100, jobList, colonyInfo, stockpileOut);
//            Assert.AreEqual(1, stockpileOut[itemConstructing]);

//            //thirdPass
//            //InstallationProcessor.GenericConstructionJobs(50, jobList, colonyInfo, stockpileOut);
//            //Assert.AreEqual(1.5, stockpileOut[itemConstructing]);            

//            //fourthPass
//            //InstallationProcessor.GenericConstructionJobs(5000, jobList, colonyInfo, stockpileOut);
//            //Assert.AreEqual(4.5, stockpileOut[itemConstructing]);

//            //todo there's probilby some edge cases to check.
//        }

//        [Test]
//        public void TestEconTick()
//        {
//            Game.Instance.AdvanceTime(68300);
//            Game.Instance.AdvanceTime(100);
//            Game.Instance.AdvanceTime(68400);
//        }
//    }
//}

using System;
using NUnit.Framework;
using Pulsar4X.ECSLib;

namespace Pulsar4X.Tests
{
    [TestFixture]
    public class CargoTests
    {
        TestGame _testGame;
        private MineralSD _duraniumSD;
        
        private DateTime _currentDateTime {get { return _testGame.Game.CurrentDateTime; }}

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
            



        }

        [Test]
        public void TestCargoAdd()
        {
            long freeSpace = _cargoStorageDB.StorageByType[DuraniumCargoTypeID].FreeCapacity;
            CargoStorageHelpers.AddItemToCargo(_cargoStorageDB, _duraniumSD, 10000);
            Assert.True(_cargoStorageDB.StorageByType[DuraniumCargoTypeID].StoredByItemID[DuraniumID] == 10000);
            Assert.False(_cargoStorageDB.StorageByType[DuraniumCargoTypeID].FreeCapacity == freeSpace, "free space not realocated" );
            
            long cargoWeight = (long)(_duraniumSD.Mass * 10000);
            Assert.AreEqual(freeSpace - cargoWeight, _cargoStorageDB.StorageByType[DuraniumCargoTypeID].FreeCapacity, "Free space incorrectly allocated");
        }
        
        [Test]
        public void TestCargoRemove()
        {
            
            CargoStorageHelpers.AddItemToCargo(_cargoStorageDB, _duraniumSD, 10000);
            long freeSpace = _cargoStorageDB.StorageByType[DuraniumCargoTypeID].FreeCapacity;
            
            CargoStorageHelpers.RemoveItemFromCargo(_cargoStorageDB, _duraniumSD, 5000);
            Assert.True(_cargoStorageDB.StorageByType[DuraniumCargoTypeID].StoredByItemID[DuraniumID] == 5000, "Incorrect Amount in cargo");
            Assert.False(_cargoStorageDB.StorageByType[DuraniumCargoTypeID].FreeCapacity == freeSpace, "free space not realocated" );
            long cargoWeight = (long)(_duraniumSD.Mass * 5000);
            Assert.AreEqual(freeSpace + cargoWeight, _cargoStorageDB.StorageByType[DuraniumCargoTypeID].FreeCapacity, "Free space incorrectly allocated");
        }
    }
}
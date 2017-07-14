using System;
using System.Collections.Generic;
using NUnit.Framework;
using Pulsar4X.ECSLib;
using Pulsar4X.ECSLib.DataSubscription;

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
            DataSubscriber dataSubsriber2 = new DataSubscriber(_testGame.Game, Guid.Empty);
            dataSubsriber2.Subscribe<CargoStorageDB>(_cargoStorageDB.OwningEntity.Guid);
            //_cargoStorageDB.HasSubscribers = true;
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

            CargoDataChange change = (CargoDataChange)_cargoStorageDB.Changes[0];
            
            Assert.True(change.Amount == 10000);
            Assert.True(change.ChangeType == CargoDataChange.CargoChangeTypes.AmountChange);
            List<Entity> subscribedEntities = _cargoStorageDB.OwningEntity.Manager.GetAllEntitiesWithDataBlob<SubscribedEntityDB>();
            Assert.True(subscribedEntities.Count == 1);
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
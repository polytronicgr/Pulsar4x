using System;
using System.Collections.Generic;
using Pulsar4X.ECSLib;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pulsar4X.Tests
{
    using NUnit.Framework;

    [TestFixture, Description("Tests the game Save/Load system.")]
    class SaveGameTests
    {
        private Game _game;
        private const string file = "./testSave.json";
        private readonly DateTime testTime = DateTime.Now;
        private Entity _humanFaction;

        [SetUp]
        public void Init()
        {
            _game = Game.NewGame("Unit Test Game", testTime, 1);

            //// add a faction:
            //_humanFaction = FactionFactory.CreateFaction(_game, "New Terran Utopian Empire", "password");

            //// add a species:
            //Entity humanSpecies = SpeciesFactory.CreateSpeciesHuman(_humanFaction, _game.GlobalManager);

            //add human faction, sol, etc.:
            _humanFaction = DefaultStartFactory.DefaultHumans(_game, "New Terran Utopian Empire");

            Entity humanSpecies = _humanFaction.GetDataBlob<FactionInfoDB>().Species[0];

            // add another faction:
            Entity greyAlienFaction = FactionFactory.CreateFaction(_game, "The Grey Empire", "password");
            // Add another species:
            Entity greyAlienSpecies = SpeciesFactory.CreateSpeciesHuman(greyAlienFaction, _game.GlobalManager);

            // Greys Name the Humans.
            humanSpecies.GetDataBlob<NameDB>().SetName(greyAlienFaction, "Stupid Terrans");
            // Humans name the Greys.
            greyAlienSpecies.GetDataBlob<NameDB>().SetName(_humanFaction, "Space bugs");
        }

        [TearDown]
        public void Cleanup()
        {
            // cleanup the test file:
            if (File.Exists(file))
            {
                //File.Delete(file); 
            }

            _game = null;
        }

        [Test]
        public void TestSaveLoad()
        {
            // lets create a bad save game:

            // Check default nulls throw:
            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                SaveGame.Save(null, file);
            });
            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                SaveGame.Save(_game, (string)null);
            });
            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                SaveGame.Load(null);
            });

            // check provided empty string throws:
            const string emptyString = "";
            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                SaveGame.Save(_game, emptyString);
            });
            Assert.Catch(typeof(ArgumentNullException), () =>
            {
                SaveGame.Load(emptyString);
            });

            // lets create a good saveGame
            SaveGame.Save(_game, file);

            Assert.IsTrue(File.Exists(file));

            // now lets give ourselves a clean game:
            _game = null;

            //and load the saved data:
            _game = SaveGame.Load(file);

            Assert.AreEqual(2, _game.Systems.Count);
            Assert.AreEqual(testTime, _game.CurrentDateTime);
            List<Entity> entities = _game.GlobalManager.GetAllEntitiesWithDataBlob<FactionInfoDB>();
            Assert.AreEqual(3, entities.Count);
            entities = _game.GlobalManager.GetAllEntitiesWithDataBlob<SpeciesDB>();
            Assert.AreEqual(2, entities.Count);

            // lets check the the refs were hocked back up:
            Entity species = _game.GlobalManager.GetFirstEntityWithDataBlob<SpeciesDB>();
            NameDB speciesName = species.GetDataBlob<NameDB>();
            Assert.AreSame(speciesName.OwningEntity, species);

            //check that systemManager reference in factionInfo is getting populated:
            Entity humanFactionEntity = _game.GlobalManager.GetEntityByGuid(_humanFaction.Guid);
            FactionInfoDB humanInfo = humanFactionEntity.GetDataBlob<FactionInfoDB>();

            List<Entity> systemManagerEntityes = Misc.LookupStarSystem(_game.Systems, humanInfo.KnownSystems[0]).SystemManager.Entities.ToList();
            foreach (var entity in systemManagerEntityes)
            {
                Assert.NotNull(entity);
            }
            // <?TODO: Expand this out to cover many more DBs, entities, and cases.
        }

        [Test]
        public void TestSingleSystemSave()
        {
            StarSystemFactory starsysfac = new StarSystemFactory(_game);
            StarSystem sol  = starsysfac.CreateSol(_game);
            StaticDataManager.ExportStaticData(sol, "./solsave.json");
        }


    }
}

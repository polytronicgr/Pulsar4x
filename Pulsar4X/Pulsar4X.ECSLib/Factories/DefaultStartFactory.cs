using System;
using System.Collections.Generic;

namespace Pulsar4X.ECSLib
{
    public static class DefaultStartFactory
    {
        public static Entity DefaultHumans(Game game, Player owner, string name)
        {
            StarSystemFactory starfac = new StarSystemFactory(game);
            StarSystem sol = starfac.CreateSol(game);
            Entity earth = sol.SystemManager.Entities[3]; //should be fourth entity created 
            Entity factionEntity = FactionFactory.CreatePlayerFaction(game, owner, name);
            Entity speciesEntity = SpeciesFactory.CreateSpeciesHuman(factionEntity, game.GlobalManager);
            Entity colonyEntity = ColonyFactory.CreateColony(factionEntity, speciesEntity, earth);

            ComponentTemplateSD mineSD = game.StaticData.Components[new Guid("f7084155-04c3-49e8-bf43-c7ef4befa550")];
            ComponentDesign mineDesign = GenericComponentFactory.StaticToDesign(mineSD, factionEntity.GetDataBlob<FactionTechDB>(), game.StaticData);
            Entity mineEntity = GenericComponentFactory.DesignToEntity(game, factionEntity, mineDesign);


            ComponentTemplateSD refinerySD = game.StaticData.Components[new Guid("90592586-0BD6-4885-8526-7181E08556B5")];
            ComponentDesign refineryDesign = GenericComponentFactory.StaticToDesign(refinerySD, factionEntity.GetDataBlob<FactionTechDB>(), game.StaticData);
            Entity refineryEntity = GenericComponentFactory.DesignToEntity(game, factionEntity, refineryDesign);

            ComponentTemplateSD labSD = game.StaticData.Components[new Guid("c203b7cf-8b41-4664-8291-d20dfe1119ec")];
            ComponentDesign labDesign = GenericComponentFactory.StaticToDesign(labSD, factionEntity.GetDataBlob<FactionTechDB>(), game.StaticData);
            Entity labEntity = GenericComponentFactory.DesignToEntity(game, factionEntity, labDesign);

            ComponentTemplateSD factorySD = game.StaticData.Components[new Guid("{07817639-E0C6-43CD-B3DC-24ED15EFB4BA}")];
            ComponentDesign factoryDesign = GenericComponentFactory.StaticToDesign(factorySD, factionEntity.GetDataBlob<FactionTechDB>(), game.StaticData);
            Entity facoryEntity = GenericComponentFactory.DesignToEntity(game, factionEntity, factoryDesign);

            Entity scientistEntity = CommanderFactory.CreateScientist(game.GlobalManager, factionEntity);
            MatedToDB.MateEntities(colonyEntity, scientistEntity);

            FactionTechDB factionTech = factionEntity.GetDataBlob<FactionTechDB>();
            //TechProcessor.ApplyTech(factionTech, game.StaticData.Techs[new Guid("35608fe6-0d65-4a5f-b452-78a3e5e6ce2c")]); //add conventional engine for testing. 
            TechProcessor.MakeResearchable(factionTech);

            var componentInstancesDB = colonyEntity.GetDataBlob<ComponentInstancesDB>();

            var mineInstances = new List<ComponentInstance> {new ComponentInstance(mineEntity)};
            componentInstancesDB.specificInstances.Add(mineEntity, mineInstances);
            var refineryInstances = new List<ComponentInstance> { new ComponentInstance(refineryEntity) };
            componentInstancesDB.specificInstances.Add(refineryEntity, refineryInstances);
            var labInstances = new List<ComponentInstance> { new ComponentInstance(labEntity) };
            componentInstancesDB.specificInstances.Add(labEntity, labInstances);
            var factoryInstances = new List<ComponentInstance> { new ComponentInstance(facoryEntity) };
            componentInstancesDB.specificInstances.Add(facoryEntity, factoryInstances);

            ReCalcProcessor.ReCalcAbilities(colonyEntity);
            colonyEntity.GetDataBlob<ColonyInfoDB>().population[speciesEntity] = 9000000000;
            
            factionEntity.GetDataBlob<FactionInfoDB>().KnownSystems.Add(sol.Guid);

            return factionEntity;
        }
    }

}
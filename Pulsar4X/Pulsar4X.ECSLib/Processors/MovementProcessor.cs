using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pulsar4X.ECSLib.DataBlobs;

namespace Pulsar4X.ECSLib
{
    internal static class MovementProcessor
    {
        public static void Initialize()
        {
        }

        /// <summary>
        /// Sets a ships position.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="systems"></param>
        /// <param name="deltaSeconds"></param>
        public static void Process(Game game, List<StarSystem> systems, int deltaSeconds)
        {
            if (game.Settings.EnableMultiThreading ?? false)
            {
                Parallel.ForEach(systems, system => ProcessSystem(game, system, deltaSeconds));
            }
            else
            {
                foreach (var system in systems)
                {
                    ProcessSystem(game, system, deltaSeconds);
                }
            }


        }

        private static void ProcessSystem(Game game, StarSystem system, int deltaSeconds)
        {

            foreach (Entity shipEntity in system.SystemManager.GetAllEntitiesWithDataBlob<PropulsionDB>())
            {
                shipEntity.GetDataBlob<PositionDB>().Position += shipEntity.GetDataBlob<PropulsionDB>().CurrentSpeed * deltaSeconds;
            }
        }


        /// <summary>
        /// recalculates a shipsMaxSpeed.
        /// </summary>
        /// <param name="ship"></param>
        public static void CalcMaxSpeed(Entity ship)
        {
            int totalEnginePower = 0;

            List<Entity> engineEntities = ship.GetDataBlob<ShipInfoDB>().ComponentList.Where(item => item.HasDataBlob<EnginePowerAbilityDB>()).ToList();
            foreach (var engine in engineEntities)
            {
                //todo check if it's damaged
                totalEnginePower += engine.GetDataBlob<EnginePowerAbilityDB>().EnginePower;
            }

            //Note: TN aurora uses the TCS for max speed calcs. 
            ship.GetDataBlob<PropulsionDB>().MaximumSpeed = (int)(totalEnginePower / ship.GetDataBlob<ShipInfoDB>().Tonnage) * 20;
        }

        private static void UpdateMatedEntities(StarSystem system)
        {
            foreach (Entity entity in system.SystemManager.GetAllEntitiesWithDataBlob<MatedToDB>())
            {
                var matedDB = entity.GetDataBlob<MatedToDB>();


            }
        }

        public static void UpdateEntityPosition(ProtoEntity entity, PositionDB newPositionDB)
        {
            var oldPositionDB = entity.GetDataBlob<PositionDB>();
            if (oldPositionDB == null)
            {
                throw new InvalidOperationException("Cannot update position of an entity with no PositionDB");
            }

            entity.SetDataBlob(newPositionDB);

            var matedToDB = entity.GetDataBlob<MatedToDB>();

            if (matedToDB == null)
            {
                return;
            }

            foreach (Entity child in matedToDB.Children)
            {
                // Recusivly update all children position.
                UpdateEntityPosition(child, newPositionDB);
            }
        }
    }
}
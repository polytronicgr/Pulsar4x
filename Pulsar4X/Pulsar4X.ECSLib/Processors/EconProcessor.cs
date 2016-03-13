using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    internal class EconProcessor
    {
        [JsonProperty]
        private DateTime _lastRun = DateTime.MinValue;
        internal readonly IndustrySubprocessor IndustrySubprocessor;
        internal DateTime _nextEvent { get; private set; } = DateTime.MaxValue;

        internal EconProcessor(Game game)
        {
            IndustrySubprocessor = new IndustrySubprocessor(game);
        }


        internal void Process(Game game, List<StarSystem> systems, int deltaSeconds)
        {
            if (game.CurrentDateTime - _lastRun < game.Settings.EconomyCycleTime)
            {
                return;
            }

            _lastRun = game.CurrentDateTime;

            if (game.Settings.EnableMultiThreading ?? false)
            {
                Parallel.ForEach(systems, system => ProcessSystem(game, system));
            }
            else
            {
                foreach (var system in systems)
                {
                    ProcessSystem(game, system);
                }
            }
        }

        private void ProcessSystem(Game game, StarSystem system)
        {
            IndustrySubprocessor.Process(system);

            if (IndustrySubprocessor.NextHaltTime < _nextEvent)
            {
                _nextEvent = IndustrySubprocessor.NextHaltTime;
            }

            foreach (Entity colonyEntity in system.SystemManager.GetAllEntitiesWithDataBlob<RuinsDB>())
            {
                // TODO: Process Ruins
            }
            
            // TODO: Process Trade Goods
        }
    }
}
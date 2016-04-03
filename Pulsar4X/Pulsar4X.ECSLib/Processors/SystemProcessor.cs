using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    internal abstract class SystemProcessor : Processor
    {
        internal override void Process(Game game)
        {
            List<StarSystem> systems = game.Systems.Values.ToList();

            for (int i = systems.Count - 1; i >= 0; i--)
            {
                StarSystem starSystem = systems[i];
                // TODO: Eject invalid systems.
            }

            if (UseMultiThreading)
            {
                Parallel.ForEach(systems, starSystem => ProcessSystem(game, starSystem));
            }
            else
            {
                foreach (StarSystem starSystem in systems)
                {
                    ProcessSystem(game, starSystem);
                }
            }
        }

        internal abstract void ProcessSystem(Game game, StarSystem system);
    }

    internal abstract class TimedSystemProcessor : SystemProcessor
    {
        [JsonProperty]
        private readonly Dictionary<Guid, DateTime> _lastRunTimes = new Dictionary<Guid, DateTime>();

        [JsonProperty]
        private TimeSpan RunFrequency { get; set; }

        protected TimedSystemProcessor(TimeSpan runFrequency)
        {
            RunFrequency = runFrequency;
        }

        internal sealed override void ProcessSystem(Game game, StarSystem system)
        {
            DateTime lastRunTime;
            if (!_lastRunTimes.TryGetValue(system.Guid, out lastRunTime))
            {
                lastRunTime = DateTime.MinValue;
                _lastRunTimes.Add(system.Guid, lastRunTime);
            }

            if (lastRunTime + RunFrequency > game.CurrentDateTime)
            {
                return;
            }

            _lastRunTimes[system.Guid] = game.CurrentDateTime;

            TimedProcessSystem(game, system);
        }

        internal abstract void TimedProcessSystem(Game game, StarSystem system);
    }
}

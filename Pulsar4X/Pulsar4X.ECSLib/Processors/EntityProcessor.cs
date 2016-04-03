using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    internal abstract class EntityProcessor : SystemProcessor
    {
        private readonly EntityFilter _processorFilter;

        internal EntityProcessor(EntityFilter filter)
        {
            _processorFilter = filter;
        }

        internal override void ProcessSystem(Game game, StarSystem system)
        {
            foreach (Entity entity in system.SystemManager.Entities)
            {
                if (_processorFilter.MatchesEntity(entity))
                {
                    ProcessEntity(game, system, entity);
                }
            }
        }

        protected abstract void ProcessEntity(Game game, StarSystem system, ProtoEntity entity);
    }

    internal abstract class TimedEntityProcessor : EntityProcessor
    {
        [JsonProperty]
        private DateTime _lastRunTime = DateTime.MinValue;
        [JsonProperty]
        private TimeSpan RunFrequency { get; }

        protected TimedEntityProcessor(EntityFilter filter, TimeSpan runFrequency) : base(filter)
        {
            RunFrequency = runFrequency;
        }

        internal override void ProcessSystem(Game game, StarSystem system)
        {
            if (_lastRunTime + RunFrequency > game.CurrentDateTime)
            {
                return;
            }

            _lastRunTime = game.CurrentDateTime;
            base.ProcessSystem(game, system);
        }
    }
}

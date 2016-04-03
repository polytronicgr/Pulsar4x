using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    internal abstract class SystemProcessor
    {
        internal bool UseMultiThreading { get; set; }

        internal int Layer { get; set; }

        internal abstract void ProcessSystem(Game game, StarSystem system);
    }

    internal abstract class TimedSystemProcessor : SystemProcessor
    {
        [JsonProperty]
        private DateTime _lastRunTime = DateTime.MinValue;
        [JsonProperty]
        private TimeSpan RunFrequency { get; set; }

        protected TimedSystemProcessor(TimeSpan runFrequency)
        {
            RunFrequency = runFrequency;
        }

        internal sealed override void ProcessSystem(Game game, StarSystem system)
        {
            if (_lastRunTime + RunFrequency > game.CurrentDateTime)
            {
                return;
            }

            _lastRunTime = game.CurrentDateTime;
            Process(game, system);
        }

        protected abstract void Process(Game game, StarSystem system);
    }
}

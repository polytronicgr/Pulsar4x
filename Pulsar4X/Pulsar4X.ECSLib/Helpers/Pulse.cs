using System;

namespace Pulsar4X.ECSLib
{
    public class PulseInterrupt
    {
        
        public Type RequestingProcessor { get; internal set; }

        
        public string Reason { get; internal set; }
    }

    public class SubpulseLimit
    {
        
        public int MaxSeconds { get; internal set; }

        
        public Type RequestingProcessor { get; internal set; }

        
        public string Reason { get; internal set; }

        internal SubpulseLimit()
        {
            MaxSeconds = int.MaxValue;
            RequestingProcessor = null;
            Reason = string.Empty;
        }
    }
}

using System;

namespace Pulsar4X.ECSLib
{
    public abstract class Processor
    {
        internal bool UseMultiThreading { get; set; }

        internal int Layer { get; set; }

        internal abstract void Process(Game game);
    }
}

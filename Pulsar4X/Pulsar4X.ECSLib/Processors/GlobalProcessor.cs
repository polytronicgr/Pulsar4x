using System;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Global processors are run on the entire game.
    /// 
    /// Global processors should
    /// </summary>
    public abstract class GlobalProcessor
    {
        public bool UseMultiThreading { get; set; }

        public int Layer { get; protected set; }

        public abstract void Process(Game game);
    }
}

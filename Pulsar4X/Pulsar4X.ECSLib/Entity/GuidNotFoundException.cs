using System;

namespace Pulsar4X.ECSLib
{
    
    public class GuidNotFoundException : Exception
    {
        
        public Guid MissingGuid { get; private set; }

        
        public GuidNotFoundException(Guid missingGuid)
        {
            MissingGuid = missingGuid;
        }
    }
}
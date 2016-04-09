using Newtonsoft.Json;
using System;

namespace Pulsar4X.ECSLib
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseDataBlob : ICloneable
    {
        
        public virtual Entity OwningEntity { get; internal set; } = Entity.InvalidEntity;

        public abstract object Clone();
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pulsar4X.ECSLib
{
    [Flags]
    public enum IndustryType
    {
        None                        = 0,
        InstallationConstruction    = 1 << 0,
        ComponentConstruction       = 1 << 1,
        ShipConstruction            = 1 << 2,
        FighterConstruction         = 1 << 3,
        OrdnanceConstruction        = 1 << 4,
        Terraforming                = 1 << 5,
        Salvage                     = 1 << 6,
        JPStabilization             = 1 << 7,
        Research                    = 1 << 8,
        Mining                      = 1 << 9,
        Refining                    = 1 << 10,
    }

    public class IndustryJob
    {
        public IndustryType IndustryType { get; internal set; }

        public Guid ItemGuid { get; internal set; }
        public int NumberOrdered { get; set; }
        public int NumberCompleted { get; internal set; }

        public float BPToNextCompletion { get; internal set; }
        public float BPPerItem { get; internal set; }

        public float PercentToUtilize { get; set; }

        [JsonProperty]
        internal Dictionary<Guid, float> materialsRequiredPerItem { get; set; } = new Dictionary<Guid, float>();

        [JsonIgnore]
        public IReadOnlyDictionary<Guid, float> MaterialsRequiredPerItem => materialsRequiredPerItem;
    }
}

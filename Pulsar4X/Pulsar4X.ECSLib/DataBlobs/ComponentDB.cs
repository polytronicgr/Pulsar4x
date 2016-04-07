using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Pulsar4X.ECSLib
{
    [Flags]
    public enum ComponentMountType
    {
        None                = 0,
        ShipComponent       = 1 << 0,
        ShipCargo           = 1 << 1,
        PlanetInstallation  = 1 << 2,
        PDC                 = 1 << 3,
        Fighter             = 1 << 4,
    }

    public class ComponentDB : BaseDataBlob
    {
        [JsonProperty]
        public Guid DesignGuid { get; internal set; }

        [JsonProperty]
        public int SizeInTons { get; internal set; }

        [JsonProperty]
        public int HTK { get; internal set; }

        [JsonProperty]
        public Dictionary<Guid, int> MineralCosts { get; internal set; }

        [JsonProperty]
        public Dictionary<Guid, int> MaterialCosts { get; internal set; }

        [JsonProperty]
        public Dictionary<Guid, int> ComponentCosts { get; internal set; }

        [JsonProperty]
        public int BuildPointCost { get; internal set; }

        [JsonProperty]
        public Guid TechRequirementToBuild { get; internal set; }

        [JsonProperty]
        public int CrewRequrements { get; internal set; }
        
        [JsonProperty]
        public ComponentMountType ComponentMountType { get; internal set; }
        
        public ComponentDB()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="designGuid">this is the design GUID, NOT the SD GUID</param>
        /// <param name="size"></param>
        /// <param name="htk"></param>
        /// <param name="materialCosts"></param>
        /// <param name="techRequrement"></param>
        /// <param name="crewReqirement"></param>
        public ComponentDB(Guid designGuid, int size, int htk, int buildPointCost,Dictionary<Guid, int> mineralCosts, Dictionary<Guid, int> materialCosts, Dictionary<Guid, int> componentCosts, Guid techRequrement, int crewReqirement)
        {
            DesignGuid = designGuid;
            SizeInTons = size;
            HTK = htk;
            BuildPointCost = buildPointCost;
            MineralCosts = mineralCosts;
            MaterialCosts = materialCosts;
            ComponentCosts = componentCosts;
            TechRequirementToBuild = techRequrement;
            CrewRequrements = crewReqirement;
        }

        public ComponentDB(ComponentDB db)
        {
            SizeInTons = db.SizeInTons;
            HTK = db.HTK;
            MaterialCosts = db.MaterialCosts;
            TechRequirementToBuild = db.TechRequirementToBuild;
        }

        public override object Clone()
        {
            return new ComponentDB(this);
        }
    }
}
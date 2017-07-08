#region Copyright/License
/* 
 *Copyright© 2017 Daniel Phelps
    This file is part of Pulsar4x.

    Pulsar4x is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pulsar4x is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
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

    public class ComponentInfoDB : BaseDataBlob
    {
        private Guid _designGuid;
        private float _sizeInTons;
        private int _htk;
        private Dictionary<Guid, int> _minerialCosts;
        private Dictionary<Guid, int> _materialCosts;
        private Dictionary<Guid, int> _componentCosts;
        private int _buildPointCost;
        private Guid _techRequirementToBuild;
        private int _crewRequrements;
        private ComponentMountType _componentMountType;
        private ConstructionType _constructionType;

        [JsonProperty]
        public Guid DesignGuid { get { return _designGuid; } internal set { SetField(ref _designGuid, value); } }

        [JsonProperty]
        public float SizeInTons { get { return _sizeInTons; } internal set { SetField(ref _sizeInTons, value); } }

        [JsonProperty]
        public int HTK { get { return _htk; } internal set { SetField(ref _htk, value); } }

        [JsonProperty]
        public Dictionary<Guid, int> MinerialCosts { get { return _minerialCosts; } internal set { SetField(ref _minerialCosts, value); } }

        [JsonProperty]
        public Dictionary<Guid, int> MaterialCosts { get { return _materialCosts; } internal set { SetField(ref _materialCosts, value); } }

        [JsonProperty]
        public Dictionary<Guid, int> ComponentCosts { get { return _componentCosts; } internal set { SetField(ref _componentCosts, value); } }

        [JsonProperty]
        public int BuildPointCost { get { return _buildPointCost; } internal set { SetField(ref _buildPointCost, value); } }

        [JsonProperty]
        public Guid TechRequirementToBuild { get { return _techRequirementToBuild; } internal set { SetField(ref _techRequirementToBuild, value); } }

        [JsonProperty]
        public int CrewRequrements { get { return _crewRequrements; } internal set { SetField(ref _crewRequrements, value); } }

        [JsonProperty]
        public ComponentMountType ComponentMountType { get { return _componentMountType; } internal set { SetField(ref _componentMountType, value); } }

        public ConstructionType ConstructionType { get { return _constructionType; } internal set { SetField(ref _constructionType, value); } }

        public ComponentInfoDB()
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
        public ComponentInfoDB(Guid designGuid, int size, int htk, int buildPointCost, Dictionary<Guid, int> minerialCosts, Dictionary<Guid, int> materialCosts, Dictionary<Guid, int> componentCosts, Guid techRequrement, int crewReqirement)
        {
            DesignGuid = designGuid;
            SizeInTons = size;
            HTK = htk;
            BuildPointCost = buildPointCost;
            MinerialCosts = minerialCosts;
            MaterialCosts = materialCosts;
            ComponentCosts = componentCosts;
            TechRequirementToBuild = techRequrement;
            CrewRequrements = crewReqirement;
        }

        public ComponentInfoDB(ComponentInfoDB db)
        {
            SizeInTons = db.SizeInTons;
            HTK = db.HTK;
            MaterialCosts = db.MaterialCosts;
            TechRequirementToBuild = db.TechRequirementToBuild;
        }

        public override object Clone()
        {
            return new ComponentInfoDB(this);
        }
    }
}
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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    [Flags]
    public enum ComponentMountType
    {
        None = 0,
        ShipComponent = 1 << 0,
        ShipCargo = 1 << 1,
        PlanetInstallation = 1 << 2,
        PDC = 1 << 3,
        Fighter = 1 << 4
    }

    public class ComponentInfoDB : BaseDataBlob
    {
        #region Fields
        private int _buildPointCost;
        private ObservableDictionary<Guid, int> _componentCosts;
        private ComponentMountType _componentMountType;
        private ConstructionType _constructionType;
        private int _crewRequrements;
        private Guid _designGuid;
        private int _htk;
        private ObservableDictionary<Guid, int> _materialCosts;
        private ObservableDictionary<Guid, int> _minerialCosts;
        private float _sizeInTons;
        private Guid _techRequirementToBuild;
        #endregion

        #region Properties
        [JsonProperty]
        public Guid DesignGuid { get { return _designGuid; } set { SetField(ref _designGuid, value); } }

        [JsonProperty]
        public float SizeInTons { get { return _sizeInTons; } set { SetField(ref _sizeInTons, value); } }

        [JsonProperty]
        public int HTK { get { return _htk; } set { SetField(ref _htk, value); } }

        [JsonProperty]
        public ObservableDictionary<Guid, int> MinerialCosts
        {
            get { return _minerialCosts; }
            set
            {
                SetField(ref _minerialCosts, value);
                MinerialCosts.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(MinerialCosts), args);
            }
        }

        [JsonProperty]
        public ObservableDictionary<Guid, int> MaterialCosts
        {
            get { return _materialCosts; }
            set
            {
                SetField(ref _materialCosts, value);
                MaterialCosts.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(MaterialCosts), args);
            }
        }

        [JsonProperty]
        public ObservableDictionary<Guid, int> ComponentCosts
        {
            get { return _componentCosts; }
            set
            {
                SetField(ref _componentCosts, value);
                ComponentCosts.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ComponentCosts), args);
            }
        }

        [JsonProperty]
        public int BuildPointCost { get { return _buildPointCost; } set { SetField(ref _buildPointCost, value); } }

        [JsonProperty]
        public Guid TechRequirementToBuild { get { return _techRequirementToBuild; } set { SetField(ref _techRequirementToBuild, value); } }

        [JsonProperty]
        public int CrewRequrements { get { return _crewRequrements; } set { SetField(ref _crewRequrements, value); } }

        [JsonProperty]
        public ComponentMountType ComponentMountType { get { return _componentMountType; } set { SetField(ref _componentMountType, value); } }

        public ConstructionType ConstructionType { get { return _constructionType; } set { SetField(ref _constructionType, value); } }
        #endregion

        #region Constructors
        public ComponentInfoDB()
        {
            MinerialCosts = new ObservableDictionary<Guid, int>();
            MaterialCosts = new ObservableDictionary<Guid, int>();
            ComponentCosts = new ObservableDictionary<Guid, int>();
        }

        /// <summary>
        /// </summary>
        /// <param name="designGuid">this is the design GUID, NOT the SD GUID</param>
        public ComponentInfoDB(Guid designGuid, int size, int htk, int buildPointCost, IDictionary<Guid, int> minerialCosts, IDictionary<Guid, int> materialCosts, IDictionary<Guid, int> componentCosts, Guid techRequrement, int crewReqirement)
        {
            DesignGuid = designGuid;
            SizeInTons = size;
            HTK = htk;
            BuildPointCost = buildPointCost;
            MinerialCosts = new ObservableDictionary<Guid, int>(minerialCosts);
            MaterialCosts = new ObservableDictionary<Guid, int>(materialCosts);
            ComponentCosts = new ObservableDictionary<Guid, int>(componentCosts);
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
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ComponentInfoDB(this);
        #endregion
    }
}
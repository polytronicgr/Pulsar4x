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
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    [Flags]
    public enum ConstructionType
    {
        None = 0,
        Installations = 1 << 0,
        ShipComponents = 1 << 1,
        Ships = 1 << 2,
        Fighters = 1 << 3,
        Ordnance = 1 << 4
    }

    public class JobBase
    {
        #region Properties
        public Guid ItemGuid { get; }
        public ushort NumberOrdered { get; set; }
        public ushort NumberCompleted { get; set; }
        public int PointsLeft { get; set; }
        public bool Auto { get; set; }
        #endregion

        #region Constructors
        public JobBase(Guid guid, ushort numberOrderd, int jobPoints, bool auto)
        {
            ItemGuid = guid;
            NumberOrdered = numberOrderd;
            NumberCompleted = 0;
            PointsLeft = jobPoints;
            Auto = auto;
        }
        #endregion
    }


    public class ConstructionJob : JobBase
    {
        #region Properties
        public ConstructionType ConstructionType { get; set; }
        public Entity InstallOn { get; internal set; }
        public Dictionary<Guid, int> MineralsRequired { get; set; }
        public Dictionary<Guid, int> MaterialsRequired { get; set; }
        public Dictionary<Guid, int> ComponentsRequired { get; set; }
        #endregion

        #region Constructors
        public ConstructionJob(Guid designGuid, ConstructionType constructionType, ushort numberOrderd, int jobPoints, bool auto, IDictionary<Guid, int> mineralCost, IDictionary<Guid, int> matCost, IDictionary<Guid, int> componentCost) : base(designGuid, numberOrderd, jobPoints, auto)
        {
            ConstructionType = constructionType;
            MineralsRequired = new Dictionary<Guid, int>(mineralCost);
            MaterialsRequired = new Dictionary<Guid, int>(matCost);
            ComponentsRequired = new Dictionary<Guid, int>(componentCost);
        }
        #endregion
    }

    public class ColonyConstructionDB : BaseDataBlob
    {
        #region Fields
        private ObservableDictionary<ConstructionType, int> _constructionRates;
        private ObservableCollection<ConstructionJob> _jobBatchList;
        private int _pointsPerTick;
        #endregion

        #region Properties
        public int PointsPerTick { get { return _pointsPerTick; } set { SetField(ref _pointsPerTick, value); } }

        [JsonProperty]
        public ObservableDictionary<ConstructionType, int> ConstructionRates
        {
            get { return _constructionRates; }
            set
            {
                SetField(ref _constructionRates, value);
                ConstructionRates.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ConstructionRates), args);
            }
        }

        [JsonProperty]
        public ObservableCollection<ConstructionJob> JobBatchList
        {
            get { return _jobBatchList; }
            set
            {
                SetField(ref _jobBatchList, value);
                JobBatchList.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(JobBatchList), args);
            }
        }
        #endregion

        #region Constructors
        public ColonyConstructionDB()
        {
            ConstructionRates = new ObservableDictionary<ConstructionType, int>
                                {
                                    {ConstructionType.Ordnance, 0},
                                    {ConstructionType.Installations, 0},
                                    {ConstructionType.Fighters, 0},
                                    {ConstructionType.ShipComponents, 0},
                                    {ConstructionType.Ships, 0}
                                };
            JobBatchList = new ObservableCollection<ConstructionJob>();
        }

        public ColonyConstructionDB(IDictionary<ConstructionType, int> rates, IEnumerable<ConstructionJob> jobBatchList) : this()
        {
            if (rates != null)
            {
                ConstructionRates.Merge(rates);
            }

            if (jobBatchList == null)
            {
                jobBatchList = new ConstructionJob[0];
            }
            foreach (ConstructionJob job in jobBatchList)
            {
                JobBatchList.Add(job);
            }
        }

        public ColonyConstructionDB(ColonyConstructionDB db) : this(db.ConstructionRates, db.JobBatchList) { }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ColonyConstructionDB(this);
        #endregion
    }
}
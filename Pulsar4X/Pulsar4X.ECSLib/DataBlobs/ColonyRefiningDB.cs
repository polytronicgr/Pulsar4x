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
    public class RefineingJob : JobBase
    {
        #region Constructors
        public RefineingJob(Guid matGuid, ushort numberOrderd, int jobPoints, bool auto) : base(matGuid, numberOrderd, jobPoints, auto) { }
        #endregion
    }

    public class ColonyRefiningDB : BaseDataBlob
    {
        #region Fields
        private int _pointsPerTick;
        #endregion

        #region Properties
        public int PointsPerTick { get { return _pointsPerTick; } set { SetField(ref _pointsPerTick, value); } }

        //recalc this on game load todo implement this in the processor. 
        public ObservableDictionary<Guid, int> RefiningRates { get; set; } = new ObservableDictionary<Guid, int>();

        [JsonProperty]
        public ObservableCollection<RefineingJob> JobBatchList { get; set; } = new ObservableCollection<RefineingJob>();
        #endregion

        #region Constructors
        public ColonyRefiningDB()
        {
            RefiningRates.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(RefiningRates), args);
            JobBatchList.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(JobBatchList), args);
        }

        public ColonyRefiningDB(IDictionary<Guid, int> refiningRates, IEnumerable<RefineingJob> jobsList) : this()
        {
            RefiningRates.Merge(refiningRates);

            if (jobsList != null)
            {
                foreach (RefineingJob job in jobsList)
                {
                    JobBatchList.Add(job);
                }
            }
        }

        public ColonyRefiningDB(ColonyRefiningDB db) : this(db.RefiningRates, db.JobBatchList) { }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ColonyRefiningDB(this);
        #endregion
    }
}
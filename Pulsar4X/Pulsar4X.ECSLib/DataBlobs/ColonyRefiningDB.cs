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
    public class RefineingJob : JobBase
    {
        public RefineingJob(Guid matGuid, ushort numberOrderd, int jobPoints, bool auto): base(matGuid, numberOrderd, jobPoints, auto)
        {
        }
    }

    public class ColonyRefiningDB : BaseDataBlob
    {
        public int PointsPerTick { get { return _pointsPerTick; } internal set { SetField(ref _pointsPerTick, value); } }

        //recalc this on game load todo implement this in the processor. 
        public Dictionary<Guid, int> RefiningRates{ get; internal set; }

        [JsonProperty] 
        private List<RefineingJob> _jobBatchList;

        private int _pointsPerTick;
        public List<RefineingJob> JobBatchList { get{return _jobBatchList;} internal set { _jobBatchList = value; } }

        
        public ColonyRefiningDB()
        {
            RefiningRates = new Dictionary<Guid, int>();
            JobBatchList = new List<RefineingJob>();
        }

        public ColonyRefiningDB(ColonyRefiningDB db)
        {
            RefiningRates = new Dictionary<Guid, int>(db.RefiningRates);
            JobBatchList = new List<RefineingJob>(db.JobBatchList);
        }

        public override object Clone()
        {
            return new ColonyRefiningDB(this);
        }
    }
}
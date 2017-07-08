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
using System.Linq;

namespace Pulsar4X.ECSLib
{
    public class RefineResourcesAtbDB : BaseDataBlob
    {
        private List<Guid> _refinableMatsList;
        private int _refineryPoints;

        [JsonProperty]
        public List<Guid> RefinableMatsList { get { return _refinableMatsList; } internal set { SetField(ref _refinableMatsList, value); } }

        [JsonProperty]
        public int RefineryPoints { get { return _refineryPoints; } internal set { SetField(ref _refineryPoints, value); } }

        public RefineResourcesAtbDB()
        {
        }

        /// <summary>
        /// this is for the parser, it takes a dictionary but turns it into a list of keys, ignoring the values.
        /// </summary>
        /// <param name="refinableMatsList">a list of guid that this is capable of refining</param>
        /// <param name="RefineryPoints"></param>
        public RefineResourcesAtbDB(Dictionary<Guid, double> refinableMatsList, double refineryPoints)
        {
            RefinableMatsList = refinableMatsList.Keys.ToList();
            RefineryPoints = (int)refineryPoints;
        }

        public RefineResourcesAtbDB(List<Guid> refinableMatsList, int refineryPoints)
        {
            RefinableMatsList = refinableMatsList;
            RefineryPoints = refineryPoints;
        }

        public RefineResourcesAtbDB(RefineResourcesAtbDB db)
        {
            RefinableMatsList = new List<Guid>(db.RefinableMatsList);
            RefineryPoints = db.RefineryPoints;
        }

        public override object Clone()
        {
            return new RefineResourcesAtbDB(this);
        }
    }
}
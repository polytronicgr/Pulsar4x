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

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// TeamsDB defines this entity has being a team of scientists/spies/etc, which can be given orders (ex: Survey Mars)
    /// </summary>
    public class TeamsDB : BaseDataBlob
    {
        #region Fields
        [JsonProperty]
        private int _teamSize;

        [JsonProperty]
        private object _teamTask;
        #endregion

        #region Properties
        /// <summary>
        /// Determines how many Labs this team can manage
        /// </summary>
        /// TODO: Pre-release
        /// Ensure Property/Fields are consistant throughout all DB usage.
        /// Example: TransitableDB uses TeamSize { get; set;}
        [PublicAPI]
        public int TeamSize
        {
            get { return _teamSize; }
            set
            {
                SetField(ref _teamSize, value);
                ;
            }
        }

        /// <summary>
        /// not sure if this should be a blob, entity or guid. and maybe a queue as well.
        /// </summary>
        /// TODO: Communications Review
        /// Detemine team orders system
        [PublicAPI]
        public object TeamTask
        {
            get { return _teamTask; }
            set
            {
                SetField(ref _teamTask, value);
                ;
            }
        }
        #endregion

        #region Constructors
        public TeamsDB() { }

        public TeamsDB(int teamSize = 0, object initialTask = null)
        {
            TeamSize = teamSize;
            TeamTask = initialTask;
        }

        public TeamsDB(TeamsDB teamsdb)
        {
            TeamSize = teamsdb.TeamSize;
            TeamTask = teamsdb.TeamTask;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new TeamsDB(this);
        #endregion
    }
}
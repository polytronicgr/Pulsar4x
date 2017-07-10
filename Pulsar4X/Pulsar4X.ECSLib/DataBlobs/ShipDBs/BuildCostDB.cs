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

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Info on the build cost of the Ship.
    /// </summary>
    public class BuildCostDB : BaseDataBlob
    {
        #region Fields
        private double _buildPointCost;
        private DateTime _buildTime;
        #endregion

        #region Properties
        public DateTime BuildTime { get { return _buildTime; } set { SetField(ref _buildTime, value); } }
        public double BuildPointCost { get { return _buildPointCost; } set { SetField(ref _buildPointCost, value); } }
        #endregion

        #region Constructors
        // add minerials

        public BuildCostDB() { }

        public BuildCostDB(BuildCostDB buildCostDB)
        {
            BuildTime = buildCostDB.BuildTime; //Struct
            BuildPointCost = buildCostDB.BuildPointCost;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new BuildCostDB(this);
        #endregion
    }
}
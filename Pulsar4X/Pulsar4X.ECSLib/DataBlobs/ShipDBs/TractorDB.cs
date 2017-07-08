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

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// This datablob contains information on a ships ability to tractor other ships 
    /// and references to any ships it has tractored.
    /// </summary>
    public class TractorDB : BaseDataBlob
    {
        private int _noOfTractors;
        private List<Guid> _tractoredShips;

        /// <summary>
        /// The number of tractors this ship has, which will determine how many ships it can tractor at once (1 ship per tractor)
        /// Will be 0 by default.
        /// </summary>
        public int NoOfTractors { get { return _noOfTractors; } set { SetField(ref _noOfTractors, value); } }

        public List<Guid> TractoredShips { get { return _tractoredShips; } set { SetField(ref _tractoredShips, value); } }

        public TractorDB() : this(0)
        {

        }

        public TractorDB(int noOfTractors)
        {
            NoOfTractors = noOfTractors;
            TractoredShips = new List<Guid>(NoOfTractors);
        }

        public TractorDB(TractorDB tractorDB)
        {
            NoOfTractors = tractorDB.NoOfTractors;
            TractoredShips = new List<Guid>(NoOfTractors); //Even if we copy datablob we shouldn't copy list of tractored ship
        }

        public override object Clone()
        {
            return new TractorDB(this);
        }
    }
}
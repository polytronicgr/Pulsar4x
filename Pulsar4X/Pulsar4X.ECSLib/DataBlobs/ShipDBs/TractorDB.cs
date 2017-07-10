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

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// This datablob contains information on a ships ability to tractor other ships 
    /// and references to any ships it has tractored.
    /// </summary>
    public class TractorDB : BaseDataBlob
    {
        private int _noOfTractors;

        /// <summary>
        /// The number of tractors this ship has, which will determine how many ships it can tractor at once (1 ship per tractor)
        /// Will be 0 by default.
        /// </summary>
        public int NoOfTractors { get { return _noOfTractors; } set { SetField(ref _noOfTractors, value); } }

        public ObservableCollection<Guid> TractoredShips { get; set; } = new ObservableCollection<Guid>();

        public TractorDB()
        {
            TractoredShips.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(TractoredShips), args);
        }

        public TractorDB(int noOfTractors, IEnumerable<Guid> tractoredShips) : this()
        {
            NoOfTractors = noOfTractors;
            if (tractoredShips != null)
            {
                foreach (Guid tractoredShip in tractoredShips)
                {
                    TractoredShips.Add(tractoredShip);
                }
            }
        }

        public TractorDB(TractorDB tractorDB) : this(tractorDB.NoOfTractors, tractorDB.TractoredShips) { }

        public override object Clone()
        {
            return new TractorDB(this);
        }
    }
}
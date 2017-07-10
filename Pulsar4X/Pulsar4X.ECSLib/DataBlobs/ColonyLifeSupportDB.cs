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

namespace Pulsar4X.ECSLib
{
    public class ColonyLifeSupportDB : BaseDataBlob
    {
        #region Fields
        private long _maxPopulation;
        #endregion

        #region Properties
        public long MaxPopulation { get { return _maxPopulation; } set { SetField(ref _maxPopulation, value); } }
        #endregion

        #region Constructors
        public ColonyLifeSupportDB() { MaxPopulation = new long(); }

        public ColonyLifeSupportDB(ColonyLifeSupportDB db) { MaxPopulation = db.MaxPopulation; }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ColonyLifeSupportDB(this);
        #endregion
    }
}
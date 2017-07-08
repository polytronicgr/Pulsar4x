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
    public class JumpDriveAtbDB : BaseDataBlob
    {
        private int _maxShipSize;
        private int _maxSquadronSize;

        public int MaxShipSize { get { return _maxShipSize; } internal set { SetField(ref _maxShipSize, value); } }
        public int MaxSquadronSize { get { return _maxSquadronSize; } internal set { SetField(ref _maxSquadronSize, value); } }
        /// <summary>
        /// Max distance from JP when arriving. Measured in KM
        /// </summary>
        public int MaxDisplacement { get; internal set; }

        public override object Clone()
        {
            return new JumpDriveAtbDB
            {
                MaxShipSize = MaxShipSize,
                MaxSquadronSize = MaxSquadronSize,
                MaxDisplacement = MaxDisplacement,
                OwningEntity = OwningEntity
            };
        }
    }
}

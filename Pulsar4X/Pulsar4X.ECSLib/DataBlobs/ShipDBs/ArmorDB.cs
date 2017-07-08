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
using System.Collections;

namespace Pulsar4X.ECSLib
{
    public class ArmorDefDB 
    {
        public string Name;

        public double Strength;

        public ArmorDefDB()
        {
        }
    }

    public class ArmorDB : BaseDataBlob
    {
        public ArmorDefDB _armorDef;
        public BitArray[] _armorStatus;

        public ArmorDefDB ArmorDef { get { return _armorDef; } set { SetField(ref _armorDef, value); } }
        public BitArray[] ArmorStatus { get { return _armorStatus; } set { SetField(ref _armorStatus, value); } }

        public ArmorDB(ArmorDefDB armorDef, BitArray[] armorStatus)
        {
        }

        public ArmorDB()
        {
            
        }

        public ArmorDB(ArmorDB armorDB)
        {
            if(armorDB.ArmorDef != null)
                ArmorDef = new ArmorDefDB() {Name = armorDB.ArmorDef.Name, Strength = armorDB.ArmorDef.Strength};
            if (armorDB.ArmorStatus != null)
            {
                ArmorStatus = new BitArray[armorDB.ArmorStatus.Length];
                armorDB.ArmorStatus.CopyTo(ArmorStatus, 0);
            }
        }


        public override object Clone()
        {
            return new ArmorDB(this);
        }
    }
}

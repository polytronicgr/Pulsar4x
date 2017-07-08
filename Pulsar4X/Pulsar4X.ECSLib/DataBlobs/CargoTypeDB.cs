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
using System.Runtime.Serialization;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Contains info on how an entitiy can be stored.
    /// NOTE an entity with this datablob must also have a MassVolumeDB
    /// </summary>
    public class CargoAbleTypeDB : BaseDataBlob , ICargoable
    {
        private string _itemName;
        private Guid _cargoTypeID;

        [JsonIgnore]
        public string ItemName { get { return _itemName; } private set { SetField(ref _itemName, value); } }

        [JsonProperty]
        public Guid CargoTypeID { get { return _cargoTypeID; } internal set { SetField(ref _cargoTypeID, value); } }

        [JsonIgnore]
        public Guid ID => OwningEntity.Guid;

        [JsonIgnore]
        public float Mass => (float)OwningEntity.GetDataBlob<MassVolumeDB>().Mass;

        [JsonIgnore]
        public string Name => OwningEntity.GetDataBlob<NameDB>()?.GetName(OwningEntity.GetDataBlob<OwnedDB>()?.ObjectOwner) ?? "Unknown Object";

        public CargoAbleTypeDB()
        {
        }

        public CargoAbleTypeDB(Guid cargoTypeID)
        {
            CargoTypeID = cargoTypeID;
        }

        public CargoAbleTypeDB(CargoAbleTypeDB cargoTypeDB)
        {
            CargoTypeID = cargoTypeDB.CargoTypeID;
        }

        public override object Clone()
        {
            return new CargoAbleTypeDB(this);
        }
        
        // JSON deserialization callback.
        [OnDeserialized]
        private void Deserialized(StreamingContext context)
        {
            //set item name to the design name if it exsists. 
            ItemName = OwningEntity.GetDataBlob<DesignInfoDB>()?.DesignEntity.GetDataBlob<NameDB>()?.DefaultName ?? Name;
        }
    }
}
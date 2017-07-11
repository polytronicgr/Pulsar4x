using System;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// A concreation of ICargoable which can more easly be seralised. 
    /// </summary>
    public struct CargoableUIData : ICargoable
    {
        public Guid ID { get; }
        public string Name { get; }
        public string ItemTypeName { get; }
        public Guid CargoTypeID { get; }
        public float Mass { get; }

        public CargoableUIData(ICargoable cargoableObject)
        {
            ID = cargoableObject.ID;
            Name = cargoableObject.Name;
            ItemTypeName = cargoableObject.ItemTypeName;
            CargoTypeID = cargoableObject.CargoTypeID;
            Mass = cargoableObject.Mass;
        }
    }
}
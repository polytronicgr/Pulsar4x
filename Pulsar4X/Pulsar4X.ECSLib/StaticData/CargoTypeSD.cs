using System;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// This defines a *type* of storage 'contaner'
    /// The ID is what the game uses, the other two entries are for display purposes.  
    /// </summary>
    [StaticData(true, IDPropertyName = "ID")]
    public struct CargoTypeSD
    {
        /// <summary>
        ///IE: General, Fuel, Ammo, etc. 
        /// </summary>
        public string Name; 
        public string Description;
        public Guid ID;
    }

    
    /// <summary>
    /// This interface is inherited by any item that is cargoable. 
    /// IE static data such as Minerals and Refined materals.
    /// Cargoable Entities have a CargoableDB which inherits this interface.   
    /// </summary>
    public interface ICargoable
    {
        Guid ID { get; }
        string Name { get; }
        /// <summary>
        /// IE "Minerals", "Refined Materials"
        /// </summary>
        string ItemTypeName { get; } 
        /// <summary>
        /// This is the CargoTypeSD.ID this defines what type of cargo container is required to store it. 
        /// </summary>
        Guid CargoTypeID { get;  }
        float Mass { get;  }
    }
}



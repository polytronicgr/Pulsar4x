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
    /// Holds all the generic information about a ship
    /// </summary>
    public class ShipInfoDB : BaseDataBlob
    {
        #region Fields
        private bool _collier;
        private bool _conscript;
        private int _internalHTK;
        private bool _isMilitary;
        private bool _obsolete;

        private Guid _shipClassDefinition;
        private bool _supplyShip;
        private bool _tanker;
        private float _tonnage;

        /// Ship orders.
        public Queue<BaseOrder> Orders;
        #endregion

        #region Properties
        /// <summary>
        /// The guid of the ship class, if this is a ship class then the Guid will be empty.
        /// use IsClassDefinition() to determin if this is a ship class definmition
        /// </summary>
        public Guid ShipClassDefinition { get { return _shipClassDefinition; } set { SetField(ref _shipClassDefinition, value); } }

        public bool Obsolete { get { return _obsolete; } set { SetField(ref _obsolete, value); } }
        public bool Conscript { get { return _conscript; } set { SetField(ref _conscript, value); } }

        // Should we have these: ??
        public bool Tanker { get { return _tanker; } set { SetField(ref _tanker, value); } }

        public bool Collier { get { return _collier; } set { SetField(ref _collier, value); } }
        public bool SupplyShip { get { return _supplyShip; } set { SetField(ref _supplyShip, value); } }

        /// <summary>
        /// The Ships health minus its armour and sheilds, i.e. the total HTK of all its internal Components.
        /// </summary>
        public int InternalHTK { get { return _internalHTK; } set { SetField(ref _internalHTK, value); } }

        public bool IsMilitary { get { return _isMilitary; } set { SetField(ref _isMilitary, value); } }

        public float Tonnage { get { return _tonnage; } set { SetField(ref _tonnage, value); } }

        public double TCS => Tonnage * 0.02;
        #endregion

        #region Constructors
        public ShipInfoDB() { Orders = new Queue<BaseOrder>(); }

        public ShipInfoDB(ShipInfoDB shipInfoDB)
        {
            if (shipInfoDB.ShipClassDefinition == Guid.Empty) //Class
            {
                ShipClassDefinition = shipInfoDB.OwningEntity.Guid;
            }
            else //Ship
            {
                ShipClassDefinition = shipInfoDB.ShipClassDefinition;
            }
            Obsolete = shipInfoDB.Obsolete;
            Conscript = shipInfoDB.Conscript;
            Tanker = shipInfoDB.Tanker;
            Collier = shipInfoDB.Collier;
            SupplyShip = shipInfoDB.SupplyShip;
            InternalHTK = shipInfoDB.InternalHTK;
            Tonnage = shipInfoDB.Tonnage;
            IsMilitary = shipInfoDB.IsMilitary;

            if (shipInfoDB.Orders == null)
            {
                Orders = null;
            }
            else
            {
                Orders = new Queue<BaseOrder>(shipInfoDB.Orders);
            }
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ShipInfoDB(this);
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns true if this is a definition of a class.
        /// </summary>
        public bool IsClassDefinition()
        {
            if (ShipClassDefinition != Guid.Empty)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
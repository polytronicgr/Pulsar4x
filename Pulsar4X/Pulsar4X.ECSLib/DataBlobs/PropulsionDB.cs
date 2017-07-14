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
    /// Contains info on the ships engines.
    /// </summary>
    public class PropulsionDB : BaseDataBlob
    {
        #region Properties
        public int MaximumSpeed { get; set; }
        public Vector4 CurrentSpeed { get; set; }
        public int TotalEnginePower { get; set; }
        public Dictionary<Guid, double> FuelUsePerKM { get; set; } = new Dictionary<Guid, double>();
        #endregion

        #region Constructors
        public PropulsionDB() { }

        public PropulsionDB(PropulsionDB propulsionDB)
        {
            MaximumSpeed = propulsionDB.MaximumSpeed;
            CurrentSpeed = propulsionDB.CurrentSpeed;
            TotalEnginePower = propulsionDB.TotalEnginePower;
            FuelUsePerKM = new Dictionary<Guid, double>(propulsionDB.FuelUsePerKM);
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new PropulsionDB(this);
        #endregion
    }

    public class TranslateOrderableDB : BaseDataBlob
    {
        #region Types
        public enum HelmStatus
        {
            Orbiting, // anchored, no move orders or waiting for non move orders to complete while not under power.
            Makingway, //moving to next waypoint under power
            Underway, //used to indicate helm needs to start towards next waypoint.
            HoldingUnderPower //keeping at an absolute position waiting for non move orders to complete.
        }
        #endregion

        #region Fields
        public HelmStatus HelmState = HelmStatus.Orbiting;
        #endregion

        #region Properties
        //public Queue<TranslationOrder> waypointQueue;

        //public TranslationOrder CurrentOrder { get; set; }

        public DateTime EstTimeToWaypoint { get; set; }

        public DateTime LastRunDate { get; set; }
        #endregion

        #region Constructors
        public TranslateOrderableDB() { }

        public TranslateOrderableDB(TranslateOrderableDB db)
        {
            HelmState = db.HelmState;
            //waypointQueue = db.waypointQueue;
            EstTimeToWaypoint = db.EstTimeToWaypoint;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new TranslateOrderableDB(this);
        #endregion
    }
}
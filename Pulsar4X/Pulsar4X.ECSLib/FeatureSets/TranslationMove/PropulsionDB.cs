using System;
using System.Collections.Generic;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Contains info on the ships engines.
    /// </summary>
    public class PropulsionDB : BaseDataBlob
    {
        public int MaximumSpeed { get; set; }
        public Vector4 CurrentSpeed { get; set; }
        public int TotalEnginePower { get; set; }
        public Dictionary<Guid, double> FuelUsePerKM { get; internal set; } = new Dictionary<Guid, double>();

        public PropulsionDB()
        {
        }

        public PropulsionDB(PropulsionDB propulsionDB)
        {
            MaximumSpeed = propulsionDB.MaximumSpeed;
            CurrentSpeed = propulsionDB.CurrentSpeed;
            TotalEnginePower = propulsionDB.TotalEnginePower;
            FuelUsePerKM = new Dictionary<Guid, double>(propulsionDB.FuelUsePerKM);
        }

        public override object Clone()
        {
            return new PropulsionDB(this);
        }
    }

    public class MovementUIData : UIData
    {
        public int MaximumSpeed { get; set; }
        public Vector4 CurrentSpeed { get; set; }
        public int TotalEnginePower { get; set; }
        public List<FuelUse> FuelUsePerKM { get; set; }


        public MovementUIData(StaticDataStore staticData, PropulsionDB db)
        {
            MaximumSpeed = db.MaximumSpeed;
            CurrentSpeed = db.CurrentSpeed;
            TotalEnginePower = db.TotalEnginePower;
            foreach (var kvp in db.FuelUsePerKM)
            {
                string name = staticData.GetICargoable(kvp.Key).Name;
                FuelUse fuelUse = new FuelUse(){FuelName = name, AmountPerKM = kvp.Value};
                FuelUsePerKM.Add(fuelUse);
            }
        }
        
        
        public struct FuelUse
        {
            public string FuelName;
            public double AmountPerKM;
        }

    }

    public class TranslateOrderableDB:BaseDataBlob
    {


        public enum HelmStatus
        {
            Orbiting, // anchored, no move orders or waiting for non move orders to complete while not under power.
            Makingway, //moving to next waypoint under power
            Underway, //used to indicate helm needs to start towards next waypoint.
            HoldingUnderPower //keeping at an absolute position waiting for non move orders to complete.
        }
        public HelmStatus HelmState = HelmStatus.Orbiting;

        //public Queue<TranslationOrder> waypointQueue;

        //public TranslationOrder CurrentOrder { get; internal set; }

        public DateTime EstTimeToWaypoint { get; internal set; }

        public DateTime LastRunDate { get; internal set; }




        public TranslateOrderableDB()
        {
        }

        public TranslateOrderableDB(TranslateOrderableDB db)
        {
            HelmState = db.HelmState;
            //waypointQueue = db.waypointQueue;
            EstTimeToWaypoint = db.EstTimeToWaypoint;
        }


        public override object Clone()
        {
            return new TranslateOrderableDB(this);
        }
    }
}
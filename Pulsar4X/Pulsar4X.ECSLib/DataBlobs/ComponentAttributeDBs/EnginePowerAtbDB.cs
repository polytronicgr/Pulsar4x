using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class EnginePowerAtbDB : BaseDataBlob, IAttributeDescription
    {
        [JsonProperty]
        public int EnginePower { get; internal set; }

        public string Name { get; } = "Engine Power";        

        public string Description { get; } = "Engine Power";

        public double Value { get { return EnginePower; } } 

        public EnginePowerAtbDB()
        {
        }

        public EnginePowerAtbDB(double power)
        {
            EnginePower = (int)power;
        }

        public EnginePowerAtbDB(int enginePower)
        {
            EnginePower = enginePower;
        }

        public EnginePowerAtbDB(EnginePowerAtbDB abilityDB)
        {
            EnginePower = abilityDB.EnginePower;
        }

        public override object Clone()
        {
            return new EnginePowerAtbDB(this);
        }
    }
}
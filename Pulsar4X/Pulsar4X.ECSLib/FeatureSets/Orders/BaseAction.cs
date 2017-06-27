using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public abstract class BaseAction
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Status { get; set; }
        /// <summary>
        /// bitmask
        /// </summary>
         [JsonProperty]
        internal int Lanes { get; set; } 
        [JsonProperty]
        internal bool IsBlocking { get; set; }
        [JsonProperty]
        internal bool IsFinished { get; set; }
        [JsonProperty]
        internal DateTime LastRunTime { get; set; }
        [JsonProperty]
        internal IActionableProcessor OrderableProcessor { get; set; }
        [JsonProperty]
        internal Entity ThisEntity { get; private set; }
        [JsonProperty]
        internal Entity FactionEntity { get; private set; }
        [JsonProperty]
        internal Entity TargetEntity { get; private set; }
        [JsonProperty]
        public DateTime EstTimeComplete { get; internal set; }
        [JsonProperty]
        public BaseOrder Order { get; private set; }
        [JsonProperty]
        public bool HasTargetEntity { get; internal set; }

        /// <summary>
        /// BaseAction constructor
        /// </summary>
        /// <param name="lanes">bitmask</param>
        /// <param name="isBlocking">if true, will block on the lanes it's running on till complete</param>
        /// <param name="order"></param>
        /// <param name="entity"></param>
        /// <param name="faction"></param>
        protected BaseAction(int lanes, bool isBlocking, BaseOrder order, Entity entity, Entity faction)
        {
            Lanes = lanes;
            IsBlocking = isBlocking;
            IsFinished = false;
            Order = order;
            ThisEntity = entity;
            FactionEntity = faction;
            HasTargetEntity = false;
        }

        protected BaseAction(int lanes, bool isBlocking, BaseOrder order, Entity entity, Entity faction, Entity target) : 
            this (lanes, isBlocking, order, entity, faction)
        {
            TargetEntity = target;
            HasTargetEntity = true;
        }        
    }

    public interface IActionableProcessor
    {
        void ProcessAction(DateTime toDate, BaseAction action);
    }

    public class DoNothingAction:IActionableProcessor
    {
        public void ProcessAction(DateTime toDate, BaseAction action) {  }
    }


}


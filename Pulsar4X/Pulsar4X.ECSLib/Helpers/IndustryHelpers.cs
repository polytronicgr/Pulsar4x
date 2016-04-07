using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Pulsar4X.ECSLib
{
    [Flags]
    public enum IndustryType
    {
        None                        = 0,
        InstallationConstruction    = 1 << 0,
        ComponentConstruction       = 1 << 1,
        ShipConstruction            = 1 << 2,
        FighterConstruction         = 1 << 3,
        OrdnanceConstruction        = 1 << 4,
        Terraforming                = 1 << 5,
        Salvage                     = 1 << 6,
        JPStabilization             = 1 << 7,
        Research                    = 1 << 8,
        Mining                      = 1 << 9,
        Refining                    = 1 << 10,
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class IndustryJob
    {
        [JsonProperty]
        public Entity ProjectManager { get; internal set; }

        [JsonProperty]
        public Entity OwningFaction { get; internal set; }

        public IndustryType IndustryType { get; internal set; }

        [JsonProperty]
        public Guid ItemGuid { get; internal set; }
        public string ItemName { get; internal set; }
        [JsonProperty]
        public int NumberOrdered { get; set; }
        [JsonProperty]
        public int NumberCompleted { get; internal set; }

        public float BPPerItem { get; internal set; }
        [JsonProperty]
        public float PartialBPApplied { get; internal set; }
        public float BPToNextItem => BPPerItem - PartialBPApplied;

        [JsonProperty]
        public float PercentToUtilize { get; set; }

        public float TotalBPApplied => NumberCompleted * BPPerItem + PartialBPApplied;
        public float TotalBPRequired => NumberOrdered * BPPerItem;
        public float PercentCompleted => TotalBPApplied / TotalBPRequired;

        public DateTime ProjectedCompletion { get; internal set; }
        
        internal Dictionary<Guid, float> materialsRequiredPerItem { get; set; } = new Dictionary<Guid, float>();
        
        public IReadOnlyDictionary<Guid, float> MaterialsRequiredPerItem => materialsRequiredPerItem;

        [JsonProperty]
        public bool AutoRepeat { get; set; }

        public IndustryJob(Game game, Entity owningFaction, Guid itemGuid, int numberOrdered, float percentToUtilize, bool autoRepeat = false)
        {
            OwningFaction = owningFaction;
            ItemGuid = itemGuid;
            NumberOrdered = numberOrdered;
            PercentToUtilize = percentToUtilize;
            AutoRepeat = autoRepeat;

            SetupJob(game);
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            SetupJob((Game)context.Context);
        }

        private void SetupJob(Game game)
        {
            object itemSD = game.StaticData.FindDataObjectUsingID(ItemGuid);
            if (itemSD != null)
            {
                // Item is a static-data object. Resolve the type.
                // Note to implementor: This will be required for Research at least.
                throw new NotImplementedException();
            }

            var itemEntity = game.GlobalManager.GetLocalEntityByGuid(ItemGuid);

            if (!itemEntity.IsValid)
            {
                throw new ArgumentException("Guid could not be resolved.", nameof(ItemGuid));
            }

            ItemName = itemEntity.GetDataBlob<NameDB>().GetName(OwningFaction);

            var ComponentDB = itemEntity.GetDataBlob<ComponentDB>();
            if (ComponentDB == null)
            {
                throw new ArgumentException("Provided Guid resolved to an entity without a ComponentDB.");
            }

            BPPerItem = ComponentDB.BuildPointCost;
            foreach (KeyValuePair<Guid, int> materialCost in ComponentDB.MaterialCosts)
            {
                materialsRequiredPerItem.Add(materialCost.Key, materialCost.Value);
            }

            foreach (KeyValuePair<Guid, int> componentCost in ComponentDB.ComponentCosts)
            {
                materialsRequiredPerItem.Add(componentCost.Key, componentCost.Value);
            }
        }
    }

    /// <summary>
    /// Helper class that pulls the DB's from the EntityManager ONCE for multiple uses.
    /// </summary>
    public class IndustrialEntity
    {
        public readonly Entity Entity;
        public readonly CargoDB CargoDB;
        public readonly IndustryDB IndustryDB;
        public readonly MatedToDB MatedToDB;
        public readonly OwnedDB OwnedDB;

        public IndustrialEntity(Entity entity)
        {
            Entity = entity;
            CargoDB = entity.GetDataBlob<CargoDB>();
            IndustryDB = entity.GetDataBlob<IndustryDB>();
            MatedToDB = entity.GetDataBlob<MatedToDB>();
            OwnedDB = entity.GetDataBlob<OwnedDB>();
        }
    }
}

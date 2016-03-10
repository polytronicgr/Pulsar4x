using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class EntityBonusesDB : BaseDataBlob
    {
        [JsonProperty]
        internal Dictionary<AbilityType, float> abilityBonuses { get; set; }
        public IReadOnlyDictionary<AbilityType, float> AbilityBonuses => abilityBonuses;

        [JsonProperty]
        internal Dictionary<ResearchCategories, float> researchBonuses { get; set; }
        public IReadOnlyDictionary<ResearchCategories, float> ResearchBonuses => researchBonuses;

        [JsonProperty]
        internal Dictionary<IndustryType, float> industrialBonuses { get; set; }
        public IReadOnlyDictionary<IndustryType, float> IndustrialBonuses => industrialBonuses;

        [JsonConstructor]
        public EntityBonusesDB(
            IDictionary<AbilityType, float> abilityBonuses = null,
            IDictionary<ResearchCategories, float> researchBonuses = null,
            IDictionary<IndustryType, float> industrialBonuses = null)
        {
            this.abilityBonuses = abilityBonuses == null ? new Dictionary<AbilityType, float>() : new Dictionary<AbilityType, float>(abilityBonuses);
            this.researchBonuses = researchBonuses == null ? new Dictionary<ResearchCategories, float>() : new Dictionary<ResearchCategories, float>(researchBonuses);
            this.industrialBonuses = industrialBonuses == null ? new Dictionary<IndustryType, float>() : new Dictionary<IndustryType, float>(industrialBonuses);
        }

        public override object Clone()
        {
            return new EntityBonusesDB(abilityBonuses, researchBonuses);
        }
    }
}
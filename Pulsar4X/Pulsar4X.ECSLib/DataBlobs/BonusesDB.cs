using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class BonusesDB : BaseDataBlob
    {
        [JsonProperty]
        internal Dictionary<AbilityType, float> abilityBonuses { get; set; }
        public IReadOnlyDictionary<AbilityType, float> AbilityBonuses => abilityBonuses;

        [JsonProperty]
        internal Dictionary<ResearchCategory, float> researchBonuses { get; set; }
        public IReadOnlyDictionary<ResearchCategory, float> ResearchBonuses => researchBonuses;

        [JsonProperty]
        internal Dictionary<IndustryType, float> industrialBonuses { get; set; }
        public IReadOnlyDictionary<IndustryType, float> IndustrialBonuses => industrialBonuses;

        [JsonConstructor]
        public BonusesDB(
            IDictionary<AbilityType, float> abilityBonuses = null,
            IDictionary<ResearchCategory, float> researchBonuses = null,
            IDictionary<IndustryType, float> industrialBonuses = null)
        {
            this.abilityBonuses = abilityBonuses == null ? new Dictionary<AbilityType, float>() : new Dictionary<AbilityType, float>(abilityBonuses);
            this.researchBonuses = researchBonuses == null ? new Dictionary<ResearchCategory, float>() : new Dictionary<ResearchCategory, float>(researchBonuses);
            this.industrialBonuses = industrialBonuses == null ? new Dictionary<IndustryType, float>() : new Dictionary<IndustryType, float>(industrialBonuses);
        }

        public override object Clone()
        {
            return new BonusesDB(abilityBonuses, researchBonuses, industrialBonuses);
        }
    }
}
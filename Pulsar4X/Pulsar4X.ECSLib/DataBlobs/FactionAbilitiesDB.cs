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

namespace Pulsar4X.ECSLib
{
    public enum AbilityType
    {
        ShipMaintenance,
        GenericConstruction, //ship parts, installations. 
        OrdnanceConstruction,
        FighterConstruction,
        ShipAssembly,
        Refinery,
        Mine,
        AtmosphericModification,
        Research,
        Commercial, //ie aurora "Finance Center" 
        Industrial, //intend to use this later on for civ economy and creating random tradegoods.
        Agricultural, //as above.
        MassDriver,
        SpacePort, //loading/unloading speed;
        GeneratesNavalOfficers,
        GeneratesGroundOfficers,
        GeneratesShipCrew,
        GeneratesTroops, //not sure how we're going to do this yet.aurora kind of did toops and crew different.
        GeneratesScientists,
        GeneratesCivilianLeaders,
        DetectionThermal, //radar
        DetectionEM, //radar
        Terraforming,
        BasicLiving, //ie Auroras infrastructure will have this ability. 

        //shipcomponent
        ReducedSize,
        LaunchMissileSize,
        ReloadRateFromMag,
        ReloadRateFromHanger,
        ReloadRateMultiplyer,
        MissileMagazine,

        ComponentSize,
        EnginePower,
        EngineEfficency,
        FuelConsumption,
        ThermalSignature,
        EMSignature,

        Nothing
    }

    public class FactionAbilitiesDB : BaseDataBlob
    {
        #region Fields
        private ObservableDictionary<AbilityType, float> _abilityBonuses;
        private float _baseGroundUnitStrengthBonus;
        private int _basePlanetarySensorStrength;
        #endregion

        #region Properties
        public int BasePlanetarySensorStrength { get { return _basePlanetarySensorStrength; } set { SetField(ref _basePlanetarySensorStrength, value); } }

        public float BaseGroundUnitStrengthBonus { get { return _baseGroundUnitStrengthBonus; } set { SetField(ref _baseGroundUnitStrengthBonus, value); } }

        public ObservableDictionary<AbilityType, float> AbilityBonuses
        {
            get { return _abilityBonuses; }
            set
            {
                SetField(ref _abilityBonuses, value);
                AbilityBonuses.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(AbilityBonuses), args);
            }
        }

        /// <summary>
        /// To determine final colony costs, from the Colonization Cost Reduction X% techs.
        /// </summary>
        public float ColonyCostMultiplier { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default faction abilities constructor.
        /// </summary>
        public FactionAbilitiesDB()
        {
            AbilityBonuses = new ObservableDictionary<AbilityType, float>();
        }

        public FactionAbilitiesDB(float constructionBonus, float fighterConstructionBonus = 1.0f, float miningBonus = 1.0f, float refiningBonus = 1.0f, float ordnanceConstructionBonus = 1.0f, float researchBonus = 1.0f, float shipAsseblyBonus = 1.0f, float terraformingBonus = 1.0f, int basePlanetarySensorStrength = 250, float groundUnitStrengthBonus = 1.0f, float colonyCostMultiplier = 1.0f) : this()
        {
            BasePlanetarySensorStrength = basePlanetarySensorStrength;
            BaseGroundUnitStrengthBonus = groundUnitStrengthBonus;
            ColonyCostMultiplier = colonyCostMultiplier;

            AbilityBonuses.Add(AbilityType.GenericConstruction, constructionBonus);
            AbilityBonuses.Add(AbilityType.FighterConstruction, fighterConstructionBonus);
            AbilityBonuses.Add(AbilityType.Mine, miningBonus);
            AbilityBonuses.Add(AbilityType.Refinery, refiningBonus);
            AbilityBonuses.Add(AbilityType.OrdnanceConstruction, ordnanceConstructionBonus);
            AbilityBonuses.Add(AbilityType.Research, researchBonus);
            AbilityBonuses.Add(AbilityType.ShipAssembly, shipAsseblyBonus);
            AbilityBonuses.Add(AbilityType.Terraforming, terraformingBonus);
        }

        public FactionAbilitiesDB(FactionAbilitiesDB db) : this()
        {
            BasePlanetarySensorStrength = db.BasePlanetarySensorStrength;
            BaseGroundUnitStrengthBonus = db.BaseGroundUnitStrengthBonus;
            ColonyCostMultiplier = db.ColonyCostMultiplier;
            AbilityBonuses.Merge(db.AbilityBonuses);
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new FactionAbilitiesDB(this);
        #endregion
    }
}
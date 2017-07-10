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

using System.Collections.Generic;

namespace Pulsar4X.ECSLib
{
    public class ColonyBonusesDB : BaseDataBlob
    {
        #region Fields
        private ObservableDictionary<AbilityType, float> _factionBonus;
        #endregion

        #region Properties
        public ObservableDictionary<AbilityType, float> FactionBonus
        {
            get { return _factionBonus; }
            set
            {
                SetField(ref _factionBonus, value);
                FactionBonus.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(_factionBonus), args);
            }
        }
        #endregion

        #region Constructors
        public ColonyBonusesDB() { FactionBonus = new ObservableDictionary<AbilityType, float>(); }

        public ColonyBonusesDB(IDictionary<AbilityType, float> bonuses) : this() { FactionBonus.Merge(bonuses); }

        public ColonyBonusesDB(ColonyBonusesDB db) : this(db.FactionBonus) { }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ColonyBonusesDB(this);
        #endregion

        #region Public Methods
        public float GetBonus(AbilityType type) => FactionBonus[type];
        #endregion
    }
}
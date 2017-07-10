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
using System.Linq;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// A single ability can provide multiple types of CP's. Some may even overlap.
    /// For example, you can have a component that provides 5 Installations CP's, and provides 2 Installations | Ships CP's.
    /// Final result will be 7 Installation CP's, and 2 Ship CP's.
    /// </summary>
    public class ConstructionAtbDB : BaseDataBlob
    {
        #region Fields
        public ObservableDictionary<ConstructionType, int> ConstructionPoints = new ObservableDictionary<ConstructionType, int>();
        #endregion

        #region Properties
        public int InstallationConstrustionPoints => GetConstructionPoints(ConstructionType.Installations);
        public int ShipConstructionPoints => GetConstructionPoints(ConstructionType.Ships);
        public int FighterConstructionPoints => GetConstructionPoints(ConstructionType.Fighters);
        public int OrdnanceConstructionPoints => GetConstructionPoints(ConstructionType.Ordnance);
        #endregion

        #region Constructors
        public ConstructionAtbDB(IDictionary<ConstructionType, double> constructionPoints) : this(constructionPoints.ToDictionary(constructionPoint => constructionPoint.Key, constructionPoint => (int)constructionPoint.Value)) { }

        [JsonConstructor]
        public ConstructionAtbDB(IDictionary<ConstructionType, int> constructionPoints = null)
        {
            ConstructionPoints.Merge(constructionPoints);
            ConstructionPoints.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ConstructionPoints), args);
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ConstructionAtbDB(ConstructionPoints);
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds up all construstion points this ability provides for a given type.
        /// </summary>
        public int GetConstructionPoints(ConstructionType type)
        {
            int totalConstructionPoints = 0;
            foreach (KeyValuePair<ConstructionType, int> keyValuePair in ConstructionPoints)
            {
                ConstructionType entryType = keyValuePair.Key;
                int constructionPoints = keyValuePair.Value;

                if ((entryType & type) != 0)
                {
                    totalConstructionPoints += constructionPoints;
                }
            }
            return totalConstructionPoints;
        }
        #endregion
    }
}
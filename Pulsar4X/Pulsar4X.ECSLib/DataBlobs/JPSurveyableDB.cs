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
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Attached to entities that are surveyed for the discovery of JumpPoints.
    /// </summary>
    /// <remarks>
    /// This is very inefficient implementation of jump points.
    /// Every system has 30 of these entities.
    /// This clogs EntityManager space too.
    /// Each of these entities individually stores every faction that scans it?
    /// PERFORMANCE OPTIMIZE:
    /// If we ever need to optimize memory usage, this may be something to look at.
    /// </remarks>
    public class JPSurveyableDB : BaseDataBlob
    {
        #region Fields
        private ObservableDictionary<Entity, int> _surveyPointsAccumulated;
        private int surveyPointsRequired;
        #endregion

        #region Properties
        [JsonProperty]
        public ObservableDictionary<Entity, int> SurveyPointsAccumulated
        {
            get { return _surveyPointsAccumulated; }
            set
            {
                SetField(ref _surveyPointsAccumulated, value);
                SurveyPointsAccumulated.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(SurveyPointsAccumulated), args);
            }
        }

        [JsonProperty]
        public int SurveyPointsRequired
        {
            get { return surveyPointsRequired; }
            set
            {
                SetField(ref surveyPointsRequired, value);
                ;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default public constructor for Json
        /// </summary>
        public JPSurveyableDB()
        {
            SurveyPointsAccumulated = new ObservableDictionary<Entity, int>();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public JPSurveyableDB(int pointsRequired, IDictionary<Entity, int> pointsAccumulated)
        {
            SurveyPointsRequired = pointsRequired;
            SurveyPointsAccumulated = new ObservableDictionary<Entity, int>(pointsAccumulated);
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        /// <summary>
        /// ICloneable interface implementation.
        /// </summary>
        /// <returns></returns>
        public override object Clone() => new JPSurveyableDB(SurveyPointsRequired, SurveyPointsAccumulated);
        #endregion
    }
}
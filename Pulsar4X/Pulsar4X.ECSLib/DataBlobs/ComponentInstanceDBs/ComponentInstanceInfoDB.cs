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

using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class ComponentInstanceInfoDB : BaseDataBlob
    {
        #region Fields
        private PercentValue _componentLoadPercent;
        private Entity _designEntity;
        private int _htkRemaining;
        private bool _isEnabled;
        private Entity _parentEntity;
        #endregion

        #region Properties
        [JsonProperty]
        public Entity ParentEntity { get { return _parentEntity; } set { SetField(ref _parentEntity, value); } }

        [JsonProperty]
        public Entity DesignEntity { get { return _designEntity; } set { SetField(ref _designEntity, value); } }

        [JsonProperty]
        public bool IsEnabled { get { return _isEnabled; } set { SetField(ref _isEnabled, value); } }

        [JsonProperty]
        public PercentValue ComponentLoadPercent { get { return _componentLoadPercent; } set { SetField(ref _componentLoadPercent, value); } }

        [JsonProperty]
        public int HTKRemaining { get { return _htkRemaining; } set { SetField(ref _htkRemaining, value); } }

        [JsonProperty]
        public int HTKMax { get; }
        #endregion

        #region Constructors
        public ComponentInstanceInfoDB() { }

        /// <summary>
        /// Constructor for a componentInstance.
        /// ComponentInstance stores component specific data such as hit points remaining etc.
        /// </summary>
        /// <param name="designEntity">The Component Entity, MUST have a ComponentInfoDB</param>
        /// <param name="isEnabled">whether the component is enabled on construction. default=true</param>
        public ComponentInstanceInfoDB(Entity designEntity, bool isEnabled = true)
        {
            if (designEntity.HasDataBlob<ComponentInfoDB>())
            {
                var componentInfo = designEntity.GetDataBlob<ComponentInfoDB>();
                DesignEntity = designEntity;
                IsEnabled = isEnabled;
                HTKRemaining = componentInfo.HTK;
                HTKMax = componentInfo.HTK;
            }
            else
            {
                throw new Exception("designEntity Must contain a ComponentInfoDB");
            }
        }


        public ComponentInstanceInfoDB(ComponentInstanceInfoDB instance)
        {
            DesignEntity = instance.DesignEntity;
            IsEnabled = instance.IsEnabled;
            ComponentLoadPercent = instance.ComponentLoadPercent;
            HTKRemaining = instance.HTKRemaining;
            HTKMax = instance.HTKMax;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new ComponentInstanceInfoDB(this);
        #endregion

        #region Public Methods
        public float HealthPercent() => HTKRemaining / HTKMax;
        #endregion
    }
}
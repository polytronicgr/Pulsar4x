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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsar4X.ECSLib
{

    public class ComponentInstanceInfoDB : BaseDataBlob
    {
        private Entity _parentEntity;
        private Entity _designEntity;
        private bool _isEnabled;
        private PercentValue _componentLoadPercent;
        private int _htkRemaining;
        private readonly int _htkMax;

        [JsonProperty]
        public Entity ParentEntity { get { return _parentEntity; } internal set { SetField(ref _parentEntity, value); } }

        [JsonProperty]
        public Entity DesignEntity { get { return _designEntity; } internal set { SetField(ref _designEntity, value); } }

        [JsonProperty]
        public bool IsEnabled { get { return _isEnabled; } internal set { SetField(ref _isEnabled, value); } }

        [JsonProperty]
        public PercentValue ComponentLoadPercent { get { return _componentLoadPercent; } internal set { SetField(ref _componentLoadPercent, value); } }

        [JsonProperty]
        public int HTKRemaining { get { return _htkRemaining; } internal set { SetField(ref _htkRemaining, value); } }

        [JsonProperty]
        public int HTKMax { get { return _htkMax; } }


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
                ComponentInfoDB componentInfo = designEntity.GetDataBlob<ComponentInfoDB>();
                DesignEntity = designEntity;
                IsEnabled = isEnabled;
                HTKRemaining = componentInfo.HTK;
                _htkMax = componentInfo.HTK;
            }
            else
                throw new Exception("designEntity Must contain a ComponentInfoDB");
        }


        public ComponentInstanceInfoDB(ComponentInstanceInfoDB instance)
        {
            DesignEntity = instance.DesignEntity;
            IsEnabled = instance.IsEnabled;
            ComponentLoadPercent = instance.ComponentLoadPercent;
            HTKRemaining = instance.HTKRemaining;
            _htkMax = instance.HTKMax;

        }

        public override object Clone()
        {
            return new ComponentInstanceInfoDB(this);
        }

        public float HealthPercent()
        { return HTKRemaining / HTKMax; }
    }
}

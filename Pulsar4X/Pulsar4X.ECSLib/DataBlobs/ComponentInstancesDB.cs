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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsar4X.ECSLib
{
    public class ComponentInstancesDB : BaseDataBlob
    {
        /// <summary>
        /// Key is the component design entity
        /// Value is a list of specific instances of that component design, that entity will hold info on damage, cooldown etc.
        /// </summary>
        [JsonProperty]
        [PublicAPI]
        public ObservableDictionary<Entity, ObservableCollection<Entity>> SpecificInstances { get; internal set; } = new ObservableDictionary<Entity, ObservableCollection<Entity>>();


        // list of components and where in the ship they are.
        public ObservableDictionary<Entity, double> ComponentDictionary { get; set; } = new ObservableDictionary<Entity, double>();

        public ComponentInstancesDB()
        {
            SpecificInstances.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(SpecificInstances), args);
            ComponentDictionary.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ComponentDictionary), args);
        }

        public ComponentInstancesDB(IDictionary<Entity, ObservableCollection<Entity>> specificInstances, IDictionary<Entity, double> componentDiectory) : this()
        {
            SpecificInstances.Merge(specificInstances);
            ComponentDictionary.Merge(componentDiectory);
        }
        
        public ComponentInstancesDB(ComponentInstancesDB db) : this(db.SpecificInstances, db.ComponentDictionary) { }

        /// <summary>
        /// this is a shallow clone. it does not clone the referenced component instance entities!!!
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new ComponentInstancesDB(this);
        }
    }
}

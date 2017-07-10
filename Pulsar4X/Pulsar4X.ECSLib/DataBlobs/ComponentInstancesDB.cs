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
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class ComponentInstancesDB : BaseDataBlob
    {
        #region Fields
        private ObservableDictionary<Entity, double> _componentDictionary;
        private ObservableDictionary<Entity, ObservableCollection<Entity>> _specificInstances;
        #endregion

        #region Properties
        /// <summary>
        /// Key is the component design entity
        /// Value is a list of specific instances of that component design, that entity will hold info on damage, cooldown etc.
        /// </summary>
        [JsonProperty]
        [PublicAPI]
        public ObservableDictionary<Entity, ObservableCollection<Entity>> SpecificInstances
        {
            get { return _specificInstances; }
            set
            {
                SetField(ref _specificInstances, value);
                SpecificInstances.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(SpecificInstances), args);
            }
        }


        // list of components and where in the ship they are.
        public ObservableDictionary<Entity, double> ComponentDictionary
        {
            get { return _componentDictionary; }
            set
            {
                SetField(ref _componentDictionary, value);
                ComponentDictionary.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ComponentDictionary), args);
            }
        }
        #endregion

        #region Constructors
        public ComponentInstancesDB()
        {
            SpecificInstances = new ObservableDictionary<Entity, ObservableCollection<Entity>>();
            ComponentDictionary = new ObservableDictionary<Entity, double>();
        }

        public ComponentInstancesDB(IDictionary<Entity, ObservableCollection<Entity>> specificInstances, IDictionary<Entity, double> componentDiectory) : this()
        {
            SpecificInstances.Merge(specificInstances);
            ComponentDictionary.Merge(componentDiectory);
        }

        public ComponentInstancesDB(ComponentInstancesDB db) : this(db.SpecificInstances, db.ComponentDictionary) { }
        #endregion

        #region Interfaces, Overrides, and Operators
        /// <summary>
        /// this is a shallow clone. it does not clone the referenced component instance entities!!!
        /// </summary>
        /// <returns></returns>
        public override object Clone() => new ComponentInstancesDB(this);
        #endregion
    }
}
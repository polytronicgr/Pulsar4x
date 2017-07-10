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

using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// this datablob allows an entity to be orderable.
    /// </summary>
    public class OrderableDB : BaseDataBlob
    {
        #region Fields
        private ObservableCollection<BaseAction> _actionQueue;
        #endregion

        #region Properties
        [JsonProperty]
        public ObservableCollection<BaseAction> ActionQueue
        {
            get { return _actionQueue; }
            set
            {
                _actionQueue = value;
                ActionQueue.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(ActionQueue), args);
            }
        }
        #endregion

        #region Constructors
        public OrderableDB() { ActionQueue = new ObservableCollection<BaseAction>(); }

        public OrderableDB(OrderableDB db) { ActionQueue = new ObservableCollection<BaseAction>(db.ActionQueue); }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new OrderableDB(this);
        #endregion
    }
}
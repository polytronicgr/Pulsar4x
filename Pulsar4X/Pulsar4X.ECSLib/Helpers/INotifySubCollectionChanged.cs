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
using System.Collections.Specialized;
using System.Reflection;

namespace Pulsar4X.ECSLib
{
    public interface INotifySubCollectionChanged
    {
        event SubCollectionChangedEventHandler SubCollectionChanged;
    }


    public delegate void SubCollectionChangedEventHandler(object sender, SubCollectionChangedEventArgs e);

    public class SubCollectionChangedEventArgs
    {
        public string CollectionPropertyName;
        public NotifyCollectionChangedEventArgs SubEventArgs;

        public SubCollectionChangedEventArgs(string collectionPropertyName, NotifyCollectionChangedEventArgs subEventArgs)
        {
            CollectionPropertyName = collectionPropertyName;
            SubEventArgs = subEventArgs;
        }
    }

    public class SubCollectionSyncHelper
    {
        public static void SyncCollection(object syncTarget, SubCollectionChangedEventArgs syncEvent)
        {
            PropertyInfo targetPropertyInfo = syncTarget?.GetType().GetProperty(syncEvent.CollectionPropertyName);
            dynamic targetCollection = Convert.ChangeType(targetPropertyInfo.GetValue(syncTarget), targetPropertyInfo.PropertyType);

            if (targetCollection == null)
            {
                return;
            }

            // Dynamic calls require unboxed values.
            dynamic unboxedValue;

            switch (syncEvent.SubEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    unboxedValue = Convert.ChangeType(syncEvent.SubEventArgs.NewItems[0], syncEvent.SubEventArgs.NewItems[0].GetType());
                    targetCollection.Add(unboxedValue);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    unboxedValue = Convert.ChangeType(syncEvent.SubEventArgs.OldItems[0], syncEvent.SubEventArgs.OldItems[0].GetType());
                    targetCollection.Remove(unboxedValue);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    targetCollection[syncEvent.SubEventArgs.OldStartingIndex] = syncEvent.SubEventArgs.NewItems[0];
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException("subcollectionSyncHelper.SyncCollection. Move feature may not be needed. Implement if exception hit.");
                    //break;
                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Pulsar4X.ECSLib
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Constants
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
        private const string KeysName = "Keys";
        private const string ValuesName = "Values";
        #endregion

        #region Properties
        protected IDictionary<TKey, TValue> Dictionary { get; private set; }


        ICollection IDictionary.Keys => ((IDictionary)Dictionary).Keys;
        ICollection IDictionary.Values => ((IDictionary)Dictionary).Values;

        public ICollection<TKey> Keys => Dictionary.Keys;
        public ICollection<TValue> Values => Dictionary.Values;


        public TValue this[TKey key] { get { return Dictionary[key]; } set { Insert(key, value, false); } }

        object IDictionary.this[object key] { get { return this[(TKey)key]; } set { this[(TKey)key] = (TValue)value; } }

        public int Count => Dictionary.Count;

        object ICollection.SyncRoot => ((ICollection)Dictionary).SyncRoot;
        bool ICollection.IsSynchronized => ((IDictionary)Dictionary).IsSynchronized;


        public bool IsReadOnly => Dictionary.IsReadOnly;

        bool IDictionary.IsFixedSize => ((IDictionary)Dictionary).IsFixedSize;
        #endregion

        #region Events
        #region INotifyCollectionChanged Members
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion


        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #endregion

        #region Constructors
        public ObservableDictionary() { Dictionary = new Dictionary<TKey, TValue>(); }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary) { Dictionary = new Dictionary<TKey, TValue>(dictionary); }

        public ObservableDictionary(IEqualityComparer<TKey> comparer) { Dictionary = new Dictionary<TKey, TValue>(comparer); }

        public ObservableDictionary(int capacity) { Dictionary = new Dictionary<TKey, TValue>(capacity); }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) { Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer); }

        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer) { Dictionary = new Dictionary<TKey, TValue>(capacity, comparer); }
        #endregion

        #region Interfaces, Overrides, and Operators
        bool IDictionary.Contains(object key) => ((IDictionary)Dictionary).Contains(key);
        void IDictionary.Add(object key, object value) { Add((TKey)key, (TValue)value); }

        IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)Dictionary).GetEnumerator();
        void IDictionary.Remove(object key) => Remove((TKey)key);

        void ICollection.CopyTo(Array array, int index) => ((ICollection)Dictionary).CopyTo(array, index);

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dictionary.GetEnumerator();
        #endregion


        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Dictionary).GetEnumerator();
        #endregion

        public void Add(TKey key, TValue value) => Insert(key, value, true);
        public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);


        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            TValue value;
            Dictionary.TryGetValue(key, out value);
            bool removed = Dictionary.Remove(key);
            if (removed)
                //OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
            {
                OnCollectionChanged();
            }

            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value) => Dictionary.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item) { Insert(item.Key, item.Value, true); }

        public void Clear()
        {
            if (Dictionary.Count > 0)
            {
                Dictionary.Clear();
                OnCollectionChanged();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => Dictionary.Contains(item);


        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => Dictionary.CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);


        protected virtual void OnPropertyChanged(string propertyName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        #endregion

        #region Public Methods
        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }


            if (items.Count > 0)
            {
                if (Dictionary.Count > 0)
                {
                    if (items.Keys.Any(k => Dictionary.ContainsKey(k)))
                    {
                        throw new ArgumentException("An item with the same key has already been added.");
                    }
                    foreach (KeyValuePair<TKey, TValue> item in items)
                    {
                        Dictionary.Add(item);
                    }
                }
                else
                {
                    Dictionary = new Dictionary<TKey, TValue>(items);
                }


                OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
            }
        }

        public void Merge(IDictionary<TKey, TValue> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (KeyValuePair<TKey, TValue> kvp in items)
            {
                if (Dictionary.ContainsKey(kvp.Key))
                {
                    this[kvp.Key] = kvp.Value;
                }
                else
                {
                    Add(kvp);
                }
            }
        }
        #endregion

        #region Private Methods
        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }


            TValue item;
            if (Dictionary.TryGetValue(key, out item))
            {
                if (add)
                {
                    throw new ArgumentException("An item with the same key has already been added.");
                }
                if (Equals(item, value))
                {
                    return;
                }
                Dictionary[key] = value;


                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
            }
            else
            {
                Dictionary[key] = value;

                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
            }
        }


        private void OnPropertyChanged()
        {
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnPropertyChanged(KeysName);
            OnPropertyChanged(ValuesName);
        }


        private void OnCollectionChanged()
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem));
        }


        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
        }


        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItems));
        }
        #endregion
    }
}
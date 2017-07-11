using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pulsar4X.ECSLib
{
    public class StoredObservableDictionary<TK, TV> : IDictionary<TK, TV>
    {
        public enum DictionaryActions
        {
            Add,
            Remove,
            Replace,
            Clear,
        }
        public struct DictionaryActionData
        {
            public DictionaryActions Action;
            public KeyValuePair<TK, TV> KeyValuePair;
        }
        
        internal Dictionary<TK, TV> InternalDictionary { get; set; }

        internal List<DictionaryActionData> Changes { get; private set; } = new List<DictionaryActionData>();

        public StoredObservableDictionary(Dictionary<TK, TV> dictionary) { InternalDictionary = dictionary; }

        /// <summary>
        /// Threadsafe
        /// </summary>
        /// <returns></returns>
        public List<DictionaryActionData> GetAndClearChanges()
        {
            List<DictionaryActionData> changes;
            lock (Changes)
            {
                changes = Changes.ToList();
                Changes = new List<DictionaryActionData>();                
            }        
            return changes;
        }

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator() => InternalDictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public void Add(KeyValuePair<TK, TV> item)
        {
            InternalDictionary.Add(item.Key, item.Value);
            Changes.Add(new DictionaryActionData()
            {
                Action = DictionaryActions.Add, 
                KeyValuePair = item
            }); 
        }

        public void Clear()
        {
            InternalDictionary.Clear();
            Changes.Add(new DictionaryActionData()
            {
                Action = DictionaryActions.Clear,                
            });
        }

        public bool Contains(KeyValuePair<TK, TV> item) => InternalDictionary.Contains(item);

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex) { throw new NotImplementedException(); }

        public bool Remove(KeyValuePair<TK, TV> item)
        {
            bool wasSuccessfull = InternalDictionary.Remove(item.Key);
            if (wasSuccessfull)
            {
                Changes.Add(new DictionaryActionData()
                {
                    Action = DictionaryActions.Remove,
                    KeyValuePair = item                    
                });
            }
            return wasSuccessfull;
        }

        public int Count => InternalDictionary.Count;
        public bool IsReadOnly { get; }

        public void Add(TK key, TV value)
        {
            Add(new KeyValuePair<TK, TV>(key,value));
        }

        public bool ContainsKey(TK key) => InternalDictionary.ContainsKey(key);

        public bool Remove(TK key) => Remove(new KeyValuePair<TK, TV>(key, InternalDictionary[key]));

        public bool TryGetValue(TK key, out TV value) => InternalDictionary.TryGetValue(key, out value);

        public TV this[TK key]
        {
            get { return InternalDictionary[key]; }
            set
            {
                InternalDictionary[key] = value;
                Changes.Add(new DictionaryActionData()
                {
                    Action = DictionaryActions.Replace,
                    KeyValuePair = new KeyValuePair<TK, TV>(key, value)
                });
            }
        }

        public ICollection<TK> Keys => InternalDictionary.Keys;
        public ICollection<TV> Values => InternalDictionary.Values;
    }
}
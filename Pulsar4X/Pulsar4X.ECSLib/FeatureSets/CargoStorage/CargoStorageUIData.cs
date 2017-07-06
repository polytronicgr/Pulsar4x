using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Antlr.Runtime;
using Newtonsoft.Json;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib
{
    public class CargoStorageUIData : UIData
    {

        public Dictionary<Guid, CargoStorageTypeData> CargoByType = new Dictionary<Guid, CargoStorageTypeData>();

        //this needs to be moved to UI side.
        public Dictionary<Guid, CargoableUIData> CargoableData = new Dictionary<Guid, CargoableUIData>();
        
        
        [JsonConstructor]
        public CargoStorageUIData() { }

        public CargoStorageUIData(StaticDataStore staticData, CargoStorageDB db)
        {
            foreach (var kvp in db.StorageByType)
            {                
                CargoByType.Add(kvp.Key, kvp.Value);
                foreach (var kvp2 in kvp.Value.StoredByItemID)
                {
                    CargoableData.Add(kvp2.Key, new CargoableUIData(staticData.GetICargoable(kvp2.Key)));
                }
            }
            


        }

  
        /// <summary>
        /// A concreation of ICargoable which can more easly be seralised. 
        /// </summary>
        public struct CargoableUIData : ICargoable
        {
            public Guid ID { get; }
            public string Name { get; }
            public string ItemTypeName { get; }
            public Guid CargoTypeID { get; }
            public float Mass { get; }

            public CargoableUIData(ICargoable cargoableObject)
            {
                ID = cargoableObject.ID;
                Name = cargoableObject.Name;
                ItemTypeName = cargoableObject.ItemTypeName;
                CargoTypeID = cargoableObject.CargoTypeID;
                Mass = cargoableObject.Mass;

            }
        }
        
        
    }

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
    
    

    public class StoredObservableList<T> : IList<T>
    {

        public enum ListActions
        {
            Add,
            Insert,
            Remove,
            RemoveAt,
            Clear,
            Replace
        }
        
        public struct ListActionData
        {
            public ListActions Action;
            public int Index;
            public T NewData;
        }
        
        
        internal List<T> InternalList { get; private set; }

        internal List<ListActionData> Changes { get; private set; } = new List<ListActionData>();
        
        public StoredObservableList(List<T> list)
        {
            InternalList = list;
        }

        /// <summary>
        /// Threadsafe
        /// </summary>
        /// <returns></returns>
        public List<ListActionData> GetAndClearChanges()
        {
            List<ListActionData> changes;
            lock (Changes)
            {
                changes = Changes.ToList();
                Changes = new List<ListActionData>();                
            }        
            return changes;
        }
        
        public IEnumerator<T> GetEnumerator() => InternalList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            InternalList.Add(item); 
            Changes.Add(new ListActionData()
            {
                Action = ListActions.Add, 
                NewData = item
            });
        }

        public void Clear()
        {
            InternalList.Clear();
            Changes.Add(new ListActionData() {Action = ListActions.Clear});
        }

        public bool Contains(T item) => InternalList.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) { InternalList.CopyTo(array, arrayIndex); }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0) {
                RemoveAt(index);
                return true;
            }

            return false;              
        }

        public int Count => InternalList.Count;
        
        public bool IsReadOnly { get; }

        public int IndexOf(T item) => InternalList.IndexOf(item);

        public void Insert(int index, T item)
        {
            InternalList.Insert(index, item);
            Changes.Add(new ListActionData(){Action = ListActions.Insert, NewData = item});
        }

        public void RemoveAt(int index)
        {
            InternalList.RemoveAt(index);
            Changes.Add(new ListActionData(){Action = ListActions.RemoveAt, Index = index});
        }

        public T this[int index]
        {
            get {return InternalList[index]; }
            set
            {
                InternalList[index] = value;
                Changes.Add(new ListActionData(){Action = ListActions.Replace, Index = index, NewData = value});
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pulsar4X.ECSLib
{
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
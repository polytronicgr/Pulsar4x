using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Antlr.Runtime;
using Newtonsoft.Json;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib
{
    public class CargoStorageUIData : UIData
    {
        public static string DataCode = "CargoData";

        public override string GetDataCode{get { return DataCode; }}

        [JsonProperty]
        public Dictionary<CargoTypeSD, long> TotalCapacities = new Dictionary<CargoTypeSD, long>();

        public Dictionary<CargoTypeSD, CargoStorageTypeData> CargoByType = new Dictionary<CargoTypeSD, CargoStorageTypeData>();

        
        [JsonConstructor]
        public CargoStorageUIData() { }

        public CargoStorageUIData(StaticDataStore staticData, CargoStorageDB db)
        {
            foreach (var kvp in db.StorageByType)
            {
                CargoTypeSD cargoType = staticData.CargoTypes[kvp.Key];
                TotalCapacities.Add(cargoType, kvp.Value.Capacity);
                
                CargoByType.Add(cargoType, kvp.Value);
                
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
        
        public class StoredObservableList<T> : IList<T>
        {

            public enum Actions
            {
                Add,
                Insert,
                Remove,
                RemoveAt,
                Clear,
                Replace
            }
            
            public struct ActionData
            {
                public Actions Action;
                public int Index;
                public T NewData;
            }
            
            
            internal List<T> InternalList { get; private set; }

            internal List<ActionData> Changes { get; } = new List<ActionData>();
            
            public StoredObservableList(List<T> list)
            {
                InternalList = list;
            }

            public IEnumerator<T> GetEnumerator() => InternalList.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(T item)
            {
                InternalList.Add(item); 
                Changes.Add(new ActionData()
                {
                    Action = Actions.Add, 
                    NewData = item
                });
            }

            public void Clear()
            {
                InternalList.Clear();
                Changes.Add(new ActionData() {Action = Actions.Clear});
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
                Changes.Add(new ActionData(){Action = Actions.Insert, NewData = item});
            }

            public void RemoveAt(int index)
            {
                InternalList.RemoveAt(index);
                Changes.Add(new ActionData(){Action = Actions.RemoveAt, Index = index});
            }

            public T this[int index]
            {
                get {return InternalList[index]; }
                set
                {
                    InternalList[index] = value;
                    Changes.Add(new ActionData(){Action = Actions.Replace, Index = index, NewData = value});
                }
            }
        }
    }
}
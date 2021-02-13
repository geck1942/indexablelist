using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polycorne.SiliconCity.Mechanics.GenericsEnhanced
{
    /// <summary>
    /// 
    /// IndexableList Extends the Generic.List class and retreive objects faster with a hidden dictionary.
    /// Objects stored must implement the IIndexable interface.
    /// ----
    /// If an item is added to the list but its key has been added already. This happens usually with a standard List.
    /// The key dictionary is not updated, but onRemove, it will look for another item with the same key to index at
    /// </summary>
    /// <typeparam name="TKey">Type of the Key on which items are indexed. Must implement IEquatable</typeparam>
    /// <typeparam name="TValue">Items of the List Type</typeparam>
    public class IndexableList<TKey, TValue> : IEnumerable<TValue>, IEnumerable
        where TKey : IEquatable<TKey>
        where TValue : IIndexable<TKey>
    {
        /// <summary>
        /// The List where elements are added. Classic Generic list.
        /// </summary>
        private List<TValue> _list;
        /// <summary>
        /// The Key-indexed dictionary. A replication of <seealso cref="_list"/>, but for indexation.
        /// </summary>
        private Dictionary<TKey, TValue> _IndexableList;

        /// <summary>
        /// Set to false, to prevent adding a null value to the list because it would lack the key counter part. 
        /// Set to True to force it. This may lead into Exceptions, getting a key from a null Object.
        /// </summary>
        public bool IndexNullValues = false;

        /// <summary>
        /// Optional to prevent several items to have the same key.
        /// Default value: true, this is how a Generic.List would behave.
        /// If set to True, this will decrease a bit the performances upon item removal, but might be very helpful to replace a List<T> object.
        /// When manipulating extra large amounts of records and not caring about which item to get as long as they identify to their Key.
        /// </summary>
        public bool AllowNonUniqueKey = true;

        /// <summary>
        /// Main constructor.
        /// </summary>
        public IndexableList()
        {
            this._list = new List<TValue>();
            this._IndexableList = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Initialize with an Array
        /// </summary>
        /// <param name="InitialArray"></param>
        public IndexableList(List<TValue> InitialArray)
            : this()
        {
            if (InitialArray != null && InitialArray.Any())
                this.AddRange(InitialArray);
        }

        #region IEnumerable & List Methods

        /// <summary>
        /// Finds quickly if an item that can be found with a unique Key is in the list.
        /// The main goal of this class is here, using an indexed dictionary over a simple List.
        /// </summary>
        /// <param name="ComparableItem">The item to delete, at least the one with that <seealso cref="IIndexable{TKey}.Key">Key</seealso></param>
        /// <returns>True if an item with the same key has been found</returns>
        public bool Contains(TValue ComparableItem)
        {
            return this._IndexableList.ContainsKey(ComparableItem.Key);
        }

        /// <summary>
        /// Adds an item to the list. If an item with the same key exists and AllowNonUniqueKey permits it, it will be added anyway.
        /// </summary>
        /// <param name="newItem">The item to add to the list</seealso></param>
        public void Add(TValue newItem)
        {
            if (IndexNullValues || newItem != null)
            {
                if (this._IndexableList.ContainsKey(newItem.Key))
                {
                    if(AllowNonUniqueKey)
                        this._list.Add(newItem);
                }
                else
                {
                    this._list.Add(newItem);
                    this._IndexableList.Add(newItem.Key, newItem);
                }
            }
        }
        /// <summary>
        /// Adds a list of items to the list. If AllowNonUniqueKey permits it and any of these items share the same key as another items, it will be added anyway.
        /// </summary>
        /// <param name="newItems">The items to add to the list</seealso></param>
        public void AddRange(IEnumerable<TValue> newItems)
        {
            if (newItems != null)
                foreach (var newItem in newItems)
                    this.Add(newItem);
        }

        /// <summary>
        /// Removes the item from the list. It will match the first item with that key.
        /// </summary>
        /// <param name="oldItem">The Item to remove</param>
        public void Remove(TValue oldItem)
        {
            this._list.Remove(oldItem);
            if (IndexNullValues || oldItem != null)
            {
                // remove from dictionary
                this._IndexableList.Remove(oldItem.Key);
                // if any item remains, adds back any other item with that key to keep stuff being found;
                if(AllowNonUniqueKey)
                {
                    // now find any other item with that key
                    var anyRemainingItemWithThisKey = this._list.FirstOrDefault(othrItem => othrItem.Key.Equals(oldItem.Key));
                    // and add it back ad indexed in dictionary
                    if (anyRemainingItemWithThisKey != null)
                        this._IndexableList.Add(oldItem.Key, anyRemainingItemWithThisKey);
                }
            }
        }

        /// <summary>
        /// Remove element at the index of the List
        /// </summary>
        public void RemoveAt(int index)
        {
            // remove value by its index
            this.Remove(this._list[index]);
        }

        /// <summary>
        /// Remove n elements from a position in the list
        /// </summary>
        public void RemoveRange(int index, int count)
        {
            for (int i = 0; i < count && this._list.Count() > index; i++)
                this.RemoveAt(index);
        }
        public int Count()
        {
            return this._list.Count();
        }
        public bool Any()
        {
            return this._list.Count() > 0;
        }
        public TValue First()
        {
            if (this._list.Any())
                return this._list[0];
            else
                return default(TValue);
        }
        public TValue ElementAt(int index)
        {
            return this._list.ElementAt(index);
        }
        public List<TValue> ToList()
        {
            return this._list;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

    }
}

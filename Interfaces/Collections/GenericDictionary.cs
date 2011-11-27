using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace B4F.TotalGiro.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class GenericDictionary<TKey, TValue> : IGenericDictionary<TKey, TValue>
    {
        #region Constructors

        /// <exclude/>
        public GenericDictionary() { }


        /// <exclude/>
        public GenericDictionary(IList bagCollection, string keyProperty)
            : this(bagCollection, keyProperty, false) 
        {
        }

        /// <exclude/>
        public GenericDictionary(IList bagCollection, string keyProperty, bool readOnly)
        {
            values = new GenericCollection<TValue>(bagCollection, readOnly); 
            this.readOnly = readOnly;

            Type t = typeof(TValue);
            foreach (TValue item in values)
            {
                TKey key = (TKey)t.InvokeMember(keyProperty, BindingFlags.GetProperty, null, item, new object[] { });
                keys.Add(key);
            }
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// Add a new item to the collection.
        /// </summary>
        /// <param name="key">The key that belongs to the item being added</param>
        /// <param name="value">The item being added</param>
        public virtual void Add(TKey key, TValue value)
        {
            checkReadOnly();
            keys.Add(key);
            values.Add(value);
        }

        /// <summary>
        /// Check whether an key exists in the collection
        /// </summary>
        /// <param name="key">The relevant key</param>
        /// <returns>True when the key exists in the collection</returns>
        public virtual bool ContainsKey(TKey key)
        {
            bool ret = false;

            foreach (TKey colItem in keys)
            {
                if (colItem.Equals(key))
                    return ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Return all the keys
        /// </summary>
        public virtual ICollection<TKey> Keys
        {
            get { return keys; }
        }

        /// <summary>
        /// Removes an item from the collection by using the key
        /// </summary>
        /// <param name="key">The relevant key</param>
        /// <returns>Returns true when successfull</returns>
        public virtual bool Remove(TKey key)
        {
            bool success = false;
            checkReadOnly();
            if (ContainsKey(key))
            {
                int count = values.Count;
                values.RemoveAt(keys.IndexOf(key));
                if (count > values.Count)
                    success = true;
            }
            return success;
        }

        /// <summary>
        /// This method tries to retrieve a value by using the key
        /// </summary>
        /// <param name="key">the relevant key</param>
        /// <param name="value">The item being returned</param>
        /// <returns>The item being searched for</returns>
        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            bool retVal = false;
            value = default(TValue);
            if (ContainsKey(key))
            {
                value = values[keys.IndexOf(key)];
                retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// The collection with the values
        /// </summary>
        public virtual ICollection<TValue> Values
        {
            get { return values; }
        }

        /// <summary>
        /// Returns an item out of the collection by a key passed in
        /// </summary>
        /// <param name="key">The relevant key</param>
        /// <returns>The relevant item</returns>
        public virtual TValue this[TKey key]
        {
            get
            {
                if (ContainsKey(key))
                    return values[keys.IndexOf(key)];
                else
                    return default(TValue);
            }
            set
            {
                if (ContainsKey(key))
                    values[keys.IndexOf(key)] = (TValue)value; ;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Add a new item to the collection.
        /// </summary>
        /// <param name="item">The key / value pair</param>
        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            checkReadOnly();
            keys.Add(item.Key);
            values.Add(item.Value);
        }

        /// <summary>
        /// Clears all items from the collection
        /// </summary>
        public virtual void Clear()
        {
            checkReadOnly();
            keys.Clear();
            values.Clear();
        }

        /// <summary>
        /// Check whether an item exists in the collection
        /// </summary>
        /// <param name="item">The relevant item</param>
        /// <returns>True when the item exists in the collection</returns>
        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return keys.Contains(item.Key);
        }

        /// <summary>
        /// This method copies the items from the collection to an array upto the supplied index
        /// </summary>
        /// <param name="array">The returned array</param>
        /// <param name="arrayIndex">The supplied index</param>
        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < values.Count; i++)
            {
                array[i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }

        /// <summary>
        /// Returns the total number of items in the collection
        /// </summary>
        public virtual int Count
        {
            get { return values.Count; }
        }

        /// <summary>
        /// Returns whether the collection is readonly
        /// </summary>
        public virtual bool IsReadOnly
        {
            get { return readOnly; }
        }

        private void checkReadOnly()
        {
            if (IsReadOnly)
                throw new ApplicationException("This action is not allowed since the collection is read-only.");

        }

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="item">The relevant item</param>
        /// <returns>Returns true when successfull</returns>
        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            bool success = false;
            checkReadOnly();
            if (keys.Contains(item.Key))
            {
                int count = values.Count;
                values.RemoveAt(keys.IndexOf(item.Key));
                keys.Remove(item.Key);
                if (count > values.Count)
                {
                    success = true;
                }
            }
            return success;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        /// <exclude/>
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < values.Count; i++)
                yield return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
        }

        #endregion

        #region IEnumerable Members

        /// <exclude/>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }

        #endregion

        #region Privates

        /// <exclude/>
        private GenericCollection<TKey> keys = new GenericCollection<TKey>();
        private GenericCollection<TValue> values = new GenericCollection<TValue>();
        private bool readOnly = false;

        #endregion
    
    }
}

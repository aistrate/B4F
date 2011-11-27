using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Utils;
using System.Reflection;

namespace B4F.TotalGiro.Collections
{
    /// <moduleiscollection>
    /// </moduleiscollection>
    public class GenericCollection<T> : IGenericCollection<T>
    {
        public GenericCollection() { }

        public GenericCollection(ICollection bagCollection) : this(bagCollection, false) { }

        public GenericCollection(ICollection bagCollection, bool readOnly) : this(new ArrayList(bagCollection), readOnly) { }

        public GenericCollection(IList bagCollection) : this(bagCollection, false) { }
        
        public GenericCollection(IList bagCollection, bool readOnly)
        {
            this.bagCollection = bagCollection;
            this.readOnly = readOnly;
        }

        #region IList<T> Members

        /// <summary>
        /// Returns the relevant index of item in the collection
        /// </summary>
        /// <param name="item">The relevant item</param>
        /// <returns>The index</returns>
        public virtual int IndexOf(T item)
        {
            return bagCollection.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item with a certain index
        /// </summary>
        /// <param name="index">The index number</param>
        /// <param name="item">The item that is to be added</param>
        public virtual void Insert(int index, T item)
        {
            checkReadOnly();
            bagCollection.Insert(index, item);
        }

        /// <summary>
        /// Removes an item from the collection by an index number
        /// </summary>
        /// <param name="index">The index number</param>
        public virtual void RemoveAt(int index)
        {
            checkReadOnly();
            bagCollection.RemoveAt(index);
        }

        /// <summary>
        /// Returns an item out of the collection by an index number passed in
        /// </summary>
        /// <param name="index">The index number</param>
        /// <returns>The relevant item</returns>
        public virtual T this[int index]
        {
            get
            {
                return (T)(bagCollection[index]);
            }
            set
            {
                bagCollection[index] = (T)value; ;
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Add a new item to the collection.
        /// </summary>
        /// <param name="item">The position being added</param>
        public virtual void Add(T item)
        {
            checkReadOnly();
            bagCollection.Add(item);
        }

        /// <summary>
        /// Clears all items from the collection
        /// </summary>
        public virtual void Clear()
        {
            checkReadOnly();
            bagCollection.Clear();
        }

        /// <summary>
        /// Check whether an item exists in the collection
        /// </summary>
        /// <param name="item">The relevant item</param>
        /// <returns>True when the item exists in the collection</returns>
        public virtual bool Contains(T item)
        {
            bool ret = false;

            foreach (T colItem in this)
            {
                if (colItem.Equals(item))
                {
                    return ret = true;
                }
            }
            return ret;
        }

        /// <summary>
        /// This method copies the items from the collection to an array upto the supplied index
        /// </summary>
        /// <param name="array">The returned array</param>
        /// <param name="arrayIndex">The supplied index</param>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            bagCollection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the total number of items in the collection
        /// </summary>
        public virtual int Count
        {
            get { return bagCollection.Count; }
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
        public virtual bool Remove(T item)
        {
            bool success = false;
            checkReadOnly();
            if (Contains(item))
            {
                int count = bagCollection.Count;
                bagCollection.Remove(item);
                if (count > bagCollection.Count)
                {
                    success = true;
                }
            }
			return success;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <exclude/>
        public virtual IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < bagCollection.Count; i++)
                yield return (T)bagCollection[i];
        }

        #endregion

        #region IEnumerable Members

        /// <exclude/>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return bagCollection.GetEnumerator();
        }

        #endregion

        #region Cloning

        protected virtual GenericCollection<T> Clone()
        {
            ArrayList clone = new ArrayList(bagCollection);
            return new GenericCollection<T>(clone);
        }

        #endregion

        public List<T> GetList()
        {
            return new List<T>(this);
        }

        #region Sorting

        protected delegate object PropertyGetter(T item);

        /// <summary>
        /// Override this method to specify the sort criterion used by SortByDefault().
        /// The returned value will be the sorting key corresponding to an object in the collection (the argument).
        /// </summary>
        protected virtual object GetDefaultSortValue(T item)
        {
            throw new ApplicationException(string.Format("Collection {0} cannot be sorted because it does not have a default sort value method.",
                                                         this.GetType().Name));
        }

        /// <summary>
        /// Generates a new collection of the same type (but still pointing to the old items), 
        /// behaving for all intents and purposes identically with the original object.
        /// A no-parameter constructor (which could be private) must exist on the collection type.
        /// </summary>
        public virtual GenericCollection<T> ShallowClone()
        {
            ConstructorInfo noParamsConstructor = this.GetType().GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (noParamsConstructor == null)
                throw new ApplicationException(string.Format("No constructor that takes no parameters could be found on type {0}.", 
                                                             this.GetType().Name));

            GenericCollection<T> clone = (GenericCollection<T>)noParamsConstructor.Invoke(new object[] { });

            foreach (FieldInfo field in getAllFields(this.GetType()))
                field.SetValue(clone, field.GetValue(this));

            return clone;
        }

        /// <summary>
        /// Generates a new collection of the same type (but still pointing to the old items), 
        /// sorted by the value returned by protected method GetDefaultSortValue().
        /// </summary>
        public IGenericCollection<T> SortedByDefault()
        {
            return SortedBy(GetDefaultSortValue);
        }
        
        /// <summary>
        /// Generates a new collection of the same type (but still pointing to the old items), 
        /// sorted by the property specified in the parameter.
        /// It uses reflection, so it will be slower (by about 20 times). For better performance and type safety use method SortedByDefault(), 
        /// after you have overriden method GetDefaultSortValue().
        /// </summary>
        public IGenericCollection<T> SortedBy(string propertyName)
        {
            return SortedBy(getPropertyGetter(propertyName));
        }

        protected IGenericCollection<T> SortedBy(PropertyGetter propertyGetter)
        {
            return SortedBy(new CollectionSorter(propertyGetter));
        }

        /// <summary>
        /// Generates a new collection of the same type (but still pointing to the old items), 
        /// sorted by the comparer specified in the parameter.
        /// </summary>
        public IGenericCollection<T> SortedBy(IComparer sorter)
        {
            ArrayList items = new ArrayList(this.bagCollection);
            items.Sort(sorter);

            GenericCollection<T> clone = this.ShallowClone();
            clone.bagCollection = items;
            return clone;
        }

        /// <summary>
        /// Generates a new collection of the same type (but still pointing to the old items), 
        /// where the order of the items is reversed.
        /// </summary>
        public IGenericCollection<T> Reversed()
        {
            ArrayList items = new ArrayList(this.bagCollection);
            items.Reverse();

            GenericCollection<T> clone = this.ShallowClone();
            clone.bagCollection = items;
            return clone;
        }

        private static List<FieldInfo> getAllFields(Type type)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            if (type != typeof(object))
            {
                fields.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                fields.AddRange(getAllFields(type.BaseType));
            }
            return fields;
        }

        private static PropertyGetter getPropertyGetter(string propertyName)
        {
            PropertyInfo property = typeof(T).GetProperty(propertyName);
            if (property == null)
                foreach (Type parentInterface in typeof(T).GetInterfaces())
                {
                    property = parentInterface.GetProperty(propertyName);
                    if (property != null)
                        break;
                }

            if (property != null)
                return delegate(T item) { return property.GetValue(item, null); };
            else
                throw new ApplicationException(string.Format("Property {0} not found on type {1}.", propertyName, typeof(T).Name));
        }

        private class CollectionSorter : IComparer
        {
            public CollectionSorter(PropertyGetter propertyGetter)
            {
                this.propertyGetter = propertyGetter;
            }

            public int Compare(object x, object y)
            {
                if (x == null) return -1;
                else if (y == null) return 1;
                else
                    return ((IComparable)propertyGetter((T)x)).CompareTo(propertyGetter((T)y));
            }

            private PropertyGetter propertyGetter;
        }

        #endregion

        #region Privates

        /// <exclude/>
        protected IList bagCollection = new ArrayList();
        private bool readOnly = false;
        
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace B4F.TotalGiro.Collections
{
    public interface IGenericCollection<T> : IList<T>
    {
        List<T> GetList();
        /// <summary>
        /// Generates a new collection of the same type (but still pointing to the old items), 
        /// behaving for all intents and purposes identically with the original object.
        /// A no-parameter constructor (which could be private) must exist on the collection type.
        /// </summary>
        GenericCollection<T> ShallowClone();
        /// <summary>
        /// Generates a new collection of the same type (but still pointing to the old items), 
        /// sorted by the value returned by protected method GetDefaultSortValue() of class GenericCollection.
        /// </summary>
        IGenericCollection<T> SortedByDefault();
        /// <summary>
        /// Generates a new collection of the same type (but still pointing to the old items), 
        /// sorted by the property specified in the parameter.
        /// It uses reflection, so it will be slower (by about 20 times). For better performance and type safety use method SortedByDefault(), 
        /// after you have overriden method GetDefaultSortValue() of class GenericCollection.
        /// </summary>
        IGenericCollection<T> SortedBy(string propertyName);
        /// <summary>
        /// Generates a new collection of the same type (but still pointing to the old items), 
        /// sorted by the comparer specified in the parameter.
        /// </summary>
        IGenericCollection<T> SortedBy(IComparer sorter);
        /// <summary>
        /// Generates a new collection of the same type (but still pointing to the old items), 
        /// where the order of the items is reversed.
        /// </summary>
        IGenericCollection<T> Reversed();
    }
}

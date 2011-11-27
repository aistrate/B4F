using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.UserTypes;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using System.Collections;

namespace B4F.TotalGiro.Collections.Persistence
{
    public class DomainCollectionFactory<TItem, TCollection> : IUserCollectionType
        where TCollection : TransientDomainCollection<TItem>, new()
    {
        public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
        {
            return new PersistentDomainCollection<TItem>(session);
        }

        public IPersistentCollection Wrap(ISessionImplementor session, object collection)
        {
            return new PersistentDomainCollection<TItem>(session, collection as IList<TItem>);
        }

        public object Instantiate(int anticipatedSize)
        {
            return new TCollection();
        }

        public IEnumerable GetElements(object collection)
        {
            return (IEnumerable)collection;
        }

        public bool Contains(object collection, object entity)
        {
            return ((IList)collection).Contains(entity);
        }

        public object IndexOf(object collection, object entity)
        {
            return ((IList)collection).IndexOf(entity);
        }

        public object ReplaceElements(object original, object target, ICollectionPersister persister,
                                      object owner, IDictionary copyCache, ISessionImplementor session)
        {
            IList result = (IList)target;
            result.Clear();
            foreach (object o in ((IEnumerable)original))
                result.Add(o);
            return result;
        }
    }
}

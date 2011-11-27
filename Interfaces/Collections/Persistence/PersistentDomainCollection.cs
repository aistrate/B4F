using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using System.Collections;
using NHibernate.Collection;

namespace B4F.TotalGiro.Collections.Persistence
{
    public class PersistentDomainCollection<T> : PersistentGenericBag<T>, IDomainCollection<T>
    {
        public PersistentDomainCollection(ISessionImplementor session)
            : base(session)
        {
        }

        public PersistentDomainCollection(ISessionImplementor session, IList<T> coll)
            : base(session, coll)
        {
        }

        public List<T> AsList()
        {
            Initialize(true);
            return (List<T>)InternalBag;
        }

        public IList<T> AsReadOnlyList()
        {
            return new ReadOnlyCollection<T>(AsList());
        }
    }
}

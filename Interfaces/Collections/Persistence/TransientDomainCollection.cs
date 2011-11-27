using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Collections.Persistence
{
    public class TransientDomainCollection<T> : List<T>, IDomainCollection<T>
    {
        public TransientDomainCollection()
            : base()
        {
        }

        public TransientDomainCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public TransientDomainCollection(int capacity)
            : base(capacity)
        {
        }

        public List<T> AsList()
        {
            return this;
        }

        public IList<T> AsReadOnlyList()
        {
            return new ReadOnlyCollection<T>(AsList());
        }

        public bool IsInitialized { get; set; }
    }
}

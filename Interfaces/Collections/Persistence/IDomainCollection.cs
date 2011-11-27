using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Collections.Persistence
{
    public interface IDomainCollection<T> : IList<T>
    {
        List<T> AsList();
        IList<T> AsReadOnlyList();
    }
}

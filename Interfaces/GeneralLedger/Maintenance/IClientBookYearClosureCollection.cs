using System;
using System.Collections.Generic;
namespace B4F.TotalGiro.GeneralLedger.Journal.Maintenance
{
    public interface IClientBookYearClosureCollection : IList<IClientBookYearClosure>
    {
        void AddClientBookYearClosure(IClientBookYearClosure closure);
        IBookYearClosure Parent { get; set; }
    }
}

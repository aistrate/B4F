using System;
using B4F.TotalGiro.Orders.Transactions;
using System.Collections.Generic;
namespace B4F.TotalGiro.Instruments.History
{
    public interface ICorporateActionCollection : IList<ICorporateAction>
    {
        void AddInstrumentHistory(ICorporateAction entry);
        IInstrumentHistory Parent { get; set; }
    }
}

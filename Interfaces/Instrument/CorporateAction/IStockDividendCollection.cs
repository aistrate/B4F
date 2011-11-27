using System;
using System.Collections.Generic;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public interface IStockDividendCollection : IList<ICorporateActionStockDividend>
    {
        void AddStockDividend(ICorporateActionStockDividend transaction);
        IDividendHistory Parent { get; set; }
        InstrumentSize TotalUnits { get; }
        Money TotalDividendAmount { get; }
    }
}

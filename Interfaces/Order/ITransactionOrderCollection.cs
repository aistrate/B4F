using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Orders
{
    public interface ITransactionOrderCollection : IList<ITransactionOrder>
    {
        IOrder Parent { get; set; }
        void AddTransactionOrder(ITransactionOrder item);
        Money TotalCounterValueSize { get;  }
        Money TotalCommission { get; }
        Money TotalServiceCharge { get; }
        Money TotalAccruedInterest { get; }
        InstrumentSize TotalValueSize { get; }
        bool ContainsTrade(ITransactionOrder trade);
        void RemoveTrade(ITransactionOrder trade);
        decimal TotalApprovedFillRatio();
        decimal TotalFillRatio();

    }
}

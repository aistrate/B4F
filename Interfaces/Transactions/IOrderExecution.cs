using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface IOrderExecution : ITransactionOrder
    {
        bool IsAllocated { get; set; }
        DateTime AllocationDate { get; }
        ICrumbleTransaction CreateCrumble(IGLLookupRecords lookups, ITradingJournalEntry tradingJournalEntry);
        InstrumentSize TotalSizeAllocated { get; }
        IOrderExecutionChildCollection Allocations { get; }
        bool IsSettled { get; set; }
        DateTime ActualSettlementDate { get;  }
        void SetIsAllocated();
        void SettleExternal(DateTime settlementDate);
    }
}

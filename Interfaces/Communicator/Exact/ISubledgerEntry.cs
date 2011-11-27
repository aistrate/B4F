using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Communicator.Exact
{
    public interface ISubledgerEntry
    {
        string FormatLine();
        int Key { get; set; }
        ILedgerEntry Parent { get; set; }
        int LineNumber { get; set; }
        string Description { get; set; }
        string GeneralLedgerAccount { get; set; }
        string Debitor { get; set; }
        Decimal Amount { get; set; }
        string Currency { get; set; }
        decimal ExRateforExact { get; set; }
        string BtwCode { get; set; }
        Decimal BtwAmount { get; set; }
        string KostendragerCode { get; set; }
        bool StornoBooking { get; set; }
        IJournalEntryLineCollection JournalEntryLines { get; }
        bool IsBankMutation { get; }
        bool IsSalesMutation { get; }

    }
}

using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface ICashTransfer : IGeneralOperationsBooking
    {
        IJournalEntryLine MainTransferLine { get; }
        Money TransferAmount { get; }
        Money TransferFee { get; }
        bool AddTransferFee(Money transferFee, IGLLookupRecords lookups, string description);
    }
}

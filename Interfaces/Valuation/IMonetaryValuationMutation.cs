using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Valuations
{
    public interface IMonetaryValuationMutation : IValuationMutation
    {
        ICashPosition CashPosition { get; }
        Money Amount { get; }
        Money Deposit { get; }
        Money DepositToDate { get; }
        Money WithDrawal { get; }
        Money WithDrawalToDate { get; }

        void AddLine(IJournalEntryLine line);
        void AddNotRelevantLine(IJournalEntryLine notRelevantLine);
    }
}

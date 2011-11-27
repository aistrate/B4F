using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Valuations
{
    public interface IValuationCashMutation
    {
        long Key { get; set; }
        IValuationCashMutation PreviousCashMutation { get; }
        //IPosition Position { get; }
        IAccountTypeInternal Account { get; }
        IInstrument Instrument { get; }
        ValuationCashTypes ValuationCashType { get; }
        DateTime Date { get; }
        Money Amount { get; }
        Money AmountToDate { get; }
        Money BaseAmount { get; }
        Money BaseAmountToDate { get; }
        string GetUniqueCode { get; }
        bool IsValid { get; }

        void AddLine(IJournalEntryLine line);
        void AddNotRelevantLine(IJournalEntryLine notRelevantLine);
        bool ContainsLine(IJournalEntryLine line);
        bool Validate();
    }
}

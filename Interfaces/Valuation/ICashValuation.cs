using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Valuations
{
    public interface ICashValuation
    {
        string Key { get; }
        IAccountTypeInternal Account { get; }
        IInstrument Instrument { get; }
        ValuationCashTypes ValuationCashType { get; }
        DateTime Date { get; }
        Money Amount { get; }
        Money AmountToDate { get; }
        Money BaseAmountToDate { get; }
        decimal MarketRate { get; }
    }
}

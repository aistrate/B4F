using System;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.Valuations
{
    public interface IValuation
    {
        int Key { get; set; }
        IAccountTypeInternal Account { get; }
        IInstrument Instrument { get; }
        ICurrency CurrencyNominal { get; }
        DateTime Date { get; }
        IValuationMutation ValuationMutation { get; }
        InstrumentSize Size { get; }
        Price BookPrice { get; }
        Money BookValue { get; }
        Money BookChange { get; }
        decimal AvgOpenExRate { get; }
        Money RealisedAmount { get; }
        Money RealisedAmountToDate { get; }
        Money BaseRealisedAmountToDate { get; }
        Money UnRealisedAmountToDate { get; }
        Price CostPrice { get; }
        Money MarketValue { get; }
        Money BaseMarketValue { get; }
        Price MarketPrice { get; }
        decimal MarketRate { get; }
        Money Deposit { get; }
        Money WithDrawal { get; }
        string DisplayInstrumentsCategory { get; }
        //IAverageHolding AverageHolding { get; }
        IValuationCashMutation AccruedInterestCashMutation { get; }
        IAssetClass AssetClass { get; }
    }
}

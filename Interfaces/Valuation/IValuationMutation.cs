using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;

namespace B4F.TotalGiro.Valuations
{
    public interface IValuationMutation
    {
        long Key { get; set; }
        IValuationMutation PreviousMutation { get; }
        IValuationMutation ConvertedMutation { get; set; }
        IAccountTypeInternal Account { get; }
        IInstrument Instrument { get; }
        DateTime Date { get; }
        InstrumentSize Size { get; }
        Money BookValue { get; }
        Money BookValueIC { get; }
        Money BookChange { get; }
        Money BookChangeIC { get; }
        Money TransferInToday { get; }
        Money TransferOutToday { get; }
        Money RealisedCurrencyGain { get; }
        Money RealisedCurrencyGainToDate { get; }
        ICurrency InstrumentCurrency { get; }
        InstrumentSize SizeChange { get; }
        decimal AvgOpenExRate { get; }
        string DisplayInstrumentsCategory { get; }
        bool IsValid { get; }
        string GetUniqueCode { get; }
        bool IsSecurityValuationMutation { get; }
        IAssetClass AssetClass { get; }

        bool Validate();
    }
}

using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Valuations;

namespace B4F.TotalGiro.Accounts.Portfolios
{
    public interface ICommonPosition
    {
        IAccountTypeInternal Account { get; }
        InstrumentSize Size { get; }
        IInstrument Instrument { get; }
        Money CurrentValue { get; }
        Money CurrentBaseValue { get; }

        DateTime OpenDate { get; }
        DateTime CreationDate { get; }
        DateTime LastUpdated { get; }

        IAssetClass AssetClass { get; }
        IValuationMutation LastMutation { get; }
        IValuation LastValuation { get; }
    }
}

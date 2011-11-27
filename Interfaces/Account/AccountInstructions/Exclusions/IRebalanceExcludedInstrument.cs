using System;

namespace B4F.TotalGiro.Accounts.Instructions.Exclusions
{
    public interface IRebalanceExcludedInstrument : IRebalanceExclusion
    {
        B4F.TotalGiro.Instruments.ITradeableInstrument Instrument { get; set; }
    }
}

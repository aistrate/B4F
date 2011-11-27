using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Valuations
{
    public interface IAverageValuation
    {
        IInstrument Instrument { get; }
        Money AvgMarketValue { get; }
        Money AvgBaseMarketValue { get; }
        int Period { get; }
        decimal ManagementFeePercentage { get; }
    }
}

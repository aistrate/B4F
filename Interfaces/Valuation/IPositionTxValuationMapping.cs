using System;

namespace B4F.TotalGiro.Valuations.Mapping
{
    public interface IPositionTxValuationMapping
    {
        int Key { get; set; }
        IValuationMutation ValuationMutation { get; }
        bool IsRelevant { get; }
    }
}

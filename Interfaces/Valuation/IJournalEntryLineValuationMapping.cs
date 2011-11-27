using System;

namespace B4F.TotalGiro.Valuations.Mapping
{
    public interface IJournalEntryLineValuationMapping
    {
        int Key { get; set; }
        IValuationMutation ValuationMutation { get; }
        bool IsRelevant { get; }
    }
}

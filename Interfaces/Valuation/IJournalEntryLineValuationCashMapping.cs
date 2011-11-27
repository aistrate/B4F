using System;

namespace B4F.TotalGiro.Valuations.Mapping
{
    public interface IJournalEntryLineValuationCashMapping
    {
        int Key { get; set; }
        IValuationCashMutation ValuationCashMutation { get; }
        bool IsRelevant { get; }
    }
}

using System;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Valuations
{
    public interface ISecurityValuationMutationCashMutation
    {
        long Key { get; set; }
        IValuationCashMutation CashMutation { get; }
    }
}

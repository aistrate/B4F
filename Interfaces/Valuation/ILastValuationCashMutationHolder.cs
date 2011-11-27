using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Valuations
{
    public interface ILastValuationCashMutationHolder
    {
        int Key { get; set; }
        ValuationCashMutationKey CashMutKey { get; }
        IValuationCashMutation LastCashMutation { get; set; }
    }
}

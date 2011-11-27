using System;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Valuations
{
    public interface ILastValuationCashMutationCollection : IGenericDictionary<ValuationCashMutationKey, ILastValuationCashMutationHolder>
    {
        IAccountTypeCustomer Account { get; }
        void Add(ValuationCashMutationKey key, IValuationCashMutation lastCashMutation);
    }
}

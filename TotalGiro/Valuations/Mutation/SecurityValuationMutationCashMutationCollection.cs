using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Valuations
{
    public class SecurityValuationMutationCashMutationCollection : GenericCollection<IValuationCashMutation>, ISecurityValuationMutationCashMutationCollection
    {
        internal SecurityValuationMutationCashMutationCollection(IList bagOfCashMutations, ISecurityValuationMutation parent)
            : base(bagOfCashMutations)
        {
            this.parent = parent;
        }

        public Money GetTotalValue(ValuationCashTypes[] returnTypes)
        {
            Money total = null;
            foreach (IValuationCashMutation mut in this)
            {
                if (includeReturnType(returnTypes, mut.ValuationCashType))
                    total += mut.BaseAmount;
            }
            return total;
        }

        private bool includeReturnType(ValuationCashTypes[] returnTypes, ValuationCashTypes typeMut)
        {
            for (int i = 0; i < returnTypes.Length; i++)
            {
                if (returnTypes[0].Equals(typeMut))
                    return true;
            }
            return false;
        }

        private ISecurityValuationMutation parent;
    }
}

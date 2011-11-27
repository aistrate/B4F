using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Valuations
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Valuations.SecurityValuationMutationCashMutationCollection">SecurityValuationMutationCashMutationCollection</see> class
    /// </summary>
    public interface ISecurityValuationMutationCashMutationCollection : IGenericCollection<IValuationCashMutation>
    {
        Money GetTotalValue(ValuationCashTypes[] returnTypes);
    }
}

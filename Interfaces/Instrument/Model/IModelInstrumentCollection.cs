using System;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts.Instructions.Exclusions;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.ModelInstrumentCollection">ModelInstrumentCollection</see> class
    /// </summary>
    public interface IModelInstrumentCollection : IList<IModelInstrument>
	{
        IModelVersion Parent { get; }
        decimal TotalAllocation { get; }
        IModelInstrument Find(IInstrument instrument);
        ICashManagementFund GetCashFund();
        List<IInstrument> Instruments { get; }
        IModelInstrumentCollection StrippedCollection(IRebalanceExclusionCollection excludedComponents);
    }
}

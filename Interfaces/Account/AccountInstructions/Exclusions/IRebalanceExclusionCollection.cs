using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions.Exclusions
{
    public interface IRebalanceExclusionCollection : IList<IRebalanceExclusion>
    {
        IRebalanceInstruction Parent { get; set; }
        List<IInstrument> Instruments { get; }
        List<ITradeableInstrument> TradeableInstruments { get; }
        void AddExclusion(ITradeableInstrument instrument);
        void AddExclusion(IPortfolioModel model);
        bool RemoveExclusionAt(int index);
    }
}

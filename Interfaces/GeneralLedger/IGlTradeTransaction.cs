using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public enum GLDefaultLookups
    {
        CostOfStockExternalUnsettled = 1,
        CostOfStockAssetManagerUnsettled = 2,
        ServiceChargeExternalUnsettled = 3,
        CostOfStockAssetManagerSettled = 4,
        IncomeGeneral = 5,
        ServiceChargeInternal = 6,
        ServiceChargeClient = 7

    }



    public interface IGlTradeTransaction : IJournalEntry
    {
        string CounterParty { get; }
        IExternalSettlement MatchedSettlement { get; set; }
    }
}

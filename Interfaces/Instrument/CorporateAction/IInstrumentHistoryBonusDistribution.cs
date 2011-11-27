using System;
using B4F.TotalGiro.Accounts;
namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public interface ICorporateActionBonusDistribution : ICorporateActionHistory
    {
        InstrumentSize TotalSizeDistributed { get; set; }
        IAccountTypeInternal CounterAccount { get; set; }
        //IBonusDistributionCollection BonusDistributions { get; }
        InstrumentSize TotalHoldingsAtDate { get; set; }
        InstrumentSize SizeToDistribute { get; set; }
        DateTime DistributionDate { get; set; }
        
    }
}

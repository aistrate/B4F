using System;

namespace B4F.TotalGiro.Instruments
{
    public enum CashManagementFundOptions
    {
        Excluded = 0,
        Included = 1,
        CashFundOnly = 2,
    }
    
    public interface IModelDetail
    {
        int Key { get; set; }
        string Description { get; }
        short DaysDurationRebalance { get; }
        short DaysFreeUpCash { get; }
        short DaysKeepFreeCash { get; }
        bool IncludeCashManagementFund { get; }
        CashManagementFundOptions CashManagementFundOption { get; }
    }
}

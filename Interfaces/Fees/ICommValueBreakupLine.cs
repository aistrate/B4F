using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Fees.Calculations
{
    public enum CommValueBreakupType
    {
        Commission = 0,
        IntroductionFee
    }

    public interface ICommValueBreakupLine
    {
        int Key { get; }
        Money CalcValue { get; set; }
        ICurrency CommCurrency { get; }
        string CalcInfo { get; }
        CommValueBreakupType CalcType { get; }
    }
}

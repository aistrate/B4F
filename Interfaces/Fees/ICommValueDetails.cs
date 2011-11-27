using System;
using System.Collections.Generic;
using System.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Fees.Calculations
{

    public interface ICommValueDetails
    {
        int Key { get; }
        ICurrency CommCurrency { get; }
        string CalcInfo { get;  }
        Money CalcValue { get; }
        ICommValueBreakupLineCollection CommLines { get; }
    }
}

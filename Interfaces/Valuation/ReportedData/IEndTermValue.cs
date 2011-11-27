using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Communicator.BelastingDienst;

namespace B4F.TotalGiro.Valuations.ReportedData
{
    public enum EndTermType
    {

        FirstQtr = 0,
        SecondQtr,
        ThirdQtr,
        FourthQtr,
        FullYear
    }

    public interface IEndTermValue
    {
        int Key { get; set; }
        DateTime EndTermDate { get;  }        
        EndTermType TermType { get;  }
        IAccountTypeInternal Account { get; set; }
        IDividWepRecord DividWepRecord { get; set; }
        IPeriodicReporting ReportingPeriod { get; set; }
        Money CashValue { get; set; }
        Money ClosingValue { get; set; }
        Money CultureFundValue { get; set; }
        Money ExternalDividend { get; set; }
        Money ExternalDividendTax { get; set; }
        Money FundValue { get; set; }
        Money GreenFundValue { get; set; }
        Money InternalDividend { get; set; }
        Money InternalDividendTax { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    public interface IFeeCalcVersion
    {
        int Key { get; set; }
        IFeeCalc Parent { get; set; }
        FeeCalcTypes FeeCalcType { get; }
        int VersionNumber { get; set;  }
	    int StartPeriod { get; }
        int EndPeriod { get; set; }
        Money FixedSetup { get; }
        string CreatedBy { get; }
        DateTime CreationDate { get; }
        DateTime LastUpdated { get; }
        bool IsFeeRelevant { get; }
        string DisplayString { get; }
    }
}

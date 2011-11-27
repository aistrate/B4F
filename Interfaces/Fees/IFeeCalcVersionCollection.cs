using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    public interface IFeeCalcVersionCollection : IGenericCollection<IFeeCalcVersion>
    {
        IFeeCalc Parent { get; }
        IFeeCalcVersion LatestVersion();
        IFeeCalcVersion GetItemByPeriod(int period);
    }
}

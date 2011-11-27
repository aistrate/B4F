using System;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees.FeeCalculations;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public interface IManagementPeriodUnitFeeCollection : IList<IManagementPeriodUnitFee>
    {
        IManagementPeriodUnit Parent { get; }
        IManagementPeriodUnitFee AddFeeItem(FeeType feeType, Money amount, IFeeCalcVersion calcSource);
        IManagementPeriodUnitFee GetItemByType(FeeTypes feeType);
        Money TotalAmount { get; }
    }
}

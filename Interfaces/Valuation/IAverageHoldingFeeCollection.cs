using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Valuations.AverageHoldings
{
    public interface IAverageHoldingFeeCollection : IGenericCollection<IAverageHoldingFee>
    {
        IAverageHoldingFee AddFeeItem(FeeType feeType, Money calculatedAmount, IManagementPeriodUnit unit, IFeeCalcVersion calcSource, decimal feePercentageUsed);
        IAverageHoldingFee GetItemByType(FeeTypes feeType);
        IAverageHoldingFeeCollection GetItemsByType(FeeTypes feeType);
        Money TotalAmount { get; }
    }
}

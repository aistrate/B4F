using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Notas
{
    public interface INotaFees : INotaGeneralOperationsBookingTaxeable
    {
        string Description { get; }
        Money ManagementFeeAmount { get; }
        Money ValueIncludingTax { get; }
        DateTime TxStartDate { get; }
        DateTime TxEndDate { get; }
        IGeneralOperationsComponentCollection FeeDetails { get; }
        IAverageHolding[] AverageHoldings { get; }
        DateTime PeriodStartDate { get; }
        DateTime PeriodEndDate { get; }
    }
}

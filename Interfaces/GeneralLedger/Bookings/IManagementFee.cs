using System;
using System.Collections.Generic;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface IManagementFee : IGeneralOperationsBookingTaxeable
    {
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        DateTime NotaStartDate { get; }
        DateTime NotaEndDate { get; }
        Money ManagementFeeAmount { get; }
        IList<IManagementPeriodUnit> Units { get; }

        void BookLines();
    }
}

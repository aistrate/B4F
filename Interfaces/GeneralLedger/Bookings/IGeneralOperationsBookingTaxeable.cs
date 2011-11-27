using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface IGeneralOperationsBookingTaxeable : IGeneralOperationsBooking
    {
        IMemorialBooking MemorialBooking { get; }
        decimal TaxPercentage { get; set; }
        Money NettAmount { get; }
        Money TaxAmount { get; }
    }
}

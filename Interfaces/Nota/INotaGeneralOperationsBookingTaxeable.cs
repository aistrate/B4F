using System;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Notas
{
    public interface INotaGeneralOperationsBookingTaxeable : INotaGeneralOperationsBooking
    {
        Money Tax { get; }
        decimal TaxPercentage { get; }
    }
}

using System;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Notas
{
    public interface INotaGeneralOperationsBooking : INota
    {
        IGeneralOperationsBooking OriginalBooking { get; }
        INota StornoedBookNota { get; }
        IGeneralOperationsBooking UnderlyingBooking { get; }
    }
}

using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface IGeneralOperationsComponentCollection : IList<IGeneralOperationsComponent>
    {
        IGeneralOperationsBooking Parent { get; set; }
        Money TotalAmount { get; }
        Money TotalBaseAmount { get; }
        Money ReturnComponentValue(BookingComponentTypes type);
        Money ReturnComponentValueInBaseCurrency(BookingComponentTypes type);
        Money ReturnComponentValue(BookingComponentTypes[] types);
        Money ReturnComponentValueInBaseCurrency(BookingComponentTypes[] types);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface IGeneralOperationsComponent: IBookingComponentParent
    {
        IGeneralOperationsBooking ParentBooking { get; set; }
        IGeneralOperationsComponent Clone();
        IGeneralOperationsComponent CloneAndStorno(IGeneralOperationsBooking parentBooking);
    }
}

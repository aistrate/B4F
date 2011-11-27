using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class ForeignExchangeComponent : GeneralOperationsComponent, IForeignExchangeComponent
    {
        protected ForeignExchangeComponent() { }
        public ForeignExchangeComponent(IGeneralOperationsBooking parentBooking, BookingComponentTypes bookingComponentType)
            : this(parentBooking, bookingComponentType, DateTime.Now) { }

        public ForeignExchangeComponent(IGeneralOperationsBooking parentBooking, BookingComponentTypes bookingComponentType,
            DateTime creationDate)
            : base(parentBooking, bookingComponentType, creationDate) { }

        public override IGeneralOperationsComponent Clone()
        {
            throw new NotImplementedException();
        }

        public override IGeneralOperationsComponent CloneAndStorno(IGeneralOperationsBooking parentBooking)
        {
            throw new NotImplementedException();
        }

        public override BookingComponentParentTypes BookingComponentParentType { get { return BookingComponentParentTypes.ForexTransaction; } }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public abstract class GeneralOperationsComponent : BookingComponentParent, IGeneralOperationsComponent
    {
        protected GeneralOperationsComponent() { }
        public GeneralOperationsComponent(IGeneralOperationsBooking parentBooking, BookingComponentTypes bookingComponentType)
            : this(parentBooking, bookingComponentType, DateTime.Now) { }

        public GeneralOperationsComponent(IGeneralOperationsBooking parentBooking, BookingComponentTypes bookingComponentType,
        DateTime creationDate)
            : base(bookingComponentType, creationDate)
        {
            this.ParentBooking = parentBooking;
        }
        public IGeneralOperationsBooking ParentBooking { get; set; }
        public override IJournalEntry BookingJournalEntry { get { return this.ParentBooking.GeneralOpsJournalEntry; } }

        public abstract IGeneralOperationsComponent Clone();
        public abstract IGeneralOperationsComponent CloneAndStorno(IGeneralOperationsBooking parentBooking);
    }
}

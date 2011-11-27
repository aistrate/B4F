using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class CashDividendComponent : GeneralOperationsComponent, ICashDividendComponent
    {
        protected CashDividendComponent() { }
        public CashDividendComponent(IGeneralOperationsBooking parentBooking, BookingComponentTypes bookingComponentType)
            : this(parentBooking, bookingComponentType, DateTime.Now) { }

        public CashDividendComponent(IGeneralOperationsBooking parentBooking, BookingComponentTypes bookingComponentType,
            DateTime creationDate)
            : base(parentBooking, bookingComponentType, creationDate) { }

        public override BookingComponentParentTypes BookingComponentParentType { get { return BookingComponentParentTypes.CashDividend; } }

        public override IGeneralOperationsComponent Clone()
        {
            return clone(false, this.ParentBooking);
        }

        public override IGeneralOperationsComponent CloneAndStorno(IGeneralOperationsBooking parentBooking)
        {
            return clone(true, parentBooking);
        }

        protected IGeneralOperationsComponent clone(bool doStorno, IGeneralOperationsBooking parentBooking)
        {
            ICashDividendComponent clone = new CashDividendComponent(parentBooking, this.BookingComponentType);
            foreach (IJournalEntryLine line in this.Component.JournalLines)
            {
                IJournalEntryLine cloneLine = line.Clone();
                if (doStorno)
                {
                    cloneLine.Balance = cloneLine.Balance.Negate();
                    line.StornoedLine = cloneLine;
                }
                clone.Component.JournalLines.AddJournalEntryLine(cloneLine);
            }
            return clone;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class CashTransferComponent : GeneralOperationsComponent, ICashTransferComponent
    {
        protected CashTransferComponent() { }
        public CashTransferComponent(IGeneralOperationsBooking parentBooking, BookingComponentTypes bookingComponentType)
            : this(parentBooking, bookingComponentType, DateTime.Now) { }

        public CashTransferComponent(IGeneralOperationsBooking parentBooking, BookingComponentTypes bookingComponentType,
            DateTime creationDate)
            : base(parentBooking, bookingComponentType, creationDate) { }

        public override BookingComponentParentTypes BookingComponentParentType { get { return BookingComponentParentTypes.CashTransfer; } }

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
            ICashTransferComponent clone = new CashTransferComponent(parentBooking, this.BookingComponentType);
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

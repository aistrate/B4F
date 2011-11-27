using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Fees;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class ManagementFeeComponent : GeneralOperationsComponent, IManagementFeeComponent
    {
        protected ManagementFeeComponent() { }
        public ManagementFeeComponent(IGeneralOperationsBooking parentBooking, BookingComponentTypes bookingComponentType, 
            int period, FeeType mgtFeeType)
            : this(parentBooking, bookingComponentType, period, mgtFeeType, DateTime.Now) { }

        public ManagementFeeComponent(IGeneralOperationsBooking parentBooking, BookingComponentTypes bookingComponentType,
            int period, FeeType mgtFeeType, DateTime creationDate)
            : base(parentBooking, bookingComponentType, creationDate)
        {
            this.Period = period;
            this.MgtFeeType = mgtFeeType;
        }

        public override BookingComponentParentTypes BookingComponentParentType { get { return BookingComponentParentTypes.ManagementFee; } }
        public virtual int Period { get; set; }
        public virtual FeeType MgtFeeType { get; set; }

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
            IManagementFeeComponent clone = new ManagementFeeComponent(parentBooking, this.BookingComponentType, this.Period, this.MgtFeeType);
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

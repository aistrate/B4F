using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings.DisplayView
{
    public class BookingDisplayView
    {
        public int Key { get { return this.Parent.Key; } }
        public IAccountTypeInternal Account { get; set; }
        public string DisplayNumberWithName { get { return this.Account.DisplayNumberWithName; } }
        public IGeneralOperationsBooking Parent { get; set; }
        public string BookingType { get { return this.Parent.BookingType.ToString(); }}
        public DateTime BookingDate { get { return this.Parent.BookDate; } }
    }
}

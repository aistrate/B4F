using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public class GLLookupRecord : IGLLookupRecord 
    {
        public GLLookupRecord() { }

        public int Key { get; set; }
        public BookingComponentParentTypes BookingComponentParentType { get; set; }
        public BookingComponentTypes BookingComponentType { get; set; }
        public ICurrency Currency { get; set; }
        public IGLAccount MainAccount { get; set; }
        public IGLAccount CounterAccount { get; set; }
        public bool IsUnSettled { get; set; }
        public bool IsExternalExecution { get; set; }
        public bool IsInternalExecution { get; set; }

       

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public interface IGLLookupRecord
    {
        int Key { get; set; }
        BookingComponentParentTypes BookingComponentParentType { get; set; }
        BookingComponentTypes BookingComponentType { get; set; }
        ICurrency Currency { get; set; }
        IGLAccount MainAccount { get; set; }
        IGLAccount CounterAccount { get; set; }
        bool IsUnSettled { get; set; }
        bool IsExternalExecution { get; set; }
        bool IsInternalExecution { get; set; }
    }
}

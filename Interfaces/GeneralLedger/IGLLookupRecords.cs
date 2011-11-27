using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public interface IGLLookupRecords
    {
        IGLLookupRecord GetGLLookupRecord(ICurrency currency,
                bool isExternalExecution,
                bool isInternalExecution,
                bool isUnsettled,
                BookingComponentTypes bookingComponentType);
        //IGLLookupRecord GetGLLookupRecord(bool isSettled, BookingComponentTypes bookingComponentType);
    }
}

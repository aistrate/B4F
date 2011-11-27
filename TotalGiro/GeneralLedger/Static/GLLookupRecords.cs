using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;


namespace B4F.TotalGiro.GeneralLedger.Static
{
    public class GLLookupRecords : IGLLookupRecords
    {

        public GLLookupRecords(IList<IGLLookupRecord> records)
        {
            this.Records = records;
        }

        public IGLLookupRecord GetGLLookupRecord(
                ICurrency currency,
                bool isExternalExecution,
                bool isInternalExecution,
                bool isUnsettled,
                BookingComponentTypes bookingComponentType)
        {

            var returnvalue = from c in Records
                              where ((c.IsExternalExecution == isExternalExecution)
                                && (c.IsInternalExecution == isInternalExecution)
                                && (c.IsUnSettled == isUnsettled)
                                && (c.BookingComponentType == bookingComponentType)
                                && (c.Currency.Equals(currency)))
                              select c;

            if (returnvalue.Count() == 0)
                throw new ApplicationException("No Lookup record is matched.");

            return returnvalue.ElementAt(0);
        }

        //public IGLLookupRecord GetGLLookupRecord(
        //        bool isSettled,
        //        BookingComponentTypes bookingComponentType)
        //{

        //    var returnvalue = from c in Records
        //                      where ((c.IsSettled == isSettled)
        //                        && (c.BookingComponentType == bookingComponentType)
        //                        && ((int)c.AcctType == 0)
        //                        && (c.IsStichting))
        //                      select c;

        //    if (returnvalue.Count() == 0)
        //        throw new ApplicationException("No Lookup record is matched.");

        //    return returnvalue.ElementAt(0);
        //}

        public IList<IGLLookupRecord> Records { get; set; }

    }
}

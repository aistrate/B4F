using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using System;
using B4F.TotalGiro.Instruments;
using NHibernate.Criterion;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public class GlLookupRecordMapper
    {
        

        public static IGLLookupRecords GetGLLookupRecords(IDalSession session)
        {

            IList<IGLLookupRecord> gLLookupRecords = session.GetTypedList<IGLLookupRecord>();
            return new GLLookupRecords(gLLookupRecords);

        }

        public static IGLLookupRecords GetGLLookupRecords(IDalSession session, BookingComponentParentTypes bookingComponentParentType)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("BookingComponentParentType", bookingComponentParentType));
            IList<IGLLookupRecord> gLLookupRecords = session.GetTypedList<IGLLookupRecord>();
            return new GLLookupRecords(gLLookupRecords);
        }
     

        public static IGLLookupRecord GetGLLookupRecord(
        ICurrency currency,
        bool isExternalExecution,
        bool isInternalExecution,
        bool isUnsettled,
        BookingComponentTypes bookingComponentType,
        IList<GLLookupRecord> lookups)
        {

            var returnvalue = from c in lookups
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


    }
}

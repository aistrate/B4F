using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    #region enums

    public enum GeneralOperationsBookingReturnClass
    {
        All = 0,
        ManagementFee = 1,
        CashDividend = 2,
        CashTransfer = 4
    }

    #endregion

    public static class GeneralOperationsBookingMapper
    {
        public static IGeneralOperationsBooking GetBooking(IDalSession session, int bookingId)
        {
            return GetBookings(session, new int[]{ bookingId }).FirstOrDefault();
        }

        public static IList<IGeneralOperationsBooking> GetBookings(IDalSession session, int[] bookingIds)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("Key", bookingIds));
            return session.GetTypedList<GeneralOperationsBooking, IGeneralOperationsBooking>(expressions);
        }

        public static IList<IGeneralOperationsBooking> GetBookings(IDalSession session, GeneralOperationsBookingReturnClass bookingType, int accountId, DateTime beginDate, DateTime endDate, bool includeStornos)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("accountId", accountId);
            parameters.Add("beginDate", beginDate);
            parameters.Add("endDate", endDate);
            parameters.Add("bookTypeId", (int)bookingType);
            if (!includeStornos)
                parameters.Add("hideStornos", 1);

            return session.GetTypedListByNamedQuery<IGeneralOperationsBooking>(
                "B4F.TotalGiro.GeneralLedger.Journal.Bookings.GetGeneralOperationsBookings",
                parameters);
        }

        public static int[] GetNotarizableBookingIds(
            IDalSession session, GeneralOperationsBookingReturnClass bookingType,
            int managementCompanyId, int accountId)
        {
            Hashtable parameters = new Hashtable();

            if (bookingType != GeneralOperationsBookingReturnClass.All)
                parameters.Add("bookingType", (int)bookingType);

            if (managementCompanyId != 0)
                parameters.Add("managementCompanyId", managementCompanyId);

            if (accountId != 0)
                parameters.Add("accountId", accountId);

            IList<int> bookingIds = session.GetTypedListByNamedQuery<int>(
                "B4F.TotalGiro.GeneralLedger.Journal.Bookings.GetNotarizableBookingIds",
                parameters);
            return bookingIds.ToArray();
        }

        public static bool Update(IDalSession session, IGeneralOperationsBooking obj)
        {
            return session.InsertOrUpdate(obj);
        }
    }
}

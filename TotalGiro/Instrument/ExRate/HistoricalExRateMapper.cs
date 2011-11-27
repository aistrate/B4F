using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using NHibernate;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments.ExRates
{
    /// <summary>
    /// This class is used to instantiate Historical Exchange Rates objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class HistoricalExRateMapper
    {
        public static IHistoricalExRate GetHistoricalExRate(IDalSession session, int historicalExRateID)
        {
            return (IHistoricalExRate)session.GetObjectInstance(typeof(HistoricalExRate), historicalExRateID);
        }

        /// <summary>
        /// Gets historical exchange rate by currency and time
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="currency">Currency</param>
        /// <param name="date">Date</param>
        /// <returns>Historical exchange object</returns>
        public static IHistoricalExRate GetHistoricalExRate(IDalSession session, ICurrency currency, DateTime date)
        {
            IList list;
            IHistoricalExRate rate = null;

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Currency.Key", currency.Key));
            expressions.Add(Expression.Eq("RateDate", date));
            list = session.GetList(typeof(HistoricalExRate), expressions);
            if (list != null && list.Count > 0)
            {
                rate = (IHistoricalExRate)list[0];
            }
            return rate;
        }

        /// <summary>
        /// Gets historical exchange rate by currency and time
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="currency">Currency</param>
        /// <param name="date">Date</param>
        /// <returns>Historical exchange object</returns>
        public static IHistoricalExRate GetNearestHistoricalExRate(IDalSession session, ICurrency currency, DateTime date)
        {
            IList list;
            IHistoricalExRate rate = null;

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Currency.Key", currency.Key));
            expressions.Add(Expression.Le("RateDate", date));
            List<Order> orderings = new List<Order>();
            orderings.Add(Order.Desc("RateDate"));
            list = session.GetList(typeof(HistoricalExRate), expressions, orderings, 1, null);
            if (list != null && list.Count > 0)
            {
                rate = (IHistoricalExRate)list[0];
            }
            return rate;
        }

        public static IHistoricalExRate GetLastValidHistoricalExRate(IDalSession session, ICurrency currency, DateTime date)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Currency.Key", currency.Key));
            expressions.Add(Expression.Le("RateDate", date));

            List<Order> orderings = new List<Order>();
            orderings.Add(Order.Desc("RateDate"));

            IList exRates = session.GetList(typeof(HistoricalExRate), expressions, orderings, 1, null);
            return (exRates != null && exRates.Count > 0 ? (IHistoricalExRate)exRates[0] : null);
        }

        /// <summary>
        /// Get collection of historical exchange rates by currency
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="currency">Currency</param>
        /// <returns>collection of historical exchange rates</returns>
        public static IList<IHistoricalExRate> GetHistoricalExRates(IDalSession session, ICurrency currency)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Currency.Key", currency.Key));
            return session.GetTypedList<HistoricalExRate,IHistoricalExRate>(expressions);
        }

        /// <summary>
        /// Get collection of historical exchange rates by currency and a period of time
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="currency">Currency</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Collection of historical exchange rates</returns>
        public static IList<IHistoricalExRate> GetHistoricalExRates(IDalSession session, ICurrency currency, DateTime startDate, DateTime endDate)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Currency.Key", currency.Key));
            expressions.Add(Expression.Between("RateDate",
                Util.IsNotNullDate(startDate) ? startDate : DateTime.MinValue,
                Util.IsNotNullDate(endDate) ? endDate : DateTime.MaxValue));
            return session.GetTypedList<HistoricalExRate,IHistoricalExRate>(expressions);
        }

        public static IList<IHistoricalExRate> GetHistoricalExRates(IDalSession session, int currencyId, DateTime[] dates)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("RateDate", dates));
            if (currencyId != 0)
                expressions.Add(Expression.Eq("Currency.Key", currencyId));
            return session.GetTypedList<HistoricalExRate, IHistoricalExRate>(expressions);
        }

        /// <summary>
        /// Get collection of historical exchange rates of known currency for one date
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="rateDate">Rate date</param>
        /// <returns>Collection of historical exchange rates</returns>
        public static IList<IHistoricalExRate> GetHistoricalExRates(IDalSession session, DateTime rateDate)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("RateDate", rateDate));
            return session.GetTypedList<HistoricalExRate,IHistoricalExRate>(expressions);
        }

        /// <summary>
        /// Get collection of historical exchange rates by currency
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="currencies">The relevant Currencies</param>
        /// <param name="rateDate">Rate date</param>
        /// <returns>collection of historical exchange rates</returns>
        public static IList<IHistoricalExRate> GetHistoricalExRates(IDalSession session, ICurrency[] currencies, DateTime rateDate)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("Currency", currencies));
            expressions.Add(Expression.Eq("RateDate", rateDate));
            return session.GetTypedList<HistoricalExRate,IHistoricalExRate>(expressions);
        }

        /// <summary>
        /// Get latest historical exchange rate
        /// </summary>
        /// <param name="DataSession">Data access object</param>
        /// <returns>Date</returns>
        public static DateTime GetMaxHistoricalExRateDate(IDalSession DataSession)
        {
            string Query = @"select top 1 {HistoricalExRates.*} from dbo.HistoricalExRates where ratedate = (Select max(RateDate) from dbo.HistoricalExRates)";
            IList returnValues = DataSession.GetListbySQLQuery(Query, "HistoricalExRates", typeof(HistoricalExRate));
            if (returnValues.Count > 0)
                return ((HistoricalExRate)returnValues[0]).RateDate;
            else
                return DateTime.MinValue;
        }

        /// <summary>
        /// Check whether the rate is within the range
        /// </summary>
        /// <param name="session"></param>
        /// <param name="checkRate"></param>
        /// <returns></returns>
        public static bool CheckHistoricalExRate(IDalSession session, IHistoricalExRate checkRate, out string errMessage)
        {
            bool success = true;
            errMessage = "";
            if (checkRate != null)
            {
                IList<IHistoricalExRate> previousRates = GetHistoricalExRates(session,
                    checkRate.Currency, checkRate.RateDate.AddDays(-5),
                    checkRate.RateDate.AddDays(-1));
                if (previousRates != null && previousRates.Count > 0)
                {
                    decimal prevRate = previousRates.OrderByDescending(x => x.RateDate).FirstOrDefault().Rate;
                    if (prevRate != 0M)
                    {
                        decimal diff = Math.Abs((prevRate - checkRate.Rate) / prevRate);
                        if (diff > 0.05M)
                        {
                            errMessage = string.Format("The new Exchange Rate is {0}% different from the previous Exchange Rate.", (diff * 100).ToString("0.0"));
                            success = false;
                        }
                    }
                }
            }
            return success;
        }


        public static bool Update(IDalSession session, IHistoricalExRate historicalExRate)
        {
            session.InsertOrUpdate(historicalExRate);
            return true;
        }
    }
}

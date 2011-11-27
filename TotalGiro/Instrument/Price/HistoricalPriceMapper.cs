using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments.Prices
{
    /// This class is used to instantiate HistoricalPrice objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    public class HistoricalPriceMapper
    {
        public static IHistoricalPrice GetHistoricalPrice(IDalSession session, int historicalPriceID)
        {
            return (IHistoricalPrice)session.GetObjectInstance(typeof(HistoricalPrice), historicalPriceID);
        }

        public static IHistoricalPrice GetLastValidHistoricalPrice(IDalSession session, IInstrument instrument, DateTime date)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Price.Instrument.Key", instrument.Key));
            expressions.Add(Expression.Le("Date", date));
            
            List<Order> orderings = new List<Order>();
            orderings.Add(Order.Desc("Date"));
            
            IList prices = session.GetList(typeof(HistoricalPrice), expressions, orderings, 1, null);
            return (prices != null && prices.Count > 0 ? (IHistoricalPrice)prices[0] : null);
        }

        /// <summary>
        /// Get all historical price by instrument
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="instrument">Instrument object</param>
        /// <returns>Collection of historical prices</returns>
        public static IList<IHistoricalPrice> GetHistoricalPrices(IDalSession session, IInstrument instrument)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Instrument.Key", instrument.Key));
            return session.GetTypedList<HistoricalPrice, IHistoricalPrice>(expressions);
        }

        public static IList<IHistoricalPrice> GetHistoricalPrices(IDalSession session, DateTime date)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Date", date));
            return session.GetTypedList<HistoricalPrice, IHistoricalPrice>(expressions);
        }

        public static IList<IHistoricalPrice> GetHistoricalPrices(IDalSession session, DateTime startDate, DateTime endDate)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Between("Date", startDate, endDate));
            return session.GetTypedList<HistoricalPrice, IHistoricalPrice>(expressions);
        }

        public static IList<IHistoricalPrice> GetHistoricalPrices(IDalSession session, DateTime[] dates)
        {
            return GetHistoricalPrices(session, 0, dates);
        }

        public static IList<IHistoricalPrice> GetHistoricalPrices(IDalSession session, int instrumentId, DateTime[] dates)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("Date", dates));
            if (instrumentId != 0)
                expressions.Add(Expression.Eq("Price.Instrument.Key", instrumentId));
            return session.GetTypedList<HistoricalPrice, IHistoricalPrice>(expressions);
        }

        public static IList<IHistoricalPrice> GetHistoricalPrices(IDalSession session, IAccount account, DateTime[] dates)
        {
            Hashtable parameters = new Hashtable(1);
            Hashtable parameterLists = new Hashtable(1);
            parameters.Add("account", account);
            parameterLists.Add("dates", dates);
            return session.GetTypedListByNamedQuery<IHistoricalPrice>(
                "B4F.TotalGiro.Instruments.Prices.GetHistoricalPrices",
                parameters, parameterLists);
        }

        /// <summary>
        /// Get historical price of instrument at point in time
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="instrument">Instrument object</param>
        /// <param name="date">Date</param>
        /// <returns>Collection of historical prices</returns>
        public static IList<IHistoricalPrice> GetHistoricalPrices(IDalSession session, IInstrumentsWithPrices instrument, DateTime date)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Price.Instrument.Key", instrument.Key));
            expressions.Add(Expression.Eq("Date", date));
            return session.GetTypedList<IHistoricalPrice, IHistoricalPrice>(expressions);
        }

        /// <summary>
        /// Get historical price of instrument for a certain period
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="instrumentID">Instrument unique identifier</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <returns>Collection of historical prices</returns>
        public static IList<IHistoricalPrice> GetHistoricalPrices(IDalSession session, int instrumentID, DateTime startDate, DateTime endDate)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Price.Instrument.Key", instrumentID));
            expressions.Add(Expression.Between("Date", 
                Util.IsNotNullDate(startDate) ? startDate : DateTime.MinValue, 
                Util.IsNotNullDate(endDate) ? endDate : DateTime.MaxValue));
            return session.GetTypedList<HistoricalPrice, IHistoricalPrice>(expressions);
        }

        /// <summary>
        /// Check whether the price is within the range
        /// </summary>
        /// <param name="session"></param>
        /// <param name="checkPrice"></param>
        /// <returns></returns>
        public static bool CheckHistoricalPrice(IDalSession session, IHistoricalPrice checkPrice, out string errMessage)
        {
            bool success = true;
            errMessage = "";
            if (checkPrice != null)
            {
                IList<IHistoricalPrice> previousPrices = GetHistoricalPrices(session,
                    checkPrice.Instrument.Key, checkPrice.Date.AddDays(-5),
                    checkPrice.Date.AddDays(-1));
                if (previousPrices != null && previousPrices.Count > 0)
                {
                    Price prevPrice = previousPrices.OrderByDescending(x => x.Date).FirstOrDefault().Price;
                    if (prevPrice == null) prevPrice = checkPrice.Instrument.Get(e => e.CurrentPrice).Get(e => e.Price);

                    if (prevPrice != null)
                    {
                        decimal diff = Math.Abs((prevPrice.Quantity - checkPrice.Price.Quantity) / prevPrice.Quantity);
                        if (diff > 0.05M)
                        {
                            errMessage = string.Format("The new Price is {0}% different from the previous price.", (diff * 100).ToString("0.0"));
                            success = false;
                        }
                    }
                }
            }
            return success;
        }

        public static bool Update(IDalSession session, IHistoricalPrice historicalPrice)
        {
            session.InsertOrUpdate(historicalPrice);
            return true;
        }
    }
}

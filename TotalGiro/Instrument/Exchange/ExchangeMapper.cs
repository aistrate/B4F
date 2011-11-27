using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate;
using NHibernate.Criterion;


namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// This class is used to instantiate Exchange objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
	public class ExchangeMapper
	{
        /// <summary>
        /// Get exchange by identifier
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="exchangeID">Unique identifier</param>
        /// <returns>Exchange object</returns>
		public static IExchange GetExchange(IDalSession session, int exchangeID)
		{
            IExchange exchange = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", exchangeID));
            IList list = session.GetList(typeof(Exchange), expressions);
            if (list != null && list.Count == 1)
            {
                exchange = (IExchange)list[0];
            }
            return exchange;
        }

        /// <summary>
        /// Get exchange by its name
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="exchangeName">The name of the exchange</param>
        /// <returns>Exchange object</returns>
        public static IExchange GetExchangeByName(IDalSession session, string exchangeName)
        {
            IExchange exchange = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("ExchangeName", exchangeName));
            IList list = session.GetList(typeof(Exchange), expressions);
            if (list != null && list.Count == 1)
            {
                exchange = (IExchange)list[0];
            }
            return exchange;
        }

        /// <summary>
        /// Get all exchanges in system
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <returns>collection of exchanges</returns>
		public static IList<IExchange> GetExchanges(IDalSession session)
		{
			return session.GetTypedList<Exchange, IExchange>();
		}
	}
}

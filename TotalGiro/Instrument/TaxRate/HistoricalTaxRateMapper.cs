using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.TaxRates
{
    public static class HistoricalTaxRateMapper
    {
        /// <summary>
        /// Gets historical tax rate by date
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="date">Date</param>
        /// <returns>Historical exchange object</returns>
        public static IHistoricalTaxRate GetHistoricalTaxRate(IDalSession session, DateTime date)
        {
            List<HistoricalTaxRate> list;
            IHistoricalTaxRate rate = null;

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Ge("StartDate", date));
            expressions.Add(Expression.Or(Expression.IsNull("EndDate"), Expression.Le("EndDate", date)));
            list = session.GetTypedList<HistoricalTaxRate>(expressions);
            if (list != null && list.Count > 0)
            {
                rate = list[0];
            }
            return rate;
        }

        /// <summary>
        /// Gets historical tax rate by date
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <returns>Historical exchange object</returns>
        public static IList<IHistoricalTaxRate> GetHistoricalTaxRates(IDalSession session)
        {
            List<HistoricalTaxRate> list = session.GetTypedList<HistoricalTaxRate>();
            return list.Select(a => (IHistoricalTaxRate)a).ToList();
        }

    }
}

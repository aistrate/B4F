using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Collections;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments.History
{
    /// <summary>
    /// This class is used to instantiate InstrumentHistory objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public abstract class InstrumentHistoryMapper
    {

        /// <summary>
        /// Get all InstrumentHistory objects
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="oldInstrument">The instrument that has been changed</param>
        /// <returns>Collection of InstrumentHistory objects</returns>
        public static IList GetInstrumentHistoryList(IDalSession session, IInstrument instrument)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Instrument.Key", instrument.Key));
            return session.GetList(typeof(IInstrumentHistory), expressions);
        }
        /// <summary>
        /// Get all InstrumentHistory objects
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="oldInstrument">The instrument that has been changed</param>
        /// <returns>Collection of InstrumentHistory objects</returns>
        public static IList GetInstrumentHistoryList(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            return session.GetList(typeof(IInstrumentHistory), expressions);
        }

        public static IList<IInstrumentsHistoryConversion> GetInstrumentConversions(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            return session.GetTypedList<InstrumentsHistoryConversion, IInstrumentsHistoryConversion>(expressions);
        }

        public static IList<IInstrumentsHistoryConversion> GetInstrumentConversions(
            IDalSession session, string isin, string instrumentName, 
            SecCategories secCategoryId, int currencyNominalId)
        {
            Hashtable parameters = new Hashtable();

            if (!string.IsNullOrEmpty(isin))
                parameters.Add("isin", Util.PrepareNamedParameterWithWildcard(isin, MatchModes.Anywhere));
            if (!string.IsNullOrEmpty(instrumentName))
                parameters.Add("instrumentName", Util.PrepareNamedParameterWithWildcard(instrumentName, MatchModes.Anywhere));
            if (secCategoryId != SecCategories.Undefined)
                parameters.Add("secCategoryId", secCategoryId);
            if (currencyNominalId > 0)
                parameters.Add("currencyNominalId", currencyNominalId);

            return session.GetTypedListByNamedQuery<IInstrumentsHistoryConversion>(
                "B4F.TotalGiro.Instruments.History.GetInstrumentConversions",
                parameters);
        }

        /// <summary>
        /// Get InstrumentHistory object by ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="oldInstrument">The instrument that has been changed</param>
        /// <returns>InstrumentHistory object</returns>
        public static IInstrumentHistory GetInstrumentHistory(IDalSession session, IInstrument instrument)
        {
            IList list = GetInstrumentHistoryList(session, instrument);
            if (list != null && list.Count == 1)
                return (IInstrumentHistory)list[0];
            else
                return null;
        }

        /// <summary>
        /// Get InstrumentHistory object by ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="oldInstrument">The instrument that has been changed</param>
        /// <returns>InstrumentHistory object</returns>
        public static IInstrumentsHistoryConversion GetInstrumentConversion(IDalSession session, int id)
        {
            return session.Session.Linq<InstrumentsHistoryConversion>()
                    .Where(x => x.Key == id).FirstOrDefault();
        }

        //public static IInstrumentHistoryBonusDistribution GetInstrumentHistoryBonusDistribution(IDalSession session, int bonusKey)
        //{
        //    var step1 = session.Session.Linq<InstrumentHistoryBonusDistribution>()
        //                        .Where(dh => dh.Key == bonusKey)
        //                        .AsQueryable();

        //    if (step1.Count() == 1)
        //        return step1.Cast<IInstrumentHistoryBonusDistribution>()
        //                    .First();
        //    else
        //        return null;
        //}

        //public static IList<IBonusDistribution> GetBonusDistributions(IDalSession session, int instrumentHistoryID)
        //{
        //    return session.Session.Linq<IBonusDistribution>()
        //            .Where(dh => dh.InstrumentTransformation.Key == instrumentHistoryID)
        //            .ToList();
        //}

    }
}

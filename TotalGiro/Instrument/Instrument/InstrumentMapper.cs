using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using NHibernate.Linq;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// This class is used to instantiate Instrument objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class InstrumentMapper
    {
		public const int TG_BASE_CURRENCY_ID = 600;

        /// <summary>
        /// Get instrument by ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="instrumentID">Identifier</param>
        /// <returns>Instrument object</returns>
		public static IInstrument GetInstrument(IDalSession session, Int32 instrumentID)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", instrumentID));
            IList result = session.GetList(typeof(Instrument), expressions);
            if ((result != null) && (result.Count > 0))
                return (IInstrument)result[0];
            else
                return null;
        }

        /// <summary>
        /// Get instrument by ISIN
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="isin">isin</param>
        /// <returns>Instrument object</returns>
        public static IInstrument GetInstrumentByIsin(IDalSession session, string isin)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Isin", isin));
            IList result = session.GetList(typeof(TradeableInstrument), expressions);
            if ((result != null) && (result.Count > 0))
                return (IInstrument)result[0];
            else
                return null;
        }

        /// <summary>
        /// Get all system instruments
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <returns>Collection of instruments</returns>
		public static IList<IInstrument> GetInstruments(IDalSession session)
		{
			return session.GetTypedList<Instrument, IInstrument>();
		}

        /// <summary>
        /// Get instrumenst by category
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="secCategory">Category</param>
        /// <returns>Collection of instruments</returns>
        public static IList<IInstrument> GetInstruments(IDalSession session, SecCategories secCategoryId)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("SecCategory.key", (int)secCategoryId));
            return session.GetTypedList<Instrument, IInstrument>(expressions);
        }

        /// <summary>
        /// Get instrumenst by their keys
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="keys">the unique identifiers of the instruments</param>
        /// <returns>Collection of instruments</returns>
        public static IList<IInstrument> GetInstruments(IDalSession session, int[] keys)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("Key", keys));
            return session.GetTypedList<Instrument, IInstrument>(expressions);
        }

        /// <exclude/>
        public static IList<ITradeableInstrument> GetInstrumentsByIsin(IDalSession session, string isin)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Like("Isin", isin, MatchMode.Anywhere));
            return session.GetTypedList<TradeableInstrument, ITradeableInstrument>(expressions);
        }
        public static IList<IInstrumentsWithPrices> GetInstrumentswithPricesByIsin(IDalSession session, string isin)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Like("Isin", isin, MatchMode.Anywhere));
            return session.GetTypedList<InstrumentsWithPrices, IInstrumentsWithPrices>(expressions);
        }

        /// <summary>
        /// Get tradeable instruments by name filter
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="InstrumentFilter">Name filter</param>
        /// <returns>collection of tradeable instruments</returns>
        public static IList<ITradeableInstrument> GetFilteredTradeableInstruments(IDalSession session, string InstrumentFilter)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            if (InstrumentFilter != null && InstrumentFilter.Length > 0)
                expressions.Add(Expression.Like("Name", InstrumentFilter, MatchMode.Anywhere));
            return session.GetTypedList<TradeableInstrument, ITradeableInstrument>(expressions);
        }

        /// <summary>
        /// Get currency by id
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="instrumentID">Identifier</param>
        /// <returns>Currency</returns>
        public static ICurrency GetCurrency(IDalSession session, Int32 instrumentID)
		{
			return (ICurrency)session.GetObjectInstance(typeof(Currency), instrumentID);
		}

        /// <summary>
        /// Get currency by name
        /// </summary>
        /// <param name="session">data access object</param>
        /// <param name="currencyName">Name</param>
        /// <returns>Currency</returns>
        public static ICurrency GetCurrencyByName(IDalSession session, string currencyName)
        {
            ICurrency currency = null;
            if (!string.IsNullOrEmpty(currencyName))
            {
                IList<ICurrency> currencies = GetCurrenciesByName(session, currencyName, "", ActivityReturnFilter.All);
                if (currencies != null && currencies.Count == 1)
                    currency = currencies[0];
            }
            return currency;
        }

        /// <summary>
        /// Get currency by name
        /// </summary>
        /// <param name="session">data access object</param>
        /// <param name="currencyName">Name</param>
        /// <param name="instrumentName">Name</param>
        /// <returns>Currency</returns>
        public static IList<ICurrency> GetCurrenciesByName(
            IDalSession session, string currencyName, 
            string instrumentName, ActivityReturnFilter activityFilter)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Like("Symbol", currencyName, MatchMode.Start));
            if (!string.IsNullOrEmpty(instrumentName))
                expressions.Add(Expression.Like("Name", instrumentName, MatchMode.Start));
            if (activityFilter != ActivityReturnFilter.All)
                expressions.Add(Expression.Eq("IsActive", activityFilter == ActivityReturnFilter.Active));

            return session.GetTypedList<Currency, ICurrency>(expressions);
        }
        
        /// <summary>
        /// Get currency by system ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="instrumentID">System ID</param>
        /// <returns>Currency</returns>
        public static ICurrency GetKnownCurrency(IDalSession session, KnownCurrency instrumentID)
		{
			return (ICurrency)session.GetObjectInstance(typeof(Currency),(int) instrumentID);
		}

        /// <summary>
        /// Get all system currencies
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <returns>Collection of currencies</returns>
        public static IList<ICurrency> GetCurrencies(IDalSession session)
		{
            return GetCurrencies(session, true);
		}

        /// Get all system currencies
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <returns>Collection of currencies</returns>
        public static IList<ICurrency> GetCurrencies(IDalSession session, bool onlyActive)
        {
            IQueryable<Currency> list = session.Session.Linq<Currency>();
            if (onlyActive)
                list = list.Where(x => x.IsActive);
           return  list.OrderBy(x => x.Symbol).Cast<ICurrency>().ToList();
        }

        public static IList<ICurrency> GetCurrenciesSorted(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();
            expressions.Add(Expression.Eq("IsActive", true));
            orderings.Add(Order.Asc("Name"));
            // Last two null parameters are necessary, otherwise sorting doesn't work
            return session.GetTypedList<Currency, ICurrency>(expressions, orderings);
        }

        public static IList<ICurrency> GetCurrenciesSortedByCurrency(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();
            expressions.Add(Expression.Eq("IsActive", true));
            orderings.Add(Order.Asc("Symbol"));
            // Last two null parameters are necessary, otherwise sorting doesn't work
            return session.GetTypedList<Currency, ICurrency>(expressions, orderings);
        }

        /// <summary>
        /// Get system base currency
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <returns>Currency</returns>
		public static ICurrency GetBaseCurrency(IDalSession session)
		{
			return InstrumentMapper.GetCurrency(session, TG_BASE_CURRENCY_ID);
		}

        /// <exclude/>
		public static InstrumentSize GetAmountInBaseCurrency(Decimal quantity)
		{
			throw new ApplicationException("Do not use the InstrumentFactory anymore.");
			//IInstrument underlying = InstrumentFactory.GetBaseCurrency();
			//return new InstrumentSize(quantity, underlying);
		}

        /// <summary>
        /// Return a list with just one tradeable instrument
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="id">Identifier</param>
        /// <returns>Collection of one instrument</returns>
        public static ITradeableInstrument GetTradeableInstrument(IDalSession session, Int32 id)
        {
            ITradeableInstrument instrument = null;
            IList<ITradeableInstrument> list = GetTradeableInstruments(session, id);
            if (list != null && list.Count == 1)
                instrument = list[0];
            return instrument;
        }

        public static IInstrumentsWithPrices GetInstrumentWithPrice(IDalSession session, Int32 id)
        {
            return (IInstrumentsWithPrices)session.GetObjectInstance(typeof(InstrumentsWithPrices), (int)id);
        }

        /// <summary>
        /// Return a list with just one tradeable instrument
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="id">Identifier</param>
        /// <returns>Collection of one instrument</returns>
        public static IList<ITradeableInstrument> GetTradeableInstruments(IDalSession session, Int32 id)
        {
            return session.GetTypedList<TradeableInstrument, ITradeableInstrument>(id);
        }

        /// <summary>
        /// Return a list with all tradeable instruments
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <returns>Collection of instruments</returns>
        public static IList<ITradeableInstrument> GetTradeableInstruments(IDalSession session)
        {
            return GetTradeableInstruments(session, SecCategoryFilterOptions.All, null, null, SecCategories.Undefined, 0, 0, true, ActivityReturnFilter.Active, null);
        }

        public static List<KeyValuePair<int, string>> GetTradeableInstrumentsForDropDownList(IDalSession session)
        {
            return GetTradeableInstrumentsForDropDownList(session, SecCategoryFilterOptions.Securities);
        }

        public static List<KeyValuePair<int, string>> GetTradeableInstrumentsForDropDownList(IDalSession session, SecCategoryFilterOptions secCategoryFilter)
        {
            return GetTradeableInstruments(session, secCategoryFilter, null, null, SecCategories.Undefined, 0, 0, true, ActivityReturnFilter.Active, null)
                .Select(i => new KeyValuePair<int, string>(i.Key, (i.Name + " - " + i.Isin)))
                .ToList()
                .OrderBy(o => o.Value)
                .ToList();                                    
        }

        public static IQueryable<ITradeableInstrument> GetTradeableInstrumentsAsQueryable(IDalSession session)
        {
            return session.Session.Linq<TradeableInstrument>()
                .Where(it => it.IsActive)
                .Select(i => i)
                .Cast<ITradeableInstrument>();
        }
        

        public static IList<ITradeableInstrument> GetTradeableInstruments(IDalSession session, 
            SecCategoryFilterOptions secCategoryFilter, 
            string isin, string instrumentName, SecCategories secCategoryId, 
            int exchangeId, int currencyNominalId, bool assetManagerMappedOnly, 
            ActivityReturnFilter activityFilter, string hqlWhere)
        {
            Hashtable parameters = new Hashtable();

            if (secCategoryFilter != SecCategoryFilterOptions.All)
                parameters.Add("secCategoryType", (int)secCategoryFilter);
            if (!string.IsNullOrEmpty(isin))
                parameters.Add("isin", Util.PrepareNamedParameterWithWildcard(isin, MatchModes.Anywhere));
            if (!string.IsNullOrEmpty(instrumentName))
                parameters.Add("instrumentName", Util.PrepareNamedParameterWithWildcard(instrumentName, MatchModes.Anywhere));
            if (secCategoryId != SecCategories.Undefined)
                parameters.Add("secCategoryId", secCategoryId);
            if (exchangeId > 0)
                parameters.Add("exchangeId", exchangeId);
            if (currencyNominalId > 0)
                parameters.Add("currencyNominalId", currencyNominalId);
            if (activityFilter != ActivityReturnFilter.All)
                parameters.Add("isActive", activityFilter == ActivityReturnFilter.Active);
            if (assetManagerMappedOnly)
            {
                IManagementCompany comp = LoginMapper.GetCurrentManagmentCompany(session);
                if (comp != null && !comp.IsStichting)
                    parameters.Add("managementCompanyID", comp.Key);
            }
            IList<ITradeableInstrument> list = session.GetTypedListByNamedQuery<ITradeableInstrument>(
                "B4F.TotalGiro.Instruments.Instrument.GetTradeableInstruments",
                hqlWhere, parameters);

            if (secCategoryFilter != SecCategoryFilterOptions.CorporateAction)
                list = list.Where(x => x.SecCategory.SecCategoryType != SecCategoryTypes.CorporateAction).ToList();

            return list;
        }

        public static IList<IBenchMark> GetBenchMarks(IDalSession session)
        {
            return session.GetTypedList<BenchMark, IBenchMark>();
        }

        public static IList<IBenchMark> GetBenchMarks(
            IDalSession session, string isin, string instrumentName, 
            int currencyNominalId, ActivityReturnFilter activityFilter)
        {
            Hashtable parameters = new Hashtable();

            if (!string.IsNullOrEmpty(isin))
                parameters.Add("isin", Util.PrepareNamedParameterWithWildcard(isin, MatchModes.Anywhere));
            if (!string.IsNullOrEmpty(instrumentName))
                parameters.Add("instrumentName", Util.PrepareNamedParameterWithWildcard(instrumentName, MatchModes.Anywhere));
            if (currencyNominalId > 0)
                parameters.Add("currencyNominalId", currencyNominalId);
            if (activityFilter != ActivityReturnFilter.All)
                parameters.Add("isActive", activityFilter == ActivityReturnFilter.Active);
            return session.GetTypedListByNamedQuery<IBenchMark>(
                "B4F.TotalGiro.Instruments.Instrument.GetBenchmarks",
                parameters);
        }


        public static IList<IVirtualFund> GetVirtualFunds(IDalSession session)
        {
            return session.GetTypedList<VirtualFund, IVirtualFund>();
        }


        /// <summary>
        /// Persist instrument changes in system
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="instrument">Instrument</param>
        /// <returns>Flag</returns>
        public static bool Update(IDalSession session, IInstrument instrument)
        {
            session.InsertOrUpdate(instrument);
            return true;
        }
    }
}

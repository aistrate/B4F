using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using System.Data;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class InstrumentPriceUpdateAdapter
    {
        public static DataSet GetFilteredInstruments(
            InstrumentCurrentPricesAdapter.DataSourceChoices secCatChoice, string isin, 
            string instrumentName, int currencyNominalId, ActivityReturnFilter activityFilter)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                switch (secCatChoice)
                {
                    case InstrumentCurrentPricesAdapter.DataSourceChoices.InstrumentsTradeable:
                        ds = InstrumentMapper.GetTradeableInstruments(
                            session, SecCategoryFilterOptions.Securities,
                            isin, instrumentName, 0, 0, currencyNominalId, false, activityFilter, "")
                            .Select(c => new
                            {
                                c.Key,
                                Description = c.DisplayIsinWithName
                            })
                            .ToDataSet();
                        break;
                    case InstrumentCurrentPricesAdapter.DataSourceChoices.Currencies:
                        ds = InstrumentMapper.GetCurrenciesByName(
                            session, isin, instrumentName, activityFilter)
                            .Where(x => !x.IsBase)
                            .Select(c => new
                            {
                                c.Key,
                                Description = c.Symbol + " - " + c.Name
                            })
                            .ToDataSet();
                        break;
                    case InstrumentCurrentPricesAdapter.DataSourceChoices.BenchMarks:
                        ds = InstrumentMapper.GetBenchMarks(
                            session, isin, instrumentName, currencyNominalId, activityFilter)
                            .Select(c => new
                            {
                                c.Key,
                                Description = c.DisplayIsinWithName
                            })
                            .ToDataSet();
                        break;
                }
                Utility.AddEmptyFirstRow(ds.Tables[0]);
                return ds;
            }
        }

        public static DataSet GetInstrumentPriceHistory(int instrumentId, ref DateTime startDate, ref DateTime endDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                IInstrument instrument = InstrumentMapper.GetInstrument(session, instrumentId);
                if (instrument != null)
                {
                    switch (instrument.SecCategory.Key)
	                {
                        case SecCategories.Cash:
                            ds = HistoricalExRateMapper.GetHistoricalExRates(session, instrument.ToCurrency, startDate, endDate)
                                .Select(c => new
                                {
                                    c.Key,
                                    instrumentId,
                                    Date = c.RateDate,
                                    PriceQuantity = c.Rate,
                                    Currency = c.Currency.AltSymbol,
                                    DecimalPlaces = 5,
                                    c.CreationDate,
                                    DateKey = getPriceHistoryKey(c.RateDate, true),
                                })
                                .ToDataSet();
                            break;
                        default:
                            ds = HistoricalPriceMapper.GetHistoricalPrices(session, instrumentId, startDate, endDate)
                                .Select(c => new
                                {
                                    c.Key,
                                    instrumentId,
                                    c.Date,
                                    PriceQuantity = c.Price.Quantity,
                                    Currency = c.Price.Underlying.AltSymbol,
                                    DecimalPlaces = instrument.DecimalPlaces,
                                    c.CreationDate,
                                    DateKey = getPriceHistoryKey(c.Date, true)
                                })
                                .ToDataSet();
                            break;
                    }
                    
                    if (Util.IsNullDate(endDate) || endDate.Equals(DateTime.MaxValue)) endDate = DateTime.Today.AddDays(-1);
                    if (Util.IsNotNullDate(startDate))
                    {
                        int days = Util.DateDiff(DateInterval.Day, startDate, endDate);
                        if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count < days)
                        {
                            DateTime[] storedDays = ds.Tables[0].AsEnumerable().Select(dr => dr.Field<DateTime>("Date")).ToArray();
                            DateTime[] allDays = Util.GetDatesArray(startDate, endDate);
                            var missingDays = allDays.GroupJoin(storedDays,
                                a => a,
                                s => s,
                                (a, s) => new { all = a, stored = s })
                                .Where(o => o.stored.Count() == 0)
                                .OrderBy(a => a.all)
                                .Select(a => a.all);

                            string currency = instrument.IsWithPrice ? 
                                ((IInstrumentsWithPrices)instrument).CurrencyNominal.Get(e => e.AltSymbol) :
                                ((ICurrency)instrument).Get(e => e.AltSymbol);
                            foreach (var date in missingDays)
                            {
                                DataRow row = ds.Tables[0].NewRow();
                                row[0] = 0;
                                row[7] = getPriceHistoryKey(date, false);
                                row[1] = instrumentId;
                                row[2] = date;
                                row[3] = 0M;
                                row[4] = currency;
                                row[5] = instrument.IsCash ? 5 : instrument.DecimalPlaces;
                                ds.Tables[0].Rows.Add(row);
                            }
                        }
                    }                    
                }
                return ds;
            }
        }

        public static string GetInstrumentDetails(int instrumentId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                if (instrumentId != 0)
                    return InstrumentMapper.GetInstrument(session, instrumentId).Get(e => e.DisplayNameWithIsin);
                else
                    return "";
            }
        }

        public static Tuple<string, DateTime, DateTime, bool> GetTradeableInstrumentPriceDetails(int instrumentId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Tuple<string, DateTime, DateTime, bool> retVal = null;
                if (instrumentId != 0)
                {
                    ITradeableInstrument instrument = (ITradeableInstrument)InstrumentMapper.GetInstrument(session, instrumentId);
                    if (instrument != null)
                    {
                        DateTime startDate = instrument.CurrentPrice != null ? instrument.CurrentPrice.Date.AddMonths(-1) : instrument.IssueDate;
                        DateTime endDate = startDate < DateTime.Today.AddYears(-1) ? startDate.AddYears(1) : DateTime.MinValue;
                        retVal = new Tuple<string, DateTime, DateTime, bool>(
                            instrument.Isin,
                            startDate,
                            endDate,
                            instrument.IsActive);
                    }
                }
                return retVal;
            }
        }

        public static void UpdateInstrumentPriceHistory(decimal newQuantity, bool ignoreWarning, int DateKey, int Key, int instrumentId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IInstrument instrument = InstrumentMapper.GetInstrument(session, instrumentId);

                var res = getDataFromPriceHistoryKey(DateKey);
                DateTime date = res.Item1;
                bool exist = res.Item2;

                if (instrument != null)
                {
                    switch (instrument.SecCategory.Key)
                    {
                        case SecCategories.Cash:
                            if (exist)
                            {
                                IHistoricalExRate newExRate = HistoricalExRateMapper.GetHistoricalExRate(session, Key);
                                if (newExRate.Rate != newQuantity)
                                {
                                    newExRate.Rate = newQuantity;
                                    if (!ignoreWarning)
                                        CheckNewRate(session, newExRate);
                                    HistoricalExRateMapper.Update(session, newExRate);
                                }
                            }
                            else
                            {
                                ICurrency currency = instrument.ToCurrency;
                                IHistoricalExRate newExRate = new HistoricalExRate(currency, newQuantity, date);
                                currency.HistoricalExRates.AddExRate(newExRate);
                                if (!ignoreWarning)
                                    CheckNewRate(session, newExRate);
                                InstrumentMapper.Update(session, currency);
                            }
                            break;
                        default:
                            IInstrumentsWithPrices iwp = (IInstrumentsWithPrices)instrument;
                            Price price = new Price(newQuantity, iwp.CurrencyNominal, iwp);
                            if (exist)
                            {
                                IHistoricalPrice newPrice = HistoricalPriceMapper.GetHistoricalPrice(session, Key);
                                if (newPrice.Price != price)
                                {
                                    newPrice.Price = price;
                                    if (!ignoreWarning)
                                        CheckNewPrice(session, newPrice);
                                    HistoricalPriceMapper.Update(session, newPrice);
                                }
                            }
                            else
                            {
                                IHistoricalPrice newPrice = new HistoricalPrice(price, date);
                                iwp.HistoricalPrices.AddHistoricalPrice(newPrice);
                                if (!ignoreWarning)
                                    CheckNewPrice(session, newPrice);
                                InstrumentMapper.Update(session, iwp);
                            }
                            break;
                    }
                }
            }
        }

        public static bool UpdateInstrumentPriceHistory(int[] dateKeys, int instrumentId, decimal newQuantity, bool ignoreWarning, out string errMessage)
        {
            bool success = true;
            errMessage = "";

            if (dateKeys == null || dateKeys.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            DateTime[] dates = dateKeys
                .Select(k => getDataFromPriceHistoryKey(k).Item1)
                .OrderBy(k => k)
                .ToArray();
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IInstrument instrument = InstrumentMapper.GetInstrument(session, instrumentId);
                List<DateTime> notExistingDates = new List<DateTime>();
                bool saveStuff = false;

                if (instrument != null)
                {
                    switch (instrument.SecCategory.Key)
                    {
                        case SecCategories.Cash:
                            IList<IHistoricalExRate> exRates = HistoricalExRateMapper.GetHistoricalExRates(session, instrumentId, dates);
                            if (exRates != null && exRates.Count > 0)
                            {
                                foreach (DateTime date in dates)
	                            {
                                    IHistoricalExRate exRate = exRates.Where(x => x.RateDate == date).FirstOrDefault();
                                    if (exRate != null)
                                    {
                                        if (exRate.Rate != newQuantity)
                                        {
                                            exRate.Rate = newQuantity;
                                            if (!ignoreWarning)
                                                ignoreWarning = CheckNewRate(session, exRate, false, out errMessage);
                                            if (!string.IsNullOrEmpty(errMessage)) return false;
                                            saveStuff = true;
                                        }
                                    }
                                    else
                                        notExistingDates.Add(date);
	                            }
                            }
                            else
                                notExistingDates.AddRange(dates);

                            if (saveStuff)
                                session.Update(exRates);
                            saveStuff = false;

                            ICurrency currency = instrument.ToCurrency;
                            foreach (DateTime date in notExistingDates)
	                        {
                                IHistoricalExRate newExRate = new HistoricalExRate(currency, newQuantity, date);
                                currency.HistoricalExRates.AddExRate(newExRate);
                                if (!ignoreWarning)
                                    ignoreWarning = CheckNewRate(session, newExRate, false, out errMessage);
                                if (!string.IsNullOrEmpty(errMessage)) return false;
                                saveStuff = true;
	                        }
                            if (saveStuff)
                                InstrumentMapper.Update(session, currency);
                            break;
                        default:
                            IInstrumentsWithPrices iwp = (IInstrumentsWithPrices)instrument;
                            Price newPrice = new Price(newQuantity, iwp.CurrencyNominal, iwp);

                            IList<IHistoricalPrice> prices = HistoricalPriceMapper.GetHistoricalPrices(session, instrumentId, dates);
                            if (prices != null && prices.Count > 0)
                            {
                                foreach (DateTime date in dates)
                                {
                                    IHistoricalPrice price = prices.Where(x => x.Date == date).FirstOrDefault();
                                    if (price != null)
                                    {
                                        if (!price.Price.Equals(newPrice))
                                        {
                                            price.Price = newPrice;
                                            if (!ignoreWarning)
                                                ignoreWarning = CheckNewPrice(session, price, false, out errMessage);
                                            if (!string.IsNullOrEmpty(errMessage)) return false;
                                            saveStuff = true;
                                        }
                                    }
                                    else
                                        notExistingDates.Add(date);
                                }
                            }
                            else
                                notExistingDates.AddRange(dates);

                            if (saveStuff)
                                session.Update(prices);
                            saveStuff = false;

                            foreach (DateTime date in notExistingDates)
                            {
                                IHistoricalPrice newHistoricalPrice = new HistoricalPrice(newPrice, date);
                                iwp.HistoricalPrices.AddHistoricalPrice(newHistoricalPrice);
                                if (!ignoreWarning)
                                    ignoreWarning = CheckNewPrice(session, newHistoricalPrice, false, out errMessage);
                                if (!string.IsNullOrEmpty(errMessage)) return false;
                                saveStuff = true;
                            }
                            if (saveStuff)
                                InstrumentMapper.Update(session, iwp);
                            break;
                    }
                }
            }
            return success;
        }

        public static bool CheckNewPrice(IDalSession session, IHistoricalPrice newPrice)
        {
            string errMessage;
            return CheckNewPrice(session, newPrice, true, out errMessage);
        }

        public static bool CheckNewPrice(IDalSession session, IHistoricalPrice newPrice, bool raiseErr, out string errMessage)
        {
            bool success = true;
            errMessage = "";
            if (!HistoricalPriceMapper.CheckHistoricalPrice(session, newPrice, out errMessage) && !string.IsNullOrEmpty(errMessage))
            {
                if (raiseErr)
                    throw new ApplicationException(errMessage);
                else
                    success = false;
            }
            return success;
        }

        public static bool CheckNewRate(IDalSession session, IHistoricalExRate newRate)
        {
            string errMessage = "";
            return CheckNewRate(session, newRate, true, out errMessage);
        }

        public static bool CheckNewRate(IDalSession session, IHistoricalExRate newRate, bool raiseErr, out string errMessage)
        {
            bool success = true;
            errMessage = "";
            if (!HistoricalExRateMapper.CheckHistoricalExRate(session, newRate, out errMessage) && !string.IsNullOrEmpty(errMessage))
            {
                if (raiseErr)
                    throw new ApplicationException(errMessage);
                else
                    success = false;
            }
            return success;
        }


        private static int getPriceHistoryKey(DateTime date, bool exist)
        {
            return Util.GetValueFromDate(date) + ((exist ? 1 : 0) * CONST_EXIST_VALUE);
        }

        private static Tuple<DateTime, bool> getDataFromPriceHistoryKey(int key)
        {
            int exists = key / CONST_EXIST_VALUE;
            key -= exists * CONST_EXIST_VALUE;

            DateTime date = Util.GetDateFromValue(key);
            return Tuple.Create(date, Convert.ToBoolean(exists));
        }

        private const int CONST_EXIST_VALUE = 100000000;
    }
}

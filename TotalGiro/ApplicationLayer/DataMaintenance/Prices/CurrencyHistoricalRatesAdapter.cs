using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.DataMigration.MissingHistoricalData;
using B4F.TotalGiro.Utils;
//using B4F.DataMigration.MissingHistoricalData;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public class CurrencyHistoricalRatesAdapter
    {
        public static DataSet GetHistoricalCurrencyRates(DateTime date, 
            string currencyName, string instrumentName, ActivityReturnFilter activityFilter)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IList<ICurrency> currencies = InstrumentMapper.GetCurrenciesByName(
                session, currencyName, instrumentName, activityFilter)
                .Where(x => !x.IsBase)
                .ToList();
            DataSet historicalRateDs = HistoricalExRateMapper.GetHistoricalExRates(session, date)
                .Select(c => new
                {
                    Currency_Key =
                        c.Currency.Key,
                    c.Rate
                })
                .ToDataSet();

            int i = 0;
            CurrencyRateRowView[] currencyRateRowViews = new CurrencyRateRowView[currencies.Count];
            foreach (Currency currency in currencies)
            {
                DataRow[] historicalRateRows = historicalRateDs.Tables[0].Select(string.Format("Currency_Key = {0}", currency.Key));
                decimal rate = (historicalRateRows.Length > 0 ? (decimal)historicalRateRows[0]["Rate"] : 0m);
                currencyRateRowViews[i++] = new CurrencyRateRowView(currency, rate);
            }

            DataSet ds = currencyRateRowViews
                    .Select(c => new
                    {
                        c.InstrumentId,
                        c.Symbol, 
                        c.CountryName, 
                        c.AltSymbol, 
                        c.Rate
                    })
                    .ToDataSet(); 
            session.Close();
            return ds;
        }

        public static DataSet GetHistoricalMissingExRates(int currencyId, DateTime date)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            // get the transfer data
            string query = "SELECT ID {mhp.Key}, InstrumentID {mhp.Instrument}, " +
                "Date {mhp.Date}, IsBizzDay {mhp.IsBizzDay} " +
                "FROM dbo.FN_TG_GetMissingExRateDates(" + currencyId.ToString() + ",NULL,'" + date.ToString("yyyy-MM-dd") +
                "') ORDER BY Date DESC";
            IList dates = session.GetListbySQLQuery(query, "mhp", typeof(MissingHistoricalInstrumentData));

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                dates, "Key, Date, IsBizzDay");

            ds.Tables[0].Columns.Add("Rate", typeof(decimal));
            session.Close();
            return ds;
        }

        public static string GetCurrencyDescription(int currencyId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            string description = "";

            ICurrency currency = InstrumentMapper.GetCurrency(session, currencyId);
            if (currency != null)
                description = currency.Name;
            session.Close();
            return description;
        }

        public static void UpdateHistoricalExRate(DateTime Date, bool ignoreWarning, decimal newQuantity, int original_InstrumentId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ICurrency currency = InstrumentMapper.GetCurrency(session, original_InstrumentId);
            IHistoricalExRate historicalExRate = HistoricalExRateMapper.GetHistoricalExRate(session, currency, Date);
            if (historicalExRate == null)
            {
                IHistoricalExRate newExRate = new HistoricalExRate(currency, newQuantity, Date);
                currency.HistoricalExRates.AddExRate(newExRate);
                if (!ignoreWarning)
                    InstrumentPriceUpdateAdapter.CheckNewRate(session, newExRate);
                InstrumentMapper.Update(session, currency);
            }
            else
            {
                if (historicalExRate.Rate != newQuantity)
                {
                    historicalExRate.Rate = newQuantity;
                    if (!ignoreWarning)
                        InstrumentPriceUpdateAdapter.CheckNewRate(session, historicalExRate);
                    HistoricalExRateMapper.Update(session, historicalExRate);
                }
            }
            session.Close();
        }
    }
}

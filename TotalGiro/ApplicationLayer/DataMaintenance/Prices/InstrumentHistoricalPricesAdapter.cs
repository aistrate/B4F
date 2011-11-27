using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public static class InstrumentHistoricalPricesAdapter
    {
        public enum DataSourceChoices
        {
            InstrumentsTradeable = 0,
            Currencies = 1,
            BenchMarks = 2
        }

        public static DataSet GetHistoricalPrices(DateTime date, 
            string isin, string instrumentName, int currencyNominalId, 
            ActivityReturnFilter activityFilter)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IList<ITradeableInstrument> instruments = InstrumentMapper.GetTradeableInstruments(
                session, SecCategoryFilterOptions.All, isin, instrumentName, SecCategories.Undefined,
                0, currencyNominalId, false, activityFilter, "");
            DataSet historicalPriceDs = 
                HistoricalPriceMapper.GetHistoricalPrices(session, date)
                .Select(c => new
                {
                    Price_Key = 
                        c.Price.Instrument.Key,
                    Price_Amount =
                        c.Price.Amount.Quantity
                })
                .ToDataSet();

            int i = 0;
            InstrumentPriceRowView[] instrumentPriceRowViews = new InstrumentPriceRowView[instruments.Count];
            foreach (ITradeableInstrument instrument in instruments)
            {
                DataRow[] historicalPriceRows = historicalPriceDs.Tables[0].Select(string.Format("Price_Key = {0}", instrument.Key));
                decimal priceQuantity = (historicalPriceRows.Length > 0 ? (decimal)historicalPriceRows[0]["Price_Amount"] : 0m);
                instrumentPriceRowViews[i++] = new InstrumentPriceRowView(instrument, priceQuantity);
            }

            DataSet ds = instrumentPriceRowViews
                    .Select(c => new
                    {
                        c.InstrumentId,
                        c.Isin,
                        c.InstrumentName,
                        c.Currency,
                        c.PriceQuantity,
                        c.DecimalPlaces
                    })
                    .ToDataSet();
            
            session.Close();
            return ds;
        }

        public static DataSet GetHistoricalBenchMarks(DateTime date,
            string isin, string instrumentName, int currencyNominalId,
            ActivityReturnFilter activityFilter)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IList<IBenchMark> instruments = InstrumentMapper.GetBenchMarks(
                session, isin, instrumentName, currencyNominalId, activityFilter);
            DataSet historicalPriceDs = HistoricalPriceMapper.GetHistoricalPrices(session, date)
                    .Select(c => new
                    {
                        Price_Key =
                            c.Price.Instrument.Key,
                        Price_Amount =
                            c.Price.Amount.Quantity
                    })
                    .ToDataSet(); 

            int i = 0;
            InstrumentPriceRowView[] instrumentPriceRowViews = new InstrumentPriceRowView[instruments.Count];
            foreach (IBenchMark instrument in instruments)
            {
                DataRow[] historicalPriceRows = historicalPriceDs.Tables[0].Select(string.Format("Price_Key = {0}", instrument.Key));
                decimal priceQuantity = (historicalPriceRows.Length > 0 ? (decimal)historicalPriceRows[0]["Price_Amount"] : 0m);
                instrumentPriceRowViews[i++] = new InstrumentPriceRowView(instrument, priceQuantity);
            }

            DataSet ds = instrumentPriceRowViews
                    .Select(c => new
                    {
                        c.InstrumentId, 
                        c.Isin, 
                        c.InstrumentName, 
                        c.Currency, 
                        c.PriceQuantity, 
                        c.DecimalPlaces
                    })
                    .ToDataSet();
            session.Close();
            return ds;
        }

        public static void CheckPrice(int instrumentId, decimal newQuantity, DateTime date, out string message)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            message = "";

            ITradeableInstrument instrument = (ITradeableInstrument)InstrumentMapper.GetInstrument(session, instrumentId);
            IHistoricalPrice prvPrice = HistoricalPriceMapper.GetLastValidHistoricalPrice(session, instrument, date.AddDays(-1));
            if (prvPrice != null)
            {
                //description = instrument.DisplayIsinWithName;
            }
            session.Close();
        }


        public static void UpdateHistoricalPrice(DateTime Date, bool ignoreWarning, decimal newQuantity, int original_InstrumentId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IInstrumentsWithPrices instrument = (IInstrumentsWithPrices)InstrumentMapper.GetInstrument(session, original_InstrumentId);
                Price price = new Price(newQuantity, instrument.CurrencyNominal, instrument);

                IList<IHistoricalPrice> historicalPrices = HistoricalPriceMapper.GetHistoricalPrices(session, instrument, Date);
                IHistoricalPrice historicalPrice = null;

                if (historicalPrices.Count == 0)
                {
                    historicalPrice = new HistoricalPrice(price, Date);
                    instrument.HistoricalPrices.AddHistoricalPrice(historicalPrice);
                    if (!ignoreWarning)
                        InstrumentPriceUpdateAdapter.CheckNewPrice(session, historicalPrice);
                    InstrumentMapper.Update(session, instrument);
                }
                else
                {
                    historicalPrice = (IHistoricalPrice)historicalPrices[0];
                    if (historicalPrice.Price != price)
                    {
                        historicalPrice.Price = price;
                        if (!ignoreWarning)
                            InstrumentPriceUpdateAdapter.CheckNewPrice(session, historicalPrice);
                        HistoricalPriceMapper.Update(session, historicalPrice);
                    }
                }
            }
        }

        public static DataSet GetHistoricalMissingPriceDates(int instrumentId, DateTime date)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            // get the transfer data
            string query = "SELECT ID {mhp.Key}, InstrumentID {mhp.Instrument}, " +
                "Date {mhp.Date}, IsBizzDay {mhp.IsBizzDay} " +
                "FROM dbo.FN_TG_GetMissingPriceDates(" + instrumentId.ToString() + ",NULL,'" + date.ToString("yyyy-MM-dd") +
                "') ORDER BY Date DESC";
            IList dates = session.GetListbySQLQuery(query, "mhp", typeof(MissingHistoricalInstrumentData));

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                dates, "Key, Date, IsBizzDay");

            ds.Tables[0].Columns.Add("Price", typeof(decimal));
            session.Close();
            return ds;
        }

        public static string GetInstrumentDescription(int instrumentId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            string description = "";

            ITradeableInstrument instrument = (ITradeableInstrument)InstrumentMapper.GetInstrument(session, instrumentId);
            if (instrument != null)
                description = instrument.DisplayIsinWithName;
            session.Close();
            return description;
        }

        public static DateTime LastWorkingDay()
        {
            DateTime temp = DateTime.Today.AddDays(-1);
            while(temp.DayOfWeek == DayOfWeek.Saturday || temp.DayOfWeek == DayOfWeek.Sunday)
                temp = temp.AddDays(-1);
            return temp;
        }

        public static DataSet GetDataSourceChoices()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable("DataSourceChoices"));
            ds.Tables["DataSourceChoices"].Columns.Add(new DataColumn("ID", typeof(int)));
            ds.Tables["DataSourceChoices"].Columns.Add(new DataColumn("InstrumentType", typeof(string)));

            DataRow row;

            row = ds.Tables["DataSourceChoices"].NewRow();
            row["ID"] = 0;
            row["InstrumentType"] = "Funds";
            ds.Tables["DataSourceChoices"].Rows.Add(row);

            row = ds.Tables["DataSourceChoices"].NewRow();
            row["ID"] = 1;
            row["InstrumentType"] = "Currencies";
            ds.Tables["DataSourceChoices"].Rows.Add(row);

            row = ds.Tables["DataSourceChoices"].NewRow();
            row["ID"] = 2;
            row["InstrumentType"] = "BenchMarks";
            ds.Tables["DataSourceChoices"].Rows.Add(row);

            return ds;
        }

    }
}

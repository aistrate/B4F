using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class InstrumentCurrentPricesAdapter
    {
        public enum DataSourceChoices
        {
            InstrumentsTradeable = 0,
            Currencies = 1,
            BenchMarks = 2
        }

        public static DataSet GetInstrumentCurrentPrices()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentMapper.GetTradeableInstruments(session)
                    .Select(c => new
                    {
                        c.Isin, 
                        c.DisplayName, 
                        c.DisplayCurrentPrice, 
                        c.DisplayCurrentPriceDate
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetCurrencyCurrentRates()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentMapper.GetCurrencies(session, false)
                    .Select(c => new
                    {
                        c.Symbol,
                        CountryOfOrigin_CountryName =
                            c.CountryOfOrigin != null ? c.CountryOfOrigin.CountryName : "",
                        c.AltSymbol,
                        ExchangeRate_Rate =
                            c.ExchangeRate != null ? c.ExchangeRate.Rate : 0M,
                        ExchangeRate_RateDate =
                            c.ExchangeRate != null ? c.ExchangeRate.RateDate : DateTime.MinValue
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetBenchMarkPrices()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentMapper.GetBenchMarks(session)
                    .Select(c => new
                    {
                        c.Isin,
                        c.DisplayName,
                        c.DisplayCurrentPrice,
                        c.DisplayCurrentPriceDate
                    })
                    .ToDataSet();
            }
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

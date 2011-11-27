using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Dal;
using NHibernate.Linq;


namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class ImportFundPricesAdapter
    {
        public static DataView GetPriceList(DateTime importDate, Stream input)
        {
            DataTable dtTab = new DataTable();
            DataRow dtRow;
            bool blnNotOk;
            string strRow;

            dtTab.Columns.Add(new DataColumn("Skip", typeof(String)));
            dtTab.Columns.Add(new DataColumn("Instrument", typeof(String)));
            dtTab.Columns.Add(new DataColumn("ISIN", typeof(String)));
            dtTab.Columns.Add(new DataColumn("Price", typeof(Decimal)));
            dtTab.Columns.Add(new DataColumn("PrevPrice", typeof(Decimal)));
            dtTab.Columns.Add(new DataColumn("Message", typeof(String)));

            StreamReader objReader = new StreamReader(input);
            IDalSession session = NHSessionFactory.CreateSession();
            DateTime prevDate = GetPreviousImportDate(importDate);

            var pricesLastFiveDays = session.Session.Linq<HistoricalPrice>()
                                .Where(x => x.Date >= prevDate.AddDays(-5))
                                .Where(x => x.Date <= prevDate)
                                .OrderByDescending(x => x.Date)
                                .Cast<IHistoricalPrice>().ToList();

            var lastPrices = pricesLastFiveDays.GroupBy(i => i.Price.Instrument.DisplayIsin)
                    .Select(g =>
                    new
                    {
                        Isin = g.Key,
                        LastPrice = g.OrderByDescending(x => x.Date).FirstOrDefault()
                    })
                    .ToList();
            IList<ITradeableInstrument> instruments = InstrumentMapper.GetFilteredTradeableInstruments(session, "");


            //loop through rows in the stream 
            while (!objReader.EndOfStream)
            {
                decimal dRes;
                blnNotOk = false;
                //add a new row
                dtRow = dtTab.NewRow();

                //read a row from the stream
                strRow = objReader.ReadLine();
                string[] arrCells = (from e in strRow.Split("\t".ToCharArray()) select e.Trim()).ToArray();
                if (arrCells.Length == 3)
                {
                    //prepare new listitem
                    if (arrCells[0] == "") { arrCells[0] = "&nbsp;"; blnNotOk = true; }
                    if (arrCells[1] == "") { arrCells[1] = "&nbsp;"; blnNotOk = true; }
                    if (arrCells[2] == "") { arrCells[2] = "&nbsp;"; blnNotOk = true; }
                    if (!Decimal.TryParse(arrCells[2], out dRes)) { blnNotOk = true; }
                    //add skip cell...
                    if (blnNotOk)
                    {
                        //...when row is not valid
                        //intCount++;
                        //dtRow = System.Drawing.Color.Red;
                        dtRow[0] = "<input type='checkbox' checked disabled='true' />";
                        //add Instrument and ISIN data
                        dtRow[1] = arrCells[0];
                        dtRow[2] = arrCells[1];
                    }
                    else
                    {
                        //...when row seems valid
                        dtRow[0] = "<input type='checkbox' enabled='true'/>";
                        //add Instrument, ISIN and Price data
                        dtRow[1] = Util.ConvertToAscii(arrCells[0]);
                        dtRow[2] = arrCells[1];
                        dtRow[3] = dRes.ToString("0.0000##"); ;

                        IHistoricalPrice lastPrice = null;
                        if (lastPrices.Any(x => x.Isin == arrCells[1]))
                        {
                            lastPrice = lastPrices.Where(x => x.Isin == arrCells[1]).First().LastPrice;
                            dtRow[4] = lastPrice.Price.Quantity.ToString("0.0000##");
                            decimal diff = Math.Abs((lastPrice.Price.Quantity - dRes) / lastPrice.Price.Quantity);
                            if (diff > 0.05M)
                                dtRow[5] = string.Format("The new Price is {0}% different from the previous price.", (diff * 100).ToString("0.0"));
                        }
                        else if (instruments.Any(x => x.Isin == arrCells[1]))
                        {
                            ITradeableInstrument instrument = instruments.Where(x => x.Isin == arrCells[1]).First();
                            if (instrument.CurrentPrice == null)
                                dtRow[5] = "No current price available";
                            else
                            {
                                dtRow[4] = instrument.CurrentPrice.Price.Quantity.ToString("0.0000##");
                                decimal diff = Math.Abs((lastPrice.Price.Quantity - dRes) / dRes);
                                if (diff > 0.05M)
                                    dtRow[5] = string.Format("The new Price is {0}% different from the previous price.", (diff * 100).ToString("0.0"));
                                dtRow[5] += string.Format(" Last known price is from {0}", instrument.CurrentPrice.Date.ToString("dd-MM-yyyy"));
                            }
                        }
                        else
                            dtRow[5] = "This is a new instrument, which does not exist in the system.";
                    }

                    //apend new tablerow to the listtable
                    dtTab.Rows.Add(dtRow);
                }
            }
            return new DataView(dtTab);
        }

        private static DateTime GetPreviousImportDate(DateTime importDate)
        {
            DateTime prevDate = importDate.AddDays(-1);
            while (Util.IsWeekendOrHoliday(prevDate, null))
                prevDate = prevDate.AddDays(-1);
            return prevDate;
        }

        public static void UpdateHistoricalPrice(DateTime date, decimal PriceQuantity, string isin)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IInstrumentsWithPrices instrument = (IInstrumentsWithPrices)InstrumentMapper.GetInstrumentswithPricesByIsin(session, isin)[0];
            Price price = new Price(PriceQuantity, instrument.CurrencyNominal, instrument);

            IList<IHistoricalPrice> historicalPrices = HistoricalPriceMapper.GetHistoricalPrices(session, instrument, date);
            IHistoricalPrice historicalPrice = null;

            if (historicalPrices.Count == 0)
            {
                instrument.HistoricalPrices.AddHistoricalPrice(new HistoricalPrice(price, date));
                InstrumentMapper.Update(session, instrument);
            }
            else
            {
                historicalPrice = (IHistoricalPrice)historicalPrices[0];
                historicalPrice.Price = price;
                HistoricalPriceMapper.Update(session, historicalPrice);
            }

            //Hashtable parameters = new Hashtable();
            //parameters.Add("InstrumentID", instrument.Key);
            //session.ExecuteStoredProcedure("EXEC dbo.TG_FillHistPricesWeekendsHolidays @p_intInstrumentID = :InstrumentID", parameters);

            session.Close();
        }
    }
}

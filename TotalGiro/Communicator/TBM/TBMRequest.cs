using System;
using System.Collections.Generic;
using System.Xml;
using System.Net;
using System.IO;
using System.Globalization;
using System.Collections;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Communicator.TBM
{
    public class TBMRequest
    {
        private const string TBMWebSite = "http://tbxds.xml.eurobench.com/tbxds.asp";
        private const string TBMXmlTempDir = "C:\\TEMP\\xmlresponses\\";
        private const string TBMXmlTempDirNotExistsErrMessage = " does not exist. Please contact your system administrator.";
        private const string TBMRequestXML = "<?xml version=\"1.0\"?><TBMDataRequest version=\"{0}\" ticker-type=\"{1}\" user-name=\"{2}\">{3}</TBMDataRequest>";
        private const string TBMTestRequest = "<Dividend collection=\"Customer.Bits4Finance.Test\"/><Fundamentals collection=\"Customer.Bits4Finance.Test\"/><Quote collection=\"Customer.Bits4Finance.Test\"/>";
        private const string TBMQuoteCollectionRequest = "<Quote collection=\"trusts.active.nl\" />";
        private const string TBMDividendInfoRequest = "<Dividend collection=\"trusts.active.nl\" />";
        private const string TBMDetailInfoRequest = "<Details collection=\"trusts.active.nl\"  package=\"full\"/>";
        private const string TBMCompositionInfoRequest = "<Composition collection=\"trusts.active.nl\"  max-count=\"{0}\" last-issue-id=\"{1}\" />";
        private const string TBMSingleQuoteRequest = "<Quotes><Quote exchange=\"{0}\" ticker=\"{1}\" ticker-type=\"{2}\"/></Quotes>";
        private const string TBMSingleQuoteHistoryRequest = "<Quotes><Quote exchange=\"{0}\" ticker=\"{1}\" ticker-type=\"{2}\" date-start=\"{3}\" date-end=\"{4}\"/></Quotes>";

//        private const string TBMSingleQuoteRequest = "<Quotes><Quote exchange=\"{0}\" ticker=\"{1}\" ticker-type=\"{2}\" date-start=\"{3}\" date-end=\"{4}\"/></Quotes>";

        private string version = "3.5";
        private string tickertype = "ISIN";
        private string username = "BITS";
        private DateTime startdate = new DateTime(2005,12,31);
        private DateTime enddate = DateTime.Today;
        private string logfile = "";

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        public string TickerType
        {
            get { return tickertype; }
            set { tickertype = value; }
        }

        public string UserName
        {
            get { return username; }
            set { username = value; }
        }

        public DateTime StartDate
        {
            get { return startdate; }
            set { startdate = value; }
        }

        public DateTime EndDate
        {
            get { return enddate; }
            set { enddate = value; }
        }

        public string LogFile
        {
            get { return logfile; }
            set { logfile = value; }
        }

        private string buildTBMXML(string messageXML)
        {

            return String.Format(TBMRequestXML, Version, TickerType, UserName, messageXML);
        }

        public XmlDocument sendRequest(string tbxds_request)
        {
            StringReader sr = new StringReader(tbxds_request);
            
            XmlDocument doc = new XmlDocument();
            doc.Load(sr);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(TBMWebSite);
            req.Method = "POST";
            Stream stm = req.GetRequestStream();
            doc.Save(stm);
            stm.Close();

            XmlDocument dom = new XmlDocument();
            WebResponse resp = req.GetResponse();
            stm = resp.GetResponseStream();
            StreamReader r = new StreamReader(stm);

            dom.Load(r);

            return dom;
        }

        public XmlTextReader Copying(Stream FromStream, string TargetFile)
        {

            try
            {
                //Creat a file to save to

                byte[] tmpbuffer = new byte[32768];

                int nbytes = FromStream.Read(tmpbuffer, 0, 32768);
                FileStream fs = new FileStream(TargetFile, FileMode.Create);

                while (nbytes > 0)
                {

                    fs.Write(tmpbuffer, 0, nbytes);

                    nbytes = FromStream.Read(tmpbuffer, 0, 32768);
                }
                fs.Close();

                fs = new FileStream(TargetFile, FileMode.Open);
                return new XmlTextReader(fs);
            }
            //use Exception e as it can handle any exception 
            catch (Exception e)
            {
                //code if u like 
            }

            return null;
        }


        public XmlTextReader sendRequestXml(string tbxds_request)
        {
            StringReader sr = new StringReader(tbxds_request);

            XmlDocument doc = new XmlDocument();
            doc.Load(sr);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(TBMWebSite);
            req.Timeout = 300000;
            req.Method = "POST";
            Stream stm = req.GetRequestStream();
            doc.Save(stm);
            stm.Close();

            WebResponse resp = req.GetResponse();
            stm = resp.GetResponseStream();


            if (this.logfile.Length > 0)
                return (XmlTextReader)Copying(stm,this.logfile);
            else
                return new XmlTextReader(stm);

        }

        public string getTestData()
        {

            XmlDocument dom = sendRequest(buildTBMXML(TBMTestRequest));
            return dom.InnerXml;

        }

        public string getQuoteCollection()
        {

            XmlDocument dom = sendRequest(buildTBMXML(TBMQuoteCollectionRequest));
            return dom.InnerXml;

        }

        public string getDividendInfo()
        {

            XmlDocument dom = sendRequest(buildTBMXML(TBMDividendInfoRequest));
            return dom.InnerXml;

        }

        public string getDetailInfo()
        {

            XmlDocument dom = sendRequest(buildTBMXML(TBMDetailInfoRequest));
            return dom.InnerXml;

        }

        public string getCompositionInfo(int maxcount, string lastid)
        {

            XmlDocument dom = sendRequest(buildTBMXML(String.Format(TBMCompositionInfoRequest,maxcount.ToString(),lastid)));
            return dom.InnerXml;

        }

        public string getTBMSingleQuoteHistoryXML(string exchangeid, string tickerid)
        {

            string requestXML = buildTBMXML(String.Format(TBMSingleQuoteHistoryRequest, exchangeid, tickerid, TickerType, this.StartDate.ToString("yyyy/MM/dd"),this.EndDate.ToString("yyyy/MM/dd")));
            XmlDocument dom = sendRequest(requestXML);
            return dom.InnerXml;
        }

        public void storeQuoteCollection()
        {
            string companyname = "";
            string issueexchange = "";
            string issuename = "";
            string issueid = "";
            string quotecurrency = "";
            string quotedate = "";
            string lastquote = "";
            string openquote = "";
            string closedquote = "";
            string highquote = "";
            string lowquote = "";
            string previousdate = "";
            string previousquote = "";

            StreamWriter exportstream = File.CreateText("c:\\temp\\prices.txt");

            XmlTextReader reader = sendRequestXml(buildTBMXML(TBMQuoteCollectionRequest));
            //XmlTextReader reader = new XmlTextReader("c:\\temp\\sampletbm.xml");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Organisation")
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
                        {
                            reader.MoveToAttribute("long");
                            companyname = reader.Value;
                        }
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Issue")
                        {
                            reader.MoveToAttribute("exchange");
                            issueexchange = reader.Value;
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
                                {
                                    reader.MoveToAttribute("long");
                                    issuename = reader.Value;
                                }
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Ticker")
                                {
                                    reader.MoveToAttribute("id");
                                    issueid = reader.Value;
                                }
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Quote")
                                {
                                    reader.MoveToAttribute("date");
                                    quotedate = reader.Value;
                                    reader.MoveToAttribute("currency");
                                    quotecurrency = reader.Value;
                                    while (reader.Read())
                                    {
                                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Open")
                                        {
                                            reader.MoveToAttribute("value");
                                            openquote = reader.Value;
                                        }
                                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Close")
                                        {
                                            reader.MoveToAttribute("value");
                                            closedquote = reader.Value;
                                        }
                                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "High")
                                        {
                                            reader.MoveToAttribute("value");
                                            highquote = reader.Value;
                                        }
                                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Low")
                                        {
                                            reader.MoveToAttribute("value");
                                            lowquote = reader.Value;
                                        }
                                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Last")
                                        {
                                            reader.MoveToAttribute("value");
                                            lastquote = reader.Value;
                                        }
                                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Previous")
                                        {
                                            reader.MoveToAttribute("value");
                                            previousquote = reader.Value;
                                            reader.MoveToAttribute("date");
                                            previousdate = reader.Value;
                                        }
                                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Quote")
                                        {
                                            exportstream.WriteLine(String.Format("{0};{1}",issueid,issuename));
                                            
                                            IDalSession session = NHSessionFactory.CreateSession();

                                            try
                                            {
                                                IList<ITradeableInstrument> instrumentlist = InstrumentMapper.GetInstrumentsByIsin(session, issueid);

                                                if (instrumentlist.Count > 0)
                                                {
                                                    CultureInfo culture = new CultureInfo(CultureInfo.CurrentCulture.Name);

                                                    NumberFormatInfo numInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));
                                                    numInfo.NumberDecimalSeparator = ".";

                                                    TradeableInstrument instrument = (TradeableInstrument)instrumentlist[0];
                                                    decimal dLastQuote = decimal.Parse(lastquote,numInfo);
                                                    DateTime dtQuoteDate = DateTime.Parse(quotedate);

                                                    ICurrency newcur = (ICurrency)InstrumentMapper.GetCurrencyByName(session, quotecurrency);
                                                    Price newprice = new Price(new Money(dLastQuote, newcur), instrument);
                                                    IHistoricalPrice newhprice = new HistoricalPrice(newprice, dtQuoteDate);
                                                    if (!instrument.HistoricalPrices.ContainsHistoricalPrice(newhprice))
                                                    {
                                                        instrument.HistoricalPrices.AddHistoricalPrice(newhprice);
                                                    }
                                                    else
                                                    {
                                                        IHistoricalPrice existinghprice = (IHistoricalPrice)instrument.HistoricalPrices.GetItemByDate(dtQuoteDate);
                                                        existinghprice.Price = newprice;
                                                    }
                                                    session.Update(instrument);

                                                }
                                                session.Close();
                                            }
                                            catch (Exception ex)
                                            {
                                                break;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Organisation")
                        break;
                }
            }
            exportstream.Close();
        }

        private bool readMetaData(XmlTextReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "MetaData")
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Event")
                    {
                        reader.MoveToAttribute("severity");
                        if (reader.Value == "critical" || reader.Value == "fatal")
                            return true;
                    }
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "MetaData")
                    {
                        break;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Updates the quote history for the instrument with the given ISIN. This call takes
        /// into account the last known price for this instrument. Only newer prices will be
        /// retrieved from TBM.
        /// </summary>
        /// <param name="sISIN">ISIN code of the instrument</param>
        public void updateQuoteHistory(string sISIN)
        {
            // Get the instrument information
            IDalSession session = null;
            TradeableInstrument instrument = null;

            try
            {
                session = NHSessionFactory.CreateSession();
                IList<ITradeableInstrument> instrumentlist = InstrumentMapper.GetInstrumentsByIsin(session, sISIN);

                if (instrumentlist.Count > 0)
                {
                    instrument = (TradeableInstrument)instrumentlist[0];
                }
                // Look for the last date we have closing price for
                this.StartDate = new DateTime(1900, 1, 1);
                if (instrument.HistoricalPrices.Count > 0)
                {
                    this.StartDate = instrument.HistoricalPrices[instrument.HistoricalPrices.Count - 1].Date;
                    this.StartDate = this.StartDate.AddDays(1);
                }

                if (this.StartDate.Date == DateTime.Now.Date)
                    return;

                IList issuedetailslist = TBMMapper.GetIssueDetailsByIsin(session, sISIN);

                string sExchange = "AEX";
                if (issuedetailslist.Count > 0)
                {
                    sExchange = ((TBMIssueDetails)issuedetailslist[0]).ExchangeCode;
                }

                if (Util.DirectoryExists(TBMXmlTempDir, TBMXmlTempDir + TBMXmlTempDirNotExistsErrMessage))
                {
                    this.LogFile = String.Format(TBMXmlTempDir + "{0}_{1}-{2}.xml", sISIN, this.StartDate.ToString("yyyyMMdd"), this.EndDate.ToString("yyyyMMdd"));
                    updateSingleQuoteHistory(session, instrument, sISIN, sExchange);

                    Hashtable parameters = new Hashtable();
                    parameters.Add("InstrumentID", instrument.Key);
                    session.ExecuteStoredProcedure("EXEC dbo.TG_FillHistPricesWeekendsHolidays @p_intInstrumentID = :InstrumentID", parameters);
                }
            }
            finally
            {
                session.Close();
            }
            return;

        }

        /// <summary>
        /// Checks for missing prices in the history of prices
        /// </summary>
        public void CheckMissingHistoricalPrices()
        {
            IDalSession session = null;
            TradeableInstrument instrument = null;

            try
            {
                session = NHSessionFactory.CreateSession();
                // Get missingprices
                IList missinghistoricalprices = TBMMapper.GetMissingHistoricalPriceList(session);
                if (missinghistoricalprices.Count > 0)
                {
                    foreach (MissingHistoricalPrice mhp in missinghistoricalprices)
                    {
                        IList<ITradeableInstrument> instrumentlist = InstrumentMapper.GetInstrumentsByIsin(session, mhp.ISINCode);

                        if (instrumentlist.Count > 0)
                        {
                            instrument = (TradeableInstrument)instrumentlist[0];
                        }
                        // Look for the last date we have closing price for
                        this.StartDate = mhp.PriceDate;
                        this.EndDate = mhp.PriceDate;

                        IList issuedetailslist = TBMMapper.GetIssueDetailsByIsin(session, mhp.ISINCode);

                        string sExchange = "AEX";
                        if (issuedetailslist.Count > 0)
                        {
                            sExchange = ((TBMIssueDetails)issuedetailslist[0]).ExchangeCode;
                        }

                        if (Util.DirectoryExists(TBMXmlTempDir, TBMXmlTempDir + TBMXmlTempDirNotExistsErrMessage))
                        {
                            this.LogFile = String.Format(TBMXmlTempDir + "{0}_{1}.xml", mhp.ISINCode, mhp.PriceDate.ToString("yyyyMMdd"));
                            updateSingleQuoteHistory(session, instrument, mhp.ISINCode, sExchange);
                        }
                    }
                    session.Close();
                }
            }
            finally
            {
                session.Close();
            }
            return;

        }

        void updateSingleQuoteHistory(IDalSession session, TradeableInstrument instrument, string sISIN, string sExchange)
        {
            XmlTextReader reader = null;

            string companyname = "";
            string issueexchange = "";
            string issuename = "";
            string issueid = "";
            string quotecurrency = "";
            string quotedate = "";
            string lastquote = "";
            string openquote = "";
            string closedquote = "";
            string highquote = "";
            string lowquote = "";
            string previousdate = "";
            string previousquote = "";

            try
            {
                reader = sendRequestXml(buildTBMXML(String.Format(TBMSingleQuoteHistoryRequest, sExchange, sISIN, TickerType, this.StartDate.ToString("yyyy/MM/dd"), this.EndDate.ToString("yyyy/MM/dd"))));
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Organisation")
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
                            {
                                reader.MoveToAttribute("long");
                                companyname = reader.Value;
                            }
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Issue")
                            {
                                reader.MoveToAttribute("exchange");
                                issueexchange = reader.Value;
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
                                    {
                                        reader.MoveToAttribute("long");
                                        issuename = reader.Value;
                                    }
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Ticker")
                                    {
                                        reader.MoveToAttribute("id");
                                        issueid = reader.Value;
                                    }
                                    //if (readMetaData(reader))
                                    //   break;
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Quote")
                                    {
                                        reader.MoveToAttribute("date");
                                        quotedate = reader.Value;
                                        reader.MoveToAttribute("currency");
                                        quotecurrency = reader.Value;
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Open")
                                            {
                                                reader.MoveToAttribute("value");
                                                openquote = reader.Value;
                                            }
                                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Close")
                                            {
                                                reader.MoveToAttribute("value");
                                                closedquote = reader.Value;
                                            }
                                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "High")
                                            {
                                                reader.MoveToAttribute("value");
                                                highquote = reader.Value;
                                            }
                                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Low")
                                            {
                                                reader.MoveToAttribute("value");
                                                lowquote = reader.Value;
                                            }
                                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Last")
                                            {
                                                reader.MoveToAttribute("value");
                                                lastquote = reader.Value;
                                            }
                                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Previous")
                                            {
                                                reader.MoveToAttribute("value");
                                                previousquote = reader.Value;
                                                reader.MoveToAttribute("date");
                                                previousdate = reader.Value;
                                            }
                                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Quote")
                                            {
                                                // Only allow insert/update of a price when it exchange has closed
                                                if (closedquote.Length > 0)
                                                {

                                                    CultureInfo culture = new CultureInfo(CultureInfo.CurrentCulture.Name);

                                                    NumberFormatInfo numInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));
                                                    numInfo.NumberDecimalSeparator = ".";

                                                    DateTime dtQuoteDate = DateTime.Parse(quotedate);

                                                    ICurrency newcur = (ICurrency)InstrumentMapper.GetCurrencyByName(session, quotecurrency);

                                                    // For TBM Data the price is the closed price
                                                    decimal dClosedQuote = decimal.Parse(closedquote, numInfo);
                                                    Price newclosedprice = new Price(new Money(dClosedQuote, newcur), instrument);
                                                    IHistoricalPrice newhprice = new HistoricalPrice(newclosedprice, dtQuoteDate);
                                                    newhprice.ClosedPrice = newclosedprice;

                                                    Price newopenprice = null;
                                                    Price newhighprice = null;
                                                    Price newlowprice = null;

                                                    if (openquote.Length > 0)
                                                    {
                                                        decimal dOpenQuote = decimal.Parse(openquote, numInfo);
                                                        newopenprice = new Price(new Money(dOpenQuote, newcur), instrument);
                                                        newhprice.OpenPrice = newopenprice;
                                                    }
                                                    if (highquote.Length > 0)
                                                    {
                                                        decimal dHighQuote = decimal.Parse(highquote, numInfo);
                                                        newhighprice = new Price(new Money(dHighQuote, newcur), instrument);
                                                        newhprice.HighPrice = newhighprice;
                                                    }
                                                    if (lowquote.Length > 0)
                                                    {
                                                        decimal dLowQuote = decimal.Parse(lowquote, numInfo);
                                                        newlowprice = new Price(new Money(dLowQuote, newcur), instrument);
                                                        newhprice.LowPrice = newlowprice;
                                                    }
                                                    if (instrument.HistoricalPrices.ContainsHistoricalPrice(newhprice))
                                                    {
                                                        // Look for previous price to see if it differs too much

                                                        instrument.HistoricalPrices.AddHistoricalPrice(newhprice);

                                                        if (instrument.HistoricalPrices.Count > 1)
                                                        {
                                                            IHistoricalPrice prevhprice = (IHistoricalPrice)instrument.HistoricalPrices.GetItemByDate(dtQuoteDate.AddDays(-1));
                                                            decimal diff = Math.Abs((prevhprice.Price.Quantity - newhprice.Price.Quantity) / prevhprice.Price.Quantity) * 100;
                                                            if (diff > 10)
                                                                // More than 10% difference, raise warning
                                                                throw new ApplicationException(String.Format("Price of instrument: {0} differs more than 10%. Old price: {1}, New price {2}, difference {3}", instrument.Isin, prevhprice.Price, newhprice.Price, diff));
                                                        }

                                                    }
                                                    else
                                                    {
                                                        IHistoricalPrice existinghprice = (IHistoricalPrice)instrument.HistoricalPrices.GetItemByDate(dtQuoteDate);
                                                        existinghprice.Price = newclosedprice;
                                                        existinghprice.ClosedPrice = newclosedprice;
                                                        existinghprice.OpenPrice = newopenprice;
                                                        existinghprice.HighPrice = newhighprice;
                                                        existinghprice.LowPrice = newlowprice;
                                                    }
                                                    session.Update(instrument);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Organisation")
                                break;
                        }
                    }
                }
            }
            finally
            {
                // Unknown ISIN or error retrieving one
                if (reader != null)
                    reader.Close();
            }
            return;

        }


        protected double FormatDouble(string value, int nDecimals)
        {
            CultureInfo culture = new CultureInfo(CultureInfo.CurrentCulture.Name);

            NumberFormatInfo numInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));
            numInfo.NumberDecimalDigits = nDecimals;
            numInfo.NumberDecimalSeparator = ".";

            double output = Convert.ToDouble(value, numInfo);

            return output;

        }
    }
}

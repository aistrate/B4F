using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Jobs;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Communicator.Currencies
{   
    /// <summary>
    /// Class that is responsible for reading Currency rates from European Central Bank.
    /// Periodically an XML formatted stream can be requested from the ECB site containing the currency exchange rates.
    /// This XML stream is parsed and the exchange rates are being stored in the database.
    /// </summary>
    public class ECBRates : AgentWorker
    {
        public const string ECBWebSite = "http://www.ecb.int/stats/eurofxref/eurofxref-hist.xml";


        /// <summary>
        /// Loads the exchange rates that have not been read into the database yet.
        /// </summary>
        /// <param name="DataSession">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <returns>true if successful else false</returns>
        public bool LoadExRates(IDalSession DataSession)
        {
            return LoadExRates(DataSession, DateTime.MinValue);
        }
        
        /// <summary>
        /// Loads the exchange rates that have not been read into the database yet.
        /// </summary>
        /// <param name="DataSession">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="startDate">The date from which we are doing the import/update</param>
        /// <returns>true if successful else false</returns>
        public bool LoadExRates(IDalSession DataSession, DateTime startDate)
        {
            Dictionary<DateTime, List<exRateDate>> theDates = new Dictionary<DateTime, List<exRateDate>>();
            XmlReader TheReader = null;
            bool continueReading = true;
            DateTime MaxRateDate = HistoricalExRateMapper.GetMaxHistoricalExRateDate(DataSession);
            if (Util.IsNotNullDate(startDate) && MaxRateDate > startDate)
                MaxRateDate = startDate;

            if (InitReader(ref TheReader))
            {
                while ((TheReader.HasAttributes) && (TheReader.MoveToFirstAttribute()) && continueReading)
                {
                    continueReading = ReadinDate(TheReader, theDates, MaxRateDate);
                    TheReader.Read();
                }
                ExRateUpdate totalGiro = new ExRateUpdate( theDates);
                totalGiro.UpdateExRates(DataSession);
            }
            
            return true;
        }

        private bool InitReader(ref XmlReader reader)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            reader = XmlReader.Create(ECBWebSite, settings);

            if (reader.IsStartElement() && reader.ReadToDescendant("Cube") && reader.Read())
                return true;
            else
                return false;
        }

        private bool ReadinDate(XmlReader reader, Dictionary<DateTime, List<exRateDate>> theDates, DateTime LastReadDate)
        {
            DateTime rateDate = reader.ReadContentAsDateTime();
            if (LastReadDate.CompareTo(rateDate) < 0)
            {
                List<exRateDate> theRates = new List<exRateDate>();
                exRateDate newKey;
                Decimal theRate = 0m;
                while (reader.Read() && reader.HasAttributes)
                {
                    if (reader.MoveToAttribute("currency"))
                    {
                        newKey = new exRateDate(rateDate, reader.ReadContentAsString());
                        if (reader.MoveToAttribute("rate"))
                        {
                            try
                            {
                                theRate = reader.ReadContentAsDecimal();
                            }
                            catch
                            {
                                theRate = 0m;
                            }
                        }
                        newKey.rate = theRate;
                        theRates.Add(newKey);
                    }
                }
                theDates.Add(rateDate, theRates);
                return true;
            }
            else return false;
        }


        #region AgentWorker

        public override WorkerResult Run(IDalSessionFactory factory, System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                IDalSession session = factory.CreateSession();
                DateTime startDate = DateTime.Now;

                if (LoadExRates(session))
                {
                    string result = "The exchangerates were imported successfully";
                    e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Ok, result);
                }
            }
            catch (Exception ex)
            {
                e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Exception, "An error occured during the retrieval of the exchangerates", "", ex);
            }
            finally
            {
                worker.ReportProgress(100);
            }
            return (WorkerResult)e.Result;
        }

        #endregion
    }
}

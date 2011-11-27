using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.ExRates;

namespace B4F.TotalGiro.Communicator.Currencies
{
    /// <summary>
    /// Class that implements the currency exchange rate input through e-mail
    /// </summary>
    public class XeNetImport
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileToImport">File (saved email) to parse for currency exchange rates</param>
        public XeNetImport(FileInfo FileToImport)
        {
            this.FileToImport = FileToImport;
        }

        /// <summary>
        /// Loads the currency exchange rates from the file into the database
        /// </summary>
        /// <param name="theSession">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <returns>A list of all known (stored) currencies</returns>
        public IList<ICurrency> ImportFile(IDalSession theSession)
        {
            if (this.FileToImport.Exists)
            {
                //Grab the collection of known Currencies
                theCurrencies = InstrumentMapper.GetCurrencies(theSession);
                
                StreamReader sr = this.FileToImport.OpenText();
                string text = "";

                while (stripFile(sr, ref text, FIRST_HEADER))
                {
                    currencyDate = getCurrencyDate(text);
                    if (stripFile(sr, ref text, FIRST_REAL_UNIT))
                    {
                        processLines(text, sr, currencyDate);
                        //theDataLayer.DalSession.InsertOrUpdate(theCurrencies);
                    }
                }

            }
            return theCurrencies;
        }


        private bool stripFile(StreamReader sr, ref string text, string compareText)
        {
            for (; ((text != null) && (String.Compare(compareText, 0, text, 0, compareText.Length)) != 0); text = sr.ReadLine())
            { }
            return ((text != null) && (String.Compare(compareText, 0, text, 0, compareText.Length) == 0));
        }

        private DateTime getCurrencyDate(string text)
        {
            int i = 0;
            do
            {
                i++;
            } while ((text.Substring(i, 2) != "20"));

            return new DateTime(int.Parse(text.Substring(i, 4)), int.Parse(text.Substring(i + 5, 2)), int.Parse(text.Substring(i + 8, 2)));
        }

        private bool processLines(string text, StreamReader sr, DateTime currencyDate)
        {
            while ((text != null) && processLine(text, currencyDate))
            {
                text = sr.ReadLine();
            }
            return true;
        }

        private bool processLine(string text, DateTime currencyDate)
        {
            Currency loadedCurrency = null;
            string[] theParts = text.Split(new char[] { ' ' });
            if ((theParts[0].Length == 3) && ((theParts[0])[0] != ' '))
            {
                if (isCurrencyLoaded(theParts[0], ref loadedCurrency))
                {
                    HistoricalExRate newRate = new HistoricalExRate(loadedCurrency, Decimal.Parse(theParts[theParts.GetUpperBound(0)]), currencyDate, 0m, 0m, 1m);
                    if (!(loadedCurrency.HistoricalExRates.ContainsExRate(newRate)))
                    {
                        loadedCurrency.HistoricalExRates.AddExRate(newRate);
                    }
                }
                Console.WriteLine(theParts[0].ToString());
                return true;
            }
            return false;
        }

        private bool isCurrencyLoaded(string Symbol, ref Currency LoadedCurrency)
        {
            foreach (Currency c in theCurrencies)
            {
                if (c.Symbol == Symbol)
                {
                    LoadedCurrency = c;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets/sets the file (complete path) with exchange rates.
        /// </summary>
        public FileInfo FileToImport
        {
            get { return fileToImport; }
            set { fileToImport = value; }
        }

        #region Private Variables

        private FileInfo fileToImport;
        private const string FIRST_HEADER = "Rates as of ";
        private const string FIRST_REAL_UNIT = "AFA";
        private const string BLOCK_FOOTER = "First on the list above are the top ten";
        private DateTime currencyDate = new DateTime(1900, 01, 01);
        IList<ICurrency> theCurrencies;

        #endregion

    }
}

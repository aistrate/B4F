using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Communicator.FSInterface
{
    /// <summary>
    /// Class to hold Fund Settle export file information such as location, Fund Settle number, creation date of the file, etc.
    /// </summary>
	public class FSExportFile : IFSExportFile
	{
        const int EXCH_FUNDSETTLE = 1; // This is the FundSettle exchange id in the database carefull!!

		#region Properties

        /// <summary>
        /// Key of the file, this is an auto generated key, readonly
        /// </summary>
		public virtual int Key
		{
			get { return fileId; }
			set { fileId = value; }
		}

        /// <summary>
        /// The full path of the file, without name and extention.
        /// </summary>
		public virtual string FilePath
		{
			get { return filePath; }
			set { filePath = value; }
		}

        /// <summary>
        /// The name of the file, without extention.
        /// </summary>
		public virtual string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

        /// <summary>
        ///  The extention of the file.
        /// </summary>
		public virtual string FileExt
		{
			get { return fileExt; }
			set { fileExt = value; }
		}

        /// <summary>
        /// The full path of the file including the name
        /// </summary>
        public virtual string FullName
        {
            get { return String.Format("{0}{1}{2}{3}", this.FilePath, this.FileName, this.Key.ToString(), this.FileExt); }
        }

        /// <summary>
        /// The number of the order batch as it is known to Fund Settle
        /// </summary>
		public virtual string FSNumber
		{
			get { return fsNumber; }
			set { fsNumber = value; }
		}

        /// <summary>
        /// Date and time of creation of the file.
        /// </summary>
		public virtual DateTime CreationDate
		{
			get { return creationDate; }
			set { creationDate = value; }
		}

        /// <summary>
        /// Date and time of the actual sending of the file to Fund Settle.
        /// </summary>
		public virtual DateTime SentDate
		{
			get { return sentDate; }
			set { sentDate = value; }
		}

        /// <summary>
        /// The seperator that is used by Fund Settle during the parsing of the export file.
        /// </summary>
		public virtual char Seperator
		{
			get { return seperator; }
			set { seperator = value; }
		}
	
		#endregion

		#region Methods

        public virtual void DeleteExportFile()
		{
			File.Delete(this.FullName);
		}

        public virtual bool CreateExportFile()
		{
            bool retVal = false;

            exportstream = File.CreateText(this.FullName);
            exportstream.Write("Account number");
            exportstream.Write(seperator);
            exportstream.Write("Order type");
			exportstream.Write(seperator);
			exportstream.Write("ISIN/Common code");
			exportstream.Write(seperator);
			exportstream.Write("Participant order reference");
			exportstream.Write(seperator);
			exportstream.Write("Cash settlement date");
			exportstream.Write(seperator);
			exportstream.Write("Number of shares");
			exportstream.Write(seperator);
			exportstream.Write("Maximum gross cash amount");
			exportstream.Write(seperator);
			exportstream.Write("Payment currency");
			exportstream.Write(seperator);
			exportstream.Write("By order of client reference");
			exportstream.Write(seperator);
			exportstream.Write("In favor of client reference");
			exportstream.Write(seperator);
			exportstream.Write("Certification completed");
			exportstream.Write(seperator);
			exportstream.Write("Dividend policy");
			exportstream.Write(seperator);
			exportstream.Write("Registered in the name of");
			exportstream.Write(seperator);
			exportstream.Write("Commission recipient 1");
			exportstream.Write(seperator);
			exportstream.Write("Commission type 1");
			exportstream.Write(seperator);
			exportstream.Write("Commission amount 1");
			exportstream.Write(seperator);
			exportstream.Write("Commission currency 1");
			exportstream.Write(seperator);
			exportstream.Write("Commission recipient 2");
			exportstream.Write(seperator);
			exportstream.Write("Commission type 2");
			exportstream.Write(seperator);
			exportstream.Write("Commission amount 2");
			exportstream.Write(seperator);
			exportstream.Write("Commission currency 2");
			exportstream.Write(seperator);
			exportstream.Write("Commission recipient 3");
			exportstream.Write(seperator);
			exportstream.Write("Commission type 3");
			exportstream.Write(seperator);
			exportstream.Write("Commission amount 3");
			exportstream.Write(seperator);
			exportstream.Write("Commission currency 3");
			exportstream.Write(seperator);
			exportstream.Write("ISIN/Common code TO");
			exportstream.Write(seperator);
			exportstream.Write("Dividend policy TO");
			exportstream.Write(seperator);
			exportstream.Write("Narrative to FundSettle");
			exportstream.Write(seperator);
			exportstream.WriteLine("Narrative to the Transfer agent");

            int iCounter = 0;
			foreach (Order order in ExportedOrders)
			{
                // do checks
                if (!order.IsSecurity)
                    throw new ApplicationException(string.Format("It is impossible to send a order to fundsettle for {0}.", order.RequestedInstrument.Name));

                if (((ISecurityOrder)order).TradedInstrument.DefaultExchange == null)
                    throw new ApplicationException(string.Format("The default exchange for instrument {0} can not be null, contact your system administrator.", order.RequestedInstrument.Name));

                // FS Account number (Participant ID) - hardcoded for now...
                exportstream.Write("11419");
                exportstream.Write(seperator);

                // Order type [SUB,REDM,SWIC]
				if (order.Side == Side.Buy)
					exportstream.Write("SUBS");
				else
					exportstream.Write("REDM");

				exportstream.Write(seperator);

				// ISIN/Common code [12c]
				string isin = "";
				if (order is ISecurityOrder)
					isin = ((ISecurityOrder)order).TradedInstrument.Isin;
				exportstream.Write(isin);
				exportstream.Write(seperator);

				// Participant order reference [16x]
				exportstream.Write(order.Key);
				exportstream.Write(seperator);

				// ***OPTIONAL***
				// In case of subscription
				// cash settlement date, blank is orderdate
				// exportstream.Write(DateTime.Today.ToString("yyyy/mm/dd");
				exportstream.Write(seperator);

                // Determine how many decimals are allowed for this fund at FundSettle
                int fsexchangeid = EXCH_FUNDSETTLE;
                if (order is ISecurityOrder)
                    fsexchangeid = ((ISecurityOrder)order).TradedInstrument.DefaultExchange.Key;
                IInstrumentExchange instrumentexchange = ((ITradeableInstrument)order.RequestedInstrument).InstrumentExchanges.GetItemByExchange(fsexchangeid);

                int nofdecimals = 6;
                if (instrumentexchange != null)
                    nofdecimals = instrumentexchange.NumberOfDecimals;

                // Always use quanitity of order and sells are noted as a negative number
                // We have to correct this for FundSettle
                decimal ordervalue = order.PlacedValue.Quantity;
                if (order.Side == Side.Sell)
                    ordervalue *= -1;

				if (order.IsSizeBased)
                    // Number of shares [10n,12n]
                    exportstream.Write(FormatFS(ordervalue, 22, nofdecimals));
				exportstream.Write(seperator);

				if (!order.IsSizeBased)
					// Maximum gross cash amount [10n,12n]
                    exportstream.Write(FormatFS(ordervalue, 22, 2));
				exportstream.Write(seperator);

				// ***OPTIONAL***
				//Payment currency [3a]
                string curr = ((ISecurityOrder)order).TradedInstrument.CurrencyNominal.Symbol;
				exportstream.Write(curr);
				exportstream.Write(seperator);

				// ***OPTIONAL***
				// By order of client reference [15x]
				exportstream.Write(seperator);

				// ***OPTIONAL***
				// In favour of client reference [15x]
				exportstream.Write(seperator);

				// ***OPTIONAL***
				// Certification completed [N,Y,P]
                // TODO: check instrument if this is necessary
                if (instrumentexchange != null) 
                {
                    if (instrumentexchange.CertificationRequired)
                        exportstream.Write('Y');
                    else
                        exportstream.Write('N');
                }
				exportstream.Write(seperator);

				// Dividend policy [CASH,DRIP]
                if (instrumentexchange != null)
                    exportstream.Write(instrumentexchange.DividendPolicy);
                exportstream.Write(seperator);

				// Registered in name of [255x]
                if (instrumentexchange != null)
                    exportstream.Write(instrumentexchange.RegisteredInNameOf);
				exportstream.Write(seperator);

				// Commission recipient 1 [140x]
                if (instrumentexchange != null)
                    exportstream.Write(instrumentexchange.CommissionRecipientName);
				exportstream.Write(seperator);

				//	Commission type 1
				exportstream.Write(seperator);

				//	Commission amount 1
				exportstream.Write(seperator);

				//	Commission currency 1
				exportstream.Write(seperator);

				//	Commission recipient 2
				exportstream.Write(seperator);

				//	Commission type 2
				exportstream.Write(seperator);

				//	Commission amount 2
				exportstream.Write(seperator);

				//	Commission currency 2
				exportstream.Write(seperator);

				//	Commission recipient 3
				exportstream.Write(seperator);

				//	Commission type 3
				exportstream.Write(seperator);

				//	Commission amount 3
				exportstream.Write(seperator);

				//	Commission currency 3
				exportstream.Write(seperator);

				//	ISIN/Common code TO
				exportstream.Write(seperator);

				//	Dividend policy TO
				exportstream.Write(seperator);

				//	Narrative to FundSettle
				exportstream.Write(seperator);

				//	Narrative to Transfer Agent
                if (iCounter < ExportedOrders.Count-1)
				    exportstream.WriteLine();
                iCounter++;
			}

			exportstream.Close();
            retVal = true;
            return retVal;
		}

		protected string FormatFS(decimal value, int nTotal, int nDecimals)
		{
			CultureInfo culture = new CultureInfo(CultureInfo.CurrentCulture.Name);

			NumberFormatInfo numInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));
			numInfo.NumberDecimalDigits = nDecimals;
			numInfo.NumberDecimalSeparator = ".";

            string decimals = new string('0',nDecimals);
            string format = "0." + decimals; 

			string output = value.ToString(format, numInfo);

            return output;
/*
            int posDecimal = output.IndexOf(numInfo.NumberDecimalSeparator);
			string sNumber = output.Substring(0, posDecimal);
			sNumber = sNumber.PadLeft(nTotal - nDecimals, '0');
			string sDecimals = output.Substring(posDecimal + 1);
			sDecimals = sDecimals.PadRight(nDecimals, '0');

			return String.Concat(sNumber, sDecimals);
*/

		}
		/// <summary>
		/// Orders is a property to allow NHibernate to access for DataStore purposes only!
		/// </summary>
		public virtual IList Orders
		{
			get { return orders; }
			set
			{
				orders = value;
				ExportedOrders = new FSExportedOrderList(this, Orders);
			}
		}

		public virtual FSExportedOrderList ExportedOrders
		{
			get { if (exportedOrders == null)
				     exportedOrders = new FSExportedOrderList(this,Orders);
				  return exportedOrders; }
			protected internal set { exportedOrders = value; }
		}

		#endregion

		#region Private variables

		private int fileId;
		private string filePath = "c:\\temp\\";
		private string fileName = "fsexport";
		private string fileExt = ".csv";
		private string fsNumber;
		private DateTime creationDate = DateTime.Now;
		private DateTime sentDate = DateTime.Now;
		private IList orders = new ArrayList();
		private FSExportedOrderList exportedOrders;
		private StreamWriter exportstream;
		private char seperator = ',';

		#endregion

	}
}

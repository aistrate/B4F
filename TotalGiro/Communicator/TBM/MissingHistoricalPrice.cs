using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Communicator.TBM
{
    /// <summary>
    /// Class to hold a list of missing prices in the historical positions of TotalGiro
    /// </summary>
    public class MissingHistoricalPrice
    {
        private long historicalpositionid;
        private string isincode;
        private DateTime pricedate;


        public long HistoricalPositionId
        {
            get { return historicalpositionid; }
            set { historicalpositionid = value; }
        }

        public string ISINCode
        {
            get { return isincode; }
            set { isincode = value; }
        }

        public DateTime PriceDate
        {
            get { return pricedate; }
            set { pricedate = value; }
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", isincode, pricedate.ToString("yyyyMMdd"));
        }
    }
}

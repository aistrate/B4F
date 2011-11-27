using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Instruments.ExRates
{
    public class HistoricalExRateCompKey : IExRate
    {
        protected HistoricalExRateCompKey() { }

        public virtual HistoricalInstrumentDataKey Key
        {
            get { return key; }
            protected set { key = value; }
        }

        #region IExRate Members

        public virtual ICurrency Currency
        {
            get { return currency; }
        }

        public virtual decimal Rate
        {
            get { return rate; }
            set { rate = value; }
        }

        public virtual DateTime RateDate
        {
            get { return key.Date; }
        }

        public virtual decimal Bid
        {
            get { return bid; }
        }

        public virtual decimal Ask
        {
            get { return ask; }
        }

        public virtual decimal PriceFactor
        {
            get { return priceFactor; }
        }

        public virtual decimal GetExRate()
        {
            return HistoricalExRate.CalculateExRate(Rate, Ask, Bid, PriceFactor, 0M);
        }

        public virtual decimal GetExRate(Side side)
        {
            return HistoricalExRate.CalculateExRate(Rate, Ask, Bid, PriceFactor, side);
        }

        public virtual bool IsOldDate
        {
            get { return Util.GetIsOldDate(RateDate, null); }
        }

        public virtual bool WasOldDateBy(DateTime referenceDate)
        {
            return (referenceDate - this.RateDate.Date).Days >= 3;
        }

        #endregion

        #region Private Variables

        private HistoricalInstrumentDataKey key;
        private ICurrency currency;
        private decimal rate;
        private DateTime rateDate;
        private decimal bid;
        private decimal ask;
        private decimal priceFactor;

        #endregion

    }
}

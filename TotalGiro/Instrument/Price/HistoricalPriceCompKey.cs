using System;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments.Prices
{
    public class HistoricalPriceCompKey : IPriceDetail
    {
        protected HistoricalPriceCompKey () { }

        public virtual HistoricalInstrumentDataKey Key
        {
            get { return key; }
            protected set { key = value; }
        }

        #region IPriceDetail Members

        public virtual Price Price
        {
            get { return price; }
            set { price = value; }
        }
	
        public virtual DateTime Date
        {
            get { return Key.Date; }
        }

        public virtual bool IsOldDate
        {
            get 
            {
                IExchangeHolidayCollection holidays = null;
                if (Price != null && Price.Instrument.IsTradeable)
                {
                    ITradeableInstrument instrument = (ITradeableInstrument)Price.Instrument;
                    if (instrument.HomeExchange != null)
                        holidays = instrument.HomeExchange.ExchangeHolidays;
                }
                return Util.GetIsOldDate(Date, holidays); 
            }
        }

        public virtual bool WasOldDateBy(DateTime referenceDate)
        {
            return (referenceDate - this.Date.Date).Days >= 3;
        }

        #endregion

        #region Privates

        private HistoricalInstrumentDataKey key;
        private Price price;

        #endregion
    }
}

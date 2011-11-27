using System;
using B4F.TotalGiro.Accounts.Portfolios;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Instruments.Prices;

namespace B4F.TotalGiro.Instruments.Nav
{
    public abstract class NavPosition : INavPosition
    {
        public NavPosition()
        {

        }

        public NavPosition(InstrumentSize Size, Price ClosingPriceUsed, Decimal ExchangeRateUsed,
            IPriceDetail ClosingPriceRecord, IExRate ExchangeRateRecord)
        {
            this.Size = Size;
            this.ClosingPriceUsed = ClosingPriceUsed;
            this.ExchangeRateUsed = ExchangeRateUsed;
            this.ClosingPriceRecord = ClosingPriceRecord;
            this.ExchangeRateRecord = ExchangeRateRecord;
            setCurrentValue();
            setCurrentBaseValue();
        }

        public NavPosition(InstrumentSize Size, Decimal ExchangeRateUsed, IExRate ExchangeRateRecord)
        {
            this.Size = Size;
            if (Size.Underlying.CurrentPrice != null)
                this.ClosingPriceUsed = Size.Underlying.CurrentPrice.Price.Clone(1m);
            this.ExchangeRateRecord = Size.Underlying.ToCurrency.ExchangeRate;
            this.ExchangeRateUsed = ExchangeRateUsed;
            this.ClosingPriceRecord = null;            
            setCurrentValue();
            setCurrentBaseValue();
        }

        public int Key { get; set; }
        public INavCalculation Parent { get; set; }
        public InstrumentSize Size { get; set; }
        public Price ClosingPriceUsed { get; set; }
        public decimal ExchangeRateUsed { get; set; }
        public IPriceDetail ClosingPriceRecord { get; set; }
        public IExRate ExchangeRateRecord { get; set; }

        public Money CurrentValue { get; set; }
        public Money CurrentBaseValue { get; set; }

        /// <summary>
        /// Is this a Security position
        /// </summary>
        public abstract bool IsSecurityPosition { get; }

        /// <summary>
        /// Is this a cash position
        /// </summary>
        public abstract bool IsCashPosition { get; }

        public abstract void setCurrentValue();
        public abstract void setCurrentBaseValue();


        public virtual IsLong Sign
        {
            get { return this.Size.Sign ? IsLong.Long : IsLong.Short; }
        }
    }
}

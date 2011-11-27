using System;
using System.Collections;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments
{
    public abstract class InstrumentsWithPrices : Instrument, IInstrumentsWithPrices
    {
        protected InstrumentsWithPrices() { }

        /// <summary>
        /// Get IsWithPrice flag
        /// </summary>
        public override bool IsWithPrice
        {
            get { return true; }
        }

        /// <summary>
        /// The pricing Type
        /// </summary>
        public virtual PricingTypes PriceType { get; set; }
        
        /// <summary>
        /// Get/set most actual price
        /// </summary>
        public override IPriceDetail CurrentPrice
        {
            get { return this.currentPrice; }
            set { this.currentPrice = value; }
        }

        /// <summary>
        /// Get/set currency where instrument is traded in
        /// </summary>
        public virtual ICurrency CurrencyNominal
        {
            get { return this.currencyNominal; }
            set { this.currencyNominal = value; }
        }

        /// <summary>
        /// Get/set ISIN-code
        /// </summary>
        public virtual string Isin
        {
            get { return this.isin; }
            set { this.isin = value; }
        }

        protected override bool validate()
        {
            return base.validate();
        }

        /// <summary>
        /// Get collection of historical prices of instrument
        /// </summary>
        public virtual IPriceDetailCollection HistoricalPrices
        {
            get
            {
                HistoricalPriceCollection items = (HistoricalPriceCollection)this.historicalPrices.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return items;
            }
        }

        #region Privates
        
        protected IPriceDetail currentPrice;
        private IDomainCollection<IPriceDetail> historicalPrices = new HistoricalPriceCollection();
        protected string isin;
        internal ICurrency currencyNominal;

        #endregion
        
    }
}

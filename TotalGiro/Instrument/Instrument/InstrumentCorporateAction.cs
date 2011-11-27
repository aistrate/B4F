using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Instruments
{
    public abstract class InstrumentCorporateAction : InstrumentsWithPrices, IInstrumentCorporateAction //, ITradeableInstrument
    {
        protected InstrumentCorporateAction()
        {
            initialize();
        }

        protected InstrumentCorporateAction(ISecurityInstrument underlying)
        {
            this.Underlying = underlying;
        }

        #region Props

        /// <summary>
	    /// The Underlying Instrument
	    /// </summary>
        public virtual ISecurityInstrument Underlying { get; set; }
        public virtual bool IsStockDividend 
        {
            get { return false; }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Get is instrument cash flag
        /// </summary>
        public override bool IsCash
        {
            get { return false; }
        }

        public override bool IsCorporateAction
        {
            get { return true; }
        }

        public virtual bool AllowNetting
        {
            get { return Underlying.AllowNetting; }
            //set { base.AllowNetting = value; }
        }

        public virtual string CompanyName
        {
            get { return Underlying.CompanyName; }
            //set { base.CompanyName = value; }
        }

        public virtual int ContractSize
        {
            get { return Underlying.ContractSize; }
        }

        public override ICountry Country
        {
            get { return Underlying.Country; }
            set { base.Country = value; }
        }

        public override ICurrency CurrencyNominal
        {
            get { return Underlying.CurrencyNominal; }
            set { base.CurrencyNominal = value; }
        }

        public override int DecimalPlaces
        {
            get { return Underlying.DecimalPlaces; }
            set { base.DecimalPlaces = value; }
        }

        public virtual IExchange DefaultExchange
        {
            get { return Underlying.DefaultExchange; }
            //set { base.DefaultExchange = value; }
        }

        public virtual string DefaultExchangeName
        {
            get { return Underlying.DefaultExchangeName; }
        }

        public virtual IRoute DefaultRoute
        {
            get { return Underlying.DefaultRoute; }
            //set { base.DefaultRoute = value; }
        }

        public virtual IExchange HomeExchange
        {
            get { return Underlying.HomeExchange; }
            //set { base.HomeExchange = value; }
        }

        public virtual IInstrumentExchangeCollection InstrumentExchanges
        {
            get { return Underlying.InstrumentExchanges; }
        }

        public override bool IsSecurity
        {
            get { return true; }
        }

        public override bool IsTradeable
        {
            get { return false; }
        }

        public override bool IsWithPrice
        {
            get { return true; }
        }

        public override PricingTypes PriceType
        {
            get { return Underlying.PriceType; }
            set { base.PriceType = value; }
        }

        public override decimal PriceTypeFactor
        {
            get { return Underlying.PriceTypeFactor; }
        }

        #endregion

        #region Methods

        public override bool Validate()
        {
            if (this.Underlying == null)
                throw new ApplicationException("The Underlying is mandatory for a corpa.");
            return base.validate();
        }

        /// <summary>
        /// Get the educated guess of the size of a instrument for a amount of money
        /// </summary>
        /// <param name="inputAmount"></param>
        /// <returns></returns>
        public override PredictedSize PredictSize(Money inputAmount)
        {
            PredictedSize retVal = new PredictedSize(PredictedSizeReturnValue.NoRate);
            Money amount;

            if (CurrentPrice != null)
            {
                retVal.RateDate = CurrentPrice.Date;
                if (inputAmount.Underlying.Equals(CurrentPrice.Price.Underlying))
                    retVal.Size = inputAmount.CalculateSize(CurrentPrice.Price);
                else
                {
                    amount = inputAmount.Convert(CurrentPrice.Price.Underlying);
                    retVal.Size = amount.CalculateSize(CurrentPrice.Price);
                }
                retVal.Rate = currentPrice.Price.ToString();
            }
            return retVal;
        }

        public override bool CalculateCosts(IOrder order, IFeeFactory feeFactory)
        {
            throw new ApplicationException("The method is mot supported for a corpa.");
        }


        public override bool CalculateCosts(IOrderAllocation transaction, IFeeFactory feeFactory, IGLLookupRecords lookups)
        {
            throw new ApplicationException("The method is mot supported for a corpa.");
        }


        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.Bond;
        }

        #endregion

        #region Privates


    	#endregion

    }
}
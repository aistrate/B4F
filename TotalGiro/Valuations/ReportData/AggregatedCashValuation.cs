using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Valuations
{
    public class AggregatedCashValuation: Valuation
    {
        #region Constructor

        internal AggregatedCashValuation(ValuationCollection cashValuations, bool aggregateToBase) 
        {
            bool hasInitialized = false;
            if (aggregateToBase)
            {
                foreach (IValuation valuation in cashValuations)
                {
                    if (valuation.Instrument.IsCash)
                    {
                        if (this.account == null)
                        {
                            this.account = valuation.Account;
                            this.date = valuation.Date;
                            this.key = valuation.Key;
                            this.marketRate = 1M;
                            this.avgOpenExRate = 1M;
                            baseCurrency = ((ICurrency)valuation.Instrument).BaseCurrency;
                            price = new Price(1M, baseCurrency, baseCurrency);
                            this.bookPrice = price;
                            this.costPrice = price;
                            this.marketPrice = price;
                            this.displayInstrumentsCategory = valuation.DisplayInstrumentsCategory;
                            this.AssetClass = valuation.ValuationMutation.AssetClass;
                            this.ValuationMutation = valuation.ValuationMutation;
                        }
                        this.size += valuation.BaseMarketValue;
                        this.bookValue += valuation.BookValue;
                        this.bookChange += valuation.BookChange;
                        this.deposit += valuation.Deposit;
                        this.withDrawal += valuation.WithDrawal;
                        if (!valuation.ValuationMutation.IsSecurityValuationMutation)
                        {
                            this.depositToDate += ((IMonetaryValuationMutation)valuation.ValuationMutation).DepositToDate;
                            this.withDrawalToDate += ((IMonetaryValuationMutation)valuation.ValuationMutation).WithDrawalToDate;
                        }
                        hasInitialized = true;
                    }
                }
            }
            if (!hasInitialized)
                throw new ApplicationException("Class AggregatedCashValuation could not be initialized");
        }


        #endregion

        #region Properties

        /// <summary>
        /// The Key of the valuation. It is the first valuation of the aggregation
        /// </summary>
        public override int Key
        {
            get { return key; }
        }

        /// <summary>
        /// The relevant account
        /// </summary>
        public override IAccountTypeInternal Account
        {
            get { return account; }
        }

        /// <summary>
        /// The relevant instrument
        /// </summary>
        public override IInstrument Instrument
        {
            get { return this.Size.Underlying; }
        }

        public new IAssetClass AssetClass { get; set; }

        /// <summary>
        /// The currency of the instrument
        /// </summary>
        public override ICurrency CurrencyNominal
        {
            get { return this.baseCurrency; }
        }

        /// <summary>
        /// The date of the valuation
        /// </summary>
        public override DateTime Date
        {
            get { return this.date; }
        }

        /// <summary>
        /// The Market price of the instrument. In instrument currency
        /// </summary>
        public override Price MarketPrice
        {
            get { return this.marketPrice; }
        }

        /// <summary>
        /// This is the MarketValue in instrument currency
        /// </summary>
        public override Money MarketValue
        {
            get { return Size.GetMoney(); }
        }

        /// <summary>
        /// This is the MarketValue in base currency.
        /// </summary>
        public override Money BaseMarketValue
        {
            get { return Size.GetMoney(); }
        }

        /// <summary>
        /// This is the market rate of the day.
        /// </summary>
        public override decimal MarketRate
        {
            get { return marketRate; }
        }

        /// <summary>
        /// The size of the instrument
        /// </summary>
        public override InstrumentSize Size
        {
            get { return size; }
        }

        /// <summary>
        /// The book price (Book Value / Size)
        /// </summary>
        public override Price BookPrice
        {
            get { return bookPrice; }
        }

        /// <summary>
        /// The Value that is paid for the current size. In base currency.
        /// </summary>
        public override Money BookValue
        {
            get { return bookValue; }
        }

        /// <summary>
        /// The change in Book Value. In base currency.
        /// </summary>
        public override Money BookChange
        {
            get { return bookChange; }
        }

        /// <summary>
        /// The average exchange rate used to create the position (only opening)
        /// </summary>
        public override decimal AvgOpenExRate
        {
            get { return avgOpenExRate; }
        }

        /// <summary>
        /// The amount that has been realised on this date. In instrument currency. (only for securities)
        /// </summary>
        public override Money RealisedAmount
        {
            get { return null; }
        }

        /// <summary>
        /// The amount that has been realised so far to this date. In instrument currency. (only for securities)
        /// </summary>
        public override Money RealisedAmountToDate
        {
            get { return null; }
        }

        /// <summary>
        /// The amount that has been realised so far to this date. In base currency. (only for securities)
        /// </summary>
        public override Money BaseRealisedAmountToDate
        {
            get { return null; }
        }

        /// <summary>
        /// This is the BaseMarketValue - BookValue. In Base Currency
        /// </summary>
        public override Money UnRealisedAmountToDate
        {
            get { return BaseMarketValue - BookValue; }
        }

        /// <summary>
        /// The cost price of the position. In instrument currency
        /// </summary>
        public override Price CostPrice
        {
            get { return costPrice; }
        }

        /// <summary>
        /// The amount deposited on this date. In base currency
        /// </summary>
        public override Money Deposit
        {
            get { return this.deposit; }
        }

        /// <summary>
        /// The amount deposited so far to this date. In base currency
        /// </summary>
        public virtual Money DepositToDate
        {
            get { return this.depositToDate; }
        }

        /// <summary>
        /// The amount withdrawn on this date. In base currency
        /// </summary>
        public override Money WithDrawal
        {
            get { return this.withDrawal; }
        }

        /// <summary>
        /// The amount withdrawn so far to this date. In base currency
        /// </summary>
        public virtual Money WithDrawalToDate
        {
            get { return this.withDrawalToDate; }
        }

        public override string DisplayInstrumentsCategory
        {
            get { return this.displayInstrumentsCategory; }
        }

        #endregion

        #region Privates

        private int key;
        private IAccountTypeInternal account;
        private DateTime date;
        private InstrumentSize size;
        private Price bookPrice;
        private Money bookValue;
        private Money bookChange;
        private decimal avgOpenExRate;
        private Price costPrice;
        private Price marketPrice;
        private decimal marketRate;
        private Money deposit;
        private Money depositToDate;
        private Money withDrawal;
        private Money withDrawalToDate;

        private ICurrency baseCurrency;
        private Price price;

        private string displayInstrumentsCategory;

        #endregion

    }
}

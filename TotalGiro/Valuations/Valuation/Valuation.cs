using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.Valuations
{
    public class Valuation : IValuation
    {
        /// <summary>
        /// The Key of the valuation. It is a description of the account + instrument + date
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// The relevant account
        /// </summary>
        public virtual IAccountTypeInternal Account
        {
            get { return this.account; }
            protected set { this.account = value; }
        }

        /// <summary>
        /// The relevant instrument
        /// </summary>
        public virtual IInstrument Instrument
        {
            get { return this.instrument; }
            protected set { this.instrument = value; }
        }

        /// <summary>
        /// The relevant instrument
        /// </summary>
        public virtual IAssetClass AssetClass
        {
            get { return this.ValuationMutation.AssetClass; }
        }

        /// <summary>
        /// The date of the valuation
        /// </summary>
        public virtual DateTime Date
        {
            get { return this.date; }
            protected set { this.date = value; }
        }

        public virtual IValuationMutation ValuationMutation
        {
            get { return this.valuationMutation; }
            protected set { this.valuationMutation = value; }
        }

        internal IPriceDetail HistoricalPrice
        {
            get { return this.historicalPrice; }
            set { this.historicalPrice = value; }
        }

        internal IHistoricalExRate HistoricalExRate
        {
            get { return this.historicalExRate; }
            set { this.historicalExRate = value; }
        }

        /// <summary>
        /// This is the MarketValue in instrument currency
        /// </summary>
        public virtual Money MarketValue
        {
            get { return this.marketValue; }
            protected set { this.marketValue = value; }
        }

        /// <summary>
        /// This is the MarketValue in base currency.
        /// </summary>
        public virtual Money BaseMarketValue
        {
            get { return baseMarketValue; }
            protected set { this.baseMarketValue = value; }
        }

        //public virtual IAverageHolding AverageHolding
        //{
        //    get { return averageHolding; }
        //    set { averageHolding = value; }
        //}

        public virtual IValuationCashMutation AccruedInterestCashMutation { get; protected set; }

        #region Derived Properties

        /// <summary>
        /// The currency of the instrument
        /// </summary>
        public virtual ICurrency CurrencyNominal
        {
            get { return ValuationMutation.InstrumentCurrency; }
        }

        /// <summary>
        /// The Market price of the instrument. In instrument currency
        /// </summary>
        public virtual Price MarketPrice
        {
            get 
            {
                if (Instrument.IsTradeable)
                    return HistoricalPrice.Price;
                else
                    return new Price(1M, CurrencyNominal, Instrument);
            }
        }

        /// <summary>
        /// This is the market rate of the day.
        /// </summary>
        public virtual decimal MarketRate
        {
            get 
            {
                if (!CurrencyNominal.IsBase)
                    return HistoricalExRate.Rate;
                else
                    return 1M;
            }
        }

        /// <summary>
        /// The size of the instrument
        /// </summary>
        public virtual InstrumentSize Size
        {
            get { return ValuationMutation.Size; }
        }

        /// <summary>
        /// The book price (Book Value / Size)
        /// </summary>
        public virtual Price BookPrice
        {
            get 
            {
                if (valuationMutation.IsSecurityValuationMutation)
                    return ((ISecurityValuationMutation)ValuationMutation).BookPrice;
                else
                    return null;
            }
        }

        /// <summary>
        /// The Value that is paid for the current size. In base currency.
        /// </summary>
        public virtual Money BookValue
        {
            get { return ValuationMutation.BookValue; }
        }

        /// <summary>
        /// The change in Book Value. In base currency.
        /// </summary>
        public virtual Money BookChange
        {
            get 
            {
                if (ValuationMutation.Date.Equals(Date))
                    return ValuationMutation.BookChange;
                else
                    return null;
            }
        }

        /// <summary>
        /// The average exchange rate used to create the position (only opening)
        /// </summary>
        public virtual decimal AvgOpenExRate
        {
            get { return ValuationMutation.AvgOpenExRate; }
        }

        /// <summary>
        /// The amount that has been realised on this date. In instrument currency.
        /// </summary>
        public virtual Money RealisedAmount
        {
            get 
            {
                if (ValuationMutation.IsSecurityValuationMutation && ValuationMutation.Date.Equals(Date))
                    return ((ISecurityValuationMutation)ValuationMutation).RealisedAmount;
                else
                    return null;
            }
        }

        /// <summary>
        /// The amount that has been realised so far to this date. In instrument currency.
        /// </summary>
        public virtual Money RealisedAmountToDate
        {
            get
            {
                if (ValuationMutation.IsSecurityValuationMutation)
                    return ((ISecurityValuationMutation)ValuationMutation).RealisedAmountToDate;
                else
                    return null;
            }
        }

        /// <summary>
        /// The amount that has been realised so far to this date. In base currency.
        /// </summary>
        public virtual Money BaseRealisedAmountToDate
        {
            get 
            {
                if (RealisedAmountToDate == null)
                    return null;
                else
                {
                    if (MarketRate != 0)
                        return RealisedAmountToDate.Convert(1 / MarketRate, (ICurrency)BookValue.Underlying);
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// This is the BaseMarketValue - BookValue. In Base Currency
        /// </summary>
        public virtual Money UnRealisedAmountToDate
        {
            get { return BaseMarketValue - BookValue; }
        }

        /// <summary>
        /// The cost price of the position. In instrument currency
        /// </summary>
        public virtual Price CostPrice
        {
            get
            {
                if (ValuationMutation.IsSecurityValuationMutation)
                    return ((ISecurityValuationMutation)ValuationMutation).CostPrice;
                else
                    return null;
            }
        }

        /// <summary>
        /// The amount deposited on this date. In base currency
        /// </summary>
        public virtual Money Deposit
        {
            get
            {
                if (ValuationMutation.Date.Equals(Date))
                    return ValuationMutation.TransferInToday;
                else
                    return null;
            }
        }

        /// <summary>
        /// The amount withdrawn on this date. In base currency
        /// </summary>
        public virtual Money WithDrawal
        {
            get
            {
                if (ValuationMutation.Date.Equals(Date))
                    return ValuationMutation.TransferOutToday;
                else
                    return null;
            }
        }

        public virtual string DisplayInstrumentsCategory
        {
            get { return this.ValuationMutation.DisplayInstrumentsCategory; }
        }

        #endregion

        #region Overrides

        //public override bool Equals(object obj)
        //{
        //    if (obj is Valuation)
        //    {
        //        Valuation temp = (Valuation)obj;
        //        if (temp.Instrument.Equals(Instrument) && temp.Date.Equals(Date))
        //            return true;
        //    }
        //    throw new ApplicationException("Fuckup!!");
        //}

        public override string ToString()
        {
            if (Instrument == null || Util.IsNullDate(Date))
                return base.ToString();
            else
            {
                string retVal = Date.ToShortDateString() + " " + Instrument.DisplayName;
                if (Size != null)
                    retVal += " " + Size.Quantity.ToString();
                return retVal;
            }
        }

        #endregion

        #region Privates

        private int key;
        private IAccountTypeInternal account;
        private IInstrument instrument;
        private DateTime date;
        private IValuationMutation valuationMutation;
        private IPriceDetail historicalPrice;
        private IHistoricalExRate historicalExRate;
        private Money marketValue;
        private Money baseMarketValue;
        //private IAverageHolding averageHolding;
        
        #endregion

    }
}

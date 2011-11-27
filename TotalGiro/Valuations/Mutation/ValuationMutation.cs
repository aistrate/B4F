using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;

namespace B4F.TotalGiro.Valuations
{
    public abstract class ValuationMutation : IValuationMutation
    {
        #region Constructors

        protected ValuationMutation() { }

        /// <summary>
        /// Creates a ValuationMutation from a previous ValuationMutation (with older date)
        /// </summary>
        /// <param name="mutationDate">The date for the new valuation mutation</param>
        /// <param name="prevMutation">The previous mutation</param>
        internal ValuationMutation(DateTime mutationDate, IValuationMutation prevMutation)
        {
            if (prevMutation != null)
            {
                this.Date = mutationDate;
                this.PreviousMutation = prevMutation;
                this.Account = prevMutation.Account;
                this.Size = prevMutation.Size;
                this.BookValue = prevMutation.BookValue;
                this.BookValueIC = prevMutation.BookValueIC;
                this.RealisedCurrencyGainToDate = prevMutation.RealisedCurrencyGainToDate;
                this.AvgOpenExRate = prevMutation.AvgOpenExRate;
                this.InstrumentCurrency = prevMutation.InstrumentCurrency;
            }
            else
                throw new ApplicationException("Previous valuation can not be null");
        }

        #endregion

        #region Methods

        public virtual bool Validate()
        {
            if (PreviousMutation == null)
            {
                if ((Size != null && Size.IsNotZero) || (BookChange != null && BookChange.IsNotZero) ||
                    (TransferInToday != null || TransferInToday.IsNotZero) || (TransferOutToday != null || TransferOutToday.IsNotZero))
                    this.IsValid = true;
            }
            else
                this.IsValid = true;

            if (BookValue != null) BookValue = BookValue.Round();
            if (BookValueIC != null) BookValueIC = BookValueIC.Round();
            if (BookChange != null) BookChange = BookChange.Round();
            if (BookChangeIC != null) BookChangeIC = BookChangeIC.Round();
            if (RealisedCurrencyGain != null) RealisedCurrencyGain = RealisedCurrencyGain.Round();
            if (RealisedCurrencyGainToDate != null) RealisedCurrencyGainToDate = RealisedCurrencyGainToDate.Round();
            if (TransferInToday != null) TransferInToday = TransferInToday.Round();
            if (TransferOutToday != null) TransferOutToday = TransferOutToday.Round();
            AvgOpenExRate = Math.Round(AvgOpenExRate, 7);
            return this.IsValid;

        }

        #endregion

        #region Valuation Props

        /// <summary>
        /// The Key of the ValuationMutation
        /// </summary>
        public virtual long Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// A pointer to the previous mutation
        /// </summary>
        public virtual IValuationMutation PreviousMutation
        {
            get { return prevMutation; }
            internal set { prevMutation = value; }
        }

        /// <summary>
        /// A pointer to the converted mutation
        /// </summary>
        public virtual IValuationMutation ConvertedMutation
        {
            get { return convertedMutation; }
            set { convertedMutation = value; }
        }

        /// <summary>
        /// The relevant account
        /// </summary>
        public virtual IAccountTypeInternal Account
        {
            get { return account; }
            set { account = value; }
        }

        /// <summary>
        /// The relevant instrument
        /// </summary>
        public virtual IInstrument Instrument
        {
            get { return Size.Underlying; }
        }

        /// <summary>
        /// Is this a valid valuation (does it mean anything).
        /// </summary>
        public virtual bool IsValid
        {
            get { return isValid; }
            set { isValid = value; }
        }

        /// <summary>
        /// The date of the mutation
        /// </summary>
        public virtual DateTime Date
        {
            get { return mutationDate; }
            set { mutationDate = value; }
        }

        /// <summary>
        /// The historical size after the mutation
        /// </summary>
        public virtual InstrumentSize Size
        {
            get { return this.size; }
            set {this.size = value; }
        }

        /// <summary>
        /// The Value that is paid for the current size. In base currency.
        /// </summary>
        public virtual Money BookValue
        {
            get { return this.bookValue; }
            set { this.bookValue = value; }
        }

        /// <summary>
        /// The Value that is paid for the current size. In instrument currency.
        /// </summary>
        public virtual Money BookValueIC
        {
            get { return this.bookValueIC; }
            set { this.bookValueIC = value; }
        }

        /// <summary>
        /// The change in Book Value. In base currency.
        /// </summary>
        public virtual Money BookChange
        {
            get { return this.bookChange; }
            set { this.bookChange = value; }
        }

        /// <summary>
        /// The change in Book Value. In instrument currency.
        /// </summary>
        public virtual Money BookChangeIC
        {
            get { return this.bookChangeIC; }
            set { this.bookChangeIC = value; }
        }

        /// <summary>
        /// The amount that has been Transferred In  on this date. In base currency.
        /// </summary>
        public virtual Money TransferInToday
        {
            get
            {
                if (this.transferInToday != null)
                    return this.transferInToday;
                else
                    return new Money(0M, InstrumentCurrency.BaseCurrency);
            }
            set { this.transferInToday = value; }
        }

        /// <summary>
        /// The amount that has been Transferred In  on this date. In base currency.
        /// </summary>
        public virtual Money TransferOutToday
        {
            get
            {
                if (this.transferOutToday != null)
                    return this.transferOutToday;
                else
                    return new Money(0M, InstrumentCurrency.BaseCurrency);
            }
            set { this.transferOutToday = value; }
        }

        /// <summary>
        /// The gain caused by the foreign currency that has been realised on this date. In base currency.
        /// </summary>
        public virtual Money RealisedCurrencyGain
        {
            get
            {
                if (this.realisedCurrencyGain != null)
                    return this.realisedCurrencyGain;
                else
                    return new Money(0M, InstrumentCurrency.BaseCurrency);
            }
            set { this.realisedCurrencyGain = value; }
        }

        /// <summary>
        /// The gain caused by the foreign currency that has been realised so far to this date. In base currency.
        /// </summary>
        public virtual Money RealisedCurrencyGainToDate
        {
            get { return this.realisedCurrencyGainToDate; }
            set { this.realisedCurrencyGainToDate = value; }
        }

        /// <summary>
        /// The size change caused by the mutation
        /// </summary>
        public virtual InstrumentSize SizeChange
        {
            get 
            {
                if (PreviousMutation != null)
                {
                    return Size.CloneToParent() - PreviousMutation.Size.CloneToParent();
                }
                else
                    return Size;
            }
        }

        /// <summary>
        /// The nominal currency of the instrument
        /// </summary>
        public ICurrency InstrumentCurrency
        {
            get { return this.instrumentCurrency; }
            set { this.instrumentCurrency = value; }
        }

        /// <summary>
        /// The average exchange rate used to create the position (only opening)
        /// </summary>
        public virtual decimal AvgOpenExRate
        {
            get { return avgOpenExRate; }
            set { avgOpenExRate = value; }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }


        public virtual string GetUniqueCode
        {
            get { return string.Format("{0}.{1}.{2}", Account.Key.ToString(), Instrument.Key.ToString(), Date.ToString("yyyy-MM-dd")); }
        }

        #endregion

        #region Derived Properties

        public abstract string DisplayInstrumentsCategory { get; }
        public abstract IAssetClass AssetClass { get; }
        public abstract bool IsSecurityValuationMutation { get; }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (Account == null || Instrument == null || Size == null || Util.IsNullDate(Date))
                return base.ToString();
            else
                return string.Format("{0} {1} {2} {3}", Date.ToShortDateString(), Size.DisplayString, Instrument.DisplayName, Account.DisplayNumberWithName);
        }

        #endregion

        #region Privates

        private long key;
        private IValuationMutation prevMutation;
        private IValuationMutation convertedMutation;
        private IAccountTypeInternal account;
        private DateTime mutationDate = DateTime.MinValue;
        private bool isValid;
        private InstrumentSize size;
        private Money bookValue;
        private Money bookValueIC;
        private Money bookChange;
        private Money bookChangeIC;
        private Money transferInToday;
        private Money transferOutToday;
        private Money realisedCurrencyGain;
        private Money realisedCurrencyGainToDate;
        private ICurrency instrumentCurrency;
        private decimal avgOpenExRate;
        private DateTime creationDate = DateTime.Now;
        public static IInstrumentsCategories defaultInstrumentCategory;

        protected enum IsOpenClose
        {
            Open,
            Close,
            Both
        }

        #endregion
    }
}

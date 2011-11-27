using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Valuations.Mapping;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using NHibernate.Collection.Generic;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Valuations
{
    public class MonetaryValuationMutation : ValuationMutation, IMonetaryValuationMutation
    {
        #region Constructors

        protected MonetaryValuationMutation() { }

        /// <summary>
        /// Creates the first ValuationMutation for a new position
        /// </summary>
        /// <param name="positionTx">The first positionTx that creates this Valuation Mutation</param>
        /// <param name="mutationDate">The date of the mutation</param>
        internal MonetaryValuationMutation(IJournalEntryLine line)
        {
            this.CashPosition = line.ParentSubPosition.ParentPosition;
            this.Account = CashPosition.Account;
            this.Date = line.BookDate;
            this.InstrumentCurrency = CashPosition.PositionCurrency;
        }

        /// <summary>
        /// Creates a MonetaryValuationMutation from a previous MonetaryValuationMutation (with older date)
        /// </summary>
        /// <param name="mutationDate">The date for the new valuation mutation</param>
        /// <param name="prevMutation">The previous mutation</param>
        internal MonetaryValuationMutation(DateTime mutationDate, IMonetaryValuationMutation prevMutation)
            : base(mutationDate, prevMutation)
        {
            this.CashPosition = prevMutation.CashPosition;
            this.DepositToDate = prevMutation.DepositToDate;
            this.WithDrawalToDate = prevMutation.WithDrawalToDate;
        }

        #endregion

        #region Methods

        public void AddLine(IJournalEntryLine line)
        {
            IsOpenClose isOpen;
            Money newSize;

            getLineInfo(line, out newSize, out isOpen);

            Money currencyGain = calculateCurrencyGain(line, isOpen);

            // Check whether the Transaction is a deposit or wihdrawal
            if (line.IsCashTransfer)
            {
                Money cashTransfer = line.BaseBalance.Negate();

                if (line.GLAccount.CashTransferType == CashTransferTypes.Deposit)
                {
                    Deposit += cashTransfer;
                    DepositToDate += cashTransfer;
                }
                else if (line.GLAccount.CashTransferType == CashTransferTypes.Withdrawal || line.GLAccount.CashTransferType == CashTransferTypes.TransferFee)
                {
                    WithDrawal += cashTransfer;
                    WithDrawalToDate += cashTransfer;
                }
            }

            calculateBookStuff(line, isOpen, newSize, null, currencyGain);
            Size = newSize;
            Mappings.Add(new JournalEntryLineValuationMapping(line, this));
        }

        public override bool Validate()
        {
            base.Validate();
            if (!base.IsValid)
            {
                base.IsValid = ((Deposit != null && Deposit.IsNotZero) || (WithDrawal != null && WithDrawal.IsNotZero));
            }
            if (DepositToDate != null) DepositToDate = DepositToDate.Round();
            if (WithDrawalToDate != null) WithDrawalToDate = WithDrawalToDate.Round();
            this.marketRate = Math.Round(MarketRate, 7);
            return base.IsValid;
        }

        protected void getLineInfo(IJournalEntryLine line, out Money newSize, out IsOpenClose isOpen)
        {
            isOpen = IsOpenClose.Close;
            newSize = Money.Add(line.Balance.Negate(), Amount, true);

            // Determine if it is opening or closing
            if (Size == null || Size.IsZero)
                isOpen = IsOpenClose.Open;
            else if (newSize.Sign != Size.Sign && newSize.IsNotZero)
            {
                isOpen = IsOpenClose.Both;
                newSize.XRate = line.Balance.XRate;
            }
            // short position -> gets larger
            else if (!Size.Sign && newSize.Sign == Size.Sign && newSize.Abs() > Size.Abs())
                isOpen = IsOpenClose.Open;
            else
            {
                // Cash -> just check the effect of the transaction
                if (Size.Sign == line.Balance.Negate().Sign)
                    isOpen = IsOpenClose.Open;
                else
                    isOpen = IsOpenClose.Close;
            }
        }

        protected void checkData(IJournalEntryLine line)
        {
            if (!CashPosition.Equals(line.ParentSubPosition.ParentPosition))
                throw new ApplicationException("It is not possible to create Valuations when multiple positions exists for one instrument.");
        }

        protected Money calculateCurrencyGain(IJournalEntryLine line, IsOpenClose isOpen)
        {
            Money gain = null;
            // calculate the gain caused by the foreign currency
            if (!this.InstrumentCurrency.IsBase && isOpen != IsOpenClose.Open)
            {
                Money realisedAmount = null;
                if (isOpen == IsOpenClose.Close)
                    realisedAmount = line.BaseBalance.Negate();
                else // if (isOpen == IsOpenClose.Both)
                    realisedAmount = Money.Multiply(line.BaseBalance.Negate(), (Size.Quantity / line.Balance.Negate().Quantity), true);
                if (line.ExchangeRate - AvgOpenExRate != 0)
                {
                    gain = realisedAmount.Convert(line.ExchangeRate - AvgOpenExRate, InstrumentCurrency.BaseCurrency, 7);
                    RealisedCurrencyGain = Money.Add(RealisedCurrencyGain, gain, true);
                    RealisedCurrencyGainToDate = Money.Add(RealisedCurrencyGainToDate, gain, true);
                }
            }
            return gain;
        }

        protected void calculateBookStuff(IJournalEntryLine line, IsOpenClose isOpen, Money newSize, Money realisedAmount, Money baseRealisedAmount)
        {
            // Book Value & AvgOpenExRate
            if (isOpen == IsOpenClose.Close)
            {
                Money oldBookValue = BookValue;
                Money oldBookValueIC = BookValueIC;

                if (Size.IsNotZero && newSize.IsNotZero)
                {
                    BookValueIC = Money.Add(Money.Add(BookValueIC, line.Balance.Negate(), true), realisedAmount, true);
                    if (newSize.IsZero && BookValueIC.IsNotZero) BookValueIC = BookValueIC.ZeroedAmount();

                    BookValue = Money.Add(BookValue, line.BaseBalance.Negate(), true);
                    if (newSize.IsZero && BookValue.IsNotZero) BookValue = BookValue.ZeroedAmount();
                }
                else
                {
                    BookValueIC = BookValueIC.Clone(0M);
                    BookValue = BookValue.Clone(0M);
                }

                BookChange = Money.Add(BookChange, Money.Subtract(BookValue, oldBookValue, true), true);
                BookChangeIC = Money.Add(BookChangeIC, Money.Subtract(BookValueIC, oldBookValueIC, true), true);
            }
            else if (isOpen == IsOpenClose.Open)
            {
                Money bookChange = line.BaseBalance.Negate();
                BookChange = Money.Add(BookChange, bookChange, true);
                BookValue = Money.Add(BookValue, bookChange, true);

                Money bookChangeIC = line.Balance.Negate();
                BookChangeIC = Money.Add(BookChangeIC, bookChangeIC, true);
                BookValueIC = Money.Add(BookValueIC, bookChangeIC, true);

                if (newSize.IsNotZero)
                {
                    if (Size != null && Size.IsNotZero)
                    {
                        if (!InstrumentCurrency.IsBase)
                        {
                            InstrumentSize totalAmt = Money.Add(Money.Divide(Amount, AvgOpenExRate, true), Money.Divide(line.Balance.Negate(), line.ExchangeRate, true), true);
                            AvgOpenExRate = newSize.Quantity / totalAmt.Quantity;
                        }
                        else
                            AvgOpenExRate = 1M;
                    }
                    else
                        AvgOpenExRate = line.ExchangeRate;
                }
            }
            else //  IsOpenClose.Both
            {
                // In Instrument currency
                Money bookChange = newSize;
                BookChangeIC = bookChange;
                BookValueIC = bookChange;

                // In Base Currency
                BookChange = bookChange.BaseAmount;
                BookValue = bookChange.BaseAmount;
                AvgOpenExRate = line.ExchangeRate;
            }
        }

        public void AddNotRelevantLine(IJournalEntryLine notRelevantLine)
        {
            Mappings.Add(new JournalEntryLineValuationMapping(notRelevantLine, this));
        }

        #endregion

        #region Valuation Props

        public virtual Money Amount
        {
            get
            {
                Money amount = null;
                if (base.Size != null)
                {
                    amount = base.Size.GetMoney();
                    amount.XRate = AvgOpenExRate;
                }
                return amount;
            }
        }

        /// <summary>
        /// The position it relates to
        /// </summary>
        public virtual ICashPosition CashPosition
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// The amount that has been Transferred In on this date. In base currency.
        /// </summary>
        public virtual Money Deposit
        {
            get { return base.TransferInToday; }
            set { base.TransferInToday = value; }
        }

        /// <summary>
        /// The amount that has been Transferred In so far to this date. In base currency.
        /// </summary>
        public virtual Money DepositToDate
        {
            get 
            { 
                if (depositToDate == null)
                    return new Money(0m, Account.AccountOwner.BaseCurrency);
                else
                    return depositToDate;
            }
            set { this.depositToDate = value; }
        }

        /// <summary>
        /// The amount that has been Transferred Out on this date. In base currency.
        /// </summary>
        public virtual Money WithDrawal
        {
            get { return base.TransferOutToday; }
            set { base.TransferOutToday = value; }
        }

        /// <summary>
        /// The amount that has been Transferred Out so far to this date. In base currency.
        /// </summary>
        public virtual Money WithDrawalToDate
        {
            get 
            { 
            if (withDrawalToDate == null) 
                return new Money(0m,Account.BaseCurrency);
                if (((ICurrency)withDrawalToDate.Underlying).IsBase)
                return withDrawalToDate;
                else
                return withDrawalToDate.Convert(MarketRate, (ICurrency)BookValue.Underlying);
            }
            set { this.withDrawalToDate = value; }
        }

        public virtual decimal MarketRate
        {
            get { return marketRate; }
        }

        internal virtual IList<IJournalEntryLineValuationMapping> Mappings
        {
            get { return mappings; }
        }

        #endregion

        #region Derived Properties

        public override string DisplayInstrumentsCategory
        {
            get { return Instrument.Name; }
        }

        public override IAssetClass AssetClass
        {
            get
            {
                IAssetClass assetClass = null;
                if (CashPosition != null)
                    assetClass = CashPosition.AssetClass;
                return assetClass;
            }
        }

        public override bool IsSecurityValuationMutation
        {
            get { return false; }
        }

        #endregion

        #region Privates

        private ICashPosition position;
        private Money depositToDate;
        private Money withDrawalToDate;
        private decimal marketRate;
        private IList<IJournalEntryLineValuationMapping> mappings = new TransientDomainCollection<IJournalEntryLineValuationMapping>();

        #endregion

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Valuations.Mapping;
using NHibernate.Collection.Generic;

namespace B4F.TotalGiro.Valuations
{
    public class SecurityValuationMutation : ValuationMutation, ISecurityValuationMutation
    {
        #region Constructors

        protected SecurityValuationMutation() { }

        /// <summary>
        /// Creates the first ValuationMutation for a new position
        /// </summary>
        /// <param name="positionTx">The first positionTx that creates this Valuation Mutation</param>
        /// <param name="mutationDate">The date of the mutation</param>
        internal SecurityValuationMutation(IFundPositionTx positionTx)
        {
            this.Position = positionTx.ParentPosition;
            this.Account = Position.Account;
            this.Date = positionTx.TransactionDate;
            this.InstrumentCurrency = Position.InstrumentOfPosition.CurrencyNominal;
        }

        /// <summary>
        /// Creates a ValuationMutation for a new position, that originates from converted position(s) due to a corporate action (split/merger)
        /// </summary>
        /// <param name="convertedPositions">The positions that resulted in the new position due to a corporate action</param>
        /// <param name="positionTx">The positionTx that creates this Valuation Mutation</param>
        /// <param name="mutationDate">The date of the mutation</param>
        internal SecurityValuationMutation(IList<IFundPosition> convertedPositions, IFundPositionTx positionTx, DateTime mutationDate)
            : this(positionTx)
        {
            foreach (IFundPosition pos in convertedPositions)
            {
                BookValue += pos.LastMutation.BookValue;
                BookValueIC += pos.LastMutation.BookValueIC;
                RealisedAmountToDate += ((ISecurityValuationMutation)pos.LastMutation).RealisedAmountToDate;
                BaseRealisedAmountToDate += ((ISecurityValuationMutation)pos.LastMutation).BaseRealisedAmountToDate;
                pos.LastMutation.ConvertedMutation = this;
            }
        }

        /// <summary>
        /// Creates a MonetaryValuationMutation from a previous MonetaryValuationMutation (with older date)
        /// </summary>
        /// <param name="mutationDate">The date for the new valuation mutation</param>
        /// <param name="prevMutation">The previous mutation</param>
        internal SecurityValuationMutation(DateTime mutationDate, ISecurityValuationMutation prevMutation)
            : base(mutationDate, prevMutation)
        {
            this.Position = prevMutation.Position;
            this.BookPrice = prevMutation.BookPrice;
            this.CostPrice = prevMutation.CostPrice;
            this.RealisedAmountToDate = prevMutation.RealisedAmountToDate;
            this.BaseRealisedAmountToDate = prevMutation.BaseRealisedAmountToDate;
        }

        #endregion

        #region Methods

        public void AddTx(IFundPositionTx posTx)
        {
            IsOpenClose isOpen;
            InstrumentSize newSize;

            if (posTx.Price == null)
                throw new ApplicationException(string.Format("Price is NULL for posTxID: {0}", posTx.Key));

            getPosTxInfo(posTx, out newSize, out isOpen);

            if (CostPrice != null && !CostPrice.Underlying.Equals(posTx.Price.Underlying))
                throw new ApplicationException(string.Format("Can not handle one instrument with different currencies for instrument {0} (posTxID: {1}", posTx.Instrument.DisplayIsinWithName, posTx.Key));

            Money currencyGain = calculateCurrencyGain(posTx, isOpen);
            Money realAmount = null;
            Money baseRealAmount = null;
            Money baseChange = null;

            if (posTx.ParentTransaction.TransactionType == TransactionTypes.NTM && posTx.Price != null && posTx.Price.IsNotZero)
            {
                baseChange = (posTx.BookValue);
                if (isOpen == IsOpenClose.Close)
                    TransferOutToday = Money.Add(TransferOutToday, baseChange, true);
                else
                    TransferInToday = Money.Add(TransferInToday, baseChange, true);
            }

            // Realised Amount
            if (!posTx.DoNotRealize)
            {
                if (isOpen == IsOpenClose.Close)
                {
                    // In instrument currency
                    //realAmount = Money.Multiply(InstrumentSize.Multiply(posTx.Size, -1M, true), (posTx.Price - CostPrice), true);
                    realAmount = InstrumentSize.Multiply(posTx.Size, -1M, true).CalculateAmount((posTx.Price - CostPrice), true);
                    RealisedAmount = Money.Add(RealisedAmount, realAmount, true);
                    RealisedAmountToDate = Money.Add(RealisedAmountToDate, realAmount, true);

                    // In Base Currency
                    //baseRealAmount = Money.Add(realAmount.Convert(1M / posTx.ExchangeRate, InstrumentCurrency.BaseCurrency, 7), currencyGain, true);
                    //baseRealAmount = (this.BookPrice * posTx.Size) - posTx.BookValue;
                    baseRealAmount = Money.Subtract(posTx.Size.CalculateAmount(this.BookPrice, true), posTx.BookValue, true);
                    BaseRealisedAmount = Money.Add(BaseRealisedAmount, baseRealAmount, true);
                    BaseRealisedAmountToDate = Money.Add(BaseRealisedAmountToDate, baseRealAmount, true);

                }
                else if (isOpen == IsOpenClose.Both)
                {
                    // Position has swapped -> the old position size has relised
                    // all in same currency -> no fuckup
                    realAmount = Size.CalculateAmount((posTx.Price - CostPrice), true);
                    RealisedAmount = Money.Add(RealisedAmount, realAmount, true);
                    RealisedAmountToDate = Money.Add(RealisedAmountToDate, realAmount, true);

                    // In Base Currency
                    baseRealAmount = Money.Add(realAmount.Convert(1M / posTx.ExchangeRate, InstrumentCurrency.BaseCurrency, 7), currencyGain, true);
                    BaseRealisedAmount = Money.Add(BaseRealisedAmount, baseRealAmount, true);
                    BaseRealisedAmountToDate = Money.Add(BaseRealisedAmountToDate, baseRealAmount, true);
                }
            }
            else
            {
                // In case of conversion, there might be some cash involved
                realAmount = posTx.Value;
                RealisedAmount = Money.Add(RealisedAmount, realAmount, true);
                RealisedAmountToDate = Money.Add(RealisedAmountToDate, realAmount, true);

                baseRealAmount = posTx.ValueInBaseCurrency;
                BaseRealisedAmount = Money.Add(BaseRealisedAmount, baseRealAmount, true);
                BaseRealisedAmountToDate = Money.Add(BaseRealisedAmountToDate, baseRealAmount, true);
            }

            //// Cost Price
            if (isOpen == IsOpenClose.Both)
                CostPrice = posTx.Price;
            else if (isOpen == IsOpenClose.Open)
            {
                if (posTx.IsConversion)
                {
                    // This is a conversion ->
                    // Use the Book Value in instrument currency to calculate the price
                    CostPrice = BookValueIC / newSize;
                }
                else if (CostPrice == null)
                    CostPrice = posTx.Price;
                else
                {
                    if (newSize.IsNotZero)
                    {
                        Money newValue = Money.Add(Size.CalculateAmount(CostPrice, true), posTx.BookValueIC, true);
                        CostPrice = Money.Divide(newValue, newSize, true);
                    }
                }
            }

            calculateBookStuff(posTx, isOpen, newSize, realAmount, baseRealAmount);
            Size = newSize;
            Mappings.Add(new PositionTxValuationMapping(posTx, this));
        }

        protected void getPosTxInfo(IFundPositionTx posTx, out InstrumentSize newSize, out IsOpenClose isOpen)
        {
            isOpen = IsOpenClose.Close;
            newSize = InstrumentSize.Add(posTx.Size, Size, true);

            // Determine if it is opening or closing
            if (Size == null || Size.IsZero)
                isOpen = IsOpenClose.Open;
            else if (newSize.Sign != Size.Sign && newSize.IsNotZero)
                isOpen = IsOpenClose.Both;
            // short position -> gets larger
            else if (!Size.Sign && newSize.Sign == Size.Sign && newSize.Abs() > Size.Abs())
                isOpen = IsOpenClose.Open;
            else
            {
                if (IsSecurityValuationMutation)
                {
                    if (posTx.Side == Side.Sell || posTx.Side == Side.XO)
                        isOpen = IsOpenClose.Close;
                    else
                        isOpen = IsOpenClose.Open;
                }
                else
                {
                    // Cash -> just check the effect of the transaction
                    if (Size.Sign == posTx.Size.Sign)
                        isOpen = IsOpenClose.Open;
                    else
                        isOpen = IsOpenClose.Close;
                }
            }
        }

        protected void checkData(IFundPositionTx posTx)
        {
            if (!Position.Equals(posTx.ParentPosition))
                throw new ApplicationException("It is not possible to create Valuations when multiple positions exists for one instrument.");
        }

        protected Money calculateCurrencyGain(IFundPositionTx posTx, IsOpenClose isOpen)
        {
            Money gain = null;
            if (!posTx.DoNotRealize)
            {
                // calculate the gain caused by the foreign currency
                if (!this.InstrumentCurrency.IsBase && isOpen != IsOpenClose.Open)
                {
                    Money realisedAmount = null;
                    if (isOpen == IsOpenClose.Close)
                        realisedAmount = posTx.BookValueIC;
                    else // if (isOpen == IsOpenClose.Both)
                        realisedAmount = Money.Multiply(posTx.BookValueIC, (Size.Quantity / posTx.Size.Quantity), true);
                    if (posTx.ExchangeRate - AvgOpenExRate != 0)
                    {
                        gain = realisedAmount.Convert(posTx.ExchangeRate - AvgOpenExRate, InstrumentCurrency.BaseCurrency, 7);
                        RealisedCurrencyGain = Money.Add(RealisedCurrencyGain, gain, true);
                        RealisedCurrencyGainToDate = Money.Add(RealisedCurrencyGainToDate, gain, true);
                    }
                }
            }
            return gain;
        }

        protected void calculateBookStuff(IFundPositionTx posTx, IsOpenClose isOpen, InstrumentSize newSize, Money realisedAmount, Money baseRealisedAmount)
        {
            // Book Value & AvgOpenExRate
            if (isOpen == IsOpenClose.Close)
            {
                if (!posTx.DoNotRealize)
                {
                    Money oldBookValue = BookValue;
                    Money oldBookValueIC = BookValueIC;

                    if (Size.IsNotZero && newSize.IsNotZero)
                    {
                        BookValueIC = Money.Add(Money.Add(BookValueIC, posTx.BookValueIC, true), realisedAmount, true);
                        if (newSize.IsZero && BookValueIC.IsNotZero) BookValueIC = BookValueIC.ZeroedAmount();

                        //BookValue = Money.Add(Money.Add(BookValue, posTx.BookValue, true), baseRealisedAmount, true);
                        BookValue = Money.Add(BookValue, posTx.Size.CalculateAmount(this.BookPrice), true);
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
                else
                {
                    BookChange = BookValue.Clone(0M);
                    BookChangeIC = BookValueIC.Clone(0M);
                }
            }
            else if (isOpen == IsOpenClose.Open)
            {
                Money bookChange = posTx.BookValue;
                BookChange = Money.Add(BookChange, bookChange, true);
                BookValue = Money.Add(BookValue, bookChange, true);

                Money bookChangeIC = posTx.BookValueIC;
                BookChangeIC = Money.Add(BookChangeIC, bookChangeIC, true);
                BookValueIC = Money.Add(BookValueIC, bookChangeIC, true);

                if (newSize.IsNotZero)
                {
                    BookPrice = Money.Divide(BookValue, newSize, true);
                    if (Size != null)
                    {
                        // for a conversion the avg ExRate does not change
                        if (!posTx.IsConversion)
                        {
                            if (!InstrumentCurrency.IsBase)
                            {
                                Money currentCostAmount = Size.CalculateAmount(CostPrice, true);
                                Money totalAmtIC = Money.Add(currentCostAmount, posTx.BookValueIC, true);
                                //Money totalAmt = Money.Add(Money.Convert(Money.Multiply(Size, CostPrice, true), (1M / AvgOpenExRate), InstrumentCurrency.BaseCurrency, true), posTx.BookValue, true);
                                Money totalAmt = Money.Add(currentCostAmount.ConvertToBase((1M / AvgOpenExRate), true), posTx.BookValue, true);

                                AvgOpenExRate = totalAmtIC.Quantity / totalAmt.Quantity;
                            }
                            else
                                AvgOpenExRate = 1M;
                        }
                    }
                    else
                        AvgOpenExRate = posTx.ExchangeRate;
                }
            }
            else //  IsOpenClose.Both
            {
                // if swap -> take the price from the Tx
                if (posTx.Price.Underlying.IsBase)
                    BookPrice = posTx.Price;
                else
                    BookPrice = posTx.Price.Convert((1 / posTx.ExchangeRate), posTx.Price.Underlying.BaseCurrency);

                // In Instrument currency
                Money bookChange = newSize.CalculateAmount(posTx.Price, true);
                BookChangeIC = bookChange;
                BookValueIC = bookChange;

                // Convert to Base Currency
                if (!((ICurrency)bookChange.Underlying).IsBase)
                    bookChange = bookChange.Convert(1 / posTx.ExchangeRate, ((ICurrency)bookChange.Underlying).BaseCurrency, 7);
                BookChange = bookChange;
                BookValue = bookChange;
                AvgOpenExRate = Math.Round(posTx.ExchangeRate, 7);
            }
        }

        public void AddNotRelevantPositionTx(IFundPositionTx notRelevantPosTx)
        {
            Mappings.Add(new PositionTxValuationMapping(notRelevantPosTx, this));
        }

        public override bool Validate()
        {
            base.Validate();
            if (!base.IsValid)
            {
                base.IsValid = ((RealisedAmount != null && RealisedAmount.IsNotZero) || (CostPrice != null && CostPrice.IsNotZero));
            }

            if (RealisedAmount != null)  RealisedAmount = RealisedAmount.Round();
            if (RealisedAmountToDate != null)  RealisedAmountToDate = RealisedAmountToDate.Round();
            if (BaseRealisedAmount != null) BaseRealisedAmount = BaseRealisedAmount.Round();
            if (BaseRealisedAmountToDate != null) BaseRealisedAmountToDate = BaseRealisedAmountToDate.Round();
            if (BookPrice != null) BookPrice = BookPrice.Round(4);
            if (CostPrice != null) CostPrice = CostPrice.Round(4);
            return base.IsValid;
        }

        #endregion

        #region Valuation Props

        /// <summary>
        /// The position it relates to
        /// </summary>
        public virtual IFundPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// The amount that has been realised on this date. In instrument currency.
        /// </summary>
        public virtual Money RealisedAmount
        {
            get 
            {
                if (this.realisedAmount != null)
                    return this.realisedAmount;
                else
                    return new Money(0M, InstrumentCurrency);
            }
            set { this.realisedAmount = value; }
        }

        /// <summary>
        /// The amount that has been realised so far to this date. In instrument currency.
        /// </summary>
        public virtual Money RealisedAmountToDate
        {
            get { return this.realisedAmountToDate; }
            set { this.realisedAmountToDate = value; }
        }

        /// <summary>
        /// The amount that has been realised on this date. In base currency.
        /// </summary>
        public virtual Money BaseRealisedAmount
        {
            get
            {
                if (this.baseRealisedAmount != null)
                    return this.baseRealisedAmount;
                else
                    return new Money(0M, InstrumentCurrency.BaseCurrency);
            }
            set { this.baseRealisedAmount = value; }
        }

        /// <summary>
        /// The amount that has been realised so far to this date. In base currency.
        /// </summary>
        public virtual Money BaseRealisedAmountToDate
        {
            get { return this.baseRealisedAmountToDate; }
            set { this.baseRealisedAmountToDate = value; }
        }

        /// <summary>
        /// The book price (Book Value / Size)
        /// </summary>
        public virtual Price BookPrice
        {
            get { return this.bookPrice; }
            set { this.bookPrice = value; }
        }

        /// <summary>
        /// The price of the instrument
        /// </summary>
        public virtual Price CostPrice
        {
            get 
            {
                if (this.costPrice != null)
                    this.costPrice.XRate = AvgOpenExRate;
                return costPrice; 
            }
            set { costPrice = value; }
        }

        /// <summary>
        /// The transactions that belong to this order.
        /// </summary>
        public virtual ISecurityValuationMutationCashMutationCollection CashMutations
        {
            get
            {
                if (cashMutations == null && bagOfCashMutations != null && bagOfCashMutations.Count > 0)
                    this.cashMutations = new SecurityValuationMutationCashMutationCollection(bagOfCashMutations, this);
                return cashMutations;
            }
        }

        public virtual Money BaseCommission
        {
            get
            {
                Money commission = null;
                if (CashMutations != null)
                    commission = CashMutations.GetTotalValue(new ValuationCashTypes[] { ValuationCashTypes.CostsCommission });

                if (commission == null)
                    commission = new Money(0M, InstrumentCurrency);

                return commission;
            }
        }

        public virtual IList<ITransaction> Transactions
        {
            get
            {
                if (!transactions.WasInitialized)
                    transactions.ForceInitialization();
                return transactions.ToList<ITransaction>();
            }
        }

        /// <summary>
        /// The change in Total Trade Amount. In instrument currency.
        /// </summary>
        public virtual Money TotalTradeAmount
        {
            get
            {
                Money amount = null;
                foreach (ITransaction tx in Transactions)
                {
                    if (tx != null)
                        amount += tx.CounterValueSize;
                }
                return amount;
            }
        }

        /// <summary>
        /// The change in Total Trade Amount. In base currency.
        /// </summary>
        public virtual Money TotalBaseTradeAmount
        {
            get
            {
                Money amount = null;
                foreach (ITransaction tx in Transactions)
                {
                    if (tx != null)
                        amount += tx.CounterValueSizeBaseCurrency;
                }
                return amount;
            }
        }

        internal virtual IList<IPositionTxValuationMapping> Mappings
        {
            get
            {
                //if (!mappings.WasInitialized)
                //    mappings.ForceInitialization();
                //return (List<IPositionTxValuationMapping>)mappings.Entries();
                return mappings;
            }
        }

        #endregion

        #region Derived Properties

        public override string DisplayInstrumentsCategory
        {
            get
            {
                string categoryName = string.Empty;
                if (Instrument.IsTradeable)
                {
                    if (Position != null && Position.InstrumentsCategories != null)
                        categoryName = Position.InstrumentsCategories.InstrumentsCategoryName;
                    else if (ValuationMutation.defaultInstrumentCategory != null)
                        categoryName = ValuationMutation.defaultInstrumentCategory.InstrumentsCategoryName;
                    else
                        categoryName = "unknown";
                }
                return categoryName;
            }
        }

        public override IAssetClass AssetClass
        {
            get
            {
                IAssetClass assetClass = null;
                if (Position != null)
                    assetClass = Position.AssetClass;
                return assetClass;
            }
        }

        public override bool IsSecurityValuationMutation
        {
            get { return true; }
        }

        #endregion


        #region Privates

        private IFundPosition position;
        private Money realisedAmount;
        private Money realisedAmountToDate;
        private Money baseRealisedAmount;
        private Money baseRealisedAmountToDate;
        private Price bookPrice;
        private Price costPrice;
        private IList bagOfCashMutations;
        private ISecurityValuationMutationCashMutationCollection cashMutations;
        private IList<IPositionTxValuationMapping> mappings = new TransientDomainCollection<IPositionTxValuationMapping>();
        private PersistentGenericBag<ITransaction> transactions;

        #endregion

    }
}

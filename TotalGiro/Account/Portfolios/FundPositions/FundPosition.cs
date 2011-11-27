using System;
using System.Collections.Generic;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Valuations;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public class FundPosition : TotalGiroBase<IFundPosition>, IFundPosition, ICommonPosition
    {
        #region Constructor

        public FundPosition()
        {
            positionTransactions = new FundPositionTxCollection(this);
            this.CreationDate = DateTime.Now;
        }

        public FundPosition(IAccountTypeInternal account, IInstrumentsWithPrices instrumentOfPosition) 
            : this()
        {
            this.Account = account;
            this.instrumentOfPosition = instrumentOfPosition;
            this.Size = new InstrumentSize(0m, instrumentOfPosition);
            this.TotalOpenSize = new InstrumentSize(0m, instrumentOfPosition);
        }

        public FundPosition(IAccountTypeInternal account, IInstrumentsWithPrices instrumentOfPosition, DateTime creationDate)
            : this(account, instrumentOfPosition)
        {
            this.CreationDate = creationDate;
        }

        #endregion

        #region Props

        public IAccountTypeInternal Account { get; set; }
        public InstrumentSize Size { get; set; }
        public InstrumentSize TotalOpenSize { get; set; }

        public virtual DateTime OpenDate
        {
            get { return (openDate.HasValue ? openDate.Value : DateTime.MinValue); }
            set { openDate = value; }
        }

        public virtual DateTime CreationDate
        {
            get { return (creationDate.HasValue ? creationDate.Value : DateTime.MinValue); }
            set { creationDate = value; }
        }

        public virtual DateTime LastUpdated
        {
            get { return (lastUpdated.HasValue ? lastUpdated.Value : DateTime.MinValue); }
        }

        /// <summary>
        /// The current value of the position.
        /// The last known price is used to calculate this value
        /// </summary>
        public virtual Money CurrentValue
        {
            get
            {
                Money value = null;
                if (Size != null)
                {
                    IInstrumentsWithPrices instrument = Size.Underlying as IInstrumentsWithPrices;
                    if (instrument != null)
                    {
                        if (instrument.IsWithPrice)
                        {
                            IPriceDetail price = ((IInstrumentsWithPrices)instrument).CurrentPrice;
                            if (price != null)
                                value =  Size.CalculateAmount(price.Price);
                        }
                        else
                        {
                            // Currencies -> Value is the same
                            value = (Money)Size.GetMoney();
                        }
                    }
                }
                return value;
            }
        }

        /// <summary>
        /// Returns the current value in base currency.
        /// When the nominal currency of the instrument is not the base currency then it is converted using the latest exchange rate.
        /// </summary>
        public virtual Money CurrentBaseValue
        {
            get
            {
                Money curValue = CurrentValue;
                if (curValue != null)
                {
                    if (curValue.Underlying.ToCurrency.IsBase)
                        return curValue;
                    else
                        return curValue.CurrentBaseAmount;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// The ideal model allocation of the instrument
        /// </summary>
        public virtual decimal ModelAllocation
        {
            get
            {
                decimal allocation = 0;
                if (Account.AccountType == AccountTypes.Customer || Account.AccountType == AccountTypes.Nostro)
                {
                    if (((IAccountTypeCustomer)Account).ModelPortfolio != null)
                    {
                        IModelVersion mv = ((IAccountTypeCustomer)Account).ModelPortfolio.LatestVersion;
                        if (mv != null)
                        {
                            foreach (IModelInstrument mi in mv.ModelInstruments)
                            {
                                if (mi.Component.Equals(this.InstrumentOfPosition))
                                {
                                    allocation = mi.Allocation;
                                    break;
                                }
                            }
                        }
                    }
                }
                return allocation;
            }
        }

        public virtual IFundPositionTxCollection PositionTransactions
        {
            get
            {
                IFundPositionTxCollection postrans = (IFundPositionTxCollection)positionTransactions.AsList();
                if (postrans.ParentPosition == null) postrans.ParentPosition = this;
                return postrans;
            }
        }

        public IInstrumentsWithPrices InstrumentOfPosition
        {
            get
            {
                if (this.instrumentOfPosition == null)
                    this.instrumentOfPosition = (IInstrumentsWithPrices)Size.Underlying;
                return this.instrumentOfPosition;
            }
        }

        public string InstrumentDescription
        {
            get { return this.InstrumentOfPosition.DisplayName + (CurrentBaseValue.IsZero ? " (gesloten)" : ""); }
        }
        /// <summary>
        /// The last updated valuation mutations
        /// </summary>
        public virtual IValuationMutation LastMutation
        {
            get { return lastMutation; }
            set { lastMutation = value; }
        }

        /// <summary>
        /// The last updated valuation
        /// </summary>
        public virtual IValuation LastValuation
        {
            get { return lastValuation; }
            private set { lastValuation = value; }
        }

        public virtual DateTime LastBondCouponCalcDate
        {
            get
            {
                return this.lastBondCouponCalcDate.HasValue ? lastBondCouponCalcDate.Value : DateTime.MinValue;
            }

            set
            { 
                this.lastBondCouponCalcDate = value;
            }
        }
        public virtual IBondCouponPaymentCollection BondCouponPayments
        {
            get
            {
                BondCouponPaymentCollection payments = (BondCouponPaymentCollection)bondCouponPayments.AsList();
                if (payments.ParentPosition == null) payments.ParentPosition = this;
                return payments;
            }
        }

        public virtual IList<IBondCouponPaymentDailyCalculation> BondCouponCalculations
        {
            get { return bondCouponCalculations; }
        }

        #endregion

        #region Derived Properties

        protected IAssetManagerInstrument assetManagerInstrument
        {
            get
            {
                if (InstrumentOfPosition.IsTradeable && Account.AccountOwner != null && !Account.AccountOwner.IsStichting)
                    return ((IAssetManager)Account.AccountOwner).AssetManagerInstruments.GetItemByInstrument((ITradeableInstrument)InstrumentOfPosition);
                else
                    return null;
            }
        }

        /// <summary>
        /// The maximum amount that can be withdrawn from this position by a withdrawal instructions (depending on the instrument)
        /// </summary>
        /// <returns>An amount in base currency</returns>
        public virtual Money MaxWithdrawalAmount
        {
            get
            {
                decimal maxWithdrawalAmountPercentage = 1M;
                if (InstrumentOfPosition.IsTradeable)
                {
                    if (assetManagerInstrument != null)
                        maxWithdrawalAmountPercentage = Convert.ToDecimal(assetManagerInstrument.MaxWithdrawalAmountPercentage) / 100M;
                    else
                        maxWithdrawalAmountPercentage = 0M;
                }
                return CurrentBaseValue * maxWithdrawalAmountPercentage;
            }
        }

        public virtual IAssetClass AssetClass
        {
            get
            {
                if (InstrumentOfPosition.IsTradeable)
                    return (assetManagerInstrument != null ? assetManagerInstrument.AssetClass : null);
                else
                    return null;
            }
        }
        public virtual IRegionClass RegionClass
        {
            get { return (assetManagerInstrument != null ? assetManagerInstrument.RegionClass : null); }
        }
        public virtual IInstrumentsCategories InstrumentsCategories
        {
            get { return (assetManagerInstrument != null ? assetManagerInstrument.InstrumentsCategories : null); }
        }
        public virtual ISectorClass SectorClass
        {
            get { return (assetManagerInstrument != null ? assetManagerInstrument.SectorClass : null); }
        }

        #endregion

        #region Privates

        private DateTime? openDate;
        private DateTime? lastUpdated;
        private DateTime? creationDate;
        private IInstrumentsWithPrices instrumentOfPosition;
        private IValuationMutation lastMutation;
        private IValuation lastValuation;
        private IDomainCollection<IFundPositionTx> positionTransactions;
        private IDomainCollection<IBondCouponPayment> bondCouponPayments = new BondCouponPaymentCollection();
        private IList<IBondCouponPaymentDailyCalculation> bondCouponCalculations;
        private DateTime? lastBondCouponCalcDate;
        #endregion

        #region ICommonPosition Members

        public IInstrument Instrument
        {
            get { return InstrumentOfPosition; }
        }

        #endregion
    }
}

using System;
using System.Collections;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Communicator;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class representing an instrument
    /// </summary>
    public abstract class Instrument : TotalGiroBase<IInstrument>, IInstrument
    {
        #region Constructor

        protected Instrument() { }

        /// <exclude/>
        protected Instrument(string instrumentName, SecCategories secCategoryID, bool isActive)
        {

            this.instrumentName = instrumentName;
            this.secCategoryID = secCategoryID;
            this.isActive = isActive;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get/set instrument name
        /// </summary>
        public virtual string Name
        {
            get { return instrumentName; }
            set { instrumentName = value; }
        }

        /// <summary>
        /// Get instrument name
        /// </summary>
        public virtual string DisplayName
        {
            get { return instrumentName; }
        }

        /// <summary>
        /// For displaying name with isin
        /// </summary>
        public virtual string DisplayNameWithIsin
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayIsin))
                    return string.Format("{0}", DisplayName);
                else
                    return string.Format("{0} - {1}", DisplayName, DisplayIsin);
            }
        }

        /// <summary>
        /// Get instrument Isin if applicable
        /// </summary>
        public virtual string DisplayIsin
        {
            get
            {
                if (IsWithPrice)
                    return ((IInstrumentsWithPrices)this).Isin;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// For displaying isin with name
        /// </summary>
        public virtual string DisplayIsinWithName
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayIsin))
                    return string.Format("{0}", DisplayName);
                else
                    return string.Format("{0} - {1}", DisplayIsin, DisplayName);
            }
        }

        /// <summary>
        /// The parent instrument
        /// </summary>
        public virtual IInstrument ParentInstrument
        {
            get { return this.parentInstrument; }
            set { this.parentInstrument = value; }
        }

        /// <summary>
        /// The top parent instrument
        /// </summary>
        public virtual IInstrument TopParentInstrument
        {
            get
            {
                if (this.topParentInstrument != null)
                    return this.topParentInstrument;
                else
                    return this;
            }
        }

        /// <summary>
        /// The instruments that changed into this instrument
        /// </summary>
        public virtual IInstrumentCollection ConvertedChildInstruments
        {
            get
            {
                if (this.convertedChildInstruments == null)
                    this.convertedChildInstruments = new InstrumentCollection(bagOfInstruments, this);
                return this.convertedChildInstruments;
            }
            set { this.convertedChildInstruments = value; }
        }

        /// <summary>
        /// The date that the instrument does not exist anymore
        /// </summary>
        public virtual DateTime InActiveDate
        {
            get
            {
                return this.inActiveDate.HasValue ? inActiveDate.Value : DateTime.MinValue;
            }
            set
            {
                if (Util.IsNotNullDate(value))
                    this.inActiveDate = value;
                else
                    this.inActiveDate = null;
                if (this.inActiveDate.HasValue && this.inActiveDate < DateTime.Now)
                    IsActive = false;
            }
        }

        /// <summary>
        /// The country of origin
        /// </summary>
        public virtual ICountry Country
        {
            get { return country; }
            set { country = value; }
        }

        /// <summary>
        /// Get/set category of the instrument
        /// </summary>
        public virtual ISecCategory SecCategory
        {
            get { return secCategory; }
            set { secCategory = value; }
        }

        /// <summary>
        /// Get/set activation in system
        /// </summary>
        public virtual bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>
        /// Get/set number of decimals
        /// </summary>
        public virtual int DecimalPlaces
        {
            get { return this.decimalPlaces; }
            set { this.decimalPlaces = value; }
        }

        /// <summary>
        /// Convert the Instrument to a currency
        /// </summary>
        public virtual ICurrency ToCurrency
        {
            get
            {
                if (IsCash)
                    return (ICurrency)this;
                else
                    return null;
            }
        }

        /// <summary>
        /// Get/set price
        /// </summary>
        public virtual IPriceDetail CurrentPrice
        {
            get
            {
                Price price = new Price(1, (ICurrency)this, this);
                return new HistoricalPrice(price, DateTime.Now);
            }
            set { }
        }

        public virtual string DisplayCurrentPrice
        {
            get
            {
                string curPrice = "";
                if (CurrentPrice != null)
                    curPrice = CurrentPrice.Price.ShortDisplayString;
                return curPrice;
            }
        }

        public virtual DateTime DisplayCurrentPriceDate
        {
            get
            {
                DateTime priceDate = DateTime.MinValue;
                if (CurrentPrice != null)
                    priceDate = CurrentPrice.Date;
                return priceDate;
            }
        }

        public abstract bool IsTradeable { get; }
        public abstract bool IsWithPrice { get; }
        public abstract bool IsCash { get; }
        public virtual bool IsCorporateAction 
        {
            get { return false; }
        }

        /// <summary>
        /// Is this a security
        /// </summary>
        public virtual bool IsSecurity
        {
            get { return false; }
        }

        /// <summary>
        /// Is this a cash management fund
        /// </summary>
        public virtual bool IsCashManagementFund
        {
            get { return false; }
        }

        /// <summary>
        /// Factor for Direct or Percentage priced instruments
        /// </summary>
        public virtual decimal PriceTypeFactor
        {
            get
            {
                PricingTypes priceType = PricingTypes.Direct;
                decimal priceTypeFactor = 1M;

                if (IsWithPrice)
                    priceType = ((IInstrumentsWithPrices)this).PriceType;
                if (priceType == PricingTypes.Percentage)
                    priceTypeFactor = 0.01M;
                return priceTypeFactor;
            }
        }

        /// <summary>
        /// Get/set creation date
        /// </summary>
        public virtual DateTime CreationDate
        {
            get { return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue; }
            set { this.creationDate = value; }
        }

        /// <summary>
        /// Get/set last update
        /// </summary>
        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
            internal set { lastUpdated = value; }
        }

        public virtual IList HistoricalTransformations
        {
            get { return historicalTransformations; }
            protected set { historicalTransformations = value; }
        }

        public virtual IInstrumentSymbolCollection ExternalSymbols
        {
            get
            {
                IInstrumentSymbolCollection symbols = (IInstrumentSymbolCollection)externalSymbols.AsList();
                if (symbols.Parent == null) symbols.Parent = this;
                return symbols;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// It returns the family tree of al related instruments (recursive)
        /// </summary>
        /// <returns>A list of instruments</returns>
        public virtual IList GetInstrumentPedigree()
        {
            ArrayList list = new ArrayList();
            list.Add(this);
            foreach (IInstrument child in ConvertedChildInstruments)
            {
                list.AddRange(child.GetInstrumentPedigree());
            }
            return list;
        }

        /// <summary>
        /// Get screen representation
        /// </summary>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public virtual string DisplayToString(decimal Quantity)
        {
            return Quantity.ToString();
        }

        public abstract bool CalculateCosts(IOrder order, IFeeFactory feeFactory);
        public abstract bool CalculateCosts(IOrderAllocation transaction, IFeeFactory feeFactory, IGLLookupRecords lookups);

        protected void checkCostCalculater(Object obj, IFeeFactory feeFactory)
        {
            if (obj == null)
                throw new ApplicationException("It is not possible to calculate the costs without an instance of an order/transaction.");

            if (feeFactory == null)
                throw new ApplicationException("It is not possible to calculate the costs without an instance of the fee factory.");
        }

        public abstract PredictedSize PredictSize(Money inputAmount);

        #endregion

        #region Equality

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="lhs">First instrument</param>
        /// <param name="rhs">Second instrument</param>
        /// <returns>Flag</returns>
        public static bool operator ==(Instrument lhs, IInstrument rhs)
        {
            if ((Object)lhs == null || (Object)rhs == null)
            {
                if ((Object)lhs == null && (Object)rhs == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (lhs.Key == rhs.Key)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Overridden unequality operator
        /// </summary>
        /// <param name="lhs">First instrument</param>
        /// <param name="rhs">Second instrument</param>
        /// <returns>Flag</returns>
        public static bool operator !=(Instrument lhs, IInstrument rhs)
        {
            if ((Object)lhs == null || (Object)rhs == null)
            {
                if ((Object)lhs == null && (Object)rhs == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (lhs.Key != rhs.Key)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region Validation

        public abstract bool Validate();

        protected virtual bool validate()
        {
            if (this.instrumentName == string.Empty)
                throw new ApplicationException("The instrument name is mandatory.");
            return true;
        }

        #endregion

        #region Private variables

        private IInstrument parentInstrument;
        private IInstrument topParentInstrument;
        private IList bagOfInstruments = new ArrayList();
        private IInstrumentCollection convertedChildInstruments;
        private IList historicalTransformations = new ArrayList();
        private DateTime? inActiveDate;
        private bool isActive = true;
        private DateTime? creationDate;
        private DateTime lastUpdated = DateTime.Now;
        private int decimalPlaces;
        protected SecCategories secCategoryID;
        protected string instrumentName;
        private ISecCategory secCategory;
        private ICountry country;
        private IDomainCollection<IInstrumentSymbol> externalSymbols = new InstrumentSymbolCollection();

        #endregion
    }
}

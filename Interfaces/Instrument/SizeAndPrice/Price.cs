using System;
using System.Collections.Generic;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// The price class is used to work with prices which consist of a quantity, a underlying instrument and the currency of the price.
    /// This is all kept as one piece of information.
    /// The class is immutable.
    /// </summary>
    public class Price : IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class.
        /// </summary>
        protected Price() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class.
        /// </summary>
        /// <param name="quantity">The quantity (just a number) of the price</param>
        /// <param name="underlying">The currency the price is in</param>
        /// <param name="instrument">The instrument to which the price belongs</param>
        public Price(Decimal quantity, ICurrency underlying, IInstrument instrument)
            :this(quantity, underlying, instrument, 0M)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class.
        /// </summary>
        /// <param name="quantity">The quantity (just a number) of the price</param>
        /// <param name="underlying">The currency the price is in</param>
        /// <param name="instrument">The instrument to which the price belongs</param>
        /// <param name="xRate">The xRate involved</param>
        public Price(Decimal quantity, ICurrency underlying, IInstrument instrument, decimal xRate)
        {
            this.quantity = quantity;
            this.underlying = underlying;
            this.instrument = instrument;
            this.xRate = xRate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class.
        /// </summary>
        /// <param name="money">This is the amount that one particular instrument to which the price belongs would cost</param>
        /// <param name="instrument">The instrument to which the price belongs</param>
        public Price(Money money, IInstrument instrument)
        {
            this.quantity = money.Quantity;
            this.underlying = (ICurrency)money.Underlying;
            this.instrument = instrument;
            this.XRate = money.XRate;
        }

        /// <summary>
        /// The actual quantity of the price, just a number
        /// </summary>
        public decimal Quantity
        {
            get { return this.quantity; }
            internal set { this.quantity = value; }
        }

        /// <summary>
        /// The currency the price is in
        /// </summary>
        public ICurrency Underlying
        {
            get { return this.underlying; }
            internal set { this.underlying = value; }
        }

        /// <summary>
        /// The currency the price is in
        /// </summary>
        public decimal XRate
        {
            get { return this.xRate; }
            set { this.xRate = value; }
        }

        /// <summary>
        /// The instrument to which the price belongs
        /// </summary>
        public IInstrument Instrument
        {
            get { return this.instrument; }
            internal set { this.instrument = value; }
        }

        /// <summary>
        /// When the Underlying is null -> retrieve it from the Instrument
        /// </summary>
        public void GetUnderlyingFromInstrument()
        {
            if (Underlying == null && Instrument != null)
            {
                if (Instrument.IsTradeable)
                    Underlying = ((ITradeableInstrument)Instrument).CurrencyNominal;
                else
                    Underlying = (ICurrency)Instrument;
            }
        }

        /// <summary>
        /// This is the amount that one particular instrument to which the price belongs would cost
        /// </summary>
        public Money Amount
        {
            get
            {
                Money amount = null;
                try
                {
                    if (this.underlying != null)
                    {
                        if (!underlying.IsBase && underlying.ExchangeRate != null)
                            amount = new Money(quantity, underlying, underlying.BaseCurrency, underlying.ExchangeRate.Rate, true);
                        else
                            amount = new Money(quantity, underlying, true);
                    }
                }
                catch (Exception)
                {
                    if (amount == null && this.underlying != null)
                        amount = new Money(quantity, underlying, this.XRate, true);
                }
                return amount;
            }
        }

        /// <summary>
        /// A method that clones the current instance into a new instance with a new quantity
        /// </summary>
        /// <param name="quantity">The new quantity</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public new Price Clone(Decimal quantity)
        {
            return new Price(quantity, this.Underlying, this.Instrument, this.XRate);
        }

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        public override string ToString()
        {
            if (Amount != null && Instrument != null)
                return ShortDisplayString + " per " + Instrument.Name;
            else
                return "Empty";
        }

        /// <summary>
        /// Returns a readable string of an object of this class
        /// </summary>
        public string DisplayString
        {
            get { return this.ToString(); }
        }

        protected PricingTypes instrumentPriceType
        {
            get
            {
                PricingTypes priceType = PricingTypes.Direct;
                if (Instrument != null && Instrument.IsWithPrice)
                    priceType = ((IInstrumentsWithPrices)Instrument).PriceType;
                return priceType;
            }
        }

        /// <summary>
        /// Returns a short readable string of an object of this class (the amount)
        /// </summary>
        public string ShortDisplayString
        {
            get
            {
                switch (instrumentPriceType)
                {
                    case PricingTypes.Percentage:
                        return Quantity.ToString("##,##0.00####") + "%";
                    default: // PricingTypes.Direct
                        return (Amount != null ? Amount : new Money(0m, ((ITradeableInstrument)Instrument).CurrencyNominal)).ToString("##,##0.00####");
                }
            }
        }

        public decimal DisplayQuantity
        {
            get { return Quantity; }
        }

        /// <summary>
        /// This method rounds the quantity to the currency's default number of decimal places
        /// </summary>
        /// <returns>A new rounded instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public Price Round()
        {
            decimal newQuantity = Math.Round(this.Quantity, this.Underlying.DecimalPlaces);
            return new Price(newQuantity, this.Underlying, this.Instrument, this.XRate);
        }

        /// <summary>
        /// This method rounds the quantity to the requested number of decimal places
        /// </summary>
        /// <param name="decimals">the requested number of decimal places</param>
        /// <returns>A new rounded instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public Price Round(int decimals)
        {
            decimal newQuantity = Math.Round(this.Quantity, decimals);
            return new Price(newQuantity, this.Underlying, this.Instrument, this.XRate);
        }

        /// <summary>
        /// This method clones the current instance however it is returned with a positive (absolute) quantity
        /// </summary>
        /// <returns>A new absolute instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public Price Abs()
        {
            decimal newQuantity = Math.Abs(this.Quantity);
            return new Price(newQuantity, this.Underlying, this.Instrument, this.XRate);
        }

        /// <summary>
        /// This method gives back the weighted average of two size & price pairs
        /// </summary>
        /// <param name="price1">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <param name="size1">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <param name="price2">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <param name="size2">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class with the weighted price</returns>
        public static Price GetAveragePrice(Price price1, InstrumentSize size1, Price price2, InstrumentSize size2)
        {
            if (price1 == null && size1 == null && price2 == null && size2 == null)
            {
                return null;
            }

            if (price1 == null || size1 == null)
            {
                return price2;
            }
            if (price2 == null || size2 == null)
            {
                return price1;
            }

            if (price1.Underlying != price2.Underlying)
            {
                throw new ApplicationException("Both prices have to be in the same currency");
            }

            if (price1.Instrument != price2.Instrument)
            {
                throw new ApplicationException("Both prices have to be for the same Instrument");
            }

            if (size1.Underlying != size2.Underlying)
            {
                throw new ApplicationException("Both amounts/sizes have to be in the same instrument");
            }

            if (price1.Instrument != size1.Underlying)
            {
                throw new ApplicationException("The price and the size have to be for the same Instrument");
            }

            Money total = size1.CalculateAmount(price1) + size2.CalculateAmount(price2);
            return total / (size1 + size2);
        }

        #region MultiplyDivide

        /// <summary>
        /// This method multiplies an instance of a <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <param name="multiplier">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public static Price Multiply(Price lhs, decimal multiplier)
        {
            Price result = null;
            if ((object)lhs != null)
            {
                result = new Price((lhs.Quantity * multiplier), lhs.Underlying, lhs.Instrument, lhs.XRate);
            }
            return result;
        }

        /// <summary>
        /// This method multiplies an instance of a <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <param name="multiplier">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public static Price operator *(Price lhs, decimal multiplier)
        {
            return Multiply(lhs, multiplier);
        }

        /// <summary>
        /// This method divides an instance of a <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <param name="divider">The number that is divided with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public static Price Divide(Price lhs, decimal divider)
        {
            Price result = null;
            if ((object)lhs != null)
            {
                result = new Price((lhs.Quantity / divider), lhs.Underlying, lhs.Instrument, lhs.XRate);
            }
            return result;
        }

        /// <summary>
        /// This method divides an instance of a <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <param name="divider">The number that is divided with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public static Price operator /(Price lhs, decimal divider)
        {
            return Divide(lhs, divider);
        }

        /// <summary>
        /// This method returns an <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> by multiplying a price with a size
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <param name="rhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money Multiply(Price lhs, InstrumentSize rhs)
        {
            return Multiply(lhs, rhs, lhs.XRate, false);
        }

        /// <exclude/>
        public static Money Multiply(Price lhs, InstrumentSize rhs, decimal xRate, bool noRounding)
        {
            if (lhs == null || rhs == null)
            {
                return null;
            }

            if (lhs.IsZero || rhs.IsZero)
            {
                // return a zero value amount
                return lhs.Amount.ZeroedAmount();
            }

            if (lhs.Instrument.Key != rhs.Underlying.Key)
            {
                throw new ApplicationException("The price and the size have to be for the same Instrument");
            }

            decimal contractSize = 1M;
            if (lhs.Instrument.IsTradeable)
                contractSize = System.Convert.ToDecimal(((ITradeableInstrument)lhs.Instrument).ContractSize);

            // check for conversion to active currency
            bool convertToActiveCurrency = (lhs.Underlying != null && lhs.Underlying.ToCurrency.IsObsoleteCurrency);

            Money amount = null;
            if (noRounding || convertToActiveCurrency)
                amount = new Money(lhs.Amount.Quantity * rhs.Quantity * rhs.Underlying.PriceTypeFactor * contractSize, lhs.Underlying, xRate, true);
            else
            {
                amount = (lhs.Amount * rhs.Quantity * rhs.Underlying.PriceTypeFactor * contractSize);
                amount.XRate = xRate;
            }
            if (amount != null && convertToActiveCurrency)
                amount = amount.AmountInActiveCurrency();
            return amount;
        }

        /// <summary>
        /// Calculate the amount using a size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Money CalculateAmount(InstrumentSize size)
        {
            return size.CalculateAmount(this, false);
        }

        /// <summary>
        /// This method returns an <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> by multiplying a price with a size
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <param name="rhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money operator *(Price lhs, InstrumentSize rhs)
        {
            return Multiply(lhs, rhs, lhs.XRate, false);
        }

        #endregion

        #region AddSubtract

        /// <summary>
        /// This method adds two instances of a <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class together
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the right hand side</param>
        /// <returns>The sum. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public static Price Add(Price lhs, Price rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Add);
        }

        /// <summary>
        /// This method adds two instances of a <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class together
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the right hand side</param>
        /// <returns>The sum. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public static Price operator +(Price lhs, Price rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Add);
        }


        /// <summary>
        /// This method subtracts two instances of a <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the right hand side</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public static Price Subtract(Price lhs, Price rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Subtract);
        }

        /// <summary>
        /// This method subtracts two instances of a <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the right hand side</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public static Price operator -(Price lhs, Price rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Subtract);
        }

        private static Price MathOperation(Price lhs, Price rhs, MathOperator op)
        {
            if (((Object)rhs == null) && ((Object)lhs == null))
            {
                //throw new ApplicationException("Both Prices may not be Null!");
                return null;
            }
            else if (((Object)rhs == null) || ((Object)lhs == null))
            {
                if ((Object)rhs == null)
                {
                    return (Price)lhs.MemberwiseClone();
                }
                else
                {
                    switch (op)
                    {
                        case MathOperator.Add:
                            return (Price)rhs.MemberwiseClone();
                        case MathOperator.Subtract:
                            return (rhs * -1);
                        default:
                            throw new ApplicationException("You did not select a valid math operation");
                    }
                }
            }
            else
            {
                // Use Eq because it is not possible to do operator overloading on an interface
                if (!(lhs.Underlying.Equals(rhs.Underlying)))
                {
                    throw new ApplicationException("Cannot add Two different currencies!");
                }
                else if (!(lhs.Instrument.Equals(rhs.Instrument)))
                {
                    throw new ApplicationException("Cannot add Two different instruments!");
                }
                else
                {
                    switch (op)
                    {
                        case MathOperator.Add:
                            return lhs.Clone(lhs.Quantity + rhs.Quantity);
                        case MathOperator.Subtract:
                            return lhs.Clone(lhs.Quantity - rhs.Quantity);
                        default:
                            throw new ApplicationException("You did not select a valid math operation");
                    }
                }
            }
        }

        #endregion

        #region Equality

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="obj">Price object to compare to</param>
        /// <returns>true if equal, false if not equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is System.DBNull)
                {
                    return false;
                }
                else
                {
                    return (this == (Price)obj);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check wether two instances of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class are equal
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the right hand side</param>
        /// <returns>true if equal, false if not equal.</returns>
        public static bool operator ==(Price lhs, Price rhs)
        {
            if (((Object)rhs == null) && ((Object)lhs == null))
            {
                return true;
            }
            else if (((Object)rhs == null) || ((Object)lhs == null))
            {
                return false;
            }
            else
            {
                // Use Eq because it is not possible to do operator overloading on an interface
                if ((lhs.Underlying.Equals(rhs.Underlying)) && (lhs.Instrument.Equals(rhs.Instrument)) && (lhs.Quantity == rhs.Quantity))
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
        /// Not equality operator
        /// </summary>
        /// <param name="obj">Price object to compare to</param>
        /// <returns>true if not equal, false if equal.</returns>
        public bool NotEquals(object obj)
        {
            return !(this == (Price)obj);
        }

        /// <summary>
        /// Check wether two instances of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class are not equal
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the right hand side</param>
        /// <returns>true if not equal, false if equal.</returns>
        public static bool operator !=(Price lhs, Price rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Overridden creation of a hashcode.
        /// </summary>
        /// <returns>Integer containing the id of the price</returns>
        public override int GetHashCode()
        {
            return this.Underlying.GetHashCode();
        }

        #endregion

        #region Conversion

        /// <summary>
        /// This method converts the current instance into a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class with another underlying currency by passing in the exrate to use
        /// </summary>
        /// <param name="rate">The rate to use</param>
        /// <param name="newCurrency">The new currency the amount will be in</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public Price Convert(decimal rate, ICurrency newCurrency)
        {
            decimal exRate = 1M;
            if (!newCurrency.IsBase)
                exRate = Math.Round(1m / rate, 7);
            return new Price(this.Quantity * rate, newCurrency, Instrument, exRate);
        }

        #endregion

        #region Comparing

        /// <summary>
        /// Does the quantity equal zero
        /// </summary>
        public bool IsZero
        {
            get
            {
                if (this.Quantity == 0)
                { return true; }
                else
                { return false; }
            }
        }

        /// <summary>
        /// Does the quantity not equal zero
        /// </summary>
        public bool IsNotZero
        {
            get
            {
                if (this.Quantity == 0)
                { return false; }
                else
                { return true; }
            }
        }

        /// <summary>
        /// Check wether one instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class is greater than the other 
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the right hand side</param>
        /// <returns>true if the left instance is greater.</returns>
        public static bool operator >(Price lhs, Price rhs)
        {
            return CompareOperation(lhs, rhs, CompareOperator.Greater);
        }

        /// <summary>
        /// Check wether one instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class is greater or equal than the other 
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the right hand side</param>
        /// <returns>true if the left instance is greater or equal.</returns>
        public static bool operator >=(Price lhs, Price rhs)
        {
            return CompareOperation(lhs, rhs, CompareOperator.GreaterOrEqual);
        }

        /// <summary>
        /// Check wether one instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class is less than the other 
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the right hand side</param>
        /// <returns>true if the left instance is less.</returns>
        public static bool operator <(Price lhs, Price rhs)
        {
            return CompareOperation(lhs, rhs, CompareOperator.Smaller);
        }

        /// <summary>
        /// Check wether one instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class is less or equal than the other 
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class on the right hand side</param>
        /// <returns>true if the left instance is less or equal.</returns>
        public static bool operator <=(Price lhs, Price rhs)
        {
            return CompareOperation(lhs, rhs, CompareOperator.SmallerOrEqual);
        }

        private static bool CompareOperation(Price lhs, Price rhs, CompareOperator cp)
        {
            bool result;

            if (((Object)rhs == null) || ((Object)lhs == null))
            {
                throw new ApplicationException("One or both Prices can not be Null!");
            }
            else
            {
                // Use Eq because it is not possible to do operator overloading on an interface
                if (!(lhs.Underlying.Equals(rhs.Underlying)))
                {
                    throw new ApplicationException("Cannot compare prices with different currencies");
                }
                else if (!(lhs.Instrument.Equals(rhs.Instrument)))
                {
                    throw new ApplicationException("Cannot compare prices with different instruments");
                }
                else
                {
                    switch (cp)
                    {
                        case CompareOperator.Greater:
                            result = (lhs.Quantity > rhs.Quantity);
                            break;
                        case CompareOperator.GreaterOrEqual:
                            result = (lhs.Quantity >= rhs.Quantity);
                            break;
                        case CompareOperator.Smaller:
                            result = (lhs.Quantity < rhs.Quantity);
                            break;
                        case CompareOperator.SmallerOrEqual:
                            result = (lhs.Quantity <= rhs.Quantity);
                            break;
                        default:
                            throw new ApplicationException("You did not select a valid comparison option");
                    }
                    return result;
                }
            }
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compare two instances of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class
        /// <param name="obj">size object to compare to</param>
        /// <returns>true if equal, false if not equal.</returns>
        public int CompareTo(object obj)
        {
            if (obj is Price)
            {
                Price temp = (Price)obj;
                ///Quick Fix MJN 26/09/2005..
                ///needed for demonstration
                ///
                int compareUnderlyingNames = 0;
                if ((this.Underlying.Name == null) && (temp.Underlying.Name == null))
                    compareUnderlyingNames = 0;
                else if (this.Underlying.Name == null)
                    compareUnderlyingNames = -1;
                else if (temp.Underlying.Name == null)
                    compareUnderlyingNames = 1;
                else
                    compareUnderlyingNames = this.Underlying.Name.CompareTo(temp.Underlying.Name);

                //int compareUnderlyingNames = this.Underlying.Name.CompareTo(temp.Underlying.Name);

                if (compareUnderlyingNames != 0)
                    return compareUnderlyingNames;
                else
                    return this.Quantity.CompareTo(temp.Quantity);
            }

            throw new ArgumentException("InstrumentSize.CompareTo: object is not an InstrumentSize");
        }

        #endregion

        #region Private Variables

        private decimal quantity;
        private ICurrency underlying;
        private decimal xRate;
        private IInstrument instrument;

        #endregion
    }
}

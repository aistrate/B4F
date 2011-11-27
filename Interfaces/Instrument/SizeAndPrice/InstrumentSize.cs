using System;
using System.Collections.Generic;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// The instrument size class is used to work with sizes of instruments as a single piece of information.
    /// The class is immutable.
    /// </summary>
    public class InstrumentSize : IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class.
        /// </summary>
        protected InstrumentSize() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class based upon a size and an instrument.
        /// </summary>
        /// <param name="quantity">The quantity of the instrument</param>
        /// <param name="underlying">The instrument involved</param>
        public InstrumentSize(Decimal quantity, IInstrument underlying)
        {
            if (underlying == null)
                throw new ApplicationException("Underlying can not be null when creating an instrumentsize object");

            this.quantity = Math.Round(quantity, underlying.DecimalPlaces);
            this.underlying = underlying;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class based upon a existing size instance (clone).
        /// </summary>
        /// <param name="existing">An existing instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        public InstrumentSize(InstrumentSize existing)
        {
            this.Quantity = existing.Quantity;
            this.Underlying = existing.Underlying;
        }

        #region NoRounding

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class
        /// </summary>
        /// <param name="quantity">The quantity of the instrument</param>
        /// <param name="underlying">The instrument involved</param>
        /// <param name="noRounding">This argument takes care that the quantity is not rounded to the instrument's default number of decimal places</param>
        internal InstrumentSize(Decimal quantity, IInstrument underlying, bool noRounding)
        {
            if (underlying == null)
                throw new ApplicationException("Underlying can not be null when creating an instrumentsize object");
            this.noRounding = noRounding;

            if (NoRounding)
                this.quantity = quantity;
            else
                this.quantity = Math.Round(quantity, underlying.DecimalPlaces);
            this.underlying = underlying;
        }

        #endregion

        /// <summary>
        /// The actual quantity of the size, just a number
        /// </summary>
        public Decimal Quantity
        {
            get { return this.quantity; }
            internal set { this.quantity = value; }
        }

        /// <summary>
        /// The instrument of the size
        /// </summary>
        public IInstrument Underlying
        {
            get
            {
                return this.underlying;
            }
            internal set
            {
                this.underlying = value;
            }
        }

        /// <summary>
        /// A read-only property to see if the quantity was rounded on instantiating
        /// </summary>
        protected bool NoRounding
        {
            get { return this.noRounding; }
        }

        /// <summary>
        /// A method that clones the current instance into a new instance with a new quantity
        /// </summary>
        /// <param name="quantity">The new quantity</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public InstrumentSize Clone(Decimal quantity)
        {
            return new InstrumentSize(quantity, this.Underlying);
        }

        public InstrumentSize CloneToParent()
        {

                return CloneToParent(this.quantity);


        }


        public InstrumentSize CloneToParent(Decimal quantity)
        {
            return new InstrumentSize(quantity, this.Underlying.TopParentInstrument);
        }


        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        public override string ToString()
        {
            return this.ToString("##,##0.#######");
        }

        public string ToString(string format)
        {
            string retValue = "";
            if (Underlying != null)
            {
                if (NoRounding)
                {
                    if (Underlying.IsCash)
                        retValue = ((ICurrency)Underlying).AltSymbol + " ";
                    return retValue + Quantity.ToString(format);
                }
                else
                {
                    retValue = Underlying.DisplayToString(this.Quantity);
                }
            }
            return retValue;
        }

        /// <summary>
        /// Returns a readable string of an object of this class
        /// </summary>
        public string DisplayString
        {
            get { return this.ToString(); }
        }

        /// <summary>
        /// Returns whether the quantity is either zero or positive (true) or negative (false)
        /// </summary>
        public bool Sign
        {
            get
            {
                if (Quantity >= 0)
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
        /// This property returns the significant number of decimals
        /// </summary>
        public short NumberOfDecimals
        {
            get
            {
                short retVal = 0;

                if (Quantity != 0)
                {
                    long intVal = (long)Quantity;
                    decimal decVal = Quantity;

                    for (int i = 1; (decimal)intVal != decVal; i++)
                    {
                        int factor = (int)Math.Pow(10, i);
                        intVal = (long)(Quantity * factor);
                        decVal = (Quantity * factor);
                        retVal++;
                    }
                }
                return retVal;
            }
        }

        public void AddTick(short decimals, bool sign)
        {
            int factor = (int)Math.Pow(10, decimals);
            decimal tick = 1 / factor;
            if (sign)
                Quantity += tick;
            else
                Quantity -= tick;
        }

        /// <summary>
        /// Is the underlying instrument of type Cash
        /// </summary>
        public bool IsMoney
        {
            get
            {
                return Underlying.SecCategory.IsCash;
            }
        }

        /// <summary>
        /// This method clones the data of this instance into a <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class instance
        /// </summary>
        /// <returns>A new <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class instance</returns>
        public Money GetMoney()
        {
            Money money = null;
            if (IsMoney)
            {
                money = new Money(this.Quantity, Underlying.ToCurrency, true);
            }
            return money;
        }

        /// <summary>
        /// This method returns a price of the instrument with a given quantity
        /// </summary>
        /// <param name="quantity">The quantity of the price being returned</param>
        /// <returns>The price object</returns>
        public Price GetPrice(decimal quantity)
        {
            if (Underlying.IsCash)
                return new Price(quantity, (ICurrency)Underlying, Underlying);
            else
            {
                ITradeableInstrument inst = (ITradeableInstrument)Underlying;
                return new Price(quantity, inst.CurrencyNominal, Underlying);
            }
        }

        /// <summary>
        /// Calculate the amount using a price
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public Money CalculateAmount(Price price)
        {
            return CalculateAmount(price, false);
        }

        /// <summary>
        /// Calculate the amount using a price
        /// </summary>
        /// <param name="price"></param>
        /// <param name="noRounding"></param>
        /// <returns></returns>
        public Money CalculateAmount(Price price, bool noRounding)
        {
            if (price.Instrument.IsTradeable)
                return Price.Multiply(price, InstrumentSize.Multiply(this, ((ITradeableInstrument)price.Instrument).ContractSize), price.XRate, noRounding);
            else
                return Price.Multiply(price, this, price.XRate, noRounding);
        }

        /// <summary>
        /// Returns the short name of the underlying instrument (when of type Cash).
        /// </summary>
        public string UnderlyingShortName
        {
            get
            {
                if (Underlying.IsCash)
                {
                    return Underlying.ToCurrency.Symbol;
                }
                else
                    return Underlying.DisplayName;
            }
        }

        /// <summary>
        /// This method rounds the quantity to the instrument's default number of decimal places
        /// </summary>
        /// <returns>A new rounded instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public virtual InstrumentSize Round()
        {
            return new InstrumentSize(Math.Round(quantity, this.Underlying.DecimalPlaces), this.Underlying);
        }

        /// <summary>
        /// This method rounds the quantity to the requested number of decimal places
        /// </summary>
        /// <param name="decimals">the requested number of decimal places</param>
        /// <returns>A new rounded instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public virtual InstrumentSize Round(int decimals)
        {
            return new InstrumentSize(Math.Round(quantity, decimals), this.Underlying);
        }

        /// <summary>
        /// This method clones the current instance however it is returned with a positive (absolute) quantity
        /// </summary>
        /// <returns>A new absolute instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public virtual InstrumentSize Abs()
        {
            return new InstrumentSize(Math.Abs(quantity), this.Underlying);
        }

        public virtual InstrumentSize Negate()
        {
            return new InstrumentSize(Decimal.Negate(quantity), this.Underlying);
        }

        #region AddSubtract

        /// <summary>
        /// This method adds two instances of a <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class together
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>The sum. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public static InstrumentSize Add(InstrumentSize lhs, InstrumentSize rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Add, false);
        }

        /// <exclude/>
        public static InstrumentSize Add(InstrumentSize lhs, InstrumentSize rhs, bool noRounding)
        {
            return MathOperation(lhs, rhs, MathOperator.Add, noRounding);
        }

        /// <summary>
        /// This method adds two instances of a <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class together
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>The sum. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public static InstrumentSize operator +(InstrumentSize lhs, InstrumentSize rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Add, false);
        }


        /// <summary>
        /// This method subtracts two instances of a <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public static InstrumentSize Subtract(InstrumentSize lhs, InstrumentSize rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Subtract, false);
        }

        /// <exclude/>
        public static InstrumentSize Subtract(InstrumentSize lhs, InstrumentSize rhs, bool noRounding)
        {
            return MathOperation(lhs, rhs, MathOperator.Subtract, noRounding);
        }

        /// <summary>
        /// This method subtracts two instances of a <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public static InstrumentSize operator -(InstrumentSize lhs, InstrumentSize rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Subtract, false);
        }

        private static InstrumentSize MathOperation(InstrumentSize lhs, InstrumentSize rhs, MathOperator op, bool noRounding)
        {
            if (((Object)rhs == null) && ((Object)lhs == null))
            {
                //throw new ApplicationException("Both Instruments may not be Null!");
                return null;
            }
            else if (((Object)rhs == null) || ((Object)lhs == null))
            {
                if ((Object)rhs == null)
                {
                    return (InstrumentSize)lhs.MemberwiseClone();
                }
                else
                {
                    switch (op)
                    {
                        case MathOperator.Add:
                            return (InstrumentSize)rhs.MemberwiseClone();
                        case MathOperator.Subtract:
                            return InstrumentSize.Multiply(rhs, -1M, noRounding);
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
                    string errorMessage;
                    if (lhs.IsMoney)
                    {
                        errorMessage = "Cannot add Two different currencies!";
                    }
                    else
                    {
                        errorMessage = "Cannot add two different Intruments";
                    }
                    throw new ApplicationException(errorMessage);
                }
                else
                {
                    switch (op)
                    {
                        case MathOperator.Add:
                            return new InstrumentSize(lhs.Quantity + rhs.Quantity, lhs.Underlying, noRounding);
                        case MathOperator.Subtract:
                            return new InstrumentSize(lhs.Quantity - rhs.Quantity, lhs.Underlying, noRounding);
                        default:
                            throw new ApplicationException("You did not select a valid math operation");
                    }
                }
            }
        }
        #endregion

        #region MultiplyDivide

        /// <summary>
        /// This method multiplies an instance of a <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <param name="multiplier">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public static InstrumentSize Multiply(InstrumentSize lhs, decimal multiplier)
        {
            return Multiply(lhs, multiplier, false);
        }

        /// <exclude/>
        public static InstrumentSize Multiply(InstrumentSize lhs, decimal multiplier, bool noRounding)
        {
            InstrumentSize result = null;
            if ((object)lhs != null)
            {
                result = new InstrumentSize(lhs.Quantity * multiplier, lhs.Underlying, noRounding);
            }
            return result;
        }

        /// <summary>
        /// This method multiplies an instance of a <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <param name="multiplier">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public static InstrumentSize operator *(InstrumentSize lhs, decimal multiplier)
        {
            return Multiply(lhs, multiplier, false);
        }

        /// <summary>
        /// This method divides an instance of a <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <param name="divider">The number that is divided with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public static InstrumentSize Divide(InstrumentSize lhs, decimal divider)
        {
            return Divide(lhs, divider, false);
        }

        /// <exclude/>
        public static InstrumentSize Divide(InstrumentSize lhs, decimal divider, bool noRounding)
        {
            InstrumentSize result = null;
            if ((object)lhs != null)
            {
                result = new InstrumentSize(lhs.Quantity / divider, lhs.Underlying, noRounding);
            }
            return result;
        }


        /// <summary>
        /// This method divides two instances of a <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class with one and other
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>The result</returns>
        public static Decimal Divide(InstrumentSize lhs, InstrumentSize rhs)
        {
            if (!((lhs == null) || (rhs == null)))
                return Decimal.Divide(lhs.Quantity, rhs.Quantity);
            else
                return 0m;
        }

        /// <summary>
        /// This method divides an instance of a <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <param name="divider">The number that is divided with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public static InstrumentSize operator /(InstrumentSize lhs, decimal divider)
        {
            return Divide(lhs, divider, false);
        }

        /// <summary>
        /// This method divides two instances of a <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class with one and other
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>The result</returns>
        public static decimal operator /(InstrumentSize lhs, InstrumentSize rhs)
        {
            return Divide(lhs, rhs);
        }

        /// <summary>
        /// This method returns an <see cref="T:B4F.TotalGiro.Instruments.Money">amount</see> by multiplying a size by a price
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <param name="rhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money Multiply(InstrumentSize lhs, Price rhs)
        {
            // implementation is in the Price class
            return Price.Multiply(rhs, lhs, rhs.XRate, false);
        }

        /// <summary>
        /// This method returns an <see cref="T:B4F.TotalGiro.Instruments.Money">amount</see> by multiplying a size by a price
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <param name="rhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money Multiply(InstrumentSize lhs, Price rhs, bool noRounding)
        {
            // implementation is in the Price class
            return Price.Multiply(rhs, lhs, rhs.XRate, noRounding);
        }


        /// <summary>
        /// This method returns an <see cref="T:B4F.TotalGiro.Instruments.Money">amount</see> by multiplying a size by a price
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <param name="rhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <returns>The result</returns>
        public static Money operator *(InstrumentSize lhs, Price rhs)
        {
            return Multiply(lhs, rhs, false);
        }

        /// <summary>
        /// The method returns a clone of the size object but with a zero quantity
        /// </summary>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public InstrumentSize ZeroedAmount()
        {
            return new InstrumentSize(0m, this.underlying);
        }

        #endregion

        #region Equality

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="obj">Size object to compare to</param>
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
                    return (this == (InstrumentSize)obj);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check wether two instances of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class are equal
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>true if equal, false if not equal.</returns>
        public static bool operator ==(InstrumentSize lhs, InstrumentSize rhs)
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
                if ((lhs.Underlying.Equals(rhs.Underlying)) && (lhs.Quantity == rhs.Quantity))
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
        /// <param name="obj">Size object to compare to</param>
        /// <returns>true if not equal, false if equal.</returns>
        public bool NotEquals(object obj)
        {
            return !(this == (InstrumentSize)obj);
        }

        /// <summary>
        /// Check wether two instances of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class are not equal
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>true if not equal, false if equal.</returns>
        public static bool operator !=(InstrumentSize lhs, InstrumentSize rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Overridden creation of a hashcode.
        /// </summary>
        /// <returns>Integer containing the id of the size</returns>
        public override int GetHashCode()
        {
            return this.Underlying.GetHashCode();
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
        /// Is the quantity within the supplied boundaries
        /// </summary>
        /// <param name="tolerance">The supplied boundary</param>
        /// <returns>True when the quantity is within the supplied boundaries</returns>
        public bool IsWithinTolerance(decimal tolerance)
        {

            if (this.Quantity == 0)
            { return true; }
            else
            {
                bool answer = false;
                if (Math.Abs(this.Quantity) <= Math.Abs(tolerance))
                {
                    answer = true;
                }
                return answer;
            }
        }

        public bool IsGreaterThanZero
        {
            get
            {
                return this.Quantity > 0m;
            }
        }

        public bool IsGreaterThanOrEqualToZero
        {
            get
            {
                return this.Quantity >= 0m;
            }
        }

        public bool IsLessThanZero
        {
            get
            {
                return this.Quantity < 0m;
            }
        }

        public bool IsLessThanOrEqualToZero
        {
            get
            {
                return this.Quantity <= 0m;
            }
        }

        /// <summary>
        /// Check wether one instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class is greater than the other 
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>true if the left instance is greater.</returns>
        public static bool operator >(InstrumentSize lhs, InstrumentSize rhs)
        {
            return CompareOperation(lhs, rhs, CompareOperator.Greater);
        }

        /// <summary>
        /// Check wether one instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class is greater or equal than the other 
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>true if the left instance is greater or equal.</returns>
        public static bool operator >=(InstrumentSize lhs, InstrumentSize rhs)
        {
            return CompareOperation(lhs, rhs, CompareOperator.GreaterOrEqual);
        }

        /// <summary>
        /// Check wether one instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class is less than the other 
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>true if the left instance is less.</returns>
        public static bool operator <(InstrumentSize lhs, InstrumentSize rhs)
        {
            return CompareOperation(lhs, rhs, CompareOperator.Smaller);
        }

        /// <summary>
        /// Check wether one instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class is less or equal than the other 
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>true if the left instance is less or equal.</returns>
        public static bool operator <=(InstrumentSize lhs, InstrumentSize rhs)
        {
            return CompareOperation(lhs, rhs, CompareOperator.SmallerOrEqual);
        }

        private static bool CompareOperation(InstrumentSize lhs, InstrumentSize rhs, CompareOperator cp)
        {
            bool result;

            if (((Object)rhs == null) || ((Object)lhs == null))
            {
                throw new ApplicationException("One or both Instruments can not be Null!");
            }
            else
            {
                // Use Eq because it is not possible to do operator overloading on an interface
                if (!(lhs.Underlying.Equals(rhs.Underlying)))
                {
                    throw new ApplicationException("Cannot compare two different instruments");
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
        /// Compare two instances of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class
        /// </summary>
        /// <param name="obj">size object to compare to</param>
        /// <returns>true if equal, false if not equal.</returns>
        public int CompareTo(object obj)
        {
            if (obj is InstrumentSize)
            {
                InstrumentSize temp = (InstrumentSize)obj;
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

        private Decimal quantity;
        private IInstrument underlying;
        private bool noRounding = false;

        #endregion
    }
}

using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Dal;
using System.Linq;
using System.ComponentModel;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// The instrument size class is used to work with amounts of currencies as a single piece of information.
    /// It inherits form <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class.
    /// The class is immutable.
    /// </summary>
    public class Money : InstrumentSize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class.
        /// </summary>
        protected Money() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with the supplied quantity, currency and exrate details
        /// </summary>
        /// <param name="quantity">The quantity (just a number) of the amount</param>
        /// <param name="underlying">The currency the amount is in</param>
        /// <param name="xRate">The rate (number) between the currency and the reference XRatecurrency</param>
        public Money(Decimal quantity, ICurrency underlying, Decimal xRate)
            : base(quantity, underlying)
        {
            this.XRateCurrency = underlying.BaseCurrency;
            this.XRate = xRate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with the supplied quantity, currency and exrate details
        /// </summary>
        /// <param name="quantity">The quantity (just a number) of the amount</param>
        /// <param name="underlying">The currency the amount is in</param>
        /// <param name="xRateCurrency">The currency used as a reference</param>
        /// <param name="xRate">The rate (number) between the currency and the reference XRatecurrency</param>
        public Money(Decimal quantity, ICurrency underlying, ICurrency xRateCurrency, Decimal xRate)
            : base(quantity, underlying)
        {
            this.XRateCurrency = xRateCurrency;
            this.XRate = xRate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class from an existing Money instance and exrate details.
        /// </summary>
        /// <param name="existing">An existing instance of <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class which is being cloned</param>
        /// <param name="xRateCurrency">The currency used as a reference</param>
        /// <param name="xRate">The rate (number) between the currency and the reference XRatecurrency</param>
        public Money(InstrumentSize existing, ICurrency xRateCurrency, Decimal xRate)
            : base(existing)
        {
            this.XRateCurrency = xRateCurrency;
            this.XRate = xRate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class from an existing Money instance but with an new quantity
        /// The ExRate Info is also retrieved from the existing instance.
        /// </summary>
        /// <param name="quantity">The quantity (just a number) of the amount</param>
        /// <param name="underlying">The currency the amount is in</param>
        public Money(Decimal quantity, ICurrency underlying)
            : base(quantity, underlying) { }

        #region NoRounding

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class
        /// </summary>
        /// <param name="Quantity">The quantity (just a number) of the amount</param>
        /// <param name="Underlying">The currency the amount is in</param>
        /// <param name="XRateCurrency">The currency used as a reference</param>
        /// <param name="XRate">The rate (number) between the currency and the reference XRatecurrency</param>
        /// <param name="noRounding">This argument takes care that the quantity is not rounded to the instrument's default number of decimal places</param>
        internal Money(Decimal quantity, ICurrency underlying, ICurrency xRateCurrency, Decimal xRate, bool noRounding)
            : base(quantity, underlying, noRounding)
        {
            this.XRateCurrency = xRateCurrency;
            this.XRate = xRate;
        }

        internal Money(Decimal quantity, ICurrency underlying, Decimal xRate, bool noRounding)
            : base(quantity, underlying, noRounding)
        {
            this.XRate = xRate;
        }

        internal Money(Decimal quantity, ICurrency underlying, bool noRounding)
            : base(quantity, underlying, noRounding)
        {
        }

        #endregion

        /// <summary>
        /// The currency used as a reference to compare to (probably the base currency of the TotalGiro system)
        /// </summary>
        public virtual ICurrency XRateCurrency
        {
            get
            {
                if (xRateCurrency == null)
                {
                    if (Underlying != null)
                        xRateCurrency = ((ICurrency)Underlying).BaseCurrency;
                }
                return xRateCurrency;
            }
            set { xRateCurrency = value; }
        }

        /// <summary>
        /// The rate (number) between the currency and the reference XRatecurrency
        /// </summary>
        public virtual Decimal XRate
        {
            get
            {
                if (xrate == 0)
                {
                    if (Underlying != null)
                    {
                        ICurrency underlying = (ICurrency)Underlying;
                        if (underlying.IsBase)
                            xrate = 1M;
                        else if (underlying.ParentInstrument != null && ((ICurrency)underlying.ParentInstrument).IsBase && underlying.LegacyExchangeRate != 0M)
                            xrate = underlying.LegacyExchangeRate;
                        else if (underlying.ExchangeRate != null)
                            xrate = underlying.ExchangeRate.Rate;
                    }
                }
                return xrate;
            }
            set { xrate = value; }
        }

        /// <summary>
        /// A method that returns the amount in base currency using the stored exchangerate
        /// </summary>
        public Money BaseAmount
        {
            get
            {
                ICurrency underlying = (ICurrency)this.Underlying;
                if (underlying.IsBase)
                    return this;
                else
                {
                    if (underlying.ParentInstrument != null && underlying.LegacyExchangeRate != 0M)
                        return new Money(this.Quantity / underlying.LegacyExchangeRate, (ICurrency)underlying.ParentInstrument).BaseAmount;
                    else
                    {
                        if (this.XRate == 0)
                            throw new ApplicationException("Exchange rate can not be 0 when converting to base amount.");

                        return new Money(this.Quantity / this.XRate, underlying.BaseCurrency);
                    }
                }
            }
        }

        /// <summary>
        /// A method that returns the amount in base currency using the current exchange rate
        /// </summary>
        public Money CurrentBaseAmount
        {
            get
            {
                ICurrency underlying = (ICurrency)this.Underlying;
                if (underlying.IsBase)
                    return this;
                else
                {
                    if (underlying.ParentInstrument != null && underlying.LegacyExchangeRate != 0M)
                        return new Money(this.Quantity / underlying.LegacyExchangeRate, (ICurrency)underlying.ParentInstrument).CurrentBaseAmount;
                    else
                    {
                        if (underlying.ExchangeRate == null)
                            throw new ApplicationException("The current Exchange rate value can not be null when converting to current base amount.");
                        return new Money(this.Quantity / underlying.ExchangeRate.Rate, underlying.BaseCurrency);
                    }
                }
            }
        }

        /// <summary>
        /// A method that returns the amount in active currency whenever the current currency is obsolete
        /// </summary>
        public Money AmountInActiveCurrency()
        {
            return AmountInActiveCurrency(DateTime.Now);
        }

        /// <summary>
        /// A method that returns the amount in active currency whenever the current currency is obsolete
        /// </summary>
        public Money AmountInActiveCurrency(DateTime date)
        {
            ICurrency underlying = (ICurrency)this.Underlying;
            if (underlying.ParentInstrument != null && underlying.LegacyExchangeRate != 0M && date >= underlying.InActiveDate)
                return new Money(this.Quantity / underlying.LegacyExchangeRate, (ICurrency)underlying.ParentInstrument).CurrentBaseAmount;
            else
                return this;
        }

        /// <summary>
        /// Calculate the size using a price
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public InstrumentSize CalculateSize(Price price)
        {
            return CalculateSize(price, false);
        }

        /// <summary>
        /// Calculate the size using a price
        /// </summary>
        /// <param name="price"></param>
        /// <param name="noRounding"></param>
        /// <returns></returns>
        public InstrumentSize CalculateSize(Price price, bool noRounding)
        {
            InstrumentSize size = Money.Divide(this, price, noRounding);
            //if (price.Instrument.IsTradeable)
            //    return InstrumentSize.Divide(size, ((ITradeableInstrument)price.Instrument).ContractSize);
            //else
                return size;
        }


        /// <exclude/>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public new Money CalculateAmount(Price price)
        {
            throw new ApplicationException("Not supported on this class");
        }

        /// <exclude/>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public new Money CalculateAmount(Price price, bool noRounding)
        {
            throw new ApplicationException("Not supported on this class");
        }

        /// <summary>
        /// A method that clones the current instance into a new instance with a new quantity
        /// </summary>
        /// <param name="quantity">The new quantity</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public new Money Clone(Decimal quantity)
        {
            return new Money(quantity, (ICurrency)this.Underlying, this.XRateCurrency, this.XRate);
        }

        /// <exclude/>
        public new Money Clone(Decimal quantity, bool noRounding)
        {
            return new Money(quantity, (ICurrency)this.Underlying, this.XRateCurrency, this.XRate, noRounding);
        }

        /// <exclude/>
        protected new Money Clone(Decimal quantity, Decimal weightedXRate, bool noRounding)
        {
            return new Money(quantity, (ICurrency)this.Underlying, this.XRateCurrency, weightedXRate, noRounding);
        }

        /// <summary>
        /// This method rounds the quantity to the instrument's default number of decimal places
        /// </summary>
        /// <returns>A new rounded instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public new Money Round()
        {
            return new Money(Math.Round(this.Quantity, this.Underlying.DecimalPlaces), (ICurrency)this.Underlying, this.XRateCurrency, this.XRate);
        }

        /// <summary>
        /// This method rounds the quantity to the requested number of decimal places
        /// </summary>
        /// <param name="decimals">the requested number of decimal places</param>
        /// <returns>A new rounded instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public new Money Round(int decimals)
        {
            return new Money(Math.Round(this.Quantity, decimals), (ICurrency)this.Underlying, this.XRateCurrency, this.XRate);
        }

        /// <summary>
        /// This method clones the current instance however it is returned with a positive (absolute) quantity
        /// </summary>
        /// <returns>A new absolute instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public new Money Abs()
        {
            return new Money(Math.Abs(Quantity), (ICurrency)this.Underlying, this.XRateCurrency, this.XRate);
        }

        public virtual Money Negate()
        {
            return new Money(Decimal.Negate(this.Quantity), (ICurrency)this.Underlying, this.XRateCurrency, this.XRate);
        }

        /// <summary>
        /// The method returns a clone of the money object but with a zero quantity
        /// </summary>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public new Money ZeroedAmount()
        {
            return new Money(0m, (ICurrency)this.Underlying, this.XRateCurrency, this.XRate);
        }

        public override string ToString()
        {
            return base.ToString("##,##0.00####");
        }

        #region Comparison



        #endregion

        #region Conversion

        /// <summary>
        /// This method converts the current instance into a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with another underlying currency
        /// </summary>
        /// <param name="newCurrency">The currency to amount should be converted to</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        ///<remarks>
        ///The amount is converted using the last known exchange rates of the instruments
        ///</remarks>
        ///<example>
        ///The following situations of buy or sell transactions can occur with foreign exchange rates:
        ///USD is only used as an example for a foreign currency.
        ///
        ///    InstCur AcctCur Side    Rate    Bid of Ask
        ///1a  USD     EUR     Buy     EUR/USD /Bid
        ///1b  USD     EUR     Sell    EUR/USD /Ask
        ///2a  EUR     USD     Buy     EUR/USD /Ask
        ///2b  EUR     USD     Sell    EUR/USD /Bid
        ///
        ///4a  GBP     USD     Buy     GBP/USD Bid/Ask
        ///4b  GBP     USD     Sell    GBP/USD Ask/Bid
        ///5a  USD     GBP     Buy     USD/GBP Bid/Ask
        ///5b  USD     GBP     Sell    USD/GBP Ask/Bid
        ///
        ///The following examples make the situations described above more clearly:
        ///Keep in mind that the base currency is EUR.
        ///
        ///    Cur TrdVal  Rate     Total      Cur Translation
        ///1a  USD 40000   0.8750   45,714.29  EUR Buy USD, Sell EUR
        ///1b  USD 40000   0.8850   45,197.74  EUR Sell USD, buy EUR
        ///2a  EUR 40000   0.8850   35,400.00  USD Buy EUR, Sell USD
        ///2b  EUR 40000   0.8750   35,000.00  USD Sell EUR, Buy USD
        ///
        ///4a  GBP 40000   0.6794   27,174.10  USD Buy GBP/Sell EUR and Sell USD/Buy EUR
        ///4b  GBP 40000   0.7010   28,039.91  USD Sell GBP/Buy EUR and Buy USD/Sell EUR
        ///5a  USD 40000   1.4265   57,061.53  GBP Buy USD/Sell EUR and Sell GBP/Buy EUR
        ///5b  USD 40000   1.4720   58,879.59  GBP Sell USD/Buy EUR and Buy GBP/Sell EUR
        ///
        ///Rates
        ///                    BID           MID                     ASK
        ///EUR/USD             0.8750        0.8800                  0.8850
        ///EUR/GBP             0.6012        0.6073                  0.6134
        ///</example>
        public Money Convert(ICurrency newCurrency)
        {
            decimal rate = 1m;
            if (newCurrency == null)
                throw new ApplicationException("Can not convert to a null currency.");

            if (Underlying == null)
                throw new ApplicationException("Can not convert a null currency.");

            if (newCurrency.Equals((ICurrency)Underlying))
                return this;
            else if ((ICurrency)Underlying.ParentInstrument != null && ((ICurrency)Underlying).LegacyExchangeRate != 0M)
            {
                if (newCurrency.Equals((ICurrency)Underlying.ParentInstrument))
                    return new Money(this.Quantity / ((ICurrency)Underlying).LegacyExchangeRate, newCurrency, newCurrency.BaseCurrency, ((ICurrency)Underlying).LegacyExchangeRate);
                else
                    return new Money(this.Quantity / ((ICurrency)Underlying).LegacyExchangeRate, (ICurrency)Underlying.ParentInstrument, newCurrency.BaseCurrency, ((ICurrency)Underlying).LegacyExchangeRate).Convert(newCurrency);
            }
            else
            {
                rate = GetExRate(((ICurrency)Underlying).ExchangeRate, newCurrency.ExchangeRate, 0M);
                return new Money(this.Quantity * rate, newCurrency, newCurrency.BaseCurrency, newCurrency.ExchangeRate.Rate);
            }
        }

        /// <summary>
        /// This method converts the current instance into a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with another underlying currency taking the <see cref="T:B4F.TotalGiro.Orders.Side">side</see> into account
        /// </summary>
        /// <param name="newCurrency">The currency to amount should be converted to</param>
        /// <param name="side">The <see cref="T:B4F.TotalGiro.Orders.Side">side</see> of the transaction</param>
        /// <param name="date">The date of the exchange rate that should be used to convert</param>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        /// <seealso cref="M:B4F.TotalGiro.Instruments.Money.Convert(B4F.TotalGiro.Instruments.ICurrency)">Exchangerate example</seealso>
        public Money Convert(ICurrency newCurrency, Side side, DateTime date, IDalSession session)
        {
            IExRate from;
            IExRate to;
            ICurrency underlying = (ICurrency)Underlying;

            if (newCurrency == null)
            {
                throw new ApplicationException("Can not convert to a null currency.");
            }

            if (underlying == null)
            {
                throw new ApplicationException("Can not convert a null currency.");
            }

            if (underlying != underlying.BaseCurrency)
            {
                if (underlying.ParentInstrument != null && underlying.LegacyExchangeRate != 0M && date >= underlying.InActiveDate)
                    return new Money(this.Quantity / underlying.LegacyExchangeRate, (ICurrency)underlying.ParentInstrument).Convert(newCurrency, side, date, session);
                else
                {
                    from = underlying.GetHistoricalExRate(session, date);
                    //if (from == null && underlying.ParentInstrument != null && underlying.LegacyExchangeRate != 0M && Util.IsNotNullDate(underlying.InActiveDate))
                    //    return new Money(this.Quantity / underlying.LegacyExchangeRate, (ICurrency)underlying.ParentInstrument).Convert(newCurrency, side, date, session);
                }
            }
            else
            {
                from = underlying.ExchangeRate;
            }

            if (newCurrency != newCurrency.BaseCurrency)
            {
                if (newCurrency.ParentInstrument != null && newCurrency.LegacyExchangeRate != 0M && date >= newCurrency.InActiveDate)
                    return new Money(this.Quantity * newCurrency.LegacyExchangeRate, newCurrency);
                else
                {
                    to = newCurrency.GetHistoricalExRate(session, date);
                    //if (to == null && newCurrency.ParentInstrument != null && newCurrency.LegacyExchangeRate != 0M && Util.IsNotNullDate(newCurrency.InActiveDate))
                    //    return new Money(this.Quantity * newCurrency.LegacyExchangeRate, newCurrency);
                }
            }
            else
            {
                to = newCurrency.ExchangeRate;
            }


            decimal rate = GetExRate(from, to, side);
            decimal exRate = 1M;
            if (!newCurrency.IsBase)
                exRate = Math.Round(1m / rate, 7);
            return new Money(this.Quantity * rate, newCurrency, exRate);
        }

        /// <summary>
        /// This method converts the current instance into a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with another underlying currency by passing in the exrate to use
        /// </summary>
        /// <param name="rate">The rate to use</param>
        /// <param name="newCurrency">The new currency the amount will be in</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public Money Convert(decimal rate, ICurrency newCurrency)
        {
            if (rate == 0)
                return Convert(newCurrency);
            else
            {
                decimal exRate = 1M;
                if (!newCurrency.IsBase)
                    exRate = Math.Round(1m / rate, 7);
                return new Money(this.Quantity * rate, newCurrency, exRate);
            }
        }

        /// <exclude/>
        public Money Convert(decimal rate, ICurrency newCurrency, int decimals)
        {
            if (rate == 0)
                throw new ApplicationException("No Rate was supplied");
            else
            {
                decimal exRate = 1M;
                if (!newCurrency.IsBase)
                    exRate = Math.Round(1m / rate, 7);
                return new Money(Math.Round(this.Quantity * rate, decimals), newCurrency, exRate);
            }
        }

        /// <summary>
        /// This method converts the current instance into a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class in base currency
        /// </summary>
        /// <param name="rate">The rate to use</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public Money ConvertToBase(decimal rate)
        {
            return ConvertToBase(rate, false);
        }

        /// <summary>
        /// This method converts the current instance into a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class in base currency
        /// </summary>
        /// <param name="rate">The rate to use</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public Money ConvertToBase(decimal rate, bool noRounding)
        {
            if (rate == 0)
                return Convert(Underlying.ToCurrency.BaseCurrency);
            else
                return new Money(this.Quantity * rate, Underlying.ToCurrency.BaseCurrency, noRounding);
        }

        /// <summary>
        /// Return the exchange rate between to currencies taking the <see cref="T:B4F.TotalGiro.Orders.Side">side</see> into account 
        /// </summary>
        /// <param name="fromCurrency">The from currency to compare</param>
        /// <param name="toCurrency">The to currency to compare</param>
        /// <param name="side">The <see cref="T:B4F.TotalGiro.Orders.Side">side</see> of the transaction</param>
        /// <returns>The rate</returns>
        /// <seealso cref="M:B4F.TotalGiro.Instruments.Money.Convert(B4F.TotalGiro.Instruments.ICurrency)">Exchangerate example</seealso>
        public static decimal GetExRate(IExRate fromCurrency, IExRate toCurrency, Side side)
        {
            decimal rate;
            decimal tempRate;
            IInstrument baseCurrency;

            // Trap the error if an exchange rate is missing in the dictionary.
            if (fromCurrency == null)
            {
                throw new ApplicationException("From Currency is mandatory when calculating the exchange rate");
            }

            // use system currency if ToCurrency is omitted
            if (toCurrency == null)
            {
                throw new ApplicationException("To Currency is mandatory when calculating the exchange rate");
            }

            baseCurrency = fromCurrency.Currency.BaseCurrency;
            if (fromCurrency.Currency != toCurrency.Currency)
            {
                if (fromCurrency.Currency != baseCurrency && toCurrency.Currency != baseCurrency)
                {
                    // Calculate the exchange rate from information stored in the recordset.
                    tempRate = getRateToBase((IExRate)baseCurrency, toCurrency, side, baseCurrency);
                    Side oppSide = (Side)((int)side * -1);
                    rate = (tempRate / getRateToBase((IExRate)baseCurrency, fromCurrency, oppSide, baseCurrency));
                    rate = Math.Round(rate, TG_EXRATE_PRECISION);
                }
                else
                {
                    rate = getRateToBase(fromCurrency, toCurrency, side, baseCurrency);
                }
            }
            else
            {
                rate = 1;
            }
            return rate;
        }

        private static decimal getRateToBase(IExRate fromCurrency, IExRate toCurrency, Side side, IInstrument baseCurrency)
        {
            decimal rate = 1;

            if (fromCurrency.Currency != baseCurrency)
            {
                Side oppSide = (Side)((int)side * -1);
                rate = (1 / getRate(fromCurrency, oppSide));
            }
            else if (toCurrency.Currency != baseCurrency)
            {
                rate = getRate(toCurrency, side);
            }
            return Math.Round(rate, TG_EXRATE_PRECISION);
        }

        private static decimal getRate(IExRate currency, Side side)
        {
            decimal rate;

            rate = currency.Rate;

            if (side == Side.Buy)
            {
                // Buy -> Ask
                // Ask is the percentage which you add at the rate
                rate += (rate * currency.Ask / 100);
            }
            else if (side == Side.Sell)
            {
                // Sell -> Bid
                // Bid is the percentage which you subtract at the rate
                rate -= (rate * currency.Bid / 100);
            }
            rate *= currency.PriceFactor;
            return rate;
        }

        #endregion

        #region AddSubtract

        /// <summary>
        /// This method adds two instances of a <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class together
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class on the right hand side</param>
        /// <returns>The sum. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money Add(Money lhs, Money rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Add, false);
        }

        /// <exclude/>
        public static Money Add(Money lhs, Money rhs, bool noRounding)
        {
            return MathOperation(lhs, rhs, MathOperator.Add, noRounding);
        }

        /// <summary>
        /// This method adds two instances of a <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class together
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class on the right hand side</param>
        /// <returns>The sum. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money operator +(Money lhs, Money rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Add, false);
        }


        /// <summary>
        /// This method subtracts two instances of a <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class on the right hand side</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money Subtract(Money lhs, Money rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Subtract, false);
        }

        /// <exclude/>
        public static Money Subtract(Money lhs, Money rhs, bool noRounding)
        {
            return MathOperation(lhs, rhs, MathOperator.Subtract, noRounding);
        }

        /// <summary>
        /// This method subtracts two instances of a <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class on the right hand side</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money operator -(Money lhs, Money rhs)
        {
            return MathOperation(lhs, rhs, MathOperator.Subtract, false);
        }

        private static Money MathOperation(Money lhs, Money rhs, MathOperator op, bool noRounding)
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
                    return (Money)lhs.MemberwiseClone();
                }
                else
                {
                    switch (op)
                    {
                        case MathOperator.Add:
                            return (Money)rhs.MemberwiseClone();
                        case MathOperator.Subtract:
                            return Money.Multiply(rhs, -1M, noRounding);
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
                else
                {
                    decimal weightedXRate = 1M;
                    if (!((ICurrency)lhs.Underlying).IsBase)
                    {
                        decimal totalAbsQuantity = Math.Abs(lhs.Quantity) + Math.Abs(rhs.Quantity);
                        weightedXRate = Math.Round(totalAbsQuantity != 0m ? (Math.Abs(lhs.Quantity * lhs.XRate) + Math.Abs(rhs.Quantity * rhs.XRate)) / totalAbsQuantity :
                                                                  (lhs.XRate + rhs.XRate) / 2M, 7);
                    }
                    switch (op)
                    {
                        case MathOperator.Add:
                            return lhs.Clone(lhs.Quantity + rhs.Quantity, weightedXRate, noRounding);
                        case MathOperator.Subtract:
                            return lhs.Clone(lhs.Quantity - rhs.Quantity, weightedXRate, noRounding);
                        default:
                            throw new ApplicationException("You did not select a valid math operation");
                    }
                }
            }
        }

        #endregion

        #region MultiplyDivide

        /// <summary>
        /// This method multiplies an instance of a <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</param>
        /// <param name="multiplier">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money Multiply(Money lhs, decimal multiplier)
        {
            return Multiply(lhs, multiplier, false);
        }

        public bool EqualCurrency(Money rhs)
        {
            return this.Underlying == rhs.Underlying;
        }

        public bool EqualCurrency(ICurrency rhs)
        {
            return (ICurrency)this.Underlying == rhs;
        }

        /// <exclude/>
        public static Money Multiply(Money lhs, decimal multiplier, bool noRounding)
        {
            Money result = null;
            if ((object)lhs != null)
            {
                result = lhs.Clone(lhs.Quantity * multiplier, lhs.XRate, noRounding);
            }
            return result;
        }

        /// <summary>
        /// This method multiplies an instance of a <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</param>
        /// <param name="multiplier">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money operator *(Money lhs, decimal multiplier)
        {
            return Multiply(lhs, multiplier, false);
        }

        /// <summary>
        /// This method divides an instance of a <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</param>
        /// <param name="divider">The number that is divided with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money Divide(Money lhs, decimal divider)
        {
            return Divide(lhs, divider, false);
        }

        /// <exclude/>
        public static Money Divide(Money lhs, decimal divider, bool noRounding)
        {
            Money result = null;
            if ((object)lhs != null)
            {
                result = lhs.Clone(lhs.Quantity / divider, lhs.XRate, noRounding);
            }
            return result;
        }

        /// <summary>
        /// This method divides an instance of a <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</param>
        /// <param name="divider">The number that is divided with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        public static Money operator /(Money lhs, decimal divider)
        {
            return Divide(lhs, divider, false);
        }


        /// <summary>
        /// This method returns an <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> by dividing an amount by a size
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</param>
        /// <param name="rhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public static Price Divide(Money lhs, InstrumentSize rhs)
        {
            return Divide(lhs, rhs, false);
        }

        /// <exclude/>
        public static Price Divide(Money lhs, InstrumentSize rhs, bool noRounding)
        {
            if (lhs == null || rhs == null)
            {
                return null;
            }

            decimal contractSize = 1M;
            if (rhs.Underlying.IsTradeable)
                contractSize = ((ITradeableInstrument)rhs.Underlying).ContractSize;

            Money priceAmount = Money.Divide((lhs / (rhs.Underlying.PriceTypeFactor * contractSize)), rhs.Quantity, noRounding);
            return new Price(priceAmount, rhs.Underlying);
        }

        ///// <summary>
        ///// This method returns an <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> by dividing an amount by a size
        ///// </summary>
        ///// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</param>
        ///// <param name="rhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        ///// <param name="decimalPlaces">The number of decimals allowed</param>
        ///// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        //public static Price Divide(Money lhs, InstrumentSize rhs, int decimalPlaces)
        //{
        //    if (lhs == null || rhs == null)
        //    {
        //        return null;
        //    }

        //    if (rhs.IsZero)
        //        throw new ApplicationException("You can not divide by zero");

        //    decimal contractSize = 1M;
        //    if (rhs.Underlying.IsTradeable)
        //        contractSize = ((ITradeableInstrument)rhs.Underlying).ContractSize;

        //    decimal priceQty = Math.Round((lhs.Quantity / (rhs.Quantity * contractSize)) / rhs.Underlying.PriceTypeFactor, decimalPlaces);
        //    return new Price(priceQty, (ICurrency)lhs.Underlying, rhs.Underlying, lhs.XRate);
        //}

        /// <summary>
        /// This method returns an <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> by dividing an amount by a size
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</param>
        /// <param name="rhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</returns>
        public static Price operator /(Money lhs, InstrumentSize rhs)
        {
            return Divide(lhs, rhs, false);
        }

        /// <summary>
        /// This method returns an <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> by dividing an amount by a price
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</param>
        /// <param name="rhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public static InstrumentSize Divide(Money lhs, Price rhs)
        {
            return Divide(lhs, rhs, false);
        }

        /// <exclude/>
        public static InstrumentSize Divide(Money lhs, Price rhs, bool noRounding)
        {
            if (lhs == null || rhs == null)
                return null;

            decimal obsoleteFactor = 1M;
            if (!(lhs.Underlying.Equals(rhs.Underlying)))
            {
                if (!rhs.Underlying.IsObsoleteCurrency)
                    throw new ApplicationException("The currencies must be the same when multiplying size and price");
                else
                    obsoleteFactor = rhs.Underlying.LegacyExchangeRate;
            }

            decimal contractSize = 1M;
            if (rhs.Instrument.IsTradeable)
                contractSize = ((ITradeableInstrument)rhs.Instrument).ContractSize;

            return new InstrumentSize(((lhs.Quantity * obsoleteFactor) / (rhs.Quantity * contractSize)) / rhs.Instrument.PriceTypeFactor, rhs.Instrument, noRounding);
        }

        /// <summary>
        /// This method returns an <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> by dividing an amount by a price
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</param>
        /// <param name="rhs">The instance of the <see cref="T:B4F.TotalGiro.Instruments.Price">Price</see> class</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class</returns>
        public static InstrumentSize operator /(Money lhs, Price rhs)
        {
            return Divide(lhs, rhs, false);
        }

        #endregion

        #region Private Variables

        private ICurrency xRateCurrency;
        private Decimal xrate;
        private const int TG_EXRATE_PRECISION = 7;

        #endregion

        #region Crap

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with the passed in quantity and base currency
        /// </summary>
        /// <param name="Quantity">The quantity (just a number) of the amount</param>
        /// <param name="BaseCurrency">The base currency the amount is in</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        //public static Money GetAmountInBaseCurrency(Decimal Quantity, ICurrency BaseCurrency)
        //{
        //    return new Money(Quantity, BaseCurrency);
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class with the passed in quantity and the account's base currency
        /// </summary>
        /// <param name="Quantity">The quantity (just a number) of the amount</param>
        /// <param name="BaseAccount">The account who supplies its base currency</param>
        /// <returns>A new instance of the <see cref="T:B4F.TotalGiro.Instruments.Money">Money</see> class</returns>
        //public static Money GetAmountInBaseCurrency(Decimal Quantity, B4F.TotalGiro.Accounts.IAccount BaseAccount)
        //{
        //    return new Money(Quantity, BaseAccount.BaseCurrency);
        //}

        #endregion

    }
}

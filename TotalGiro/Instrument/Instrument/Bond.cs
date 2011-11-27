using System;
using System.Collections.Generic;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments.CorporateAction;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class representing a bond
    /// </summary>
    public class Bond : SecurityInstrument, IBond
    {
        public Bond()
        {
            initialize();
        }

        #region Props

        /// <summary>
	    /// The Nominal value of the bond
	    /// </summary>
        public virtual Money NominalValue
	    {
		    get { return nominalValue;}
		    set { nominalValue = value;}
	    }
		
        /// <summary>
        /// The coupon rate
        /// </summary>
	    public virtual decimal CouponRate
	    {
		    get { return couponRate;}
		    set { couponRate = value;}
	    }
	
	    /// <summary>
	    /// The frequency of dividend payment per year
	    /// </summary>
        public virtual Regularities CouponFreq
	    {
		    get { return couponFreq;}
		    set { couponFreq = value;}
	    }
	
	    /// <summary>
	    /// The date that the bond matures
	    /// </summary>
        public virtual DateTime MaturityDate
	    {
		    get { return maturityDate;}
		    set { maturityDate = value.Date;}
	    }
	
	    /// <summary>
	    /// The type of calculation for accrued interest
	    /// </summary>
        public virtual AccruedInterestCalcTypes AccruedInterestCalcType
	    {
		    get { return accruedInterestCalcType;}
		    set { accruedInterestCalcType = value;}
	    }

        public virtual bool DoesPayInterest 
        { 
            get { return (int)AccruedInterestCalcType > (int)AccruedInterestCalcTypes.Spaarbrief; }
        }

        /// <summary>
        /// The first coupon payment
        /// </summary>
        public virtual DateTime FirstCouponPaymntDate
        {
            get { return this.firstCouponPaymntDate; }
            set
            {
                if (Util.IsNotNullDate(value))
                {
                    // must be after the issue date
                    if (Util.IsNotNullDate(IssueDate))
                    {
                        if (IssueDate > value)
                            throw new ApplicationException("First Coupon Payment Date must be after or equal to the issue date.");
                    }
                    firstCouponPaymntDate = value;
                }
                else
                    if (DoesPayInterest)
                        throw new ApplicationException("Initial Coupon Payment Date is mandatory.");
            }
        }

        public virtual bool UltimoDating { get; set; }
        public virtual bool IsPerpetual { get; set; }
        public virtual bool IsFixedCouponRate { get; set; }
        public virtual Money RedemptionAmount { get; set; }

        /// <summary>
        /// The coupons that belong to this item.
        /// </summary>
        public virtual ICouponHistoryCollection Coupons
        {
            get
            {
                CouponHistoryCollection items = (CouponHistoryCollection)this.coupons.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return items;
            }
        }

        /// <summary>
        /// The coupons that belong to this item.
        /// </summary>
        public virtual IBondCouponRateHistoryCollection CouponRates
        {
            get
            {
                BondCouponRateHistoryCollection items = (BondCouponRateHistoryCollection)this.couponRates.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return items;
            }
        }

        public override bool SupportsStockDividend
        {
            get { return false; }
        }

        #endregion

        #region Methods

        public override bool Transform(DateTime changeDate, decimal oldChildRatio, byte newParentRatio, bool isSpinOff,
                string instrumentName, string isin, DateTime issueDate)
        {
            throw new ApplicationException("This method is not supported by the bond class");
        }

        public override bool Validate()
        {
            if ((int)this.accruedInterestCalcType < 0)
                throw new ApplicationException("The accruedInterestCalcType is mandatory.");
            if (this.nominalValue == null)
                throw new ApplicationException("The nominal Value is mandatory.");
            if (IsFixedCouponRate && this.couponRate <= 0 && DoesPayInterest)
                throw new ApplicationException("The couponRate has to be higher than 0.");
            if (this.couponFreq <= 0 && DoesPayInterest)
                throw new ApplicationException("The couponFreq has to be higher than 0.");
            if (Util.IsNullDate(this.firstCouponPaymntDate) && DoesPayInterest)
                throw new ApplicationException("The firstCouponPaymntDate is mandatory.");

            if (DoesPayInterest && this.UltimoDating && !IsUltimo(this.firstCouponPaymntDate))
                throw new ApplicationException("The firstCouponPaymntDate is not ultimo.");

            return base.validate();
        }

        public override TransactionFillDetails GetTransactionFillDetails(
            IOrderSizeBased order, Price price, DateTime settlementDate, IFeeFactory feeFactory,
            decimal fillRatio, IExchange exchange)
        {
            TransactionFillDetails details = base.GetTransactionFillDetails(order, price, settlementDate, feeFactory, fillRatio, exchange);

            // accrued interest
            if (DoesPayInterest)
            {
                AccruedInterestDetails calc = AccruedInterest(details.Size, settlementDate, exchange);
                if (calc.IsRelevant)
                    details.AccruedInterest = calc.AccruedInterest.Abs() * (decimal)order.Side * -1M;
            }
            return details;
        }

        protected override TransactionFillDetails getTransactionFillDetailsAmountBasedOrderByGoalSeek(
            Money grossAmount, Side side, bool isCommissionRelevant, bool isValueInclComm,
            DateTime settlementDate, Price price, IExchange exchange,
            ICommRule rule, ICommClient client, decimal servChargePerc, int precision)
        {
            decimal realAmount;
            decimal guess = grossAmount.Abs().CalculateSize(price).Quantity;
            FinancialMath.MaxCycles = 200;

            // Check -> use Commission
            bool useComm = true;
            bool useAddComm = false;
            if (!isCommissionRelevant || rule == null)
                useComm = false;

            if (useComm)
                useAddComm = (rule.AdditionalCalculation != null);

            realAmount = FinancialMath.GoalSeek(x =>
                new InstrumentSize(x, this).CalculateAmount(price).Quantity +
                (useComm ? rule.CommCalculation.Calculate(client.GetNewInstance(new InstrumentSize(x, this), price, (useAddComm ? rule.AdditionalCalculation.Calculate(client.GetNewInstance(new InstrumentSize(x, this), price)) :null) )).Quantity : 0M) +
                (useAddComm ? rule.AdditionalCalculation.Calculate(client.GetNewInstance(new InstrumentSize(x, this), price)).Quantity : 0M) +
                (DoesPayInterest ? this.AccruedInterest(new InstrumentSize(x, this), settlementDate, exchange).AccruedInterest.Quantity : 0M) +
                (new InstrumentSize(x, this).CalculateAmount(price).Abs().Quantity * servChargePerc),
                grossAmount.Abs().Quantity, guess, precision);

            InstrumentSize size = new InstrumentSize(realAmount, this);
            Money amount = size.CalculateAmount(price);
            InstrumentSize cleanSize = amount.CalculateSize(price);

            Money accInt = this.AccruedInterest(cleanSize, settlementDate, null).AccruedInterest;
            Money servCh = (amount.Abs() * servChargePerc);
            Money comm = amount.ZeroedAmount();
            Money addComm = amount.ZeroedAmount();
            if (useComm)
            {
                if (rule.AdditionalCalculation != null)
                    addComm = rule.AdditionalCalculation.Calculate(client.GetNewInstance(cleanSize, price));
                comm = rule.CommCalculation.Calculate(client.GetNewInstance(cleanSize, price, addComm));

                // if sell -> comm is already in the amount
                if (side == Side.Sell && (comm + addComm) != null && (comm + addComm).IsNotZero)
                {
                    amount += (comm + addComm);
                    cleanSize = amount.CalculateSize(price);
                    if (!isValueInclComm)
                    {
                        if (rule.AdditionalCalculation != null)
                            addComm = rule.AdditionalCalculation.Calculate(client.GetNewInstance(cleanSize, price));
                        comm = rule.CommCalculation.Calculate(client.GetNewInstance(cleanSize, price, addComm));
                    }
                }
            }
            return new TransactionFillDetails(cleanSize, amount, accInt, servCh, servChargePerc, comm + addComm, grossAmount.Abs(), side);
        }



        public decimal GetCouponRate(DateTime settlementDate)
        {
            DateTime lastCouponPaymentDate = CalculateLastCouponDate(FirstCouponPaymntDate, CouponFreq, UltimoDating, settlementDate);
            return GetCouponRate(lastCouponPaymentDate, settlementDate);
        }

        public decimal GetCouponRate(DateTime lastCouponPaymentDate, DateTime settlementDate)
        {
            decimal rate = CouponRate;
            if (!IsFixedCouponRate)
                rate = CouponRates.GetWeightedCouponRateForPeriod(lastCouponPaymentDate, settlementDate);
            return rate;
        }

        public InstrumentSize CalculateSizeBackwards(Money amount)
        {
            return CalculateSizeBackwards(amount, this.CurrentPrice.Price, GetSettlementDate(DateTime.Today, DefaultExchange ?? HomeExchange));
        }

        public InstrumentSize CalculateSizeBackwards(Money amount, Price price, DateTime settlementDate)
        {
            if (price == null)
                throw new ApplicationException(string.Format("It is not possible to calculate the size of {0} without a price", Name));

            if (Util.IsNullDate(settlementDate))
                throw new ApplicationException(string.Format("It is not possible to calculate the size of {0} without a valid settlement date", Name));

            DateTime lastCouponPaymentDate;
            DateTime nextCouponPaymentDate;
            decimal factor = ai_Factor(settlementDate, null, out lastCouponPaymentDate, out nextCouponPaymentDate);

            // Calculate backwards the number of bonds
            return amount.CalculateSize((price + price.Clone(factor * GetCouponRate(lastCouponPaymentDate, settlementDate))));
        }

        public override PredictedSize PredictSize(Money inputAmount)
        {
            PredictedSize retVal = new PredictedSize(PredictedSizeReturnValue.NoRate);
            Money amount = inputAmount;

            if (CurrentPrice != null)
            {
                retVal.RateDate = CurrentPrice.Date;
                if (!inputAmount.Underlying.Equals(CurrentPrice.Price.Underlying))
                    amount = inputAmount.Convert(CurrentPrice.Price.Underlying);
                retVal.Size = CalculateSizeBackwards(amount);
                retVal.Rate = currentPrice.Price.ToString();
            }
            return retVal;
        }

        /// <summary>
        /// The next coupon payment relative to the passed in settlement date
        /// </summary>
        /// <param name="settlementDate">the passed in settlement date</param>
        /// <returns>A date</returns>
        public DateTime NextCouponDate(DateTime settlementDate)
        {
            return NextCouponDate(settlementDate, DefaultExchange ?? HomeExchange);
        }

        /// <summary>
        /// The next coupon payment relative to the passed in settlement date
        /// </summary>
        /// <param name="settlementDate">the passed in settlement date</param>
        /// <returns>A date</returns>
        public DateTime NextCouponDate(DateTime settlementDate, IExchange exchange)
        {
            DateTime retVal = DateTime.MinValue;
            
            if (Util.IsNullDate(settlementDate))
                throw new ApplicationException("The settlement date can not be null");
    
            if (FirstCouponPaymntDate > settlementDate)
                return FirstCouponPaymntDate;
            else
            {
                if (CouponFreq > 0)
                {
                    retVal = CalculateLastCouponDate(FirstCouponPaymntDate, CouponFreq, UltimoDating, settlementDate);
                    //retVal = retVal.AddMonths(12 / (int)CouponFreq);

                    int months = 12 / (int)CouponFreq;
                    retVal = Util.DateAdd(DateInterval.Month, months, retVal, DateIntervalOptions.None, null);
                    if (UltimoDating)
                        retVal = Util.GetLastDayOfMonth(retVal);
                    if (!IsPerpetual && Util.IsNotNullDate(MaturityDate) && retVal > MaturityDate)
                        retVal = MaturityDate;
                }
            }
            return retVal;
        }

        /// <summary>
        /// The actual previous coupon payment. The previous coupon payment relative to the passed in settlement date
        /// </summary>
        /// <param name="settlementDate">the passed in settlement date</param>
        /// <returns>A date</returns>
        public DateTime LastCouponDate(DateTime settlementDate)
        {
            if (Util.IsNullDate(settlementDate))
                throw new ApplicationException("The settlement date can not be null");
            return CalculateLastCouponDate(FirstCouponPaymntDate, CouponFreq, UltimoDating, settlementDate);
        }

        public DateTime LastCouponPaymentDate(DateTime settlementDate)
        {
            //Coupon payment date maybe later then coupon date, this is depending on calender. ????
            DateTime dtmDate;
    
            if (Util.IsNullDate(settlementDate))
                throw new ApplicationException("The settlement date can not be null");

            dtmDate = LastCouponDate(settlementDate);
            if (Util.IsNotNullDate(dtmDate))
            {
                int days = 0;
                if (dtmDate.DayOfWeek == DayOfWeek.Saturday)
                    days = 2;
                else if (dtmDate.DayOfWeek == DayOfWeek.Sunday)
                    days = 1;
                dtmDate = dtmDate.AddDays(days);
            }
            return dtmDate;
        }

        /// <summary>
        /// Method that calculates the last coupon payment date relative to the passed in settlementDate
        /// </summary>
        /// <param name="firstCouponDate">The First Coupon payment date</param>
        /// <param name="freq">The coupon frequency</param>
        /// <param name="settlementDate">The passed in settlement date</param>
        /// <returns>A date</returns>
        public DateTime CalculateLastCouponDate(DateTime firstCouponDate, Regularities freq, bool ultimoDating, DateTime settlementDate)
        {
            // find the last coupon payment date.
            // first find the first payment date that is later than the settlement date ->
            // then take the previous date
            // Note, there is a Special case when the settlementdate is between the Issue Date
            // and the First Coupon Date. This has special consequences when the FirstCouponDate
            // is Greater (or less) then (IssueDate + Frequency) as in case of long Bonds.

            DateTime date;
            Int16 intI = 0; 
            
            // Return null date, when settlement date is before the first coupon payment date.
            if (firstCouponDate.Date > settlementDate.Date)
            {
                date = DateTime.MinValue;
            }
            else
            {
                date = firstCouponDate.Date;

                if (Util.IsNullDate(date))
                    throw new ApplicationException("The first coupon payment date can not be null");
                else
                {
                    while (settlementDate.Date > date.AddMonths(12 / (int)freq))
                    {
                        date = date.AddMonths(12 / (int)freq);
                        if (UltimoDating)
                            date = Util.GetLastDayOfMonth(date);
                        if (!IsPerpetual && Util.IsNotNullDate(MaturityDate) && date > MaturityDate)
                        {
                            date = MaturityDate;
                            break;
                        }
                        intI++;
                        if (intI >= 400) return DateTime.MinValue;
                    }
                }
            }
            return date;
        }

        /// <summary>
        /// Returns the interest which has accrued on a bond since it's last coupon date.
        /// </summary>
        /// <param name="size">The Bond size of the order/trade</param>
        /// <param name="tradeDate">The date the bond was traded</param>
        /// <param name="settlementPeriod">The used settlement period</param>
        /// <param name="exchange">The exchange the bond was traded on</param>
        /// <returns>The accrued interest</returns>
        public AccruedInterestDetails AccruedInterest(InstrumentSize size, DateTime tradeDate, Int16 settlementPeriod, IExchange exchange) // out int interestDays?
        {
            // if the settlementperiod is 0 then do not take holidays and weekends into account. 
            // This way the actual accrued interest on the date that is passed (TradeDate) to this function, can
            // be calculated. This is used in the EODValuations where the accrued interest of
            // a position is calculated for different valuation days. Without this modification
            // the accrued interest will stay the same for friday, saturday and sunday, and also
            // during exchange holidays. While on the portman reports you want to see the pure
            // accrued interest. For settlement periods that are greater than 0, the holidays and
            // weekends are taken into account. Because in the order entry screens the
            // settlementperiod is > 0, so it won't fuck up. In the EODValuations a settlementperiod
            // of 0 is passed, so the pure accrued interest is calculated.
            DateTime settlementDate;
            if (settlementPeriod == 0 && Util.IsNotNullDate(tradeDate))
            {
                //Do not take weekends and exchange holidays into account.
                settlementDate = tradeDate.Date;
            }
            else
            {
                //Get the settlement date, take holdidays in account.
                settlementDate = GetSettlementDate(tradeDate, exchange, settlementPeriod);
            }
            return AccruedInterest(size, settlementDate, exchange);
        }

        /// <summary>
        /// Returns the interest which has accrued on a bond since it's last coupon date.
        /// </summary>
        /// <param name="size">The Bond Volume of the order/trade</param>
        /// <param name="settlementDate">The used settlement date</param>
        /// <param name="exchange">The exchange the bond was traded on</param>
        /// <returns>The accrued interest</returns>
        public AccruedInterestDetails AccruedInterest(InstrumentSize size, DateTime settlementDate, IExchange exchange)
        {
            DateTime lastCouponPaymentDate;
            DateTime nextCouponPaymentDate;
            Money accruedInterest = new Money(0M, CurrencyNominal);
            AccruedInterestDetails retVal;

            if (Util.IsNullDate(settlementDate))
                throw new ApplicationException("Can not calculate the Accrued interst without a valid settlement date");

            if (size == null)
                throw new ApplicationException("Can not calculate the accrued interest without a specified Volume");

            if (size.IsZero)
            {
                nextCouponPaymentDate = NextCouponDate(settlementDate, exchange);
                lastCouponPaymentDate = LastCouponDate(settlementDate);
                return new AccruedInterestDetails(accruedInterest, 0M, couponRate, 
                    GetInterestDays(lastCouponPaymentDate, settlementDate.Date, nextCouponPaymentDate), 
                    lastCouponPaymentDate, nextCouponPaymentDate, settlementDate);
            }

            //Calculate the accrued interest based on the method.
            switch (AccruedInterestCalcType)
            {
                case AccruedInterestCalcTypes.Zero:
                case AccruedInterestCalcTypes.Spaarbrief:
                    //Zero coupon bonds --> no accrued interest & interestDays = 0
                    retVal = new AccruedInterestDetails(false);
                    break;
                default:
                    if (exchange == null)
                        exchange = DefaultExchange;

                    decimal factor = ai_Factor(
                        settlementDate.Date,
                        exchange,
                        out lastCouponPaymentDate,
                        out nextCouponPaymentDate);
                    decimal couponRate = GetCouponRate(lastCouponPaymentDate, settlementDate.Date);
                    accruedInterest = new Money((size.Quantity * couponRate / 100) * factor, NominalValue.Underlying.ToCurrency);

                    //days = (The number of days between the last Coupon Date and (Settlement date -1 ))
                    int days = GetInterestDays(lastCouponPaymentDate, settlementDate.Date, nextCouponPaymentDate);

                    retVal = new AccruedInterestDetails(accruedInterest, factor, couponRate, days, lastCouponPaymentDate, nextCouponPaymentDate, settlementDate);
                    break;
            }
            return retVal;                
        }

        public decimal AI_Factor(DateTime settlementDate)
        {
            DateTime lastCouponPaymentDate;
            DateTime nextCouponPaymentDate;
            return ai_Factor(settlementDate, null, out lastCouponPaymentDate, out nextCouponPaymentDate);
        }

        /// <summary>
        /// Method to calculate the accrued interest factor
        /// </summary>
        /// <param name="settlementDate"></param>
        /// <param name="exchange"></param>
        /// <param name="lastCouponPaymentDate"></param>
        /// <param name="nextCouponPaymentDate"></param>
        /// <returns></returns>
        protected decimal ai_Factor(
            DateTime settlementDate,
            IExchange exchange,
            out DateTime lastCouponPaymentDate, 
            out DateTime nextCouponPaymentDate)
        {

            // accruedInterestCalcType is day count method code
            // lastCouponPaymentDate is D1.M1.Y1 etc.
            // frequency is the coupon frequency
            // maturity is the maturity date
            int D1; int D1x; int M1; int Y1;     // variables used to hold day, month and year
            int D2; int D2x; int M2; int Y2;    // values separately
            int D3; int M3; int Y3; 
            DateTime Anchor;                    // "anchor" date: start date for notional...
            int AD; int AM; int AY;             // ...periods
            int WM; int WY;                      // "working dates" in notional period loop
            DateTime Target;                     // end date for notional period loop
            int n; int Nx;                      // number of interest-bearing days
            decimal Y;                          // length of a year (ISMA-Year)
            int L;                              // regular coupon length in months
            decimal C; decimal Cx;              // notional period length in days
            decimal Fx;                         // applicable coupon frequency
            bool Regular;                       // various flags
            int Direction;
            DateTime CurrC; DateTime NextC;     // used for temporary serial date values
            DateTime TempD; 
            int i;                              // temporary loop variable
            decimal retVal = 0;

            if (CouponFreq == 0)
                throw new ApplicationException("Can not calculate the Accrued interst without a specified Coupon frequenty");

            nextCouponPaymentDate = NextCouponDate(settlementDate.Date, exchange);
            lastCouponPaymentDate = CalculateLastCouponDate(FirstCouponPaymntDate, CouponFreq, UltimoDating, settlementDate);
            if (Util.IsNullDate(lastCouponPaymentDate))
                lastCouponPaymentDate = IssueDate;

            // Check input dates
            if (Util.IsNullDate(lastCouponPaymentDate) || Util.IsNullDate(settlementDate) || Util.IsNullDate(nextCouponPaymentDate) ||
                (settlementDate < lastCouponPaymentDate) || (nextCouponPaymentDate <= lastCouponPaymentDate) || (!IsPerpetual && Util.IsNullDate(MaturityDate)))
                throw new ApplicationException("Accrued Interest calculation error: not enough data for calculation");


            // Determine Number of Interest-bearing days, N
            switch (AccruedInterestCalcType)
            {
                case AccruedInterestCalcTypes.German_30_360: // RULE 1 -> JS 8-12-2004 added 30/act
                case AccruedInterestCalcTypes.ac30_ACT: 
                    D1 = lastCouponPaymentDate.Day; M1 = lastCouponPaymentDate.Month; Y1 = lastCouponPaymentDate.Year;
                    D2 = settlementDate.Day; M2 = settlementDate.Month; Y2 = settlementDate.Year;
                    if (D1 == 31)
                        D1x = 30;
                    else if (IsFebUltimo(lastCouponPaymentDate)) // end of February
                        D1x = 30;
                    else
                        D1x = D1;

                    if (D2 == 31)
                        D2x = 30;
                    else if (IsFebUltimo(settlementDate)) // end of February
                        D2x = 30;
                    else
                        D2x = D2;

                    n = (D2x - D1x) + 30 * (M2 - M1) + 360 * (Y2 - Y1);
                    break;

                case AccruedInterestCalcTypes.German_30E_360: // RULE 2
                    D1 = lastCouponPaymentDate.Day; M1 = lastCouponPaymentDate.Month; Y1 = lastCouponPaymentDate.Year;
                    D2 = settlementDate.Day; M2 = settlementDate.Month; Y2 = settlementDate.Year;

                    if (D1 == 31)
                        D1x = 30;
                    else
                        D1x = D1;

                    if (D2 == 31)
                        D2x = 30;
                    else
                        D2x = D2;

                    n = (D2x - D1x) + 30 * (M2 - M1) + 360 * (Y2 - Y1);
                    break;

                case AccruedInterestCalcTypes.English_ACT_365: // RULES 3, 4, 6, 7
                case AccruedInterestCalcTypes.French_ACT_360:
                case AccruedInterestCalcTypes.ACT_365_L:
                case AccruedInterestCalcTypes.ACT_ACT:
                case AccruedInterestCalcTypes.ACT_ACT_Ultimo:
                    n = Util.DateDiff(DateInterval.Day, lastCouponPaymentDate, settlementDate) - 1;
                    break;

                case AccruedInterestCalcTypes.US_30U_360: // RULE 5
                    D1 = lastCouponPaymentDate.Day; M1 = lastCouponPaymentDate.Month; Y1 = lastCouponPaymentDate.Year;
                    D2 = settlementDate.Day; M2 = settlementDate.Month; Y2 = settlementDate.Year;
                    
                    D1x = D1; D2x = D2;
                    if (IsFebUltimo(lastCouponPaymentDate) && IsFebUltimo(settlementDate))
                        D2x = 30;
                    if (IsFebUltimo(lastCouponPaymentDate))
                        D1x = 30;
                    if ((D2x == 31) && (D1x >= 30))
                        D2x = 30;
                    if (D1x == 31)
                        D1x = 30;
                    n = (D2x - D1x) + 30 * (M2 - M1) + 360 * (Y2 - Y1);
                    break;

                default:
                    throw new ApplicationException("Accrued Interest calculation error: unknown AccruedInterestCalcTypeID");
            }
            
            // Determine Basic Accrued Interest Factor
            switch (AccruedInterestCalcType)
            {
                case AccruedInterestCalcTypes.German_30_360: // RULES 8, 9, 11, 12
                case AccruedInterestCalcTypes.German_30E_360:
                case AccruedInterestCalcTypes.French_ACT_360:
                case AccruedInterestCalcTypes.US_30U_360: 
                    if (n != 0)
                        retVal = (decimal)n / 360M; // force double precision arithmetic!
                    break;

                case AccruedInterestCalcTypes.English_ACT_365: // RULE 10
                    if (n != 0)
                        retVal = (decimal)n / 365M; // force double precision arithmetic!
                    break;

                case AccruedInterestCalcTypes.ACT_365_L:
                    D1 = lastCouponPaymentDate.Day; M1 = lastCouponPaymentDate.Month; Y1 = lastCouponPaymentDate.Year;
                    D3 = nextCouponPaymentDate.Day; M3 = nextCouponPaymentDate.Month; Y3 = nextCouponPaymentDate.Year;
                    if (CouponFreq == Regularities.Annual) // RULE 14
                    {
                        i = Util.DateDiff(DateInterval.Day, lastCouponPaymentDate, nextCouponPaymentDate) - 1;
                        if ((i == 365) || (i == 366))
                            Y = i;
                        else
                        {
                            Y = 365;
                            for (i = Y1; i < Y3; i++)
                            {
                                TempD = GetUltimo(i, 2); // last day in February
                                if ((TempD.Day == 29) && (TempD > lastCouponPaymentDate) && (TempD <= nextCouponPaymentDate))
                                {
                                    Y = 366;
                                    break;
                                }
                            }
                        }
                    }
                    else // RULE 15
                    {
                        if (((Y3 % 4 == 0) && (Y3 % 100 != 0)) || (Y3 % 400 == 0))
                            Y = 366;
                        else
                            Y = 365;
                    }
                    retVal = (decimal)n / (decimal)Y; // RULE 13
                    break;
                
                case AccruedInterestCalcTypes.ac30_ACT: // -> JS 8-12-2004 added 30/act
                    C = Util.DateDiff(DateInterval.Day, lastCouponPaymentDate, nextCouponPaymentDate) - 1;
                    retVal = (decimal)(1 / (int)CouponFreq) * ((decimal)n / C);
                    break;
                    
                case AccruedInterestCalcTypes.ACT_ACT: 
                case AccruedInterestCalcTypes.ACT_ACT_Ultimo:
                    D1 = lastCouponPaymentDate.Day; M1 = lastCouponPaymentDate.Month; Y1 = lastCouponPaymentDate.Year;
                    D3 = nextCouponPaymentDate.Day; M3 = nextCouponPaymentDate.Month; Y3 = nextCouponPaymentDate.Year;
                    
                    // check whether the frequency is periodic or not and look if the period is regular
                    // set up default values (assume aperiodic, irregular unless otherwise)
                    // aperiodic
                    L = 12;             // regular period length in months
                    Fx = 1;             // applicable coupon frequency
                    Regular = false;

                    if (CouponFreq >= Regularities.Annual) // RULE 21
                    {
                        if ((12 / (int)CouponFreq) == (12M / (decimal)CouponFreq)) // RULES 19, 20
                        {
                            // periodic
                            L = 12 / (int)CouponFreq;   // regular period length in months
                            Fx = (decimal)CouponFreq;   // applicable coupon frequency
                            Regular = false;            // default: not regular

                            if (((Y3 - Y1) * 12 + (M3 - M1)) == L) // RULES 23, 24
                            {
                                if (AccruedInterestCalcType ==  AccruedInterestCalcTypes.ACT_ACT) // ISMA-99 Normal
                                {
                                    if (D1 == D3)
                                        Regular = true;
                                    else if (InvalidDate(Y1, M1, D3) && IsUltimo(lastCouponPaymentDate))
                                        Regular = true;
                                    else if (InvalidDate(Y3, M3, D1) && IsUltimo(nextCouponPaymentDate))
                                        Regular = true;
                                }
                                else // ISMA-99 Ultimo
                                {
                                    if (IsUltimo(lastCouponPaymentDate) && IsUltimo(nextCouponPaymentDate)) Regular = true;
                                }
                            }
                        }
                    }

                    if (Regular) // RULE 17
                    {
                        C =  Util.DateDiff(DateInterval.Day, lastCouponPaymentDate, nextCouponPaymentDate) - 1;
                        if (n != 0)
                            retVal = (1 / Fx) * (n / C);
                    }
                    else // generate notional periods
                    {
                        if (!IsPerpetual && nextCouponPaymentDate == MaturityDate) // RULE 18
                        {
                            Direction = 1; // ... forwards
                            Anchor = lastCouponPaymentDate;
                            AY = Y1; AM = M1; AD = D1;
                            Target = nextCouponPaymentDate;
                        }
                        else
                        {
                            Direction = -1; // ... backwards
                            Anchor = nextCouponPaymentDate;
                            AY = Y3; AM = M3; AD = D3;
                            Target = lastCouponPaymentDate;
                        }
                        
                        CurrC = Anchor; // start notional loop
                        i = 0;

                        while ((Direction * (CurrC - Target).Days) < 0)
                        {
                            i = i + Direction;
                            WY = GetNewYear(AY, AM, (i * L));   // next notional year and...
                            WM = GetNewMonth(AM, (i * L));       // ...month (handling year changes)
                            
                            if (AccruedInterestCalcType == AccruedInterestCalcTypes.ACT_ACT) // ISMA-99 Normal
                            {
                                if (InvalidDate(WY, WM, AD)) // RULE 23
                                    NextC = GetUltimo(WY, WM);
                                else
                                    NextC = new DateTime(WY, WM, AD);
                            }
                            else // ISMA-99 Ultimo
                            {
                                NextC = GetUltimo(WY, WM); // RULE 24
                            }

                            Nx = (Min(settlementDate, Max(NextC, CurrC)) - Max(lastCouponPaymentDate, Min(CurrC, NextC))).Days;
                            Cx = Direction * (NextC - CurrC).Days;
                            if (Nx > 0) // RULE 22
                                retVal = retVal + ((decimal)Nx / Cx); // RULE 21
                            CurrC = NextC;
                        }
                        if (retVal != 0)
                            retVal = retVal / Fx; // RULE 22
                    }
                    break;

                default:
                    break;
            }
            return retVal;
        }

        private static DateTime Min(DateTime A, DateTime B )
        {
            // lowest of two dates
            if (A < B) return A; else return B;
        }

        private static DateTime Max(DateTime C, DateTime D)
        {
            // greatest of two dates
            if (C > D) return C; else return D;
        }

        private static DateTime GetUltimo(int YY, int MM)
        {
            //last day in month MM.YY
            // NB: MM.YY must be valid
            // if MM = 12 then handles new year boundary
            DateTime tempDate = new DateTime(YY + (MM / 12), (MM % 12) + 1, 1);
            return tempDate.AddDays(-1);
        }

        private static bool IsUltimo(DateTime DS)
        {
            // is DS last day in month
            return (DS.AddDays(1).Day == 1);
        }

        private static bool IsFebUltimo(DateTime DS)
        {
            // is DS last day in February
            return (DS.AddDays(1).Day == 1) && (DS.AddDays(1).Month == 3);
        }

        private static bool InvalidDate(int YY, int MM, int DD)
        {
            // check if a valid date
            bool isDate = false;
            try
            {
                DateTime date = new DateTime(YY, MM, DD);
                isDate = true;
            }
            catch (ArgumentOutOfRangeException)
            {
                // do nothing
            }
            return !isDate;

        }

        private static int GetNewMonth(int MM, int Num)
        {
            // new month MM +/- Num months

            int NM; // NB: MM must be valid
            
            NM = MM + Num;
            if (NM > 0)
                return (NM - 1) % 12 + 1;
            else
                return 12 + (NM % 12);
        }

        private static int GetNewYear(int YY, int MM, int Num)
        {
            // get new year starting from MM.YY
            
            int NM; // going +/- Num months (MM.YY valid)
            NM = MM + Num;
            if (NM > 0)
                return YY + ((NM - 1) / 12);
            else
                return YY - 1 + (NM / 12);
        }

        public int GetInterestDays(DateTime lastCouponPaymentDate, DateTime settlementDate, DateTime nextCouponPaymentDate)
        {
            int intDayFrom;
            int intMonthFrom;
            int intYearFrom;
            int intDayTo;
            int intMonthTo;
            int intYearTo;

            // If no payment yet -> prev date is issue date
            if (Util.IsNullDate(lastCouponPaymentDate))
                lastCouponPaymentDate = IssueDate;

            //Validation.
            if (Util.IsNullDate(settlementDate) || Util.IsNullDate(lastCouponPaymentDate) || (lastCouponPaymentDate > settlementDate))
                throw new ApplicationException("Pass correct dates to GetInterestDays");

            intDayFrom = lastCouponPaymentDate.Day; intMonthFrom = lastCouponPaymentDate.Month; intYearFrom = lastCouponPaymentDate.Year;
            intDayTo = settlementDate.Day; intMonthTo = settlementDate.Month; intYearTo = settlementDate.Year;

            switch (AccruedInterestCalcType)
            {
                case AccruedInterestCalcTypes.German_30_360:
                    // Each month is treated as having 30 days, so a period from February 1, 2005 to April 1, 2005 is considered to be 60 days. 
                    // The year is considered to have 360 days. This convention is frequently chosen for ease of calcuation: the payments tend to be regular and at predictable amounts.

                    if (lastCouponPaymentDate.Day == 31)
                        intDayFrom -= 1;
                    else if (isEndOfFeb(lastCouponPaymentDate))
                        intDayFrom = 30;

                    if (settlementDate.Day == 31 && lastCouponPaymentDate.Day >= 30)
                        intDayTo -= 1;
                    else if (settlementDate.Day == 31 && lastCouponPaymentDate.Day < 30)
                        intDayTo += 1;
                    else if (isEndOfFeb(settlementDate))
                        intDayTo = 30;

                    return (intDayTo - intDayFrom) + (30 * (intMonthTo - intMonthFrom)) + (360 * (intYearTo - intYearFrom));

                case AccruedInterestCalcTypes.German_30E_360:
                    //Every month has 30 days, except for February.
                    if (intDayFrom == 31)
                        intDayFrom = 30;
                    if (intDayTo == 31)
                        intDayTo = 30;

                    return (intDayTo - intDayFrom) + (30 * (intMonthTo - intMonthFrom)) + (360 * (intYearTo - intYearFrom));

                case AccruedInterestCalcTypes.US_30U_360:
                    if (intDayFrom == 31)
                        intDayFrom = 30;
                    if (intDayTo == 31)
                    {
                        intDayTo = 1;
                        intMonthTo += 1;
                    }

                    return (intDayTo - intDayFrom) + (30 * (intMonthTo - intMonthFrom)) + (360 * (intYearTo - intYearFrom));

                case AccruedInterestCalcTypes.ac30_ACT:
                    if (lastCouponPaymentDate.Day == 31)
                        intDayFrom -= 1;
                    else if (isEndOfFeb(lastCouponPaymentDate))
                        intDayFrom = 30;

                    if (settlementDate.Day == 31 && lastCouponPaymentDate.Day >= 30)
                        intDayTo -= 1;
                    else if (settlementDate.Day == 31 && lastCouponPaymentDate.Day < 30)
                        intDayTo += 1;
                    else if (isEndOfFeb(settlementDate))
                        intDayTo = 30;

                    return (intDayTo - intDayFrom) + (30 * (intMonthTo - intMonthFrom)) + (360 * (intYearTo - intYearFrom));
                default:
                //ACT_ACT Days per month as per calendar.

                // ACT_365:
                    // The only major currency currently using this convention is the UK pound. 
                    // Each month is treated normally, and the year is assumed to have 365 days, regardless of leap year status. 
                    // For example, a period from February 1, 2005 to April 1, 2005 is considered to be 59 days. 
                    // This convention results in periods having slightly different lengths

                // ACT_360:
                    // This is the most common convention and is used by most currencies including the US Dollar and Euro. 
                    // Each month is treated normally and the year is assumed to be 360 days 
                    // e.g. in a period from February 1, 2005 to April 1, 2005 T is considered to be 59 days divided by 360.

                // ACT_365_L:
                    return Util.DateDiff(DateInterval.Day, lastCouponPaymentDate, settlementDate) - 1;
            }
        }

        private bool isEndOfMonth(DateTime dtmDate)
        {
            return (dtmDate.AddDays(1).Day == 1);
        }

        private bool isEndOfFeb(DateTime dtmDate)
        {
            return ((dtmDate.AddDays(1).Day == 1) && (dtmDate.AddDays(1).Month == 3));
        }

        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.Bond;
        }

        #endregion

        #region Privates

        private Money nominalValue;
        private decimal couponRate;
        private Regularities couponFreq;
        private DateTime maturityDate = DateTime.MinValue;
        private AccruedInterestCalcTypes accruedInterestCalcType;
        private DateTime firstCouponPaymntDate = DateTime.MinValue;
        private IDomainCollection<ICouponHistory> coupons;
        private IDomainCollection<IBondCouponRateHistory> couponRates;

    	#endregion

    }
}

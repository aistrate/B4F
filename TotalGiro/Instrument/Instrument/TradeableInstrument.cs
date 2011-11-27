using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class represents instrument tradeable on a exchange
    /// </summary>
    public abstract class TradeableInstrument : InstrumentsWithPrices, ITradeableInstrument
    {
		
		protected TradeableInstrument() { }

        #region TradeableInstrument Stuff
        
        /// <summary>
        /// Get/set name of company issuing the instrument
        /// </summary>
		public virtual string CompanyName
        {
			get { return this.companyName; }
			set { this.companyName = value; }
        }

        /// <summary>
        /// Get/set default exchange for instrument
        /// </summary>
		public virtual IExchange DefaultExchange
        {
			get { return this.defaultExchange; }
			set { this.defaultExchange = value; }
        }

        /// <summary>
        /// Get/set home exchange for instrument (where traded)
        /// </summary>
        public virtual IExchange HomeExchange
        {
            get { return homeExchange; }
            set { homeExchange = value; }
        }

        public virtual string DefaultExchangeName
        {
            get { return (this.defaultExchange != null ? this.defaultExchange.ExchangeName : "" ); }
        }

        /// <summary>
        /// Get/set route for ordering
        /// </summary>
        public virtual IRoute DefaultRoute
        {
            get { return route; }
            set { this.route = value; }
        }

        /// <summary>
        /// Flag to allow netting
        /// </summary>
        public virtual bool AllowNetting
        {
            get { return this.allowNetting; }
            set { this.allowNetting = value; }
        }

        /// <summary>
        /// The date that the instrument was issued
        /// </summary>
        public virtual DateTime IssueDate
        {
            get
            {
                if (issueDate.HasValue)
                    return issueDate.Value;
                else
                    return DateTime.MinValue;
            }
            set { issueDate = value; }
        }



        /// <summary>
        /// Get collection of exchanges where instrument is traded
        /// </summary>
        public virtual IInstrumentExchangeCollection InstrumentExchanges
        {
            get
            {
                InstrumentExchangeCollection items = (InstrumentExchangeCollection)this.instrumentExchanges.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return items;
            }
        }

        /// <summary>
        /// Get  is instrument tradeable flag
        /// </summary>
        public override bool IsTradeable
        {
            get { return true; }
        }

        /// <summary>
        /// Get is instrument cash flag
        /// </summary>
        public override bool IsCash
        {
            get { return false; }
        }

        /// <summary>
        /// Is the commission linear for this instrument
        /// </summary>
        public virtual bool IsCommissionLinear
        {
            get
            {
                bool isLinear = false;
                switch (SecCategory.Key)
                {
                    case SecCategories.MutualFund:
                    case SecCategories.VirtualFund:
                    case SecCategories.CashManagementFund:
                        isLinear = true;
                        break;
                }
                return isLinear;
            }
        }

        public bool IsGreenFund { get; set; }
        public bool IsCultureFund { get; set; }
        public IGLAccount SettlementDifferenceAccount { get; set; }
        public abstract int ContractSize { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Get the educated guess of the size of a instrument for a amount of money
        /// </summary>
        /// <param name="inputAmount"></param>
        /// <returns></returns>
        public override PredictedSize PredictSize(Money inputAmount)
        {
            PredictedSize retVal = new PredictedSize(PredictedSizeReturnValue.NoRate);
            Money amount;

            if (CurrentPrice != null)
            {
                retVal.RateDate = CurrentPrice.Date;
                if (inputAmount.Underlying.Equals(CurrentPrice.Price.Underlying))
                    retVal.Size = inputAmount.CalculateSize(CurrentPrice.Price);
                else
                {
                    amount = inputAmount.Convert(CurrentPrice.Price.Underlying);
                    retVal.Size = amount.CalculateSize(CurrentPrice.Price);
                }
                retVal.Rate = currentPrice.Price.ToString();
            }
            return retVal;
        }


        public DateTime GetSettlementDate(DateTime tradeDate, IExchange exchange)
        {
            return GetSettlementDate(tradeDate, exchange, 0);
        }

        public DateTime GetSettlementDate(DateTime tradeDate, IExchange exchange, Int16 settlementDays)
        {

            Int16 counter = 0;
            Int16 dayValue;
            DateTime settlementDate = tradeDate;
    
            // Default settlement period ?
            if (settlementDays == 0)
            {
                IInstrumentExchange instrumentExchange = this.InstrumentExchanges.GetItemByExchange(exchange.Key);
                if (instrumentExchange != null && instrumentExchange.DefaultSettlementPeriod > 0)
                    settlementDays = instrumentExchange.DefaultSettlementPeriod;
                
                if (settlementDays == 0)
                    settlementDays = exchange.DefaultSettlementPeriod;
            }
    

            switch (Util.CompareToZero((int)settlementDays))
	        {
                case CompareToZeroOperator.GreaterThanZero:
                    //Add a day for every loop.
                    dayValue = 1;
                    break;
		        case CompareToZeroOperator.SmallerThanZero:
                    //Subtract days, instead of adding them.
                    dayValue = -1;
                    break;
                default:
                    dayValue = 1;
                    //Make sure it goes into the loop by setting the counter to -1, to compensate
                    //the effect of this subtract one day from the date.
                    counter = -1;
                    settlementDate = settlementDate.AddDays(dayValue * -1);
                    break;
	        }
            
            //Calculate the settlementdate.
            while (counter < Math.Abs(settlementDays))
	        {
                //Add or subtract one day.
                settlementDate = settlementDate.AddDays(dayValue);
                //The day only counts if it is not a weekend day.
                if (settlementDate.DayOfWeek != DayOfWeek.Saturday && 
                    settlementDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    // TODO check for Exchange Holidays
                    ////The day only counts if it is not a holiday on the exchange.
                    //if (!exchange.IsHoliday(settlementDate)
                    //    counter++;
                    counter++;
                }
	        }
            return settlementDate;
        }

        public override bool CalculateCosts(IOrder order, IFeeFactory feeFactory)
        {
            checkCostCalculater(order, feeFactory);
            Commission commDetails = feeFactory.CalculateCommission(order);
            if (commDetails != null)
            {
                order.CommissionDetails = commDetails;
                order.CommissionDetails.Parent = (ICommissionParent)order;
            }
            return true;
        }

        public override bool CalculateCosts(IOrderAllocation transaction, IFeeFactory feeFactory, IGLLookupRecords lookups)
        {
            checkCostCalculater(transaction, feeFactory);
            Commission commDetails = feeFactory.CalculateCommission(transaction);
            if (commDetails != null)
                transaction.setCommission(lookups, commDetails.Amount);
            return true;
        }

        public Money GetServiceChargeForOrder(IOrder order)
        {
            return getServiceChargeForOrder(order, InstrumentExchanges.GetDefault());
        }

        public Money GetServiceChargeForOrder(IOrder order, IExchange exchange)
        {
            return getServiceChargeForOrder(order, InstrumentExchanges.GetItemByExchange(exchange.Key));
        }

        private Money getServiceChargeForOrder(IOrder order, IInstrumentExchange instrumentExchange)
        {
            Money serviceCharge = null;

            if (order == null)
                throw new ApplicationException("It is not possible to calculate the Service Charge when the order id null");

            if (instrumentExchange != null && order.Amount != null && order.Amount.IsNotZero)
            {
                decimal percentage = instrumentExchange.GetServiceChargePercentageForOrder(order);
                serviceCharge = order.Amount * (percentage / (1M + percentage));
            }
            if (serviceCharge == null)
                serviceCharge = new Money(0M, order.OrderCurrency);
            else
                serviceCharge = (serviceCharge.Abs() * -1);

            return serviceCharge;
        }

        protected decimal getServiceChargePercentage(IOrder order, IExchange exchange)
        {
            decimal percentage = 0M;
            IInstrumentExchange ie = null;
            if (IsSecurity)
            {
                if (InstrumentExchanges != null && InstrumentExchanges.Count > 0)
                {
                    if (exchange == null)
                        ie = InstrumentExchanges.GetDefault();
                    else
                        ie = InstrumentExchanges.GetItemByExchange(exchange.Key);
                }
                percentage = ie.GetServiceChargePercentageForOrder(order);
            }
            return percentage;
        }

        public virtual TransactionFillDetails GetTransactionFillDetails(
            IOrder order, Price price, DateTime settlementDate, IFeeFactory feeFactory,
            decimal fillRatio, IExchange exchange)
        {
            if (order.IsSizeBased)
                return GetTransactionFillDetails((IOrderSizeBased)order, price, settlementDate, feeFactory, fillRatio, exchange);
            else
                return GetTransactionFillDetails((IOrderAmountBased)order, price, settlementDate, feeFactory, fillRatio, exchange);
        }

        public virtual TransactionFillDetails GetTransactionFillDetails(
            IOrderSizeBased order, Price price, DateTime settlementDate, IFeeFactory feeFactory,
            decimal fillRatio, IExchange exchange)
        {
            Money serviceCharge = null;
            Money commission = null;
            decimal serviceChargePercentageforOrder = getServiceChargePercentage(order, exchange);

            // Use the Value of the child order -> difference will go to Crumble account
            InstrumentSize size = order.Value * fillRatio;
            Money amount = size.CalculateAmount(price);

            if (serviceChargePercentageforOrder != 0M)
                serviceCharge = (amount * serviceChargePercentageforOrder).Abs().Negate();

            TransactionFillDetails details = new TransactionFillDetails(size, amount, null, serviceCharge, serviceChargePercentageforOrder, commission, order.GrossAmount, order.Side);
            details.SetSign(order.Side);
            return details;
        }

        public TransactionFillDetails GetTransactionFillDetails(
            IOrderAmountBased order, Price price, DateTime settlementDate, IFeeFactory feeFactory,
            decimal fillRatio, IExchange exchange)
        {
            decimal serviceChargePercentageforOrder = getServiceChargePercentage(order, exchange);
            TransactionFillDetails details = null;

            if (IsCommissionLinear)
            {
                Money amount = order.Amount * fillRatio;
                Money serviceCharge = null;

                if (serviceChargePercentageforOrder != 0M)
                {
                    Money newAmount = amount * (decimal)(1M / (1M + serviceChargePercentageforOrder));
                    serviceCharge = (amount.Abs() - newAmount.Abs());
                    amount = newAmount;
                }

                // Calculate Commission
                Money commission = null;
                // if the trade fills the Order completely -> take the Commission from the Order
                if (fillRatio == 1M)
                    commission = order.Commission;

                // Convert amount when necessary
                if (!amount.Underlying.Equals(price.Underlying) && !price.Underlying.IsObsoleteCurrency)
                    amount = amount.Convert(order.ExRate, price.Underlying);

                InstrumentSize size = amount.CalculateSize(price);

                details = new TransactionFillDetails(size, amount, null, serviceCharge, serviceChargePercentageforOrder, commission, order.GrossAmount, order.Side);
            }
            else
            {
                // Do the goalseek
                ICommClient client;
                ICommRule rule = feeFactory.GetRelevantCommRule(order.Account, this, order.Side,
                    order.ActionType, settlementDate, true, out client);

                for (int i = 4; i > 0; i--)
                {
                    details = getTransactionFillDetailsAmountBasedOrderByGoalSeek(
                        order, settlementDate, price, exchange, 
                        rule, client, serviceChargePercentageforOrder, i);

                    if (details.IsOK && !details.IsDiff)
                        break;
                }
                if (!details.IsOK || details.IsDiff)
                    throw new ApplicationException("Not possible to calculate the trade amounts, probably a commission rule applied with a minimum amount.");
            }
            if (details != null)
                details.SetSign(order.Side);
            return details;
        }

        protected virtual TransactionFillDetails getTransactionFillDetailsAmountBasedOrderByGoalSeek(
            IOrderAmountBased order, DateTime settlementDate, Price price, IExchange exchange,
            ICommRule rule, ICommClient client, decimal servChargePerc, int precision)
        {
            try
            {
                TransactionFillDetails details = getTransactionFillDetailsAmountBasedOrderByGoalSeek(
                    order.GrossAmount, order.Side, order.IsCommissionRelevant, order.IsValueInclComm,
                    settlementDate, price, exchange, rule, client, servChargePerc, precision);
                if (details.IsOK)
                {
                    Money diff = details.Diff;
                    if (diff != null && diff.IsNotZero && diff.IsWithinTolerance(0.09M))
                    {
                        details.FixUp(order);
                        details.Size = details.Amount.CalculateSize(price);
                        details.Info = string.Format("F{0}", precision);
                    }
                }
                return details;
            }
            catch
            {
            }
            return new TransactionFillDetails();
        }

        protected virtual TransactionFillDetails getTransactionFillDetailsAmountBasedOrderByGoalSeek(
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
                (useComm ? rule.CommCalculation.Calculate(client.GetNewInstance(new InstrumentSize(x, this), price, (useAddComm ? rule.AdditionalCalculation.Calculate(client.GetNewInstance(new InstrumentSize(x, this), price)) : null))).Quantity : 0M) +
                (useAddComm ? rule.AdditionalCalculation.Calculate(client.GetNewInstance(new InstrumentSize(x, this), price)).Quantity : 0M) +
                (new InstrumentSize(x, this).CalculateAmount(price).Abs().Quantity * servChargePerc),
                grossAmount.Abs().Quantity, guess, precision);

            InstrumentSize size = new InstrumentSize(realAmount, this);
            Money amount = size.CalculateAmount(price);
            InstrumentSize cleanSize = amount.CalculateSize(price);

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
            return new TransactionFillDetails(cleanSize, amount, null, servCh, servChargePerc, comm + addComm, grossAmount.Abs(), side);
        }

        #endregion

        #region Validation

        protected override bool validate()
        {
            if (IsSecurity)
            {
                if (this.Isin == string.Empty)
                    throw new ApplicationException("The isin code is mandatory.");
            }
            if (this.DefaultExchange == null)
                throw new ApplicationException("The Default Exchange is mandatory.");
            if (this.CurrencyNominal == null)
                throw new ApplicationException("The Nominal currency is mandatory.");

            return base.validate();
        }

        #endregion

		#region Private Variables

		private IExchange defaultExchange;
        private IExchange homeExchange;
        private IRoute route;
        private string companyName;
        private DateTime? issueDate;
        private bool allowNetting;
        private IDomainCollection<IInstrumentExchange> instrumentExchanges = new InstrumentExchangeCollection();

		#endregion
	}
}

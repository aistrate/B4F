using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Communicator.FSInterface;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Abstract class describing the general functionality for an order object
    /// </summary>
    abstract public partial class Order : TotalGiroBase<IOrder>,  IOrder, ICommissionParent, IOrderFormulaDetails
    {
        protected Order() 
        {
            childOrders = new OrderCollection(this);
        }

        protected Order(IAccountTypeInternal account, InstrumentSize value) : this()
        {
			if (account == null)
				throw new ApplicationException("Account is mandatory on an order.");
			if (value == null)
				throw new ApplicationException("Value is mandatory on an order.");
            if (!IsAggregateOrder)
            {
                if (account.Status == AccountStati.Inactive)
                    throw new ApplicationException("The order is not allowed since the account is not active");
            }
            this.account = account;
            this.orderValue = value;
			
			if (value.Sign)
			{
				this.side = Side.Buy;
			}
			else
			{
				this.side = Side.Sell;
			}
		}

        protected virtual void setCommission(IFeeFactory feeFactory)
        {
            if (!DoNotChargeCommission && feeFactory != null)
                RequestedInstrument.CalculateCosts(this, feeFactory);
        }

        /// <summary>
        /// Unique identifier, same as Key
        /// </summary>
        public virtual int OrderID
        {
            get { return Key; }
        }

        /// <summary>
        /// Is the order size based?
        /// </summary>
		public virtual bool IsSizeBased
		{
			get { return false; }
		}

        /// <summary>
        /// Is the order amount based?
        /// </summary>
        public virtual bool IsAmountBased
        {
            get { return false; }
        }

        /// <summary>
        /// Is it a monetary order?
        /// </summary>
		public virtual bool IsMonetary
		{
			get { return false; }
		}

        public OpenClose OrderOpenClose { get; set; }

        /// <summary>
        /// Is it a security order?
        /// </summary>
        public virtual bool IsSecurity
        {
            get { return false; }
        }

        /// <summary>
        /// Is this order aggregated?
        /// </summary>
		public virtual bool IsAggregateOrder
		{
			get { return false; }
		}

        /// <summary>
        /// Is this an order on stichting level?
        /// </summary>
        public virtual bool IsStgOrder
        {
            get { return false; }
        }

        /// <summary>
        /// Has this order been netted?
        /// </summary>
        public virtual bool IsNetted
        {
            get { return this.isNetted; }
            internal set { this.isNetted = value; }
        }

        /// <summary>
        /// Type of order (AmountBased or SizeBased)
        /// </summary>
		public virtual OrderTypes OrderType
		{
			get { return OrderTypes.AmountBased; }
		}

        /// <summary>
        /// Gets/sets the action that caused this order to be created
        /// </summary>
		public virtual OrderActionTypes ActionType
		{
			get 
            {
                if ((int)this.actionType == 0)
                    return OrderActionTypes.NoAction;
                else
                    return this.actionType; 
            }
			set { this.actionType = value; }
		}

        /// <summary>
        /// Returns the ordertype as a string for display purposes
        /// </summary>
		public virtual string DisplayIsSizeBased
        {
            get { return (IsSizeBased ? "Size" : "Amt"); }
        }

        /// <summary>
        /// User account
        /// </summary>
        public virtual IAccountTypeInternal Account
        {
            get { return this.account; }
            set { this.account = value; }
        }

        /// <summary>
        /// Returns the side of the order (Buy/Sell)
        /// </summary>
		public virtual Side Side
        {
            get { return this.side; }
            internal set { this.side = value; }
        }

        /// <summary>
        /// Is the order completely filled by one transaction?
        /// </summary>
        public virtual bool IsCompleteFilled
        {
            get { return isCompleteFilled; }
            internal set { isCompleteFilled = value; }
        }
	
		/// <summary>
		/// Value indicates 
		/// a) the Gross (Brutto) amount of the Order is an AmountBased Order
		/// b) the Size of the Order if a Sized Based Order
		/// </summary>
		public virtual InstrumentSize Value
        {
            get { return this.orderValue; }
            internal set { this.orderValue = value; }
        }

        /// <summary>
        /// The price of the instrument of this order
        /// </summary>
		public virtual Price Price
		{
			get { return this.price; }
			set { this.price = value; }
		}

        public abstract IInstrument RequestedInstrument { get; }

        /// <summary>
        /// Gets the name of the traded instrument for display purposes
        /// </summary>
        public virtual string DisplayTradedInstrumentIsin
        {
            get
            {
                string isin = "";
                if (IsSecurity && ((ISecurityOrder)this).TradedInstrument != null)
                    isin = ((ISecurityOrder)this).TradedInstrument.Isin;
                return isin;
            }
        }

        /// <summary>
        /// Currency in which the order is settled.
        /// </summary>
		public virtual ICurrency OrderCurrency
		{
			get 
			{
				if (this.Value.IsMoney)
					return (ICurrency)this.Value.Underlying;
				else
					return ((ITradeableInstrument)this.Value.Underlying).CurrencyNominal;
			}
		}

        /// <summary>
        /// Commission to be paid for this order
        /// </summary>
		public virtual Money Commission
		{
			get 
            {
                if (DoNotChargeCommission && Amount != null)
                    return Amount.ZeroedAmount();
                else if (CommissionDetails != null)
                    return CommissionDetails.Amount;
                else
                    return null;
            }
		}

        /// <summary>
        /// Description of the rules that generated the commission.
        /// </summary>
		public virtual string CommissionInfo
		{
			get { return commissionInfo; }
			set { commissionInfo = value; }
		}

        /// <summary>
        /// Commission to be paid for this order, with breakup information
        /// </summary>
        public virtual Commission CommissionDetails
        {
            get { return commissionDetails; }
            set { commissionDetails = value; }
        }

        /// <summary>
        /// No commission is charged on this order
        /// </summary>
        public virtual bool DoNotChargeCommission
        {
            get { return doNotChargeCommission; }
            set { doNotChargeCommission = value; }
        }

        /// <summary>
        /// Exchange rate for the instrument
        /// </summary>
		public virtual decimal ExRate
		{
			get { return exRate; }
		}

        /// <summary>
        /// Sets the exchange rate, propagating it to any child orders
        /// </summary>
        /// <param name="rate">Exchange rate</param>
        /// <returns>true if succesfull</returns>
		public virtual bool SetExRate(decimal rate)
		{
			this.exRate = rate;

            IOrderAmountBased order = null;
            if (IsAmountBased && IsSecurity)
                order = (IOrderAmountBased)this;

            if (order != null)
            {
                foreach (Order child in order.ChildOrders)
                {
                    child.SetExRate(rate);
                }
            }
            return true;
		}

        /// <summary>
        /// Returns parent order of this order
        /// </summary>
		public virtual IOrder ParentOrder
		{
			get { return parentOrder; }
            set { SetParentOrder(value); }
		}

        /// <summary>
        /// Returns the TopMost order (being actually sent to the exchange) of the current order
        /// </summary>
        public virtual IOrder TopParentOrder
        {
            get
            {
                if (this.formulaDetails != null && this.formulaDetails.TopParentOrder != null)
                    return this.formulaDetails.TopParentOrder;
                else
                    return this;
            }
        }

        /// <summary>
        /// Returns the status of the top parent order as a string for display purposes.
        /// When the top parent is null -> return empty string
        /// </summary>
        public virtual string TopParentDisplayStatus
        {
            get
            {
                string status = string.Empty;
                if (this.formulaDetails != null)
                    status = this.formulaDetails.TopParentDisplayStatus;
                return status;
            }
        }

        /// <summary>
        /// Sets the parent order
        /// </summary>
        /// <param name="parent">Parent order</param>
        public void SetParentOrder(IOrder parent)
        {
            this.parentOrder = parent;
        }

        /// <summary>
        /// The status of the order
        /// </summary>
		public virtual OrderStati Status
		{
			get { return status; }
			// Setter is public because of NHibernate's Casting
            set { status = value; }
		}

        /// <summary>
        /// The Cancel status of the order
        /// </summary>
		public virtual OrderCancelStati CancelStatus
		{
			get { return cancelStatus; }
            // Setter is public because of NHibernate's Casting
            set { cancelStatus = value; }
		}

		#region IOrder

		//public IList GetOrdersForAccount(IAccount Account)
		//{
		//    return ((Order.DataAccessLayer)DataAccess).GetOrders(Account);
		//}

		//public bool Update(B4F.TotalGiro.Orders.OldTransactions.ITransaction Transaction)
		//{
		//    return ((Order.DataAccessLayer)DataAccess).Update(this, Transaction);
		//}

		public bool UpdateStateFromTX()
		{
			return OrderStateMachine.SetNewStatus(this, OrderStateEvents.CheckFill);
		}

        abstract public Money Amount { get;}
        abstract public Money GrossAmount { get;}
        abstract public Money OpenAmount { get;}
        abstract public Decimal GetChildRatio();

        public virtual Money BaseAmount
        {
            get { return Amount.CurrentBaseAmount; }
        }

        public virtual Money GrossAmountBase
        {
            get { return GrossAmount.CurrentBaseAmount; }
        }

        public virtual Money EstimatedAmount
        {
            get
            {
                Money amount = Amount;
                if ((amount == null || amount.IsZero) && IsSizeBased)
                {
                    if (Value.IsMoney)
                        amount = Value.GetMoney();
                    else
                    {
                        ITradeableInstrument instrument = ((IOrderSizeBased)this).TradedInstrument;
                        if (instrument.CurrentPrice != null && instrument.CurrentPrice.Price != null)
                            amount = Value.CalculateAmount(instrument.CurrentPrice.Price);
                        else
                            throw new ApplicationException(string.Format("Could not estimate the value since no price was found for {0}", instrument.DisplayName));
                    }
                }
                return amount;
            }
        }


        public virtual InstrumentSize PlacedValue
        {
            get 
            {
                if (this.placedValue != null)
                    return this.placedValue;
                else
                    return Value;
            }
            internal set { this.placedValue = value; }
        }

		#endregion

        /// <summary>
        /// Returns the status of the order as a string for display purposes.
        /// </summary>
		public virtual string DisplayStatus
		{
			get 
			{
				string status = Status.ToString();
				if (CancelStatus != OrderCancelStati.Neutral)
				{
					status += " (" + CancelStatus.ToString() + ")";
				}
				return status;
			}
		}

        /// <summary>
        /// Has this order been approved?
        /// </summary>
		public virtual bool Approved
		{
			get { return this.approved; }
			set { this.approved = value; }
		}

        /// <summary>
        /// The Date/Time this order has been approved.
        /// </summary>
		public virtual DateTime ApprovalDate
		{
            get
            {
                if (approvalDate.HasValue)
                    return approvalDate.Value;
                else
                    return DateTime.MinValue;
            }
			internal set { this.approvalDate = value; }
		}

        /// <summary>
        /// Is this order UnApproveable
        /// </summary>
        public virtual bool IsUnApproveable
        {
            get
            {
                bool retVal = false;
                if (Approved && Status == OrderStati.New)
                    retVal = true;
                return retVal;
            }
        }

        ///// <summary>
        ///// The transactions that belong to this order.
        ///// </summary>
        //public virtual ITransactionCollection Transactions
        //{
        //    get 
        //    {
        //        if (transactions == null)
        //            this.transactions = new TransactionCollection(bagOfTransactions, this);
        //        return transactions; 
        //    }
        //    set { transactions = value; }
        //}

        public virtual ITransactionOrderCollection Transactions
        {
            get
            {
                ITransactionOrderCollection comp = (ITransactionOrderCollection)transactions.AsList();
                if (comp.Parent == null) comp.Parent = this;
                return comp;
            }
        }

        /// <summary>
        /// Determines if this order is fillable or not.
        /// </summary>
		public abstract OrderFillability IsFillable { get; }

        /// <summary>
        /// Determines whether commission should be calculated for the transaction during fill time.
        /// </summary>
        public virtual bool IsCommissionRelevant 
        {
            get
            {
                bool useComm = true;
                if (Account.AccountType != AccountTypes.Customer || DoNotChargeCommission)
                    useComm = false;
                else if (this.Key != 0 && (Commission == null ||
                    (Commission != null && Commission.IsZero)))
                    useComm = false;
                return useComm;
            }
        }


        protected bool isSendable
        {
            get
            {
                bool retVal = false;
                if (ExRate != 0 && Route != null)
                {
                    retVal = true;
                }
                return retVal;
            }
        }


		/// <summary>
		/// Indicates the 
		/// a) Total Filled Amount of the Order if a Amount Based Order
		/// b) Total Filled Size of the Order if a Size Based Order.
		/// </summary>
		public virtual InstrumentSize FilledValue
		{
			get { return this.filledValue; }
			internal set { this.filledValue = value; }
		}

        /// <summary>
        /// The value of the order that remains to be filled.
        /// </summary>
		public virtual InstrumentSize OpenValue
		{
			get 
            {
                if (isFilled)
                    return Value.Clone(0M);
                else
                {
                    if (FilledValue != null)
                    {
                        InstrumentSize diff = Value - FilledValue;
                        if (diff.IsZero || diff.IsWithinTolerance(0.02M))
                            return Value.Clone(0M);
                        else
                            return (Value - FilledValue);
                    }
                    else
                        return Value;
                }
            }
		}

        protected bool isFilled
        {
            get
            {
                bool retVal = false;
                if (IsCompleteFilled)
                    retVal = true;
                else if (Status >= OrderStati.Filled && CancelStatus == OrderCancelStati.Neutral)
                    retVal = true;

                return retVal;
            }
        }

        /// <summary>
        /// Error code if one occurred during allocation
        /// </summary>
		public virtual OrderErrors Err
		{
			get { return errorID; }
			set { errorID = value; }
		}

        /// <summary>
        /// Description of the order
        /// </summary>
		public virtual string ErrDescription
		{
			get { return errDescription; }
			set { errDescription = value; }
		}

        /// <summary>
        /// Export file that this order has been written to
        /// </summary>
		public virtual IFSExportFile ExportFile
		{
			get { return this.fsExportFile; }
			set { this.fsExportFile = value; }
		}

        /// <summary>
        /// Route that this order follows (E.g. automatic or manual desk)
        /// </summary>
        public virtual IRoute Route
        {
            get { return this.route; }
            set { this.route = value; }
        }

        protected IInstrumentExchange getInstrumentExchange(bool useDefaultWhenNoExchange)
        {
            IInstrumentExchange ie = null;

            if (!IsMonetary)
            {
                if ((Route != null && Route.Exchange != null) || useDefaultWhenNoExchange)
                {
                    ITradeableInstrument instrument = ((ISecurityOrder)this).TradedInstrument;
                    if (instrument != null && instrument.InstrumentExchanges != null && instrument.InstrumentExchanges.Count > 0)
                    {
                        if (Route != null && Route.Exchange != null)
                            ie = instrument.InstrumentExchanges.GetItemByExchange(Route.Exchange.Key);
                        else
                            ie = instrument.InstrumentExchanges.GetDefault();
                    }
                }
            }
            return ie;
        }

        protected IInstrumentExchange getInstrumentExchange(IExchange exchange)
        {
            IInstrumentExchange ie = null;
            if (IsSecurity)
            {
                ITradeableInstrument instrument = ((ISecurityOrder)this).TradedInstrument;
                if (instrument != null && instrument.InstrumentExchanges != null && instrument.InstrumentExchanges.Count > 0)
                {
                    if (exchange == null)
                        ie = instrument.InstrumentExchanges.GetDefault();
                    else
                        ie = instrument.InstrumentExchanges.GetItemByExchange(exchange.Key);
                }
            }
            return ie;
        }

        ///// <summary>
        ///// Transactions that belong to this order
        ///// </summary>
        //public virtual IList BagOfTransactions
        //{
        //    get { return this.bagOfTransactions; }
        //}		

        /// <summary>
        /// Child orders of this order
        /// </summary>
        //public virtual IOrderCollection ChildOrders
        //{
        //    get 
        //    {
        //        if (childOrders == null)
        //            this.childOrders = new OrderCollection(bagOfChildOrders, this);
        //        return childOrders; 
        //    }
        //    set { childOrders = value; }
        //}

        public virtual IOrderCollection ChildOrders
        {
            get
            {
                OrderCollection pos = (OrderCollection)childOrders.AsList();
                if (pos.ParentOrder == null) pos.ParentOrder = this;
                return pos;
            }
        }

        /// <summary>
        /// The date/time that the allocation of this order has been done
        /// </summary>
		public virtual DateTime AllocationDate
		{
            get
            {
                if (allocationDate.HasValue)
                    return allocationDate.Value;
                else
                    return DateTime.MinValue;
            }
			set { allocationDate = value; }
		}

        /// <summary>
        /// Date/time this order was created
        /// </summary>
        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
            internal set { this.creationDate = value; }
        }

        /// <summary>
        /// Date/time when this order has last been updated
        /// </summary>
        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }

        /// <summary>
        /// Date/time the order was closed
        /// </summary>
        public virtual DateTime DateClosed
        {
            get { return this.dateClosed.HasValue ? dateClosed.Value : DateTime.MinValue; }
            set { dateClosed = value; }
        }

        /// <summary>
        /// Is this order closed?
        /// </summary>
        public virtual bool IsClosed
        {
            get 
            {
                return Util.IsNotNullDate(DateClosed);
            }
        }

        protected bool IsTypeConverted
        {
            get { return this.isTypeConverted; }
            set { this.isTypeConverted = value; }
        }

        public virtual IInstruction Instruction
        {
            get { return this.instruction; }
            set { this.instruction = value; }
        }

        public virtual string OrderInfo
        {
            get { return this.orderInfo; }
            set { this.orderInfo = value; }
        }

        /// <summary>
        /// Is this order closed?
        /// </summary>
        public virtual InstrumentSize RelevantInstrumentPositionSize
        {
            get 
            {
                IInstrument relevantInstrument;

                if (IsSizeBased)
                    relevantInstrument = Value.Underlying;
                else
                    relevantInstrument = RequestedInstrument;

                InstrumentSize retVal = new InstrumentSize(0m, relevantInstrument);
                IFundPosition position = Account.Portfolio.PortfolioInstrument.GetPosition((ITradeableInstrument) relevantInstrument);
                if (position != null)
                    retVal = position.Size;
                return retVal;
            }
        }

        public virtual string DisplayInstructionKey
        {
            get 
            {
                string key = string.Empty;
                if (Instruction != null)
                    key = Instruction.Key.ToString();
                return key; 
            }
        }

        protected InstrumentSize ConvertedValue
        {
            get 
            {
                InstrumentSize convValue = null;
                if (IsTypeConverted && ChildOrders != null && ChildOrders.Count == 1)
                    convValue = ChildOrders[0].Value;
                return convValue; 
            }
        }

        public override string ToString()
        {
            string instrument = string.Empty;
            string val = string.Empty;

            if (IsMonetary)
                instrument = ((IMonetaryOrder)this).RequestedInstrument.DisplayName;
            else
                instrument = ((ISecurityOrder)this).TradedInstrument.DisplayName;

            if (IsSizeBased)
                val = " " + Value.DisplayString;
            else
                val = " " + Value.DisplayString + " of";


            return Side.ToString() + val + " " + instrument + " (" + OrderID.ToString() + ")";
        }

        #region ICommissionParent Members

        CommissionParentTypes ICommissionParent.Type
        {
            get { return CommissionParentTypes.Order; }
        }

        IOrder ICommissionParent.Order
        {
            get { return this; }
        }

        //ITransactionOrder ICommissionParent.Transaction
        //{
        //    get { return null; }
        //}

        #endregion


		#region Private Variables

        private bool approved;
        private bool doNotChargeCommission;
        private bool isCompleteFilled;
        private bool isNetted = false;
        private bool isTypeConverted = false;
        private Commission commissionDetails;
        private DateTime? allocationDate;
        private DateTime? approvalDate;
        private DateTime creationDate = DateTime.Now;
        private DateTime? dateClosed;
        private DateTime lastUpdated;
        private IAccountTypeInternal account;
        private IDomainCollection<IOrder> childOrders;
        private IDomainCollection<ITransactionOrder> transactions;
        private IFSExportFile fsExportFile;
        private IInstruction instruction;
        private IInstrument requestedInstrument;
        private IMonetaryOrder moneyOrder;
        private InstrumentSize filledValue;
        private InstrumentSize orderValue;
        private IOrder parentOrder;
        private IOrderFormulaDetails formulaDetails;
        private IRoute route;
        private OrderActionTypes actionType = OrderActionTypes.NoAction;
        private OrderCancelStati cancelStatus = OrderCancelStati.Neutral;
        private OrderErrors errorID = OrderErrors.Neutral;
        private OrderStati status = OrderStati.New;
        private Side side;
        private string commissionInfo;
        private string errDescription;
        private string orderInfo;
        protected decimal exRate = 1M;
        protected InstrumentSize placedValue;
        protected internal decimal serviceChargeForBuy = 0M;
        protected internal decimal serviceChargeForSell = 0M;
        protected internal decimal valueToBuy = 0M;
        protected internal decimal valueToSell = 0M;
        protected Price price;		


		#endregion

    }
}
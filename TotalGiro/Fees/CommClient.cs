using System;
using System.Collections.Generic;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Fees.CommRules;

namespace B4F.TotalGiro.Fees
{
    public class CommClient : ICommClient
    {
        #region Constructor

        public CommClient(IOrder order)
        {
            if (order == null)
                throw new ApplicationException("It is not possible to calculate the commission when the order is null.");
            this.order = order;
            type = CommClientType.Order;
        }

        public CommClient(ITransactionOrder transaction, InstrumentSize newValue)
        {
            if (transaction == null)
                throw new ApplicationException("It is not possible to calculate the commission when the transaction is null.");

            if (newValue == null)
                throw new ApplicationException("It is not possible to calculate the commission when the transaction value is null.");

            this.transaction = transaction;
            this.orderValue = newValue;
            this.amount = newValue.GetMoney();
            type = CommClientType.Transaction;
        }

        /// <summary>
        /// Constructor for creating a test FeeClient object
        /// </summary>
        /// <param name="account"></param>
        /// <param name="instrument"></param>
        /// <param name="side"></param>
        /// <param name="actiontype"></param>
        /// <param name="transactionDate"></param>
        /// <param name="issizebased"></param>
        /// <param name="orderValue"></param>
        /// <param name="amount"></param>
        /// <param name="price"></param>
        /// <param name="ordercurrency"></param>
        /// <param name="isValueInclComm"></param>
        public CommClient(IAccountTypeInternal account,IInstrument instrument, Side side, OrderActionTypes actiontype, 
                         DateTime transactionDate, bool issizebased, InstrumentSize orderValue, Money amount, Price price,
                         ICurrency ordercurrency, bool isValueInclComm)
        {
            if (account == null)
                throw new ApplicationException("It is not possible to calculate the commission when the account is unknown.");

            if (instrument == null)
                throw new ApplicationException("It is not possible to calculate the commission when the instrument value is unknown.");

            this.account = account;
            this.instrument = instrument;
            this.Side = side;
            this.ActionType = actiontype;
            this.TransactionDate = transactionDate;
            this.IsSizeBased = issizebased;
            this.OriginalOrderType = issizebased ? BaseOrderTypes.SizeBased : BaseOrderTypes.AmountBased;
            this.Value = orderValue;
            this.amount = amount;
            this.Price = price;
            this.OrderCurrency = ordercurrency;
            this.IsValueInclComm = isValueInclComm;

            type = CommClientType.Test;
        }

        #endregion

        #region Methods

        public ICommClient GetNewInstance(InstrumentSize size, Price price)
        {
            return GetNewInstance(size, price, null);
        }

        public ICommClient GetNewInstance(InstrumentSize size, Price price, Money previousCalculatedFee)
        {
            this.Value = size;
            this.Price = price;
            this.PreviousCalculatedFee = previousCalculatedFee;
            this.GrossAmount = size.CalculateAmount(price);
            this.amountIsNett = true;
            return this;
        }

        #endregion

        #region FeeClient Members

        public CommClientType Type
        {
            get { return this.type; }
        }

        public IAccountTypeInternal Account
        {
            get 
            {
                switch (Type)
                {
                    case CommClientType.Order:
                        account = order.Account; 
                        break;
                    case CommClientType.Transaction:
                        account = transaction.AccountA;
                        break;
                }
                return account;
            }
            set { this.account = value; }
        }

        public bool HasEmployerRelation 
        {
            get
            {
                bool hasEmployerRelation = false;
                if (Account.AccountType == AccountTypes.Customer)
                    hasEmployerRelation = ((ICustomerAccount)Account).EmployerRelationship != AccountEmployerRelationship.None;
                return hasEmployerRelation;
            }
        }

        public IInstrument TradedInstrument
        {
            get
            {
                switch (Type)
                {
                    case CommClientType.Order:
                        instrument = order.RequestedInstrument;
                        break;
                    case CommClientType.Transaction:
                        instrument = transaction.TradedInstrument;
                        break;
                }
                return instrument;
            }
            set { this.instrument = value; }
        }

        public Side Side
        {
            get
            {
                switch (Type)
                {
                    case CommClientType.Order:
                        side = order.Side;
                        break;
                    case CommClientType.Transaction:
                        side = transaction.TxSide;
                        break;
                }
                return side;
            }
            set { this.side = value; }
        }

        public bool IsValueInclComm
        {
            get
            {
                // Default -> false
                switch (Type)
                {
                    case CommClientType.Order:
                        if (order.OrderType == OrderTypes.AmountBased)
                            isValueInclComm = ((IOrderAmountBased)order).IsValueInclComm;
                        break;
                }
                return isValueInclComm;
            }
            set { this.isValueInclComm = value; }
        }

        public OrderActionTypes ActionType
        {
            get
            {
                // Default -> OrderActionTypes.NoAction
                switch (Type)
                {
                    case CommClientType.Order:
                        actionType = order.ActionType;
                        break;
                    case CommClientType.Transaction:
                        actionType = transaction.Order.ActionType;
                        break;
                }
                return actionType;
            }
            set { this.actionType = value; }
        }

        public OrderTypes OrderType
        {
            get
            {
                switch (Type)
                {
                    case CommClientType.Order:
                        orderType = order.OrderType;
                        break;
                    case CommClientType.Transaction:
                        // OrderType of a transaction -> AmountBased
                        // Because amount is already there
                        orderType = OrderTypes.AmountBased;
                        break;
                }
                return orderType;
            }
            set { this.orderType = value; }
        }

        public BaseOrderTypes OriginalOrderType
        {
            get
            {
                switch (Type)
                {
                    case CommClientType.Order:
                        originalOrderType = (order.OrderType == OrderTypes.SizeBased ? BaseOrderTypes.SizeBased : BaseOrderTypes.AmountBased);
                        break;
                    case CommClientType.Transaction:
                        originalOrderType = (transaction.Order.OrderType == OrderTypes.SizeBased ? BaseOrderTypes.SizeBased : BaseOrderTypes.AmountBased);
                        break;
                }
                return originalOrderType;
            }
            set { originalOrderType = value; }
        }

        public DateTime TransactionDate
        {
            get
            {
                switch (Type)
                {
                    case CommClientType.Order:
                        transactionDate = order.CreationDate.Date;
                        break;
                    case CommClientType.Transaction:
                        // OrderType of a transaction -> AmountBased
                        // Because amount is already there
                        transactionDate = transaction.TransactionDate;
                        break;
                }
                if (Util.IsNullDate(transactionDate))
                    transactionDate = DateTime.Today;
                return transactionDate;
            }
            set { this.transactionDate = value; }
        }

        public DateTime SettlementDate
        {
            get
            {
                if (Util.IsNullDate(settlementDate))
                {
                    switch (Type)
                    {
                        case CommClientType.Transaction:
                            settlementDate = transaction.ContractualSettlementDate;
                            break;
                    }
                }
                return settlementDate;
            }
            set { this.settlementDate = value; }
        }

        public bool IsSizeBased
        {
            get
            {
                switch (Type)
                {
                    case CommClientType.Order:
                        isSizeBased = order.IsSizeBased;
                        break;
                    case CommClientType.Transaction:
                        isSizeBased = false;
                        break;
                }
                return isSizeBased;
            }
            set { this.isSizeBased = value; }
        }

        public InstrumentSize Value
        {
            get
            {
                // Transaction -> get the value from constructor
                switch (Type)
                {
                    case CommClientType.Order:
                        if (order.IsSizeBased)
                            orderValue = order.Value;
                        else // amountbased -> deduct serviceCharge
                            orderValue = (InstrumentSize)MoneyMath.AdjustAmountForServiceCharge(order.Value.GetMoney(), ServiceCharge, order.Side, MathOperator.Subtract);
                        break;
                }
                return orderValue;
            }
            set { this.orderValue = value; }
        }

        public Money Amount
        {
            get
            {
                Money amt = GrossAmount;
                if (!amountIsNett && !this.IsSizeBased)
                {
                    switch (Type)
                    {
                        case CommClientType.Order:
                        case CommClientType.Test:
                            if (!amountIsNett && this.instrument.SecCategory.Key == SecCategories.Bond)
                            {
                                IBond bond = (IBond)this.instrument;
                                if (bond != null && bond.DoesPayInterest)
                                {
                                    // Calculate backwards the number of bonds
                                    if (Util.IsNullDate(SettlementDate))
                                        SettlementDate = bond.GetSettlementDate(TransactionDate, bond.DefaultExchange ?? bond.HomeExchange);
                                    Price price = Price ?? bond.CurrentPrice.Price;
                                    InstrumentSize size = bond.CalculateSizeBackwards(amount, price, SettlementDate);
                                    amt = size.CalculateAmount(price);

                                    AccruedInterestDetails calc = bond.AccruedInterest(size, SettlementDate, bond.DefaultExchange ?? bond.HomeExchange);
                                    if (calc.IsRelevant)
                                        AccruedInterest = calc.AccruedInterest;
                                }
                            }
                            break;
                    }
                }
                return amt;
            }
        }

        public bool AmountIsNett
        {
            get { return amountIsNett; }
        }

        public Money GrossAmount
        {
            get 
            {
                switch (Type)
                {
                    case CommClientType.Order:
                        if (order.IsSizeBased)
                            amount = order.Amount;
                        else // amountbased -> deduct serviceCharge
                            amount = MoneyMath.AdjustAmountForServiceCharge(order.Amount, ServiceCharge, order.Side, MathOperator.Subtract);
                        break;
                }
                return amount;
            }
            set { this.amount = value; }
        }

        public Money AccruedInterest
        {
            get { return accruedInterest; }
            protected set { this.accruedInterest = value; }
        }

        public Money PreviousCalculatedFee
        {
            get { return previousCalculatedFee; }
            set { this.previousCalculatedFee = value; }
        }
        
        private Money ServiceCharge
        {
            get
            {
                if (serviceCharge == null && Type == CommClientType.Order)
                {
                    if (TradedInstrument.IsTradeable)
                        serviceCharge = ((ITradeableInstrument)TradedInstrument).GetServiceChargeForOrder(order);
                }
                return serviceCharge;
            }
        }

        public Price Price
        {
            get
            {
                switch (Type)
                {
                    case CommClientType.Order:
                        price = order.Price;
                        break;
                    case CommClientType.Transaction:
                        price = transaction.Price;
                        break;
                }
                return price;
            }
            set { this.price = value; }
        }

        public ICurrency OrderCurrency
        {
            get
            {
                switch (Type)
                {
                    case CommClientType.Order:
                        orderCurrency = order.OrderCurrency;
                        break;
                    case CommClientType.Transaction:
                        orderCurrency = transaction.Order.OrderCurrency;
                        break;
                }
                if (orderCurrency == null)
                {
                    if (instrument.IsWithPrice)
                        orderCurrency = ((IInstrumentsWithPrices)instrument).CurrencyNominal;
                    if (orderCurrency.IsObsoleteCurrency)
                        orderCurrency = orderCurrency.ParentInstrument as ICurrency;
                }
                return orderCurrency;
            }
            set { this.orderCurrency = value; }
        }

        public string CommissionInfo
        {
            get
            {
                return this.commissionInfo;

            }
            set
            {
                this.commissionInfo = value;
                if (Type == CommClientType.Order)
                    order.CommissionInfo = value;
            }
        }

        #endregion

        #region Privates

        private IOrder order;
        private ITransactionOrder transaction;
        private CommClientType type = CommClientType.Test;
        private IAccountTypeInternal account = null;
        private IInstrument instrument;
        private Side side;
        private bool isValueInclComm = false;
        private OrderActionTypes actionType = OrderActionTypes.NoAction;
        private OrderTypes orderType;
        private BaseOrderTypes originalOrderType;
        private bool isSizeBased;
        private bool isMonetary = false;
        private InstrumentSize orderValue;
        private Money amount;
        private Price price;
        private ICurrency orderCurrency;
        private Money serviceCharge;
        private Money accruedInterest;
        private string commissionInfo;
        private DateTime transactionDate;
        private DateTime settlementDate = DateTime.MinValue;
        private Money previousCalculatedFee;
        private bool amountIsNett = false;

        #endregion
    }
}

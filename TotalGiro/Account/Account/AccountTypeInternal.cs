using System;
using System.Collections;
using B4F.TotalGiro.Accounts.Portfolios;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This is an abstract class and a subclass of the <see cref="T:B4F.TotalGiro.Accounts.Account">Account</see> class.
    /// It serves as a base class for accounts that are internal in the TotalGiro system, like customer and own accounts.
    /// </summary>
    abstract public class AccountTypeInternal : Account, IAccountTypeInternal
    {
        #region Ctors


        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeInternal">AccountTypeInternal</see> class.
        /// </summary>
        public AccountTypeInternal()
        {
            //glpositions = new GLPortfolio(this);
            //instrumentPositions = new FundPortfolio(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeInternal">AccountTypeInternal</see> class.
        /// </summary>
        /// <param name="number">The Account's number</param>
        /// <param name="shortName">Shortname of the account</param>
        /// <param name="accountOwner">The owner of the account</param>
        public AccountTypeInternal(string number, string shortName, IManagementCompany AccountOwner)
            : base(number, shortName)
        {
            this.AccountOwner = AccountOwner;
            //glpositions = new GLPortfolio(this);
        }
        #endregion
        /// <summary>
        /// This is the owner of the internal account.
        /// Customer accounts are owned by a Asset managing company, while the accounts of the asset managing company are owned by the stichting.
        /// </summary>
        public virtual IManagementCompany AccountOwner
        {
            get { return accountOwner; }
            set { accountOwner = value; }
        }

        /// <summary>
        /// The base currency of the internal account
        /// </summary>
        public override ICurrency BaseCurrency
        {
            get { return this.accountOwner.BaseCurrency; }
            set { throw new ApplicationException("not allowed"); }
        }



        /// <summary>
        /// This account is used when orders for the internal account are aggregated to an aggregated order.
        /// This account on this aggregated order will be the AccountforAggregation.
        /// </summary>
        public virtual ITradingAccount AccountforAggregation
        {
            get { return this.AccountOwner.TradingAccount; }
        }


        /// <summary>
        /// This account will receive the External account to be Used for Transfers
        /// </summary>
        public virtual IAccountTypeExternal DefaultAccountforTransfer
        {
            get { return this.AccountOwner.DefaultAccountforTransfer; }
        }


        
        /// <summary>
        /// A flag indicating whether this account is internal
        /// </summary>
        public new bool IsInternal
        {
            get { return true; }
        }


        /// <summary>
        /// Retrieve the first Transaction date of the account.
        /// </summary>
        public virtual DateTime FirstTransactionDate
        {
            get
            {
                DateTime firstDate = PortfolioCashGL.FirstCashTxDate;
                if (Util.IsNullDate(firstDate) || firstDate > PortfolioInstrument.FirstTxDate)
                    firstDate = PortfolioInstrument.FirstTxDate;
                return firstDate;
            }
        }

        /// <summary>
        /// The way how the account stores its positions
        /// </summary>
        public override StorePositionsLevel StorePositions
        {
            get { return StorePositionsLevel.Chronological; }
        }

        /// <summary>
        /// Does the account need commission calculation
        /// </summary>
        public virtual bool CommissionCalcReqd
        {
            get { return false; }
            //    set { this.commissionCalcReqd = value; }
        }

        /// <summary>
        /// Does the account need valuation calculation
        /// </summary>
        public virtual bool ValuationsRequired
        {
            get { return valuationsRequired; }
            set { valuationsRequired = value; }
        }

        /// <summary>
        /// The date after which no more valuations are needed
        /// </summary>
        public virtual DateTime? ValuationsEndDate
        {
            get { return valuationsEndDate; }
            set { valuationsEndDate = value; }
        }

        /// <summary>
        /// The active orders that exist for this internal account
        /// </summary>
        //public virtual IOrderCollection OrdersForAccount
        //{
        //    get
        //    {
        //        if (this.ordersForAccount == null)
        //            this.ordersForAccount = new B4F.TotalGiro.Accounts.AccountTypeInternal.OrderCollection(this, bagOfOrders);
        //        return ordersForAccount;
        //    }
        //    set { ordersForAccount = value; }
        //}

        public virtual IAccountOrderCollection OpenOrdersForAccount
        {
            get
            {
                IAccountOrderCollection pos = (IAccountOrderCollection)openOrdersForAccount.AsList();
                if (pos.ParentAccount == null) pos.ParentAccount = this;
                return pos;
            }
        }
        public virtual IEndTermValueCollection EndTermValues
        {
            get
            {
                IEndTermValueCollection etv = (IEndTermValueCollection)endTermValues.AsList();
                if (etv.ParentAccount == null) etv.ParentAccount = this;
                return etv;
            }
        }

        /// <summary>
        /// The total cash amount (not including the cash fund) of the internal account returned in base currency
        /// </summary>
        public virtual Money TotalCashAmount
        {
            get { return TotalPositionAmount(PositionAmountReturnValue.Cash); }
        }

        /// <summary>
        /// This method retrieves the cash amount in base currency of the relevant type of positions (Cash, cash fund, securities).
        /// </summary>
        /// <param name="retVal">The filter used on the positions</param>
        /// <returns>The amount in base currency</returns>
        public Money TotalPositionAmount(PositionAmountReturnValue retVal)
        {
            Money amount = new Money(0, BaseCurrency);
            switch (retVal)
            {
                case PositionAmountReturnValue.Cash:
                    amount = this.PortfolioCashGL.SettledCashTotalInBaseValue;
                    break;
                case PositionAmountReturnValue.CashFund:
                    amount = this.PortfolioInstrument.CashFundValueInBaseCurrency;
                    break;
                case PositionAmountReturnValue.BothCash:
                    amount = this.PortfolioCashGL.SettledCashTotalInBaseValue + this.PortfolioInstrument.CashFundValueInBaseCurrency;
                    break;
                case PositionAmountReturnValue.All:
                    amount = this.PortfolioCashGL.SettledCashTotalInBaseValue + this.PortfolioInstrument.TotalValueInBaseCurrency;
                    break;
                default:
                    amount = this.PortfolioCashGL.SettledCashTotalInBaseValue + this.PortfolioInstrument.TotalValueInBaseCurrency;
                    break;
            }

            //bool include = false;

            //foreach (Position pos in Portfolio)
            //{
            //    include = false;
            //    switch (retVal)
            //    {
            //        case PositionAmountReturnValue.Cash:
            //            if (pos.Size.Underlying.SecCategory.Key == SecCategories.Cash)
            //                include = true;
            //            break;
            //        case PositionAmountReturnValue.CashFund:
            //            if (pos.Size.Underlying.SecCategory.Key == SecCategories.CashManagementFund)
            //                include = true;
            //            break;
            //        case PositionAmountReturnValue.BothCash:
            //            if (pos.Size.Underlying.SecCategory.Key == SecCategories.Cash ||
            //                pos.Size.Underlying.SecCategory.Key == SecCategories.CashManagementFund)
            //                include = true;
            //            break;
            //        default:
            //            include = true;
            //            break;
            //    }

            //    if (include)
            //        amount += pos.CurrentBaseValue;
            //}
            return amount;
        }

        public virtual Money TotalCash { get { return TotalPositionAmount(PositionAmountReturnValue.Cash); } }

        public virtual Money TotalCashFund { get { return TotalPositionAmount(PositionAmountReturnValue.CashFund); } }

        public virtual Money TotalBothCash { get { return TotalPositionAmount(PositionAmountReturnValue.BothCash); } }

        public virtual Money TotalAll { get { return TotalPositionAmount(PositionAmountReturnValue.All); } }

        /// <summary>
        /// The total gross amount in base currency of all open (active) orders
        /// </summary>
        /// <returns>The amount in base currency</returns>
        public virtual Money OpenOrderAmount()
        {
            return OpenOrderAmount(OpenOrderAmountReturnValue.Gross, OrderSideFilter.All);
        }

        /// <summary>
        /// An overload of the <see cref="M:B4F.TotalGiro.Accounts.AccountTypeInternal.OpenOrderAmount">OpenOrderAmount</see> method.
        /// With this method it is possible to return either the total nett or total gross open order amount.
        /// </summary>
        /// <param name="retVal">Value determines if either the gross or nett value is returned</param>
        /// <returns>The amount in base currency</returns>
        public virtual Money OpenOrderAmount(OpenOrderAmountReturnValue retVal)
        {
            return OpenOrderAmount(retVal, OrderSideFilter.All);
        }

        /// <summary>
        /// An overload of the <see cref="M:B4F.TotalGiro.Accounts.AccountTypeInternal.OpenOrderAmount">OpenOrderAmount</see> method.
        /// With this method it is possible to return either the total nett or total gross open order amount.
        /// And it is possible to filter on either buy, sell or both orders.
        /// </summary>
        /// <param name="retVal">Value determines if either the gross or nett value is returned</param>
        /// <param name="sideFilter">Value determines which orders are included depending on the <see cref="T:B4F.TotalGiro.Orders.Side">side</see> of the order</param>
        /// <returns>The amount in base currency</returns>
        public virtual Money OpenOrderAmount(OpenOrderAmountReturnValue retVal, OrderSideFilter sideFilter)
        {
            ICurrency baseCur = AccountOwner.StichtingDetails.BaseCurrency;
            Money amount = new Money(0, baseCur);

            foreach (IOrder order in this.OpenOrdersForAccount)
            {
                if (sideFilter == OrderSideFilter.All ||
                   (sideFilter == OrderSideFilter.Buy && order.Side == Side.Buy) ||
                   (sideFilter == OrderSideFilter.Sell && order.Side == Side.Sell))
                {
                    amount += order.OpenAmount.CurrentBaseAmount;
                    if (order.Commission != null && order.Commission.IsNotZero && order.Side == Side.Buy && retVal == OpenOrderAmountReturnValue.Gross)
                        amount -= order.Commission.CurrentBaseAmount;
                }
            }
            return amount;
        }

        public virtual Tradeability TradeableStatus { get; set; }
        public virtual DateTime DateTradeabilityStatusChanged { get; set; }
        public virtual ICashPortfolio PortfolioCashGL
        {
            get
            {
                //ICashPortfolio positions = (ICashPortfolio)glpositions.AsList();
                //if (positions.ParentAccount == null) positions.ParentAccount = this;
                //return positions;
                return this.Portfolio.PortfolioCashGL;
            }
        }

        public virtual IFundPortfolio PortfolioInstrument
        {
            get
            {
                //IFundPortfolio pos = (IFundPortfolio)instrumentPositions.AsList();
                //if (pos.ParentAccount == null) pos.ParentAccount = this;
                //return pos;
                return this.Portfolio.PortfolioInstrument;
            }
        }

        public IPortfolio  Portfolio { get; set; }



        #region Private Variables

        private IManagementCompany accountOwner;
        private IList bagOfMoneyOrders = new ArrayList();
        private IList bagOfHistPositionKeys = new ArrayList();
        private IDomainCollection<IOrder> openOrdersForAccount;
        private bool valuationsRequired = true;
        private DateTime? valuationsEndDate;
        private IDomainCollection<B4F.TotalGiro.Valuations.ReportedData.IEndTermValue> endTermValues;



        #endregion



    }
}

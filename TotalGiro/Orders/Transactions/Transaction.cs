using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Orders.Transactions
{
    public abstract class Transaction : TotalGiroBase<ITransaction>, ITransaction, ICashPresentation
    {
        public Transaction()
        {
            components = new TransactionComponentCollection(this);
            positionTransactions = new TxPositionTxCollection(this);
        }

        public Transaction(IAccountTypeInternal acctA, IAccount acctB,
                InstrumentSize valueSize,
                Price price, decimal exRate, DateTime transactionDate, DateTime transactionDateTime,
                Decimal ServiceChargePercentage, Side txSide,
                ITradingJournalEntry tradingJournalEntry,
                IGLLookupRecords lookups, ListOfTransactionComponents[] components)
            : this()
        {
            this.AccountA = acctA;
            this.AccountB = acctB;
            if (valueSize != null) this.ValueSize = valueSize;
            this.TradingJournalEntry = tradingJournalEntry;
            this.TradingJournalEntry.Trade = this;
            this.Price = price;
            this.TransactionDate = transactionDate;
            this.TransactionDateTime = transactionDateTime;
            this.TxSide = txSide;
            this.ExchangeRate = exRate;
            this.CreationDate = DateTime.Now;
            this.CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
            this.ServiceChargePercentage = ServiceChargePercentage;
            createComponents(lookups, components);
            checkAccountStillTradeable();
        }

        public virtual IAccountTypeInternal AccountA { get; set; }
        public virtual IAccount AccountB { get; set; }
        public virtual bool Approved { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual string Description 
        { 
            get 
            {
                if (string.IsNullOrEmpty(this.description))
                    setDescription();
                return this.description; 
            }
            set { this.description = value; }
        }
        public virtual IExchange Exchange { get; set; }
        public virtual Decimal ExchangeRate { get; set; }
        public virtual bool IsClientSettled { get; set; }
        public virtual ITradingJournalEntry TradingJournalEntry { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        public virtual DateTime TransactionDateTime { get; set; }
        public abstract TransactionTypes TransactionType { get; }
        public string TransactionTypeDisplay { get { return this.TransactionType.ToString(); } }
        public virtual Side TxSide { get; set; }
        public virtual InstrumentSize ValueSize { get; set; }
        public virtual ITransaction StornoTransaction { get; set; }
        public virtual bool IsStorno { get; set; }
        public virtual ITransaction OriginalTransaction { get; set; }
        public virtual int MigratedTradeKey { get; set; }
        public virtual DateTime CreationDate
        {
            get { return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue; }
            set { this.creationDate = value; }
        }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime LastUpdated { get; set; }
        public virtual Decimal ServiceChargePercentage { get; set; }
        public virtual ITransactionType TradeType { get; protected set; }
        public virtual string StornoReason { get; set; }
        public virtual bool IsExecution { get { return this.TransactionType == TransactionTypes.Execution; } }
        public virtual Money ObsoleteCounterValue { get; set; }
        public virtual Money ObsoleteCommission { get; set; }
        public virtual Money ObsoleteServiceCharge { get; set; }
        public virtual bool TempMigrationFlag { get; set; }

        protected void createComponents(IGLLookupRecords lookups, ListOfTransactionComponents[] components)
        {
            if (components != null && components.Length > 0)
            {
                foreach (ListOfTransactionComponents tx in components)
                {
                    if ((tx != null) && ((tx.ComponentValue != null) && (tx.ComponentValue.IsNotZero)))
                    {
                        ITransactionComponent newComponent = new TransactionComponent(this, tx.ComponentType, this.CreationDate);
                        bool isInternalExecution = (this.IsExecution && this.AccountB.IsInternal);
                        bool isExternalExecution = (this.IsExecution && !(this.AccountB.IsInternal));
                        newComponent.AddLinesToComponent(tx.ComponentValue, tx.ComponentType, false, isExternalExecution, isInternalExecution, lookups, AccountA, AccountB);
                        this.Components.Add(newComponent);

                        //Obsolete storage method used for Interim Shadow administration
                        switch (tx.ComponentType)
                        {
                            case BookingComponentTypes.CValue:
                                this.ObsoleteCounterValue = tx.ComponentValue;
                                break;
                            case BookingComponentTypes.Commission:
                                this.ObsoleteCommission = tx.ComponentValue;
                                break;
                            case BookingComponentTypes.ServiceCharge:
                                this.ObsoleteServiceCharge = tx.ComponentValue;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        protected void checkAccountStillTradeable()
        {
            // When Account.Status is inactive -> revive the account
            if (AccountA.Status != AccountStati.Active)
            {
                AccountA.Status = AccountStati.Active;
                AccountA.NeedsAttention = true;
            }
            if (AccountB.Status != AccountStati.Active)
            {
                AccountB.Status = AccountStati.Active;
                AccountB.NeedsAttention = true;
            }
        }

        public virtual DateTime ContractualSettlementDate
        {
            get
            {
                if (contractualSettlementDate.HasValue)
                    return contractualSettlementDate.Value;
                else
                    return DateTime.MinValue;
            }
            set
            {
                contractualSettlementDate = value;
            }
        }

        public virtual DateTime ApprovalDate
        {
            get
            {
                if (approvalDate.HasValue)
                    return approvalDate.Value;
                else
                    return DateTime.MinValue;
            }
            set
            {
                approvalDate = value;
            }
        }

        public virtual string DisplayTradedInstrumentIsin
        {
            get
            {
                string isin = "";
                if (TradedInstrument != null && TradedInstrument.IsTradeable)
                    isin = ((ITradeableInstrument)TradedInstrument).Isin;
                return isin;
            }
        }

        public virtual bool IsStornoable
        {
            get { return StornoTransaction == null; }
        }

        /// <summary>
        /// When this Transaction is either a storno or a stornoed transaction, it is not relevant
        /// </summary>
        public virtual bool IsRelevant
        {
            get
            {
                bool isRelevant = true;
                if (StornoTransaction != null || IsStorno)
                    isRelevant = false;
                return isRelevant;
            }
        }

        public abstract ITransaction Storno(IAccountTypeInternal stornoAccount, IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry);

        protected ITransaction storno(IAccountTypeInternal stornoAccount, IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry, ITransaction stornoTransaction)
        {
            try
            {
                stornoTransaction.AccountA = this.AccountA;
                stornoTransaction.AccountB = stornoAccount; 
                if (this.ValueSize != null) stornoTransaction.ValueSize = this.ValueSize.Negate();
                stornoTransaction.TradingJournalEntry = tradingJournalEntry;
                stornoTransaction.TradingJournalEntry.Trade = stornoTransaction;
                stornoTransaction.Price = this.Price;
                stornoTransaction.TransactionDate = this.TransactionDate;
                stornoTransaction.TransactionDateTime = this.TransactionDateTime;
                stornoTransaction.TxSide = this.TxSide;
                stornoTransaction.ExchangeRate = this.ExchangeRate;
                stornoTransaction.CreationDate = DateTime.Now;
                stornoTransaction.CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
                stornoTransaction.IsStorno = true;
                stornoTransaction.OriginalTransaction = this;
                stornoTransaction.Exchange = this.Exchange;
                stornoTransaction.IsClientSettled = this.IsClientSettled;
                stornoTransaction.StornoReason = reason;


                foreach (ITransactionComponent comp in this.Components)
                {
                    ITransactionComponent newComp = new TransactionComponent(stornoTransaction, comp.BookingComponentType);
                    //var oldLine = comp.JournalLines.ToList().Where(x => (x.GiroAccount != null && x.GiroAccount.Key == this.AccountA.Key)).ElementAt(0);

                    IJournalEntryLine lineA = comp.JournalLines.Where(x => x.GLAccount.IsSettledWithClient).FirstOrDefault().Clone();
                    lineA.Balance = lineA.Balance.Negate();
                    newComp.JournalLines.AddJournalEntryLine(lineA);
                    newComp.MainLine = lineA;
                    lineA.StornoedLine = comp.MainLine;
                    stornoTransaction.Components.Add(newComp);

                    IJournalEntryLine lineB;
                    if (comp.BookingComponentType == BookingComponentTypes.Commission)
                    {
                        lineB = comp.JournalLines.Where(x => x.GLAccount.IsIncome).FirstOrDefault().Clone();
                        lineB.Balance = lineB.Balance.Negate();
                    }
                    else
                    {
                        lineB = comp.JournalLines.Where(x => x.GLAccount.IsSettledWithClient).FirstOrDefault().Clone();
                        lineB.GiroAccount = stornoAccount;
                    }
                    newComp.JournalLines.AddJournalEntryLine(lineB);
                }
                this.StornoTransaction = stornoTransaction;
                stornoTransaction.Approve(employee);
            }
            catch (Exception ex)
            {
                string message = Util.GetMessageFromException(ex);
                string logMessage = string.Format("Error in Storno Transactions -> TransactionID: {0}", this.Key, message);
                log.Error(logMessage);
            }
            return stornoTransaction;
        }

        public IList<IBondCouponPayment> GetBondPaymentsToStorno()
        {
            IList<IBondCouponPayment> payments = null;
            // If Bond Transaction -> storno the payments
            if (this.TradedInstrument.SecCategory.Key == SecCategories.Bond)
            {
                IBond bond = (IBond)TradedInstrument;
                if (bond != null && bond.DoesPayInterest)
                {
                    if ((this.StornoTransaction != null || IsStorno) && AccountA.IsAccountTypeCustomer)
                        payments = getBondPaymentsToStornoPerAccount(bond, AccountA);
                }
            }
            return payments;
        }

        protected IList<IBondCouponPayment> getBondPaymentsToStornoPerAccount(IBond bond, IAccountTypeInternal account)
        {
            // If Bond Transaction -> storno the payments
            if (account != null && account.IsAccountTypeCustomer)
            {
                IFundPosition pos = account.Portfolio.PortfolioInstrument.GetPosition(bond);
                if (pos != null && pos.BondCouponPayments != null && pos.BondCouponPayments.Count > 0)
                {
                    return pos.BondCouponPayments
                        .Where(x => x.CouponHistory.EndAccrualDate >= TransactionDate && 
                            x.Status != BondCouponPaymentStati.Cancelled &&
                            x.StornoBooking == null && !x.IsStorno)
                        .ToList();
                }
            }
            return null;
        }

        public virtual Price Price
        {
            get 
            {
                this.price.XRate = ExchangeRate;
                return this.price; 
            }
            set { this.price = value; }
        }


        public virtual Money CounterValueSize
        {
            get { return Components.ReturnComponentValue(BookingComponentTypes.CValue); }
        }

        public virtual DateTime BookDate
        {
            get { return this.TransactionDate; }
        }

        public virtual Money CounterValueSizeBaseCurrency
        {
            get { return Components.ReturnComponentValueInBaseCurrency(BookingComponentTypes.CValue); }
        }

        /// <summary>
        /// Returns the total nett amount involved for this transaction
        /// </summary>
        public virtual Money TotalAmount
        {
            get { return Components.TotalValue; }
        }

        /// <summary>
        /// Returns the total nett amount (in base currency) involved for this transaction
        /// </summary>
        public virtual Money TotalBaseAmount
        {
            get { return Components.BaseTotalValue; }
        }

        public virtual Money Commission
        {
            get { return Components.ReturnComponentValue(BookingComponentTypes.Commission); }
        }

        public virtual Money ServiceCharge
        {
            get { return Components.ReturnComponentValue(BookingComponentTypes.ServiceCharge); }
        }

        public virtual Money AccruedInterest
        {
            get { return Components.ReturnComponentValue(BookingComponentTypes.AccruedInterest); }
        }

        public virtual ITradeableInstrument TradedInstrument
        {
            get { return (ITradeableInstrument)ValueSize.Underlying; }
        }

        #region Approve()

        public virtual bool Approve(IInternalEmployeeLogin employee)
        {
            return Approve(employee, false);
        }

        public virtual bool Approve(IInternalEmployeeLogin employee, bool raiseStornoLimitExceptions)
        {
            return approve(employee, raiseStornoLimitExceptions);
        }

        protected bool approve(IInternalEmployeeLogin employee, bool raiseStornoLimitExceptions)
        {
            if (employee == null)
                throw new ApplicationException("It is not possible to approve when the employee is not specified.");

            bool success = false;
            if (!Approved)
            {
                if (IsStorno)
                {
                    if (!employee.VerifyStornoLimit(CounterValueSize, raiseStornoLimitExceptions))
                        return false;

                    if (employee.LoginType != LoginTypes.ComplianceEmployee && AccountB.AccountType == AccountTypes.Nostro &&
                        !((INostroAccount)AccountB).VerifyStornoLimit(CounterValueSize, raiseStornoLimitExceptions))
                        return false;
                }
                else if (TransactionType == TransactionTypes.Execution &&
                    employee.UserName.Equals(CreatedBy, StringComparison.CurrentCultureIgnoreCase))
                        throw new ApplicationException("The transaction can not be approved by the same person who created the transaction.");
                
                this.TradingJournalEntry.BookLines();
                createPositionTx();
                Approved = true;
                ApprovalDate = DateTime.Now;
                ApprovedBy = employee.UserName;
                setDescription();

                // check ManagementStartDate
                DateTime possibleMgtStartDate = TransactionDate;
                if (AccountA.AccountType == AccountTypes.Customer || AccountA.AccountType == AccountTypes.Nostro)
                {
                    if (Util.IsNullDate(((IAccountTypeCustomer)AccountA).FirstManagementStartDate) || ((IAccountTypeCustomer)AccountA).FirstManagementStartDate > possibleMgtStartDate)
                        ((IAccountTypeCustomer)AccountA).FirstManagementStartDate = possibleMgtStartDate;
                    if (Util.IsNotNullDate(((IAccountTypeInternal)AccountA).ValuationsEndDate) && ((IAccountTypeInternal)AccountA).ValuationsEndDate < TransactionDate)
                        ((IAccountTypeInternal)AccountA).ValuationsEndDate = TransactionDate;
                }
                if (AccountB.AccountType == AccountTypes.Customer || AccountB.AccountType == AccountTypes.Nostro)
                {
                    if (Util.IsNullDate(((IAccountTypeCustomer)AccountB).FirstManagementStartDate) || ((IAccountTypeCustomer)AccountB).FirstManagementStartDate > possibleMgtStartDate)
                        ((IAccountTypeCustomer)AccountB).FirstManagementStartDate = possibleMgtStartDate;
                    if (Util.IsNotNullDate(((IAccountTypeInternal)AccountB).ValuationsEndDate) && ((IAccountTypeInternal)AccountB).ValuationsEndDate < TransactionDate)
                        ((IAccountTypeInternal)AccountB).ValuationsEndDate = TransactionDate;
                }

                if ((this.TransactionType == TransactionTypes.Execution) && ((this.AccountB.AccountType == AccountTypes.Nostro) || (this.AccountB.AccountType == AccountTypes.Crumble)))
                {
                    ((IOrderExecution)this).SettleExternal(this.TransactionDate);
                }
                

                success = Approved;
            }
            return success;
        }

        public virtual bool IsApproveable
        {
            get
            {
                string currentUser = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
                bool isAllocated = true;
                if (this.TransactionType == TransactionTypes.Execution)
                    isAllocated = ((IOrderExecution)this).IsAllocated;
                return !(this.Approved && isAllocated) && !currentUser.Equals(CreatedBy, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public void Migrate()
        {
            setDescription();
            TempMigrationFlag = true;
        }
        protected abstract void setDescription();

        private void createPositionTx()
        {
            createPositionTxs(AccountA, AccountB, ValueSize, PositionsTxValueTypes.Value);
        }

        protected void createPositionTxs(IAccountTypeInternal accountA, IAccount accountB, InstrumentSize value, PositionsTxValueTypes type)
        {
            if (value != null && value.IsNotZero)
            {
                createPositionTxSub(accountA, type, TransactionSide.A);
                createPositionTxSub(accountB, type, TransactionSide.B);
            }
        }

        private void createPositionTxSub(IAccount account, PositionsTxValueTypes type, TransactionSide transSide)
        {
            StorePositionsLevel level = StorePositionsLevel.Not;
            IAccountTypeInternal acct;

            level = account.StorePositions;
            acct = account as IAccountTypeInternal;

            if (level != StorePositionsLevel.Not && acct != null)
            {
                IFundPositionTx posTx = acct.Portfolio.PortfolioInstrument.CreatePositionTx(this, transSide, type);
                this.PositionTransactions.AddPositionTx(posTx);
            }
        }

        #endregion

        #region Notas

        public virtual INota TxNota { get; set; }

        public virtual bool NotaMigrated { get; protected set; }

        public abstract INota CreateNota();

        #endregion

        public virtual IAccountTypeInternal[] GetStornoAccounts(IManagementCompany managementCompany)
        {
            return new IAccountTypeInternal[] { managementCompany.OwnAccount  };
        }

        public virtual ITransactionComponentCollection Components
        {
            get
            {
                ITransactionComponentCollection comp = (ITransactionComponentCollection)components.AsList();
                if (comp.Parent == null) comp.Parent = this;
                return comp;
            }
        }

        public virtual ITxPositionTxCollection PositionTransactions
        {
            get
            {
                ITxPositionTxCollection pos = (ITxPositionTxCollection)positionTransactions.AsList();
                if (pos.ParentTransaction == null) pos.ParentTransaction = this;
                return pos;
            }
        }

        #region Privates

        private DateTime? contractualSettlementDate;
        private DateTime? approvalDate;
        private DateTime? creationDate;
        private Price price;
        protected string description;
        private IDomainCollection<ITransactionComponent> components;
        private IDomainCollection<IFundPositionTx> positionTransactions;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("System");

        #endregion

        #region ICashPresentation Members

        public virtual string CashPresentationKey
        {
            get { return "T" + Key.ToString(); }
        }

        public virtual string TypeID
        {
            get
            {
                string id = "666";
                if (TradeType != null)
                    id = TradeType.Key.ToString();
                return "T" + id;
            }
        }

        public virtual string TypeDescription
        {
            get
            {
                string description = "Onbekend";
                if (TradeType != null)
                    description = TradeType.Description;
                return description;
            }
        }

        public virtual CashPresentationTypes CashPresentationType
        {
            get { return CashPresentationTypes.Transaction; }
        }

        public virtual IAccountTypeInternal Account
        {
            get { return (IAccountTypeInternal)AccountA; }
        }

        #endregion
    }
}

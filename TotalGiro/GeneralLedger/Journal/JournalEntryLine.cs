using System;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Communicator.Exact;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public class JournalEntryLine : IJournalEntryLine
    {
        public JournalEntryLine()
        {
            Status = JournalEntryLineStati.New;
            this.CreationDate = DateTime.Now;
            this.CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
        }

        public JournalEntryLine(int lineNumber)
            : this()
        {
            LineNumber = lineNumber;
        }

        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual IJournalEntry Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual int LineNumber
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }

        public virtual JournalEntryLineStati Status
        {
            get { return status; }
            private set { status = value; }
        }

        public virtual IGLAccount GLAccount
        {
            get { return glAccount; }
            set { glAccount = value; }
        }

        public virtual Money Debit
        {
            get { return debit; }
            set { debit = value; }
        }

        public bool DoNotExport { get; set; }

        public virtual Money Credit
        {
            get { return credit; }
            set { credit = value; }
        }

        public virtual Money Balance
        {
            get { return debit - credit; }
            set
            {
                if (value.IsGreaterThanZero)
                {
                    debit = value;
                    credit = value.ZeroedAmount();
                }
                else if (value.IsLessThanZero)
                {
                    debit = value.ZeroedAmount();
                    credit = value.Abs();
                }
                else
                    throw new ApplicationException("Cannot set Balance of Journal Entry Line to zero.");
            }
        }

        public virtual Money BaseBalance
        {
            get
            {
                Money amount = null;
                if (Balance != null)
                    amount = Balance.BaseAmount;
                else
                    amount = new Money(0M, Currency.BaseCurrency);
                return amount;
            }
        }

        public virtual ICurrency Currency
        {
            get
            {
                ICurrency cur = null;
                if (Balance != null)
                    cur = (ICurrency)Balance.Underlying;
                return cur;
            }
        }

        /// <summary>
        /// Exchange rate to base currency
        /// </summary>
        public virtual decimal ExchangeRate
        {
            get
            {
                if (Balance != null)
                    return Balance.XRate;
                else
                    return 1M;
            }
        }

        public virtual bool IsAdminBooking
        {
            get
            {
                return this.Parent.Journal.IsAdminAccount;
            }


        }

        public virtual IAccountTypeInternal GiroAccount
        {
            get { return giroAccount; }
            set { giroAccount = value; }
        }

        public virtual DateTime BookDate
        {
            get { return (bookDate.HasValue ? bookDate.Value : Parent.TransactionDate); }
            set { bookDate = value; }
        }

        public virtual DateTime ValueDate
        {
            get { return (bookDate.HasValue ? bookDate.Value : Parent.TransactionDate); }
            set { bookDate = value; }
        }

        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        public virtual string OriginalDescription
        {
            get { return originalDescription; }
            set { originalDescription = value; }
        }

        public virtual IJournalEntryLine StornoedLine
        {
            get { return stornoedLine; }
            set { stornoedLine = value; }
        }

        public virtual IJournalEntryLine Storno
        {
            get { return (bagStorno != null && bagStorno.Count == 1 ? (IJournalEntryLine)bagStorno[0] : null); }
        }

        /// <summary>
        /// If the journalEntryLine is either stornoed or a storno it is not relevant
        /// </summary>
        public virtual bool IsRelevant
        {
            get
            {
                bool isRelevant = true;
                if (Storno != null || IsStorno)
                    isRelevant = false;
                else if (Parent.JournalEntryType == JournalEntryTypes.Transaction)
                {
                    isRelevant = false;
                    ITransaction transaction = ((ITradingJournalEntry)this.Parent).Trade;
                    if (transaction != null)
                        isRelevant = transaction.IsRelevant;
                }
                return isRelevant;
            }
        }

        public virtual bool IsCashTransfer
        {
            get
            {
                bool isCashTransfer = false;
                if (GLAccount != null &&
                    (GLAccount.CashTransferType == CashTransferTypes.Deposit ||
                     GLAccount.CashTransferType == CashTransferTypes.Withdrawal))
                    isCashTransfer = true;
                return isCashTransfer;
            }
        }

        public virtual bool IsForeignExchange
        {
            get
            {
                bool isForeignExchange = false;
                if (GLAccount != null &&
                    (GLAccount.CashTransferType == CashTransferTypes.ForexBaseCurrency ||
                     GLAccount.CashTransferType == CashTransferTypes.ForexNonBaseCurrency))
                    isForeignExchange = true;
                return isForeignExchange;
            }
        }

        public virtual IInstrument BookingRelatedInstrument
        {
            get
            {
                IInstrument instrument = null;
                if (this.BookComponent != null)
                {
                    switch (this.BookComponent.Parent.BookingComponentParentType)
                    {
                        case BookingComponentParentTypes.Transaction:
                            instrument = ((ITransactionComponent)this.BookComponent.Parent).ParentTransaction.TradedInstrument;
                            break;
                        case BookingComponentParentTypes.CashDividend:
                            IGeneralOperationsBooking generalBooking = ((IGeneralOperationsComponent)this.BookComponent.Parent).ParentBooking;
                            instrument = ((ICashDividend)generalBooking).DividendDetails.Instrument;
                            break;
                        case BookingComponentParentTypes.AccruedInterest:
                            IBondCouponPayment couponPayment = (IBondCouponPayment)((IGeneralOperationsComponent)this.BookComponent.Parent).ParentBooking;
                            instrument = couponPayment.Bond;
                            break;
                    }
                }
                return instrument;
            }
        }

        public virtual ISubledgerEntry SubledgerEntry
        {
            get { return subledgerEntry; }
            set { subledgerEntry = value; }
        }

        public virtual IImportedBankMovement ImportedBankMovement
        {
            get { return importedBankMovement; }
            set { importedBankMovement = value; }
        }

        public virtual bool IsSettledStatus { get; set; }
        public virtual ICashSubPosition ParentSubPosition { get; set; }
        public virtual string TegenrekeningNumber { get; set; }
        public virtual CashTransferDetailTypes CashTransferDetailType { get; set; }


        public virtual string TotalGiroOrderID
        {
            get
            {
                if (this.tradeTransaction == null) this.tradeTransaction = (ITransactionOrder)this.BookComponent.Parent;
                return this.tradeTransaction.Order.OrderID.ToString();
            }
        }

        public virtual IBookingComponent BookComponent { get; set; }
        public virtual IExternalSettlement MatchedSettlement { get; set; }

        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }


        /// <summary>
        /// This line is used in a buy model instruction.
        /// It is important that either the instruction is filled, the CashInitiatedOrder is filled or the SkipOrders flag is set.
        /// If not this line will keep appearing in the new cash screen.
        /// </summary>
        public virtual IInstruction Instruction { get; set; }

        /// <summary>
        /// This order is initiated by this line.
        /// For instance to buy/sell cashfund to get rid of the short/long cash position.
        /// </summary>
        public virtual IOrderAmountBased CashInitiatedOrder { get; set; }

        /// <summary>
        /// When true this line does not generate a CashInitiatedOrder or is used in an instruction
        /// </summary>
        public virtual bool SkipOrders { get; set; }

        public virtual int GroupingKey
        {
            get
            {
                if (this.BookComponent != null)
                    return this.BookComponent.Parent.Key;
                else
                    return this.Key;
            }
        }

        public virtual bool GroupingbyTransaction
        {
            get
            {
                return this.BookComponent != null;
            }
        }

        public virtual string DisplayStatus
        {
            get { return Status.ToString(); }
        }

        public virtual bool GLAccountIsFixed
        {
            get { return (GLAccount != null ? GLAccount.IsFixed : false); }
        }

        public virtual bool IsStorno
        {
            get { return (StornoedLine != null); }
        }

        public virtual bool IsStornoable
        {
            get
            {
                bool isStornoable = false;
                if (Key != 0 && !GLAccountIsFixed && Status == JournalEntryLineStati.Booked && !IsStorno && Storno == null)
                {
                    if (BookComponent == null)
                        isStornoable = true;
                    else if (BookComponent.Parent != null)
                    {
                        switch (BookComponent.Parent.BookingComponentParentType)
                        {
                            case BookingComponentParentTypes.CashTransfer:
                            case BookingComponentParentTypes.ForexTransaction:
                                isStornoable = true;
                                break;
                        }
                    }
                }
                return isStornoable;
            }
        }

        public virtual bool IsEditable
        {
            get { return (Key != 0 && !GLAccountIsFixed && Status == JournalEntryLineStati.New && !IsStorno); }
        }

        public virtual bool IsDeletable
        {
            get { return (Key != 0 && !GLAccountIsFixed && Status == JournalEntryLineStati.New); }
        }

        public virtual bool IsAllowedToAddTransferFee
        {
            get
            {
                bool isAllowed = false;
                if (Status == JournalEntryLineStati.Booked && Storno == null && StornoedLine == null &&
                    GiroAccount != null && IsCashTransfer &&
                    BookComponent != null && BookComponent.Parent != null &&
                    BookComponent.Parent.BookingComponentParentType == BookingComponentParentTypes.CashTransfer)
                {
                    ICashTransfer mutation = ((IGeneralOperationsComponent)BookComponent.Parent).ParentBooking as ICashTransfer;
                    if (mutation != null)
                    {
                        Money fee = mutation.TransferFee;
                        if (fee == null || fee.IsZero)
                            isAllowed = true;
                    }
                }
                return isAllowed;
            }
        }

        public virtual DateTime CreationDate
        {
            get
            {
                return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue;
            }
            set
            {
                this.creationDate = value;
            }
        }

        public virtual string CreatedBy { get; set; }
        public virtual string BookedBy { get; set; }

        #region Methods

        public IJournalEntryLine CreateStorno()
        {
            if (IsStornoable)
            {
                IJournalEntryLine storno = new JournalEntryLine();
                storno.GLAccount = GLAccount;
                storno.Balance = Balance.Negate();
                storno.GiroAccount = GiroAccount;
                storno.Description = string.Format("Storno of line {0}", LineNumber);
                storno.StornoedLine = this;
                storno.OriginalDescription = OriginalDescription;
                return storno;
            }
            else
                throw new ApplicationException(string.Format("Journal Entry Line number {0} is not stornoable.", LineNumber));
        }

        public IJournalEntryLine CreateStorno(int lineNumber)
        {
            IJournalEntryLine storno = this.CreateStorno();
            storno.LineNumber = lineNumber;
            return storno;
        }

        public void ClientSettle(ITradingJournalEntry clientSettleJournal)
        {
            IJournalEntryLine newLine1 = new JournalEntryLine();
            clientSettle(clientSettleJournal, newLine1, this.GLAccount, this.Balance.Negate(), this.BookComponent, this.GiroAccount);

            IJournalEntryLine newLine2 = new JournalEntryLine();
            clientSettle(clientSettleJournal, newLine2, this.GLAccount.GLSettledAccount, this.Balance, this.BookComponent, this.GiroAccount);
            if (BookComponent.MainLine.Key == this.key)
                BookComponent.MainLine = newLine2;

            this.IsSettledStatus = true;

        }

        internal void clientSettle(IJournalEntry clientSettleJournal, IJournalEntryLine newLine, IGLAccount account, Money balance, IBookingComponent bookingComponent, IAccountTypeInternal giroAccount)
        {
            newLine.GLAccount = account;
            newLine.Balance = balance;
            newLine.BookComponent = bookingComponent;
            newLine.GiroAccount = giroAccount;
            newLine.IsSettledStatus = true;
            clientSettleJournal.Lines.AddJournalEntryLine(newLine);
            newLine.BookLine();

        }

        public IJournalEntryLine Clone()
        {
            if (Status == JournalEntryLineStati.Booked)
            {
                IJournalEntryLine clone = new JournalEntryLine();
                clone.GLAccount = GLAccount;
                clone.Balance = Balance;
                clone.GiroAccount = GiroAccount;
                clone.Description = string.Format("Correction of line {0}", LineNumber);
                clone.OriginalDescription = OriginalDescription;
                return clone;
            }
            else
                throw new ApplicationException(
                    string.Format("Could not clone Journal Entry Line number {0} because its status was not '{1}'.",
                                  LineNumber, JournalEntryLineStati.Booked));
        }

        public IJournalEntryLine Clone(int lineNumber)
        {
            IJournalEntryLine clone = this.Clone();
            clone.LineNumber = lineNumber;
            return clone;
        }

        public void BookLine()
        {
            if ((this.Status == JournalEntryLineStati.New) || (this.Status == JournalEntryLineStati.Booked))
            {
                if ((this.GLAccount.RequiresGiroAccount) && (this.GiroAccount != null) && (this.GiroAccount.AccountType != AccountTypes.Trading))
                {
                    ICashSubPosition acctPosition = this.GiroAccount.Portfolio.PortfolioCashGL.GetSubPosition(this.Debit.Underlying.ToCurrency, this.GLAccount);
                    acctPosition.JournalLines.AddLine(this);

                    // check for afsluit provisie
                    if (this.GLAccount.CashTransferType == CashTransferTypes.TransferFee)
                    {
                        IJournalEntryLine mainLine = ((ICashTransfer)((ICashTransferComponent)this.BookComponent.Parent).ParentBooking).MainTransferLine;
                        
                        if (mainLine.SkipOrders || (mainLine.Instruction != null && mainLine.Instruction.Orders != null && mainLine.Instruction.Orders.Count > 0))
                            this.SkipOrders = true;
                    }
                }

                // check ManagementStartDate
                // For Cash -> Add 1 day
                if (!Parent.Journal.IsAdminAccount &&
                    GiroAccount != null &&
                    !GLAccount.IsToSettleWithClient &&
                    (GiroAccount.AccountType == AccountTypes.Customer || GiroAccount.AccountType == AccountTypes.Nostro))
                {
                    // When Account.Status is inactive -> revive the account
                    if (GiroAccount.Status != AccountStati.Active)
                    {
                        GiroAccount.Status = AccountStati.Active;
                        GiroAccount.NeedsAttention = true;
                    }

                    DateTime possibleMgtStartDate = Parent.TransactionDate.AddDays(1);
                    if (Util.IsNullDate(((IAccountTypeCustomer)GiroAccount).FirstManagementStartDate) || ((IAccountTypeCustomer)GiroAccount).FirstManagementStartDate > possibleMgtStartDate)
                        ((IAccountTypeCustomer)GiroAccount).FirstManagementStartDate = possibleMgtStartDate;
                    if (Util.IsNotNullDate(((IAccountTypeInternal)GiroAccount).ValuationsEndDate) && ((IAccountTypeInternal)GiroAccount).ValuationsEndDate < Parent.TransactionDate)
                        ((IAccountTypeInternal)GiroAccount).ValuationsEndDate = Parent.TransactionDate;
                }
                this.BookDate = this.BookDate;
                this.status = JournalEntryLineStati.Booked;
                if (!this.IsAdminBooking) checkCreateCashMutationComponent();
                this.BookedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
            }
        }

        private void checkCreateCashMutationComponent()
        {
            if (GiroAccount != null && GLAccount != null &&
                Status == JournalEntryLineStati.Booked &&
                BookComponent == null)
            {
                if (IsCashTransfer)
                {
                    ICashTransfer cashMut = new CashTransfer(this);
                }
                // TODO
                //else if (IsForeignExchange)
                //{
                //    IForeignExchange forex = new ForeignExchange(this);
                //}
                else
                    throw new ApplicationException("It is not possible to create a booking for a giro account else than either deposit or withdrawal.");
            }
        }

        #endregion

        #region Privates

        private DateTime? creationDate;
        private DateTime? bookDate;
        private DateTime lastUpdated = DateTime.MinValue;
        private IAccountTypeInternal giroAccount;
        private IGLAccount glAccount;
        private IImportedBankMovement importedBankMovement;
        private IJournalEntry parent;
        private IJournalEntryLine stornoedLine;
        private IList bagStorno = new ArrayList();
        private int key;
        private int lineNumber;
        private ISubledgerEntry subledgerEntry;
        private ITransactionOrder tradeTransaction;
        private JournalEntryLineStati status;
        private Money credit;
        private Money debit;
        private string description;
        private string originalDescription;

        #endregion

        //#region ICashPresentation Members

        //public virtual string CashPresentationKey
        //{
        //    get { return "J" + Key.ToString(); }
        //}

        //public virtual CashPresentationTypes CashPresentationType
        //{
        //    get { return CashPresentationTypes.JournalBooking; }
        //}

        //public virtual string TypeID
        //{
        //    get
        //    {
        //        string id = "666";
        //        if (GLAccount != null && GLAccount.GLBookingType != null)
        //            id = GLAccount.GLBookingType.Key.ToString();
        //        return "GL" + id;
        //    }
        //}

        //public virtual string TypeDescription
        //{
        //    get 
        //    {
        //        string description = "Onbekend";
        //        if (GLAccount != null && GLAccount.GLBookingType != null)
        //            description = GLAccount.GLBookingType.Description;
        //        return description;
        //    }
        //}

        //public virtual IAccountTypeCustomer Account
        //{
        //    get { return (IAccountTypeCustomer)GiroAccount; }
        //}

        //public virtual Money TotalAmount
        //{
        //    get { return Balance.Negate(); }
        //}

        //public virtual DateTime TransactionDate 
        //{
        //    get { return Parent.TransactionDate; }
        //}

        //#endregion
    }
}

using System;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public abstract class GeneralOperationsBooking : IGeneralOperationsBooking, ICashPresentation
    {
        #region Constructor

        protected GeneralOperationsBooking() 
        {
            this.CreationDate = DateTime.Now;
            this.IsNotarizable = true;
        }

        public GeneralOperationsBooking(IAccountTypeInternal account, IJournalEntry journalEntry, string description)
            :this()
        {
            this.Account = account;
            this.GeneralOpsJournalEntry = journalEntry;
            this.Description = description;
        }

        #endregion

        #region Props

        public virtual int Key { get; set; }
        public virtual IJournalEntry GeneralOpsJournalEntry { get; set; }
        public abstract GeneralOperationsBookingTypes BookingType { get; }
        public virtual IAccountTypeInternal Account { get; set; }
        public virtual decimal TaxPercentage { get; set; }
        public virtual string Description { get; set; }
        public virtual IGeneralOperationsBooking StornoBooking { get; set; }
        public virtual bool IsStorno { get; set; }
        public virtual string StornoReason { get; set; }
        public virtual IGeneralOperationsBooking OriginalBooking { get; set; }
        public virtual IGLBookingType GLBookingType { get; protected set; }
        public virtual bool IsNotarizable { get; protected set; }
        public virtual INota BookNota { get; set; }
        public virtual bool NotaMigrated { get; protected set; }

        public virtual DateTime CreationDate
        {
            get
            {
                if (creationDate.HasValue)
                    return creationDate.Value;
                else
                    return DateTime.MinValue;
            }
            set { creationDate = value; }
        }

        public virtual bool IsStornoable
        {
            get { return StornoBooking == null; }
        }

        public virtual IGeneralOperationsComponentCollection Components
        {
            get
            {
                IGeneralOperationsComponentCollection comp = (IGeneralOperationsComponentCollection)components.AsList();
                if (comp.Parent == null) comp.Parent = this;
                return comp;
            }
        }

        public virtual IOrderAmountBased CashInitiatedOrder
        {
            get
            {
                IOrderAmountBased order = null;
                if (this.Components.FirstOrDefault() != null)
                    order = this.Components.First().MainLine.CashInitiatedOrder;
                return order;
            }
            set
            {
                foreach (IGeneralOperationsComponent comp in Components)
                {
                    comp.MainLine.CashInitiatedOrder = value;
                }
            }
        }

        #endregion

        #region Methods

        public abstract IGeneralOperationsBooking Storno(IInternalEmployeeLogin employee, string reason, IMemorialBooking journalEntry);
        public abstract INota CreateNota();

        protected IGeneralOperationsBooking storno(IInternalEmployeeLogin employee, string reason, IMemorialBooking journalEntry, IGeneralOperationsBooking stornoBooking)
        {
            if (employee == null)
                throw new ApplicationException(
                    "Creating a Storno is not possible if employee not specified.");

            if (employee != null)
                employee.VerifyStornoLimit(this.Components.TotalBaseAmount.Abs(), true);
            
            stornoBooking.Account = Account;
            stornoBooking.GeneralOpsJournalEntry = journalEntry;
            stornoBooking.Description = Description;
            stornoBooking.TaxPercentage = TaxPercentage;
            stornoBooking.IsStorno = true;
            stornoBooking.StornoReason = reason;
            stornoBooking.OriginalBooking = this;
            stornoBooking.CreationDate = DateTime.Now;

            foreach (IGeneralOperationsComponent comp in this.Components)
            {
                IGeneralOperationsComponent newComp = comp.CloneAndStorno(stornoBooking);
                newComp.MainLine = newComp.JournalLines.Where(x => x.GiroAccount != null).FirstOrDefault();
                stornoBooking.Components.Add(newComp);
                newComp.ParentBooking = stornoBooking;
            }
            this.StornoBooking = stornoBooking;
            return StornoBooking;
        }
        
        /// <summary>
        /// This method checks whether an order for this cash tx needs to be created
        /// </summary>
        /// <returns></returns>
        public bool NeedToCreateCashInitiatedOrder(out ITradeableInstrument instrument)
        {
            bool placeOrder = false;
            instrument = null;

            if (CashInitiatedOrder != null)
                throw new ApplicationException("This transaction does not need an order");

            Money orderAmount = Components.TotalAmount;
            if (Account.AccountType == AccountTypes.Customer && Account.Status == AccountStati.Active &&
                orderAmount != null && orderAmount.IsNotZero)
            {
                // check if absolutely necessary
                Money totalCash = Account.TotalCash;
                if (totalCash != null && totalCash.IsGreaterThanZero && (totalCash + orderAmount).IsGreaterThanOrEqualToZero)
                {
                    IAccountOrderCollection orders = Account.OpenOrdersForAccount.Filter(OrderTypes.AmountBased, OrderSideFilter.Buy);
                    if (orders == null || orders.Count == 0)
                        return false;
                }

                // get the instrument 
                ICustomerAccount account = (ICustomerAccount)Account;
                IModelHistory modelHistory = account.ModelPortfolioChanges.GetItemByDate(this.GeneralOpsJournalEntry.TransactionDate);

                if (modelHistory == null)
                    instrument = ((IAssetManager)account.AccountOwner).CashManagementFund;
                else
                {
                    IModelVersion mv = modelHistory.ModelPortfolio.LatestVersion;
                    if (mv.ModelInstruments.Count > 0)
                        instrument = mv.GetCashFundOrAlternative();
                }

                // If instrument not null & IsTradeable -> place order for it
                if (instrument != null && instrument.IsTradeable)
                {
                    // check if there is enough the instrument
                    IFundPosition pos = Account.Portfolio.PortfolioInstrument.GetPosition(instrument);
                    if (pos != null && pos.Size.IsGreaterThanZero && ((Money)(pos.CurrentBaseValue + orderAmount)).Sign)
                    {
                        // check if an order already exists for the position
                        Money existingOrderAmt = null;
                        IAccountOrderCollection orders = account.OpenOrdersForAccount.Filter(instrument, OrderSideFilter.All);
                        if (orders != null)
                        {
                            existingOrderAmt = orders.TotalAmount();
                            if (existingOrderAmt != null && existingOrderAmt.IsNotZero)
                            {
                                if (((Money)(pos.CurrentBaseValue + orderAmount + existingOrderAmt)).Sign)
                                    placeOrder = true;
                            }
                            else
                                placeOrder = true;
                        }
                        else
                            placeOrder = true;
                    }
                    else
                    {
                        // reset instrument to null
                        instrument = null;
                    }
                }

                // If total portfolio is enough -> sell other positions
                if (!placeOrder && (Account.Portfolio.PortfolioInstrument.TotalValueInBaseCurrency + orderAmount).IsGreaterThanOrEqualToZero)
                    placeOrder = true;
            }
            return placeOrder;
        }

        #endregion

        #region Privates

        private DateTime? creationDate;
        private IDomainCollection<IGeneralOperationsComponent> components = new GeneralOperationsComponentCollection();

        #endregion

        #region ICashPresentation Members

        public virtual string CashPresentationKey
        {
            get { return "G" + Key.ToString(); }
        }

        public virtual string TypeID
        {
            get
            {
                string id = "666";
                if (GLBookingType != null)
                    id = GLBookingType.Key.ToString();
                return "GL" + id;
            }
        }

        public virtual string TypeDescription
        {
            get
            {
                string description = "Onbekend";
                if (GLBookingType != null)
                    description = GLBookingType.Description;
                return description;
            }
        }

        public virtual CashPresentationTypes CashPresentationType
        {
            get 
            {
                switch (BookingType)
                {
                    case GeneralOperationsBookingTypes.ManagementFee:
                        return CashPresentationTypes.ManagementFee;
                    case GeneralOperationsBookingTypes.CashDividend:
                        return CashPresentationTypes.CashDividend;
                    default:
                        return CashPresentationTypes.CashTransfer;
                }
            }
        }

        public virtual Money TotalAmount
        {
            get 
            { 
                return Components.TotalAmount;
            }
        }

        public virtual DateTime BookDate
        {
            get { return this.GeneralOpsJournalEntry.TransactionDate; }
        }

        #endregion
    }
}

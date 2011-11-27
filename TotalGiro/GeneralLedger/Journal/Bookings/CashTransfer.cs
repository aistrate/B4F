using System;
using System.Linq;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.CorporateAction;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class CashTransfer : GeneralOperationsBooking, ICashTransfer
    {
        #region Constructor

        protected CashTransfer() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dividendDetails">The details of the cash dividend (date, price)</param>
        /// <param name="units">The total number of units over which dividend is paid</param>
        public CashTransfer(IJournalEntryLine line)
            : base((ICustomerAccount)line.GiroAccount, line.Parent, getDescription(line))
        {
            if (!(line.Status == JournalEntryLineStati.Booked && line.BookComponent == null &&
                (line.GLAccount.CashTransferType == CashTransferTypes.Deposit || line.GLAccount.CashTransferType == CashTransferTypes.Withdrawal)))
                throw new ApplicationException("This journal entry line is not a valid cashmutation.");

            ICashTransferComponent newComponent = new CashTransferComponent(this, BookingComponentTypes.CashTransfer, this.CreationDate);
            line.BookComponent = newComponent.Component;
            newComponent.JournalLines.Add(line);
            line.BookComponent.MainLine = line;
            newComponent.Component.SetDescription(this.Description);
            this.Components.Add(newComponent);
        }

        protected static string getDescription(IJournalEntryLine line)
        {
            string description = "";
            if (!string.IsNullOrEmpty(line.Description))
                description = line.Description;
            else
                description = line.GLAccount.CashTransferType.ToString();
            return description;
        }

        #endregion

        #region Props

        public virtual IJournalEntryLine MainTransferLine 
        {
            get
            {
                ICashTransferComponent comp = this.Components.Where(x => x.BookingComponentType == BookingComponentTypes.CashTransfer).FirstOrDefault() as ICashTransferComponent;
                if (comp != null)
                    return comp.MainLine;
                else
                    throw new ApplicationException("This is a clusterfuck");
            }
        }

        public virtual Money TransferAmount
        {
            get
            {
                return this.Components.ReturnComponentValue(BookingComponentTypes.CashTransfer);
            }
        }

        public virtual Money TransferFee
        {
            get
            {
                return this.Components.ReturnComponentValue(BookingComponentTypes.CashTransferFee);
            }
        }

        #endregion

        #region Overrides

        public override GeneralOperationsBookingTypes BookingType { get { return GeneralOperationsBookingTypes.CashTransfer; } }

        public override IGeneralOperationsBooking Storno(IInternalEmployeeLogin employee, string reason, IMemorialBooking journalEntry)
        {
            CashTransfer newStorno = new CashTransfer();
            return this.storno(employee, reason, journalEntry, newStorno);
        }

        /// <summary>
        /// This method creates the nota of the ManagementFee
        /// </summary>
        /// <returns>True when successfull</returns>
        public override INota CreateNota()
        {
            if (this.GeneralOpsJournalEntry.Status == JournalEntryStati.Booked && !NotaMigrated && StornoBooking == null)
            {
                if (BookNota == null)
                    return new NotaDeposit(this);
                else
                    throw new ApplicationException(string.Format("ManagementFee {0} already has a nota ({1}).", Key, BookNota.Key));
            }
            return null;
        }

        #endregion

        #region Methods

        public bool AddTransferFee(Money transferFee, IGLLookupRecords lookups, string description)
        {
            bool success = false;
            if (transferFee == null || lookups == null)
                throw new ApplicationException("It is not possible to add transfer fee since not all parameters are valid.");

            IGeneralOperationsComponent comp = Components.Where(u => u.BookingComponentType == BookingComponentTypes.CashTransfer).FirstOrDefault();
            if (!(comp != null && comp.MainLine != null && comp.MainLine.IsAllowedToAddTransferFee))
                throw new ApplicationException("It is not possible to add transfer fee to this transfer.");

            if (TransferFee != null && TransferFee.IsNotZero)
                throw new ApplicationException("It is not possible to add transfer fee more than once.");

            if (transferFee.Sign || transferFee.Abs().Quantity > TransferAmount.Abs().Quantity)
                throw new ApplicationException("The transfer fee can not be higher than the transfer amount.");

            if (transferFee != null && transferFee.IsNotZero)
            {
                ICashTransferComponent newComponent = new CashTransferComponent(this, BookingComponentTypes.CashTransferFee, this.CreationDate);
                newComponent.AddLinesToComponent(transferFee, BookingComponentTypes.CashTransferFee, true, false, false, lookups, Account);
                newComponent.Component.SetDescription(description);
                this.Components.Add(newComponent);
                success = true;
            }
            return success;
        }

        #endregion
    }
}

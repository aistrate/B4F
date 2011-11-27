using System;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Communicator.KasBank;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.BackOffice.Orders
{
    public class MoneyTransferOrder : IMoneyTransferOrder
    {
        #region Constructors

        public MoneyTransferOrder() { }

        public MoneyTransferOrder(IEffectenGiro stichtingDetails,
                            Money amount,
                            ICustomerAccount transfereeAccount,
                            ICounterAccount transfereeCounterAccount,
                            string transferDescription1,
                            string transferDescription2,
                            string transferDescription3,
                            string transferDescription4,
                            DateTime processDate,
                            string narBenef1,
                            string narBenef2,
                            string narBenef3,
                            string narBenef4,
                            IndicationOfCosts costIndication)
        {
            this.CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
            this.TransferorJournal = stichtingDetails.DefaultWithdrawJournal;
            this.NarDebet1 = stichtingDetails.StichtingName;
            this.NarDebet2 = stichtingDetails.ResidentialAddress.AddressLine1;
            this.NarDebet3 = stichtingDetails.ResidentialAddress.AddressLine2;
            this.NarDebet4 = stichtingDetails.ResidentialAddress.Country.CountryName;

            this.ProcessDate = processDate;
            if (amount != null)
                this.Amount = amount.Abs();
            this.TransfereeAccount = transfereeAccount;
            this.TransfereeCounterAccount = transfereeCounterAccount;

            this.NarBenef1 = narBenef1;
            this.NarBenef2 = narBenef2;
            this.NarBenef3 = narBenef3;
            this.NarBenef4 = narBenef4;

            this.TransferDescription1 = transferDescription1;
            this.TransferDescription2 = transferDescription2;
            this.TransferDescription3 = transferDescription3;
            this.TransferDescription4 = transferDescription4;

            this.CostIndication = costIndication;
        }

        public MoneyTransferOrder(ICashWithdrawalInstruction instruction)
        {
            if (instruction == null)
                throw new ApplicationException(err_def + "the instruction is mandatory");

            ICounterAccount counterAccount = null;
            if (instruction.CounterAccount != null)
                counterAccount = instruction.CounterAccount;
            else if (instruction.Rule != null)
                counterAccount = instruction.Rule.CounterAccount;
            if (counterAccount == null)
                throw new ApplicationException(err_def + "the counter account is mandatory");

            Money amount = null;
            if (instruction.Amount != null)
                amount = instruction.Amount.Abs();

            fillFromInstruction(instruction, counterAccount, amount);

            this.TransferDescription1 = getDescriptionLine(instruction, 1);
            this.TransferDescription2 = getDescriptionLine(instruction, 2);
            this.TransferDescription3 = getDescriptionLine(instruction, 3);
            this.TransferDescription4 = getDescriptionLine(instruction, 4, counterAccount);

            instruction.MoneyTransferOrder = this;
        }

        public MoneyTransferOrder(IClientDepartureInstruction instruction, Money amount)
        {
            if (instruction == null)
                throw new ApplicationException(err_def + "the instruction is mandatory");

            ICounterAccount counterAccount = null;
            if (instruction.CounterAccount != null)
                counterAccount = instruction.CounterAccount;
            if (counterAccount == null)
                throw new ApplicationException(err_def + "the counter account is mandatory");

            fillFromInstruction(instruction, counterAccount, amount);

            this.TransferDescription1 = getDescriptionLine(instruction, 1);
            this.TransferDescription2 = getDescriptionLine(instruction, 2);
            this.TransferDescription3 = getDescriptionLine(instruction, 3);
            this.TransferDescription4 = getDescriptionLine(instruction, 4, counterAccount);

            instruction.MoneyTransferOrder = this;
        }

        protected void fillFromInstruction(IInstruction instruction, ICounterAccount counterAccount, Money amount)
        {
            this.CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
            ICustomerAccount transfereeAccount = (ICustomerAccount)instruction.Account;
            if (transfereeAccount == null)
                throw new ApplicationException(err_def + "the account is mandatory");

            string errMessage = err_def + string.Format("Could not create money transfer order for account {0}: ", transfereeAccount.DisplayNumberWithName);

            IEffectenGiro stichtingDetails = transfereeAccount.AccountOwner.StichtingDetails;
            if (stichtingDetails == null)
                throw new ApplicationException(errMessage + "the stichting details are mandatory");

            this.TransferorJournal = stichtingDetails.DefaultWithdrawJournal;
            this.NarDebet1 = stichtingDetails.StichtingName;
            this.NarDebet2 = stichtingDetails.ResidentialAddress.AddressLine1;
            this.NarDebet3 = stichtingDetails.ResidentialAddress.AddressLine2;
            this.NarDebet4 = stichtingDetails.ResidentialAddress.Country.CountryName;

            this.ProcessDate = DateTime.Today;
            this.Amount = amount;
            this.TransfereeAccount = transfereeAccount;
            this.TransfereeCounterAccount = counterAccount;

            this.NarBenef1 = getMaxLengthString(counterAccount.AccountName, 35);
            this.BenefBankAcctNr = counterAccount.Number;

            Address address = null;
            if (counterAccount.BeneficiaryAddress != null && !counterAccount.BankAddress.IsEmpty)
                address = counterAccount.BeneficiaryAddress;
            else if (transfereeAccount.PrimaryAccountHolder != null &&
                transfereeAccount.PrimaryAccountHolder.Contact != null &&
                transfereeAccount.PrimaryAccountHolder.Contact.CurrentNAW != null)
            {
                IContactsNAW contactsNaw = transfereeAccount.PrimaryAccountHolder.Contact.CurrentNAW;
                if (contactsNaw.ResidentialAddress != null && !contactsNaw.ResidentialAddress.IsEmpty)
                    address = contactsNaw.ResidentialAddress;
                else
                    address = contactsNaw.PostalAddress;
            }

            if (address != null && !address.IsEmpty)
            {
                this.NarBenef2 = getAddressLine(address, 1);
                this.NarBenef3 = getAddressLine(address, 2);
                this.NarBenef4 = getAddressLine(address, 3);
            }
        }

        #endregion

        #region Props

        public virtual int Key { get; set; }
        public virtual IJournal TransferorJournal { get; set; }
        public virtual Money Amount { get; set; }
        public virtual ICustomerAccount TransfereeAccount { get; set; }
        public virtual ICounterAccount TransfereeCounterAccount { get; set; }
        public virtual string BenefBankAcctNr { get; set; }
        public virtual string NarDebet1 { get; set; }
        public virtual string NarDebet2 { get; set; }
        public virtual string NarDebet3 { get; set; }
        public virtual string NarDebet4 { get; set; }
        public virtual string NarBenef1 { get; set; }
        public virtual string NarBenef2 { get; set; }
        public virtual string NarBenef3 { get; set; }
        public virtual string NarBenef4 { get; set; }
        public virtual string SwiftAddress { get; set; }
        public virtual string TransferDescription1 { get; set; }
        public virtual string TransferDescription2 { get; set; }
        public virtual string TransferDescription3 { get; set; }
        public virtual string TransferDescription4 { get; set; }
        public virtual DateTime ProcessDate { get; set; }
        public virtual IGLDSTD GLDSTDRecord { get; set; }
        public virtual bool Approved { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime ApprovalDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual IndicationOfCosts CostIndication { get; set; }

        public virtual string Reference
        {
            get
            {
                if ((this.reference == null || this.reference.Length == 0) && Key != 0)
                    setReference();
                return this.reference;
            }
            set { this.reference = value; }
        }

        /// <summary>
        /// Returns the total Description
        /// </summary>
        public virtual string DisplayDescription
        {
            get
            {
                StringBuilder sb = new StringBuilder(TransferDescription1.Trim());
                if (!string.IsNullOrEmpty(TransferDescription2))
                    sb.Append(" " + TransferDescription2.Trim());
                if (!string.IsNullOrEmpty(TransferDescription3))
                    sb.Append(" " + TransferDescription3.Trim());
                if (!string.IsNullOrEmpty(TransferDescription4))
                    sb.Append(" " + TransferDescription4.Trim());
                return sb.ToString();
            }
        }
        
        /// <summary>
        /// Returns the status of the order as a string for display purposes.
        /// </summary>
        public virtual string DisplayStatus
        {
            get
            {
                return Status.ToString();
            }
        }

        public virtual bool IsApproveable
        {
            get
            {
                string currentUser = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
                return !this.Approved && !currentUser.Equals(CreatedBy, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        
        public virtual bool IsEditable
        {
            get
            {
                return this.Status == MoneyTransferOrderStati.New;
            }
        }

        public virtual bool IsCancellable
        {
            get
            {
                return this.Status == MoneyTransferOrderStati.New;
            }
        }

        public virtual bool IsSendable
        {
            get
            {
                return this.Status == MoneyTransferOrderStati.New && this.Approved;
            }
        }

        public virtual MoneyTransferOrderStati Status
        {
            get { return status; }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }

        #endregion
        
        #region Methods

        public virtual bool Approve()
        {
            bool success = false;
            if (!Approved)
            {
                string approver = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
                if (approver.Equals(CreatedBy, StringComparison.CurrentCultureIgnoreCase))
                    throw new ApplicationException("The money transfer order can not be approved by the same person who created the order.");

                ApprovedBy = approver;
                ApprovalDate = DateTime.Now;
                Approved = true;
                success = Approved;
            }
            return success;
        }

        public virtual bool UnApprove()
        {
            bool success = false;
            if (Approved)
            {
                if (Status != MoneyTransferOrderStati.New)
                    throw new ApplicationException("The money transfer order can no longer be unapproved.");
                
                ApprovedBy = "";
                ApprovalDate = DateTime.MinValue;
                Approved = false;
                success = !Approved;
            }
            return success;
        }

        public virtual bool Cancel()
        {
            bool success = false;
            if (!Approved)
            {
                if (!IsCancellable)
                    throw new ApplicationException("The money transfer order can no longer be cancelled.");

                SetStatus(MoneyTransferOrderStati.Cancelled);
                success = (Status == MoneyTransferOrderStati.Cancelled);
            }
            return success;
        }

        public virtual Tuple<bool, string> Validate()
        {
            Tuple<bool, string> retVal = new Tuple<bool, string>(true, "");

            // only check for customer accounts
            if (TransfereeAccount != null)
            {
                // check enough Cash
                Money totalCashAmount = TransfereeAccount.TotalPositionAmount(PositionAmountReturnValue.Cash);
                if (totalCashAmount == null || totalCashAmount.IsZero)
                {
                    retVal = new Tuple<bool, string>(false,
                        string.Format("This order ({0}) can not be entered since there is no cash for account {1}.", 
                            Amount.DisplayString,
                            TransfereeAccount.DisplayNumberWithName));
                }
                else if (!((Money)(totalCashAmount - this.Amount)).Sign)
                {
                    retVal = new Tuple<bool, string>(false,
                        string.Format("This order ({0}) exceeds the total cash amount ({1}) of account {2}.", 
                            Amount.DisplayString, 
                            totalCashAmount.DisplayString,
                            TransfereeAccount.DisplayNumberWithName));
                }
            }
            return retVal;
        }

        public virtual void SetStatus(MoneyTransferOrderStati newStatus)
        {
            this.status = newStatus;
        }
        
        public virtual void setReference()
        {
            Reference = "PAEREL" + Key.ToString().PadLeft(10, '0');
        }

        #endregion

        #region Helpers

        private string getAddressLine(Address address, int line)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("");

            if (address != null)
            {
                switch (line)
	            {
		            case 1:
                        int lenHouseNumber = getStringLength(address.HouseNumber) + getStringLength(address.HouseNumberSuffix);
                        if (getStringLength(address.Street) > 0)
                            sb.Append(getMaxLengthString(address.Street, 35 - (lenHouseNumber + 1)));
                        if (getStringLength(address.HouseNumber) > 0)
                            sb.Append(" " + address.HouseNumber);
                        if (getStringLength(address.HouseNumberSuffix) > 0)
                            sb.Append(address.HouseNumberSuffix);
                        break;
                    case 2:
                        int lenPostalCode = getStringLength(address.PostalCode);
                        if (getStringLength(address.City) > 0)
                            sb.Append(getMaxLengthString(address.City, 35 - (lenPostalCode + 1)));
                        if (getStringLength(address.PostalCode) > 0)
                            sb.Append(" " + address.PostalCode);
                        break;
                    case 3:
                        if (address.Country != null)
                            sb.Append(getMaxLengthString(address.Country.InternationalName, 35));
                        break;
	            }
            }
            return sb.ToString();
        }

        private string getDescriptionLine(ICashWithdrawalInstruction instruction, int line)
        {
            return getDescriptionLine(instruction, line, null);
        }

        private string getDescriptionLine(ICashWithdrawalInstruction instruction, int line, ICounterAccount counterAccount)
        {
            string transferDescription = getDescriptionLine(instruction.TransferDescription, line);
            switch (line)
            {
                case 1:
                    return string.Format("{0} onttrekking", instruction.Account.Number);
                case 2:
                    if (!string.IsNullOrEmpty(transferDescription))
                        return transferDescription;
                    else
                        return string.Format("{0} {1} {2}", (instruction.Rule != null ? instruction.Rule.Regularity.Description : "Eenmalig"), instruction.Amount.Underlying.ToCurrency.Symbol, instruction.Amount.Abs().Quantity.ToString());
                case 3:
                    if (!string.IsNullOrEmpty(transferDescription))
                        return transferDescription;
                    else
                        return string.Format("Voor {0}", instruction.Account.ShortName);
                case 4:
                    if (!string.IsNullOrEmpty(transferDescription))
                        return transferDescription;
                    else
                    {
                        if (counterAccount.IsPublic)
                            return "Transfer naar derden";
                        else
                            return "Transfer naar eigen rekening";
                    }
            }
            return "";
        }

        private string getDescriptionLine(IClientDepartureInstruction instruction, int line)
        {
            return getDescriptionLine(instruction, line, null);
        }

        private string getDescriptionLine(IClientDepartureInstruction instruction, int line, ICounterAccount counterAccount)
        {
            string transferDescription = getDescriptionLine(instruction.TransferDescription, line);
            switch (line)
            {
                case 1:
                    return string.Format("{0} eind afrekening", instruction.Account.Number);
                case 2:
                    if (!string.IsNullOrEmpty(transferDescription))
                        return transferDescription;
                    else
                        return "Liquidatie portfolio";
                case 3:
                    if (!string.IsNullOrEmpty(transferDescription))
                        return transferDescription;
                    else
                        return string.Format("Voor {0}", instruction.Account.ShortName);
                case 4:
                    if (!string.IsNullOrEmpty(transferDescription))
                        return transferDescription;
                    else
                    {
                        if (counterAccount.IsPublic)
                            return "Transfer naar derden";
                        else
                            return "Transfer naar eigen rekening";
                    }
            }
            return "";
        }

        private string getDescriptionLine(string transferDescription, int line)
        {
            string description = "";
            switch (line)
            {
                case 2:
                    if (!string.IsNullOrEmpty(transferDescription))
                        description = Util.Substring(transferDescription, 0, 35);
                    break;
                case 3:
                    if (!string.IsNullOrEmpty(transferDescription) && transferDescription.Length > 35)
                        description = Util.Substring(transferDescription, 35, 35);
                    break;
                case 4:
                    if (!string.IsNullOrEmpty(transferDescription) && transferDescription.Length > 70)
                        description = Util.Substring(transferDescription, 70, 35);
                    break;
            }
            return description;
        }

        private string getMaxLengthString(string value, int maxLength)
        {
            if (value == null || value.Length <= maxLength)
                return value;
            else
                return value.Substring(0, maxLength);
        }

        private int getStringLength(string value)
        {
            if (value == null)
                return 0;
            else
                return value.Length;
        }

        #endregion

        #region Privates

        private string reference = null;
        private DateTime creationDate;
        private MoneyTransferOrderStati status = MoneyTransferOrderStati.New;
        private const string err_def = "Could not create money transfer order: ";

        #endregion

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Accounts
{
    public class CounterAccount : ICounterAccount
    {
        #region Constructor

        protected CounterAccount() { }

        public CounterAccount(string number, string accountName, IBank bank, string bankName, Address bankAddress,
            IManagementCompany managementCompany, bool isPublic, Address beneficiaryAddress, bool ByPassValidation)
        {
            this.Number = number;
            this.AccountName = accountName;
            this.Bank = bank;
            this.BankName = bankName;
            this.BankAddress = bankAddress;
            this.ManagementCompany = managementCompany;
            this.IsPublic = isPublic;
            this.BeneficiaryAddress = beneficiaryAddress;

            if (!ByPassValidation && !IsValid)
                throw new ApplicationException("This Counter Account is not valid.");
        }

        public CounterAccount(string number, string accountName, IBank bank, string bankName, Address bankAddress,
                IManagementCompany managementCompany, bool isPublic, Address beneficiaryAddress)
            : this(number, accountName, bank, bankName, bankAddress, managementCompany, isPublic, beneficiaryAddress, false)
        {

        }


        #endregion

        #region Props

        public virtual int Key { get; set; }
        public virtual string Number { get; set; }
        public virtual string AccountName { get; set; }
        public virtual IBank Bank { get; set; }
        public virtual string BankName 
        {
            get
            {
                if (Bank != null)
                    return Bank.Name;
                else
                    return bankName;
            }
            set { bankName = value;  }
        }
        
        public virtual Address BankAddress 
        {
            get
            {
                if (bankAddress != null && !bankAddress.IsEmpty)
                    return bankAddress;
                else
                {
                    if (Bank != null)
                        return Bank.Address;
                    else
                        return bankAddress;
                }
            }
            set { bankAddress = value; }
        }

        public virtual IManagementCompany ManagementCompany { get; set; }
        public virtual bool IsPublic { get; set; }
        public Address BeneficiaryAddress { get; set; }
        private IList bagOfContacts;
        public IList Contacts
        {
            get { return bagOfContacts; }
        }

        public virtual string DisplayName
        {
            get { return string.Format("{0}{1} {2}", (BankName != null && BankName.Length > 0 ? BankName + " " : ""), Number, AccountName).Trim(); }
        }

        public virtual string DisplayNameShort
        {
            get { return string.Format("{0} {1}", Number, AccountName).Trim(); }
        }

        public virtual bool IsValid
        {
            get
            {
                bool retVal = false;
                if (Number != null && Number.Length > 0 &&
                    AccountName != null && AccountName.Length > 0)
                    retVal = true;
                return retVal;
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return DisplayName;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ICounterAccount))
            {
                return false;
            }
            return this.Key == ((ICounterAccount)obj).Key;
        }

        #endregion

        #region Privates

        private string bankName;
        private Address bankAddress;

        #endregion
    }
}

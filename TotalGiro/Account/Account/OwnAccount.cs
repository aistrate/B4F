using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Accounts
{
    public abstract class OwnAccount : AccountTypeCustomer, IOwnAccount, ICounterPartyAccount
    {
        public OwnAccount() { }

        public OwnAccount(string number, string shortName, IManagementCompany accountOwner)
            : base(number, shortName, accountOwner, null)
        {
        }

        public override bool IsCompanyAccount
        {
            get { return true; }
        }

        /// <summary>
        /// This is the Company that holds the internal account.
        /// Customer accounts do not belong to a company, system and nostro accounts belong either to an Asset managing company or the stichting.
        /// </summary>
        public virtual IManagementCompany Company
        {
            get { return company; }
        }

        #region ICounterPartyAccount Members

        B4F.TotalGiro.Instruments.IExchange ICounterPartyAccount.DefaultExchange
        {
            get
            {
                return null;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion


        #region Private Variables

        private IManagementCompany company = null;

        #endregion
    }
}

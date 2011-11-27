using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This is an abstract class and a subclass of the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeInternal">AccountTypeInternal</see> class.
    /// It serves as a base class for accounts that are used in the TotalGiro system with a specific system function, like the overflow and the commission account.
    /// </summary>
    abstract public class AccountTypeSystem : AccountTypeInternal, IAccountTypeSystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeSystem">AccountTypeSystem</see> class.
        /// </summary>
        public AccountTypeSystem() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeSystem">AccountTypeSystem</see> class.
        /// </summary>
        /// <param name="number">The Account's number</param>
        /// <param name="shortName">Shortname of the account</param>
        /// <param name="accountOwner">The owner of the account</param>
        public AccountTypeSystem(string number, string shortName, IManagementCompany accountOwner)
            : base(number, shortName, accountOwner)
        {
        }

        /// <summary>
        /// This is the Company that holds the internal account.
        /// Customer accounts do not belong to a company, system and nostro accounts belong either to an Asset managing company or the stichting.
        /// </summary>
        public abstract IManagementCompany Company { get; }

        public override bool IsCompanyAccount
        {
            get { return true; }
        }

        #region Private Variables

        protected IManagementCompany company = null;

        #endregion
    }
}

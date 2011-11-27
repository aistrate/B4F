using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This is an abstract class and a subclass of the <see cref="T:B4F.TotalGiro.Accounts.Account">Account</see> class.
    /// It serves as a base class for accounts that are external to the TotalGiro system, like counterparties and custody accounts.
    /// </summary>
    public abstract class AccountTypeExternal : Account, IAccountTypeExternal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeExternal">AccountTypeExternal</see> class.
        /// </summary>
        protected AccountTypeExternal() { }

        /// <summary>
        /// This is the base currency of the external account
        /// </summary>
        public override ICurrency BaseCurrency
        {
            get { return baseCurrency; }
            set { baseCurrency = value; }
        }

        #region Private Variables

        private ICurrency baseCurrency;

        #endregion

    }
}

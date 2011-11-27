using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Accounts
{
    public class VirtualFundTradingAccount : AccountTypeCustomer, IVirtualFundTradingAccount, ICounterPartyAccount
    {
        /// <summary>
        /// The AccountType defines the type of account. For example customer or counterparty.
        /// </summary>
        public override AccountTypes AccountType
        {
            get { return AccountTypes.VirtualFundTradingAccount; }
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
    }
}

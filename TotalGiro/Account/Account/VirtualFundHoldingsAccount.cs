using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Accounts
{
    public class VirtualFundHoldingsAccount : AccountTypeCustomer, IVirtualFundHoldingsAccount
    {

        /// <summary>
        /// The AccountType defines the type of account. For example customer or counterparty.
        /// </summary>
        public override AccountTypes AccountType
        {
            get { return AccountTypes.VirtualFundHoldingsAccount; }
        }
    }
}

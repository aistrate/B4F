using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This class is used to instantiate account instances that are used for storing the overflow of orders.
    /// For instance the shortage or surplus that is left after the transaction allocation will go to this account.
    /// But also in the case when the instrument does not support fractions when trading, the number of positions to round up the order will also be placed on the overflow account.
    /// It is a system account of the TotalGiro system.
    /// </summary>
    public class CrumbleAccount : OwnAccount, ICrumbleAccount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.OverFlowAccount">OverFlowAccount</see> class.
        /// </summary>
        public CrumbleAccount() { }

        /// <summary>
        /// The AccountType defines the type of account.
        /// </summary>
        public override AccountTypes AccountType
        {
            get { return AccountTypes.Crumble; }
        }

        private ITradingAccount specialTradingAccount;

        public override ITradingAccount AccountforAggregation
        {
            get { return specialTradingAccount; }
        }
	
    }
}

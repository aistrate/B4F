using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using System.Text;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This class is used to instantiate counterparty account instances that are used to trade with.
    /// They serve as the counterparty on the <see cref="T:B4F.TotalGiro.Orders.OldTransactions.ObsoleteTransaction">transactions</see> objects.
    /// It is an external account of the TotalGiro system.
    /// </summary>
    public class CounterPartyAccount : AccountTypeExternal, ICounterPartyAccount
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.CounterPartyAccount">CounterPartyAccount</see> class.
        /// </summary>
        protected CounterPartyAccount() { }

        /// <summary>
        /// The AccountType defines the type of account.
        /// </summary>
        public override AccountTypes AccountType
        {
            get { return AccountTypes.Counterparty; }
        }
	
        /// <summary>
        /// The default exchange where the counterparty is trading.
        /// </summary>
		public IExchange DefaultExchange
		{
			get { return defaultExchange; }
			set { defaultExchange = value; }
		}        

		#region Private Variables

		private IExchange defaultExchange;
        //private ICurrency baseCurrency;

		#endregion

	}
}

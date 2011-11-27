using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This account is a custodian (keeps custody of the positions).
    /// It is an external account of the TotalGiro system.
    /// </summary>
    public class CustodyAccount : AccountTypeExternal, ICustodyAccount
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.CustodyAccount">CustodyAccount</see> class.
        /// </summary>
        protected CustodyAccount() { }

        /// <summary>
        /// The AccountType defines the type of account.
        /// </summary>
        public override AccountTypes AccountType
        {
            get { return AccountTypes.Custody; }
        }

        /// <summary>
        /// The name of the Custodian account
        /// </summary>
        public virtual string CustodianName
		{
			get { return custodianName; }
			set { custodianName = value; }
		}        

		#region Private Variables

		private string custodianName;

		#endregion

	}
}

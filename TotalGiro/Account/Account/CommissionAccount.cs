using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This class is used to instantiate account instances that are used for storing the commission.
    /// It is a system account of the TotalGiro system.
    /// </summary>
    public class CommissionAccount : AccountTypeSystem, ICommissionAccount
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.CommissionAccount">CommissionAccount</see> class.
        /// </summary>
        protected CommissionAccount() { }


        /// <summary>
        /// The AccountType defines the type of account.
        /// </summary>
        public override AccountTypes AccountType
        {
            get { return AccountTypes.Commission; }
        }

        /// <summary>
        /// This is the Company that holds the internal account.
        /// Customer accounts do not belong to a company, system and nostro accounts belong either to an Asset managing company or the stichting.
        /// </summary>
        public override IManagementCompany Company
        {
            get { return company; }
        }
    }
}

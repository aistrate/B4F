using System;
using System.Collections.Generic;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This class is used to instantiate account instances that are used for trading.
    /// This account is stored on the <see cref="T:B4F.TotalGiro.Orders.IStgOrder">actual orders</see> that are used for trading 
    /// It is a system account of the TotalGiro system.
    /// </summary>
    public class TradingAccount : AccountTypeSystem, ITradingAccount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.TradingAccount">TradingAccount</see> class.
        /// </summary>
        public TradingAccount()  { }

        /// <summary>
        /// The AccountType defines the type of account.
        /// </summary>
        /// 
        public TradingAccount(string number, string shortName, IEffectenGiro accountOwner)
            : base(number, shortName, accountOwner)
        {
        }
        public override AccountTypes AccountType
        {
            get { return AccountTypes.Trading; }
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

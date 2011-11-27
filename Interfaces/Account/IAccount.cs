using System;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
[assembly: CLSCompliant(true)]

namespace B4F.TotalGiro.Accounts
{
    #region enums

    /// <summary>
    /// This enumerations lists all the distinct account roles
    /// </summary>
    public enum AccountTypes
    {
        /////// <summary>
        /////// A Remisier (intermediatery) account
        /////// </summary>
        ////Remisier = -13,
        ///// <summary>
        ///// The commission account, that gets all the commission
        ///// </summary>
        //Commission = -12,
        ///// <summary>
        ///// The overflow (crumble) account. See the <see cref="T:B4F.TotalGiro.Accounts.OverFlowAccount">OverFlowAccount</see> class
        ///// </summary>
        //Crumble = -11,
        ////InHouseFundTrading = -10,
        ///// <summary>
        ///// The account that keeps custody of the positions
        ///// </summary>
        //Custodian = -9,
        ////ProfTrader = -8,
        //Transfer = -7,
        ////Model = -6,
        ///// <summary>
        ///// The nostro account
        ///// </summary>
        //Nostro = -5,
        ///// <summary>
        ///// The trading account
        ///// </summary>
        //Trader = -4,
        ///// <summary>
        ///// The counterparty account
        ///// </summary>
        //Counterparty = -3,

        ///// <summary>
        ///// The customer account
        ///// </summary>
        Customer = 15,
        Crumble = 25,
        Nostro = 30,
        Trading = 45,
        Commission = 50,
        Transfer = 55,
        Counterparty = 75,
        Custody = 80,
        VirtualFundHoldingsAccount = 85,
        VirtualFundTradingAccount = 90
    }


    /// <summary>
    /// This enumerations describes the possible ways to store positions in the TotalGiro system for an account
    /// </summary>
    public enum StorePositionsLevel
    {
        /// <summary>
        /// Do not store any positions at all (external parties).
        /// </summary>
        Not = 0,
        /// <summary>
        /// Store the positions but disregard the chronological order of the transactions
        /// </summary>
        NotChronological = 1,
        /// <summary>
        /// Store the positions with the chronological order of the transactions in mind
        /// </summary>
        Chronological = 2
    }

    public enum AccountStati
    {
        Active = 1,
        Inactive = 2
    }

    #endregion

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.Account">Account</see> class
    /// </summary>
    public interface IAccount : ITotalGiroBase<IAccount>, IAuditable
	{
        /// <summary>
        /// See <see cref="P:B4F.TotalGiro.Accounts.Account.AccountType">AccountType</see> for a description
        /// </summary>
        AccountTypes AccountType { get; }
        bool IsCompanyAccount { get; }
        /// <summary>
        /// See <see cref="P:B4F.TotalGiro.Accounts.Account.BaseCurrency">BaseCurrency</see> for a description
        /// </summary>
        ICurrency BaseCurrency { get; set;}
        /// <summary>
        /// See <see cref="P:B4F.TotalGiro.Accounts.Account.Key">Key</see> for a description
        /// </summary>
        int Key { get; }
        /// <summary>
        /// See <see cref="P:B4F.TotalGiro.Accounts.Account.Number">Number</see> for a description
        /// </summary>
        string Number { get; set; }
        /// <summary>
        /// See <see cref="P:B4F.TotalGiro.Accounts.Account.ShortName">ShortName</see> for a description
        /// </summary>
        string ShortName { get; set;}
        /// <summary>
        /// See <see cref="P:B4F.TotalGiro.Accounts.Account.DisplayNumberWithName">DisplayNumberWithName</see> for a description
        /// </summary>
        /// 
        string FullName { get; set; }

        string DisplayNumberWithName { get; }
        /// <summary>
        /// See <see cref="P:B4F.TotalGiro.Accounts.Account.IsInternal">IsInternal</see> for a description
        /// </summary>
        bool IsInternal { get; }
        /// <summary>
        /// See <see cref="P:B4F.TotalGiro.Accounts.Account.IsActive">IsActive</see> for a description
        /// </summary>
        //bool IsActive { get; }
        /// <exclude/>
        StorePositionsLevel StorePositions { get; }
        /// <exclude/>
        DateTime CreationDate { get; set; }
        /// <exclude/>
        DateTime LastUpdated { get; }

        ICounterAccount CounterAccount { get; set; }
        AccountStati Status { get; set; }
        DateTime LastDateStatusChanged { get; set; }
        bool NeedsAttention { get; set; }
        bool IsAccountTypeCustomer { get; }

        /// <exclude/>
        /// <exclude/>
    }
}

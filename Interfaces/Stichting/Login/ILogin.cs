using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Stichting.Login
{
    /// <summary>
    /// An enumeration that lists the options that are used with retrieving account data but keeping the security in mind.
    /// If the person who logged on is a stichting employee, he has more rights than for instance a Asset Manager.
    /// And the Asset Manager can of course see more than a customer.
    /// </summary>
    public enum SecurityInfoOptions
    {
        /// <summary>
        /// Data is retrieved both from the managed accounts and the the trading accounts.
        /// </summary>
        Both,
        /// <summary>
        /// Data is retrieved only from the managed accounts.
        /// In the case of a stichting employee, the managed accounts are the asset managers.
        /// In the case of a asset manager, the managed accounts are the customers.
        /// </summary>
        ManagedsAcctsOnly,
        /// <summary>
        /// Data is retrieved only from the trading accounts.
        /// In the case of a stichting employee, he can view the stichting's trading account data
        /// In the case of a asset manager, he can view the asset manager's trading account data
        /// </summary>
        TradingAcctOnly,
        /// <summary>
        /// This applies to the stichting only.
        /// It is possible to see all data.
        /// </summary>
        NoFilter
    }

    [Flags]
    public enum LoginTypes
    {
        InternalEmployee = 0x01,        // 00000001 (StichtingEmployee OR AssetManagerEmployee OR ComplianceEmployee)
        StichtingEmployee = 0x03,       // 00000011
        AssetManagerEmployee = 0x05,    // 00000101
        ComplianceEmployee = 0x09,      // 00001001
        RemisierEmployee = 0x10,        // 00010000
        Customer = 0x20                 // 00100000
    }

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Stichting.Login.Login">Login</see> class.
    /// </summary>
    public interface ILogin
    {
        int Key { get; set; }
        string UserName { get; set; }
        bool IsActive { get; set; }
        bool IsEmployee { get; }
        LoginTypes LoginType { get; }
        DateTime CreationDate { get; }
        DateTime LastUpdated { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeExternal">AccountTypeExternal</see> class
    /// </summary>
    public interface IAccountTypeExternal : IAccount
    {
        new ICurrency BaseCurrency { get; }
    }
}

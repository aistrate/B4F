using System;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.CustodyAccount">CustodyAccount</see> class
    /// </summary>
    public interface ICustodyAccount : IAccountTypeExternal
	{
		string CustodianName { get; set; }
	}
}

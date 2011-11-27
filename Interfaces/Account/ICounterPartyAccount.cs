using System;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.CounterPartyAccount">CounterPartyAccount</see> class
    /// </summary>
    public interface ICounterPartyAccount : IAccountTypeExternal
	{
		B4F.TotalGiro.Instruments.IExchange DefaultExchange { get; set; }
	}
}

using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.MonetaryOrder">MonetaryOrder</see> class
    /// </summary>
    public interface IMonetaryOrder : IOrder
	{
        ICurrency RequestedCurrency { get; }
        IOrder MoneyParent { get; }
        string DisplayParent { get; }
        bool Cancel();
	}
}

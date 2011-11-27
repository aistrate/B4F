using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.OrderAmountBased">OrderAmountBased</see> class
    /// </summary>
    public interface IOrderAmountBased : ISecurityOrder
	{
        bool IsValueInclComm { get; }
        Money ClientAmount { get; }
	}
}

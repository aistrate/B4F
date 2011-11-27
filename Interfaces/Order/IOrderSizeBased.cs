using System;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.OrderSizeBased">OrderSizeBased</see> class
    /// </summary>
    public interface IOrderSizeBased : ISecurityOrder
	{
        bool IsClosure { get; }
    }
}

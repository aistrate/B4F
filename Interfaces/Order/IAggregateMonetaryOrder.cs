using System;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.AggregateMonetaryOrder">AggregateMonetaryOrder</see> class
    /// </summary>
    public interface IAggregateMonetaryOrder : IAggregatedOrder, IMonetaryOrder
	{
	}
}

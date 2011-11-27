using System;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.AggregateSizeOrder">AggregateSizeOrder</see> class
    /// </summary>
    public interface IAggregateSizeOrder : IOrderSizeBased, IAggregatedOrder
	{
		//bool Place();
		//bool Send();
		//B4F.TotalGiro.Instruments.InstrumentSize TotalOpenValue();
	}
}

using System;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.AggregateAmtOrder">AggregateAmtOrder</see> class
    /// </summary>
    public interface IAggregateAmtOrder : IAggregatedOrder, IOrderAmountBased
	{
		//bool Place();
		//bool Send();
		//B4F.TotalGiro.Instruments.InstrumentSize TotalOpenValue();
	}
}

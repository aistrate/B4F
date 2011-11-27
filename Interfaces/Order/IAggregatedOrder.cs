using System;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Routes;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for all the aggregated orders
    /// </summary>
    public interface IAggregatedOrder : IOrder
	{
        /// <summary>
        /// Returns the total value of the order that has not been filled.
        /// </summary>
        /// <returns>The total value of the order that has not been filled.</returns>
        InstrumentSize TotalOpenValue();
	}
}

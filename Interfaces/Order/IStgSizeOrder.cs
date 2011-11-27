using System;
using System.Collections.Generic;
using B4F.TotalGiro.Routes;
using System.Text;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.StgSizeOrder">StgSizeOrder</see> class
    /// </summary>
    public interface IStgSizeOrder : IAggregateSizeOrder, IStgOrder
    {
        bool SetNumberOfDecimals(short decimals);
        void ResetPlacedValue();
    }
}

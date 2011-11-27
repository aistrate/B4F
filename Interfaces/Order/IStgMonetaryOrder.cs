using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.StgMonetaryOrder">StgMonetaryOrder</see> class
    /// </summary>
    public interface IStgMonetaryOrder : IAggregateMonetaryOrder, IStgOrder
    {
    }
}

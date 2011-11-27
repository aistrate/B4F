using System;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.OrderRouteMapper
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.OrderRouteMapper.OrderRouteMapper">OrderRouteMapper</see> class
    /// </summary>
    public interface IOrderRouteMapper
	{
		IRoute GetRoute(IOrder order);
	}
}

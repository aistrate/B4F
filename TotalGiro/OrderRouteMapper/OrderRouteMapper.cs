using System;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.OrderRouteMapper
{
    /// <summary>
    /// The OrderRouteMapper class is used to retrieve the route for the order.
    /// The class implements the singleton design pattern
    /// </summary>
    public class OrderRouteMapper : IOrderRouteMapper
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.OrderRouteMapper.OrderRouteMapper">OrderRouteMapper</see> class.
        /// </summary>
        internal OrderRouteMapper() { }

		/// <summary>
        /// This method returns a (singleton) instance of the <see cref="T:B4F.TotalGiro.OrderRouteMapper.OrderRouteMapper">OrderRouteMapper</see> class
		/// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <returns>An instance of the <see cref="T:B4F.TotalGiro.OrderRouteMapper.OrderRouteMapper">OrderRouteMapper</see> class</returns>
        public static OrderRouteMapper GetInstance(IDalSession session)
		{
			// Initialise the class
			mapper.initialise(session);
			return mapper;
		}

		/// <summary>
		/// This method sets the route on an order
		/// </summary>
		/// <param name="order">The specific order</param>
		/// <returns>The new route for the order</returns>
        public IRoute GetRoute(IOrder order)
		{
			IRoute route = null;

			if (order == null)
				throw new ApplicationException("Order can not be null when retrieving the route");

			if (order.IsAggregateOrder)
			{
				if (!order.IsMonetary) // Security Order
				{
					IInstrument instrument = ((ISecurityOrder)order).TradedInstrument;
					if (instrument == null)
						throw new ApplicationException("Traded instrument on the Order can not be null when retrieving the route");
					
					// Get the route from the instrument
                    if (instrument.IsTradeable)
					    route = ((ITradeableInstrument)instrument).DefaultRoute;

					if (route == null)
					{
						// Get the default route
						route = mapper.getDefaultRoute();
					}
				}
				else
				{
					// order is monetary order
					// send the order to money desk
					route = mapper.getSpecificRoute(RouteTypes.MoneyDesk);
				}
			}
			
			return route;
		}

		private void initialise(IDalSession session)
		{
            if (routes == null)
            {
                routes = RouteMapper.GetRoutes(session);
            }
			if (routes == null || routes.Count == 0)
			{
				throw new ApplicationException("No routes have been set up in the system");
			}
		}

		private IRoute getDefaultRoute()
		{
			foreach (IRoute route in routes)
			{
				if (route.IsDefault)
				{
					return route;
				}
			}
			throw new ApplicationException("No default Route has been set up in the system");
		}

        private IRoute getSpecificRoute(RouteTypes type)
		{
			foreach (IRoute route in routes)
			{
                if (route.Type == type)
				{
					return route;
				}
			}
			throw new ApplicationException(string.Format("Could not find the route of type {0}", type.ToString()));
		}

        #region PrivateVariables

        private static OrderRouteMapper mapper = new OrderRouteMapper();
		private static IList routes;

        #endregion
    }
}

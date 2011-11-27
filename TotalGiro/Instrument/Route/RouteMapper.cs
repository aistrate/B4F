using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Routes
{
    /// This class is used to instantiate Route objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    public class RouteMapper
    {
        /// <summary>
        /// Get route by ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="RouteID">Identifier</param>
        /// <returns></returns>
        public static IRoute GetRoute(IDalSession session, int RouteID)
        {
            IRoute route = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", RouteID));
            IList list = session.GetList(typeof(Route), expressions);
            if (list != null && list.Count > 0)
            {
                route = (IRoute)list[0];
            }
            return route;
            //return (Route)session.GetObjectInstance(typeof(Route), RouteID);
        }

        /// <summary>
        /// Get all system routes
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <returns>Collection of routes</returns>
        public static IList GetRoutes(IDalSession session)
        {
            return session.GetList(typeof(Route));
        }

        /// <summary>
        /// Get system default route
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <returns>route object</returns>
        public static IRoute GetDefaultRoute(IDalSession session)
        {
            IRoute route = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("IsDefault", true));
            IList list = session.GetList(typeof(Route), expressions);
            if (list != null && list.Count > 0)
            {
                route = (IRoute)list[0];
            }
            return route;
        }

        /// <summary>
        /// Returns a route depending on its type
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="type">The route type of interest</param>
        /// <returns>route object</returns>
        public static IRoute GetRouteByType(IDalSession session, RouteTypes type)
        {
            IRoute route = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Type", type));
            IList list = session.GetList(typeof(Route), expressions);
            if (list != null && list.Count > 0)
            {
                route = (IRoute)list[0];
            }
            return route;
        }

        /// <summary>
        /// Returns a route depending on its link with a specific exchange
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="exchange">The exchange of interest</param>
        /// <returns>route object</returns>
        public static IRoute GetRouteByExchange(IDalSession session, IExchange exchange)
        {
            IRoute route = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Exchange.Key", exchange.Key));
            IList list = session.GetList(typeof(Route), expressions);
            if (list != null && list.Count > 0)
            {
                route = (IRoute)list[0];
            }
            return route;
        }

    }
}

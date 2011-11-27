using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Enumeration that specifies which type of orders you want.
    /// </summary>
    public enum OrderReturnClass
    {
        /// <summary>
        /// Security Order
        /// </summary>
        SecurityOrder = 12,
        /// <summary>
        /// Amount based order
        /// </summary>
        AmountBasedOrder = 1,
        /// <summary>
        /// Size based order
        /// </summary>
        SizeBasedOrder = 2,
        /// <summary>
        /// Monetary order
        /// </summary>
        MonetaryOrder = 3
    }

    /// <summary>
    /// Enumeration used for identifying amount/size based or all orders. Used for filtering orders.
    /// </summary>
    public enum MonetaryOrderReturnClass
    {
        /// <summary>
        /// Amount and size based orders
        /// </summary>
        All,
        /// <summary>
        /// Amount based orders
        /// </summar
        AmountBased,
        /// <summary>
        /// Size based orders
        /// </summary>
        SizeBased
    }

    /// <summary>
    /// Enumeration used for identifying the aggregation level of the orders. Used for filtering orders.
    /// </summary>
    [Flags]
    public enum OrderAggregationLevel : int
    {
        /// <summary>
        /// All orders
        /// </summary>
        All = 0,
        /// <summary>
        /// The order is not aggregated
        /// </summary>
        None = 1,
        /// <summary>
        /// The order is aggregated on assetmanager level
        /// </summary>
        AssetManager = 2,
        /// <summary>
        /// The order is aggregated on stichting level
        /// </summary>
        Stichting = 4,
        /// <summary>
        /// The order is aggregated and netted on stichting level
        /// </summary>
        StichtingNetted = 8
    }

    /// <summary>
    /// Enumeration that specifies the approval state of an order.
    /// </summary>
    public enum ApprovalState
    {
        /// <summary>
        /// All orders
        /// </summary>
        All,
        /// <summary>
        /// Orders that have been approved
        /// </summary>
        Approved,
        /// <summary>
        /// Orders that have not been approved
        /// </summary>
        UnApproved
    }

    /// <summary>
    /// Enumeration to indicate the state of the parent of the order
    /// </summary>
    public enum ParentalState
    {
        /// <summary>
        /// All orders
        /// </summary>
        All,
        /// <summary>
        /// Orders that have a parent order
        /// </summary>
        Null,
        /// <summary>
        /// Orders that do not have a parent order
        /// </summary>
        NotNull
    }

    /// <summary>
    /// Enumeration used for identifying the active or closed state of the orders. Used for filtering orders.
    /// </summary>
    public enum ActiveClosedState
    {
        /// <summary>
        /// Return all orders independent whether their either active or closed
        /// </summary>
        All,
        /// <summary>
        /// Only return active orders
        /// </summary>
        Active,
        /// <summary>
        /// Only return closed orders
        /// </summary>
        Closed
    }

    #region Status Filter Class
    
    /// <summary>
    /// Enumeration for order status filter options
    /// </summary>
    public enum OrderStatusFilterOptions
	{
        /// <summary>
        /// No filter
        /// </summary>
	    None,
        /// <summary>
        /// Exclude Closed Stati
        /// </summary>
        NoClosedStati,
        /// <summary>
        /// Include Closed Stati
        /// </summary>
        IncludeClosedStatiToday
	}

    /// <summary>
    /// Class to manage the status filter for order queries.
    /// </summary>
    public class OrderStatusFilter
    {
        /// <summary>
        /// Set order status filter by passing an array of OrderStati objects.
        /// </summary>
        /// <param name="orderStati">Array of OrderStati objects</param>
        public OrderStatusFilter(params OrderStati[] orderStati)
        {
            this.orderStati = orderStati;
        }

        /// <summary>
        /// Set order status filter by passing an OrderStatusFilterOptions
        /// </summary>
        /// <param name="filterOption">filter option</param>
        public OrderStatusFilter(OrderStatusFilterOptions filterOption)
        {
            this.option = filterOption;
        }

        /// <summary>
        /// Gets/sets order stati
        /// </summary>
        public OrderStati[] Stati
        {
            get { return orderStati; }
            set { orderStati = value; }
        }

        /// <summary>
        /// Gets/sets filter option
        /// </summary>
        public OrderStatusFilterOptions FilterOption
        {
            get { return option; }
            set { option = value; }
        }

        internal void GetExpressions(List<ICriterion> expressions)
        {
            ICriterion crit = null;
            ICriterion critTmp = null;

            if (Stati != null && Stati.Length > 0)
                crit = Expression.In("Status", Stati);

            if (FilterOption != OrderStatusFilterOptions.None)
            {
                // In both cases all open stati
                critTmp = Expression.Sql(getOrderStatusSQL(true));
                if (FilterOption == OrderStatusFilterOptions.IncludeClosedStatiToday)
                {
                    critTmp = Expression.Or(critTmp, Expression.And(Expression.Sql(getOrderStatusSQL(false)), Expression.Ge("dateClosed", DateTime.Now.Date)));
                }
            }

            if (crit != null && critTmp != null)
                crit = Expression.And(crit, critTmp);
            else if (crit == null && critTmp != null)
                crit = critTmp;

            if (crit != null)
            {
                if (expressions == null)
                    expressions = new List<ICriterion>();
                expressions.Add(crit);
            }
        }

        private string getOrderStatusSQL(bool isOpen)
        {
            return string.Format("this_.OrderStatusID IN (SELECT OrderStatusID FROM OrderStatus WHERE (IsOpen = {0}))", (isOpen ? 1 : 0));
        }

        #region Privates

        OrderStati[] orderStati;
        private OrderStatusFilterOptions option = OrderStatusFilterOptions.None;

        #endregion

    }

    #endregion

    /// <summary>
    /// This class is used to instantiate Order objects. 
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class OrderMapper
    {
        /// <summary>
        /// Returns an Order object through the order id (key)
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="orderID">Order Id</param>
        /// <returns>Order object</returns>      
        public static IOrder GetOrder(IDalSession session, Int32 orderID)
        {
            return GetOrder(session, orderID, SecurityInfoOptions.Both);
        }

        /// <summary>
        /// Returns an Order object through the order id (key) and checks for security info. The
        /// order list is filtered by the company and roles the user belongs to
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="orderID">Order Id</param>
        /// <param name="option">Security info</param>
        /// <returns>A list of orders</returns>
        public static IOrder GetOrder(IDalSession session, Int32 orderID, SecurityInfoOptions option)
        {
            IOrder order = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", orderID));
            LoginMapper.GetSecurityInfo(session, expressions, option);
            IList<IOrder> list = session.GetTypedList<Order, IOrder>(expressions);
            if (list != null && list.Count == 1)
                order = list[0];
            return order;
        }

        ///// <summary>
        ///// Returns the security orders
        ///// </summary>
        ///// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        ///// <returns>A list of security orders</returns>
        //public static IList GetSecurityOrders(IDalSession session)
        //{
        //    List<ICriterion> expressions = new List<ICriterion>();
        //    LoginMapper.GetSecurityInfo(session, expressions);
        //    return session.GetList(typeof(SecurityOrder), expressions);
        //}

        ///// <summary>
        ///// Returns the monetary orders
        ///// </summary>
        ///// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        ///// <returns>A list of monetary orders</returns>
        //public static IList GetMonetaryOrders(IDalSession session)
        //{
        //    List<ICriterion> expressions = new List<ICriterion>();
        //    LoginMapper.GetSecurityInfo(session, expressions);
        //    return session.GetList(typeof(MonetaryOrder), expressions);
        //}

        ///// <summary>
        ///// Returns monetary orders, based on a variety of criteria
        ///// </summary>
        ///// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        ///// <param name="retClass">Class describing if it is amount based or size based.</param>
        ///// <param name="aggLevel">Aggregation level</param>
        ///// <param name="state">Approval state</param>
        ///// <param name="option">Security info option</param>
        ///// <param name="familyState">State of the parent</param>
        ///// <returns>List of monetary orders that comply to the filter parameters</returns>
        //public static IList GetMonetaryOrders(IDalSession session, MonetaryOrderReturnClass retClass, OrderAggregationLevel aggLevel, 
        //    ApprovalState state, SecurityInfoOptions option, ParentalState familyState)
        //{
        //    return GetMonetaryOrders(session, retClass, aggLevel, state, option, familyState, null, null, 0, null, null);
        //}

        ///// <summary>
        ///// Returns monetary orders, based on a variety of criteria
        ///// </summary>
        ///// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        ///// <param name="retClass">Class describing if it is amount based or size based.</param>
        ///// <param name="aggLevel">Aggregation level</param>
        ///// <param name="state">Approval state</param>
        ///// <param name="option">Security info option</param>
        ///// <param name="familyState">State of the parent</param>
        ///// <param name="orderStatusFilter">Order status</param>
        ///// <returns>List of monetary orders that comply to the filter parameters</returns>
        //public static IList GetMonetaryOrders(IDalSession session, MonetaryOrderReturnClass retClass, OrderAggregationLevel aggLevel,
        //    ApprovalState state, SecurityInfoOptions option, ParentalState familyState, OrderStatusFilter orderStatusFilter)
        //{
        //    return GetMonetaryOrders(session, retClass, aggLevel, state, option, familyState, null, orderStatusFilter, 0, null, null);
        //}

        ///// <summary>
        ///// Returns monetary orders, based on a variety of criteria
        ///// </summary>
        ///// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        ///// <param name="retClass">Class describing if it is amount based or size based.</param>
        ///// <param name="aggLevel">Aggregation level</param>
        ///// <param name="state">Approval state</param>
        ///// <param name="option">Security info option</param>
        ///// <param name="familyState">State of the parent</param>
        ///// <param name="expressions">HQL Criterion list</param>
        ///// <returns>List of monetary orders that comply to the filter parameters</returns>
        //public static IList GetMonetaryOrders(IDalSession session, MonetaryOrderReturnClass retClass, OrderAggregationLevel aggLevel, 
        //    ApprovalState state, SecurityInfoOptions option, ParentalState familyState, List<ICriterion> expressions)
        //{
        //    return GetMonetaryOrders(session, retClass, aggLevel, state, option, familyState, expressions, null, 0, null, null);
        //}

        ///// <summary>
        ///// Returns monetary orders, based on a variety of criteria
        ///// </summary>
        ///// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        ///// <param name="retClass">Class describing if it is amount based or size based.</param>
        ///// <param name="aggLevel">Aggregation level</param>
        ///// <param name="state">Approval state</param>
        ///// <param name="option">Security info option</param>
        ///// <param name="familyState">State of the parent</param>
        ///// <param name="expressions">HQL Criterion list</param>
        ///// <param name="orderStatusFilter">Order status filter</param>
        ///// <returns>List of monetary orders that comply to the filter parameters</returns>
        //public static IList GetMonetaryOrders(IDalSession session, MonetaryOrderReturnClass retClass, OrderAggregationLevel aggLevel,
        //    ApprovalState state, SecurityInfoOptions option, ParentalState familyState, List<ICriterion> expressions,
        //    OrderStatusFilter orderStatusFilter)
        //{
        //    return GetMonetaryOrders(session, retClass, aggLevel, state, option, familyState, expressions, orderStatusFilter, 0, null, null);
        //}

        ///// <summary>
        ///// Returns monetary orders, based on a variety of criteria
        ///// </summary>
        ///// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        ///// <param name="retClass">Class describing if it is amount based or size based.</param>
        ///// <param name="aggLevel">Aggregation level</param>
        ///// <param name="state">Approval state</param>
        ///// <param name="option">Security info option</param>
        ///// <param name="familyState">State of the parent</param>
        ///// <param name="orderStatusFilter">Order status filter</param>
        ///// <param name="assetManagerId">Asset manager</param>
        ///// <param name="accountNumber">Account number</param>
        ///// <param name="accountName">Account name</param>
        ///// <returns>List of monetary orders that comply to the filter parameters</returns>
        //public static IList GetMonetaryOrders(IDalSession session, MonetaryOrderReturnClass retClass, OrderAggregationLevel aggLevel,
        //    ApprovalState state, SecurityInfoOptions option, ParentalState familyState, List<ICriterion> expressions, 
        //    OrderStatusFilter orderStatusFilter, int assetManagerId, string accountNumber, string accountName)
        //{
        //    if (expressions == null)
        //        expressions = new List<ICriterion>();

        //    if (retClass != MonetaryOrderReturnClass.All)
        //    {
        //        IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
        //        ICurrency baseCur = company.BaseCurrency;
        //        if (retClass == MonetaryOrderReturnClass.AmountBased)
        //        {
        //            expressions.Add(Expression.Eq("Value.Underlying.Key", baseCur.Key));
        //        }
        //        else if (retClass == MonetaryOrderReturnClass.SizeBased)
        //        {
        //            expressions.Add(Expression.Not(Expression.Eq("Value.Underlying.Key", baseCur.Key)));
        //        }
        //    }
        //    return GetOrders(session, OrderReturnClass.MonetaryOrder, aggLevel, state, option, familyState, orderStatusFilter, null,
        //                     expressions, assetManagerId, accountNumber, accountName, ActiveClosedState.Active);
        //}

        /// <summary>
        /// Returns a list of orders, based on their key
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="orderIDs">List of order id's (Key)</param>
        /// <returns></returns>
        public static IList<IOrder> GetOrders(IDalSession session, int[] orderIDs)
        {
            return GetOrders(session, orderIDs, false);
        }

        public static IList<IOrder> GetPOSUnaggregatedOrders(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            LoginMapper.GetSecurityInfo(session, expressions, SecurityInfoOptions.NoFilter);
            IManagementCompany stichting = ManagementCompanyMapper.GetEffectenGiroCompany(session);
            expressions.Add(Expression.Eq("Account.Key", stichting.StichtingDetails.CrumbleAccount.Key));
            expressions.Add(Expression.Eq("Approved", false));
            getAggregationLevel(expressions, OrderAggregationLevel.None);
            expressions.Add(Expression.IsNull("ParentOrder"));
            getActiveOrdersExpression(expressions, ActiveClosedState.Active);

            return session.GetTypedList<Order, IOrder>(expressions);
        }

        /// <summary>
        /// Returns a list of orders, based on their key, with the option of bypassing security. 
        /// If security is bypassed, all orders will be returned regardless the rights the current user has.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="orderIDs">List of order id's (Key)</param>
        /// <param name="bypassSecurity">Are we bypassing security</param>
        /// <returns>A list or orders</returns>
        public static IList<IOrder> GetOrders(IDalSession session, int[] orderIDs, bool bypassSecurity)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            if (bypassSecurity)
                LoginMapper.GetSecurityInfo(session, expressions, SecurityInfoOptions.NoFilter);
            else
                LoginMapper.GetSecurityInfo(session, expressions);
            expressions.Add(Expression.In("Key", orderIDs));
            return session.GetTypedList<Order, IOrder>(expressions);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">Class describing if it is amount based or size based.</param>
        /// <param name="aggLevel">Aggregation level</param>
        /// <param name="state">Approval state</param>
        /// <param name="option">Security info option</param>
        /// <param name="familyState">State of the parent</param>
        /// <returns>A list of orders</returns>
        public static IList<T> GetOrders<T>(IDalSession session, OrderReturnClass retClass, OrderAggregationLevel aggLevel, ApprovalState state, 
            SecurityInfoOptions option, ParentalState familyState)
            where T : IOrder
        {
            return GetOrders<T>(session, retClass, aggLevel, state, option, familyState, null, null);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">Class describing if it is amount based or size based.</param>
        /// <param name="aggLevel">Aggregation level</param>
        /// <param name="state">Approval state</param>
        /// <param name="option">Security info option</param>
        /// <param name="familyState">State of the parent</param>
        /// <param name="orderStatusFilter">Order status filter</param>
        /// <returns>A list of orders</returns>
        public static IList<T> GetOrders<T>(IDalSession session, OrderReturnClass retClass, OrderAggregationLevel aggLevel, ApprovalState state, 
            SecurityInfoOptions option, ParentalState familyState, OrderStatusFilter orderStatusFilter)
            where T : IOrder
        {
            return GetOrders<T>(session, retClass, aggLevel, state, option, familyState, orderStatusFilter, null, null);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">Class describing if it is amount based or size based.</param>
        /// <param name="aggLevel">Aggregation level</param>
        /// <param name="state">Approval state</param>
        /// <param name="option">Security info option</param>
        /// <param name="familyState">State of the parent</param>
        /// <param name="routes">Route information</param>
        /// <returns>A list of orders</returns>
        public static IList<T> GetOrders<T>(IDalSession session, OrderReturnClass retClass, OrderAggregationLevel aggLevel, ApprovalState state, 
            SecurityInfoOptions option, ParentalState familyState, params int[] routes)
            where T : IOrder
        {
            return GetOrders<T>(session, retClass, aggLevel, state, option, familyState, null, routes, null);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">Class describing if it is amount based or size based.</param>
        /// <param name="aggLevel">Aggregation level</param>
        /// <param name="state">Approval state</param>
        /// <param name="option">Security info option</param>
        /// <param name="familyState">State of the parent</param>
        /// <param name="orderStatusFilter">Order status filter</param>
        /// <param name="routes">Route information</param>
        /// <returns>A list of orders</returns>
        public static IList<T> GetOrders<T>(IDalSession session, OrderReturnClass retClass, OrderAggregationLevel aggLevel, ApprovalState state, 
            SecurityInfoOptions option, ParentalState familyState, OrderStatusFilter orderStatusFilter, params int[] routes)
            where T : IOrder
        {
            return GetOrders<T>(session, retClass, aggLevel, state, option, familyState, orderStatusFilter, routes, null);
        }

        public static IList<T> GetOrders<T>(IDalSession session, OrderReturnClass retClass, OrderAggregationLevel aggLevel, ApprovalState state,
            SecurityInfoOptions option, ParentalState familyState, OrderStatusFilter orderStatusFilter, string isin, string instrumentName, 
            SecCategories secCategoryId, int currencyNominalId, params int[] routes)
            where T : IOrder
        {
            return GetOrders<T>(session, retClass, aggLevel, state, option, familyState, orderStatusFilter, routes, null, 0, null, null, 
                ActiveClosedState.All, isin, instrumentName, secCategoryId, currencyNominalId);
        }

        private static IList<T> GetOrders<T>(IDalSession session, OrderReturnClass retClass, OrderAggregationLevel aggLevel, ApprovalState state, 
            SecurityInfoOptions option, ParentalState familyState, OrderStatusFilter orderStatusFilter, int[] routes, 
            List<ICriterion> expressions)
            where T : IOrder
        {
            return GetOrders<T>(session, retClass, aggLevel, state, option, familyState, orderStatusFilter, routes, expressions, 0, null, null, 
                ActiveClosedState.Active);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">Class describing if it is amount based or size based.</param>
        /// <param name="aggLevel">Aggregation level</param>
        /// <param name="state">Approval state</param>
        /// <param name="option">Security info option</param>
        /// <param name="familyState">State of the parent</param>
        /// <param name="assetManagerId">Asset manager</param>
        /// <param name="accountNumber">Account number</param>
        /// <param name="accountName">Account name</param>
        /// <returns>A list of orders</returns>
        public static IList<T> GetOrders<T>(IDalSession session, OrderReturnClass retClass, OrderAggregationLevel aggLevel, ApprovalState state,
            SecurityInfoOptions option, ParentalState familyState, int assetManagerId, string accountNumber, string accountName)
             where T : IOrder
       {
            return GetOrders<T>(session, retClass, aggLevel, state, option, familyState, null, null, null, assetManagerId, accountNumber, accountName, ActiveClosedState.Active);
        }

        public static IList<T> GetOrders<T>(IDalSession session, OrderReturnClass retClass, OrderAggregationLevel aggLevel, ApprovalState state,
            SecurityInfoOptions option, ParentalState familyState, OrderStatusFilter orderStatusFilter, int[] routes,
            List<ICriterion> expressions, int assetManagerId, string accountNumber, string accountName, ActiveClosedState activeClosedState)
            where T : IOrder
        {
            return GetOrders<T>(session, retClass, aggLevel, state, option, familyState, orderStatusFilter, routes, expressions, assetManagerId,
                accountNumber, accountName, activeClosedState, null, null, 0, 0);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">Class describing if it is amount based or size based.</param>
        /// <param name="aggLevel">Aggregation level</param>
        /// <param name="state">Approval state</param>
        /// <param name="option">Security info option</param>
        /// <param name="familyState">State of the parent</param>
        /// <param name="orderStatusFilter">Order status filter</param>
        /// <param name="routes">Route information</param>
        /// <param name="expressions">HQL Criterion list</param>
        /// <param name="assetManagerId">Asset manager</param>
        /// <param name="accountNumber">Account number</param>
        /// <param name="accountName">Account name</param>
        /// <param name="activeClosedState">Active state of the order</param>
        /// <param name="isin">Isin of the instrument</param>
        /// <param name="instrumentName">Name of the instrument</param>
        /// <param name="secCategoryId">SecCategory of the instrument</param>
        /// <param name="currencyNominalId">Nominal currency of the instrument to be ordered</param>
        /// <returns>A list of orders</returns>
        public static IList<T> GetOrders<T>(IDalSession session, OrderReturnClass retClass, OrderAggregationLevel aggLevel, ApprovalState state,
            SecurityInfoOptions option, ParentalState familyState, OrderStatusFilter orderStatusFilter, int[] routes,
            List<ICriterion> expressions, int assetManagerId, string accountNumber, string accountName, ActiveClosedState activeClosedState,
            string isin, string instrumentName, SecCategories secCategoryId, int currencyNominalId)
            where T : IOrder
        {
            return GetOrders<T>(session, retClass, aggLevel, state, option, familyState, orderStatusFilter, routes, expressions, assetManagerId,
                accountNumber, accountName, activeClosedState, isin, instrumentName, secCategoryId, currencyNominalId, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">Class describing if it is amount based or size based.</param>
        /// <param name="aggLevel">Aggregation level</param>
        /// <param name="state">Approval state</param>
        /// <param name="option">Security info option</param>
        /// <param name="familyState">State of the parent</param>
        /// <param name="orderStatusFilter">Order status filter</param>
        /// <param name="routes">Route information</param>
        /// <param name="expressions">HQL Criterion list</param>
        /// <param name="assetManagerId">Asset manager</param>
        /// <param name="accountNumber">Account number</param>
        /// <param name="accountName">Account name</param>
        /// <param name="activeClosedState">Active state of the order</param>
        /// <param name="isin">Isin of the instrument</param>
        /// <param name="instrumentName">Name of the instrument</param>
        /// <param name="secCategoryId">SecCategory of the instrument</param>
        /// <param name="currencyNominalId">Nominal currency of the instrument to be ordered</param>
        /// <returns>A list of orders</returns>
        public static IList<T> GetOrders<T>(IDalSession session, OrderReturnClass retClass, OrderAggregationLevel aggLevel, ApprovalState state,
            SecurityInfoOptions option, ParentalState familyState, OrderStatusFilter orderStatusFilter, int[] routes,
            List<ICriterion> expressions, int assetManagerId, string accountNumber, string accountName, ActiveClosedState activeClosedState,
            string isin, string instrumentName, SecCategories secCategoryId, int currencyNominalId, DateTime dateFrom, DateTime dateTo)
            where T : IOrder
        {
            if (expressions == null)
                expressions = new List<ICriterion>();

            LoginMapper.GetSecurityInfo(session, expressions, option);
            getAggregationLevel(expressions, aggLevel);
            if (state != ApprovalState.All)
                expressions.Add(Expression.Eq("Approved", (state == ApprovalState.Approved)));
            switch (familyState)
            {
                case ParentalState.NotNull:
                    expressions.Add(Expression.IsNotNull("ParentOrder"));
                    break;
                case ParentalState.Null:
                    expressions.Add(Expression.IsNull("ParentOrder"));
                    break;
            }
            if (orderStatusFilter != null)
                orderStatusFilter.GetExpressions(expressions);
            if (routes != null && routes.Length > 0)
                expressions.Add(Expression.In("Route.Key", routes));

            // filter by Account
            string whereAccount = "";
            if (assetManagerId > 0)
                whereAccount += string.Format(" INNER JOIN VweCustomerAccounts AI ON AI.AccountID = A.AccountID AND AI.ManagementCompanyID = {0}",
                                       assetManagerId);
            if (accountNumber != null && accountNumber.Length > 0)
                whereAccount += string.Format(" {0} A.AccountNumber LIKE '%{1}%'", (whereAccount == "" ? "WHERE" : "AND"), accountNumber);
            if (accountName != null && accountName.Length > 0)
                whereAccount += string.Format(" {0} A.AccountShortName LIKE '%{1}%'", (whereAccount == "" ? "WHERE" : "AND"), accountName);

            if (whereAccount != "")
                expressions.Add(Expression.Sql(string.Format("this_.AccountID IN (SELECT A.AccountID FROM Accounts A {0})", whereAccount)));

            // filter by Instrument
            string joinInstrument = "", whereInstrument = "";
            if ((isin != null && isin.Length > 0) || (currencyNominalId > 0))
                joinInstrument += " INNER JOIN InstrumentsWithPrices IT ON IT.InstrumentID = I.InstrumentID";
            if (isin != null && isin.Length > 0)
                whereInstrument += string.Format(" {0} IT.ISIN LIKE '%{1}%'", (whereInstrument == "" ? "WHERE" : "AND"), isin);
            if (instrumentName != null && instrumentName.Length > 0)
                whereInstrument += string.Format(" {0} I.Name LIKE '%{1}%'", (whereInstrument == "" ? "WHERE" : "AND"), instrumentName);
            if (secCategoryId > 0)
                whereInstrument += string.Format(" {0} I.secCategoryID = {1}", (whereInstrument == "" ? "WHERE" : "AND"), (int)secCategoryId);
            if (currencyNominalId > 0)
                whereInstrument += string.Format(" {0} IT.CurrencyNominalID = {1}", (whereInstrument == "" ? "WHERE" : "AND"), currencyNominalId);
            whereInstrument = joinInstrument + whereInstrument;

            if (whereInstrument != "")
                expressions.Add(Expression.Sql(string.Format("this_.InstrumentID IN (SELECT I.InstrumentID FROM Instruments I {0})", whereInstrument)));
           
            // filter by CreationDate
            if (Util.IsNotNullDate(dateFrom) && Util.IsNotNullDate(dateTo))
                expressions.Add(Expression.Between("CreationDate", dateFrom, dateTo.AddDays(1)));
            if (Util.IsNotNullDate(dateFrom) && Util.IsNullDate(dateTo))
                expressions.Add(Expression.Ge("CreationDate", dateFrom));
            if (Util.IsNullDate(dateFrom) && Util.IsNotNullDate(dateTo))
                expressions.Add(Expression.Le("CreationDate", dateTo));
            
            // only return orders that are still alive
            if (activeClosedState != ActiveClosedState.All)
                getActiveOrdersExpression(expressions, activeClosedState);

            //if (OrderMapper.pagingEnabled && OrderMapper.MaximumRows > 0 && OrderMapper.StartRowIndex >= 0)
            //    return session.GetList(getType(retClass), expressions, OrderMapper.MaximumRows, OrderMapper.StartRowIndex);
            //else
                return NHSession.ToList<T>(session.GetList(getType(retClass), expressions));
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">Class describing if it is amount based or size based.</param>
        /// <param name="account">User account</param>
        /// <returns>A list of orders</returns>
        public static IList GetOrders(IDalSession session, OrderReturnClass retClass, IAccountTypeInternal account)
        {
            return GetOrders(session, retClass, ApprovalState.All, account);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">Class describing if it is amount based or size based.</param>
        /// <param name="state">Approval state</param>
        /// <param name="account">User account</param>
        /// <returns>A list of orders</returns>
        public static IList GetOrders(IDalSession session, OrderReturnClass retClass, ApprovalState state, IAccountTypeInternal account)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            LoginMapper.GetSecurityInfo(session, expressions);
            expressions.Add(Expression.Eq("Account.Key", account.Key));
            if (state != ApprovalState.All)
                expressions.Add(Expression.Eq("Approved", (state == ApprovalState.Approved ? true : false)));
            // only return orders that are still alive
            getActiveOrdersExpression(expressions, ActiveClosedState.Active);
            return session.GetList(getType(retClass), expressions);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="instruction">User account</param>
        /// <returns>A list of orders</returns>
        public static IList<IOrder> GetOrders(IDalSession session, IInstruction instruction)
        {
            return GetOrders(session, instruction, SecurityInfoOptions.Both);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="instruction">User account</param>
        /// <param name="option">Security Option Filter</param>
        /// <returns>A list of orders</returns>
        public static IList<IOrder> GetOrders(IDalSession session, IInstruction instruction, SecurityInfoOptions option)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            LoginMapper.GetSecurityInfo(session, expressions, option);
            expressions.Add(Expression.Eq("Instruction.Key", instruction.Key));
            // return all orders
            getActiveOrdersExpression(expressions, ActiveClosedState.All);
            return session.GetTypedList<Order, IOrder>(expressions);
        }

        /// <summary>
        /// Get orders, based on some filtercriteria
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">Class describing if it is amount based or size based.</param>
        /// <param name="state">Approval state</param>
        /// <param name="accountKeys">Array of account keys</param>
        /// <returns>A list of orders</returns>
        public static IList GetOrders(IDalSession session, OrderReturnClass retClass, ApprovalState state, int[] accountKeys)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            LoginMapper.GetSecurityInfo(session, expressions);
            expressions.Add(Expression.In("Account.Key", accountKeys));
            if (state != ApprovalState.All)
                expressions.Add(Expression.Eq("Approved", (state == ApprovalState.Approved ? true : false)));
            // only return orders that are still alive
            getActiveOrdersExpression(expressions, ActiveClosedState.Active);
            return session.GetList(getType(retClass), expressions);
        }

        /// <summary>
        /// Returns an order's child orders.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="parentOrderID">Order ID (Key) of the parent order</param>
        /// <returns>A list of child orders belonging to the given parent</returns>
        public static IList GetChildOrders(IDalSession session, int parentOrderID)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("ParentOrder.Key", parentOrderID));
            LoginMapper.GetSecurityInfo(session, expressions);
            // only return orders that are still alive
            getActiveOrdersExpression(expressions, ActiveClosedState.Active);
            return session.GetList(typeof(Order), expressions);
        }

        public static IList<int> GetChildFillableOrderKeys(IDalSession session, int topParentOrderID)
        {
            IList<int> list = session.Session.GetNamedQuery(
                "B4F.TotalGiro.Orders.GetChildFillableOrderKeys")
                .SetInt32("orderId", topParentOrderID)
                .List<int>();
            return list;
        }

        public static int GetChildOrderCount(IDalSession session, int topParentOrderID)
        {
            return session.Session.GetNamedQuery(
                "B4F.TotalGiro.Orders.GetChildOrderCount")
                .SetInt32("orderId", topParentOrderID)
                .UniqueResult<int>();
        }

        /// <summary>
        /// Returns an order's child orders with the option of bypassing security. 
        /// If security is bypassed, all orders will be returned regardless the rights the current user has.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="parentOrderID">Order ID (Key) of the parent order</param>
        /// <param name="bypassSecurity">Are we bypassing security?</param>
        /// <returns>A list of child orders belonging to the given parent</returns>
        public static IList GetChildOrders(IDalSession session, int parentOrderID, bool bypassSecurity)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("ParentOrder.Key", parentOrderID));
            if (bypassSecurity)
                LoginMapper.GetSecurityInfo(session, expressions, SecurityInfoOptions.NoFilter);
            else
                LoginMapper.GetSecurityInfo(session, expressions); return session.GetList(typeof(Order), expressions);
        }

        public static int[] GetNotarizableOrderIds(IDalSession session)
        {
            return GetNotarizableOrderIds(session, 0, 0);
        }

        public static int[] GetNotarizableOrderIds(IDalSession session, int managementCompanyId, int accountId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("status", (int)OrderStati.Checked);

            if (managementCompanyId != 0)
                parameters.Add("managementCompanyId", managementCompanyId);

            if (accountId != 0)
                parameters.Add("accountId", accountId);

            IList<int> orderIds = session.GetTypedListByNamedQuery<int>(
                "B4F.TotalGiro.Orders.GetNotarizableOrderIds",
                parameters);
            return orderIds.ToArray();
        }
        
        #region Exotics

        /// <summary>
        /// Gets a list of accounts with the number of orders and the total amount of commission involved.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="approved">Filter approved orders</param>
        /// <returns>A list of account with summarized order info</returns>
        public static IList GetUnAggregatedGroupedOrders(IDalSession session, bool approved)
        {
            InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            if (emp != null)
            {
                string hql = String.Format(@"select ord.Account.Key, ord.Account.ShortName, ord.Account.Number, sum(ord.Commission.Quantity), count(ord.Account.Key)
							      from SecurityOrder ord 
							      where ord.ParentOrder is null 
							      and ord.Status = 1
                                  and ord.Approved = {0}
                                  and ord.Account.AccountOwner.Key = {1}
							      group by ord.Account.Key, ord.Account.Number, ord.Account.ShortName", (approved ? 1 : 0), emp.Employer.Key);
                return session.GetListByHQL(hql);
            }
            else
                throw new AuthenticationException("You are not a registered user for this function: GetUnAggregatedGroupedOrders");

        }

        /// <summary>
        /// Gets a list of accounts with the number of orders and the total amount of commission involved.
        /// Also, a filter can be given on approvement, account name and number
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="accountNumber">(Part of) Account number</param>
        /// <param name="accountName">(Part of) Account name</param>
        /// <param name="approved">Filter approved orders</param>
        /// <returns>A list of account with summarized order info</returns>
        public static IList GetGroupedOrders(IDalSession session, string accountNumber, string accountName, bool approved)
        {
            InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            if (emp != null)
            {
                string filter = "";
                if (accountNumber != null && accountNumber.Length > 0)
                    filter += string.Format(" and ord.Account.Number like '%{0}%' ", accountNumber);
                if (accountName != null && accountName.Length > 0)
                    filter += string.Format(" and ord.Account.ShortName like '%{0}%' ", accountName);

                string hql = String.Format(@"select ord.Account.Key, ord.Account.ShortName, ord.Account.Number, sum(ord.CommissionDetails.Amount.Quantity), count(ord.Account.Key)
							      from SecurityOrder ord 
							      where ord.ParentOrder is null 
                                  and ord.Status = 1
                                  and ord.Approved = {0}
                                  and ord.Account.AccountOwner.Key = {1}
							      {2}
							      group by ord.Account.Key, ord.Account.Number, ord.Account.ShortName", 
                                        approved ? 1 : 0, emp.Employer.Key, filter);
                return session.GetListByHQL(hql);
            }
            else
                throw new AuthenticationException("You are not a registered user for this function: GetGroupedOrdersByAssetManager");

        }

        /// <summary>
        /// Returns a list of orders that have been exported in an export file
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="fileid">Idenitfier of the export file</param>
        /// <returns>Returns the orders that were in the export file with key fileid.</returns>
        public static IList GetOrdersPerExportFile(IDalSession session, int fileid)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("ExportFile.Key", fileid));
            LoginMapper.GetSecurityInfo(session, expressions);
            return session.GetList(typeof(SecurityOrder), expressions);
        }

        #endregion

        #region Actions

        /// <summary>
        /// Approve all orders that belong to the given accounts
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="accountKeys">Array of account keys</param>
        public static void ApproveOrdersPerAccount(IDalSession session, int[] accountKeys)
        {
            ArrayList orders = (ArrayList)GetOrders(session, OrderReturnClass.SecurityOrder, ApprovalState.UnApproved, accountKeys);
            foreach (Order order in orders)
            {
                if (!order.Approved && !order.IsClosed)
                    order.Approve();
            }
            OrderMapper.Update(session, orders);
        }

        /// <summary>
        /// Approve all orders in a given list
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="orders">A list of orders</param>
        public static void ApproveOrders(IDalSession session, IList orders)
        {
            foreach (Order order in orders)
            {
                if (!order.Approved && !order.IsClosed)
                    order.Approve();
            }
            OrderMapper.Update(session, orders);
        }

        /// <summary>
        /// Approve money orders. Approving them also does a send and place.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="orders">A list of orders that have to be approved</param>
        public static void ApproveMoneyOrders(IDalSession session, IList orders)
        {
            foreach (Order order in orders)
            {
                if (order.IsStgOrder)
                {
                    if (!order.Approved && !order.IsClosed)
                    {
                        order.Approve();
                        ((IStgMonetaryOrder)order).Send();
                        ((IStgMonetaryOrder)order).Place();
                    }
                }
            }
            OrderMapper.Update(session, orders);
        }

        /// <summary>
        /// Cancel all orders for specific accounts
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="accountKeys">A list of account numbers</param>
        public static void CancelOrdersPerAccount(IDalSession session, int[] accountKeys)
        {
            ArrayList orders = (ArrayList)GetOrders(session, OrderReturnClass.SecurityOrder, ApprovalState.All, accountKeys);
            foreach (Order order in orders)
            {
                order.Cancel();
            }
            OrderMapper.Update(session, orders);
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Creates a new object in the database
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">Object of type Order</param>
        public static void Insert(IDalSession session, IOrder obj)
        {
            session.Insert(obj);
        }

        /// <summary>
        /// Creates new Order objects in the database
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="list">A list of orders</param>
        public static void Insert(IDalSession session, IEnumerable list)
        {
            session.Insert(list);
        }

        /// <summary>
        /// Updates an Order object. Saves the data of the object to the database.
        /// </summary>
        /// <param name="DataSession">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">Order object</param>
        /// <returns></returns>
        public static bool Update(IDalSession session, IOrder obj)
        {
            session.Update(obj);
            return true;
        }

        /// <summary>
        /// Updates a list of objects, saves their data to the database
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="list">A list of objects</param>
        public static void Update(IDalSession session, IList list)
        {
            session.Update(list);
        }

        public static void Update(IDalSession session, IList<IOrder> list)
        {
            session.Update(list);
        }

        internal static void Delete(IDalSession session, Order obj)
        {
            session.Delete(obj);
        }

        internal static void Delete(IDalSession session, IList list)
        {
            session.Delete(list);
        }

        /// <summary>
        /// Stores order objects in the database by inserting/updating the orders objects
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="orders">List of orders to save to the database.</param>
        public static void SaveAggregatedOrders(IDalSession session, IList<IOrder> orders)
        {
            session.InsertOrUpdate(orders);
        }

        #endregion

        #region Helpers

        private static System.Type getType(OrderReturnClass retClass)
        {
            System.Type type;
            switch (retClass)
            {
                case OrderReturnClass.AmountBasedOrder:
                    type = typeof(OrderAmountBased);
                    break;
                case OrderReturnClass.SizeBasedOrder:
                    type = typeof(OrderSizeBased);
                    break;
                case OrderReturnClass.MonetaryOrder:
                    type = typeof(MonetaryOrder);
                    break;
                default:
                    type = typeof(SecurityOrder);
                    break;
            }
            return type;
        }

        private static void getAggregationLevel(List<ICriterion> expressions, OrderAggregationLevel aggLevel)
        {
            string types = "";

            if (containsAggregationLevel(aggLevel, OrderAggregationLevel.None))
                types = "1,2,3";

            if (containsAggregationLevel(aggLevel, OrderAggregationLevel.AssetManager))
                types += (types == string.Empty ? "" : ",") + "11,12,13";

            if (containsAggregationLevel(aggLevel, OrderAggregationLevel.Stichting) || containsAggregationLevel(aggLevel, OrderAggregationLevel.StichtingNetted))
                types += (types == string.Empty ? "" : ",") + "21,22,23";

            if (types != string.Empty)
            {
                if (expressions == null)
                    expressions = new List<ICriterion>();
                expressions.Add(Expression.Sql("this_.OrderTypeID IN (" + types + ")"));
                if (containsAggregationLevel(aggLevel, OrderAggregationLevel.StichtingNetted))
                    expressions.Add(Expression.Eq("IsNetted", true));
            }
        }

        private static void getActiveOrdersExpression(List<ICriterion> expressions, ActiveClosedState activeClosedState)
        {
            if (activeClosedState != ActiveClosedState.All)
            {
                if (expressions == null)
                    expressions = new List<ICriterion>();
                if (activeClosedState == ActiveClosedState.Active)
                    expressions.Add(Expression.IsNull("dateClosed"));
                else
                    expressions.Add(Expression.IsNotNull("dateClosed"));
            }
        }

        private static bool containsAggregationLevel(OrderAggregationLevel combined, OrderAggregationLevel checkagainst)
        {
            return ((combined & checkagainst) == checkagainst);
        }

        #endregion
    }
}

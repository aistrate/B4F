using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting.Login;
using System.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.Orders.AssetManager
{
    public static class AggregateSendAdapter
    {
        public static DataSet GetUnAggregatedChildOrders(string isin, string instrumentName, SecCategories secCategoryId, int currencyNominalId)
        {
            //"Key, Account.Number, Account.ShortName, DisplayTradedInstrumentIsin, TradedInstrument.DisplayName, Side, Value, Value.DisplayString, 
            //"Commission, Commission.DisplayString, DisplayIsSizeBased, CreationDate");
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return OrderMapper.GetOrders<ISecurityOrder>(session, OrderReturnClass.SecurityOrder, OrderAggregationLevel.None, ApprovalState.Approved,
                    SecurityInfoOptions.ManagedsAcctsOnly, ParentalState.Null, new OrderStatusFilter(OrderStati.New), 
                    isin, instrumentName, secCategoryId, currencyNominalId, null)
                    .Select(c => new
                    {
                        c.Key,
                        Account_Key = c.Account.Key,
                        Account_Number = c.Account.Number,
                        Account_ShortName = c.Account.ShortName,
                        c.DisplayTradedInstrumentIsin,
                        TradedInstrument_DisplayName = c.TradedInstrument.DisplayName,
                        c.Side,
                        c.Value,
                        Value_DisplayString = c.Value.DisplayString,
                        c.Commission,
                        Commission_DisplayString = (c.Commission != null ? c.Commission.DisplayString : ""),
                        c.DisplayIsSizeBased,
                        c.CreationDate
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetAggregatedOrders(string isin, string instrumentName, SecCategories secCategoryId, int currencyNominalId)
        {
            //"Key, ParentOrder, ChildOrders.Count, TradedInstrument.DisplayName, DisplayTradedInstrumentIsin, Side, Value, Value.DisplayString,
            //"DisplayIsSizeBased, CreationDate, OrderID");
            //Route.Name
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return OrderMapper.GetOrders<IAggregatedOrder>(session, OrderReturnClass.SecurityOrder, OrderAggregationLevel.AssetManager, ApprovalState.UnApproved,
                    SecurityInfoOptions.TradingAcctOnly, ParentalState.Null, new OrderStatusFilter(OrderStati.New),
                    isin, instrumentName, secCategoryId, currencyNominalId, null)
                    .Select(c => new
                    {
                        c.Key,
                        c.ParentOrder,
                        ChildOrders_Count = c.ChildOrders.Count,
                        TradedInstrument_DisplayName = c.RequestedInstrument.DisplayName,
                        c.DisplayTradedInstrumentIsin,
                        c.Side,
                        c.Value,
                        Value_DisplayString = c.Value.DisplayString,
                        c.DisplayIsSizeBased,
                        c.CreationDate,
                        c.OrderID
                    })
                    .ToDataSet();
            }
        }

        public static void AggregateOrders(int[] orderIds, out string errorMessage)
        {
            AggregateOrders(orderIds, out errorMessage, false, null);
        }

        public static void AggregateOrders(int[] orderIds, out string errorMessage, bool isAggregateSpecial, int[] parentOrderIds)
        {
            string message;
            errorMessage = "";

            if (orderIds == null || orderIds.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
            
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                IList<IOrder> unAggregatedOrders = OrderMapper.GetOrders(session, orderIds);
                if (unAggregatedOrders != null && unAggregatedOrders.Count > 0)
                {
                    // If parentOrderIds NOT NULL -> Aggregate Special
                    IList<IOrder> aggregateOrders = null;
                    if (isAggregateSpecial)
                    {
                        if (parentOrderIds != null && parentOrderIds.Length > 0)
                            aggregateOrders = OrderMapper.GetOrders(session, parentOrderIds);
                    }
                    else
                    {
                        aggregateOrders = OrderMapper.GetOrders<IOrder>(session, OrderReturnClass.SecurityOrder, OrderAggregationLevel.AssetManager,
                        ApprovalState.UnApproved, SecurityInfoOptions.TradingAcctOnly, ParentalState.Null, new OrderStatusFilter(OrderStati.New));
                    }
                    IList<IOrder> orders = Order.AggregateOrders(aggregateOrders, unAggregatedOrders, out message);
                    if (orders != null)
                    {
                        OrderMapper.SaveAggregatedOrders(session, orders);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                session.Close();
            }
        }

        public static void DeAggregateOrders(int[] orderIds)
        {
            bool success = false;

            if (orderIds == null || orderIds.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
            
            IDalSession session = NHSessionFactory.CreateSession();
            IList<IOrder> orders = OrderMapper.GetOrders(session, orderIds);
            foreach (Order order in orders)
            {
                success = order.DeAggregate();

            }
            if (success)
                OrderMapper.Update(session, orders);
            session.Close();
        }

        public static void UnApproveOrders(int[] orderIds)
        {
            bool success = false;
            if (orderIds == null || orderIds.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            IDalSession session = NHSessionFactory.CreateSession();
            IList<IOrder> orders = OrderMapper.GetOrders(session, orderIds);
            foreach (Order order in orders)
            {
                success = order.UnApprove();

            }
            if (success)
                OrderMapper.Update(session, orders);
            session.Close();
        }

        public static void SendOrders(int[] orderIds)
        {
            if (orderIds == null || orderIds.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
            
            IDalSession session = NHSessionFactory.CreateSession();
            IList<IOrder> orders = OrderMapper.GetOrders(session, orderIds);
            foreach (Order order in orders)
            {
                order.Approve();

            }
            OrderMapper.Update(session, orders);
            session.Close();
        }
    }
}

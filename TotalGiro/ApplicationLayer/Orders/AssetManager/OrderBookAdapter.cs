using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.Auditing;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.ApplicationLayer.Orders.AssetManager
{
    public static class OrderBookAdapter
    {
        public static DataSet GetOrders(int orderType, string accountNumber, string accountName, string isin, string instrumentName, SecCategories secCategoryID, int activeFlag, string orderID, DateTime DateFrom, DateTime DateTo)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IList<IOrder> orders;
            int id;

            if (orderID != string.Empty && int.TryParse(orderID, out id))
            {
                int[] ids = new int[1] { id };

                orders = OrderMapper.GetOrders(session, ids, true);
            }
            else
                orders = OrderMapper.GetOrders<IOrder>(session,
                    (OrderReturnClass)orderType,
                    (OrderAggregationLevel.None | OrderAggregationLevel.AssetManager),
                    ApprovalState.All,
                    SecurityInfoOptions.NoFilter,
                    ParentalState.All, null, null, null, 0, accountNumber, accountName, (ActiveClosedState)activeFlag, isin, instrumentName, secCategoryID, 0, DateFrom, DateTo
                    );

            //"Account.Number, Account.ShortName, RequestedInstrument.DisplayName, DisplayTradedInstrumentIsin, Side, Value, Value.DisplayString, FilledValue, " +
            //"CommissionInfo, Status, DisplayStatus, TopParentDisplayStatus, CreationDate, OrderID");
            DataSet ds = orders
                .Select(c => new
                {
                    Account_Number = c.Account.Number,
                    Account_ShortName = c.Account.ShortName,
                    RequestedInstrument_DisplayName = c.RequestedInstrument.DisplayName,
                    c.DisplayTradedInstrumentIsin,
                    c.Side,
                    c.Value,
                    Value_DisplayString = c.Value.DisplayString,
                    c.FilledValue,
                    c.CommissionInfo,
                    Status = c.TopParentOrder != null ? c.TopParentOrder.Status : c.Status,
                    DisplayStatus = c.TopParentDisplayStatus != "" ? c.TopParentDisplayStatus : c.DisplayStatus,
                    c.CreationDate,
                    c.OrderID
                })
                .ToDataSet();


            session.Close();
            HttpContext.Current.Session["RefreshedTimeOrderBook"] = DateTime.Now;
            return ds;
        }

        public static DataSet GetOrderTransactions(object orderId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;

                if (orderId != null)
                {
                    ITransactionOrder[] transactions = null;

                    IOrder order = OrderMapper.GetOrder(session, (int)orderId, SecurityInfoOptions.NoFilter);

                    transactions = new ITransactionOrder[order.Transactions.Count];
                    order.Transactions.CopyTo(transactions, 0);

                    //"Order.OrderID, ValueSize.DisplayString, CounterValueSize.DisplayString, Price.DisplayString, Commission.DisplayString, ServiceCharge.DisplayString, ExchangeRate, TransactionDate");
                    ds = transactions
                        .Select(c => new
                        {
                            Order_OrderID = c.Order.OrderID,
                            ValueSize_DisplayString = c.ValueSize.DisplayString,
                            CounterValueSize_DisplayString = (c.CounterValueSize != null ? c.CounterValueSize.DisplayString : ""),
                            Price_DisplayString = c.Price.DisplayString,
                            Commission_DisplayString = (c.Commission != null ? c.Commission.DisplayString : ""),
                            ServiceCharge_DisplayString = (c.ServiceCharge != null ? c.ServiceCharge.DisplayString : ""),
                            AccruedInterest_DisplayString = (c.AccruedInterest != null ? c.AccruedInterest.DisplayString : ""),
                            c.ExchangeRate,
                            c.TransactionDate
                        })
                        .ToDataSet();
                }
                return ds;
            }
        }

        public static DataSet GetActiveStati()
        {
            return Util.GetDataSetFromEnum(typeof(ActiveClosedState));
        }

        public static DataSet GetOrderTypes()
        {
            return Util.GetDataSetFromEnum(typeof(OrderReturnClass), SortingDirection.Descending); 
        }

        public static string GetOrderClassName(int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            
            IOrder order = OrderMapper.GetOrder(session, orderId);
            
            session.Close();

            return order.GetType().Name;
        }
    }
}

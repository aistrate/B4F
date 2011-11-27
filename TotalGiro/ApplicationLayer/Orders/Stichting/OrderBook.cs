using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting.Login;
using System.Web;
using System.Collections;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public static class OrderBook
    {
        public static DataTable DtGetOrders()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            DataTable dt = new System.Data.DataTable("dt");
            dt.Columns.Add(new System.Data.DataColumn("OrderID", typeof(int)));
            dt.Columns.Add(new System.Data.DataColumn("Account_ShortName", typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn("DisplayStatus", typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn("ChildOrders", typeof(int)));

            IList<ISecurityOrder> orders = OrderMapper.GetOrders<ISecurityOrder>(
                                                session,
                                                OrderReturnClass.SecurityOrder,
                                                OrderAggregationLevel.Stichting,
                                                ApprovalState.All,
                                                SecurityInfoOptions.Both,
                                                ParentalState.Null);

            IList<ISecurityOrder> noneAggrOrders = OrderMapper.GetOrders<ISecurityOrder>(
                                                session,
                                                OrderReturnClass.SecurityOrder,
                                                OrderAggregationLevel.None,
                                                ApprovalState.All,
                                                SecurityInfoOptions.Both,
                                                ParentalState.Null);

            foreach (ISecurityOrder noneAggrOrder in noneAggrOrders)
                orders.Add(noneAggrOrder);

            foreach (ISecurityOrder order in orders)
            {
                DataRow dr = dt.NewRow();
                dr[0] = order.OrderID;
                dr[1] = order.Account.ShortName.ToString();
                dr[2] = order.DisplayStatus;
                if (order.ChildOrders.Count > 0)
                {
                    dr[3] = 1;
                }
                else
                {
                    dr[3] = 0;
                }
                //if (order.OrderID == 98307)
                    dt.Rows.Add(dr);
            }

            HttpContext.Current.Session["RefreshedTimeOrderBookCockpit"] = DateTime.Now;
            session.Close();

            return dt;
        }

        //public static DataSet GetOrder(int orderID)
        //{
        //    DataSet ds = new DataSet();
        //    IDalSession session = NHSessionFactory.CreateSession();

        //    IOrder  order = OrderMapper.GetOrder(session, orderID, SecurityInfoOptions.NoFilter);
        //    if (order == null)
        //    {
        //        DataTable dt = new DataTable();

        //        dt.Columns.Add(new DataColumn("OrderID"));
        //        dt.Columns.Add(new DataColumn("Account_Number"));
        //        dt.Columns.Add(new DataColumn("Account_ShortName"));
        //        dt.Columns.Add(new DataColumn("TradedInstrument_DisplayName"));
        //        dt.Columns.Add(new DataColumn("DisplayTradedInstrumentIsin"));
        //        dt.Columns.Add(new DataColumn("Value_DisplayString"));
        //        dt.Columns.Add(new DataColumn("CommissionInfo"));
        //        dt.Columns.Add(new DataColumn("DisplayStatus"));

        //        DataRow dataRow = dt.NewRow();

        //        dt.Rows.Add(dataRow);
        //        ds.Tables.Add(dt);
        //        return ds;
        //    }
        //    ArrayList orderList = new ArrayList();
        //    orderList.Add(order);

        //    ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
        //            orderList,
        //            "OrderID, Account.Number, Account.ShortName, TradedInstrument.DisplayName, DisplayTradedInstrumentIsin, Side, Value, Value.DisplayString, " +
        //            "CommissionInfo, Status, DisplayStatus");

        //    session.Close();

        //    return ds;
        //}
    }
}

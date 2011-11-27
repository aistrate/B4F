using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public static class AggOrderChildrenAdapter
    {
        // this is also used by AssetManager/AggOrderChildren.aspx
        public static DataSet GetAggregatedChildOrders(int parentOrderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                OrderMapper.GetChildOrders(session, parentOrderId),
                "Key, Account.Number, Account.ShortName, RequestedInstrument.DisplayName, DisplayTradedInstrumentIsin, Side, Value, Value.DisplayString, " +
                    "Commission, Commission.DisplayString, DisplayIsSizeBased, Status, CreationDate, OrderID");
            session.Close();

            return ds;
        }

        public static void DeleteOrder(int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IOrder delOrder = OrderMapper.GetOrder(session, orderId);
            delOrder.Cancel();
            OrderMapper.Update(session, delOrder);
            session.Close();
        }
    }
}

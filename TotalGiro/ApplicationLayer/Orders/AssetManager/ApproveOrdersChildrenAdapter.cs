using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts;


namespace B4F.TotalGiro.ApplicationLayer.Orders.AssetManager
{
    public static class ApproveOrdersChildrenAdapter
    {
        public static DataSet GetOrdersByAccount(int accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IAccountTypeInternal account = (IAccountTypeInternal) AccountMapper.GetAccount(session,accountId);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                OrderMapper.GetOrders(session, OrderReturnClass.SecurityOrder, ApprovalState.UnApproved, account),
                "Key, Account.Number, Account.ShortName, TradedInstrument.DisplayName, DisplayTradedInstrumentIsin, Side, Value, Value.DisplayString, " +
                    "Commission, Commission.DisplayString, DisplayIsSizeBased, Status, DisplayStatus, OrderID, DisplayInstructionKey, CreationDate");
            session.Close();

            return ds;
        }

        public static void CancelOrder(int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IOrder cancelOrder = OrderMapper.GetOrder(session, orderId);
            cancelOrder.Cancel();
            OrderMapper.Update(session, cancelOrder);
            session.Close();
        }
    }
}

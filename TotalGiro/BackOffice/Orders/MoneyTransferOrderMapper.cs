using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace B4F.TotalGiro.BackOffice.Orders
{
    public static class MoneyTransferOrderMapper
    {
        public static IMoneyTransferOrder GetMoneyTransferOrder(IDalSession session, int MoneyTransferOrderID)
        {
            return (IMoneyTransferOrder)session.GetObjectInstance(typeof(MoneyTransferOrder), MoneyTransferOrderID);
        }

        public static IList<IMoneyTransferOrder> GetMoneyTransferOrders(IDalSession session)
        {
            return session.GetTypedList<MoneyTransferOrder,IMoneyTransferOrder>();
        }

        public static IMoneyTransferOrderStatus GetMoneyTransferOrderStatus(IDalSession session, MoneyTransferOrderStati moneyTransferOrderStati)
        {
            return (IMoneyTransferOrderStatus)session.GetObjectInstance(typeof(MoneyTransferOrderStatus), moneyTransferOrderStati);
        }

        public static bool Update(IDalSession session, IMoneyTransferOrder obj)
        {

            bool blnSuccess = session.InsertOrUpdate(obj);
            if (blnSuccess && obj.Reference == null)
            {
                obj.setReference();
                blnSuccess = session.InsertOrUpdate(obj);
            }
            return blnSuccess;
        }

        public static bool Update(IDalSession session, IList listofOrders)
        {

            bool blnSuccess = true;

            foreach (IMoneyTransferOrder obj in listofOrders)
                if (!Update(session, obj)) blnSuccess = false;

            return blnSuccess;
        }

        public static IList<MoneyTransferOrder> GetMoneyTransferOrders(IDalSession session, int[] orderIDs, bool unSentOnly)
        {                        
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("Key", orderIDs));
            IList<MoneyTransferOrder> orders = session.GetTypedList<MoneyTransferOrder>(expressions);

            if (unSentOnly)
                return orders.Where(o => o.IsSendable).ToList();
            else
                return orders;
        }
    }
}

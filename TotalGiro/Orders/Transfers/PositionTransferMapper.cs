using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using NHibernate.Linq;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Orders.Transfers
{
    public static class PositionTransferMapper
    {

        public static IPositionTransfer getTransfer(IDalSession session, int transferID)
        {
            IPositionTransfer transfer = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", transferID));
            IList<IPositionTransfer> list = session.GetTypedList<PositionTransfer, IPositionTransfer>(expressions);
            if (list != null && list.Count == 1)
                transfer = list[0];
            return transfer;
        }

        public static IList<IPositionTransfer> GetPositionTransfers(IDalSession session)
        {
            return session.GetTypedList<IPositionTransfer>();
        }

        public static IPositionTransferDetail getTransferDetail(IDalSession session, int transferDetailID)
        {
            return session.Session.Linq<IPositionTransferDetail>()
                                .Where(dh => dh.Key == transferDetailID)
                                .First();
        }

    }
}

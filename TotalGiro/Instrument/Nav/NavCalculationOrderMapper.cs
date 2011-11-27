using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders;
using System.Collections;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Instruments.Nav
{
    public static class NavCalculationOrderMapper
    {
        public static IList<IOrder> NewOrdersForFund(IDalSession session, int virtualFundID, DateTime approvalDate)
        {
            Hashtable parameters = new Hashtable();
            string strHql = "B4F.TotalGiro.Instruments.Nav.NewOrdersForFund";

            parameters.Add("VirtualFundID", virtualFundID);
            parameters.Add("Status", (int)OrderStati.Placed);
            parameters.Add("ApprovalDate", approvalDate.AddHours(15));

            return session.GetTypedListByNamedQuery<IOrder>(strHql, parameters);

        }

        public static IList<IOrder> OrdersForFund(IDalSession session, int calcID)
        {
            Hashtable parameters = new Hashtable();
            string strHql = "B4F.TotalGiro.Instruments.Nav.OrdersForFund";

            parameters.Add("CalcID", calcID);

            return session.GetTypedListByNamedQuery<IOrder>(strHql, parameters);

        }

    }
}

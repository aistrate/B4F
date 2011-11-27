using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Collections;

namespace B4F.TotalGiro.GeneralLedger.Journal.Reporting
{
    public static class ClientCashPositionFromGLLedgerMapper
    {
        public static ClientCashPositionFromGLLedger GetClientCashPositionFromGLLedger(IDalSession session, DateTime bookDate)
        {
            string hql;
            Hashtable parameters = new Hashtable();


            hql = @"FROM ClientCashPositionFromGLLedger T WHERE T.Key = :bookDate";
            parameters.Add("bookDate", bookDate);

            IList balances = session.GetListByHQL(hql, parameters);

            return (balances.Count > 0 ? (ClientCashPositionFromGLLedger)balances[0] : null);
        }
    }
}

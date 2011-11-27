using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Collections;

namespace B4F.TotalGiro.GeneralLedger.Journal.Maintenance
{
    public static class BookYearClosureMapper
    {
        public static IBookYearClosure GetBookYearClosure(IDalSession session, int bookYearClosureID)
        {
            string hql;
            Hashtable parameters = new Hashtable();

            hql = @"FROM BookYearClosure B WHERE B.Key = :bookYearClosureID";
            parameters.Add("bookYearClosureID", bookYearClosureID);

            IList balances = session.GetListByHQL(hql, parameters);
            return (balances.Count > 0 ? (IBookYearClosure)balances[0] : null);
        }
    }
}

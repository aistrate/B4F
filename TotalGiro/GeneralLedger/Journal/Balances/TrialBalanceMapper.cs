using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using System.Collections;

namespace B4F.TotalGiro.GeneralLedger.Journal.Balances
{
    public static class TrialBalanceMapper
    {
        public static TrialBalance GetTrialBalance(IDalSession session, DateTime transactionDate)
        {
            string hql;
            Hashtable parameters = new Hashtable();


            hql = @"FROM TrialBalance T WHERE T.Key = :TransactionDate";
            parameters.Add("TransactionDate", transactionDate);

            IList balances = session.GetListByHQL(hql, parameters);

            return (balances.Count > 0 ? (TrialBalance)balances[0] : null);
        }


    }
}

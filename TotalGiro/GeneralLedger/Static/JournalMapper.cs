using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public static class JournalMapper
    {
        public static IList<IJournal> GetJournals(IDalSession session, JournalTypes journalType)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();
            expressions.Add(Expression.Eq("JournalType", journalType));
            orderings.Add(Order.Asc("JournalNumber"));
            return session.GetTypedList<Journal, IJournal>(expressions, orderings);
        }

        public static IList<IJournal> GetJournals(IDalSession session)
        {
            return session.GetTypedList<Journal, IJournal>();
        }

        public static IJournal GetJournal(IDalSession session, int journalId)
        {
            IJournal journal = null;

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", journalId));
            IList list = session.GetList(typeof(Journal), expressions);
            if (list != null && list.Count == 1)
                journal = (IJournal)list[0];

            return journal;
        }

        public static IJournal GetJournal(IDalSession session, JournalTypes journalType, int currencyKey)
        {
            IJournal journal = null;
            Hashtable parameters = new Hashtable();

            string query = @"B4F.TotalGiro.GeneralLedger.Static.Journal.GetJournalByType";

            parameters.Add("journalType", journalType);
            parameters.Add("currencyKey", currencyKey);

            IList list =session.GetListByNamedQuery(query, parameters);
            if (list != null && list.Count == 1)
                journal = (IJournal)list[0];

            return journal;
        }

        public static IJournal GetSettlementDifferenceJournal(IDalSession session)
        {
                int journalID = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get("DefaultSettlementDifference")));
                return GetJournal(session, journalID);
        }

        public static IList GetManagementFeeDatesToProcess(IDalSession session, DateTime startDate)
        {

            Hashtable parameters = new Hashtable();

            StringBuilder sb = new StringBuilder(@"select distinct m.TransactionDate from ManagementFee m");
            sb.Append(@" where m.TransactionDate > :startDate");
            sb.Append(@" and m.Key not in (Select J.TotalGiroTrade.Key from JournalEntryLine J where J.TotalGiroTrade is not null)");
            sb.Append(@" order by m.TransactionDate");

            parameters.Add("startDate", startDate);

            string hql = sb.ToString();
            IList balances = session.GetListByHQL(hql, parameters);

            return balances;
        }

        public static IList GetManagementFeesToProcess(IDalSession session, DateTime transactionDate)
        {

            Hashtable parameters = new Hashtable();

            StringBuilder sb = new StringBuilder(@"from ManagementFee m");
            sb.Append(@" where m.TransactionDate = :transactionDate");
            sb.Append(@" and m.Key not in (Select J.TotalGiroTrade.Key from JournalEntryLine J where J.TotalGiroTrade is not null)");

            parameters.Add("transactionDate", transactionDate);

            string hql = sb.ToString();
            IList balances = session.GetListByHQL(hql, parameters);

            return balances;
        }




    }
}

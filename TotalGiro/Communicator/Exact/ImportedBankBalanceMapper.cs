using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NHibernate.Criterion;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Communicator.Exact
{
    public static class ImportedBankBalanceMapper
    {
        public static IImportedBankBalance GetImportedBankBalance(IDalSession session, IJournal bankJournal, DateTime bookBalanceDate)
        {

            Hashtable parameters = new Hashtable();
            parameters.Add("BankJournalKey", bankJournal.Key);
            parameters.Add("BookBalanceDate", bookBalanceDate);

            string hql = @"from ImportedBankBalance I
                            where I.BankJournal.Key = :BankJournalKey
                            and I.BookBalanceDate = :BookBalanceDate";

            IList result = session.GetListByHQL(hql, parameters);
            if (result != null && result.Count == 1)
                return (ImportedBankBalance)result[0];
            else
                return null;

            //List<ICriterion> expressions = new List<ICriterion>();
            //expressions.Add(Expression.Eq("BankJournal.Key", bankJournal.Key));
            //expressions.Add(Expression.Eq("BookBalanceDate", bookBalanceDate));
            //IList result = session.GetList(typeof(ImportedBankBalance), expressions);
            //if (result != null && result.Count == 1)
            //    return (IImportedBankBalance)result[0];
            //else
            //    return null;
        }
    }
}

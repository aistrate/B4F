using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NHibernate.Criterion;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Communicator.Exact
{
    public static class ImportedBankMovementMapper
    {
        public static IList GetImportedBankMovements(IDalSession session, IJournal bankJournal, DateTime bankStatementDate)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();
            expressions.Add(Expression.Eq("BankJournal.Key", bankJournal.Key));
            expressions.Add(Expression.Eq("BankStatementDate", bankStatementDate));
            orderings.Add(Order.Asc("CloseBalanceProcessDate"));
            orderings.Add(Order.Asc("CloseBalanceProcessTime"));
            
            // Last two null parameters are necessary, otherwise sorting doesn't work
            return session.GetList(typeof(ImportedBankMovement), expressions, orderings, null, null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using System.Collections;

namespace B4F.TotalGiro.Communicator.Exact
{
    public static class ExactJournalMapper
    {
        public static IExactJournal GetExactJournal(IDalSession session, int id)
        {

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", id));
            IList files = session.GetList(typeof(ExactJournal), expressions);
            if (files != null && files.Count == 1)
                return (ExactJournal)(files[0]);
            else
                return null;
        
        }

        public static IList<IExactJournal> GetExactJournals(IDalSession session)
        {
            return session.GetTypedList<ExactJournal, IExactJournal>();
        }
    }
}

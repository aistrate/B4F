using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using System.Collections;

namespace B4F.TotalGiro.Communicator.BelastingDienst
{
    public static class DividWepFileMapper
    {


        public static DividWepFile GetDividWepFile(IDalSession session, int FileID)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", FileID));
            IList files = session.GetList(typeof(DividWepFile), expressions);
            if (files != null && files.Count == 1)
                return (DividWepFile)(files[0]);
            else
                return null;
        }

        public static IList<IDividWepFile> GetDividWepFiles(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            return session.GetTypedList<IDividWepFile>(expressions);
        }

        public static IDividWepFile GetLastDividWepFileCreated(IDalSession session)
        {

            string hql = string.Format(
                @"from DividWepFile DW
                    where DW.FinancialYear in  
                    (Select Max(D.FinancialYear)
                    from DividWepFile D)");
            IList files = session.GetListByHQL(hql);
            if (files != null && files.Count == 1)
                return (DividWepFile)(files[0]);
            else
                return null;
        }

        public static bool Update(IDalSession session, DividWepFile obj)
        {
            return session.Update(obj);
        }
    }
}

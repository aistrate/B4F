using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using System.Linq;


namespace B4F.TotalGiro.Communicator.KasBank
{
    public static class GLDSTDMapper
    {
        public static IList GetRecordsByFile(IDalSession session, GLDSTDFile parentFile)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("ParentFile", parentFile));
            return session.GetList(typeof(GLDSTD), expressions);
        }

        public static IList GetRecordsByFile(IDalSession session, int parentFileID)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("parentFileID", parentFileID);

            string hql = "from GLDSTD G " +
                    "where G.ParentFile.Key = :parentFileID ";

            return session.GetListByHQL(hql, parameters);
        }

        public static IGLDSTD GetRecord(IDalSession session, int recordID)
        {

            IGLDSTD record = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", recordID));
            IList list = session.GetList(typeof(GLDSTD), expressions);
            if (list != null && list.Count == 1)
            {
                record = (IGLDSTD)list[0];
            }
            return record;

        }
    }
}

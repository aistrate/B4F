using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using System.Linq;


namespace B4F.TotalGiro.Communicator.KasBank
{
    public static class GLDSTDFileMapper
    {
        public static bool Update(IDalSession session, IGLDSTDFile obj)
        {
            bool blnSuccess = session.InsertOrUpdate(obj);
            return blnSuccess;
        }

        public static IList GetExportFilesByCriteria(IDalSession p_objDataSession, DateTime p_datStartDate, DateTime p_datEndDate, string p_strReference)
        {

            //prep an expressionlist
            List<NHibernate.Criterion.ICriterion> l_objExpressions = new List<NHibernate.Criterion.ICriterion>();


            ////do we have a Reference Number
            //if (p_strFsNumber != null && p_strFsNumber != "" && l_objExpressions.Count == 0)
            //{
            //    l_objExpressions.Add(NHibernate.Expression.Expression.Eq("FSNumber", p_strFsNumber));
            //}

            //only add date criteria when no other criteria are used
            if (l_objExpressions.Count == 0)
            {
                l_objExpressions.Add(NHibernate.Criterion.Expression.Ge("CreationDate", p_datStartDate));
                l_objExpressions.Add(NHibernate.Criterion.Expression.Le("CreationDate", p_datEndDate.AddDays(1)));
            }
            return p_objDataSession.GetList(typeof(GLDSTDFile), l_objExpressions);

        }

    }
}

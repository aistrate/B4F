using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;

using NHibernate.Criterion;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Communicator.ImportedBankFiles
{
    public class FileToImportMapper
    {
        public static IList GetFilesToImport(IDalSession session)
        {
            return session.GetList(typeof(FileToImport));
        }

        public static bool Update(IDalSession session, FileToImport obj)
        {
            return session.InsertOrUpdate(obj);
        }

        public static IList GetEnabledFilesToImport(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Enabled", true));
            return session.GetList(typeof(FileToImport), expressions);
        }
    }
}

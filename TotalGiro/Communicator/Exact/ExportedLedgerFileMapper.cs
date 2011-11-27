using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Collections;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Communicator.Exact
{
    public static class ExportedLedgerFileMapper
    {
        public static int GetNextOrdinal(IDalSession session, string fileName)
        {
            string sqlQuery = string.Format("SELECT TOP 1 {0} FROM ExportedLedgerFile E WHERE E.Name = '{1}' ORDER BY E.Ordinal DESC",
                                            "{E.*}", fileName);
            IList exportedLedgerFiles = session.GetListbySQLQuery(sqlQuery, "E", typeof(ExportedLedgerFile));

            if (exportedLedgerFiles.Count > 0)
                return ((IExportedLedgerFile)exportedLedgerFiles[0]).Ordinal + 1;
            else
                return 0;
        }

        public static IExportedLedgerFile GetExportedFile(IDalSession session, int fileID)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", fileID));
            IList result = session.GetList(typeof(ExportedLedgerFile), expressions);
            if ((result != null) && (result.Count > 0))
                return (IExportedLedgerFile)result[0];
            else
                return null;
        }

        public static IList<IExportedLedgerFile> GetExportedLedgerFiles(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            return session.GetTypedList<IExportedLedgerFile>(expressions);
        }
        
        public static bool Update(IDalSession session, IExportedLedgerFile obj)
        {
            return session.InsertOrUpdate(obj);
        }
    }
}

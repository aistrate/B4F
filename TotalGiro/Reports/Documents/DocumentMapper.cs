using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Reports.Financial;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Reports.Documents
{
    /// <summary>
    /// Specifies the kind of document for which a notification email is to be sent.
    /// Used by the back office GUI (Reports / Notas pages).
    /// </summary>
    public enum DocumentSubtypes
    {
        Notas = 1,
        QuarterlyReports = 2,
        YearlyReports = 4
    }

    public static class DocumentMapper
    {
        public static List<INotaDocument> GetNotaDocuments(IDalSession session, int accountId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("AccountId", accountId);

            string hql = @"FROM NotaDocument DD WHERE
                           DD.Key IN (SELECT D.Key FROM NotaDocument D
                                      JOIN D.bagOfNotas N
                                      WHERE N.NotaAccount.Account.Key = :AccountId)";

            return session.GetTypedListByHQL<INotaDocument>(hql, parameters);
        }

        public static List<NotaDocumentView> GetNotaDocumentViews(IDalSession session, int accountId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("AccountId", accountId);

            string hql = @"SELECT D.Key, D.CreationDate, MIN(N.Key), COUNT(*)
                           FROM Nota N
                           INNER JOIN N.Document D
                           WHERE N.NotaAccount.Account.Key = :AccountId
                           GROUP BY D.Key, D.CreationDate";

            var documents = session.GetListByHQL(hql, parameters)
                                   .ToDataTableFromHibernateList("Key, CreationDate, FirstNotaId, NotaCount", "Documents")
                                   .Rows.Cast<DataRow>()
                                   .Select(r => new
                                   {
                                       Key = (int)r["Key"],
                                       CreationDate = (DateTime)r["CreationDate"],
                                       FirstNotaId = (int)r["FirstNotaId"],
                                       NotaCount = (int)(long)r["NotaCount"],
                                   })
                                   .ToList();

            List<INota> notas = NotaMapper.GetNotas(session, documents.Select(d => d.FirstNotaId).ToArray());

            return documents.Select(d =>
                            {
                                INota nota = notas.First(n => n.Key == d.FirstNotaId);

                                return new NotaDocumentView(
                                                d.Key,
                                                d.CreationDate,
                                                nota.NotaNumber,
                                                nota.Title,
                                                d.NotaCount);
                            })
                            .ToList();
        }

        public static List<IFinancialReportDocument> GetFinancialReportDocuments(IDalSession session, int accountId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("AccountId", accountId);
            parameters.Add("ReportStatus", (int)ReportStatuses.PrintSuccess);

            string hql = @"FROM FinancialReportDocument D
                           WHERE D.Report.Account = :AccountId
                             AND D.Report.ReportStatusId = :ReportStatus";

            return session.GetTypedListByHQL<IFinancialReportDocument>(hql, parameters);
        }

        public static IDocument GetDocument(IDalSession session, int documentId)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", documentId));
            List<IDocument> documents = session.GetTypedList<Document, IDocument>(expressions);
            if (documents != null && documents.Count > 0)
                return documents[0];
            else
                return null;
        }

        public static int[] GetAccountIdsWithFreshDocuments(IDalSession session, DocumentSubtypes documentSubtype, 
                                                            int managementCompanyId, int accountId)
        {
            Hashtable parameters = new Hashtable();
            
            string hql = string.Format(@"SELECT CA.Key FROM CustomerAccount CA 
                                          WHERE CA.Key IN (SELECT DISTINCT A.Key {0})", hqlDocumentsWithJoins(documentSubtype, false));

            if (managementCompanyId != 0)
            {
                parameters.Add("ManagementCompanyId", managementCompanyId);
                hql += " AND CA.AccountOwner.Key = :ManagementCompanyId";
            }

            if (accountId != 0)
            {
                parameters.Add("AccountId", accountId);
                hql += " AND CA.Key = :AccountId";
            }

            hql += " ORDER BY CA.Number";

            return session.GetTypedListByHQL<int>(hql, parameters).ToArray();
        }

        public static List<IDocument> GetFreshDocuments(IDalSession session, DocumentSubtypes documentSubtype, int accountId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("AccountId", accountId);
            
            string hql = string.Format(@"FROM Document DD
                                         WHERE DD.Key IN (SELECT DISTINCT D.Key {0} AND A.Key = :AccountId)
                                         ORDER BY DD.CreationDate",
                                       hqlDocumentsWithJoins(documentSubtype, false));
            
            return session.GetTypedListByHQL<IDocument>(hql, parameters);
        }

        private static string hqlDocumentsWithJoins(DocumentSubtypes documentSubtype, bool? emailNotificationHandled)
        {
            string hql;

            if (documentSubtype == DocumentSubtypes.Notas)
                hql = @"FROM NotaDocument D
                        INNER JOIN D.bagOfNotas N
                        INNER JOIN N.NotaAccount.Account A
                        WHERE N.PrintCount != 0";
            else
            {
                Type reportSubclass = (documentSubtype == DocumentSubtypes.QuarterlyReports ? 
                                                                typeof(ReportQuarter) : 
                                                                typeof(ReportEOY));
                hql = string.Format(@"FROM FinancialReportDocument D
                                      INNER JOIN D.Report R
                                      INNER JOIN R.Account A
                                      WHERE R.ReportStatusId = {0}
                                      AND R.Key IN (SELECT RR.Key FROM {1} RR)", (int)ReportStatuses.PrintSuccess, reportSubclass.Name);
            }

            if (emailNotificationHandled != null)
                hql += " AND D.EmailNotificationHandled = " + emailNotificationHandled.ToString().ToLower();

            return hql;
        }

        public static bool Update(IDalSession session, IDocument obj)
        {
            return session.InsertOrUpdate(obj);
        }

        public static int CountDocumentsSentByPost(IDalSession session, int accountId, DateTime startDate, DateTime endDate)
        {
            long count = session.Session.GetNamedQuery(
                "B4F.TotalGiro.Reports.Documents.CountDocumentsSentByPost")
                .SetInt32("accountId", accountId)
                .SetInt32("reportStatusId", (int)ReportStatuses.PrintSuccess)
                .SetDateTime("startDate", startDate)
                .SetDateTime("endDate", endDate)
                .UniqueResult<long>();
            return Convert.ToInt32(count);
        }
    }
}

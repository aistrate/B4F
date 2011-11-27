using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Reports;
using NHibernate;
using NHibernate.Criterion;

using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting;

using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Reports.Financial
{
    public class ReportMapper
    {
        public static IReport GetReport(IDalSession session, int accountId, int reportLetterYear, ReportLetterTypes reportLetterType)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("AccountId", accountId);
            parameters.Add("ReportLetterYear", reportLetterYear);
            parameters.Add("ReportLetterType", (int)reportLetterType);

            string hql = @"FROM  Report R 
                           WHERE R.Account.Key = :AccountId
                             AND R.ReportLetter.ReportLetterYear = :ReportLetterYear
                             AND R.ReportLetter.ReportLetterTypeId = :ReportLetterType
                           ORDER BY R.ReportLetter.CreationDate DESC";

            List<IReport> reports = session.GetTypedListByHQL<IReport>(hql, parameters);
            if (reports.Count == 0)
                return null;
            else if (reports.Count == 1)
                return reports[0];
            else
                throw new ApplicationException(string.Format("More than one Report object found for account {0}, year {1}, and letter type '{2}'",
                                                             accountId, reportLetterYear, reportLetterType));
        }

        public static List<IReport> GetReports(IDalSession session, int reportLetterYear, ReportLetterTypes reportLetterType, int managementCompanyId)
        {
            return GetReports(session, reportLetterYear, reportLetterType, managementCompanyId, null, null);
        }

        public static List<IReport> GetReports(IDalSession session, int reportLetterYear, ReportLetterTypes reportLetterType, int[] accountIds, 
                                               ReportStatuses reportStatus)
        {
            return GetReports(session, reportLetterYear, reportLetterType, null, accountIds, reportStatus);
        }

        public static List<IReport> GetReports(IDalSession session, int reportLetterYear, ReportLetterTypes reportLetterType, 
                                               int? managementCompanyId, int[] accountIds, ReportStatuses? reportStatus)
        {
            Hashtable parameters = new Hashtable();
            Hashtable parameterLists = new Hashtable();
            
            string hql = @"FROM  Report R 
                           WHERE R.ReportLetter.ReportLetterYear = :ReportLetterYear
                             AND R.ReportLetter.ReportLetterTypeId = :ReportLetterType";

            parameters.Add("ReportLetterYear", reportLetterYear);
            parameters.Add("ReportLetterType", (int)reportLetterType);

            if (managementCompanyId != null)
            {
                parameters.Add("CompanyId", managementCompanyId);
                hql += @" AND R.Account.Key IN (SELECT AI.Key FROM AccountTypeInternal AI
                                                 WHERE AI.AccountOwner.Key = :CompanyId)";
            }

            if (accountIds != null)
            {
                if (accountIds.Length == 0)
                    accountIds = new int[] { 0 };
                parameterLists.Add("AccountIds", accountIds);
                hql += " AND R.Account.Key IN (:AccountIds)";
            }

            if (reportStatus != null)
            {
                parameters.Add("ReportStatus", (int)reportStatus);
                hql += " AND R.ReportStatusId = :ReportStatus";
            }

            hql += " ORDER BY R.Account.Number";

            return session.GetTypedListByHQL<IReport>(hql, parameters, parameterLists);
        }
        
        #region CRUD
        public static void Update(IDalSession session, IReport obj)
        {
            session.Update(obj);
        }

        public static void Insert(IDalSession session, IReport obj)
        {
            session.Insert(obj);
        }

        public static void Insert(IDalSession session, IList list)
        {
            session.Insert(list);
        }
        #endregion
    }
}

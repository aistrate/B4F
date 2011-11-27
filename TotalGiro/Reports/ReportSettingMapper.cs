using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using NHibernate;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Reports
{
    public class ReportSettingMapper
    {
        public static IReportSetting GetReportAccountSetting(IDalSession session, Int32 accountId)
        {
            return (IReportSetting)session.GetObjectInstance(typeof(ReportSetting), accountId);
        }

        public static IList GetReportAccountSettings(int[] accountIds)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            Hashtable parameters = new Hashtable();
            Hashtable parameterLists = new Hashtable(1);
            parameterLists.Add("Accounts", accountIds);

            string hql = @"from ReportSetting R 
                           Left join fetch R.Account A
                           where R.Account in (:Accounts) ";
            IList accHasReports = session.GetListByHQL(hql, parameters, parameterLists);

            session.Close();
            return accHasReports;
        }

        // Even vergeten.
        // Using Dataset on ReportSettingMapper: To get Account ReportSetting.
        public static IList GetReportAccountSettingsTesten(int[] accountIds)
        {
            IList accountReportSetting = new ArrayList();

            IDalSession session = NHSessionFactory.CreateSession();
            List<ICriterion> expressions = new List<ICriterion>();

            expressions.Add(Expression.In("Account.Key", accountIds));
            accountReportSetting = session.GetList(typeof(ReportSetting), expressions);

            session.Close();
            return accountReportSetting;

        }

    
    }
}

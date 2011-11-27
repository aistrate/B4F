using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Stichting;
using System.Collections;

namespace B4F.TotalGiro.Reports
{
    // First enum for QuarterReport.
    public enum ReportReturnClass
    {
        QuarterReport,
        FiscalYearReport,
        ResultOfPrintedReport
    }

    public class ReportTemplateMapper
    {
        public static IReportTemplate GetReportTemplate(IDalSession session, int id)
        {
            return (IReportTemplate)session.GetObjectInstance(typeof(ReportTemplate), id);
        }

        public static IReportTemplate GetReportTemplate(IDalSession session, IManagementCompany managementCompany, string reportName)
        {
            return GetReportTemplate(session, managementCompany, reportName, false);
        }

        public static IReportTemplate GetReportTemplate(
            IDalSession session, IManagementCompany managementCompany, string reportName, bool throwException)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("ManagementCompany.Key", managementCompany.Key));
            expressions.Add(Expression.Eq("ReportName", reportName));
            IList result = session.GetList(typeof(ReportTemplate), expressions);

            if ((result != null) && (result.Count > 0))
                return (IReportTemplate)result[0];
            else if (throwException)
                throw new ApplicationException(string.Format(
                    "Could not find report template name for management company '{0}' and report '{1}'.",
                    managementCompany.CompanyName, reportName));
            else
                return null;
        }

        public static List<IReportTemplate> GetReportTemplates(IDalSession session, string reportName)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("ReportName", reportName));
            return session.GetTypedList<ReportTemplate, IReportTemplate>(expressions);
        }

        #region CRUD

        /// <summary>
        /// Updates a ReportTemplate
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="list">The ReportTemplate</param>
        public static void Update(IDalSession session, IReportTemplate obj)
        {
            session.Update(obj);
        }
        
        #endregion
    }
}

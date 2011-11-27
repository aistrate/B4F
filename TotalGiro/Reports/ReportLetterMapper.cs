using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.CRM;

namespace B4F.TotalGiro.Reports
{
    public class ReportLetterMapper
    {
        public static IReportLetter GetReportLetter(IDalSession session, int id)
        {
            return (IReportLetter)session.GetObjectInstance(typeof(ReportLetter), id);
        }

        public static IList GetReportLetters(IDalSession session)
        {
            return session.GetList(typeof(ReportLetter));
        }

        public static IReportLetter GetLatestReportLetter(IDalSession session, int managementCompanyId,
                                                          ReportLetterTypes reportLetterType, int reportLetterYear)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();
            expressions.Add(Expression.Eq("ManagementCompany.Key", managementCompanyId));
            expressions.Add(Expression.Eq("ReportLetterTypeId", (int)reportLetterType));
            expressions.Add(Expression.Eq("ReportLetterYear", reportLetterYear));
            orderings.Add(Order.Desc("CreationDate"));

            List<IReportLetter> reportLetters = session.GetTypedList<ReportLetter, IReportLetter>(expressions, orderings, null, null);
            return (reportLetters.Count > 0 ? reportLetters[0] : null);
        }

        public static ReportLetterTypes GetReportLetterType(string reportLetterTypeName)
        {
            return (ReportLetterTypes)Enum.Parse(typeof(ReportLetterTypes), reportLetterTypeName, true);
        }

        public static SendableDocumentCategories ToSendableDocumentCategory(ReportLetterTypes reportLetterType)
        {
            switch (reportLetterType)
            {
                case ReportLetterTypes.Q1:
                case ReportLetterTypes.Q2:
                case ReportLetterTypes.Q3:
                case ReportLetterTypes.Q4:
                    return SendableDocumentCategories.NotasAndQuarterlyReports;
                case ReportLetterTypes.EOY:
                    return SendableDocumentCategories.YearlyReports;
                default:
                    throw new ApplicationException(string.Format("Sendable Document Category not known for Report Letter Type {0}.", 
                                                                 reportLetterType.ToString()));
            }
        }

        #region CRUD
        // Return an boolean.
        public static bool Insert(IDalSession session, IReportLetter obj)
        {
            return session.Insert(obj);
        }

        public static void Insert(IDalSession session, IList list)
        {
            session.Insert(list);
        }
        #endregion
    }
}


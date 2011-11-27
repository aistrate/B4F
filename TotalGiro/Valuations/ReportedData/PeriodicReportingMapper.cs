using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Valuations.ReportedData
{
    public class PeriodicReportingMapper
    {
        public static IPeriodicReporting GetReportingPeriod(IDalSession session, ReportingPeriodDetail reportingPeriodDetail)
        {
            Hashtable parameters = new Hashtable(2);
            if(reportingPeriodDetail.TermType == EndTermType.FullYear)
                parameters.Add("termType", EndTermType.FourthQtr);
            else
                parameters.Add("termType", reportingPeriodDetail.TermType);
            parameters.Add("endTermYear", reportingPeriodDetail.EndTermYear);

            string hql = string.Format(
                @"from PeriodicReporting P 
                where P.ReportingPeriod.TermType = :termType
                and P.ReportingPeriod.EndTermYear = :endTermYear");
            IList result = session.GetListByHQL(hql, parameters);
            if ((result != null) && (result.Count > 0))
                return (IPeriodicReporting)result[0];
            else
                return null;
        }

        public static IPeriodicReporting GetReportingPeriod(IDalSession session, int id)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", id));
            IList periods = session.GetList(typeof(PeriodicReporting), expressions);
            if (periods != null && periods.Count == 1)
                return (IPeriodicReporting)periods[0];
            else
                return null;
        }

        public static IPeriodicReporting GetLastReportingPeriod(IDalSession session)
        {
            string hql = string.Format(
                @"from PeriodicReporting A
                where A.EndTermDate in (
                Select max(PR.EndTermDate)
                from PeriodicReporting PR)");
            IList result = session.GetListByHQL(hql);
            if ((result != null) && (result.Count > 0))
                return (IPeriodicReporting)result[0];
            else
                return null;
        }

        public static IList<IPeriodicReporting> GetReportedPeriods(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            return session.GetTypedList<IPeriodicReporting>(expressions);
        }


    }
}

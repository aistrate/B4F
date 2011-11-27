using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
//using NHibernate;
//using NHibernate.Expression;
using System.Data;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.Instruments;


namespace B4F.TotalGiro.Valuations.ReportedData
{
    public class EndTermValueMapper
    {
        public static IList GetEndValues(IDalSession session, DateTime endTermDate)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("endTermDate", endTermDate);

            string hql = string.Format(
                @"from EndTermValue A 
                left join A.ReportingPeriod R
                where R.EndTermDate = :endTermDate");
            return session.GetListByHQL(hql, parameters);
        }

        public static IList<IEndTermValue> GetYearEndValues(IDalSession session, DateTime endTermDate)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("endTermDate", endTermDate);

            string hql = string.Format(
                @"From EndTermValue E
                  where E.Key in ( 
                    Select A.Key
                    from EndTermValue A 
                    left join A.ReportingPeriod R
                    where R.EndTermDate = :endTermDate)");
            return session.GetTypedListByHQL <IEndTermValue>(hql, parameters);
        }



        public static IEndTermValue GetEndValue(IDalSession session, DateTime endTermDate, IAccount account)
        {
            Hashtable parameters = new Hashtable(2);
            parameters.Add("endTermDate", endTermDate);
            parameters.Add("account", account);

            string hqlString = @"B4F.TotalGiro.Valuations.ReportedData.GetEndValue";

//            string hql = string.Format(
//                @"from EndTermValue A 
//                left join ReportingPeriod R
//                where R.EndTermDate = :endTermDate
//                and A.Account = :account");
            IList<IEndTermValue> result =  session.GetTypedListByNamedQuery<IEndTermValue>(hqlString, parameters);
            if ((result != null) && (result.Count > 0))
                return result[0];
            else
                return null;

        }

        public static IList GetEndValues(IDalSession session, IAccount account, EndTermType term, int year)
        {
            Hashtable parameters = new Hashtable(3);
            DateTime[] dates = GetEndDates(term, year);

            parameters.Add("account", account);
            parameters.Add("startDate", dates[0]);
            parameters.Add("endDate", dates[1]);

            string hql = string.Format(
                @"from EndTermValue A
                left join ReportingPeriod R
                where A.Account = :account
                and R.EndTermDate in (:startDate, :endDate)");
            return session.GetListByHQL(hql, parameters);
        }

        public static IList GetEndValues(IDalSession session, IAccountTypeInternal account, EndTermType term, int year, bool ForcedSet)
        {
            IEndTermValue getter;
            IList returnValue = new EndTermValue[2];
            DateTime[] dates = GetEndDates(term, year);
            ReportingPeriodDetail reportingPeriodDetail = new ReportingPeriodDetail(term, year);
            IPeriodicReporting reportingPeriod = PeriodicReportingMapper.GetReportingPeriod(session, reportingPeriodDetail);

            getter = GetEndValue(session, dates[0], account);
            if (getter != null)
                returnValue[0] = getter;
            else
                returnValue[0] = new EndTermValue(account, reportingPeriod, new Money(0m, account.BaseCurrency), new Money(0m, account.BaseCurrency), new Money(0m, account.BaseCurrency));

            getter = GetEndValue(session, dates[1], account);
            if (getter != null)
                returnValue[1] = getter;
            else
                returnValue[1] = new EndTermValue(account, reportingPeriod, new Money(0m, account.BaseCurrency), new Money(0m, account.BaseCurrency), new Money(0m, account.BaseCurrency));

            return returnValue;

        }


        public static DateTime[] GetEndDates(EndTermType term, int year)
        {
            DateTime[] returnValue = new DateTime[2];

            switch (term)
            {
                case EndTermType.FullYear:
                    returnValue[0] = (new DateTime(year, 1, 1)).AddDays(-1);
                    returnValue[1] = (new DateTime(year + 1, 1, 1)).AddDays(-1);
                    break;
                case EndTermType.FirstQtr:
                    returnValue[0] = (new DateTime(year, 1, 1)).AddDays(-1);
                    returnValue[1] = (new DateTime(year, 4, 1)).AddDays(-1);
                    break;
                case EndTermType.SecondQtr:
                    returnValue[0] = (new DateTime(year, 4, 1)).AddDays(-1);
                    returnValue[1] = (new DateTime(year, 7, 1)).AddDays(-1);
                    break;
                case EndTermType.ThirdQtr:
                    returnValue[0] = (new DateTime(year, 7, 1)).AddDays(-1);
                    returnValue[1] = (new DateTime(year, 10, 1)).AddDays(-1);
                    break;
                case EndTermType.FourthQtr:
                    returnValue[0] = (new DateTime(year, 10, 1)).AddDays(-1);
                    returnValue[1] = (new DateTime(year + 1, 1, 1)).AddDays(-1);
                    break;
                default:
                    break;
            }
            return returnValue;

        }

    }
}

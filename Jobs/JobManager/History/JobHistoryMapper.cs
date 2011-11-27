using System;
using System.Collections;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Stichting;
using System.Collections.Generic;

namespace B4F.TotalGiro.Jobs.Manager.History
{
    public class JobHistoryMapper
    {
        /// <summary>
        /// Either creates or edits a JobHistory object in the database
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">Object of type JobHistory</param>
        public static void InsertOrUpdate(IDalSession session, JobHistory obj)
        {
            session.InsertOrUpdate(obj);
        }

        public static IList GetJobHistoryDetails(IDalSession session, IManagementCompany company, string jobName, string componentName, DateTime startDate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable();
            string hql = "";

            if (company == null)
                throw new ApplicationException("The current company can not be null");
            else
            {
                if (!company.IsStichting)
                    hql = " where IsNull(J.ManagementCompanyID, 0) in (0," + company.Key.ToString() + ")";
            }

            if (endDate != DateTime.MinValue)
                endDate = endDate.AddDays(1).Date;

            if (jobName + string.Empty != string.Empty)
                hql = (hql == "" ? " where " : " and ") + "J.Job like '%" + jobName + "%'";

            if (componentName + string.Empty != string.Empty)
                hql += (hql == "" ? " where " : " and ") + "J.JobComponent like '%" + componentName + "%'";

            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                parameters.Add("StartTime", startDate);
                parameters.Add("EndTime", endDate);
                hql += (hql == "" ? " where " : " and ") + "J.StartTime between :StartTime and :EndTime";
            }
            else
            {
                if (startDate != DateTime.MinValue)
                {
                    hql += (hql == "" ? " where " : " and ") + "J.StartTime >= :StartTime";
                    parameters.Add("StartTime", startDate);
                }

                if (endDate != DateTime.MinValue)
                {
                    hql += (hql == "" ? " where " : " and ") + "J.StartTime <= :EndTime";
                    parameters.Add("EndTime", endDate);
                }
            }
            
            hql = "from JobHistory J " + hql;
            return session.GetListByHQL(hql, parameters);
        }

        public static IJobHistory GetJobHistoryDetail(IDalSession session, int jobHistoryID)
        {
            string hql = "";
            Hashtable parameters = new Hashtable();
            IJobHistory detail = null;

            hql = "from JobHistory J where J.Key = :key";
            parameters.Add("key", jobHistoryID);

            IList list = session.GetListByHQL(hql, parameters);
            if (list != null && list.Count == 1)
            {
                detail = (IJobHistory)list[0];
            }
            return detail;
        }
    }
}

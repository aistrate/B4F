using System;
using System.Collections.Generic;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Jobs.Manager.History;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class JobHistoryAdapter
    {
        public static DataSet GetJobHistoryDetails(string jobName, string componentName, DateTime startDate, DateTime endDate)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                JobHistoryMapper.GetJobHistoryDetails(session, company, jobName, componentName, startDate, endDate),
                "Key, Job, JobComponent, StartTime, EndTime, Status, RetryCount, Details, CreationDate");
            session.Close();

            return ds;
        }

        public static bool GetJobHistoryDetail(int key, out string details)
        {
            details = "";
            IDalSession session = NHSessionFactory.CreateSession();
            IJobHistory jobHist = JobHistoryMapper.GetJobHistoryDetail(session, key);
            if (jobHist != null)
                details = jobHist.Details;
            session.Close();

            return true;
        }
    }
}

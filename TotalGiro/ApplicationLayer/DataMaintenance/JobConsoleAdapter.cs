using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Runtime.Remoting;
using B4F.TotalGiro.Jobs.Manager;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class JobConsoleAdapter
    {
        public static DataSet GetJobReport()
        {
            DataSet ds = null;
            RemoteJobManager manager = getRemoteManager();
            if (manager != null)
                ds = manager.GetData(getCurrentManagmentCompanyDetails());
            return ds;
        }

        public static void StartJobManager()
        {
            RemoteJobManager manager = getRemoteManager();
            if (manager != null)
                manager.Start();
        }

        public static void StopJobManager()
        {
            RemoteJobManager manager = getRemoteManager();
            if (manager != null)
                manager.Stop();
        }

        public static void StartJob(int JobID)
        {
            RemoteJobManager manager = getRemoteManager();
            if (manager != null)
                manager.StartJob(getCurrentManagmentCompanyDetails(), JobID);
        }

        public static void StopJob(int JobID)
        {
            RemoteJobManager manager = getRemoteManager();
            if (manager != null)
                manager.StopJob(getCurrentManagmentCompanyDetails(), JobID);
        }

        private static RemoteJobManager getRemoteManager()
        {
            string reportUrl = ConfigurationManager.AppSettings["RemoteJobManagerUrl"];
            RemoteJobManager manager = (RemoteJobManager)RemotingServices.Connect(typeof(RemoteJobManager), reportUrl);
            return manager;
        }

        private static ManagementCompanyDetails getCurrentManagmentCompanyDetails()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            return company.Details;
        }
    }
}

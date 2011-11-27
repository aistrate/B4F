using System;
using System.Data;
using System.Collections.Generic;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Jobs.Manager
{
    public class RemoteJobManager : MarshalByRefObject, IJobManager
    {
        #region IJobManager Members

        public void Start()
        {
            RemoteJobManager.Manager.Start();
        }

        public void Stop()
        {
            RemoteJobManager.Manager.Stop();
        }

        public void StartJob(ManagementCompanyDetails companyDetails, int jobID)
        {
            RemoteJobManager.Manager.StartJob(companyDetails, jobID);
        }

        public void StopJob(ManagementCompanyDetails companyDetails, int jobID)
        {
            RemoteJobManager.Manager.StopJob(companyDetails, jobID);
        }

        public System.Data.DataSet GetData(ManagementCompanyDetails companyDetails)
        {
            return RemoteJobManager.Manager.GetData(companyDetails);
        }

        #endregion

        public static IJobManager Manager;
    }
}

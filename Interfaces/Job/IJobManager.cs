using System;
using System.Collections.Generic;
using System.Data;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Jobs.Manager
{
    public interface IJobManager
    {
        void Start();
        void Stop();
        void StartJob(ManagementCompanyDetails companyDetails, int jobID);
        void StopJob(ManagementCompanyDetails companyDetails, int jobID);
        DataSet GetData(ManagementCompanyDetails companyDetails);
    }
}

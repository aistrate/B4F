using System;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Jobs.Manager.History
{
    public class JobHistory : IJobHistory
    {
        #region Constructor

        protected JobHistory() { }

        internal JobHistory(JobComponent comp)
        {
            this.Job = comp.Job.Name;
            this.JobComponent = comp.Name;
            this.RetryCount = comp.lastIgnorableExceptionCount;
            this.StartTime = DateTime.Now;
        }

        #endregion

        #region Methods

        public void RecordJobError(JobComponentFinishedEventArgs args)
        {
            if (!args.JobComponentName.Equals(JobComponent))
                throw new ApplicationException("This is not the same job history object");

            this.Status = args.Result.Status;
            this.Details = args.Result.DetailedMessage;
            this.EndTime = args.Result.TimeFinished;
        }

        public void RecordJobFinished(JobComponentFinishedEventArgs args)
        {
            if (!args.JobComponentName.Equals(JobComponent))
                throw new ApplicationException("This is not the same job history object");

            this.Status = args.Result.Status;
            this.Details = args.Result.DetailedMessage;
            this.EndTime = args.Result.TimeFinished;
        }

        #endregion

        #region Props

        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        public int ManagementCompanyID
        {
            get { return managementCompanyID; }
            set { managementCompanyID = value; }
        }

        public DateTime CreationDate
        {
            get { return creationDate; }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            internal set { startTime = value; }
        }

        public DateTime EndTime
        {
            get { return endTime; }
            internal set { endTime = value; }
        }

        public string Job
        {
            get { return job; }
            internal set { job = value; }
        }

        public string JobComponent
        {
            get { return jobComponent; }
            internal set { jobComponent = value; }
        }

        public WorkerResultStatus Status
        {
            get { return status; }
            internal set { status = value; }
        }

        public int RetryCount
        {
            get { return retryCount; }
            set { retryCount = value; }
        }

        public string Details
        {
            get { return details; }
            internal set { details = value; }
        }

        #endregion

        #region Privates

        private int key;
        private int managementCompanyID;
        private DateTime creationDate;
        private DateTime startTime;
        private DateTime endTime = DateTime.MinValue;
        private string job;
        private string jobComponent;
        private WorkerResultStatus status;
        private int retryCount;
        private string details;

        #endregion
    }
}

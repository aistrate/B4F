using System;

namespace B4F.TotalGiro.Jobs.Manager.Notifier
{
    public class JobNotification : IJobNotification
    {
        protected JobNotification() { }

        public JobNotification(WorkerResult result)
        {
            if (result == null)
                throw new ApplicationException("Impossible to log this notification");

            this.Status = result.Status;
            this.NotificationDate = result.TimeFinished;
            this.Details = result.DetailedMessage;
            if (result.WorkerException != null)
            {
                this.ErrorDetails = result.WorkerException.Message;
                if (result.WorkerException.InnerException != null)
                    this.ErrorDetails += ": " + result.WorkerException.InnerException.Message;
            }
        }

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

        public DateTime NotificationDate
        {
            get { return notificationDate; }
            internal set { notificationDate = value; }
        }

        public WorkerResultStatus Status
        {
            get { return status; }
            internal set { status = value; }
        }

        public string Details
        {
            get { return details; }
            internal set { details = value; }
        }

        public string ErrorDetails
        {
            get { return errDetails; }
            internal set { errDetails = value; }
        }

        #endregion

        #region Privates

        private int key;
        private int managementCompanyID;
        private DateTime creationDate;
        private DateTime notificationDate;
        private WorkerResultStatus status;
        private string details;
        private string errDetails;

        #endregion

    }
}

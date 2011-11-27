using System;

namespace B4F.TotalGiro.Jobs
{
    #region Job Progress

    /// <summary>
    /// Job progress update EventArgs class
    /// </summary>
    public class JobProgressEventArgs : EventArgs 
    {
        public JobProgressEventArgs(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get { return message; }
        }

        #region Privates

        private string message;

        #endregion
	
    }

    /// <summary>
    /// The delegate for the progress updates
    /// </summary>
    /// <param name="sender">The object</param>
    /// <param name="e">The event arguments</param>
    public delegate void JobProgressEventHandler(object sender, JobProgressEventArgs e);   // delegate declaration

    #endregion

    #region Job Result

    /// <summary>
    /// Job result update EventArgs class
    /// </summary>
    public class JobResultEventArgs : JobProgressEventArgs
    {
        public JobResultEventArgs(string message, DateTime startDate, int successCount, int errorCount)
            :
            base(message)
        {
            this.startDate = startDate;
            this.endDate = DateTime.Now;
            this.successCount = successCount;
            this.errorCount = errorCount;
        }

        public DateTime StartDate
        {
            get { return startDate; }
        }

        public DateTime EndDate
        {
            get { return endDate; }
        }

        public int ErrorCount
        {
            get { return errorCount; }
        }

        public int SuccessCount
        {
            get { return successCount; }
        }
	

        #region Privates

        private DateTime startDate;
        private DateTime endDate;
        private int successCount;
        private int errorCount;

        #endregion

    }

    /// <summary>
    /// The delegate for the progress updates
    /// </summary>
    /// <param name="sender">The object</param>
    /// <param name="e">The event arguments</param>
    public delegate void JobResultEventHandler(object sender, JobResultEventArgs e);   // delegate declaration

    #endregion

    public interface IJobComponent
    {
        void Run();
        void Stop();

        event JobProgressEventHandler Progress;
        event JobResultEventHandler Finished;
    }
}

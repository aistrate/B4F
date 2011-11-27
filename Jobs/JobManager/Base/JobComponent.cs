using System;
using System.Collections;
using System.Diagnostics;
using B4F.TotalGiro.Jobs.Manager.Notifier;
using B4F.TotalGiro.Jobs.Manager.Scheduler;
using System.Threading;
using System.ComponentModel;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Jobs.Manager.History;
using log4net;

namespace B4F.TotalGiro.Jobs.Manager
{
    /// <summary>
    /// This is the .NET Agent Framework's base class for a job component.</summary>
    /// <remarks>
    /// The Job Component has a worker.</remarks>
    [Serializable]
    public class JobComponent
    {
        #region Constructor

        /// <summary>
        /// Default the JobComponent's name to the name of the JobComponent's class.</summary>
        public JobComponent()
        {
	        Name = this.GetType().Name;
        }

        #endregion

        /// <summary>
        /// This event will be fired when the job component has finished.</summary>
        public event JobComponentFinishedEventHandler Finished;

        /// <summary>
        /// This event will be fired when the job component has finished.</summary>
        public event JobComponentErrorEventHandler FinishedWithError;

        #region Props

        /// <summary>
        /// Use this TraceSwitch when tracing in derived classes.</summary>
        protected static TraceSwitch JobSwitch
        {
          get { return _jobSwitch; }
        }

        /// <summary>
        /// The id of the job.</summary>
        public int SequenceNumber
        {
            get { return _sequenceNr; }
            set { _sequenceNr = value; }
        }

        /// <summary>
        /// The name of the JobComponent.</summary>
        public string Name 
        {
          get { return _name; }
          set { _name = value; }
        }

        /// <summary>
        /// Is this component enabled.</summary>
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        /// <summary>
        /// The previous job component run should have been ok.</summary>
        public bool AbortWhenPrevJobNotOK
        {
            get { return abortWhenPrevJobNotOK; }
            set { abortWhenPrevJobNotOK = value; }
        }

        /// <summary>
        /// A pointer to the manager object
        /// </summary>
        internal AgentJob Job
        {
            get { return job; }
        }

        /// <summary>
        /// The job's worker.</summary>
        public AgentWorker Worker 
        {
          get { return _worker; }
          set { _worker = value; }
        }

        /// <summary>
        /// How much of the workers is finished
        /// </summary>
        public int ProgressPercentage
        {
            get { return progressPercentage; }
            protected set { progressPercentage = value; }
        }

	    /// <summary>
	    /// Is this job still running
	    /// </summary>
        public bool IsBusy
	    {
		    get { return isBusy;}
		    protected set { isBusy = value;}
	    }


        /// <summary>
        /// Returns a readable status of the component
        /// </summary>
        public string DisplayStatus
        {
            get 
            {
                string status = string.Empty;
                if (IsBusy)
                    status = ProgressPercentage.ToString() + "% completed";
                return status; 
            }
        }

        /// <summary>
        /// The previous result from running the worker.</summary>
        public WorkerResult LastWorkerResult 
        {
            get { return _lastWorkerResult; }
        }

        /// <summary>
        /// Returns a readable status of the component
        /// </summary>
        public string DisplayLastResult
        {
            get
            {
                string result = string.Empty;
                if (LastWorkerResult != null)
                    result = string.Format("{0}: status {1}{2}", LastWorkerResult.TimeFinished.ToString(), LastWorkerResult.Status.ToString(), (LastWorkerResult.DetailedMessage != string.Empty ? ": " + LastWorkerResult.DetailedMessage : ""));
                return result;
            }
        }


        // related to ignoring exceptions:

        /// <summary>
        /// The job's ignorable exceptions.</summary>
        public IgnorableExceptionCollection IgnorableExceptions 
        {
            get { return _ignorableExceptions; }
        }

        /// <summary>
        /// The global ignorable exceptions (usually set by the manager).</summary>
        public IgnorableExceptionCollection GlobalIgnorableExceptions 
        {
            get { return _globalIgnorableExceptions; }
            set { _globalIgnorableExceptions = (value == null) ? new IgnorableExceptionCollection() : value; }
        }

        /// <summary>
        /// Maximum number of consecutive "ignorable exceptions" to be ignored.</summary>
        public int MaximumConsecutiveExceptionIgnoreCount
        {
            get { return _maximumConsecutiveExceptionIgnoreCount; }
            set { _maximumConsecutiveExceptionIgnoreCount = value; }
        }

        /// <summary>
        /// Number of milliseconds to wait before retrying after encountering an exception.</summary>
        public long ExceptionRetryDelayMilliseconds
        {
            get { return _exceptionRetryDelayMilliseconds; }
            set { _exceptionRetryDelayMilliseconds = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the job (call before the manager starts).</summary>
        public void Init(AgentJob job)
        {
            // check sequence number
            if (SequenceNumber <= 0)
                throw new ApplicationException("Sequence Number of a job component can not be 0");
            
            this.job = job;

            // reference the manager's global ignorable exception list
            _globalIgnorableExceptions = job.Manager.IgnorableExceptions;

            // initialize & hook up the backgroundworker
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
        }

        /// <summary>
        /// Start the job.</summary>
        public void Start()
        {
            // make sure the job really has a worker
            // if the job is still running skip it
            if (_worker != null && !IsBusy && !backgroundWorker.IsBusy)
            {
                // log the that the worker is running
                Trace.WriteLineIf(_jobSwitch.TraceVerbose,
                    string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tJob\t{2}\t{3}",
                    DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Worker",
                    _worker.Description));

                // run the worker!
                IDalSessionFactory factory = job.Manager.GetSessionFactory();
                backgroundWorker.RunWorkerAsync(factory);
            }
        }

        /// <summary>
        /// Stops the job.</summary>
        public void Stop()
        {
            if (IsBusy || backgroundWorker.IsBusy || !backgroundWorker.CancellationPending)
                backgroundWorker.CancelAsync();
        }

        /// <summary>
        /// Check's to see if the worker's result is an exception that can be ignored and the work rescheduled.</summary>
        protected virtual bool CheckForIgnorableException(WorkerResult result, ref long retryDelayMilliseconds) 
        {
            // assume the worst
            bool ignorableException = true;

            // was there an exception?
            if ( result.Status != WorkerResultStatus.Exception ) 
            {
                // no exception - therefore not an ignorable exception
                ignorableException = false;
            } 
            else 
            {
                // there was an exception - search through the job's ignorable exeptions for a match
                AgentIgnorableException exception = _ignorableExceptions.Find(result.WorkerException);

                // if there was no match, search through the global ignorable exceptions for a match
                if ( exception == null )
                    exception = _globalIgnorableExceptions.Find(result.WorkerException);

                // was this an ignorable exception?
                if ( exception == null ) 
                {
                    // if there was still no match, then it was not an ignorable exception
                    ignorableException = false;
                }
                else
                {
                    // IGNORABLE EXCEPTION

                    // return the number of milliseconds to wait before re-trying after this exception
                    retryDelayMilliseconds = exception.RetryDelayMilliseconds;

                    // is this the same as the previous ignorable exception?
                    if ( lastIgnorableExceptionName != exception.Name ) 
                    {
                        // it is not the same - reset the previous name to this one and the count to 0
                        lastIgnorableExceptionName = exception.Name;
                        lastIgnorableExceptionCount = 0;
                    }

                    // increment the consecutive specific ignorable exception count
                    lastIgnorableExceptionCount++;

                    // log the ignorable exception count
                    Trace.WriteLineIf(_jobSwitch.TraceVerbose,
                        "Execpetion count: " + lastIgnorableExceptionCount.ToString());


                    // increment the consecutive total ignorable exception count
                    totalConsecutiveIgnorableExceptionsCount++;

                    // see if the consecutive counts exceeds the limits
                    if ( lastIgnorableExceptionCount > exception.MaximumConsecutiveIgnoreCount ) 
                    {
                        // specific one does exceed the limit, so it is no longer ignorable
                        ignorableException = false;
                    } 
                    else if ( totalConsecutiveIgnorableExceptionsCount > MaximumConsecutiveExceptionIgnoreCount ) 
                    {
                    // total count exceeds the limit, so it is no longer ignorable
                    ignorableException = false;
                    }
                } // exception is ignorable
            } // result is an exception

            // is this an ignorable exception?
            if ( !ignorableException ) 
            {
                // it is NOT an ignorable exeption so reset the counts and previous exception name
                totalConsecutiveIgnorableExceptionsCount = 0;
                lastIgnorableExceptionCount = 0;
                lastIgnorableExceptionName = string.Empty;
            }

            // return the final status of whether this was ignorable or not
            return ignorableException;
        }

        #endregion

        #region Backgroundworker

        // This event handler is where the actual,
        // potentially time-consuming work is done.
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Set the IsBusy flag
            isBusy = true;
            
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;
            IDalSessionFactory factory = (IDalSessionFactory)e.Argument;

            // this will store the worker's result
            WorkerResult result = _worker.Run(factory, worker, e);
            e.Result = result;
        }

        // This event handler updates the progress bar.
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressPercentage = e.ProgressPercentage;
            string progressDesc = _worker.Description + ": " + this.ProgressPercentage.ToString() + "%";

            // log the that the worker is running
            Trace.WriteLineIf(_jobSwitch.TraceVerbose,
              string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tJob\t{2}\t{3}",
              DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Worker_ProgressChanged",
              progressDesc));
        }

        // This event handler deals with the results of the
        // background operation.
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerResult result;
            // log the that the worker is running
            Trace.WriteLineIf(_jobSwitch.TraceVerbose,
              string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tJob\t{2}\t{3}",
              DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Worker",
              _worker.Description));

            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Exception, Name,
                    string.Format(_worker.MessageException, DateTime.Now, e.Error.Message),
                    e.Error);
            }
            else
            {
                try
                {
                    result = e.Result as WorkerResult;
                }
                catch (Exception) 
                {
                    // error or whatever
                    result = null;
                }

                if (e.Cancelled)
                {
                    // Next, handle the case where the user canceled the operation.
                    // Note that due to a race condition in the DoWork event handler, the Cancelled
                    // flag may not have been set, even though CancelAsync was called.
                    if (result == null)
                        result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, "JobComponent " + Name + " was cancelled");
                }
                else
                {
                    // Finally, handle the case where the operation succeeded.
                    if (result == null)
                        result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Ok, Name + " ran ok");
                }
            }

            // Check to see if the result is an ignorable exception and if so when the worker should be rescheduled
            long retryDelayMilliseconds = 0;
            bool ignoreException = CheckForIgnorableException(result, ref retryDelayMilliseconds);

            // log the worker result
            Trace.WriteLineIf(_jobSwitch.TraceInfo,
              string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tJob\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
              DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Worker",
              _worker.Description, result.Status,
              (result.WorkerException == null) ? null : result.WorkerException.GetType(),
              (result.WorkerException == null) ? string.Empty : result.WorkerException.Message,
              ignoreException ? "IGNORE" : "NOTIFY"));

            // determine how to proceed
            if (ignoreException)
            {
                // Tell the Job that this jobComponent has finished with an error
                FinishedWithError(this, new JobComponentFinishedEventArgs(this, result));

                // this was an exception that can be ignored, but we need to reschedule
                // the job with the scheduler that called us
                AgentScheduler scheduler = this.Job.Schedulers.GetLastScheduled();
                if (scheduler != null)
                {
                    TimeSpan retryDelayTimeSpan = new TimeSpan(TimeSpan.TicksPerMillisecond * retryDelayMilliseconds);
                    scheduler.RequestRescheduling(retryDelayTimeSpan);
                }
            }
            else
            {
                // this is a normal worker result that is not ignored, so let's request notification
                foreach (AgentNotifier notifier in Job.manager.Notifiers)
                    notifier.RequestNotification(result);

                // Tell the Job that this jobComponent has finished
                Finished(this, new JobComponentFinishedEventArgs(this, result));
            }

            progressPercentage = 0;
            isBusy = false;
            // store this result as the previous result
            _lastWorkerResult = result;

        }

        #endregion

        #region Privates

        // trace switch for logging
        private static TraceSwitch _jobSwitch = new TraceSwitch("AgentJob", "AgentJob TraceSwitch");

        // the sequence & name of the job component
        private int _sequenceNr;
        private string _name;

        // is this component enabled
        private bool isEnabled = true;

        // the previous job component run should have been ok
        private bool abortWhenPrevJobNotOK;

        // the job's that owns the component
        private AgentJob job;

        // the job's worker
        private AgentWorker _worker = null;

        // the previous result from running the worker
        private WorkerResult _lastWorkerResult = null;

        private bool isBusy;
        private int progressPercentage;

        // related to ignoring exceptions:

        // maximum number of consecutive "ignorable exceptions" to be ignored
        private int _maximumConsecutiveExceptionIgnoreCount = 3;

        // number of milliseconds to wait before retrying after encountering an exception
        private long _exceptionRetryDelayMilliseconds = 20000;

        // the job's ignorable exceptions
        private IgnorableExceptionCollection _ignorableExceptions = new IgnorableExceptionCollection();

        // the global ignorable exceptions (usually in the manager)
        private IgnorableExceptionCollection _globalIgnorableExceptions = null;

        // current count of consecutive "ignorable exceptions" that have been encountered
        private int totalConsecutiveIgnorableExceptionsCount = 0;

        // count of consecutive "ignorable exceptions" encountered for most recent specific exception
        internal int lastIgnorableExceptionCount = 0;

        // name of most recent specific exception
        private string lastIgnorableExceptionName = string.Empty;

        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        #endregion
    
    }

    /// <summary>
    /// This is the .NET Agent Framework's job component collection.</summary>
    /// <remarks>
    /// JobComponentCollection derives from CollectionBase.</remarks>
    [Serializable]
    public class JobComponentCollection : CollectionBase 
    {
        /// <summary>
        /// Add a job component to the collection.</summary>
        public void Add(JobComponent jobComponent) 
        {
            List.Add(jobComponent);
        }

        /// <summary>
        /// Remove a job component from the collection.</summary>
        public void Remove(JobComponent jobComponent)
        {
            List.Remove(jobComponent);
        }

        /// <summary>
        /// Set or retrieve a job component at the specific index in the collection.</summary>
        public JobComponent this[int index] 
        {
	        get 
	        {
                return (JobComponent)List[index];
	        }
	        set 
	        {
		        List[index] = value;
	        }
        }
    }

    #region EventHandler

    public class JobComponentFinishedEventArgs : System.EventArgs
    {
        public JobComponentFinishedEventArgs(JobComponent jobComp, WorkerResult result)
            : base()
        {
            this._sequenceNr = jobComp.SequenceNumber;
            this.jobComponentName = jobComp.Name;
            if (result == null)
                throw new ApplicationException("The result of the job can not be null");
            else if (result.Status == WorkerResultStatus.Ok)
                isOK = true;
            this.result = result;
        }

        public int JobSequenceNumber
        {
            get { return _sequenceNr; }
        }

        public string JobComponentName
        {
            get { return jobComponentName; }
        }

        public bool IsOK
        {
            get { return isOK; }
        }

        public WorkerResult Result
        {
            get { return result; }
        }
	

        private int _sequenceNr;
        private string jobComponentName;
        private bool isOK = false;
        private WorkerResult result;
    }

    public delegate void JobComponentFinishedEventHandler(object sender, JobComponentFinishedEventArgs e);
    public delegate void JobComponentErrorEventHandler(object sender, JobComponentFinishedEventArgs e);

    #endregion
}

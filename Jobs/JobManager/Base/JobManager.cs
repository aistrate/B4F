using System;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.Data;
using System.IO;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Jobs.Manager.Notifier;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Jobs.Manager
{
    /// <summary>
    /// This is the .NET Agent Framework's base class for a manager.</summary>
    /// <remarks>
    /// The JobManager has a collection of jobs and checks the jobs'
    /// schedules at regular intervals. The Start and Stop methods are
    /// used to control the manager, and the Init method may also be
    /// called directly. Classes that inherit from this class may override
    /// OnInit, OnStart, and OnStop to implement custom code.</remarks>
    [Serializable]
    public class JobManager : MarshalByRefObject, IJobManager
    {
        #region Constructor

        /// <summary>
        /// Intializes the timer used by the manager.</summary>
        public JobManager()
        {
            // initializes NHSessionFactory
            factory = NHSessionFactory.GetInstance();

            _notifiers = new AgentNotifierCollection(this);

            // initialize the Timer control
            _timer = new System.Timers.Timer();
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.AutoReset = true;
        }

        #endregion

        public static JobManager GetInstance()
        {
            JobManager manager = null;
            Trace.Listeners.Add(new TextWriterTraceListener());

            DynamicXmlObjectLoader loader = new DynamicXmlObjectLoader();
            string path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf(@"\")) + @"\jobs.config";
            FileInfo fInfo = new FileInfo(path);
            if (fInfo.Exists)
            {
                manager = (JobManager)loader.Load(path);
            }
            return manager;
        }
        
        /// <summary>
        /// Use this TraceSwitch when tracing in derived classes.</summary>
        protected static TraceSwitch ManagerSwitch 
        {
            get { return _managerSwitch; }
        }

        /// <summary>
        /// True when the manager has started.</summary>
        public bool IsRunning 
        {
            get { return _isRunning; }
        }

        public DateTime HeartBeat
        {
            get { return _heartBeat; }
            protected set { _heartBeat = value; }
        }

        /// <summary>
        /// Determine the interval at which the job schedules are checked.</summary>
        public TimeSpan HeartbeatFrequency 
        {
            get { return TimeSpan.FromMilliseconds(_timer.Interval); }
            set { _timer.Interval = value.TotalMilliseconds; }
        }

        /// <summary>
        /// Returns the collection of jobs under management.</summary>
        public AgentJobCollection Jobs 
        {
            get { return _jobs; }
        }

        /// <summary>
        /// Collection of the job's notifiers.</summary>
        public AgentNotifierCollection Notifiers
        {
            get { return _notifiers; }
        }

        /// <summary>
        /// Returns the collection of globally ignorable exceptions.</summary>
        public IgnorableExceptionCollection IgnorableExceptions
        {
            get { return _ignorableExceptions; }
        }

        /// <summary>
        /// Determines whether or not the GC should be manually invoked after a job completes.</summary>
        internal bool CollectGarbage
        {
            get { return _collectGarbage; }
            set { _collectGarbage = value; }
        }

        internal IDalSessionFactory GetSessionFactory()
        {
            SecurityManager.Initialize(SecuritySetupType.InMemory);
            SecurityManager.SetUser("manual", string.Empty);

            if (factory == null)
                factory = NHSessionFactory.GetInstance();
            return factory;
        }

        internal IDalSession GetSession()
        {
            return GetSessionFactory().CreateSession();
        }

        /// <summary>
        /// Intializes the manager, including calling OnInit.</summary>
        private void Init() 
        {
            // Make sure we have not already initialized
            if ( _isInitialized )
                throw(new InvalidOperationException("Already initialized."));

            // logging
            Trace.WriteLineIf(_managerSwitch.TraceInfo,
                string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Init()") );

            // check HeartbeatFrequency
            if (HeartbeatFrequency.Equals(new TimeSpan(0,0,0)))
                HeartbeatFrequency = new TimeSpan(0,0,30);

            // derived classes may override OnInit() with their own initialization code
            OnInit();

            // Initialize jobs
            foreach (AgentJob job in _jobs)
                job.Init(this);

            // record that Init() has been called
            _isInitialized = true;
        }

        /// <summary>
        /// Starts the manager, including calling OnStart.
        /// </summary>
        public void Start() 
	    {    
            // Make sure we have not already started
	        if ( _isRunning ) 
	            throw(new InvalidOperationException("Already started."));

            // logging
            Trace.WriteLineIf(_managerSwitch.TraceInfo,
                string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Start()"));

            // initialize, if not already done
            if ( !_isInitialized )
                Init();

            // hook for derived classes to add custom Start code
            OnStart();

            // note status change
            _isRunning = true;

            // reset Stop events
            _stopSignal.Reset();

            // start the heartbeat timer
            _timer.Interval = HeartbeatFrequency.TotalMilliseconds;
            _timer.Start();

            // logging
            Trace.WriteLineIf(_managerSwitch.TraceInfo,
                string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "STARTED"));
        }

        /// <summary>
        /// Stops the manager, including calling OnStop.</summary>
        public void Stop() 
	    {
            Trace.WriteLineIf(_managerSwitch.TraceInfo,
                string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Stop()"));

            if ( !_isRunning ) 
                throw(new InvalidOperationException("Not started."));

            _stopSignal.Set();
            _timer.Stop();

            foreach (AgentJob job in this.Jobs)
                job.Stop();

            _isRunning = false;

            // hook for derived classes to add custom Stop code
            OnStop();
            	
            Trace.WriteLineIf(_managerSwitch.TraceInfo,
                string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "STOPPED"));
        }

        /// <summary>
        /// Starts a specific job by it's id.
        /// </summary>
        public void StartJob(ManagementCompanyDetails companyDetails, int jobID)
        {
            Trace.WriteLineIf(_managerSwitch.TraceInfo,
                string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "StartJob()"));

            if (!_isRunning)
                throw (new InvalidOperationException("Job Manager not started."));

            foreach (AgentJob job in this.Jobs)
            {
                if (job.ID.Equals(jobID))
                {
                    if (job.IsJobRelevant(companyDetails))
                    {
                        job.Start();
                        Trace.WriteLineIf(_managerSwitch.TraceInfo,
                            string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}",
                            DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Job Started"));
                    }
                }
            }
        }

        /// <summary>
        /// Stops a specific job by it's id.
        /// </summary>
        public void StopJob(ManagementCompanyDetails companyDetails, int jobID)
        {
            Trace.WriteLineIf(_managerSwitch.TraceInfo,
                string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "StopJob()"));

            if (!_isRunning)
                throw (new InvalidOperationException("Job Manager not started."));

            foreach (AgentJob job in this.Jobs)
            {
                if (job.ID.Equals(jobID))
                {
                    if (job.IsJobRelevant(companyDetails))
                    {
                        job.Stop();
                        Trace.WriteLineIf(_managerSwitch.TraceInfo,
                            string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}",
                            DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Job STOPPED"));
                    }
                }
            }
        }

        /// <summary>
        /// Repeatedly called to check the jobs' schedules.</summary>
        protected void Timer_Elapsed(object sender, ElapsedEventArgs args) 
        {
            HeartBeat = DateTime.Now;
          
            // The Timer is multi-threaded and the Elapsed event might might be called
            // even after the Stop method has been called. 
            if ( !_stopSignal.WaitOne(0, true) ) 
            {
                Trace.WriteLineIf(_managerSwitch.TraceVerbose,
                string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "HEARTBEAT-IN"));

                bool anyJobsScheduled = false;
                try 
                {
                    foreach(AgentJob job in _jobs) 
                        if ( job.CheckSchedules() )
                            anyJobsScheduled = true;

                    if ( anyJobsScheduled && CollectGarbage )
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers(); 
                    }
                } 
                catch (Exception e) 
                {
                    Trace.WriteLineIf(_managerSwitch.TraceVerbose,
                    string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}[{3}]",
                    DateTime.Now, Thread.CurrentThread.ManagedThreadId, "HEARTBEAT-EXC", e));
                }

                Trace.WriteLineIf(_managerSwitch.TraceVerbose,
                string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tManager\t{2}[{3}]",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "HEARTBEAT-OUT", anyJobsScheduled));
            }
        }

        /// <summary>
        /// This method creates a dataset and fills it with the current status of all the jobs
        /// </summary>
        /// <returns>a dataset</returns>
        protected DataSet generateReportDataSet(ManagementCompanyDetails companyDetails)
        {
            DataSet ds = new DataSet("Manager Report");
            DataTable tMan = ds.Tables.Add("Manager");
            tMan.Columns.Add("IsRunning", typeof(Boolean));
            tMan.Columns.Add("Heartbeat", typeof(String));
            
            DataTable tJobs = ds.Tables.Add("Jobs");
            tJobs.Columns.Add("JobID", typeof(Int32));
            tJobs.Columns.Add("Name", typeof(String));
            tJobs.Columns.Add("Status", typeof(String));
            tJobs.Columns.Add("IsBusy", typeof(Boolean));
            tJobs.Columns.Add("LastScheduled", typeof(DateTime));
            tJobs.Columns.Add("NextScheduled", typeof(DateTime));
            tJobs.Columns.Add("Components", typeof(Int32));

            DataTable tComp = ds.Tables.Add("Components");
            tComp.Columns.Add("JobID", typeof(Int32));
            tComp.Columns.Add("SequenceNumber", typeof(Int32));
            tComp.Columns.Add("Name", typeof(String));
            tComp.Columns.Add("IsEnabled", typeof(Boolean));
            tComp.Columns.Add("Status", typeof(String));
            tComp.Columns.Add("LastResult", typeof(String));

            ds.Relations.Add("rel_Jobs_Components", tJobs.Columns["JobID"], tComp.Columns["JobID"]);

            if (_isInitialized)
            {
                tMan.Rows.Add(new object[] { IsRunning, HeartBeat.ToLongTimeString() } );
                
                foreach (AgentJob job in this._jobs)
                {
                    if (job.IsJobRelevant(companyDetails))
                    {
                        tJobs.Rows.Add(new object[]
                        {
                            job.ID,
                            job.Name,
                            job.DisplayStatus,
                            job.IsBusy,
                            job.LastScheduled,
                            job.NextScheduled,
                            job.JobComponents.Count
                        });

                        foreach (JobComponent comp in job.JobComponents)
                        {
                            tComp.Rows.Add(new object[]
                            {
                                comp.Job.ID,
                                comp.SequenceNumber,
                                comp.Name,
                                comp.IsEnabled,
                                comp.DisplayStatus,
                                comp.DisplayLastResult
                            });
                        }
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// Retrieves the current status of all the jobs
        /// </summary>
        /// <returns>A serialized dataset</returns>
        public DataSet GetData(ManagementCompanyDetails companyDetails)
        {
            //System.IO.StringWriter sw = new System.IO.StringWriter();
            //generateReportDataSet().WriteXml(sw, XmlWriteMode.WriteSchema);
            //sw.Flush();
            //return sw.ToString();
            return generateReportDataSet(companyDetails);
        }

        /// <summary>
        /// Provides a hook implement custom initialization code.</summary>
        protected virtual void OnInit() { }
        	
        /// <summary>
        /// Provides a hook implement custom start code.</summary>
        protected virtual void OnStart() { }
        	
        /// <summary>
        /// Provides a hook implement custom stop code.</summary>
        protected virtual void OnStop() { }

        #region Privates

        private NHSessionFactory factory;
        
        // for logging & debugging
        private static TraceSwitch _managerSwitch = new TraceSwitch("JobManager", "JobManager TraceSwitch");

        // current state of the manager
        private bool _isInitialized = false;
        private bool _isRunning = false;
        private DateTime _heartBeat = DateTime.Now;

        // timer used to fire events to check the schedules
        private System.Timers.Timer _timer = new System.Timers.Timer();

        // The ManualResetEvent allows threads to communicate with each other.
        // In this case, I use it to signal that the Manager's Stop method
        // has been called so that the Timer's Elapsed event know whether or
        // not to do anything.
        private ManualResetEvent _stopSignal = new ManualResetEvent(false);

        // collection of Jobs loaded into the manager
        private AgentJobCollection _jobs = new AgentJobCollection();

        // collection of the job's notifiers
        private AgentNotifierCollection _notifiers;

        // collection of Exceptions (global; not job-level) loaded into the manager
        private IgnorableExceptionCollection _ignorableExceptions = new IgnorableExceptionCollection();

        // set/clear whether or not to collect garbage
        private bool _collectGarbage = true;

        #endregion
    
    }
}

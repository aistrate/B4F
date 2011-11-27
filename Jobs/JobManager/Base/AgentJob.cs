using System;
using System.Collections;
using System.Diagnostics;
using B4F.TotalGiro.Jobs.Manager.Notifier;
using B4F.TotalGiro.Jobs.Manager.Scheduler;
using System.Threading;
using System.ComponentModel;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Jobs.Manager.History;
using B4F.TotalGiro.Stichting;
using log4net;

namespace B4F.TotalGiro.Jobs.Manager
{
    /// <summary>
    /// This is the .NET Agent Framework's base class for a job.</summary>
    /// <remarks>
    /// The AgentJob has a collection of schedulers, a collection of notifiers,
    /// and a worker.</remarks>
    [Serializable]
    public class AgentJob
    {
        #region Constructor

        /// <summary>
        /// Default the job's name to the name of the job's class.</summary>
        public AgentJob()
        {
	        Name = this.GetType().Name;
            _schedulers = new AgentSchedulerCollection(this);
        }

        #endregion

        #region Props

        /// <summary>
        /// Use this TraceSwitch when tracing in derived classes.</summary>
        protected static TraceSwitch JobSwitch
        {
          get { return _jobSwitch; }
        }

        /// <summary>
        /// The id of the job.</summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The name of the job.</summary>
        public string Name 
        {
          get { return _name; }
          set { _name = value; }
        }

        /// <summary>
        /// The Management Company that owns this Job. If 0 every management company can see this job.
        /// </summary>
        public int ManagementCompanyId
        {
            get { return _managementCompanyId; }
            set { _managementCompanyId = value; }
        }

        /// <summary>
        /// A pointer to the manager object
        /// </summary>
        internal JobManager Manager
        {
            get { return manager; }
        }

        /// <summary>
        /// Collection of the job's schedulers.</summary>
        public AgentSchedulerCollection Schedulers 
        {
          get { return _schedulers; }
        }

        /// <summary>
        /// Collection of the job Components.</summary>
        public JobComponentCollection JobComponents
        {
            get { return _jobComponents; }
        }

	    /// <summary>
	    /// Is this job still running
	    /// </summary>
        public bool IsBusy
	    {
		    get 
            {
                bool isBusy = false;
                foreach (JobComponent comp in JobComponents)
                {
                    if (comp.IsBusy)
                    {
                        isBusy = true;
                        break;
                    }
                }
                return isBusy;
            }
	    }

        /// <summary>
        /// Returns a readable status of the Job
        /// </summary>
        public string DisplayStatus
        {
            get
            {
                string status = string.Empty;
                foreach (JobComponent comp in JobComponents)
                {
                    if (comp.IsBusy)
                    {
                        status = comp.Name + " is running";
                        break;
                    }
                }
                return status;
            }
        }

        /// <summary>
        /// Returns a the next time that the job will run
        /// </summary>
        public DateTime NextScheduled
        {
            get 
            {
                DateTime nextTime = DateTime.MaxValue;
                foreach (AgentScheduler scheduler in Schedulers)
                {
                    if (scheduler.NextScheduled < nextTime)
                        nextTime = scheduler.NextScheduled;
                }

                return nextTime; 
            }
        }

        /// <summary>
        /// Returns a the next time that the job will run
        /// </summary>
        public DateTime LastScheduled
        {
            get 
            {
                DateTime lastTime = DateTime.MinValue;
                foreach (AgentScheduler scheduler in Schedulers)
                {
                    if (scheduler.LastScheduled > lastTime)
                        lastTime = scheduler.LastScheduled;
                }
                return lastTime; 
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the job (call before the manager starts).</summary>
        public void Init(JobManager manager)
        {
            this.manager = manager;

            // Hook up Scheduler_OnScheduled to all Scheduler events.
            foreach(AgentScheduler scheduler in _schedulers)
                scheduler.Scheduled += new EventHandler(Scheduler_OnScheduled);

            // Initialize job components
            foreach (JobComponent comp in JobComponents)
            {
                comp.Init(this);
                comp.Finished += new JobComponentFinishedEventHandler(Jobcomponent_OnFinished);
                comp.FinishedWithError += new JobComponentErrorEventHandler(Jobcomponent_OnFinishedWithError);
            }
        }

        /// <summary>
        /// Starts the job.</summary>
        internal void Start()
        {
            if (!IsBusy)
                KickOffComponent(0, true);
        }


        /// <summary>
        /// Stops the job.</summary>
        public void Stop()
        {
            // Stop the job components
            foreach (JobComponent comp in JobComponents)
                comp.Stop();
        }

        /// <summary>
        /// Check's the job's schedules.</summary>
        public bool CheckSchedules() 
        {
            bool scheduled = false;

            if (!IsBusy)
            {
                foreach (AgentScheduler scheduler in _schedulers)
                    if (scheduler.CheckSchedule())
                        scheduled = true;
            }
            return scheduled;
        }

        /// <summary>
        /// This method is is called when a scheduler determines that a job should be run.</summary>
        protected void Scheduler_OnScheduled(object sender, EventArgs args) 
        {
            if (!IsBusy)
            {
                // Tell Scheduler that event was successfull
                ((AgentScheduler)sender).ScheduledSuccessFully();
                // kick of component
                KickOffComponent(0, true);
            }
            else
            {
                // log that the job was not fired
                log.Warn(string.Format("The job {0} was not fired at {1} since a previous started instance was still running.", Name, DateTime.Now));
            }
        }

        /// <summary>
        /// This method is is called when a jobcomponent has finished witn an error.</summary>
        protected void Jobcomponent_OnFinishedWithError(object sender, JobComponentFinishedEventArgs args)
        {
            try
            {
                // log in database
                if (history != null)
                {
                    history.RecordJobError(args);
                    JobHistoryMapper.InsertOrUpdate(Manager.GetSession(), history);
                    log.Info(string.Format("Error in job component {0}", Name));
                }

                // kick of next component
                KickOffComponent(args.JobSequenceNumber, args.IsOK);
            }
            catch (Exception ex)
            {
                log.Error("An error occured during the finishing of a component", ex);
            }
        }


        /// <summary>
        /// This method is is called when a jobcomponent has finished. Then the next jobcomponent will be fired</summary>
        protected void Jobcomponent_OnFinished(object sender, JobComponentFinishedEventArgs args)
        {
            try
            {
                // log in database
                if (history != null)
                {
                    history.RecordJobFinished(args);
                    JobHistoryMapper.InsertOrUpdate(Manager.GetSession(), history);
                    log.Info(string.Format("Finish job component {0}", args.JobComponentName));
                }

                // kick of next component
                KickOffComponent(args.JobSequenceNumber, args.IsOK);
            }
            catch (Exception ex)
            {
                log.Error("An error occured during the finishing of a component", ex);
            }
        }

        /// <summary>
        /// This method starts the next job component
        /// </summary>
        /// <param name="prevSequence">The previous job component that finished (0 when the first has to start).</param>
        /// <param name="lastRunOK">Did the previous job run ok.</param>
        protected void KickOffComponent(int prevSequence, bool lastRunOK)
        {
            try
            {
                foreach (JobComponent comp in JobComponents)
                {
                    if (comp.SequenceNumber > prevSequence && comp.IsEnabled)
                    {
                        if (lastRunOK || (!lastRunOK && !comp.AbortWhenPrevJobNotOK))
                        {
                            history = new JobHistory(comp);
                            JobHistoryMapper.InsertOrUpdate(Manager.GetSession(), history);
                            comp.Start();
                            log.Info(string.Format("Start job component {0}", comp.Name));
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("An error occured during kicking off a new component", ex);
            }
        }

        public bool IsJobRelevant(ManagementCompanyDetails companyDetails)
        {
            bool isRelevant = false;
            if (this.ManagementCompanyId == 0 || companyDetails.IsStichting || companyDetails.Key.Equals(this.ManagementCompanyId))
                isRelevant = true;
            return isRelevant;
        }


        #endregion


        #region Privates

        // trace switch for logging
        private static TraceSwitch _jobSwitch = new TraceSwitch("JobComponent", "JobComponent TraceSwitch");

        // the id & name of the job
        private int _id;
        private string _name;
        private int _managementCompanyId = 0;

        // collection of the job's schedulers
        private AgentSchedulerCollection _schedulers;

        // collection of the job components
        private JobComponentCollection _jobComponents = new JobComponentCollection();

        // the job's manager
        internal JobManager manager;

        // the last jobhistory log
        private JobHistory history;

        // the log4net logger
        private static readonly ILog log = LogManager.GetLogger("System");

        #endregion
    
    }

    /// <summary>
    /// This is the .NET Agent Framework's job collection.</summary>
    /// <remarks>
    /// AgentJobCollection derives from CollectionBase.</remarks>
    [Serializable]
    public class AgentJobCollection : CollectionBase 
    {
        /// <summary>
        /// Add a job to the collection.</summary>
        public void Add(AgentJob job) 
        {
	        List.Add(job);
        }

        /// <summary>
        /// Remove a job from the collection.</summary>
        public void Remove(AgentJob job)
        {
	        List.Remove(job);
        }

        /// <summary>
        /// Set or retrieve a job at the specific index in the collection.</summary>
        public AgentJob this[int index] 
        {
	        get 
	        {
		        return (AgentJob) List[index];
	        }
	        set 
	        {
		        List[index] = value;
	        }
        }
    }
}

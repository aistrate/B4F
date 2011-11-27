using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using log4net;

namespace B4F.TotalGiro.Jobs.Manager.Notifier
{

    /// <summary>
    /// This is the .NET Agent Framework's base class for a notifier.</summary>
    /// <remarks>
    /// The AgentNotifier defines a Scheduled event and a method for firing that
    /// event. It also provides abstract methods that must be implemented in
    /// derived classes.</remarks>
    [Serializable]
    public abstract class AgentNotifier
    {
        /// <summary>
        /// Use this TraceSwitch when tracing in derived classes.</summary>
        protected static TraceSwitch NotifierSwitch 
        {
          get { return _notifierSwitch; }
        }

        /// <summary>
        /// Determines for which worker statuses a notification should be sent.</summary>
        public WorkerResultStatus NotifyOnWorkerResultStatus 
        {
          get { return _notifyOnWorkerResultStatus; }
          set { _notifyOnWorkerResultStatus = value; }
        }

        /// <summary>
        /// Determines the maximum frequency with which notifications for the
        /// same state should be sent. In other words, this is the minimum
        /// amount of time that must pass before another notification is sent
        /// for the same state.</summary>
        public TimeSpan MaxNotificationFrequency 
        {
          get { return _maxNotificationFrequency; }
          set { _maxNotificationFrequency = value; }
        }

        /// <summary>
        /// Sets the maximum number of notifications that should be sent
        /// in a given period of time, MaxNotificationPeriod.</summary>
        public int MaxNotificationCountInPeriod 
        {
          get { return _maxNotificationCountInPeriod; }
          set { _maxNotificationCountInPeriod = value; }
        }

        /// <summary>
        /// The period of time in which the maximum number of notifications,
        /// MaxNotificationCountInPeriod, applies.</summary>
        public TimeSpan MaxNotificationPeriod 
        {
          get { return _maxNotificationPeriod; }
          set { _maxNotificationPeriod = value; }
        }

        /// <summary>
        /// This method must be implemented by the derived class and must
        /// do the actual notification.</summary>
        protected abstract void Notify(WorkerResult result);


        /// <summary>
        /// Default constructor.</summary>
        public AgentNotifier() { }


        /// <summary>
        /// This method is called by the job after the worker is run.
        /// The code should determine, based on the worker result,
        /// whether or not a notification should be sent.</summary>
        public void RequestNotification(WorkerResult result) 
        {
          bool notify = false;

          // If the status warrants notification and no notification was sent for this status in the allotted time period, send it.
          if ( ShouldNotify(result.Status) &&	(DateTime.Now.Subtract(LastNotified(result.State)) >= MaxNotificationFrequency) ) 
            notify = true;

            // If the PREVIOUS status warrants notification and the state has changed, send this notification too.
          else if ( (_lastWorkerResult != null) && ShouldNotify(_lastWorkerResult.Status) && (result.State != _lastWorkerResult.State) )
            notify = true;

          // no matter what, do not exceed this many notifications in the specified period
          if ( CheckPeriodNotificationsCount() >= MaxNotificationCountInPeriod )
            notify = false;

          // NOTIFY?
          if ( notify )
          {
            _lastWorkerResult = result;
            _lastNotifiedByState[result.State] = DateTime.Now;
            _notificationHistory.Add(DateTime.Now);
            FireNotify(result);
          }
        }

        /// <summary>
        /// This method is called when RequestNotify determines that notification should take place.</summary>
        protected void FireNotify(WorkerResult result) 
        {
          try 
          {
            Notify(result);
            if ( NotifierSwitch.TraceVerbose )
              System.Diagnostics.Trace.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tNotifier\t{2}\t{3}\t{4}",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Notify()",
                this.GetType(), "OK" ));
          } 
          catch (Exception e) 
          {
              // log that the Notifier was not fired
              log.Error(string.Format("The Notifier {0} was not fired: {1}", this.ToString(), e.Message));

              if (NotifierSwitch.TraceError)
              System.Diagnostics.Trace.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}\t{1}\tNotifier\t{2}\t{3}\t{4}",
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, "Notify()",
                this.GetType(), e.Message ));
          }
        }


        /// <summary>
        /// Returns true if the specified result status is a status
        /// that warrants notification.</summary>
        protected virtual bool ShouldNotify(WorkerResultStatus status) 
	        {
          // compare the status to the notification statuses
		        return (NotifyOnWorkerResultStatus & status) != 0;
	        }


        /// <summary>
        /// Returns the last date/time that notification was sent
        /// for a particular state.</summary>
        protected DateTime LastNotified(int state) 
	        {
		        object lastNotified = _lastNotifiedByState[state];
		        if ( lastNotified == null )
			        return DateTime.MinValue;
		        else
			        return (DateTime) lastNotified;
	        }


        /// <summary>
        /// Returns the number of notifications that have been sent in the notification period.</summary>
        protected int CheckPeriodNotificationsCount() 
        {
            // determine when the current notification period started
	        DateTime periodStart = DateTime.Now.Subtract( MaxNotificationPeriod );
            int notificationsInPeriodCount = 0;

             // look at history of all notifications sent
	        for( int i=0; i < _notificationHistory.Count; ) 
	        {
		        if ( ((DateTime) _notificationHistory[i]) >= periodStart ) 
		        {
			        //Notifications in the period go to the total count
			        notificationsInPeriodCount++;
			        i++;
		        }
		        else 
		        {
			        //remove older ones
			        _notificationHistory.RemoveAt(i);
		        }
	        }

	        return notificationsInPeriodCount;
        }

        #region Privates

        private static TraceSwitch _notifierSwitch = new TraceSwitch("AgentNotifier", "AgentNotifier TraceSwitch");

        internal JobManager Parent;
        private WorkerResultStatus _notifyOnWorkerResultStatus = WorkerResultStatus.All;
        private TimeSpan _maxNotificationPeriod = TimeSpan.FromDays(1);
        private TimeSpan _maxNotificationFrequency = TimeSpan.FromDays(1);
        private int _maxNotificationCountInPeriod = 10;

        // the previous worker result
        private WorkerResult _lastWorkerResult = null;

        // lookup table to store the last notification by state
        private Hashtable _lastNotifiedByState = new Hashtable();

        // history of notification date/times
        private ArrayList _notificationHistory = new ArrayList();

        // the log4net logger
        private static readonly ILog log = LogManager.GetLogger("System");

        #endregion

    }//class


    /// <summary>
    /// This is the .NET Agent Framework's notifier collection.</summary>
    /// <remarks>
    /// AgentNotifierCollection derives from CollectionBase.</remarks>
    [Serializable]
    public class AgentNotifierCollection : CollectionBase 
    {
        public AgentNotifierCollection(JobManager manager)
        {
            Parent = manager;
        }

        /// <summary>
        /// Add a notifier to the collection.</summary>
        public void Add(AgentNotifier notifier) 
        {
            notifier.Parent = Parent;
            List.Add(notifier);
        }

        /// <summary>
        /// Remove a notifier from the collection.</summary>
        public void Remove(AgentNotifier notifier)
        {
            notifier.Parent = null;
            List.Remove(notifier);
        }

        /// <summary>
        /// Set or retrieve a notifier at the specific index in the collection.</summary>
        public AgentNotifier this[int index] 
        {
            get 
            {
                return (AgentNotifier) List[index];
            }
            set 
            {
                List[index] = value;
            }
        }

        public JobManager Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        #region Privates

        private JobManager parent;

        #endregion
    }
}

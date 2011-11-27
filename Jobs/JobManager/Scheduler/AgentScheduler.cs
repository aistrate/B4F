using System;
using System.Collections;
using System.Diagnostics;

namespace B4F.TotalGiro.Jobs.Manager.Scheduler
{
    [Serializable]
    [Flags]
    public enum ScheduleDaysOfWeek
    {
        Monday = 0x00000001,
        Tuesday = 0x00000002,
        Wednesday = 0x00000004,
        Thursday = 0x00000008,
        Friday = 0x00000010,
        Saturday = 0x00000020,
        Sunday = 0x00000040,
        Everyday = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
        Weekends = Saturday | Sunday
    };

    /// <summary>
    /// This is the .NET Agent Framework's base class for a scheduler.</summary>
    /// <remarks>
    /// The AgentScheduler defines a Scheduled event and a method for firing that
    /// event. It also provides abstract methods that must be implemented in
    /// derived classes.</remarks>
    [Serializable]
    public abstract class AgentScheduler
    {
        public DateTime LastScheduled;
        public abstract DateTime NextScheduled { get; }
        public ScheduleDaysOfWeek DaysOfWeek = ScheduleDaysOfWeek.Everyday;
        internal AgentJob Parent;

        /// <summary>
        /// Use this TraceSwitch when tracing in derived classes.</summary>
        protected static TraceSwitch SchedulerSwitch
        {
          get { return _schedulerSwitch; }
        }

        // for logging and debugging
        private static TraceSwitch _schedulerSwitch = new TraceSwitch("AgentScheduler", "AgentScheduler TraceSwitch");

        /// <summary>
        /// This event will be fired when it is time.</summary>
        public event EventHandler Scheduled;
        	
        /// <summary>
        /// This method is called to check the schedule.
        /// True should be returned if the checking the schedule has
        /// resulted in firing the Scheduled event. Note that if checking
        /// the schedule determines that the Scheduled event should be
        /// fired, then the FireScheduled method should also be called.</summary>
        public abstract bool CheckSchedule();

        /// <summary>
        /// This method is called when the job wants to reschedule
        /// for a given period of time into the future. This is typically
        /// called when exceptions are being ignored and an attempt to
        /// retry the job will be made.</summary>
        public abstract void RequestRescheduling(TimeSpan when);

        /// <summary>
        /// This method is called to fire the Scheduled event.</summary>
        public void FireScheduled() 
        {
	        Scheduled(this, new EventArgs());
        }

        /// <summary>
        /// This method is called to fire the Scheduled event.</summary>
        public void ScheduledSuccessFully()
        {
            LastScheduled = DateTime.Now;
        }


        protected ScheduleDaysOfWeek ConvertDateTimeDayOfWeek(DateTime dateTime)
        {
            switch (dateTime.DayOfWeek)
            {
                case DayOfWeek.Monday: return ScheduleDaysOfWeek.Monday;
                case DayOfWeek.Tuesday: return ScheduleDaysOfWeek.Tuesday;
                case DayOfWeek.Wednesday: return ScheduleDaysOfWeek.Wednesday;
                case DayOfWeek.Thursday: return ScheduleDaysOfWeek.Thursday;
                case DayOfWeek.Friday: return ScheduleDaysOfWeek.Friday;
                case DayOfWeek.Saturday: return ScheduleDaysOfWeek.Saturday;
                case DayOfWeek.Sunday: return ScheduleDaysOfWeek.Sunday;
            }
            throw (new ArgumentException("Invalid DayOfWeek", dateTime.DayOfWeek.ToString()));
        }
    }


    /// <summary>
    /// This is the .NET Agent Framework's scheduler collection.</summary>
    /// <remarks>
    /// AgentSchedulerCollection derives from CollectionBase.</remarks>
    [Serializable]
    public class AgentSchedulerCollection : CollectionBase 
    {
        public AgentSchedulerCollection(AgentJob job)
        {
            Parent = job;
        }
        
        /// <summary>
        /// Add a scheduler to the collection.</summary>
        public void Add(AgentScheduler scheduler) 
        {
            scheduler.Parent = Parent;
            List.Add(scheduler);
        }

        /// <summary>
        /// Remove a scheduler from the collection.</summary>
        public void Remove(AgentScheduler scheduler)
        {
            scheduler.Parent = null;
            List.Remove(scheduler);
        }

        /// <summary>
        /// Set or retrieve a scheduler at the specific index in the collection.</summary>
        public AgentScheduler this[int index] 
        {
            get { return (AgentScheduler) List[index]; }
            set { List[index] = value; }
        }

        public AgentJob Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        /// Retrieves scheduler that scheduled the last time.</summary>
        public AgentScheduler GetLastScheduled()
        {
            DateTime lastScheduled = DateTime.MinValue;
            AgentScheduler item = null;

            foreach (AgentScheduler scheduler in this)
            {
                if (lastScheduled < scheduler.LastScheduled)
                {
                    item = scheduler;
                    lastScheduled = scheduler.LastScheduled;
                }
            }
            return item;
        }

        #region Privates

        private AgentJob parent;

        #endregion
    }
}

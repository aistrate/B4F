using System;

namespace B4F.TotalGiro.Jobs.Manager.Scheduler
{
	[Serializable]
	public class FixedIntervalScheduler : AgentScheduler
	{
        public TimeSpan StartTime = new TimeSpan(0, 0, 0, 0, 0);
        public TimeSpan EndTime = new TimeSpan(0, 23, 59, 59, 999);
        private TimeSpan _rescheduleRequest = TimeSpan.Zero;

	    public TimeSpan Frequency;

        public override bool CheckSchedule() 
        {
          bool scheduled = false;
          if ( CheckIfScheduled() )
          {
            //LastScheduled = DateTime.Now;
            _rescheduleRequest = TimeSpan.Zero;
            scheduled = true;
            FireScheduled();
          }
          return scheduled;
        }

        public override void RequestRescheduling(TimeSpan when)
        {
          _rescheduleRequest = when;
        }

        public override DateTime NextScheduled
        {
            get { return LastScheduled.Add(Frequency); }
        }

        private bool CheckIfScheduled() 
        {
          DateTime when = DateTime.Now;
          bool scheduled = false;
          if ( 
            //check Day Of Week is in schedule
            ((ConvertDateTimeDayOfWeek(when) & DaysOfWeek) != 0)
            //Check that Frequency has been set
            && (Frequency.Ticks > 0)
            //Check time is in range
            && (
            ((EndTime > StartTime) && (when.TimeOfDay >= StartTime) && (when.TimeOfDay <= EndTime))
            || ((EndTime < StartTime) && ((when.TimeOfDay >= StartTime) || (when.TimeOfDay <= EndTime)))
            )
            )
          {
            TimeSpan waitTime = Frequency;
            if ( (_rescheduleRequest != TimeSpan.Zero) && (_rescheduleRequest.Ticks < waitTime.Ticks) )
              waitTime = _rescheduleRequest;

            DateTime nextScheduled = LastScheduled.Add(waitTime);
            if (nextScheduled <= when) 
            {
              scheduled = true;
            }
          }

          return scheduled;
        }
	}
}

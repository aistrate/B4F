using System;
using System.Collections.Generic;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Jobs.Manager.Scheduler
{
    [Serializable]
    public class FixedTimeScheduler: AgentScheduler
    {
        public TimeSpan StartTime = new TimeSpan(0, 0, 0, 0, 0);
        private DateTime _rescheduleRequest = DateTime.MinValue;

        private TimeSpan MaxElapsedTime
        {
            get { return StartTime.Add(Parent.Manager.HeartbeatFrequency).Add(Parent.Manager.HeartbeatFrequency); }
        }

        public override DateTime NextScheduled
        {
            get 
            {
                DateTime startDate = DateTime.Now.Date.Add(StartTime);
                double days = 0;

                do
                {
                    DateTime nextDate = startDate.AddDays(days);
                    if (nextDate > DateTime.Now && (ConvertDateTimeDayOfWeek(nextDate) & DaysOfWeek) == ConvertDateTimeDayOfWeek(nextDate))
                        return nextDate;
                    days++;
                } while (days < 100);
                return DateTime.MaxValue;
            }
        }

        public override bool CheckSchedule()
        {
            bool scheduled = false;
            if (CheckIfScheduled())
            {
                //LastScheduled = DateTime.Now;
                _rescheduleRequest = DateTime.MinValue;
                scheduled = true;
                FireScheduled();
            }
            return scheduled;
        }

        public override void RequestRescheduling(TimeSpan when)
        {
            if (when > this.Parent.Manager.HeartbeatFrequency)
                _rescheduleRequest = DateTime.Now.Add(when);
            else
                _rescheduleRequest = DateTime.Now.Add(this.Parent.Manager.HeartbeatFrequency);
        }

        private bool CheckIfScheduled()
        {
            DateTime when = DateTime.Now;
            bool scheduled = false;
            
            // check if already scheduled
            if (!LastScheduled.Date.Equals(when.Date))
            {
                //check Day Of Week is in schedule
                if ((ConvertDateTimeDayOfWeek(when) & DaysOfWeek) != 0)
                {
                    //Check time
                    if (when.TimeOfDay >= StartTime && when.TimeOfDay < MaxElapsedTime)
                        scheduled = true;
                }
            }
            else if (_rescheduleRequest != DateTime.MinValue)
            {
                if (when >= _rescheduleRequest)
                    scheduled = true;
            }
            return scheduled;
        }

    }
}

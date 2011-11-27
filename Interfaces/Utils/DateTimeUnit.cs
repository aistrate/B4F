using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Utils
{
    /// <summary>
    /// Different type of Date parts
    /// </summary>
    public enum DateParts
    {
        Year = 1,
        Month = 12,
        Week = 52,
        Day = 365,
        Hour = 24,
        Minute = 60
    }

    public class DateTimeUnit
    {
        protected DateTimeUnit() { }
        
        public DateTimeUnit(DateParts datePart, int units)
        {
            this.DatePart = datePart;
            this.Units = units;
        }

        public DateParts DatePart { get; protected set; }
        public int Units { get; protected set; }

        public DateTime AddUnitToDate(DateTime date)
        {
            return AddUnitToDate(date, DateIntervalOptions.ExcludeWeekendsAndHolidays, null);
        }
        
        public DateTime AddUnitToDate(DateTime date, DateIntervalOptions option)
        {
            return AddUnitToDate(date, option, null);
        }

        public DateTime AddUnitToDate(DateTime date, DateIntervalOptions option, IDateTimeCollection holidays)
        {
            DateInterval interval = DateInterval.Year;
            
            switch (DatePart)
            {
                case DateParts.Month:
                    interval = DateInterval.Month;
                    break;
                case DateParts.Week:
                    interval = DateInterval.Week;
                    break;
                case DateParts.Day:
                    interval = DateInterval.Day;
                    break;
                case DateParts.Hour:
                    interval = DateInterval.Hour;
                    break;
                case DateParts.Minute:
                    interval = DateInterval.Minute;
                    break;
                default: // Year
                    break;
            }
            return Util.DateAdd(interval, Units, date, option, holidays);
        }
    }
}

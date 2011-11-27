using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.Utils
{
    #region Enums

    /// <summary>
	/// This enumeration is used to list compare operators
	/// </summary>
    public enum CompareOperator
	{
		/// <summary>
        /// The Greater then operator
		/// </summary>
        Greater,
        /// <summary>
        /// The Greater then or equal operator
        /// </summary>
        GreaterOrEqual,
        /// <summary>
        /// The Less then operator
        /// </summary>
        Smaller,
        /// <summary>
        /// The Less then or equal operator
        /// </summary>
        SmallerOrEqual
	}

    /// <summary>
    /// This enumeration is used to list math operators
    /// </summary>
    public enum MathOperator
	{
        /// <summary>
        /// The Add operator
        /// </summary>
        Add,
        /// <summary>
        /// The Subtract operator
        /// </summary>
        Subtract
	}

    /// <summary>
    /// This enumeration is used to list Equality operators
    /// </summary>
    public enum EqualityOperator
	{
        /// <summary>
        /// The Equals operator
        /// </summary>
        Equals,
        /// <summary>
        /// The NotEquals operator
        /// </summary>
        NotEquals
	}
	
    /// <summary>
    /// Specifies the direction in which to sort a list of items. 
    /// </summary>
    public enum SortingDirection
    {
        /// <summary>
        /// Sort from smallest to largest. For example, from A to Z.
        /// </summary>
        Ascending,
        /// <summary>
        /// Sort from largest to smallest. For example, from Z to A. 
        /// </summary>
        Descending
    }

    /// <summary>
    /// This enumeration is used to compare numbers with zero
    /// </summary>
    public enum CompareToZeroOperator
    {
        /// <summary>
        /// The value is smaller than zero
        /// </summary>
        SmallerThanZero = -1,
        /// <summary>
        /// The value is zero
        /// </summary>
        IsZero = 0,
        /// <summary>
        /// The value is greater than zero
        /// </summary>
        GreaterThanZero = 1
    }

    /// <summary>
    /// This enumeration is used to compare the sign of values
    /// </summary>
    public enum SignValues
    {
        /// <summary>
        /// All Values are allowed
        /// </summary>
        All = 0,
        /// <summary>
        /// Only positive values are allowed
        /// </summary>
        Positive = 1,
        /// <summary>
        /// Only negative values are allowed
        /// </summary>
        Negative = -1
    }

    /// <summary>
    /// This enumeration is used to filter on activity states
    /// </summary>
    public enum ActivityReturnFilter
    {
        /// <summary>
        /// Return all items
        /// </summary>
        All,
        /// <summary>
        /// Return only the Active items
        /// </summary>
        Active,
        /// <summary>
        /// Return only the InActive items
        /// </summary>
        InActive
    }

    /// <summary>
    /// Indicates how to determine and format date intervals when calling date-related functions.
    /// </summary>
    public enum DateInterval
    {
        Second,
        Minute,
        Hour,
        Day,
        BusinessDay,
        Week,
        Month,
        Year
    }

    /// <summary>
    /// Extra options for the DateAdd function.
    /// </summary>
    public enum DateIntervalOptions
    {
        None,
        ExcludeWeekends,
        ExcludeWeekendsAndHolidays
    }

    /// <summary>
    /// Different type of regularities
    /// </summary>
    public enum Regularities
    {
        Annual = 1,
        SemiAnnual = 2,
        Quarterly = 4,
        Monthly = 12
    }

    public enum ElfProefCheckType
    {
        Bank,
        BSN
    }

    public enum MatchModes
    {
        Anywhere,
        End,
        Start,
        Exact
    }

    public enum MainTypeCodes
    {
        String,
        Number,
        DateTime,
        Boolean,
        Object,
        Empty
    }

    #endregion

    /// <summary>
    /// Class with utilities
    /// </summary>
    public static class Util
	{
        /// <summary>
        /// The Nulldate
        /// </summary>
        public static DateTime NullDate
        {
            get
            {
                return System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
            }
        }

        /// <summary>
        /// The Nulldate
        /// </summary>
        public static bool IsNullDate(DateTime date)
        {
            bool retVal = false;
            if (date.Equals(DateTime.MinValue))
                retVal = true;
            else if (date.Equals(System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0))))
                retVal = true;
            return retVal;
        }

        public static bool IsNullDate(DateTime? date)
        {
            if (date.HasValue)
                return IsNullDate(date.Value);
            else
                return true;
        }

        public static bool IsNotNullDate(DateTime date)
        {
            return !IsNullDate(date);
        }

        public static bool IsNotNullDate(DateTime? date)
        {
            return !IsNullDate(date);
        }

        /// <summary>
        /// Finding the number of leap years between two dates in C#
        /// </summary>
        public static int GetLeapYearCount(DateTime start, DateTime end)
        {
            //Leap year checker
            int leapValueToSubtract = 0;
            if (start.Year == end.Year)
            {
                //if the same year AND a leap year, we just need to subtract 1 day
                if (DateTime.IsLeapYear(start.Year) && start.DayOfYear < 60)
                {
                    leapValueToSubtract++;
                }
            }
            else if (start.Year != end.Year)
            {
                if (DateTime.IsLeapYear(start.Year) && start.DayOfYear < 60)
                {
                    leapValueToSubtract++;
                }

                int holder = start.Year;
                holder = holder + (4 - (holder % 4));

                for (int i = 0; i < 10; i++)
                {
                    if (holder < end.Year)
                    {
                        leapValueToSubtract++;
                        holder = holder + 4;
                    }
                }
                if (DateTime.IsLeapYear(end.Year) && end.DayOfYear > 60)
                {
                    leapValueToSubtract++;
                }
            }
            return leapValueToSubtract;
        }

        /// <summary>
        /// Returns true if date is between start and end date inclusive. Put in some static class, etc.
        /// </summary>
        public static bool DateBetween(DateTime start, DateTime end, DateTime date)
        {
            if ( date >= start && date <= end )
                return true;
            return false;
        }

        public static bool IsNumeric(string value)
        {
            bool retVal = false;
            long test;
            if (long.TryParse(value, out test))
                retVal = true;
            return retVal;
        }

        public static bool IsNumeric(object obj)
        {
            return IsNumeric(obj.ToString());
        }

        public static MainTypeCodes GetMainTypeCode(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return MainTypeCodes.Boolean;
                case TypeCode.String:
                case TypeCode.Char:
                    return MainTypeCodes.String;
                case TypeCode.DBNull:
                case TypeCode.Empty:
                    return MainTypeCodes.Empty;
                case TypeCode.DateTime:
                    return MainTypeCodes.DateTime;
                case TypeCode.Object:
                    return MainTypeCodes.Object;
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return MainTypeCodes.Number;
                default:
                    return MainTypeCodes.Empty;
            }
        }

        public static string DateTimeToString(DateTime date)
        {
            if (date == DateTime.MinValue || date == Util.NullDate)
                return "";
            else
                return (date.TimeOfDay == new TimeSpan() ? date.ToString("d") : date.ToString());
        }

        public static string Capitalize(string text)
        {
            text = (text != null ? text.Trim() : string.Empty);
            return (text != string.Empty ? text = text.Substring(0, 1).ToUpper() + text.Substring(1) : string.Empty);
        }

        public static string Substring(string text, int startIndex, int length)
        {
            text = (text != null ? text.Trim() : string.Empty);
            
            if (string.IsNullOrEmpty(text) || startIndex > text.Length)
                return "";

            if ((startIndex + length) > text.Length)
                return text.Substring(startIndex);
            else
                return text.Substring(startIndex, length);
        }

        // http://blogs.msdn.com/michkap/archive/2007/05/14/2629747.aspx
        public static string RemoveDiacritics(string unicodeString)
        {
            string stFormD = unicodeString.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }


        public static string ConvertToAscii(string unicodeString)
        {
            // Create two different encodings.
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            // Convert the string into a byte[].
            byte[] unicodeBytes = unicode.GetBytes(RemoveDiacritics(unicodeString));

            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            // Convert the new byte[] into a char[] and then into a string.
            // This is a slightly different approach to converting to illustrate
            // the use of GetCharCount/GetChars.
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);

            return asciiString;
        }

        public static CompareToZeroOperator CompareToZero(int value)
        {
            if (value == 0M)
                return CompareToZeroOperator.IsZero;
            else if (value > 0M)
                return CompareToZeroOperator.GreaterThanZero;
            else
                return CompareToZeroOperator.SmallerThanZero;
        }

        /// <summary>
        /// Is the percentage within the supplied boundaries
        /// </summary>
        /// <param name="percentage">The percentage we want to test</param>
        /// <param name="presumedPercentage">The percentage that it should be</param>
        /// <param name="tolerance">The supplied boundary</param>
        /// <returns>True when the percentage is within the supplied boundaries</returns>
        public static bool IsPercentageWithinTolerance(decimal percentage, decimal presumedPercentage, decimal tolerance)
        {
            bool answer = false;
            if (percentage != 0M && presumedPercentage != 0M)
            {
                decimal diff;
                decimal delta = percentage / presumedPercentage;
                if (delta < 1M)
                    diff = 1M - delta;
                else
                    diff = delta - 1M;

                if (diff == 0M || diff <= tolerance)
                    answer = true;
            }
            return answer;
        }

        public static bool IsWithinTolerance(decimal value, decimal presumedValue, decimal percentualTolerance)
        {
            if (value * presumedValue != 0m)
            {
                decimal delta = Math.Abs(1m - Math.Abs(value / presumedValue));
                if (delta <= percentualTolerance)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a Int value specifying the number of time intervals between two Date values
        /// </summary>
        /// <param name="interval">The requested type of interval</param>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>The diffrence between the two dates</returns>
        public static int DateDiff(DateInterval interval, DateTime startDate, DateTime endDate)
        {
            return DateDiff(interval, startDate, endDate, null);
        }

        /// <summary>
        /// Returns a Int value specifying the number of time intervals between two Date values
        /// </summary>
        /// <param name="interval">The requested type of interval</param>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="holidays">The holiday collection that belongs to a certain exchange</param>
        /// <returns>The diffrence between the two dates</returns>
        public static int DateDiff(DateInterval interval, DateTime startDate, DateTime endDate, IDateTimeCollection holidays)
        {
            if (endDate < startDate)
                throw new ApplicationException("Start date must be before end date");

            TimeSpan dateDiff = endDate - startDate;
            
            switch (interval)
            {
                case DateInterval.Second:
                    return dateDiff.Seconds;
                case DateInterval.Minute:
                    return dateDiff.Minutes;
                case DateInterval.Hour:
                    return dateDiff.Hours;
                case DateInterval.Month:
                    return (endDate.Month - startDate.Month) + (12 * (endDate.Year - startDate.Year));
                case DateInterval.Year:
                    return (endDate.Year - startDate.Year);
                case DateInterval.BusinessDay:
                    DateTime temp = endDate.Date;
                    int days = 0;
                    while (temp > startDate.Date)
                    {
                        if (!(temp.DayOfWeek == DayOfWeek.Saturday || temp.DayOfWeek == DayOfWeek.Sunday || (holidays != null && holidays.Contains(temp))))
                            days++;

                        temp = temp.AddDays(-1);
                    };
                    return days;
                default:
                    // DateInterval.Day
                    return dateDiff.Days + 1;
            }
        }

        /// <summary>
        /// Returns a Date changed depending on the interval and the number
        /// </summary>
        /// <param name="interval">The requested type of interval</param>
        /// <param name="number">The number to change the date with</param>
        /// <param name="date">The start date</param>
        /// <param name="option">Tells what days to return</param>
        /// <param name="holidays">The holiday collection that belongs to a certain exchange</param>
        /// <returns>The diffrence between the two dates</returns>
        public static DateTime DateAdd(DateInterval interval, int number, DateTime date, DateIntervalOptions option, IDateTimeCollection holidays)
        {
            DateTime retDate = DateTime.MinValue;
            int step = 0;
            switch (interval)
            {
                case DateInterval.Second:
                    retDate = date.AddSeconds(number);
                    break;
                case DateInterval.Minute:
                    retDate = date.AddMinutes(number);
                    break;
                case DateInterval.Hour:
                    retDate = date.AddHours(number);
                    break;
                case DateInterval.Month:
                    retDate = date.AddMonths(number);
                    break;
                case DateInterval.Week:
                    retDate = date.AddDays(number * 7);
                    break;
                case DateInterval.Year:
                    retDate = date.AddYears(number);
                    break;
                case DateInterval.BusinessDay:
                    DateTime temp = date;
                    int days = 0;
                    step = (number < 0 ? -1 : 1);
                    while (Math.Abs(days) < Math.Abs(number))
                    {
                        temp = temp.AddDays(step);
                        if (!(temp.DayOfWeek == DayOfWeek.Saturday || temp.DayOfWeek == DayOfWeek.Sunday || (holidays != null && holidays.Contains(temp))))
                            days = days + step;
                    };
                    return temp;
                default:
                    // DateInterval.Day
                    retDate = date.AddDays(number);
                    break;
            }

            if (option != DateIntervalOptions.None)
            {
                if (retDate.DayOfWeek == DayOfWeek.Saturday)
                    step = -1;
                else if (retDate.DayOfWeek == DayOfWeek.Sunday)
                    step = 1;

                if (step != 0)
                    retDate = retDate.AddDays(step);

                if (option == DateIntervalOptions.ExcludeWeekendsAndHolidays && holidays != null)
                {
                    while (holidays.Contains(retDate))
                    {
                        retDate = retDate.AddDays((step == 0 ? 1 : step));
                    }
                }
            }
            return retDate;
        }

        /// <summary>
        /// This method returns a date relative from the initialdate.
        /// But not after the endDate, then it returns a nulldate
        /// </summary>
        /// <param name="regularity">The regularity</param>
        /// <param name="number">The number of how many units to move forward (positive) or backwards (negative)</param>
        /// <param name="initialDate">The initial date, also the minimum date (before this date will return null)</param>
        /// <param name="endDate">The end date, beyond this date will return null</param>
        /// <param name="holidays">The known holidays</param>
        /// <returns></returns>
        public static DateTime DateAddByRegularity(Regularities regularity, int number, DateTime initialDate, DateTime endDate, IDateTimeCollection holidays)
        {
	        DateTime retDate;
	        DateTime maxDate;
	        DateTime lastDate;
            int months;

	        months = 12 / (int)regularity;

	        if (Util.IsNullDate(endDate) || endDate > DateTime.Today)
		        maxDate = DateTime.Today;
	        else
		        maxDate = endDate;

            lastDate = initialDate;

	        /* Find the Last recent Date */
            for (int i = 0; Util.DateAdd(DateInterval.Month, (months * i), initialDate, DateIntervalOptions.None, null) < maxDate; i++)
			{
                lastDate = Util.DateAdd(DateInterval.Month, (months * i), initialDate, DateIntervalOptions.None, null);
			}

	        /* Go to the prefrerred date */
            retDate = Util.DateAdd(DateInterval.Month, months * number, lastDate, DateIntervalOptions.ExcludeWeekendsAndHolidays, holidays);

            /* Date can not be bigger than lastDate */
            if (!Util.IsNullDate(endDate) && retDate > endDate)
		        retDate = DateTime.MinValue;
            else if (retDate < initialDate && number <= 0)
                retDate = DateTime.MinValue;

	        return retDate;
        }

        // Static Method to return ISO WeekNumber (1-53) for a given year
        public static int GetWeekNumber(DateTime date)
        {
            CultureInfo culture = new CultureInfo("", false);
            return culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime GetDayByWeekNumber(int year, int weekNumber)
        {
            DateTime firstDayOfWeekTwo = new DateTime(year, 1, 1);

            CultureInfo c = CultureInfo.CurrentCulture;
            while (c.Calendar.GetWeekOfYear(firstDayOfWeekTwo, c.DateTimeFormat.CalendarWeekRule, c.DateTimeFormat.FirstDayOfWeek) != 2)
                firstDayOfWeekTwo = firstDayOfWeekTwo.AddDays(1);

            return firstDayOfWeekTwo.AddDays(7 * (weekNumber - 2));
        }

        // Static Method to return the relevant quarter of the year o a certain date
        public static int GetQuarter(DateTime date)
        {
            //Get the current month
            int i = date.Month;

            //Based on the current month return the quarter
            if (i <= 3)
            { return 1; }
            else if (i >= 4 && i <= 6)
            { return 2; }
            else if (i >= 7 && i <= 9)
            { return 3; }
            else if (i >= 10 && i <= 12)
            { return 4; }
            else
                //Something probably is wrong 
                return 0;
        }

        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetFirstDayOfMonth(int period)
        {
            string strPeriod = period.ToString();
            if (strPeriod.Length != 6)
                throw new ApplicationException("This is not a valid period");

            int year = Convert.ToInt32(strPeriod.Substring(0, 4));
            int month = Convert.ToInt32(strPeriod.Substring(4, 2));
            return new DateTime(year, month, 1);
        }

        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        public static DateTime GetLastDayOfMonth(int period)
        {
            string strPeriod = period.ToString();
            if (strPeriod.Length != 6)
                throw new ApplicationException("This is not a valid period");

            int year = Convert.ToInt32(strPeriod.Substring(0, 4));
            int month = Convert.ToInt32(strPeriod.Substring(4, 2));
            return new DateTime(year, month, DateTime.DaysInMonth(year, month));
        }

        public static int GetLastDayOfPeriod(int period)
        {
            return GetLastDayOfMonth(period).Day;
        }

        public static bool GetQuarterYearByPeriod(int period, out int year, out int quarter)
        {
            string strPeriod = period.ToString();
            if (strPeriod.Length != 6)
                throw new ApplicationException("This is not a valid period");

            year = Convert.ToInt32(strPeriod.Substring(0, 4));
            int month = Convert.ToInt32(strPeriod.Substring(4, 2));
            quarter = GetQuarter(new DateTime(year, month, 1));
            return true;
        }

        public static Tuple<int, int> GetPreviousQuarterYear(int quarter, int year)
        {
            int prevQuarter = quarter - 1;
            int prevYear = year;

            if (prevQuarter == 0)
            {
                prevQuarter = 4;
                prevYear = year - 1;
            }
            return new Tuple<int, int>(prevQuarter, prevYear);
        }

        public static DateTime GetSpecificDayInPeriod(int period, DayOfWeek dayOfWeek, int sequence)
        {
            string strPeriod = period.ToString();
            if (strPeriod.Length != 6)
                throw new ApplicationException("This is not a valid period");
            if (sequence > 6)
                throw new ApplicationException("This is not a valid sequence");

            int year = Convert.ToInt32(strPeriod.Substring(0, 4));
            int month = Convert.ToInt32(strPeriod.Substring(4, 2));

            int hitCount = 0;
            int day = 1;
            DateTime returnDay = DateTime.MinValue;
            while (hitCount < sequence)
            {
                returnDay = new DateTime(year, month, day);
                if (returnDay.DayOfWeek == dayOfWeek)
                    hitCount++;
                day++;
            }
            return returnDay;
        }

        public static bool IsLastDayOfQuarter(DateTime date)
        {
            bool retVal = false;

            if (date.Month % 3 == 0 && date.Day == DateTime.DaysInMonth(date.Year, date.Month))
                retVal = true;
            return retVal;
        }

        /// <summary>
        /// Returns whether a date is a saterday/sunday or holiday day
        /// </summary>
        /// <param name="date">The start date</param>
        /// <param name="holidays">The holiday collection that belongs to a certain exchange</param>
        /// <returns>true or false</returns>
        public static bool IsWeekendOrHoliday(DateTime date, IDateTimeCollection holidays)
        {
            bool isWeekendOrHoliday = false;
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday || (holidays != null && holidays.Contains(date)))
                isWeekendOrHoliday = true;
            return isWeekendOrHoliday;
        }

        public static DateTime[] GetDatesArray(DateTime startDate, DateTime endDate)
        {
            int days = ((TimeSpan)(endDate - startDate)).Days + 1;
            if (days > 0)
            {
                DateTime[] dates = new DateTime[days];
                for (int i = 0; i < days; i++)
                    dates[i] = startDate.AddDays(i);
                return dates;
            }
            else
                return null;
        }

        public static void GetDatesFromQuarter(int year, string quarter, out DateTime beginDate, out DateTime endDate)
        {
            int q = Convert.ToInt32(quarter.Substring(1));
            GetDatesFromQuarter(year, q, out beginDate, out endDate);
        }

        public static void GetDatesFromQuarter(int year, int quarter, out DateTime beginDate, out DateTime endDate)
        {
            switch (quarter)
            {
                case 1:
                    beginDate = new DateTime(year, 1, 1);
                    endDate = new DateTime(year, 3, 31);
                    break;
                case 2:
                    beginDate = new DateTime(year, 4, 1);
                    endDate = new DateTime(year, 6, 30);
                    break;
                case 3:
                    beginDate = new DateTime(year, 7, 1);
                    endDate = new DateTime(year, 9, 30);
                    break;
                case 4:
                    beginDate = new DateTime(year, 10, 1);
                    endDate = new DateTime(year, 12, 31);
                    break;
                default:
                    throw new ApplicationException("This is not a valid quarter");
            }
        }

        public static void GetDatesFromPeriod(int period, out DateTime beginDate, out DateTime endDate)
        {
            int year = period / 100;
            int month = period - (year * 100);

            beginDate = new DateTime(year, month, 1);
            endDate = Util.GetLastDayOfMonth(beginDate);
        }

        public static DateTime GetFirstDateFromPeriod(int period)
        {
            DateTime beginDate, endDate;
            GetDatesFromPeriod(period, out beginDate, out endDate);
            return beginDate;
        }

        public static DateTime GetLastDateFromPeriod(int period)
        {
            DateTime beginDate, endDate;
            GetDatesFromPeriod(period, out beginDate, out endDate);
            return endDate;
        }
        
        /// <summary>
        /// Returns a integer that identifies a a month and a year.
        /// for instance 15-05-2007 is returned as 200705
        /// </summary>
        /// <param name="date"></param>
        public static int GetPeriodFromDate(DateTime date)
        {
            return (date.Year * 100) + date.Month;
        }

        /// <summary>
        /// Returns a collection with the periods in the quarter
        /// </summary>
        /// <param name="quarter"></param>
        /// <param name="year"></param>
        public static int[] GetPeriodsFromQuarter(int year, int quarter)
        {
            int[] list = new int[3];
            for (int i = 0; i < 3; i++)
			{
                int index = 2 - i;
                list[index] = (year * 100) + ((quarter * 3) - i);
			}
            return list;
        }

        /// <summary>
        /// Returns a collection with the periods from the whole year
        /// </summary>
        /// <param name="year"></param>
        public static int[] GetPeriodsFromYear(int year)
        {
            int[] list = new int[12];
            for (int i = 0; i < 12; i++)
            {
                list[i] = (year * 100) + (i + 1);
            }
            return list;
        }

        public static int GetValueFromDate(DateTime date)
        {
            return (date.Year * (int)PriceHistoryKeyValues.year) +
                (date.Month * (int)PriceHistoryKeyValues.month) +
                (date.Day * (int)PriceHistoryKeyValues.day);
        }

        public static DateTime GetDateFromValue(int value)
        {
            int year = value / (int)PriceHistoryKeyValues.year;
            int month = (value - (year * (int)PriceHistoryKeyValues.year)) / (int)PriceHistoryKeyValues.month;
            int day = (value - (year * (int)PriceHistoryKeyValues.year) - (month * (int)PriceHistoryKeyValues.month)) / (int)PriceHistoryKeyValues.day;
            return new DateTime(year, month, day);
        }

        public static int CalculateCurrentAge(DateTime birthDate)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - birthDate.Year;
            if (birthDate > now.AddYears(-age)) age--;
            return age;
        }

        public static NumberFormatInfo GetYankeeNumberFormat()
        {
            CultureInfo culture = new CultureInfo(CultureInfo.CurrentCulture.Name);
            NumberFormatInfo numInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));
            numInfo.NumberDecimalDigits = 2;
            numInfo.NumberDecimalSeparator = ".";
            return numInfo;
        }

        public static bool IsEnumInValue(int value, int enumValue)
        {
            bool retVal = false;
            if ((value & enumValue) == enumValue)
                retVal = true;
            return retVal;
        }

        public static bool IsEnumInValue<TEnum>(int value, TEnum enumValue)
            where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            return EnumComparer<TEnum>.Instance.ContainsValue(value, enumValue);
        }

        public static bool IsEnumInValue<TEnum>(TEnum value, TEnum enumValue)
            where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            return EnumComparer<TEnum>.Instance.ContainsValue(value, enumValue);
        }

        public static DataSet GetDataSetFromEnum(Type enumType)
        {
            return GetDataSetFromEnum(enumType, string.Empty, string.Empty, SortingDirection.Ascending);
        }

        public static DataSet GetDataSetFromEnum(Type enumType, SortingDirection direction)
        {
            return GetDataSetFromEnum(enumType, string.Empty, string.Empty, direction);
        }

        public static DataSet GetDataSetFromEnum(Type enumType, string nameKeyField, string nameDescriptionField, SortingDirection direction)
        {
            return GetDataSetFromEnum(enumType, nameKeyField, nameDescriptionField, direction, false);
        }

        public static DataSet GetDataSetFromEnum(
            Type enumType, string nameKeyField, string nameDescriptionField, SortingDirection direction, bool addSpaces)
        {
            if (nameKeyField == string.Empty) nameKeyField = "Key";
            if (nameDescriptionField == string.Empty) nameDescriptionField = "Description";
            
            DataSet dsTypes = new DataSet("Types");
            DataTable dtTypes = new DataTable("Types");
            dsTypes.Tables.Add(dtTypes);
            dtTypes.Columns.Add(nameKeyField, typeof(int));
            dtTypes.Columns.Add(nameDescriptionField, typeof(string));

            if (direction == SortingDirection.Ascending)
            {
                foreach (int i in Enum.GetValues(enumType))
                {
                    DataRow row = dtTypes.NewRow();
                    row[0] = i;
                    if (addSpaces)
                        row[1] = AddSpacesBetweenCapitals(Enum.GetName(enumType, i));
                    else
                        row[1] = Enum.GetName(enumType, i);
                    dtTypes.Rows.Add(row);
                }
            }
            else // Descending
            {
                Array values = Enum.GetValues(enumType);
                for (int i = values.Length; i > 0; i--)
                {
                    int j = (int)values.GetValue(i - 1);
                    DataRow row = dtTypes.NewRow();
                    row[0] = j;
                    if (addSpaces)
                        row[1] = AddSpacesBetweenCapitals(Enum.GetName(enumType, j));
                    else
                        row[1] = Enum.GetName(enumType, j);
                    dtTypes.Rows.Add(row);
                }
            }

            return dsTypes;
        }

        public static DataSet AddEnumAsStringColumn<EnumType>(this DataSet ds, string sourceColumnName, string destColumnName)
        {
            ds.Tables[0].AddEnumAsStringColumn<EnumType>(sourceColumnName, destColumnName);
            return ds;
        }

        public static DataTable AddEnumAsStringColumn<EnumType>(this DataTable dt, string sourceColumnName, string destColumnName)
        {
            dt.Columns.Add(destColumnName, typeof(string));

            foreach (DataRow dr in dt.Rows)
                dr[destColumnName] = (Enum.ToObject(typeof(EnumType), (int)dr[sourceColumnName])).ToString();

            return dt;
        }

        public static void AddNewRowToDataTableWhenRecordNotExists(DataTable table, object key, params object[] values)
        {
            bool hit = false;
            foreach (DataRow row in table.Rows)
            {
                if (row[0].Equals(key))
                    return;
            }

            if (!hit)
            {
                DataRow row = table.NewRow();
                row[0] = key;
                for (int i = 0; i < values.Length; i++)
                {
                    row[i + 1] = values[i];
                }
                table.Rows.Add(row);
            }
        }

        public static void AddSpacesBetweenCapitalsInDataColumn(DataTable table, int columnIndex)
        {
            foreach (DataRow row in table.Rows)
            {
                if (row[columnIndex] != null)
                {
                    row[columnIndex] = AddSpacesBetweenCapitals(row[columnIndex].ToString());
                }
            }
        }

        public static void SortDataTable(DataTable dt, string sort)
        {
            Util.FilterDataTable(dt, null, sort);
        }

        public static void FilterDataTable(DataTable dt, string filterExpression, string sort)
        {
            DataTable newDT = dt.Clone();
            DataRow[] foundRows = dt.Select(filterExpression, sort); // Sort with Column name 
            int rowCount = foundRows.Length;

            for (int i = 0; i < rowCount; i++)
            {
                object[] arr = new object[dt.Columns.Count];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    arr[j] = foundRows[i][j];
                }
                DataRow data_row = newDT.NewRow();
                data_row.ItemArray = arr;
                newDT.Rows.Add(data_row);
            }

            //clear the incoming dt 
            dt.Rows.Clear();

            for (int i = 0; i < newDT.Rows.Count; i++)
            {
                object[] arr = new object[dt.Columns.Count];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    arr[j] = newDT.Rows[i][j];
                }

                DataRow data_row = dt.NewRow();
                data_row.ItemArray = arr;
                dt.Rows.Add(data_row);
            }
        }

        public static int[] GetPageKeys(DataTable dt, int maximumRows, int pageIndex, string keyPropertyName)
        {
            int startRowIndex = maximumRows * pageIndex;
            if (dt.Rows.Count < startRowIndex)
                startRowIndex = 0;
            int endRowIndex = Math.Min(startRowIndex + maximumRows, dt.Rows.Count) - 1;
            ArrayList keys = new ArrayList();
            for (int i = startRowIndex; i <= endRowIndex; i++)
                keys.Add(dt.Rows[i][keyPropertyName]);
            return (int[])keys.ToArray(typeof(int));
        }

        public static bool DirectoryExists(string path, string errorMessage)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
                throw new ApplicationException(errorMessage);
            else
                return true;
        }

        public static string JoinFromIntArray(string separator, int[] ids)
        {
            string retVal = string.Empty;

            if (ids.Length > 0)
            {
                string[] str = new string[ids.Length];
                for (int i = 0; i < ids.Length; i++)
                {
                    str[i] = ids[i].ToString();
                }

                retVal = String.Join(separator, str);
            }
            return retVal;
        }

        public static string FormatDecimal(decimal value, int decimals)
        {
            return FormatDecimal(value, decimals, decimals);
        }

        public static string FormatDecimal(decimal value, int minDecimals, int maxDecimals)
        {
            minDecimals = Math.Max(1, minDecimals);
            maxDecimals = Math.Max(minDecimals, maxDecimals);
            string format = "0." + new string('0', minDecimals) + new string('#', maxDecimals - minDecimals);
            return value.ToString(format);
        }

        public static string PrepareNamedParameterWithWildcard(string value)
        {
            return PrepareNamedParameterWithWildcard(value, MatchModes.Anywhere);
        }

        public static string PrepareNamedParameterWithWildcard(string value, MatchModes matchMode)
        {
            switch (matchMode)
            {
                case MatchModes.Anywhere:
                    return "%" + value + "%";
                case MatchModes.End:
                    return "%" + value;
                case MatchModes.Start:
                    return value + "%";
                default: //MatchModes.Exact
                    return value;
            }
        }

        public static string GetMessageFromException(Exception ex)
        {
            string message = "";
            Util.getMessageFromExceptionRecursively(ex, ref message);
            return message;
        }

        private static void getMessageFromExceptionRecursively(Exception ex, ref string message)
        {
            if (ex != null)
            {
                if (ex.InnerException != null)
                    Util.getMessageFromExceptionRecursively(ex.InnerException, ref message);
                message = ex.Message + (message + "" != "" ? Environment.NewLine : "") + message;
            }
        }

        public static bool GetIsOldDate(DateTime date, IExchangeHolidayCollection holidays)
        {

            int days;
            if (!Int32.TryParse(ConfigSettingsInfo.GetInfo("MaxDaysNoPriceAlert"), out days))
                days = 3;
            return GetIsOldDate(date, holidays, days);
        }

        public static bool GetIsOldDate(DateTime date, IExchangeHolidayCollection holidays, int days)
        {
            bool retVal = true;
            if (date > DateTime.Now.Date)
                retVal = false;
            else if (Util.DateDiff(DateInterval.BusinessDay, date, DateTime.Now.Date, holidays) < days)
                retVal = false;
            return retVal;
        }

        public static string AddSpacesBetweenCapitals(string value)
        {
            StringBuilder sb = new StringBuilder();
            //     string strAddedSpaces = string.Empty;
            const string LOWERCASE_PATTERN = "[a-z]";
            Regex LowerCase_Regex = new Regex(LOWERCASE_PATTERN);
            foreach (char c in value)
            {
                if (LowerCase_Regex.IsMatch(c.ToString()))
                    sb.Append(c);
                else
                    sb.Append(" " + c);
            }
            return sb.ToString();
        }

        public static string SplitCamelCase(string text)
        {
            return decamelizer.Replace(text.Trim(), "${last} ");
        }

        private static Regex decamelizer = new Regex(@" ( (?<last>[a-z_0-9]) (?=[A-Z]) )   |   ( (?<last>[A-Z]) (?=[A-Z][a-z_0-9]) ) ",
                                                     RegexOptions.IgnorePatternWhitespace);

        
        
        public static bool PerformElfProefCheck(ElfProefCheckType elfProefType, string number)
        {
            decimal nTotal = 0m;
            decimal u;
            bool retVal = false;
            int maxLength;

            switch (elfProefType)
            {
                case ElfProefCheckType.BSN:
                    maxLength = 9;
                    break;
                default: // ElfProefCheckType.Bank
                    maxLength = 10;
                    break;
            }


            if (elfProefType == ElfProefCheckType.Bank && number[0].ToString().ToUpper() == "P")
                return true; //We cannot validate Postbank accounts yet!!!!

            if (number.Length < (maxLength - 1))
                retVal = false;
            else
            {
                if (number.Length == (maxLength - 1))
                    number = "0" + number;

                for (int counter = 0; counter < maxLength; counter++)
                {
                    int multiplier = (maxLength - counter);
                    if (multiplier == 1 && elfProefType == ElfProefCheckType.BSN)
                        nTotal = nTotal - Convert.ToDecimal(number.Substring(counter, 1));
                    else
                        nTotal = nTotal + Convert.ToDecimal(number.Substring(counter, 1)) * multiplier;
                }

                u = nTotal % 11;
                if (nTotal > 0 && u == 0m)
                    retVal = true;
            }
            return retVal;
        }

        #region Privates

        private enum PriceHistoryKeyValues
        {
            year = 10000,
            month = 100,
            day = 1
        }

        #endregion
    }
}

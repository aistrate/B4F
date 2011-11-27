using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace B4F.TotalGiro.Dal.Utils
{
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

        public static string DateTimeToString(DateTime date)
        {
            if (date == DateTime.MinValue || date == Util.NullDate)
                return "(min)";
            else
                return (date.TimeOfDay == new TimeSpan() ? date.ToString("dd-MM-yyyy") : date.ToString("dd-MM-yyyy HH:mm:ss"));
        }
    }
}

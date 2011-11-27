using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Communicator.Exact.Formatters
{
    internal class DateFormatter : ObjectFormatter
    {
        protected override string FormatNonNull(object value)
        {
            if (value.GetType() != typeof(DateTime))
                throw new ArgumentException(string.Format("Value {0} of type {1} could not be converted to a date.",
                                                          value, value.GetType().FullName));

            DateTime date = (DateTime)value;

            if (date == DateTime.MinValue || Util.IsNullDate(date))
                return "";
            else
                return date.ToString("ddMMyyyy");
        }
    }
}

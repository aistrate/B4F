using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Communicator.Exact.Formatters
{
    internal class AlphanumFormatter : ObjectFormatter
    {
        public AlphanumFormatter(int maxLength)
        {
            this.maxLength = maxLength;
        }

        protected override string FormatNonNull(object value)
        {
            if (value.GetType() != typeof(string))
                throw new ArgumentException(string.Format("Value {0} of type {1} could not be converted to a string.",
                                                          value, value.GetType().FullName));

            string result = (string)value;

            if (result.Length > MaxLength)
                result = result.Substring(0, MaxLength);
            
            return result;
        }

        public int MaxLength
        {
            get { return maxLength; }
        }

        private int maxLength;
    }
}

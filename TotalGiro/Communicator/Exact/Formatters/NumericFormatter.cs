using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;

namespace B4F.TotalGiro.Communicator.Exact.Formatters
{
    internal class NumericFormatter : ObjectFormatter
    {
        public NumericFormatter(int integralDigits)
            : this(integralDigits, 0)
        {
        }

        public NumericFormatter(int integralDigits, int fractionalDigits)
        {
            this.integralDigits = Math.Max(integralDigits, 1);
            this.fractionalDigits = Math.Max(fractionalDigits, 0);

            this.formatString = new string('#', IntegralDigits - 1) + "0";
            if (FractionalDigits > 0)
                this.formatString += "." + new string('#', FractionalDigits);
        }

        protected override string FormatNonNull(object value)
        {
            if (value.GetType() == typeof(string) || !Information.IsNumeric(value))
                throw new ArgumentException(string.Format("Value {0} of type {1} could not be converted to numerical format N{2}{3}.",
                    value, value.GetType().FullName, IntegralDigits, (FractionalDigits > 0 ? "." + FractionalDigits.ToString() : "")));

            // TODO: truncate to length of IntegralDigits + FractionalDigits [+ 1] (on right/left side?)
            string template = "{0:" + FormatString + "}";
            return string.Format(template, value).Replace(',', '.');
        }

        public int IntegralDigits
        {
            get { return integralDigits; }
        }

        public int FractionalDigits
        {
            get { return fractionalDigits; }
        }

        public string FormatString
        {
            get { return formatString; }
        }

        private int integralDigits, fractionalDigits;
        private string formatString;
    }
}

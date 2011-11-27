using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Reports
{
    public class ReportException : Exception
    {
        public ReportException()
            : base("Generic report exception.") { }
        public ReportException(string message)
            : base(message) { }
        public ReportException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

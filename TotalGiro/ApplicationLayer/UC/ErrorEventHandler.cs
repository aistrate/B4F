using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(Exception exception)
        {
            this.exception = exception;
        }

        public Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        private Exception exception;
    }
}

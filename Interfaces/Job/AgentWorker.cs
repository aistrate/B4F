using System;
using System.Diagnostics;
using System.ComponentModel;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Jobs
{
    /// <summary>
    /// This is the .NET Agent Framework's base class for a worker.</summary>
    /// <remarks>
    /// The AgentWorker defines some properties of a worker and the abstract
    /// method Run that must be implemented by derived worker classes.</remarks>
    [Serializable]
    public abstract class AgentWorker
    {
        /// <summary>
        /// Use this TraceSwitch when tracing in derived classes.</summary>
        protected static TraceSwitch WorkerSwitch
        {
            get { return _workerSwitch; }
        }

        /// <summary>
        /// Name of the worker.</summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Description of the worker.</summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// The managementcompany that this worker belongs to.</summary>
        public int ManagementCompanyID
        {
            get { return managementCompanyID; }
            set { managementCompanyID = value; }
        }

        /// <summary>
        /// Message to be used when an exception is encountered.
        /// The Run method would string.Format to format this message
        /// and store it in the WorkerResult. The default message
        /// assumes that the parameters included in string.Format will
        /// be the current date/time and the exception's Message
        /// property. Exceptions may be handled in the Run method,
        /// but uncaught exceptions may be handled elsewhere and
        /// will use this value.</summary>
        public string MessageException
        {
            get { return _messageException; }
            set { _messageException = value; }
        }


        private static TraceSwitch _workerSwitch = new TraceSwitch("AgentWorker", "AgentWorker TraceSwitch");
        private string _name;
        private string _description = null;
        private string _messageException = "Exception at {0:u}: {1}";
        private int managementCompanyID;

        /// <summary>
        /// Default the job's name to the name of the job's class.</summary>
        public AgentWorker()
        {
            Name = this.GetType().Name;
        }

        /// <summary>
        /// This is where the worker performs its work and returns a WorkResult.</summary>
        public abstract WorkerResult Run(IDalSessionFactory factory, BackgroundWorker worker, DoWorkEventArgs e);
    }
}

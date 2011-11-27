using System;
using System.Collections;
using System.Diagnostics;

namespace B4F.TotalGiro.Jobs.Manager
{
    /// <summary>
    /// This is the .NET Agent Framework's base class for an ignorable
    /// exception.</summary>
    /// <remarks>
    /// The AgentIgnorableException defines an exception that may be ignored
    /// under certain circumstances.</remarks>
    public class AgentIgnorableException
    {
        /// <summary>
        /// Use this TraceSwitch when tracing in derived classes.</summary>
        protected static TraceSwitch IgnorableExceptionSwitch 
        {
          get { return _ignorableExceptionSwitch; }
        }

        /// <summary>
        /// Name of the ignorable exception.</summary>
        public string Name
        {
          get { return _name; }
          set { _name = value; }
        }

        /// <summary>
        /// Exception class of the ignorable exception.
        /// If not specified, then Message should be specified.</summary>
        public string Exception
        {
          get { return _exception; }
          set { _exception = value; }
        }

        /// <summary>
        /// Message property of the ignorable exception.
        /// If not specified, then Exception should be specified.</summary>
        public string Message
        {
          get { return _message; }
          set { _message = value; }
        }

        /// <summary>
        /// Maximum number of consecutives times this exception may be ignored.</summary>
        public int MaximumConsecutiveIgnoreCount
        {
          get { return _maximumConsecutiveIgnoreCount; }
          set { _maximumConsecutiveIgnoreCount = value; }
        }

        /// <summary>
        /// The duration to wait before retrying the job after this exception has been ignored.</summary>
        public long RetryDelayMilliseconds
        {
          get { return _retryDelayMilliseconds; }
          set { _retryDelayMilliseconds = value; }
        }

        private static TraceSwitch _ignorableExceptionSwitch = new TraceSwitch("AgentException", "AgentIgnorableException TraceSwitch");

        /// <summary>
        /// Default the name.</summary>
        public AgentIgnorableException()
        {
            Name = this.GetType().Name;
        }

        #region Privates

        private string _name;
        private string _exception = string.Empty;
        private string _message = string.Empty;
        private int _maximumConsecutiveIgnoreCount = 3;
        private long _retryDelayMilliseconds = 20000;

        #endregion
    
    }//class


    /// <summary>
    /// This is the .NET Agent Framework's ignorable exception collection.</summary>
    /// <remarks>
    /// IgnorableExceptionCollection derives from CollectionBase.</remarks>
    [Serializable]
    public class IgnorableExceptionCollection : CollectionBase
    {
        /// <summary>
        /// Add an ignorable exception to the collection.</summary>
        public void Add(AgentIgnorableException exception) 
        {
            List.Add(exception);
        }

        /// <summary>
        /// Remove an ignorable exception from the collection.</summary>
        public void Remove(AgentIgnorableException exception)
        {
            List.Remove(exception);
        }

        /// <summary>
        /// Set or retrieve an ignorable exception at the specific index in the collection.</summary>
        public AgentIgnorableException this[int index] 
        {
            get 
            {
	            return (AgentIgnorableException) List[index];
            }
            set 
            {
	            List[index] = value;
            }
        }

        /// <summary>
        /// Search for a matching ignorable exception in the collection.</summary>
        public AgentIgnorableException Find(Exception e) 
        {
          foreach (AgentIgnorableException eb in this) 
          {
            if ( ((eb.Message.Length == 0) || (eb.Message == e.Message))
              && ((eb.Exception.Length == 0) || (eb.Exception == e.GetType().ToString())) )
              return eb;
          }
          return null;
        }
    }
}

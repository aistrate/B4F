using System;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Jobs
{
    /// <summary>
    /// This enumeration defines the result status of a worker.</summary>
    /// <remarks>
    /// This enumeration uses the FlagsAttribute and defines statuses
    /// for Ok, Warning, Critical, and Exception. Two additional values
    /// are combinations of the others - All and WarningAndCritical.</remarks>
    [Serializable]
    [Flags]
    public enum WorkerResultStatus
    {
        Ok = 0x00000001,
        Cancelled = 0x00000010,
        Warning = 0x00000100,
        Critical = 0x00001000,
        Exception = 0x10000000,
        All = Ok | Cancelled | Warning | Critical | Exception,
        WarningAndCritical = Warning | Critical | Exception
    }

    /// <summary>
    /// This class defines the entire result of a worker.</summary>
    /// <remarks>
    /// The result includes the state, which is an arbitrary number defined
    /// by the worker class itself, the WorkerResultStatus, a short message,
    /// a detailed message, and a worker exception.</remarks>
    [Serializable]
    public class WorkerResult
    {
        #region Constructor

        /// <summary>
        /// Constructor to intialize with minimal values.</summary>
        public WorkerResult(int state, WorkerResultStatus status)
        {
            _state = state;
            _status = status;
        }

        /// <summary>
        /// Constructor to intialize with values including a short message.</summary>
        public WorkerResult(int state, WorkerResultStatus status, string shortMessage)
        {
            _state = state;
            _status = status;
            _shortMessage = shortMessage;
        }

        /// <summary>
        /// Constructor to intialize with values including short and detailed messages.</summary>
        public WorkerResult(int state, WorkerResultStatus status, string shortMessage, string detailedMessage)
        {
            _state = state;
            _status = status;
            _shortMessage = shortMessage;
            _detailedMessage = detailedMessage;
        }

        /// <summary>
        /// Constructor to intialize with values when there is an exception.</summary>
        public WorkerResult(int state, WorkerResultStatus status, string shortMessage, string detailedMessage, Exception workerException)
        {
            _state = state;
            _status = status;
            _shortMessage = shortMessage;
            _detailedMessage = detailedMessage;
            _workerException = workerException;
        }

        #endregion

        #region Props

        /// <summary>
        /// Worker-specific state number that should change if the state changes.</summary>
        /// <remarks>
        /// Notifier classes use a change in this number to indicate a change in
        /// state of the job for notification purposes.</remarks>
        public int State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Result of running the worker.</summary>
        public WorkerResultStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        /// Short message to be delivered by a notifier.</summary>
        public string ShortMessage
        {
            get { return _shortMessage; }
            set { _shortMessage = value; }
        }

        /// <summary>
        /// Detailed message to be delivered by a notifier.</summary>
        public string DetailedMessage
        {
            get 
            { 
                string message = ((_detailedMessage + string.Empty) != string.Empty ? _detailedMessage : ShortMessage);
                if (WorkerException != null)
                    message += Environment.NewLine + Util.GetMessageFromException(WorkerException);
                return message;
            }
            set { _detailedMessage = value; }
        }

        /// <summary>
        /// Exception encountered by the worker.</summary>
        public Exception WorkerException
        {
            get { return _workerException; }
            set { _workerException = value; }
        }

        public DateTime TimeFinished
        {
            get { return _timeFinished; }
            set { _timeFinished = value; }
        }

        #endregion

        #region Privates

        private int _state;
        private WorkerResultStatus _status;
        private string _shortMessage;
        private string _detailedMessage;
        private Exception _workerException;
        private DateTime _timeFinished = DateTime.Now;

        /// <summary>
        /// The value to be used for the WorkerResult's state if
        /// an exception is encountered. Exceptions may be handled in
        /// the Run method, but uncaught exceptions may be handled
        /// elsewhere and will use this value.</summary>
        public const int STATE_EXCEPTION = int.MinValue;
        public const int STATE_NORMAL = 0;

        #endregion
    }
}

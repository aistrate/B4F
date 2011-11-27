using System;

namespace B4F.TotalGiro.Jobs.Manager.History
{
    public interface IJobHistory
    {
        int Key { get; set; }
        int ManagementCompanyID { get; }
        DateTime CreationDate { get; }
        DateTime StartTime { get; }
        DateTime EndTime { get; }
        string Job { get; }
        string JobComponent { get; }
        WorkerResultStatus Status { get; }
        int RetryCount { get; }
        string Details { get; }
    }
}

using System;

namespace B4F.TotalGiro.Jobs.Manager.Notifier
{
    public interface IJobNotification
    {
        int Key { get; set; }
        int ManagementCompanyID { get; }
        DateTime CreationDate { get; }
        DateTime NotificationDate { get; }
        WorkerResultStatus Status { get; }
        string Details { get; }
        string ErrorDetails { get; }
    }
}

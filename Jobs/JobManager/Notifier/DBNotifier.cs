using System;

namespace B4F.TotalGiro.Jobs.Manager.Notifier
{
    [Serializable]
    public class DBNotifier : AgentNotifier
    {
        protected override void Notify(WorkerResult result)
        {
            // Log stuff
            JobNotification note = new JobNotification(result);
            JobNotificationMapper.Insert(this.Parent.GetSession(), note);
        }
    }
}

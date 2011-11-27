using System;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Jobs.Manager.Notifier
{
    public class JobNotificationMapper
    {
        /// <summary>
        /// Either creates or edits a JobNotification object in the database
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">Object of type JobNotification</param>
        public static void Insert(IDalSession session, JobNotification obj)
        {
            session.Insert(obj);
        }
    }
}

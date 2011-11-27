using System;
using System.Collections.Generic;
using B4F.TotalGiro.Notifications;

namespace B4F.TotalGiro.CRM.Contacts
{
    public interface IContactNotificationsCollection : IList<Notification>
    {
        string DisplayMessages { get; }
        NotificationTypes DisplayNotificationType { get; }
        string GetAllMessages(bool HTMLLayout, out NotificationTypes notificationType);
    }
}

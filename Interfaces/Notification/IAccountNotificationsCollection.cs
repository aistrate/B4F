using System;
using System.Collections.Generic;
using B4F.TotalGiro.Notifications;

namespace B4F.TotalGiro.Accounts
{
    public interface IAccountNotificationsCollection : IList<Notification>
    {
        IAccountTypeCustomer Parent { get; }
        string DisplayMessages { get; }
        NotificationTypes DisplayNotificationType { get; }
        string GetAllMessages(bool HTMLLayout, out NotificationTypes notificationType);
    }
}

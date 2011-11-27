using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Notifications;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Accounts
{
    public class AccountNotificationsCollection : TransientDomainCollection<Notification>, IAccountNotificationsCollection
    {
        public AccountNotificationsCollection() : base() { }
        public AccountNotificationsCollection(IAccountTypeCustomer parent)
            : base()
        {
            this.Parent = parent;
        }

        public virtual IAccountTypeCustomer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual string DisplayMessages
        {
            get { return Notification.GetAllMessages(Parent, true, out this.notificationType); }
        }

        public virtual NotificationTypes DisplayNotificationType
        {
            get { return this.notificationType; }
        }

        public virtual string GetAllMessages(bool HTMLLayout, out NotificationTypes notificationType)
        {
            return Notification.GetAllMessages(Parent, HTMLLayout, out notificationType);
        }

        #region Private Variables

        private IAccountTypeCustomer parent;
        private NotificationTypes notificationType = NotificationTypes.Info;

        #endregion
    }

}

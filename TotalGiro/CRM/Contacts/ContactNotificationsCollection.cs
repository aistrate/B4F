using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Notifications;

namespace B4F.TotalGiro.CRM.Contacts
{
    public class ContactNotificationsCollection : TransientDomainCollection<Notification>, IContactNotificationsCollection
    {
        public ContactNotificationsCollection() : base() { }
        public ContactNotificationsCollection(IContact parent, IList bagOfNotifications)
            : base()
        {
            this.Parent = parent;
        }

        public IContact Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual string DisplayMessages
        {
            get { return Notification.GetAllMessages(this, true, out this.notificationType); }
        }

        public virtual NotificationTypes DisplayNotificationType
        {
            get { return this.notificationType; }
        }

        public string GetAllMessages(bool HTMLLayout, out NotificationTypes notificationType)
        {
            return Notification.GetAllMessages(this, HTMLLayout, out notificationType);
        }

        #region Private Variables

        private IContact parent;
        private NotificationTypes notificationType = NotificationTypes.Info;

        #endregion
    }

}

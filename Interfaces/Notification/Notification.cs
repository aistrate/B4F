using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Notifications
{
    /// <summary>
    /// The type of the notification
    /// </summary>
    public enum NotificationTypes
    {
        /// <summary>
        /// This is just some information to display
        /// </summary>
        Info,
        /// <summary>
        /// This is a warning
        /// </summary>
        Warning
    }
   
    /// <summary>
    /// Notifications for accounts, contacts or whatever
    /// </summary>
    public class Notification
    {
        #region Constructor

        public Notification() { }

        public Notification(string message, NotificationTypes notificationType, DateTime startDate, DateTime dueDate, string createdBy)
        {
            Message = message;
            NotificationType = notificationType;
            StartDate = startDate;
            if (Util.IsNotNullDate(dueDate))
                DueDate = dueDate;
            CreatedBy = createdBy;
        }

        #endregion

        #region Props

        public virtual int Key { get; internal set; }
        public virtual string Message { get; set; }
        public virtual NotificationTypes NotificationType { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime DueDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual bool IsActive 
        {
            get
            {
                if (Util.IsNotNullDate(DueDate) && DueDate < DateTime.Today)
                    this.isActive = false;
                return this.isActive;
            }
            set { this.isActive = value; }
        }

        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
        }

        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }

        public virtual string DisplayMessage
        {
            get 
            { 
                NotificationTypes notificationType;
                return Notification.GetAllMessages(new Notification[] { this }, true, out notificationType, true); 
            }
        }

        public virtual IList<IAccountTypeCustomer> Accounts
        {
            get { return accounts; }
        }


        public virtual IList<IContact> Contacts
        {
            get { return contacts; }
        }

        #endregion

        #region Methods

        public static string GetAllMessages(IAccountTypeCustomer account, bool HTMLLayout, out NotificationTypes notificationType)
        {
            if (account == null)
                throw new ApplicationException("The account can not be null");

            IList<Notification> notifications = account.Notifications;
            if (account.PrimaryAccountHolder != null && 
                account.PrimaryAccountHolder.Contact != null &&
                account.PrimaryAccountHolder.Contact.Notifications.Count > 0)
            {
                foreach (Notification notification in account.PrimaryAccountHolder.Contact.Notifications)
	            {
                    if (notification.IsActive && !notifications.Contains(notification))
                        notifications.Add(notification);
	            }
            }
            return GetAllMessages(notifications, HTMLLayout, out notificationType);
        }

        public static string GetAllMessages(IList<Notification> notifications, bool HTMLLayout, out NotificationTypes notificationType)
        {
            return GetAllMessages(notifications, HTMLLayout, out notificationType, false);
        }

        public static string GetAllMessages(IList<Notification> notifications, bool HTMLLayout, out NotificationTypes notificationType, bool showAlways)
        {
            string message = "";
            notificationType = NotificationTypes.Info;

            foreach (Notification ntf in notifications.OrderBy(x => x.StartDate))
            {
                if (showAlways || ntf.IsActive && (Util.IsNullDate(ntf.DueDate) || ntf.DueDate > DateTime.Today))
                {
                    string createdBy = "";
                    if (ntf.CreatedBy != "")
                        createdBy = "by " + ntf.CreatedBy + " ";

                    message += (message != "" ? "\r\n\r\n" : "") + string.Format(
@"******************************************************************
    {0} {1}
******************************************************************

{2}

******************************************************************",
    ntf.StartDate.ToString("dd-MM-yyyy"), createdBy, ntf.Message);

                    if (ntf.NotificationType == NotificationTypes.Warning)
                        notificationType = NotificationTypes.Warning;
                }
            }
            if (message != "" && HTMLLayout)
                return message.Replace("\r\n", "<br/>");
            else
                return message;
        }

        #endregion

        #region Private Variables

        private DateTime creationDate = DateTime.Now;
        private DateTime lastUpdated;
        private bool isActive = true;
        private IList<IAccountTypeCustomer> accounts = new TransientDomainCollection<IAccountTypeCustomer>();
        private IList<IContact> contacts = new TransientDomainCollection<IContact>();

        #endregion

    }
}

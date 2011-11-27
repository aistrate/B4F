using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Notifications;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public class NotificationDetails
    {
        public NotificationDetails()
        {
            this.NotificationKey = 0;
            this.NotificationTypeId = int.MinValue;
            Selection = new List<AccountContactSelectedDetails>();
        }

        public NotificationDetails(Notification notification)
            : this()
        {
            this.NotificationKey = notification.Key;
            this.Message = notification.Message;
            this.NotificationTypeId = (int)notification.NotificationType;
            this.StartDate = notification.StartDate;
            this.DueDate = notification.DueDate;

            if (notification.Contacts.Count > 0)
            {
                foreach (IContact contact in notification.Contacts)
                    Selection.Add(new AccountContactSelectedDetails(contact.Key, AccountContactSelectedTypes.Contact, contact.FullName + " (" + contact.GetBSN + ")"));
            }

            if (notification.Accounts.Count > 0)
            {
                foreach (IAccountTypeCustomer account in notification.Accounts)
                    Selection.Add(new AccountContactSelectedDetails(account.Key, AccountContactSelectedTypes.Account, account.FullName + " (" + account.Number + ")"));
            }
        }

        public int NotificationKey { get; set; }
        public string Message { get; set; }
        public int NotificationTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsActive { get; set; }

        public List<AccountContactSelectedDetails> Selection { get; set; }

    }
}

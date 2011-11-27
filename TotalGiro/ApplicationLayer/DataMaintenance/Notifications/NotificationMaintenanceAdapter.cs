using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Notifications;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.ApplicationLayer.UC;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class NotificationMaintenanceAdapter
    {
        public static DataSet GetNotificationTypes()
        {
            DataSet ds = Util.GetDataSetFromEnum(typeof(NotificationTypes));
            Utility.AddEmptyFirstRow(ds);
            return ds;
        }

        public static DataSet GetNotifications(int assetManagerId, string accountNumber, 
            string contactName, string bsN_KvK, int notificationTypeId, bool showActive, bool showInactive)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable();

                if (notificationTypeId >= 0)
                    parameters.Add("notificationTypeId", notificationTypeId);
                if (!((showActive && showInactive) || (!showActive && !showInactive)))
                    parameters.Add("activityFilter", showActive ? true : false);
                if (assetManagerId != 0)
                    parameters.Add("assetManagerId", assetManagerId);
                if (!string.IsNullOrEmpty(contactName))
                    parameters.Add("contactName", Util.PrepareNamedParameterWithWildcard(contactName, MatchModes.Anywhere));
                if (!string.IsNullOrEmpty(bsN_KvK))
                    parameters.Add("bsN_KvK", Util.PrepareNamedParameterWithWildcard(bsN_KvK, MatchModes.Anywhere));

                // if only accountnumber -> skip contacts
                if (!string.IsNullOrEmpty(accountNumber) && string.IsNullOrEmpty(contactName) && string.IsNullOrEmpty(bsN_KvK))
                    parameters.Add("contactName", Util.PrepareNamedParameterWithWildcard("&@##$$", MatchModes.Exact));

                // Get the contact notifications
                IList notificationsContact = session.GetListByNamedQuery(
                    "B4F.TotalGiro.Notifications.GetContactNotifications",
                    parameters);

                // Now get the account notifications
                if (string.IsNullOrEmpty(contactName) && parameters.ContainsKey("contactName"))
                    parameters.Remove("contactName");

                if (!string.IsNullOrEmpty(accountNumber))
                    parameters.Add("accountNumber", Util.PrepareNamedParameterWithWildcard(accountNumber, MatchModes.Anywhere));

                //if (!string.IsNullOrEmpty(contactName) && !string.IsNullOrEmpty(accountNumber))
                //    parameters.Add("numberAndName", 1);
                //else if (!string.IsNullOrEmpty(contactName))
                //    parameters.Add("nameOnly", 1);
                //else if (!string.IsNullOrEmpty(accountNumber))
                //    parameters.Add("numberOnly", 1);

                IList notificationsAccount = session.GetListByNamedQuery(
                    "B4F.TotalGiro.Notifications.GetAccountNotifications",
                    parameters);

                var allNotifications = from a in notificationsContact.Cast<object[]>().Union(notificationsAccount.Cast<object[]>())
                                       select new { Notification = (Notification)a[0], ToWho = (string)a[1] };

                return allNotifications.GroupBy(g => g.Notification.Key)
                        .Select(g =>
                        new
                        {
                            g.Key,
                            g.First().Notification.NotificationType,
                            g.First().Notification.Message,
                            g.First().Notification.DisplayMessage,
                            g.First().Notification.StartDate,
                            g.First().Notification.DueDate,
                            g.First().Notification.IsActive,
                            g.First().Notification.CreatedBy,
                            g.First().Notification.CreationDate,
                            ToWho = String.Join(",", g.Select(x => x.ToWho).ToArray())
                        })
                        .ToDataSet();
            }
        }

        public static NotificationDetails GetNotificationData(int notificationId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Notification not = session.GetTypedList<Notification>(notificationId).FirstOrDefault();
                if (not != null)
                    return new NotificationDetails(not);
                else
                    throw new ApplicationException("Could not retrieve notification " + notificationId.ToString());
            }
        }

        public static bool SaveNotificationData(NotificationDetails details)
        {
            bool success = false;

            if (details.Selection == null || details.Selection.Count == 0)
                throw new ApplicationException("It is mandatory to attach at least one contact or account.");
            
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Notification notif = null;
                if (details.NotificationKey != 0)
                    notif = session.GetTypedList<Notification>(details.NotificationKey).FirstOrDefault();
                else
                {
                    notif = new Notification();
                    notif.CreatedBy = Security.SecurityManager.CurrentUser;
                }

                notif.NotificationType = (NotificationTypes)details.NotificationTypeId;
                notif.Message = details.Message;
                notif.StartDate = details.StartDate;
                notif.DueDate = details.DueDate;
                if (Util.IsNotNullDate(notif.DueDate) && notif.DueDate < DateTime.Today)
                    notif.IsActive = false;
                else
                    notif.IsActive = true;

                // First -> delete attached accounts / contacts that are removed
                if (notif.Key != 0)
                {
                    for (int i = notif.Accounts.Count; i > 0; i--)
                    {
                        var account = notif.Accounts[i - 1];
                        if (!details.Selection.Any(x => x.SelectedType == AccountContactSelectedTypes.Account && x.EntityKey == account.Key))
                            notif.Accounts.Remove(account);
                    }
                    for (int i = notif.Contacts.Count; i > 0; i--)
                    {
                        var contact = notif.Contacts[i - 1];
                        if (!details.Selection.Any(x => x.SelectedType == AccountContactSelectedTypes.Contact && x.EntityKey == contact.Key))
                            notif.Contacts.Remove(contact);
                    }
                }

                // Add the new attached accounts / contacts
                foreach (var item in details.Selection)
                {
                    if (item.SelectedType == AccountContactSelectedTypes.Account)
                    {
                        if (notif.Accounts == null || !notif.Accounts.Any(x => x.Key == item.EntityKey))
                        {
                            IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, item.EntityKey);
                            notif.Accounts.Add(account);
                        }
                    }
                    else if (item.SelectedType == AccountContactSelectedTypes.Contact)
                    {
                        if (notif.Contacts == null || !notif.Contacts.Any(x => x.Key == item.EntityKey))
                        {
                            IContact contact = ContactMapper.GetContact(session, item.EntityKey);
                            notif.Contacts.Add(contact);
                        }
                    }
                }
                success = session.InsertOrUpdate(notif);
            }
            return success;
        }

        public static bool DeActivateNotification(int notificationId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Notification notif = session.GetTypedList<Notification>(notificationId).FirstOrDefault();
                if (notif == null)
                    throw new ApplicationException("Could not find notification " + notificationId.ToString());

                notif.DueDate = DateTime.Today;
                notif.IsActive = false;

                return session.Update(notif);
            }
        }
    }
}

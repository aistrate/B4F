using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class AttachCounterAccountToContactEditAdapter
    {
        public static void AddCounterAccount(int contactID, int counterAccountID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IContact contact = ContactMapper.GetContact(session, contactID);
            ICounterAccount acc = CounterAccountMapper.GetCounterAccount(session, counterAccountID);
            contact.CounterAccounts.Add(acc);
            ContactMapper.Update(session, contact);
            session.Close();
        }
    }
}

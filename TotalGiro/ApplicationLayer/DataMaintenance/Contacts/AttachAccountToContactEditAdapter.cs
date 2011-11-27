using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Stichting;
using System.Data;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class AttachAccountToContactEditAdapter
    {
        public static string GetName(int contactID)
        {
            string name = "";
            IDalSession session = NHSessionFactory.CreateSession();

            IContact contact = (IContact)ContactMapper.GetContact(session, contactID);
            if (contact != null)
            {
                name = contact.FullName;
            }

            session.Close();
            return name;
        }

        public static void AddAccountHolder(int newContactID, int accountNrID)
        {
            AccountEditAdapter.AddAccountHolder(newContactID, accountNrID);
        }

        public static DataSet GetCustomerAccounts(int assetManagerId, string accountNumber, string propertyList)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return AccountMapper.GetCustomerAccounts(session, assetManagerId, 0, 0, 0, 0,
                    accountNumber, null, false, false, true, false, 0, true, false)
                    .ToDataSet(propertyList);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using System.Data;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.CRM.Contacts;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public static class ucAccountsEditAdapter
    {
        public static DataSet GetAccountInfo(int contactID)
        {

            IDalSession session = null;
            DataSet ds = null;
            session = NHSessionFactory.CreateSession();
            ArrayList listAccounts = new ArrayList();

            IContact contact = ContactMapper.GetContact(session, contactID);
            if (contact != null && contact.AccountHolders != null)
            {
                foreach (IAccountHolder contactAH in contact.AccountHolders)
                {
                    ICustomerAccount acc = contactAH.GiroAccount;

                    AccountInfo accountInfo = new AccountInfo();
                    accountInfo.AccountKey = acc.Key;
                    accountInfo.Number = contactAH.GiroAccount.Number;
                    accountInfo.ShortName = acc.ShortName;
                    foreach (IAccountHolder accAH in acc.AccountHolders)
                    {
                        if (accAH.IsPrimaryAccountHolder)
                        {
                            IContact primaryContact = accAH.Contact;
                            accountInfo.PrimaryAhName = primaryContact.FullName;
                        }

                    }
                    accountInfo.ContactKey = contact.Key;
                    listAccounts.Add(accountInfo);
                }
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                     listAccounts, "AccountKey, ContactKey, Number, PrimaryAhName, ShortName");
            }
            session.Close();
            return ds;
        }

        public class AccountInfo
        {
            public AccountInfo() { }
            private int accountKey;
            private int contactKey;
            private string number;
            private string primaryAhName;
            private string shortName;

            public int AccountKey
            {
                get { return accountKey; }
                set { accountKey = value; }
            }

            public int ContactKey
            {
                get { return contactKey; }
                set { contactKey = value; }
            }

            public string Number
            {
                get { return number; }
                set { number = value; }
            }

            public string PrimaryAhName
            {
                get { return primaryAhName; }
                set { primaryAhName = value; }
            }

            public string ShortName
            {
                get { return shortName; }
                set { shortName = value; }
            }

        }

        public static DataSet GetContactAccountHolders(int contactID)
        {
            IDalSession session = null;
            DataSet ds = null;
            session = NHSessionFactory.CreateSession();

            IContact contact = ContactMapper.GetContact(session, contactID);
            if (contact != null)
            {
                IContactAccountHoldersCollection ahColl = contact.AccountHolders;

                IAccountHolder[] ahList = new IAccountHolder[ahColl.Count];
                ahColl.CopyTo(ahList, 0);

                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                     ahList, "GiroAccount.Key, GiroAccount.Number, GiroAccount.ShortName");
            }

            session.Close();
            return ds;
        }

        public static DataSet GetFirstOfCollectionAccountHolder(int accountID)
        {
            DataSet ds = null;
            if (accountID != 0)
            {
                IDalSession session = NHSessionFactory.CreateSession();
                IAccount account = AccountMapper.GetAccount(session, accountID);
                if (account.AccountType == AccountTypes.Customer)
                {
                    ICustomerAccount acc = (ICustomerAccount)account;

                    if (acc != null && acc.AccountHolders != null && acc.AccountHolders.Count > 0)
                    {
                        IAccountHolder[] listAH = new IAccountHolder[1];
                        listAH[0] = acc.AccountHolders[0];

                        ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                             listAH, "Contact.Key, Contact.CurrentNAW.ResidentialAddress.Street," +
                                     "Contact.CurrentNAW.ResidentialAddress.HouseNumber," +
                                     "Contact.CurrentNAW.ResidentialAddress.HouseNumberSuffix," +
                                     "Contact.CurrentNAW.ResidentialAddress.PostalCode," +
                                     "Contact.CurrentNAW.ResidentialAddress.City");
                    }
                }

                session.Close();
            }
            return ds;
        }

        public static DataSet GetAccountAccountHolders(int accountID)
        {
            DataSet ds = null;
            if (accountID != 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    IAccountTypeCustomer acc = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountID);
                    if (acc.AccountHolders != null)
                    {
                        // Contact.Key, Contact.CurrentNAW.Name, IsPrimaryAccountHolder, Contact.FullName, GiroAccount.ShortName
                        ds = acc.AccountHolders
                            .Select(c => new
                            {
                                Contact_Key = c.Contact.Key,
                                Contact_CurrentNAW_Name = c.Contact.CurrentNAW.Name,
                                c.IsPrimaryAccountHolder,
                                Contact_FullName = c.Contact.FullName,
                                GiroAccount_ShortName = c.GiroAccount.ShortName,
                                c.CreationDate
                            })
                            .ToDataSet();
                    }
                }
            }
            return ds;
        }

        public static void DetachAccountHolder(int accountKey, int contactKey)
        {
            AccountEditAdapter.DetachAccountHolder(accountKey, contactKey);
        }
    }
}

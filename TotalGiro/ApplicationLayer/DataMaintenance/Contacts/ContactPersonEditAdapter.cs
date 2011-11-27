using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using System.Collections;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public class ContactPersonEditAdapter
    {
        public static void DetachContactPerson(int companyID, int contactPersonID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IContactCompany comp = (IContactCompany)ContactMapper.GetContact(session, companyID);
            IContactPerson pers = (IContactPerson)ContactMapper.GetContact(session, contactPersonID);
            ICompanyContactPerson companyPerson = CompanyContactPersonMapper.GetCompanyContactPerson(session, comp, pers);
            if (companyPerson != null)
            {
                CompanyContactPersonMapper.Delete(session, companyPerson);
            }
            session.Close();
        }

        public static void DetachCounterAccount(int contactPersonID, int counterAccountID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IContactPerson pers = (IContactPerson)ContactMapper.GetContact(session, contactPersonID);
            ICounterAccount acc = CounterAccountMapper.GetCounterAccount(session, counterAccountID);
            if (pers != null && acc != null)
            {
                // check if counter account not attached
                foreach (IAccountHolder holder in pers.AccountHolders)
                {
                    if (holder.GiroAccount.CounterAccount != null && holder.GiroAccount.CounterAccount.Equals(acc))
                        throw new ApplicationException(string.Format("Counter Account {0} is still attached to account {1}. Remove the counter account first from the account in order to dettach it.", acc.DisplayName, holder.GiroAccount.DisplayNumberWithName));
                }
                
                pers.CounterAccounts.Remove(acc);
                ContactMapper.Update(session, pers);
            }
            session.Close();
        }

        public static DataSet GetCounterAccounts(int personKey)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            IContactPerson pers = (IContactPerson)ContactMapper.GetContact(session, personKey);
            if (pers != null)
            {
                ICounterAccountCollection collAccounts = pers.CounterAccounts;
                ICounterAccount[] listAccounts = new ICounterAccount[collAccounts.Count];
                collAccounts.CopyTo(listAccounts, 0);

                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                     listAccounts, "Key, Number, AccountName, BankName, IsPublic");
            }
            session.Close();
            return ds;
        }

        public static DataSet GetCompanyContactPersons(int personKey)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            IContactPerson pers = (IContactPerson)ContactMapper.GetContact(session, personKey);
            if (pers != null)
            {
                ICompanyContactPersonCollection collCompanyContacts = pers.ContactCompanies;
                ICompanyContactPerson[] listCompanies = new ICompanyContactPerson[collCompanyContacts.Count];
                collCompanyContacts.CopyTo(listCompanies, 0);

                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    listCompanies, 
                    @"Company.Key, Company.CurrentNAW.Name, Company.KvKNumber,
                    Company.CurrentNAW.ResidentialAddress.DisplayAddress,
                    Company.ContactDetails.Email");
            }
            session.Close();
            return ds;
        }

        public static DataSet GetPersons(int assetManagerId, string accountNumber, string contactName)
        {
            IDalSession session;
            DataSet ds = null;
            session = NHSessionFactory.CreateSession();

            IAssetManager assetManager = null;

            if (assetManagerId > 0)
                assetManager = (IAssetManager)ManagementCompanyMapper.GetAssetManager(session, assetManagerId);

            IList collContacts = ContactMapper.GetContacts(session, assetManager, accountNumber, contactName, true, false, ContactTypeEnum.Person);

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                       collContacts,
                                       @"Key, FullName, CurrentNAW.Name, GetBSN, GetBirthFounding, ContactDetails.Email, 
                                       ContactDetails.Telephone.Number, ContactDetails.Mobile.Number, 
                                       CurrentNAW.ResidentialAddress.DisplayAddress");

            session.Close();
            return ds;
        }

        public static DataSet GetCompanies(int assetManagerId, string accountNumber, string contactName)
        {
            IDalSession session;
            DataSet ds = null;
            IAssetManager assetManager = null;

            session = NHSessionFactory.CreateSession();

            if (assetManagerId > 0)
                assetManager = (IAssetManager)ManagementCompanyMapper.GetAssetManager(session, assetManagerId);

            IList collContacts = ContactMapper.GetContacts(session, assetManager, accountNumber, contactName, true, false, ContactTypeEnum.Company);

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                       collContacts,
                                       @"Key, CurrentNAW.Name, GetBSN, ContactDetails.Email, 
                                       ContactDetails.Telephone.Number, ContactDetails.Mobile.Number, 
                                       CurrentNAW.ResidentialAddress.DisplayAddress");
            session.Close();
            return ds;
        }
    }
}

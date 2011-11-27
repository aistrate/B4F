using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.CRM;
using System.Web;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class ContactOverviewAdapter
    {
        public static string GetContactType(int key)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ContactTypeEnum? contactType = ContactMapper.GetContactType(session, key);
            session.Close();
            return (contactType != null ? contactType.ToString() : "");
        }

        public static DataSet GetContacts(int assetManagerId, string accountNumber, string contactName)
        {
           return GetContacts(assetManagerId, accountNumber, contactName, null,  true, false);
        }

        public static DataSet GetContacts(int assetManagerId, string accountNumber, string contactName, string bsN_KvK)
        {
            return GetContacts(assetManagerId, accountNumber, contactName, bsN_KvK, true, false);
        }

        public static DataSet GetContacts(int assetManagerID, string accountNumber, string contactName, 
            string bsN_KvK, bool contactActive, bool contactInactive)
        {
            IDalSession session;
            DataSet ds = null;
            session = NHSessionFactory.CreateSession();

            IAssetManager assetManager = null;

            if (assetManagerID > 0)
                assetManager = (IAssetManager)ManagementCompanyMapper.GetAssetManager(session, assetManagerID);

            IList collContacts = ContactMapper.GetContacts(session, assetManager, accountNumber, contactName, bsN_KvK, contactActive, contactInactive);

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                       collContacts,
                                       @"Key, ContactType, IsActive, GetBSN, GetBirthFounding, StatusNAR, 
                                       FullName, CurrentNAW.Name, CurrentNAW.ResidentialAddress.DisplayAddress");

            session.Close();
            return ds;
        }

        public static void UpdateStatusContact(bool status, int Key)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IContact contact = (IContact)ContactMapper.GetContact(session, Key);
            contact.IsActive = status;
            ContactMapper.Update(session, contact);

            session.Close();
        }


    }
}
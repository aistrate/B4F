using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Accounts;
using System.Collections;


namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class CompanyContactPersonEditAdapter
    {
        public static string GetContactName(int contactID)
        {
            return AccountEditAdapter.GetContactName(contactID);
        }
         
        public static void AddContactPerson(int compID, int persID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IContactCompany comp = (IContactCompany)ContactMapper.GetContact(session, compID);
            IContactPerson pers = (IContactPerson)ContactMapper.GetContact(session, persID);
            ICompanyContactPerson contactPerson = new CompanyContactPerson(pers, comp);

            if (!comp.CompanyContacts.Contains(contactPerson))
            {
                comp.CompanyContacts.Add(contactPerson);
                ContactMapper.Update(session, comp);
            }
            session.Close();
        }


    }
}

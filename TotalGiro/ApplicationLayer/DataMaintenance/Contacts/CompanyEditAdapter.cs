using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.CRM;
using System.Web;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.CRM.Contacts;
using System.Data;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Security;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public class CompanyDetails
    {
        public string CompanyName, DateOfFoundation, KvKNumber, Email, Mobile,
            Fax, Telephone, TelephoneAH, Street, HouseNumber, HouseNumberSuffix,
            Postalcode, City, Country, PostalStreet, PostalHouseNumber, PostalHouseNumberSuffix,
            PostalPostalcode, PostalCity, PostalCountry, InternetEnabled, Introducer, IntroducerEmployee, 
            IsActive;
    }

    public static class CompanyEditAdapter
    {

        private static IContactCompany GetContact(IDalSession session, int contactID)
        {
            return ContactMapper.GetContact(session, contactID) as IContactCompany;
        }

        public static CompanyDetails GetCompany(int key)
        {
            CompanyDetails compDetails = null;
            IDalSession session = NHSessionFactory.CreateSession();
            IContactDetails contactDetails;
            Address postalAddress;
            Address residentialAddress;
            IContactsNAW naw;

            IContactCompany company = GetContact(session, key);

            if (company != null)
            {
                compDetails = new CompanyDetails();

                compDetails.IsActive = company.IsActive.ToString();


                if (company.CurrentIntroducer != null)
                {
                    IContactsIntroducer contactsIntroducer = company.CurrentIntroducer;
                    compDetails.Introducer = contactsIntroducer.Remisier.Key.ToString();
                    compDetails.IntroducerEmployee = contactsIntroducer.RemisierEmployee.Key.ToString();
                }

                if (company.InternetEnabled.ToString().Length > 0)
                    compDetails.InternetEnabled = company.InternetEnabled.ToString();
                else
                    compDetails.InternetEnabled = InternetEnabled.Unknown.ToString();

                compDetails.KvKNumber = company.KvKNumber;

                if (company.DateOfFounding != null)
                    compDetails.DateOfFoundation = Convert.ToString(company.DateOfFounding);

                if (company.ContactDetails != null)
                {
                    contactDetails = company.ContactDetails;
                    if (contactDetails.Mobile != null)
                        compDetails.Mobile = contactDetails.Mobile.Number;

                    if (contactDetails.Fax != null)
                        compDetails.Fax = contactDetails.Fax.Number;

                    if (contactDetails.Telephone != null)
                        compDetails.Telephone = contactDetails.Telephone.Number;

                    if (contactDetails.TelephoneAH != null)
                        compDetails.TelephoneAH = contactDetails.TelephoneAH.Number;

                    compDetails.Email = company.ContactDetails.Email;
                }

                if (company.ContactsNAWs != null)
                {
                    naw = company.CurrentNAW;
                    postalAddress = naw.PostalAddress;
                    residentialAddress = naw.ResidentialAddress;

                    compDetails.CompanyName = naw.Name;

                    if (residentialAddress != null)
                    {
                        compDetails.Street = residentialAddress.Street;
                        compDetails.HouseNumber = residentialAddress.HouseNumber;
                        compDetails.HouseNumberSuffix = residentialAddress.HouseNumberSuffix;
                        compDetails.Postalcode = residentialAddress.PostalCode;
                        compDetails.City = residentialAddress.City;
                        if (residentialAddress.Country != null)
                            compDetails.Country = residentialAddress.Country.Key.ToString();
                    }

                    if (postalAddress != null)
                    {
                        compDetails.PostalStreet = postalAddress.Street;
                        compDetails.PostalHouseNumber = postalAddress.HouseNumber;
                        compDetails.PostalHouseNumberSuffix = postalAddress.HouseNumberSuffix;
                        compDetails.PostalPostalcode = postalAddress.PostalCode;
                        compDetails.PostalCity = postalAddress.City;
                        if (postalAddress.Country != null)
                            compDetails.PostalCountry = postalAddress.Country.Key.ToString();
                    }
                }
            }

            session.Close();
            return compDetails;
        }

        public static void SaveCompany(ref int compID, ref bool blnSave, CompanyDetails compDetails)
        {
            if (!SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
                throw new System.Security.SecurityException("You are not authorized to update contact details.");
            
            IDalSession session = NHSessionFactory.CreateSession(); ;
            
            try
            {
                IContactCompany comp = null;
                Address postalAddress = null;
                Address residentialAddress = null;
                IContactsNAW newNaw = null;
                IContactsNAW currentNaw = null;
                IContactsIntroducer currentIntroducer = null;
                IContactsIntroducer newIntroducer = null;

                bool boolNawInsert = true;

                if (compID != 0)
                {
                    comp = GetContact(session, compID);
                    currentNaw = comp.CurrentNAW;
                    currentIntroducer = comp.CurrentIntroducer;
                }
                else
                {
                    comp = new ContactCompany();
                    currentNaw = new ContactsNAW();
                }

                comp.IsActive = Convert.ToBoolean(compDetails.IsActive);

                if (compDetails.DateOfFoundation.Length > 0)
                    comp.DateOfFounding = Convert.ToDateTime(compDetails.DateOfFoundation);
                else
                    comp.DateOfFounding = DateTime.MinValue;

                if (comp.ContactDetails == null)
                    comp.ContactDetails = new ContactDetails();
                if (comp.ContactDetails.Fax == null)
                    comp.ContactDetails.Fax = new TelephoneNumber();
                if (comp.ContactDetails.Telephone == null)
                    comp.ContactDetails.Telephone = new TelephoneNumber();
                if (comp.ContactDetails.Mobile == null)
                    comp.ContactDetails.Mobile = new TelephoneNumber();
                if (comp.ContactDetails.TelephoneAH == null)
                    comp.ContactDetails.TelephoneAH = new TelephoneNumber();

                if (compDetails.InternetEnabled.Length > 0)
                {
                    if (compDetails.InternetEnabled.Equals(InternetEnabled.No.ToString()))
                        comp.InternetEnabled = InternetEnabled.No;
                    else if (compDetails.InternetEnabled.Equals(InternetEnabled.Yes.ToString()))
                        comp.InternetEnabled = InternetEnabled.Yes;
                    else
                        comp.InternetEnabled = InternetEnabled.Unknown;
                }
                else
                    comp.InternetEnabled = InternetEnabled.Unknown;

                if (compDetails.Introducer != null && compDetails.Introducer.Length > 0 && Convert.ToInt32(compDetails.Introducer) != int.MinValue)
                {
                    newIntroducer = new ContactsIntroducer();
                    newIntroducer.Remisier = RemisierMapper.GetRemisier(session, Convert.ToInt32(compDetails.Introducer));
                    newIntroducer.RemisierEmployee = RemisierEmployeeMapper.GetRemisierEmployee(session, Convert.ToInt32(compDetails.IntroducerEmployee));
                }

                comp.ContactDetails.Email = compDetails.Email;
                comp.ContactDetails.Fax.Number = compDetails.Fax;
                comp.ContactDetails.Mobile.Number = compDetails.Mobile;
                comp.ContactDetails.Telephone.Number = compDetails.Telephone;
                comp.ContactDetails.TelephoneAH.Number = compDetails.TelephoneAH;

                if (compDetails.Street.Length > 0 || compDetails.HouseNumber.Length > 0 ||
                    compDetails.HouseNumberSuffix.Length > 0 || compDetails.Postalcode.Length > 0 ||
                    compDetails.City.Length > 0)
                {
                    residentialAddress = new Address();
                    residentialAddress.Street = compDetails.Street;
                    residentialAddress.HouseNumber = compDetails.HouseNumber;
                    residentialAddress.HouseNumberSuffix = compDetails.HouseNumberSuffix;
                    residentialAddress.PostalCode = compDetails.Postalcode;
                    residentialAddress.City = compDetails.City;
                    if (int.Parse(compDetails.Country) != int.MinValue)
                        residentialAddress.Country = CountryMapper.GetCountry(session, Convert.ToInt32(compDetails.Country));
                    else
                        residentialAddress.Country = null;
                }

                if (compDetails.PostalStreet.Length > 0 || compDetails.PostalHouseNumber.Length > 0 ||
                    compDetails.PostalHouseNumberSuffix.Length > 0 || compDetails.PostalPostalcode.Length > 0 ||
                    compDetails.PostalCity.Length > 0)
                {
                    postalAddress = new Address();
                    postalAddress.Street = compDetails.PostalStreet;
                    postalAddress.HouseNumber = compDetails.PostalHouseNumber;
                    postalAddress.HouseNumberSuffix = compDetails.PostalHouseNumberSuffix;
                    postalAddress.PostalCode = compDetails.PostalPostalcode;
                    postalAddress.City = compDetails.PostalCity;
                    if (int.Parse(compDetails.PostalCountry) != int.MinValue)
                        postalAddress.Country = CountryMapper.GetCountry(session, Convert.ToInt32(compDetails.PostalCountry));
                    else
                        postalAddress.Country = null;
                }

                if (residentialAddress != null && postalAddress == null)
                    postalAddress = residentialAddress;
                else if (residentialAddress == null && postalAddress != null)
                    residentialAddress = postalAddress;

                newNaw = new ContactsNAW(compDetails.CompanyName, postalAddress, residentialAddress);

                if (currentNaw.Name == null || (currentNaw != null && !currentNaw.Name.Equals(newNaw.Name)))
                    boolNawInsert = true;
                // Then compare addresses
                else if (currentNaw.Name.Equals(newNaw.Name) && residentialAddress != null)
                {
                    if (currentNaw.ResidentialAddress != null)
                    {
                        // If addresses the same: no need for an insert
                        if (newNaw.Equals(currentNaw))
                            boolNawInsert = false;
                    }
                    else
                        boolNawInsert = true;
                }
                else
                {
                    newNaw = new ContactsNAW();
                    newNaw.Name = compDetails.CompanyName;
                    boolNawInsert = true;
                }

                if (boolNawInsert)
                {
                    comp.ContactsNAWs.Add(newNaw);
                    comp.CurrentNAW = newNaw;
                }

                if (currentIntroducer != null || newIntroducer != null)
                {
                    if (!(currentIntroducer != null && newIntroducer != null &&
                            newIntroducer.Equals(currentIntroducer)))
                    {
                        comp.ContactsIntroducers.Add(newIntroducer);
                        comp.CurrentIntroducer = newIntroducer;
                    }
                }

                if (compID == 0 && !LoginMapper.IsLoggedInAsStichting(session))
                    comp.AssetManager = (IAssetManager)LoginMapper.GetCurrentManagmentCompany(session);

                comp.KvKNumber = compDetails.KvKNumber;

                if (comp.CurrentNAW.Name.Length > 0 &&
                     comp.KvKNumber.Length > 0 &&
                     Util.IsNotNullDate(comp.DateOfFounding) &&
                     comp.InternetEnabled != InternetEnabled.Unknown &&
                     (comp.ContactDetails != null &&
                      comp.ContactDetails.Email.Length > 0 &&
                      comp.ContactDetails.Telephone != null &&
                      comp.ContactDetails.Telephone.Number.Length > 0) &&
                     residentialAddress != null &&
                     residentialAddress.Street.Length > 0 &&
                     residentialAddress.HouseNumber.Length > 0 &&
                     residentialAddress.PostalCode.Length > 0 &&
                     residentialAddress.City.Length > 0)
                {
                    comp.StatusNAR = EnumStatusNAR.Complete;
                }
                else
                    comp.StatusNAR = EnumStatusNAR.Incomplete;

                if (comp.CreationDate.Year < 2000) comp.CreationDate = DateTime.Now;

                if (compID == 0)
                    blnSave = ContactMapper.Insert(session, comp);
                else
                    blnSave = ContactMapper.Update(session, comp);

                compID = comp.Key;
            }
            finally
            {
                session.Close();
            }
        }

        public static string GetCompanyName(int contactID)
        {
            string companyName = "";
            IDalSession session = NHSessionFactory.CreateSession();

            IContactCompany comp = GetContact(session, contactID);
            if (comp != null)
            {
                companyName = comp.FullName;
            }

            session.Close();
            return companyName;
        }

        public static void DetachCounterAccount(int companyID, int counterAccountID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IContactCompany company = (IContactCompany)ContactMapper.GetContact(session, companyID);
            ICounterAccount acc = CounterAccountMapper.GetCounterAccount(session, counterAccountID);
            if (company != null && acc != null)
            {
                // check if counter account not attached
                foreach (IAccountHolder holder in company.AccountHolders)
                {
                    if (holder.GiroAccount.CounterAccount != null && holder.GiroAccount.CounterAccount.Equals(acc))
                        throw new ApplicationException(string.Format("Counter Account {0} is still attached to account {1}. Remove the counter account first from the account in order to dettach it.", acc.DisplayName, holder.GiroAccount.DisplayNumberWithName));
                }

                company.CounterAccounts.Remove(acc);
                ContactMapper.Update(session, company);
            }
            session.Close();
        }

        public static DataSet GetCounterAccounts(int companyID)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            IContactCompany company = (IContactCompany)ContactMapper.GetContact(session, companyID);
            if (company != null)
            {
                ICounterAccountCollection collAccounts = company.CounterAccounts;
                ICounterAccount[] listAccounts = new ICounterAccount[collAccounts.Count];
                collAccounts.CopyTo(listAccounts, 0);

                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                     listAccounts, "Key, Number, AccountName, BankName, IsPublic");
            }
            session.Close();
            return ds;
        }

        public static DataSet GetCompanyContactPersons(int companyKey)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            IContactCompany comp = GetContact(session, companyKey);
            if (comp != null)
            {
                ICompanyContactPersonCollection collContactPerson = comp.CompanyContacts;

                ICompanyContactPerson[] listContactPerson = new ICompanyContactPerson[collContactPerson.Count];
                collContactPerson.CopyTo(listContactPerson, 0);

                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                     listContactPerson, @"ContactPerson.Key, ContactPerson.FullName, ContactPerson.CurrentNAW.Name, 
                        ContactPerson.GetBSN, ContactPerson.GetBirthFounding, 
                        ContactPerson.CurrentNAW.ResidentialAddress.DisplayAddress, 
                        AuthorizedSignature");
                // ContactPerson.ContactDetails.Email, ContactPerson.ContactDetails.Telephone.Number, ContactPerson.ContactDetails.Mobile.Number, 
            }
            session.Close();
            return ds;
        }

        public static void DetachContactperson(int companyID, int contactPersonID)
        {
            ContactPersonEditAdapter.DetachContactPerson(companyID, contactPersonID);
        }
    }
}

 using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.CRM;
using System.Web;
using B4F.TotalGiro.StaticData;
using System.Data;
using System.Collections;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Security;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{

    // helper class
    public class PersonDetails
    {
        public string LastName, Initials, MiddleName, BirthDate, Gender, InternetEnabled, Nationality,
            Title, IDType, IDNumber, IDExpirationDate, BurgerServiceNummer, Email, Mobile,
            Fax, Telephone, TelephoneAH, Street, HouseNumber, HouseNumberSuffix, 
            Postalcode, City, Country, PostalStreet, PostalHouseNumber, PostalHouseNumberSuffix,
            PostalPostalcode, PostalCity, PostalCountry, Introducer, IsActive, IntroducerEmployee, ResidentialState;
        public bool HasMinimumData, SendNewsItem;
    }

    public static class PersonEditAdapter
    {
        private static IContactPerson GetContact(IDalSession session, int contactID)
        {
            return ContactMapper.GetContact(session, contactID) as IContactPerson;
        }

        public static void SavePerson(ref int persID, ref bool blnSaveSuccess, PersonDetails persDetails)
        {
            if (!SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
                throw new System.Security.SecurityException("You are not authorized to update contact details.");
            
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IContactPerson person = null;
                Address postalAddress = null;
                Address residentialAddress = null;
                IContactsNAW newNaw = null;
                IContactsNAW currentNaw = null;
                IContactsIntroducer currentIntroducer = null;
                IContactsIntroducer newIntroducer = null;

                bool boolNawInsert = true;
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                bool loggedInAsStichting = company.IsStichting;

                if (persID != 0)
                {
                    person = GetContact(session, persID);
                    currentNaw = person.CurrentNAW;
                    currentIntroducer = person.CurrentIntroducer;
                }
                else
                {
                    person = new ContactPerson();
                    currentNaw = new ContactsNAW();
                }

                // check if bsn does not already exists
                long bsnEntries = session.Session.GetNamedQuery(
                    "B4F.TotalGiro.CRM.Contact.CheckBSNIsUnique")
                    .SetParameter("personId", persID)
                    .SetParameter("companyId", (person.AssetManager != null ? person.AssetManager.Key : company.Key))
                    .SetParameter("bsn", persDetails.BurgerServiceNummer)
                    .UniqueResult<long>();
                if (bsnEntries > 0)
                    throw new ApplicationException("It is not possible to enter this person since the bsn already exists");

                person.IsActive = Convert.ToBoolean(persDetails.IsActive);
                person.HasMinimumData = persDetails.HasMinimumData;

                if (person.Identification == null)
                    person.Identification = new Identification();
                if (person.Identification.IdentificationType == null)
                    person.Identification.IdentificationType = new IdentificationType();
                if (person.ContactDetails == null)
                    person.ContactDetails = new ContactDetails();
                if (person.ContactDetails.Fax == null)
                    person.ContactDetails.Fax = new TelephoneNumber();
                if (person.ContactDetails.Telephone == null)
                    person.ContactDetails.Telephone = new TelephoneNumber();
                if (person.ContactDetails.Mobile == null)
                    person.ContactDetails.Mobile = new TelephoneNumber();
                if (person.ContactDetails.TelephoneAH == null)
                    person.ContactDetails.TelephoneAH = new TelephoneNumber();

                if (persDetails.Introducer != null && persDetails.Introducer.Length > 0 && Convert.ToInt32(persDetails.Introducer) != int.MinValue)
                {
                    newIntroducer = new ContactsIntroducer();
                    newIntroducer.Remisier = RemisierMapper.GetRemisier(session, Convert.ToInt32(persDetails.Introducer));
                    newIntroducer.RemisierEmployee = RemisierEmployeeMapper.GetRemisierEmployee(session, Convert.ToInt32(persDetails.IntroducerEmployee));
                }

                if (persDetails.InternetEnabled.Length > 0)
                {
                    if (persDetails.InternetEnabled.Equals(InternetEnabled.No.ToString()))
                        person.InternetEnabled = InternetEnabled.No;
                    else if (persDetails.InternetEnabled.Equals(InternetEnabled.Yes.ToString()))
                        person.InternetEnabled = InternetEnabled.Yes;
                    else
                        person.InternetEnabled = InternetEnabled.Unknown;
                }
                else
                    person.InternetEnabled = InternetEnabled.Unknown;

                if (int.Parse(persDetails.IDType) != int.MinValue)
                {
                    person.Identification.Number = persDetails.IDNumber;

                    if (persDetails.IDExpirationDate.Length > 0)
                    {
                        person.Identification.ValidityPeriod = Convert.ToDateTime(persDetails.IDExpirationDate);
                    }
                    else
                    {
                        person.Identification.ValidityPeriod = DateTime.MinValue;
                    }
                    person.Identification.IdentificationType.Key = Convert.ToInt32(persDetails.IDType);
                }
                else
                    person.Identification = null;

                person.ContactDetails.SendNewsItem = persDetails.SendNewsItem;
                person.ContactDetails.Email = persDetails.Email;
                person.ContactDetails.Mobile.Number = persDetails.Mobile;
                person.ContactDetails.Telephone.Number = persDetails.Telephone;
                person.ContactDetails.TelephoneAH.Number = persDetails.TelephoneAH;
                person.ContactDetails.Fax.Number = persDetails.Fax;

                if (persDetails.Street.Length > 0 || persDetails.HouseNumber.Length > 0 ||
                    persDetails.HouseNumberSuffix.Length > 0 || persDetails.Postalcode.Length > 0 ||
                    persDetails.City.Length > 0)
                {
                    residentialAddress = new Address();
                    residentialAddress.Street = persDetails.Street;
                    residentialAddress.HouseNumber = persDetails.HouseNumber;
                    residentialAddress.HouseNumberSuffix = persDetails.HouseNumberSuffix;
                    residentialAddress.PostalCode = persDetails.Postalcode;
                    residentialAddress.City = persDetails.City;
                    if (int.Parse(persDetails.Country) != int.MinValue)
                        residentialAddress.Country = CountryMapper.GetCountry(session, Convert.ToInt32(persDetails.Country));
                    else
                        residentialAddress.Country = null;
                }

                if (persDetails.PostalStreet.Length > 0 || persDetails.PostalHouseNumber.Length > 0 ||
                    persDetails.PostalHouseNumberSuffix.Length > 0 || persDetails.PostalPostalcode.Length > 0 ||
                    persDetails.PostalCity.Length > 0)
                {
                    postalAddress = new Address();
                    postalAddress.Street = persDetails.PostalStreet;
                    postalAddress.HouseNumber = persDetails.PostalHouseNumber;
                    postalAddress.HouseNumberSuffix = persDetails.PostalHouseNumberSuffix;
                    postalAddress.PostalCode = persDetails.PostalPostalcode;
                    postalAddress.City = persDetails.PostalCity;
                    if (int.Parse(persDetails.PostalCountry) != int.MinValue)
                        postalAddress.Country = CountryMapper.GetCountry(session, Convert.ToInt32(persDetails.PostalCountry));
                    else
                        postalAddress.Country = null;
                }

                if (residentialAddress != null && postalAddress == null)
                    postalAddress = residentialAddress;
                else if (residentialAddress == null && postalAddress != null)
                    residentialAddress = postalAddress;

                newNaw = new ContactsNAW(persDetails.LastName, postalAddress, residentialAddress);

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
                    newNaw.Name = persDetails.LastName;
                    boolNawInsert = true;
                }

                if (boolNawInsert)
                {
                    person.ContactsNAWs.Add(newNaw);
                    person.CurrentNAW = newNaw;
                }

                if (currentIntroducer != null || newIntroducer != null)
                {
                    if (!(currentIntroducer != null && newIntroducer != null &&
                            newIntroducer.Equals(currentIntroducer)))
                    {
                        person.ContactsIntroducers.Add(newIntroducer);
                        person.CurrentIntroducer = newIntroducer;
                    }
                }

                person.Title = persDetails.Title;
                person.FirstName = persDetails.Initials;
                person.MiddleName = persDetails.MiddleName;
                if (persDetails.Gender.Length > 0)
                    person.Gender = (Convert.ToInt32(persDetails.Gender).Equals((int)Gender.Male) ? Gender.Male : Gender.Female);
                if (persDetails.ResidentialState.Length > 0)
                    person.ResidentialState = (Convert.ToInt32(persDetails.ResidentialState).Equals((int)ResidentStatus.Resident) ? ResidentStatus.Resident : ResidentStatus.NonResident);

                if (int.Parse(persDetails.Nationality) != int.MinValue)
                {
                    if (person.Nationality == null || person.Nationality.Key != int.Parse(persDetails.Nationality))
                        person.Nationality = NationalityMapper.GetNationality(session, Convert.ToInt32(persDetails.Nationality));
                }
                person.BurgerServiceNummer = persDetails.BurgerServiceNummer;

                if (persDetails.BirthDate.Length > 0)
                    person.DateOfBirth = Convert.ToDateTime(persDetails.BirthDate);
                else
                    person.DateOfBirth = DateTime.MinValue;

                if (person.CurrentNAW.Name.Length > 0 && person.FirstName.Length > 0 &&
                        Util.IsNotNullDate(person.DateOfBirth) &&
                        persDetails.Gender.Length > 0 &&
                        person.Nationality != null &&
                        (person.Identification != null &&
                          person.Identification.Number.Length > 0 &&
                          Util.IsNotNullDate(person.Identification.ValidityPeriod)) &&
                        person.BurgerServiceNummer.Length > 0 &&
                        person.InternetEnabled != InternetEnabled.Unknown &&
                        (person.ContactDetails != null &&
                           person.ContactDetails.Email.Length > 0 &&
                           person.ContactDetails.Telephone != null &&
                           person.ContactDetails.Telephone.Number.Length > 0) &&
                       residentialAddress != null &&
                       residentialAddress.Street.Length > 0 &&
                       residentialAddress.HouseNumber.Length > 0 &&
                       residentialAddress.PostalCode.Length > 0 &&
                       residentialAddress.City.Length > 0)
                {
                    person.StatusNAR = EnumStatusNAR.Complete;
                }
                else
                    person.StatusNAR = EnumStatusNAR.Incomplete;

                if (persID == 0 && !loggedInAsStichting)
                    person.AssetManager = (IAssetManager)company;
                if (persID == 0)
                    blnSaveSuccess = ContactMapper.Insert(session, person);
                else
                {
                    blnSaveSuccess = ContactMapper.Update(session, person);
                }

                persID = person.Key;
            }
            finally
            {
                session.Close();
            }
        }

        public static DataSet GetNationalities()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                NationalityMapper.GetNationalities(session), "Key, Description");

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            session.Close();
            return ds;
        }

        public static DataSet GetCountries()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                CountryMapper.GetCountries(session), "Key, CountryName");
            Utility.AddEmptyFirstRow(ds.Tables[0]);

           session.Close();

            return ds;
        }

        public static DataSet FakeRetDs()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            INationality nat = NationalityMapper.GetNationality(session, 1);
            ArrayList natAL = new ArrayList();
            natAL.Add(nat);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                natAL, "Key");
            session.Close();
            return ds;
        }

        public static DataSet FakeRet()
        {
            // Create a new DataTable.
            System.Data.DataTable table = new DataTable("ParentTable");
            // Declare variables for DataColumn and DataRow objects.
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, 
            // ColumnName and add to DataTable.    
            column = new DataColumn();
            column.DataType = typeof(int);
            column.ColumnName = "id";
            column.ReadOnly = true;
            column.Unique = true;
            // Add the Column to the DataColumnCollection.
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = typeof(string);
            column.ColumnName = "ParentItem";
            column.AutoIncrement = false;
            column.Caption = "ParentItem";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            table.Columns.Add(column);

            // Make the ID column the primary key column.
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = table.Columns["id"];
            table.PrimaryKey = PrimaryKeyColumns;

            // Create three new DataRow objects and add 
            // them to the DataTable
            for (int i = 0; i <= 2; i++)
            {
                row = table.NewRow();
                row["id"] = i;
                row["ParentItem"] = "ParentItem " + i;
                table.Rows.Add(row);
            }
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(table);
            int count = dataSet.Tables[0].Rows.Count;
            return dataSet; 

        }

        public static DataSet GetIdentificationType()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                IdentificationTypeMapper.GetIdentificationType(session), "Key, IdType");

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            session.Close();

            return ds;
        }

        public static PersonDetails GetPerson(int key)
        {
            PersonDetails persDetails = null;
            IDalSession session = NHSessionFactory.CreateSession();
            IIdentification identification;
            IContactDetails contactDetails;
            Address postalAddress;
            Address residentialAddress;
            IContactsNAW naw;

            IContactPerson person = GetContact(session, key);

            if (person != null)
            {
                persDetails = new PersonDetails();

                if (person.CurrentIntroducer != null)
                {
                    IContactsIntroducer contactsIntroducer = person.CurrentIntroducer;
                    if (contactsIntroducer.Remisier != null)
                        persDetails.Introducer = contactsIntroducer.Remisier.Key.ToString();
                    if (contactsIntroducer.RemisierEmployee != null)
                        persDetails.IntroducerEmployee = contactsIntroducer.RemisierEmployee.Key.ToString();
                }
                persDetails.IsActive = person.IsActive.ToString();
                persDetails.HasMinimumData = person.HasMinimumData;

                if (person.InternetEnabled.ToString().Length > 0)
                    persDetails.InternetEnabled = person.InternetEnabled.ToString();
                else
                    persDetails.InternetEnabled = InternetEnabled.Unknown.ToString();

                persDetails.Title = person.Title;
                persDetails.Initials = person.FirstName;
                persDetails.MiddleName = person.MiddleName;
                persDetails.Gender = Convert.ToString((int)person.Gender);
                persDetails.ResidentialState = Convert.ToString((int)person.ResidentialState);
                if (person.Nationality != null)
                    persDetails.Nationality = person.Nationality.Key.ToString();
                persDetails.BurgerServiceNummer = person.BurgerServiceNummer;
                if (person.DateOfBirth != null)
                    persDetails.BirthDate = Convert.ToString(person.DateOfBirth);

                identification = person.Identification;
                if (identification != null)
                {
                    persDetails.IDType = Convert.ToString(identification.IdentificationType.Key);
                    persDetails.IDNumber = identification.Number;
                    persDetails.IDExpirationDate = Convert.ToString(identification.ValidityPeriod);
                }
                if (person.ContactDetails != null)
                {
                    contactDetails = person.ContactDetails;
                    if (contactDetails.Mobile != null)
                        persDetails.Mobile = contactDetails.Mobile.Number;

                    if (contactDetails.Fax != null)
                        persDetails.Fax = contactDetails.Fax.Number;

                    if (contactDetails.Telephone != null)
                        persDetails.Telephone = contactDetails.Telephone.Number;

                    if (contactDetails.TelephoneAH != null)
                        persDetails.TelephoneAH = contactDetails.TelephoneAH.Number;

                    persDetails.Email = person.ContactDetails.Email;
                    persDetails.SendNewsItem = person.ContactDetails.SendNewsItem;
                }

                if (person.CurrentNAW != null)
                {
                    naw = person.CurrentNAW;
                    postalAddress = naw.PostalAddress;
                    residentialAddress = naw.ResidentialAddress;

                    persDetails.LastName = naw.Name;

                    if (residentialAddress != null)
                    {
                        persDetails.Street = residentialAddress.Street;
                        persDetails.HouseNumber = residentialAddress.HouseNumber;
                        persDetails.HouseNumberSuffix = residentialAddress.HouseNumberSuffix;
                        persDetails.Postalcode = residentialAddress.PostalCode;
                        persDetails.City = residentialAddress.City;
                        if (residentialAddress.Country != null)
                            persDetails.Country = residentialAddress.Country.Key.ToString();
                    }

                    if (postalAddress != null)
                    {
                        persDetails.PostalStreet = postalAddress.Street;
                        persDetails.PostalHouseNumber = postalAddress.HouseNumber;
                        persDetails.PostalHouseNumberSuffix = postalAddress.HouseNumberSuffix;
                        persDetails.PostalPostalcode = postalAddress.PostalCode;
                        persDetails.PostalCity = postalAddress.City;
                        if (postalAddress.Country != null)
                            persDetails.PostalCountry = postalAddress.Country.Key.ToString();
                    }
                }

            }

            session.Close();
            return persDetails;
        }

        public static string GetPersonName(int contactID)
        {
            string persName = "";
            IDalSession session = NHSessionFactory.CreateSession();

            IContactPerson pers = GetContact(session, contactID);
            if (pers != null)
            {
                persName = pers.FullName;
            }

            session.Close();
            return persName;
        }
    }
}

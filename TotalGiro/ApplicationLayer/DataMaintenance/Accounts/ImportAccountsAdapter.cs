using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.CRM;
using B4F.DataMigration.EffectenGiro;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.CRM.Contacts;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Accounts
{
    public static class ImportAccountsAdapter
    {
        private const int VIERLANDER = 10;
        public const string VALIDATE_AANVRAAG = "dbo.TG_ValidateEGAanvraag";

        public static IList ImportAccountsFromEffectenGiro()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            prepareDataforImport(session);
            IList NewAccounts = new ArrayList();

            IList oldAccounts = B4F.DataMigration.EffectenGiro.EGAccountMapper.GetUnMappedAccounts(session);

            foreach (IEGAccount acc in oldAccounts)
            {
                ICustomerAccount iac = processAanvraag(session, acc);
                if (iac != null) NewAccounts.Add(iac);
            }

            return NewAccounts;
        }

        private static ICustomerAccount processAanvraag(IDalSession session, IEGAccount acc)
        {
            IEGAanvraag aacReq = acc.AccountRequest;

            //Most Important ... Manage Contact First
            IContact primary = null;
            IContact secondary = null;
            DateTime creationdate = session.GetServerTime();

            if (aacReq.IsPersonalAccount)
            {
                primary = CreateContactPerson(session, aacReq.SOFI,
                                                        aacReq.Voorletters,
                                                        aacReq.Tussenvoegsels,
                                                        aacReq.Naam,
                                                        aacReq.PostalAddress,
                                                        aacReq.ResidentialAddress1,
                                                        aacReq.ContactDetails1,
                                                        aacReq.PrimaryGender,
                                                        aacReq.Nationality1,
                                                        aacReq.Identification1,
                                                        aacReq.Geboortedatum,
                                                        acc.AssetManager, 
                                                        creationdate);
                if (aacReq.IsDualAccount)
                    secondary = CreateContactPerson(session, aacReq.PSOFI,
                                                        aacReq.PVoorletters,
                                                        aacReq.PTussenvoegsels,
                                                        aacReq.PNaam,
                                                        aacReq.PostalAddress,
                                                        aacReq.ResidentialAddress2,
                                                        aacReq.ContactDetails2,
                                                        aacReq.SecondaryGender,
                                                        aacReq.Nationality2,
                                                        aacReq.Identification2,
                                                        aacReq.PGeboortedatum,
                                                        acc.AssetManager,
                                                        creationdate);
            }
            else
            {
                primary = CreateContactCompany(session, aacReq.KVK,
                                                        aacReq.BNaam,
                                                        aacReq.PostalAddress,
                                                        aacReq.BAddress,
                                                        aacReq.BContactDetails,
                                                        aacReq.DatumOprichting,
                                                        acc.AssetManager,
                                                        creationdate);

                // contact person on company
                secondary = CreateContactPerson(session, aacReq.SOFI,
                                                        aacReq.Voorletters,
                                                        aacReq.Tussenvoegsels,
                                                        aacReq.Naam,
                                                        aacReq.PostalAddress,
                                                        aacReq.ResidentialAddress1,
                                                        aacReq.ContactDetails1,
                                                        aacReq.PrimaryGender,
                                                        aacReq.Nationality1,
                                                        aacReq.Identification1,
                                                        aacReq.Geboortedatum,
                                                        acc.AssetManager,
                                                        creationdate);

            }
            ICounterAccount counterAccount = CreateCounterAccount(session, primary, acc);

            // store contacts
            B4F.TotalGiro.CRM.ContactMapper.Update(session, primary);
            if (secondary != null) B4F.TotalGiro.CRM.ContactMapper.Update(session, secondary);

            // check for contactperson on company
            if (!aacReq.IsPersonalAccount && secondary != null)
            {
                ICompanyContactPerson compContactPerson = new CompanyContactPerson((IContactPerson)secondary, (IContactCompany)primary);

                if (!((IContactCompany)primary).CompanyContacts.Contains(compContactPerson))
                {
                    ((IContactCompany)primary).CompanyContacts.Add(compContactPerson);
                    ContactMapper.Update(session, primary);
                }
            }

            // get the family
            IAccountFamily family = AccountFamilyMapper.GetAccountFamily(session, acc.NummerPreFix);
            ICustomerAccount newAcc = CreateAccount(acc, creationdate, family);

            //add as accountHolders                    
            newAcc.AccountHolders.Add(new AccountHolder(newAcc, primary));
            newAcc.AccountHolders.SetPrimaryAccountHolder(primary);

            if (secondary != null)
            {
                newAcc.AccountHolders.Add(new AccountHolder(newAcc, secondary));
                secondary.CounterAccounts.Add(newAcc.CounterAccount);
            }

            //Add CounterAccount
            newAcc.CounterAccount = counterAccount;

            //set the account on the aanvraag
            acc.TGAccount = newAcc;

            //update all
            B4F.TotalGiro.Accounts.AccountMapper.Update(session, newAcc);

            //Set the Model History
            IInternalEmployeeLogin employee = (IInternalEmployeeLogin)LoginMapper.GetCurrentLogin(session);
            IModelHistory item = new ModelHistory(newAcc, newAcc.Lifecycle, newAcc.ModelPortfolio, newAcc.IsExecOnlyCustomer, newAcc.EmployerRelationship, employee, creationdate);
            newAcc.ModelPortfolioChanges.Add(item);
            B4F.TotalGiro.Accounts.AccountMapper.Update(session, newAcc);

            return newAcc;
        }

        private static IContactPerson CreateContactPerson(IDalSession session,
                                                        string BurgerServiceNummer,
                                                        string Voorletters,
                                                        string Tussenvoegsels,
                                                        string Naam,
                                                        Address PostalAddress,
                                                        Address ResidentialAddress,
                                                        IContactDetails ContactDetails,
                                                        Gender ContactGender,
                                                        INationality ContactNationality,
                                                        IIdentification Id,
                                                        DateTime Dob,
                                                        IManagementCompany AssetManager,
                                                        DateTime creationdate)
        {
            IContactPerson theContact = null;

            //1st see if the contact exists
            if (!LookupContactPersonByBSN(session, BurgerServiceNummer, out theContact))
            {
                if (!string.IsNullOrEmpty(Naam))
                {
                    theContact = new ContactPerson(Voorletters,
                                                    Tussenvoegsels,
                                                    ContactGender,
                                                    ContactNationality,
                                                    Naam,
                                                    PostalAddress,
                                                    ResidentialAddress,
                                                    ContactDetails);
                    theContact.BurgerServiceNummer = BurgerServiceNummer;
                    theContact.AssetManager = (IAssetManager)AssetManager;
                    theContact.Identification = Id;
                    theContact.DateOfBirth = Dob;
                    theContact.CreationDate = creationdate;
                }
            }
            return theContact;

        }

        private static IContactCompany CreateContactCompany(IDalSession session,
                                                        string KvKNummer,
                                                        string Naam,
                                                        Address PostalAddress,
                                                        Address ResidentialAddress,
                                                        IContactDetails ContactDetails,
                                                        DateTime dateOfFounding,
                                                        IManagementCompany AssetManager,
                                                        DateTime creationdate)
        {
            IContactCompany theCompany = null;

            if (!LookupContactCompanybyKVK(session, KvKNummer, out theCompany))
            {
                theCompany = new ContactCompany(Naam, PostalAddress, ResidentialAddress, ContactDetails, KvKNummer);
            }
            theCompany.DateOfFounding = dateOfFounding;
            theCompany.AssetManager = (IAssetManager)AssetManager;
            theCompany.CreationDate = creationdate;


            return theCompany;

        }

        private static ICompanyContactPerson CreateCompanyContactPerson(IDalSession session,
                                                        IContactCompany company,
                                                        string BurgerServiceNummer,
                                                        string Voorletters,
                                                        string Tussenvoegsels,
                                                        string Naam,
                                                        Address PostalAddress,
                                                        Address ResidentialAddress,
                                                        IContactDetails ContactDetails,
                                                        Gender ContactGender,
                                                        INationality ContactNationality,
                                                        IIdentification Id,
                                                        DateTime Dob,
                                                        IManagementCompany AssetManager,
                                                        DateTime creationdate)
        {
            ICompanyContactPerson cmpcp = null;
            IContactPerson contact = null;

            if (company != null && !string.IsNullOrEmpty(Naam))
            {
                if (!LookupContactPersonByBSN(session, BurgerServiceNummer, out contact))
                {
                    contact = new ContactPerson(Voorletters,
                                                    Tussenvoegsels,
                                                    ContactGender,
                                                    ContactNationality,
                                                    Naam,
                                                    PostalAddress,
                                                    ResidentialAddress,
                                                    ContactDetails);
                    contact.BurgerServiceNummer = BurgerServiceNummer;
                    contact.AssetManager = (IAssetManager)AssetManager;
                    contact.Identification = Id;
                    contact.DateOfBirth = Dob;
                    contact.CreationDate = creationdate;
                }
                cmpcp = new CompanyContactPerson(contact, company);
            }
            return cmpcp;
        }

        private static bool LookupContactPersonByBSN(IDalSession session, string BurgerServiceNummer, out IContactPerson lookUp)
        {
            if (BurgerServiceNummer != "000000000")
                lookUp = B4F.TotalGiro.CRM.ContactMapper.GetContactbyBSN(session, BurgerServiceNummer);
            else
                lookUp = null;

            if (lookUp != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static bool LookupContactCompanybyKVK(IDalSession session, string KVKNummer, out IContactCompany lookUp)
        {
            lookUp = B4F.TotalGiro.CRM.ContactMapper.GetContactbyKVK(session, KVKNummer);
            if (lookUp != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static ICustomerAccount CreateAccount(IEGAccount egAccount, DateTime creationDate, IAccountFamily family)
        {
            ICustomerAccount newAcc = new CustomerAccount(egAccount.Nummer, egAccount.AccountRequest.AccountName,
                                                egAccount.AssetManager, egAccount.AccountRequest.ModelPortfolio, creationDate);

            newAcc.Family = family;
            IEGAanvraag aacRequest = egAccount.AccountRequest;

            Decimal firstDeposit = 0m;
            if (decimal.TryParse(egAccount.AccountRequest.EersteInleg, out firstDeposit))
                newAcc.FirstPromisedDeposit = new B4F.TotalGiro.Instruments.Money(firstDeposit, newAcc.BaseCurrency);
            newAcc.ValuationsRequired = true;
            newAcc.IsExecOnlyCustomer = egAccount.AccountRequest.IsExecutionOnly;
            return newAcc;
        }

        private static ICounterAccount CreateCounterAccount(IDalSession session, IContact contact, IEGAccount egAccount)
        {
            if (contact == null)
                throw new ApplicationException("Contact is mandatory in creating a new account");
            
            IEGAanvraag aacRequest = egAccount.AccountRequest;

            ICounterAccount counterAcc = CounterAccountMapper.GetCounterAccount(session, aacRequest.TegenRekening);

            if (counterAcc == null)
            {
                counterAcc = new CounterAccount(aacRequest.TegenRekening,
                   aacRequest.TegenRekeningTNV,
                   aacRequest.Bank,
                   (aacRequest.Bank == null ? aacRequest.TegenRekeningBank : null),
                   new Address(aacRequest.TegenRekeningPlaats, null),
                   egAccount.AssetManager, false, null, true);
            }
            if (!contact.CounterAccounts.Contains(counterAcc))
                contact.CounterAccounts.Add(counterAcc);
            return counterAcc;
        }

        private static bool prepareDataforImport(IDalSession session)
        {
            Hashtable myparams = new Hashtable();
            return session.ExecuteStoredProcedure(VALIDATE_AANVRAAG, myparams);
        }



    }
}

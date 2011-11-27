using System;
using System.Data;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class CounterAccountEditAdapter
    {
        // helper class
        public class CounterAccountDetails
        {
            public string TegenrekeningNr, TegenrekTNV, TegenrekNameBank,
                BankStreet, BankHouseNumber, BankHouseNumberSuffix,
                BankPostalcode, BankCity,
                BeneficiaryStreet, BeneficiaryHouseNumber, BeneficiaryHouseNumberSuffix,
                BeneficiaryPostalcode, BeneficiaryCity;
            public int BankCountryID = int.MinValue, BeneficiaryCountryID = int.MinValue, BankID = int.MinValue;
            public int ManagementCompanyID;
            public bool IsPublic, IsBankAddressEmpty = false, IsBeneficiaryAddressEmpty = false;
            public bool UseElfProef = true;
        }

        public static void GetCounterAccountDetails(int counterAccountID, out CounterAccountDetails details)
        {
            details = null;
            IDalSession session = NHSessionFactory.CreateSession();
            ICounterAccount acc = CounterAccountMapper.GetCounterAccount(session, counterAccountID);

            if (acc != null)
            {
                details = new CounterAccountDetails();

                details.TegenrekeningNr = acc.Number;
                details.TegenrekTNV = acc.AccountName;
                details.TegenrekNameBank = acc.BankName;
                if (acc.Bank != null)
                {
                    details.BankID = acc.Bank.Key;
                    details.UseElfProef = acc.Bank.UseElfProef;
                }

                if (acc.BankAddress != null)
                {
                    details.BankStreet = acc.BankAddress.Street;
                    details.BankHouseNumber = acc.BankAddress.HouseNumber;
                    details.BankHouseNumberSuffix = acc.BankAddress.HouseNumberSuffix;
                    details.BankPostalcode = acc.BankAddress.PostalCode;
                    details.BankCity = acc.BankAddress.City;
                    if (acc.BankAddress.Country != null)
                        details.BankCountryID = acc.BankAddress.Country.Key;
                }
                details.ManagementCompanyID = acc.ManagementCompany.Key;
                details.IsPublic = acc.IsPublic;
                if (acc.BeneficiaryAddress != null)
                {
                    details.BeneficiaryStreet = acc.BeneficiaryAddress.Street;
                    details.BeneficiaryHouseNumber = acc.BeneficiaryAddress.HouseNumber;
                    details.BeneficiaryHouseNumberSuffix = acc.BeneficiaryAddress.HouseNumberSuffix;
                    details.BeneficiaryPostalcode = acc.BeneficiaryAddress.PostalCode;
                    details.BeneficiaryCity = acc.BeneficiaryAddress.City;
                    if (acc.BeneficiaryAddress.Country != null)
                        details.BeneficiaryCountryID = acc.BeneficiaryAddress.Country.Key;
                }
            }
            session.Close();
        }

        public static DataSet GetBanks()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                BankMapper.GetBanks(session),
                "Key, Name");
            session.Close();
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static bool GetBankAddressDetails(int bankID, out CounterAccountDetails details)
        {
            details = null;
            IDalSession session = NHSessionFactory.CreateSession();
            IBank bank = BankMapper.GetBank(session, bankID);

            if (bank != null && bank.Address != null && !bank.Address.IsEmpty)
            {
                details = new CounterAccountDetails();
                details.BankStreet = bank.Address.Street;
                details.BankHouseNumber = bank.Address.HouseNumber;
                details.BankHouseNumberSuffix = bank.Address.HouseNumberSuffix;
                details.BankPostalcode = bank.Address.PostalCode;
                details.BankCity = bank.Address.City;
                details.BankCountryID = bank.Address.Country.Key;
                details.UseElfProef = bank.UseElfProef;
            }
            session.Close();
            return true;
        }

        public static void SaveCounterAccount(ref int counterAccountID, int contactID, CounterAccountDetails details, out ContactTypeEnum contactType)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ICounterAccount acc = null;
            bool isNewAccount = false;

            if (counterAccountID == int.MinValue)
                isNewAccount = true;

            ICountry bankCountry = null;
            if (details.BankCountryID != int.MinValue)
                bankCountry = CountryMapper.GetCountry(session, details.BankCountryID);

            IBank bank = null;
            if (details.BankID != int.MinValue)
                bank = BankMapper.GetBank(session, details.BankID);

            Address bankAddress = null;
            if (!details.IsBankAddressEmpty)
            {
                bankAddress = new Address(
                                    details.BankStreet,
                                    details.BankHouseNumber,
                                    details.BankHouseNumberSuffix,
                                    details.BankPostalcode,
                                    details.BankCity,
                                    bankCountry);
            }

            // check bank Address not the same as the bank's adress
            if (bankAddress != null && !bankAddress.IsEmpty &&  bank != null && bank.Address != null && !bank.Address.IsEmpty)
            {
                if (bankAddress.Equals(bank.Address))
                    bankAddress = null;
            }

            Address beneficiaryAddress = null;
            if (!details.IsBeneficiaryAddressEmpty)
            {
                ICountry beneficiaryCountry = null;
                if (details.BeneficiaryCountryID != int.MinValue)
                    beneficiaryCountry = CountryMapper.GetCountry(session, details.BeneficiaryCountryID);

                beneficiaryAddress = new Address(
                                    details.BeneficiaryStreet,
                                    details.BeneficiaryHouseNumber,
                                    details.BeneficiaryHouseNumberSuffix,
                                    details.BeneficiaryPostalcode,
                                    details.BeneficiaryCity,
                                    beneficiaryCountry);
            }

            if (contactID == int.MinValue)
                throw new ApplicationException("The contact is mandatory to create a new Counter Account");

            IContact contact = ContactMapper.GetContact(session, contactID);
            if (contact == null)
                throw new ApplicationException("Could not find the contact");

            if (isNewAccount)
            {
                IManagementCompany am = LoginMapper.GetCurrentManagmentCompany(session);
                acc = new CounterAccount(details.TegenrekeningNr,
                                        details.TegenrekTNV,
                                        bank,
                                        details.TegenrekNameBank,
                                        bankAddress,
                                        am,
                                        details.IsPublic, 
                                        beneficiaryAddress);
                contact.CounterAccounts.Add(acc);
            }
            else
            {
                acc = CounterAccountMapper.GetCounterAccount(session, counterAccountID);
                if (acc != null)
                {
                    acc.Number = details.TegenrekeningNr;
                    acc.AccountName = details.TegenrekTNV;
                    acc.BankName = details.TegenrekNameBank;
                    acc.Bank = bank;
                    acc.BankAddress = bankAddress;
                    acc.BeneficiaryAddress = beneficiaryAddress;
                    acc.IsPublic = details.IsPublic;
                }
            }

            contactType = contact.ContactType;

            if (isNewAccount)
            {
                session.BeginTransaction();
                CounterAccountMapper.Insert(session, acc);
                ContactMapper.Update(session, contact);
                session.CommitTransaction();
            }
            else
                CounterAccountMapper.Update(session, acc);

            counterAccountID = acc.Key;

            session.Close();
        }
    }
}

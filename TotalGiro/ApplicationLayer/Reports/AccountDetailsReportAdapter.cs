using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.CRM;

namespace B4F.TotalGiro.ApplicationLayer.Reports
{
    public class AccountDetailsReportAdapter
    {
        public static DataSet getAccountDetails(Int32 accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ICustomerAccount account = (ICustomerAccount) B4F.TotalGiro.Accounts.AccountMapper.GetAccount(session, accountId);

            return getAccountDetails(account);

        }

        public static DataSet getAccountDetails(ICustomerAccount account)
        {
            AccountDetails details = new AccountDetails(account);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                new AccountDetails[] { details },
                "AccountNumber, VermogensBeheer, TegenRekening, TegenRekeningBank,TegenRekeningTNV, TegenRekeningPlaats,   ModelPortfolio, PLastName, SLastName, PVoorLetters, SVoorLetters, PTussenVoegsels, STussenVoegsels, PGeslacht, SGeslacht, PGeboortedatum, SGeboortedatum, PNationaliteit, SNationaliteit, PBSN, SBSN, PostalAdres1, PostalCode, WoonPlaats, PLegitimatie, SLegitimatie, PLegitimatieNummer, SLegitimatieNummer, PLegitimatieExpiry, SLegitimatieExpiry, PTelefoon, STelefoon, PMobiel, SMobiel, PEmail, SEmail",
                "AccountDetail");

            return ds;

        }

        public class AccountDetails
        {
            public AccountDetails(ICustomerAccount account)
            {
                this.account = account;
                this.AccountNumber = account.Number;
                if( account.ModelPortfolio != null) this.ModelPortfolio = account.ModelPortfolio.ModelName;
                this.Primary = account.PrimaryAccountHolder.Contact;
                this.VermogensBeheer = account.AccountOwner.CompanyName;
                if (account.CounterAccount != null)
                {
                    this.TegenRekening = account.CounterAccount.Number;
                    this.TegenRekeningBank = account.CounterAccount.BankName;
                    this.TegenRekeningTNV = account.CounterAccount.AccountName;
                    if (account.CounterAccount.BankAddress != null)
                        this.TegenRekeningPlaats = account.CounterAccount.BankAddress.City;
                }

                if (Primary.ContactType == ContactTypeEnum.Person)
                {
                    IContactPerson cPrimary = (IContactPerson) Primary;
                    this.PLastName = cPrimary.CurrentNAW.Name;
                    this.PVoorLetters = cPrimary.FirstName;
                    this.PTussenVoegsels = cPrimary.MiddleName;
                    this.PGeslacht = (cPrimary.Gender == B4F.TotalGiro.StaticData.Gender.Female) ? "Vrouw" : "Man";
                    this.PGeboortedatum = cPrimary.DateOfBirth;
                    if (cPrimary.Nationality != null) this.PNationaliteit = cPrimary.Nationality.Description;
                    this.PBSN = cPrimary.GetBSN;
                    this.PostalAdres1 = cPrimary.CurrentNAW.PostalAddress.AddressLine1;
                    this.PostalCode = cPrimary.CurrentNAW.PostalAddress.PostalCode;
                    this.WoonPlaats = cPrimary.CurrentNAW.PostalAddress.City;
                    if (cPrimary.Identification != null) this.PLegitimatie = cPrimary.Identification.IdentificationType.IdType;
                    if (cPrimary.Identification != null) this.PLegitimatieNummer = cPrimary.Identification.Number;
                    if (cPrimary.Identification != null) this.PLegitimatieExpiry = cPrimary.Identification.ValidityPeriod;
                    

                }


                if (account.EnOfAccountHolder != null)
                {
                    IContactPerson Secondary = (IContactPerson)account.EnOfAccountHolder.Contact;
                    this.SLastName = Secondary.CurrentNAW.Name;
                    this.SVoorLetters = Secondary.FirstName;
                    this.STussenVoegsels = Secondary.MiddleName;
                    this.SGeslacht = (Secondary.Gender == B4F.TotalGiro.StaticData.Gender.Female) ? "Vrouw" : "Man";
                    this.SGeboortedatum = Secondary.DateOfBirth;
                    if (Secondary.Nationality != null) this.SNationaliteit = Secondary.Nationality.Description;
                    this.SBSN = Secondary.GetBSN;
                    if(Secondary.Identification != null)  this.SLegitimatie = Secondary.Identification.IdentificationType.IdType;
                    if (Secondary.Identification != null) this.PLegitimatieNummer = Secondary.Identification.Number;
                    if (Secondary.Identification != null) this.SLegitimatieExpiry = Secondary.Identification.ValidityPeriod;

                }


            }
            public string AccountNumber { get; set; }
            public string ModelPortfolio { get; set; }
            public string VermogensBeheer { get; set; }
            public IContact Primary { get; set; }
            public string PLastName { get; set; }
            public string SLastName { get; set; }

            public string PVoorLetters { get; set; }
            public string SVoorLetters { get; set; }
            public string PTussenVoegsels { get; set; }
            public string STussenVoegsels { get; set; }
            public string PGeslacht { get; set; }
            public string SGeslacht { get; set; }

            public DateTime PGeboortedatum { get; set; }
            public DateTime SGeboortedatum { get; set; }
            public string PNationaliteit { get; set; }
            public string SNationaliteit { get; set; }
            public string PBSN { get; set; }
            public string SBSN { get; set; }

            public string PostalAdres1 { get; set; }
            public string PostalCode { get; set; }
            public string WoonPlaats { get; set; }

            public string PLegitimatie { get; set; }
            public string SLegitimatie { get; set; }
            public string PLegitimatieNummer { get; set; }
            public string SLegitimatieNummer { get; set; }
            public DateTime PLegitimatieExpiry { get; set; }
            public DateTime SLegitimatieExpiry { get; set; }

            public string PTelefoon { get; set; }
            public string STelefoon { get; set; }
            public string PMobiel { get; set; }
            public string SMobiel { get; set; }
            public string PEmail { get; set; }
            public string SEmail { get; set; }

            public string TegenRekening { get; set; }
            public string TegenRekeningTNV { get; set; }
            public string TegenRekeningBank { get; set; }
            public string TegenRekeningPlaats { get; set; }

            public ICustomerAccount account { get; set; }

        }
    }
}

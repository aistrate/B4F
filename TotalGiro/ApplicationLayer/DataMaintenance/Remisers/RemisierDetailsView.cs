using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.ApplicationLayer.Remisers
{
    public class RemisierDetailsView
    {
        public RemisierDetailsView()
        {
            this.RemisierKey = int.MinValue;
            this.CompanyID = int.MinValue;
            this.BankID = int.MinValue;
            this.IsActive = true;
        }

        public RemisierDetailsView(IRemisier remisier)
            : this()
        {
            this.RemisierKey = remisier.Key;
            this.CompanyID = remisier.AssetManager.Key;
            this.CompanyName = remisier.AssetManager.CompanyName;
            this.Name = remisier.Name;
            this.InternalRef = remisier.InternalRef;
            this.RemisierType = remisier.RemisierType;
            this.OfficeAddress = remisier.OfficeAddress;
            this.PostalAddress = remisier.PostalAddress;
            this.ContactPerson = remisier.ContactPerson;
            this.Email = remisier.Email;
            if (remisier.Telephone != null)
                this.Telephone = remisier.Telephone.Number;
            if (remisier.Fax != null)
                this.Fax = remisier.Fax.Number;
            if (remisier.BankDetails != null)
            {
                this.BankName = remisier.BankDetails.BankName;
                this.BankAccountNumber = remisier.BankDetails.AccountNumber;
                this.BankAccountName = remisier.BankDetails.BankAccountName;
                this.BankCity = remisier.BankDetails.BankAddress.City;
                if (remisier.BankDetails.Bank != null)
                    this.BankID = remisier.BankDetails.Bank.Key;
            }
            if (remisier.Employees != null)
                EmployeeCount = remisier.Employees.Count;

            if (remisier.ParentRemisier != null)
            {
                ParentRemisierKey = remisier.ParentRemisier.Key;
                ParentRemisierKickBackPercentage = remisier.ParentRemisierKickBackPercentage;
            }

            //this.ProvisieAfspraak = remisier.ProvisieAfspraak;
            this.DatumOvereenkomst = remisier.DatumOvereenkomst;
            this.NummerOvereenkomst = remisier.NummerOvereenkomst;
            this.NummerAFM = remisier.NummerAFM;
            this.NummerKasbank = remisier.NummerKasbank;
            this.IsActive = remisier.IsActive;
        }

        public int RemisierKey { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string InternalRef { get; set; }
        public RemisierTypes RemisierType { get; set; }
        public Address OfficeAddress { get; set; }
        public Address PostalAddress { get; set; }
        public Person ContactPerson { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public int BankID { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public string BankCity { get; set; }
        public int EmployeeCount { get; set; }
        public int? ParentRemisierKey { get; set; }
        public decimal ParentRemisierKickBackPercentage { get; set; }
        //public string ProvisieAfspraak { get; set; }
        public string DatumOvereenkomst { get; set; }
        public string NummerOvereenkomst { get; set; }
        public string NummerAFM { get; set; }
        public string NummerKasbank { get; set; }
        public bool IsActive { get; set; }

        public bool IsBankEmpty 
        {
            get
            {
                bool isEmpty = true;
                if (!string.IsNullOrEmpty(string.Format("{0}{1}{2}{3}", BankName, BankAccountNumber, BankAccountName, BankCity)) || BankID != int.MinValue)
                    isEmpty = false;
                return isEmpty;
            }
        }
    }
}

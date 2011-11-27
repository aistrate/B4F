using System;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.CRM
{
    public enum LoginStatus
    {
        NoEmail = 0,
        NoLogin = 1,
        NoPassword = 2,
        ActiveLogin = 3
    }

    public interface IContactPerson : IContact
    {
        string Title { get; set; }
        string FirstName { get; set; }
        string MiddleName { get; set; }
        Gender Gender { get; set; }
        INationality Nationality { get; set; }
        string BurgerServiceNummer { get; set; }
        DateTime DateOfBirth { get; set; }
        IIdentification Identification { get; set; }
        ICompanyContactPersonCollection ContactCompanies { get; set; }
        bool HasMinimumData { get; set; }
    }
}

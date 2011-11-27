using System;
namespace B4F.TotalGiro.CRM.Contacts
{
    public interface ICompanyContactPerson
    {
        AuthorizedSignatureEnum AuthorizedSignature { get; set; }
        IContactPerson ContactPerson { get; set; }
        IContactCompany Company { get; set; }
        int Key { get; set; }
    }
    public enum AuthorizedSignatureEnum
    {
        None,
        Independent,
        Common
    }

}

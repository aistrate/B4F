using System;
using B4F.TotalGiro.CRM.Contacts;

namespace B4F.TotalGiro.CRM
{
    public interface IContactCompany : IContact
    {
        ICompanyContactPersonCollection CompanyContacts { get; set; }
        DateTime DateOfFounding { get; set; }
        string KvKNumber { get; set; }
    }
}

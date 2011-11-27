using System;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.CRM
{
    public interface IContactsNAW
    {
        IContact Contact { get; set; }
        int Key { get; set; }
        string Name { get; set; }
        Address PostalAddress { get; set; }
        Address ResidentialAddress { get; set; }
        DateTime CreationDate { get; }
        ContactsFormatter Formatter { get; }
    }
}

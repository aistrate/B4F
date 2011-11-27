using System;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.CRM
{
    public interface IContactsIntroducer
    {
        int Key { get; set; }
        IContact Contact { get; }
        IRemisier Remisier { get; set; }
        IRemisierEmployee RemisierEmployee { get; set; }
        DateTime CreationDate { get; }
    }
}
using System;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.CRM
{
    public interface IIdentification
    {
        IIdentificationType IdentificationType { get; set; }
        string Number { get; set; }
        DateTime ValidityPeriod { get; set; }
    }
}

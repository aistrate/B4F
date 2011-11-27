using System;
using B4F.TotalGiro.Collections;
namespace B4F.TotalGiro.CRM.Contacts
{
    public interface ICompanyContactPersonCollection : IGenericCollection<ICompanyContactPerson>
    {
        IContact Parent { get; set; }
    }
}

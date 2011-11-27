using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Collections;
namespace B4F.TotalGiro.CRM
{
    public interface IContactCollection : IGenericCollection<IContact>
    {
        void Add(IContact item);
        bool Contains(IContact item);
        ICustomerAccount Parent { get; set; }
    }
}

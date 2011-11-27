using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.CRM.Contacts
{
    public interface IContactsNAWCollection : IGenericCollection<IContactsNAW>
    {
        IContact Parent { get; }
        IContactsNAW Current { get; }
    }
}

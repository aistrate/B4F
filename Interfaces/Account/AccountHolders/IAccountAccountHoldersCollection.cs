using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.CRM;

namespace B4F.TotalGiro.Accounts
{
    public interface IAccountAccountHoldersCollection : IGenericCollection<IAccountHolder>
    {
        IAccountTypeCustomer Parent { get; }
        IAccountHolder PrimaryAccountHolder { get; }
        IAccountHolder EnOfAccountHolder { get; }
        IAccountHolder GetItemByContact(IContact contact);
        void SetPrimaryAccountHolder(IContact contact);
    }
}

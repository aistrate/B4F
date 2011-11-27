using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.CRM;

namespace B4F.TotalGiro.Accounts
{
    public interface IContactAccountHoldersCollection : IList<IAccountHolder>
    {
        IContact Parent { get; }
        IAccountHolder PrimaryAccountHolder { get; }
    }
}

using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.CRM
{
    public interface ICounterAccountCollection : IGenericCollection<ICounterAccount>
    {
        IContact Parent { get; }
    }
}

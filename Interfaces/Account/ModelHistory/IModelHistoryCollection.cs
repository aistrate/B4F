using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Accounts.ModelHistory
{
    public interface IModelHistoryCollection : IGenericCollection<IModelHistory>
    {
        IAccountTypeCustomer Parent { get; }
        IModelHistory GetItemByDate(DateTime date);
    }
}

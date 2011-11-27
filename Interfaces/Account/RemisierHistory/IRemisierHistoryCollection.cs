using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Accounts.RemisierHistory
{
    public interface IRemisierHistoryCollection : IGenericCollection<IRemisierHistory>
    {
        ICustomerAccount Parent { get; }
        IRemisierHistory GetItemByDate(DateTime date);
    }
}

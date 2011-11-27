using System;
using System.Collections.Generic;
namespace B4F.TotalGiro.Accounts
{
    public interface IAccountFinancialTargetCollection :IList<IAccountFinancialTarget>
    {
        void AddAccountFinancialTarget(B4F.TotalGiro.Accounts.IAccountFinancialTarget entry);
        B4F.TotalGiro.Accounts.ICustomerAccount Parent { get; set; }
    }
}

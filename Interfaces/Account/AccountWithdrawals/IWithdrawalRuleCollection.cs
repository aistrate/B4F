using System;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Accounts.Withdrawals
{
    public interface IWithdrawalRuleCollection : IGenericCollection<IWithdrawalRule>
    {
        ICustomerAccount ParentAccount { get; }
    }
}

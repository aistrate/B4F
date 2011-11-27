using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Valuations
{
    public interface IDepositWithdrawal
    {
        int Key { get; set; }
        IAccountTypeInternal Account { get; }
        DateTime Date { get; }
        Money Deposit { get; }
        Money WithDrawal { get; }
    }
}

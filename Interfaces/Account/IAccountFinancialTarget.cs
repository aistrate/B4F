using System;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Instruments;
namespace B4F.TotalGiro.Accounts
{
    public interface IAccountFinancialTarget
    {
        int Key { get; set; }
        ILogin CreatedBy { get; set; }
        DateTime CreationDate { get; set; }
        Money DepositPerYear { get; set; }
        ICustomerAccount ParentAccount { get; set; }
        Money TargetAmount { get; set; }
        DateTime TargetEndDate { get; set; }
    }
}

using System;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.Accounts.RemisierHistory
{
    public interface IRemisierHistory
    {
        int Key { get; set; }
        IAccountTypeCustomer Account { get; set;  }
        IRemisierEmployee RemisierEmployee { get; set; }
        decimal KickBack { get; set; }
        decimal IntroductionFee { get; set; }
        decimal SubsequentDepositFee { get; set; }
        decimal IntroductionFeeReduction { get; set; }
        decimal SubsequentDepositFeeReduction { get; set; }
        IInternalEmployeeLogin Employee { get; }
        DateTime ChangeDate { get; set; }
        DateTime EndDate { get; }
        bool Equals(object obj);
        string ToString();
        string DepositFeeInfo { get; }
    }
}

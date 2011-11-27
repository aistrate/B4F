using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Accounts.Instructions;

namespace B4F.TotalGiro.Accounts.Withdrawals
{
    public enum PandhouderPermissions
    {
        Unknown = 0,
        No = 1,
        Yes = 2
    }
    
    public interface IWithdrawalRule
    {
        int Key { get; set; }
        ICustomerAccount Account { get; set; }
        Money Amount { get; set; }
        WithdrawalRuleRegularity Regularity { get; }
        string TransferDescription { get; set; }
        DateTime FirstDateWithdrawal { get; }
        DateTime EndDateWithdrawal { get; set; }
        PandhouderPermissions PandhouderPermission { get; set; }
        ICounterAccount CounterAccount { get; set; }
        bool IsActive { get; set; }
        bool IsInValid { get; }
        bool DoNotChargeCommission { get; set; }
        DateTime CreationDate { get; }
        string CreatedBy { get; }

        DateTime LastWithdrawalDate { get; }
        DateTime NextWithdrawalDate1 { get; }
        DateTime NextWithdrawalDate2 { get; }
        DateTime NextWithdrawalDate3 { get; }
        DateTime GetSpecificDate(int number);
        DateTime MaxWithdrawalDate { get; }
        ICashWithdrawalInstructionCollection WithdrawalInstructions { get; }

        DateTime GetMaxWithdrawalDate();
        bool Validate();
    }
}

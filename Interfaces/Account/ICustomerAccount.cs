using System;
using B4F.TotalGiro.Accounts.RemisierHistory;
using B4F.TotalGiro.Accounts.Withdrawals;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Fees.FeeRules;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.Accounts
{
    public enum AccountEmployerRelationship
    {
        None,
        Employee,
        EmployeePartner,
        EmployeeChild,
        EmployeeParent,
        EmployeeParentInLaw
    }

    [Flags()]
    public enum AccountContinuationStati
    {
        All = 0,
        Leaving = 1,
        Current = 2
    }

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.CustomerAccount">CustomerAccount</see> class
    /// </summary>
    public interface ICustomerAccount : IAccountTypeCustomer
	{
        IWithdrawalRuleCollection WithdrawalRules { get; set; }
        ContactsFormatter Formatter { get; }
        IRemisierEmployee RemisierEmployee { get; set; }
        IRemisierHistoryCollection RemisierDetailChanges { get; }
        IRemisierHistory CurrentRemisierDetails { get; set;  }
        IFeeRuleCollection FeeRules { get; }
        bool IsJointAccount { get; set; }
        bool ContactContractsValidated { get; }
        Money FirstPromisedDeposit { get; set; }
        IPandHouder PandHouder { get; set; }
        IVerpandSoort VerpandSoort { get; set; }
        IAccountFamily Family { get; set; }
        IInternalEmployeeLogin RelatedEmployee { get; set; }
        AccountEmployerRelationship EmployerRelationship { get; set; }
        IAccountFinancialTargetCollection FinancialTargetHistory { get; }
        IAccountFinancialTarget CurrentFinancialTarget { get; set; }
	}
}

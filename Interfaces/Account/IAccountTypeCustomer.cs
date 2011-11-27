using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Valuations;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts.Withdrawals;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.BackOffice.Orders;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeCustomer">AccountTypeCustomer</see> class
    /// </summary>
    public interface IAccountTypeCustomer : IAccountTypeInternal
	{
        bool AllowNewRebalanceInstruction { get; }
        bool HasPrimaryAH { get; }
        bool IsExecOnlyCustomer { get; set; }
        bool NeedsSendByPost(SendableDocumentCategories documentCategory);
        bool ReportToTax { get; set; }
        bool SendByEmail { get; }
        bool SendByPost { get; }
        bool SetModelPortfolio(IPortfolioModel newModelPortfolio, IInternalEmployeeLogin employee, DateTime changeDate);
        //bool SetModelPortfolio(IPortfolioModel model, bool isExecOnlyCustomer, AccountEmployerRelationship employerRelationship, IInternalEmployeeLogin employee);
        bool SetModelPortfolio(ILifecycle lifecycle, IPortfolioModel newModelPortfolio, bool isExecOnlyCustomer, AccountEmployerRelationship employerRelationship, IInternalEmployeeLogin employee, DateTime changeDate);
        bool UseKickback { get; set; }
        bool UseManagementFee { get; set; }
        DateTime CurrentRebalanceDate { get; }
        DateTime FinalManagementEndDate { get; set; }
        DateTime FirstManagementStartDate { get; set; }
        DateTime GetLatestManagementEndDate(ManagementTypes managementType);
        DateTime LastRebalanceDate { get; }
        DateTime LastValuationDate { get; set; }
        DateTime ManagementEndDate { get; set; }
        DateTime ManagementStartDate { get; }
        DateTime ValuationMutationValidityDate { get; set; }
        DateTime ValuationValidityDate { get; }
        IAccountAccountHoldersCollection AccountHolders { get; set; }
        IAccountHolder EnOfAccountHolder { get; }
        IAccountHolder PrimaryAccountHolder { get; }
        IAccountNotificationsCollection Notifications { get; }
        IAccountTypeCustomer ExitFeePayingAccount { get; set; }
        ICashWithdrawalInstruction CreateWithdrawalInstruction(DateTime executionDate, DateTime withdrawalDate, Money withdrawalAmount, ICounterAccount counterAccount, IWithdrawalRule rule, string transferDescription, bool doNotChargeCommission);
        ICashWithdrawalInstructionCollection ActiveWithdrawalInstructions { get; }
        IContactSendingOptionCollection ContactSendingOptions { get; }
        IInstructionCollection AccountInstructions { get; }
        IInstructionCollection ActiveAccountInstructions { get; }
        IInstructionCollection ActiveRebalanceInstructions { get; }
        bool IsDeparting { get; }
        bool IsUnderRebalance { get; }
        IList<IMoneyTransferOrder> ActiveMoneyTransferOrders { get; }
        IInstructionTypeRebalance CreateInstruction(InstructionTypes instructionType, OrderActionTypes orderActionType, DateTime executionDate, bool doNotChargeCommission);
        IInstructionTypeRebalance CreateInstruction(InstructionTypes instructionType, OrderActionTypes orderActionType, DateTime executionDate, bool doNotChargeCommission, IList<IJournalEntryLine> cashTransfers);
        IClientDepartureInstruction CreateDepartureInstruction(DateTime executionDate, ICounterAccount counterAccount, string transferDescription, bool doNotChargeCommission);
        ILastValuationCashMutationCollection LastValuationCashMutations { get; }
        IManagementPeriod CurrentKickBackFeePeriod { get; }
        IManagementPeriod CurrentManagementFeePeriod { get; set; }
        IManagementPeriodCollection ManagementPeriods { get; }
        IModelHistoryCollection ModelPortfolioChanges { get; }
        ILifecycle Lifecycle { get; set; }
        IPortfolioModel ModelPortfolio { get; set; }
        string ModelPortfolioName { get; }        

    }
}

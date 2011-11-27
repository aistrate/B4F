using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Valuations;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Communicator.Exact;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    [Flags]
    public enum CashTransferTypes
    {
        None = 0,
        Deposit = 1,
        Withdrawal = 2,
        TransferFee = 4,
        ForexNonBaseCurrency = 8,
        ForexBaseCurrency = 16
    }
    
    public interface IGLAccount
    {
        bool IsAllowedManual { get; set; }
        bool IsBankFixedLine { get; set; }
        bool IsBankSettlement { get; set; }
        bool IsClientOpenBalance { get; set; }
        bool IsCostOfStockExternal { get; set; }
        bool IsDefaultDeposit { get; set; }
        bool IsDefaultWithdrawal { get; set; }
        bool IsDividendTaxExternal { get; set; }
        bool IsDividendTaxInternal { get; set; }
        bool IsExternalSettlement { get; set; }
        bool IsGrossDividendExternal { get; set; }
        bool IsGrossDividendInternal { get; set; }
        bool IsFixed { get; set; }
        bool IsIncome { get; set; }
        bool IsSettledWithClient { get; set; }
        bool IsSettlementDifference { get; set; }
        bool IsToSettleWithClient { get; set; }
        bool IsVirtualFundUse { get; set; }
        bool RequiresGiroAccount { get; set; }
        CashTransferTypes CashTransferType { get; set; }
        ICurrency DefaultCurrency { get; set; }
        IGLAccount GLSettledAccount { get; set; }
        IGLBookingType GLBookingType { get; }
        IGLClass ClassOfAccount { get; set; }
        int Key { get; set; }
        string Description { get; set; }
        string ExactAccount { get; }
        string FullDescription { get; }
        string GLAccountNumber { get; }
        ValuationCashTypes ValuationCashType { get; set; }
        IValuationCashType ValuationCashTypeDetails { get; }
        IExactAccount AccountinExact { get; set; }
        ICashSubPositionUnSettledType UnSettledType { get; set; }
        bool IsSystem { get; set; }
    }
}

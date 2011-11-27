using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Valuations;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Communicator.Exact;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public class GLAccount : IGLAccount
    {
        public virtual int Key { get; set; }
        public virtual bool IsAllowedManual { get; set; }
        public virtual bool IsBankFixedLine { get; set; }
        public virtual bool IsBankSettlement { get; set; }
        public virtual bool IsClientOpenBalance { get; set; }
        public virtual bool IsCostOfStockExternal { get; set; }
        public virtual bool IsDefaultDeposit { get; set; }
        public virtual bool IsDefaultWithdrawal { get; set; }
        public virtual bool IsDividendTaxExternal { get; set; }
        public virtual bool IsDividendTaxInternal { get; set; }
        public virtual bool IsExternalSettlement { get; set; }
        public virtual bool IsGrossDividendExternal { get; set; }
        public virtual bool IsGrossDividendInternal { get; set; }
        public virtual bool IsFixed { get; set; }
        public virtual bool IsIncome { get; set; }
        public virtual bool IsSettledWithClient { get; set; }
        public virtual bool IsSettlementDifference { get; set; }
        public virtual bool IsToSettleWithClient { get; set; }
        public virtual bool IsVirtualFundUse { get; set; }
        public virtual bool RequiresGiroAccount { get; set; }
        public virtual CashTransferTypes CashTransferType { get; set; }
        public virtual ICurrency DefaultCurrency { get; set; }
        public virtual IGLAccount GLSettledAccount { get; set; }
        public virtual IGLBookingType GLBookingType { get; protected set; }
        public virtual IGLClass ClassOfAccount { get; set; }
        public virtual string Description { get; set; }
        public IExactAccount AccountinExact { get; set; }
        public virtual string ExactAccount { get { return this.AccountinExact.AccountNumber; } }
        public virtual string GLAccountNumber { get; private set; }
        public virtual ValuationCashTypes ValuationCashType { get; set; }
        public virtual bool IsSystem { get; set; }
        public virtual IValuationCashType ValuationCashTypeDetails { get; protected set; }
        public virtual ICashSubPositionUnSettledType UnSettledType { get; set; }

        public string FullDescription
        {
            get { return string.Format("{0} - {1}", GLAccountNumber, Description); }
        }

        public override string ToString()
        {
            return FullDescription;
        }
    }
}

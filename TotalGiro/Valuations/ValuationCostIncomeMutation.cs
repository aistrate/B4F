using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Accounts.Positions;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Valuations
{
    public class ValuationCostIncomeMutation
    {
        #region Constructors

        internal ValuationCostIncomeMutation(ValuationKey key, DateTime mutationDate) 
        {
            this.Key = key;
            this.MutationDate = mutationDate;
        }


        internal ValuationCostIncomeMutation(ValuationKey key, DateTime mutationDate, ValuationCostIncomeMutation prevMutation)
            : this(key, mutationDate)
        {
            if (prevMutation != null)
            {
                this.CostsCommissionToDate = prevMutation.CostsCommissionToDate;
                this.CostsTaxToDate = prevMutation.CostsTaxToDate;
                this.CostsOtherToDate = prevMutation.CostsOtherToDate;
                this.IncomeCashDividendToDate = prevMutation.IncomeCashDividendToDate;
                this.IncomeInterestToDate = prevMutation.IncomeInterestToDate;
                this.IncomeOtherToDate = prevMutation.IncomeOtherToDate;
            }
        }

        #endregion

        #region Methods

        internal void AddTx(IPositionTx posTx)
        {
            Money amount = posTx.Size.GetMoney();

            switch (posTx.ValuationCashTxMapping)
            {
                case B4F.TotalGiro.Orders.Transactions.ValuationCashTxTypeMapping.CostsCommission:
                    CostsCommission += amount;
                    CostsCommissionToDate += amount;
                    break;
                case B4F.TotalGiro.Orders.Transactions.ValuationCashTxTypeMapping.CostsTax:
                    CostsTax += amount;
                    CostsTaxToDate += amount;
                    break;
                case B4F.TotalGiro.Orders.Transactions.ValuationCashTxTypeMapping.CostFee:
                    CostsFee += amount;
                    CostsFeeToDate += amount;
                    break;
                case B4F.TotalGiro.Orders.Transactions.ValuationCashTxTypeMapping.CostsOther:
                    CostsOther += amount;
                    CostsOtherToDate += amount;
                    break;
                case B4F.TotalGiro.Orders.Transactions.ValuationCashTxTypeMapping.IncomeCashDividend:
                    IncomeCashDividend += amount;
                    IncomeCashDividendToDate += amount;
                    break;
                case B4F.TotalGiro.Orders.Transactions.ValuationCashTxTypeMapping.IncomeInterest:
                    IncomeInterest += amount;
                    IncomeInterestToDate += amount;
                    break;
                case B4F.TotalGiro.Orders.Transactions.ValuationCashTxTypeMapping.IncomeOther:
                    IncomeOther += amount;
                    IncomeOtherToDate += amount;
                    break;
            }
        }

        #endregion

        public ValuationKey Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public DateTime MutationDate
        {
            get { return mutationDate; }
            set { mutationDate = value; }
        }

        #region Costs

        public Money CostsCommission
        {
            get { return this.commission; }
            set { this.commission = value; }
        }

        public Money CostsCommissionToDate
        {
            get { return this.commissionToDate; }
            set { this.commissionToDate = value; }
        }

        public Money CostsTax
        {
            get { return this.tax; }
            set { this.tax = value; }
        }

        public Money CostsTaxToDate
        {
            get { return this.taxToDate; }
            set { this.taxToDate = value; }
        }

        public Money CostsFee
        {
            get { return this.fee; }
            set { this.fee = value; }
        }

        public Money CostsFeeToDate
        {
            get { return this.feeToDate; }
            set { this.feeToDate = value; }
        }

        public Money CostsOther
        {
            get { return this.costsOther; }
            set { this.costsOther = value; }
        }

        public Money CostsOtherToDate
        {
            get { return this.costsOtherToDate; }
            set { this.costsOtherToDate = value; }
        }

        #endregion

        #region Income

        public Money IncomeCashDividend
        {
            get { return this.cashDividend; }
            set { this.cashDividend = value; }
        }

        public Money IncomeCashDividendToDate
        {
            get { return this.cashDividendToDate; }
            set { this.cashDividendToDate = value; }
        }

        public Money IncomeInterest
        {
            get { return this.interest; }
            set { this.interest = value; }
        }

        public Money IncomeInterestToDate
        {
            get { return this.interestToDate; }
            set { this.interestToDate = value; }
        }

        public Money IncomeOther
        {
            get { return this.incomeOther; }
            set { this.incomeOther = value; }
        }

        public Money IncomeOtherToDate
        {
            get { return this.incomeOtherToDate; }
            set { this.incomeOtherToDate = value; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (Key == null || Key.Instrument == null || MutationDate == Util.NullDate)
                return base.ToString();
            else
                return MutationDate.ToShortDateString() + " " + Key.Instrument.DisplayName;
        }

        #endregion

        #region Privates

        private ValuationKey key;
        private DateTime mutationDate = Util.NullDate;
        private Money commission;
        private Money commissionToDate;
        private Money tax;
        private Money taxToDate;
        private Money fee;
        private Money feeToDate;
        private Money costsOther;
        private Money costsOtherToDate;
        private Money cashDividend;
        private Money cashDividendToDate;
        private Money interest;
        private Money interestToDate;
        private Money incomeOther;
        private Money incomeOtherToDate;

        private enum IsOpenClose
        {
            Open,
            Close,
            Both
        }

        #endregion

    }
}

using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Valuations
{
    public class LastValuationCashMutationHolder : ILastValuationCashMutationHolder
    {
        protected LastValuationCashMutationHolder() { }
        
        internal LastValuationCashMutationHolder(ValuationCashMutationKey key, IValuationCashMutation lastCashMutation)
        {
            this.CashMutKey = key;
            this.LastCashMutation = lastCashMutation;
        }

        internal LastValuationCashMutationHolder(IAccountTypeCustomer account, ITradeableInstrument instrument, ValuationCashTypes valuationCashType, IValuationCashMutation lastCashMutation)
        {
            this.CashMutKey = new ValuationCashMutationKey(account, instrument, valuationCashType);
            this.LastCashMutation = lastCashMutation;
        }
        
        public int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public ValuationCashMutationKey CashMutKey
        {
            get { return cashMutKey; }
            private set { cashMutKey = value; }
        }

        public IValuationCashMutation LastCashMutation
        {
            get { return lastCashMutation; }
            set { lastCashMutation = value; }
        }

        #region Privates

        private int key;
        private ValuationCashMutationKey cashMutKey;
        private IValuationCashMutation lastCashMutation;

        #endregion

    }
}

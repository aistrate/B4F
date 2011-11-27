using System;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Valuations.Mapping
{
    public class JournalEntryLineValuationCashMapping : IJournalEntryLineValuationCashMapping
    {
        protected JournalEntryLineValuationCashMapping() { }

        internal JournalEntryLineValuationCashMapping(IJournalEntryLine line, IValuationCashMutation mutation)
        {
            this.key = line.Key;
            this.ValuationCashMutation = mutation;
            this.IsRelevant = line.IsRelevant;
        }

        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        public IValuationCashMutation ValuationCashMutation
        {
            get { return valuationCashMutation; }
            set { valuationCashMutation = value; }
        }

        public bool IsRelevant
        {
            get { return this.isRelevant; }
            set { this.isRelevant = value; }
        }

        #region Privates

        private int key;
        private IValuationCashMutation valuationCashMutation;
        private bool isRelevant;

        #endregion
    }
}

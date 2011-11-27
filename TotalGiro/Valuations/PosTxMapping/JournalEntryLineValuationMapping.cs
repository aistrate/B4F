using System;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Valuations.Mapping
{
    public class JournalEntryLineValuationMapping : IJournalEntryLineValuationMapping
    {
        protected JournalEntryLineValuationMapping() { }

        internal JournalEntryLineValuationMapping(IJournalEntryLine line, IValuationMutation mutation)
        {
            this.key = line.Key;
            this.ValuationMutation = mutation;
            this.IsRelevant = line.IsRelevant;
        }

        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        public IValuationMutation ValuationMutation
        {
            get { return valuationMutation; }
            set { valuationMutation = value; }
        }

        public bool IsRelevant
        {
            get { return this.isRelevant; }
            set { this.isRelevant = value; }
        }

        #region Privates

        private int key;
        private IValuationMutation valuationMutation;
        private bool isRelevant;

        #endregion
    }
}

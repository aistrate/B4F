using System;
using System.Collections;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public class ExternalSettlementJournalEntriesCollection : TransientDomainCollection<ITradingJournalEntry>, IExternalSettlementJournalEntriesCollection
{
        public ExternalSettlementJournalEntriesCollection()
            : base() { }

        public ExternalSettlementJournalEntriesCollection(IExternalSettlement parent)
            : base()
        {
            Parent = parent;
        }

        public IExternalSettlement Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                IsInitialized = true;
            }
        }

        public virtual Money TotalExternalSettlementAmount
        {
            get 
            {
                if (this.SelectMany(x => x.Lines).GroupBy(x => x.Balance.Underlying).Count() == 1)
                    return this.SelectMany(x => x.Lines).Where(x => x.GLAccount.IsCostOfStockExternal && x.Balance != null).Select(x => x.Balance).Sum();
                else
                    return TotalExternalSettlementBaseAmount;
            }
        }

        public virtual Money TotalExternalSettlementBaseAmount
        {
            get { return this.SelectMany(x => x.Lines).Where(x => x.GLAccount.IsCostOfStockExternal && x.Balance != null).Select(x => x.Balance.BaseAmount).Sum(); }
        }

        public void Add(ITradingJournalEntry entry)
        {
            if (IsInitialized)
                entry.MatchedSettlement = Parent;
            base.Add(entry);
        }

        private IExternalSettlement parent;
    }
}

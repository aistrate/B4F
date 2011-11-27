using System;
using System.Collections;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public class ExternalSettlementJournalLinesCollection : TransientDomainCollection<IJournalEntryLine>, IExternalSettlementJournalLinesCollection
    {
        public ExternalSettlementJournalLinesCollection()
            : base() { }

        public ExternalSettlementJournalLinesCollection(IExternalSettlement parent)
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

        public void Add(IJournalEntryLine line)
        {
            if (IsInitialized)
                line.MatchedSettlement = Parent;
            line.IsSettledStatus = true;
            base.Add(line);
        }

        public Money Balance
        {
            get
            {
                if (this.GroupBy(x => x.Balance.Underlying).Count() == 1)
                    return this.Select(x => x.Balance).Sum();
                else
                    return BaseBalance;
            }
        }

        public Money BaseBalance
        {
            get { return this.Select(x => x.Balance.BaseAmount).Sum(); }
        }

        private IExternalSettlement parent;
    }
}

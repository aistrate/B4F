using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class GLJournalEntryLineCollection : TransientDomainCollection<IJournalEntryLine>, IGLJournalEntryLineCollection
    {
        public GLJournalEntryLineCollection()
            : base() { }

        public GLJournalEntryLineCollection(ICashSubPosition parent)
            : base()
        {
            Parent = parent;
        }

        public void AddLine(IJournalEntryLine item)
        {
            Add(item);
            item.ParentSubPosition = Parent;
            Parent.Size += item.Balance.Negate();
            if (Util.IsNullDate(Parent.ParentPosition.OpenDate))
                Parent.ParentPosition.OpenDate = item.Parent.TransactionDate;
            else if (Util.IsNotNullDate(item.ValueDate) && item.ValueDate < Parent.ParentPosition.OpenDate)
                Parent.ParentPosition.OpenDate = item.ValueDate;

        }

        public ICashSubPosition Parent { get; set; }
    }
}

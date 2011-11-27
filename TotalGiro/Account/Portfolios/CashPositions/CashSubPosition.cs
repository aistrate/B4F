using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public abstract class CashSubPosition : ICashSubPosition
    {
        internal CashSubPosition()
        {
            journalLines = new GLJournalEntryLineCollection(this);
        }

        internal CashSubPosition(ICashPosition parentPosition)
            : this()
        {
            this.ParentPosition = parentPosition;
            this.CreationDate = DateTime.Now;
        }

        public virtual int Key { get; set; }
        public virtual ICashPosition ParentPosition { get; set; }
        public virtual CashPositionSettleStatus SettledFlag { get; protected set; }
        public virtual Money Size { get; set; }
        public virtual DateTime CreationDate
        {
            get { return (creationDate.HasValue ? creationDate.Value : DateTime.MinValue); }
            set { creationDate = value; }
        }
        public virtual Money SizeInBaseCurrency
        {
            get { return Size.CurrentBaseAmount; }
        }

        public virtual bool IsSettled
        {
            get { return (SettledFlag == CashPositionSettleStatus.Settled); }
        }

        public virtual IGLJournalEntryLineCollection JournalLines
        {
            get
            {
                IGLJournalEntryLineCollection lines = (IGLJournalEntryLineCollection)journalLines.AsList();
                if (lines.Parent == null) lines.Parent = this;
                return lines;
            }
        }
        #region Privates        

        private IDomainCollection<IJournalEntryLine> journalLines;
        private DateTime? lastUpdated;
        private DateTime? creationDate;

        #endregion
    }
}

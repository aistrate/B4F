using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public class ExternalSettlement : IExternalSettlement
    {
        public ExternalSettlement() { }

        public ExternalSettlement(
            IList<IJournalEntryLine> bankSettlements, 
            IList<ITradingJournalEntry> tradeStatements, 
            DateTime settlementDate)
        {
            if (bankSettlements == null || bankSettlements.Count == 0)
                throw new ApplicationException("It is not possible to settle without bank settlements.");

            if (tradeStatements == null || tradeStatements.Count == 0)
                throw new ApplicationException("It is not possible to settle without trade statements.");
            
            if (tradeStatements.Where(x => !x.Trade.Approved).Count() > 0)
                throw new ApplicationException("It is not possible to settle unapproved order executions.");

            this.SettlementDate = settlementDate;
            foreach (IJournalEntryLine jel in bankSettlements)
                this.BankSettlements.Add(jel);
            foreach (ITradingJournalEntry trade in tradeStatements)
                this.TradeStatements.Add(trade);
        }

        public virtual int Key { get; set; }
        public virtual DateTime SettlementDate { get; set; }
        public virtual IMemorialBooking MemorialBooking { get; set; }

        public virtual IExternalSettlementJournalLinesCollection BankSettlements
        {
            get
            {
                IExternalSettlementJournalLinesCollection lines = (IExternalSettlementJournalLinesCollection)bankSettlements.AsList();
                if (lines.Parent == null) lines.Parent = this;
                return lines;
            }
        }

        public virtual IExternalSettlementJournalEntriesCollection TradeStatements
        {
            get
            {
                IExternalSettlementJournalEntriesCollection trades = (IExternalSettlementJournalEntriesCollection)tradeStatements.AsList();
                if (trades.Parent == null) trades.Parent = this;
                return trades;
            }
        }

        public virtual Money SettleDifference
        {
            get 
            {
                Money bankAmount = BankSettlements.Balance;
                Money tradeAmount = TradeStatements.TotalExternalSettlementAmount;
                if (bankAmount.EqualCurrency((ICurrency)bankAmount.Underlying))
                    return bankAmount + tradeAmount; 
                else
                    return BankSettlements.BaseBalance + TradeStatements.TotalExternalSettlementBaseAmount; 
            }
        }

        public bool Settle(IJournal journal, string nextJournalEntryNumber, IGLAccount settleDiffGLAccount)
        {
            bool success = false;
            if ((TradeStatements != null && TradeStatements.Count > 0) && (BankSettlements != null && BankSettlements.Count > 0))
            {
                // TODO -> What if not in base currency????
                Money diff = SettleDifference;
                foreach (ITradingJournalEntry journalEntry in TradeStatements)
                {
                    ((IOrderExecution)journalEntry.Trade).SettleExternal(SettlementDate);
                }

                if (diff != null && diff.IsNotZero)
                {
                    MemorialBooking = new MemorialBooking(journal, nextJournalEntryNumber);
                    MemorialBooking.TransactionDate = SettlementDate;
                    MemorialBooking.Description = "Settlement Difference";

                    IGLAccount glAcctA = BankSettlements[0].GLAccount;
                    IGLAccount glAcctB = settleDiffGLAccount;

                    IJournalEntryLine sideA = new JournalEntryLine();
                    sideA.GLAccount = glAcctA;
                    sideA.Balance = diff.Negate();
                    sideA.Description = MemorialBooking.Description;
                    sideA.IsSettledStatus = true;
                    sideA.ValueDate = SettlementDate;
                    MemorialBooking.Lines.AddJournalEntryLine(sideA);

                    IJournalEntryLine sideB = new JournalEntryLine();
                    sideB.GLAccount = glAcctB;
                    sideB.Balance = diff;
                    sideB.Description = MemorialBooking.Description;
                    sideB.IsSettledStatus = true;
                    sideB.ValueDate = SettlementDate;
                    MemorialBooking.Lines.AddJournalEntryLine(sideB);

                    MemorialBooking.BookLines();
                }
                success = true;
            }
            return success;
        }

        public virtual DateTime CreationDate
        {
            get { return  this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue; }
        }

        #region Privates

        private IDomainCollection<IJournalEntryLine> bankSettlements = new ExternalSettlementJournalLinesCollection();
        private IDomainCollection<ITradingJournalEntry> tradeStatements = new ExternalSettlementJournalEntriesCollection();
        private DateTime? creationDate;
        #endregion
    }
}

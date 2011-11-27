using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class TransactionComponent : BookingComponentParent, ITransactionComponent
    {
        public TransactionComponent() { }
        public TransactionComponent(ITransaction parentTransaction, BookingComponentTypes bookingComponentType)
            : this(parentTransaction, bookingComponentType, DateTime.Now) { }

        public TransactionComponent(ITransaction parentTransaction, BookingComponentTypes bookingComponentType,
            DateTime creationDate)
            : base(bookingComponentType, creationDate)
        {
            this.ParentTransaction = parentTransaction;
        }

        public override BookingComponentParentTypes BookingComponentParentType { get { return BookingComponentParentTypes.Transaction; } }
        public ITransaction ParentTransaction { get; set; }
        public override IJournalEntry BookingJournalEntry { get { return this.ParentTransaction.TradingJournalEntry; } }

    }
}

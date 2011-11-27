using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class CrumbleTransaction : OrderExecutionChild, ICrumbleTransaction
    {
        public CrumbleTransaction() { }

        public CrumbleTransaction(IOrder order, ICrumbleAccount acctA, IAccount acctB,
                InstrumentSize valueSize, Price price, decimal exRate, DateTime transactionDate,
                DateTime transactionDateTime, Decimal ServiceChargePercentage, Side txSide, ITradingJournalEntry tradingJournalEntry,
                IGLLookupRecords lookups, ListOfTransactionComponents[] components)
            : base(order, acctA, acctB, valueSize, price, exRate, transactionDate, transactionDateTime, ServiceChargePercentage,
                txSide, tradingJournalEntry, lookups, components)
        {

        }

        public override bool Approve(IInternalEmployeeLogin employee)
        {
            return Approve(employee, false);
        }

        public override bool Approve(IInternalEmployeeLogin employee, bool raiseStornoLimitExceptions)
        {
            return approve(employee, raiseStornoLimitExceptions);
        }

        public override ITransaction Storno(IAccountTypeInternal stornoAccount, B4F.TotalGiro.Stichting.Login.IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry)
        {
            ICrumbleTransaction newStorno = new CrumbleTransaction();
            newStorno.Order = Order;
            this.storno(stornoAccount, employee, reason, tradingJournalEntry, newStorno);
            return newStorno;
        }

        public override TransactionTypes TransactionType
        {
            get { return TransactionTypes.Crumble; }
        }

        public override INota CreateNota()
        {
            throw new ApplicationException("Cannot create a Nota for a Crumble Transaction.");
        }
    }
}

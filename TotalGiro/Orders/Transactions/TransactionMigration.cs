using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class TransactionMigration : TransactionTrading, ITransactionMigration
    {
        public TransactionMigration() : base() { }
        public TransactionMigration(IAccountTypeInternal acctA, IAccount acctB,
                InstrumentSize valueSize, Price price, decimal exRate, DateTime transactionDate,
                DateTime transactionDateTime, Decimal ServiceChargePercentage, Side txSide, 
                ITradingJournalEntry tradingJournalEntry,
                IGLLookupRecords lookups, ListOfTransactionComponents[] components)
            : base(acctA, acctB, valueSize, price, exRate, transactionDate,
                    transactionDateTime, ServiceChargePercentage, txSide,
                    tradingJournalEntry, lookups, components)
        { }

        public override ITransaction Storno(IAccountTypeInternal stornoAccount, B4F.TotalGiro.Stichting.Login.IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry)
        {
            ITransactionMigration newStorno = new TransactionMigration();
            this.storno(stornoAccount, employee, reason, tradingJournalEntry, newStorno);
            return newStorno;
        }

        public override TransactionTypes TransactionType
        {
            get { return TransactionTypes.Migration; }
        }

        public override INota CreateNota()
        {
            throw new ApplicationException("Cannot create a Nota for a Migration.");
        }
    }
}

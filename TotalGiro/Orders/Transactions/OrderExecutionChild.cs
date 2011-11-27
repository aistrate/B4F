using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Fees;

namespace B4F.TotalGiro.Orders.Transactions
{
    public abstract class OrderExecutionChild : TransactionOrder, IOrderExecutionChild
    {
        public OrderExecutionChild()
        {

        }


        public OrderExecutionChild(IOrder order, IAccountTypeInternal acctA, IAccount acctB,
                InstrumentSize valueSize, Price price, decimal exRate, DateTime transactionDate,
                DateTime transactionDateTime, Decimal ServiceChargePercentage, Side txSide, ITradingJournalEntry tradingJournalEntry,
                IGLLookupRecords lookups, ListOfTransactionComponents[] components)
            : base( order, acctA, acctB, valueSize, price, exRate, transactionDate, transactionDateTime,
                    ServiceChargePercentage, txSide,
                    tradingJournalEntry, lookups, components)
        {

        }



        public override TransactionTypes TransactionType
        {
            get
            {
                return TransactionTypes.TransactionOrderChild;
            }
        }

        public IOrderExecution ParentExecution { get; set; }

    }
}

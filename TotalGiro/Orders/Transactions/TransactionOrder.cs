using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Orders.Transactions
{
    public abstract class TransactionOrder : TransactionTrading, ITransactionOrder
    {
        public TransactionOrder() : base() { }

        public TransactionOrder(IOrder order, IAccountTypeInternal acctA, IAccount acctB,
				InstrumentSize valueSize, Price price, decimal exRate, DateTime transactionDate,
                DateTime transactionDateTime, Decimal ServiceChargePercentage, Side txSide, ITradingJournalEntry tradingJournalEntry, 
                IGLLookupRecords lookups, ListOfTransactionComponents[] components)
            : base(acctA, acctB, valueSize, price, exRate, transactionDate,
                    transactionDateTime, ServiceChargePercentage, txSide, 
                    tradingJournalEntry, lookups, components)
        {
            this.Order = order;
        }

        public virtual IOrder Order { get; set; }        

        public decimal FillRatio { get; set; }

        public override bool Approve(IInternalEmployeeLogin employee, bool raiseStornoLimitExceptions)
        {
            bool success = approve(employee, raiseStornoLimitExceptions);
            if (success && !IsStorno)
                setOrderStatus(OrderStateEvents.CheckFill);
            return success;
        }
            
        private void setOrderStatus(OrderStateEvents newEvent)
        {
            OrderStateMachine.SetNewStatus(this.Order, newEvent);
        }

        public override TransactionTypes TransactionType
        {
            get
            {
                return TransactionTypes.TransactionOrder;
            }

        }

        
    }
}

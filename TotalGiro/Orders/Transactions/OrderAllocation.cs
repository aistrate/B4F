using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class OrderAllocation : OrderExecutionChild, IOrderAllocation
    {
        public OrderAllocation() { }

        public OrderAllocation(IOrder order, IAccountTypeInternal acctA, IAccount acctB,
                InstrumentSize valueSize, Price price, decimal exRate, DateTime transactionDate,
                DateTime transactionDateTime, Decimal ServiceChargePercentage, Side txSide, IFeeFactory feeFactory, ITradingJournalEntry tradingJournalEntry,
                IGLLookupRecords lookups, ListOfTransactionComponents[] components)
            : base(order, acctA, acctB, valueSize, price, exRate, transactionDate, transactionDateTime,
                   ServiceChargePercentage, txSide,
                   tradingJournalEntry, lookups, components)
        {

        }

        public override ITransaction Storno(IAccountTypeInternal stornoAccount, B4F.TotalGiro.Stichting.Login.IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry)
        {
            IOrderAllocation newStorno = new OrderAllocation();
            newStorno.Order = Order;
            this.storno(stornoAccount, employee, reason, tradingJournalEntry, newStorno);
            if (this.IsClientSettled)
            {
                newStorno.IsClientSettled = true;
                newStorno.ClientSettlementDate = this.ClientSettlementDate;
            }
            return newStorno;
        }

        public void setCommission(IGLLookupRecords lookups, Money Commission)
        {
            if (this.Components.Any(x => x.BookingComponentType == BookingComponentTypes.Commission))
            {

            }
            else
            {
                ListOfTransactionComponents comm = new ListOfTransactionComponents() { ComponentType = BookingComponentTypes.Commission, ComponentValue = Commission };
                createComponents(lookups, new ListOfTransactionComponents[1] { comm });
            }
        }

        public override TransactionTypes TransactionType
        {
            get { return TransactionTypes.Allocation; }
        }

        public override INota CreateNota()
        {
            throw new ApplicationException(
                            "Cannot create a Nota for an Order Allocation transaction. A Nota should be created for the corresponding Order.");
        }
    }
}

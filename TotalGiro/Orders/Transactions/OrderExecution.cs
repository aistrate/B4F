using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class OrderExecution : TransactionOrder, IOrderExecution
    {
        public OrderExecution()
            : base()
        {
            allocations = new OrderExecutionChildCollection(this);
        }

        public OrderExecution(IOrder order, IAccount acctB,
                InstrumentSize valueSize, Price price, decimal exRate, DateTime transactionDate,
                DateTime transactionDateTime, Decimal ServiceChargePercentage, ITradingJournalEntry tradingJournalEntry,
                IGLLookupRecords lookups, ListOfTransactionComponents[] components)
            : base(order, order.Account, acctB, valueSize, price, exRate, transactionDate, transactionDateTime, ServiceChargePercentage,
                order.Side, tradingJournalEntry, lookups, components)
        {
            allocations = new OrderExecutionChildCollection(this);
        }

        public bool IsAllocated { get; set; }
        public DateTime AllocationDate { get { return this.allocationDate.HasValue ? allocationDate.Value : DateTime.MinValue; } }
        public bool IsSettled { get; set; }
        public DateTime ActualSettlementDate { get { return this.actualSettlementDate.HasValue ? actualSettlementDate.Value : DateTime.MinValue; } }
        public ICrumbleTransaction CreateCrumble(IGLLookupRecords lookups, ITradingJournalEntry tradingJournalEntry)
        {
            ICrumbleTransaction newTrans = null;
            InstrumentSize totalAllocations = this.TotalSizeAllocated;
            if (totalAllocations.IsNotZero)
            {
                ICrumbleAccount acctA = (ICrumbleAccount)this.AccountA.AccountOwner.StichtingDetails.CrumbleAccount;
                IAccount acctB = this.AccountA;
                InstrumentSize valueSize = totalAllocations;
                Price price = this.Price;
                decimal exRate = this.ExchangeRate;
                DateTime transDate = this.TransactionDate;
                DateTime transDateTime = this.TransactionDateTime;
                Money Cvalue = valueSize.CalculateAmount(price).Negate();
                B4F.TotalGiro.Orders.Side side = ((valueSize.IsGreaterThanZero) ? Side.Buy : Side.Sell);
                ListOfTransactionComponents[] comp = new ListOfTransactionComponents[2];
                comp[0] = new ListOfTransactionComponents() { ComponentType = BookingComponentTypes.CValue, ComponentValue = Cvalue };

                // accrued interest
                if (acctA.IsAccountTypeCustomer && valueSize.Underlying.SecCategory.Key == SecCategories.Bond)
                {
                    IBond bond = (IBond)valueSize.Underlying;
                    if (bond != null && bond.DoesPayInterest)
                    {
                        IExchange exchange = this.Exchange;
                        if (exchange == null)
                            exchange = bond.DefaultExchange ?? bond.HomeExchange;
                        AccruedInterestDetails calc = bond.AccruedInterest(valueSize, this.ContractualSettlementDate, exchange);
                        if (calc.IsRelevant)
                        {
                            Money accruedInterest = calc.AccruedInterest.Abs() * (decimal)this.TxSide * -1M;
                            comp[1] = new ListOfTransactionComponents() { ComponentType = BookingComponentTypes.AccruedInterest, ComponentValue = accruedInterest };
                        }
                    }
                }

                newTrans = new CrumbleTransaction(this.Order, acctA, acctB, valueSize, price, exRate, transDate, transDateTime, 0m, side, tradingJournalEntry, lookups, comp);
                newTrans.ParentExecution = this;
                this.Allocations.AddAllocation(newTrans);
            }
            return newTrans;
        }

        public InstrumentSize TotalSizeAllocated
        {
            get
            {
                InstrumentSize totalSizeAllocated = Allocations.TotalAllocations;
                return (this.ValueSize - totalSizeAllocated);
            }
        }

        public override TransactionTypes TransactionType
        {
            get { return TransactionTypes.Execution; }
        }

        public void SettleExternal(DateTime settlementDate)
        {
            List<IJournalEntryLine> newLines  = new List<IJournalEntryLine>();
            foreach (IJournalEntryLine line in TradingJournalEntry.Lines.Where(l => (l.GLAccount.IsExternalSettlement && l.IsSettledStatus == false)))
            {
                IJournalEntryLine bookOff = line.Clone();
                bookOff.Balance = line.Balance.Negate();
                bookOff.IsSettledStatus = true;
                bookOff.ValueDate = settlementDate;
                newLines.Add(bookOff);

                IJournalEntryLine bookOn = line.Clone();
                bookOn.Balance = line.Balance;
                bookOn.GLAccount = line.GLAccount.GLSettledAccount;
                bookOn.IsSettledStatus = true;
                bookOn.ValueDate = settlementDate;
                newLines.Add(bookOn);                
            }
            foreach (IJournalEntryLine line in newLines)
                TradingJournalEntry.Lines.AddJournalEntryLine(line);

            IsSettled = true;
            actualSettlementDate = settlementDate;
            TradingJournalEntry.BookLines();
        }

        public override ITransaction Storno(IAccountTypeInternal stornoAccount, B4F.TotalGiro.Stichting.Login.IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry)
        {
            IOrderExecution newStorno = new OrderExecution();
            newStorno.Order = Order;
            this.storno(stornoAccount, employee, reason, tradingJournalEntry, newStorno);
            return newStorno;
        }

        public override INota CreateNota()
        {
            throw new ApplicationException("Cannot create a Nota for an Order Execution.");
        }

        public void SetIsAllocated()
        {
            if ((this.TotalSizeAllocated.IsZero) && (this.Allocations.IsFullyApproved()))
            {
                IsAllocated = true;
                allocationDate = DateTime.Now;
            }
        }

        public virtual IOrderExecutionChildCollection Allocations
        {
            get
            {
                IOrderExecutionChildCollection alloc = (IOrderExecutionChildCollection)allocations.AsList();
                if (alloc.ParentExecution == null) alloc.ParentExecution = this;
                return alloc;
            }
        }
        private IDomainCollection<IOrderExecutionChild> allocations;
        private DateTime? allocationDate;
        private DateTime? actualSettlementDate;
    }
}

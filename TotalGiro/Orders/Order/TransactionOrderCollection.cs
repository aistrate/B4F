using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Orders
{
    public class TransactionOrderCollection : TransientDomainCollection<ITransactionOrder>, ITransactionOrderCollection
    {
        public TransactionOrderCollection()
            : base() { }

        public TransactionOrderCollection(IOrder parent)
            : base()
        {
            Parent = parent;
        }

        public void AddTransactionOrder(ITransactionOrder item)
        {
            item.Order = Parent;

            base.Add(item);
        }

        public Money TotalCounterValueSize
        {
            get
            {
                if (this.Count() > 0)
                {
                    return this.SelectMany(x => x.Components).Where(c => c.BookingComponentType == BookingComponentTypes.CValue).ToList().Select(x => x.ComponentValue).Sum();
                }
                else
                    return Parent.Amount.ZeroedAmount();
            }

        }

        public Money TotalServiceCharge
        {
            get
            {
                if (this.Count() > 0)
                {
                    return this.SelectMany(x => x.Components).Where(c => c.BookingComponentType == BookingComponentTypes.ServiceCharge).ToList().Select(x => x.ComponentValue).Sum();
                }
                else
                    return Parent.Amount.ZeroedAmount();
            }

        }

        public Money TotalAccruedInterest
        {
            get
            {
                if (this.Count() > 0)
                {
                    return this.SelectMany(x => x.Components).Where(c => c.BookingComponentType == BookingComponentTypes.AccruedInterest).ToList().Select(x => x.ComponentValue).Sum();
                }
                else
                    return Parent.Amount.ZeroedAmount();
            }

        }

        public Money TotalCommission
        {
            get
            {
                if (this.Count() > 0)
                {
                    return this.SelectMany(x => x.Components).Where(c => c.BookingComponentType == BookingComponentTypes.Commission).ToList().Select(x => x.ComponentValue).Sum();
                }
                else
                    return Parent.Amount.ZeroedAmount();
            }

        }

        public bool ContainsTrade(ITransactionOrder trade)
        {
            return this.Any(x => x.Key == trade.Key);
        }

        public void RemoveTrade(ITransactionOrder trade)
        {
            if (ContainsTrade(trade))
                this.Remove(trade);
            trade.Order = null;
        }


        public InstrumentSize TotalValueSize
        {
            get
            {
                InstrumentSize total = null;
                if (this.Count() > 0)
                    total = this.Where(x => x.ValueSize != null).Select(v => v.ValueSize).Sum();
                if (total == null)
                    total = new InstrumentSize(0m, this.Parent.RequestedInstrument);
                return total;
            }
        }

        public decimal TotalFillRatio()
        {
            return this.Select(x => x.FillRatio).Sum();
        }

        public decimal TotalApprovedFillRatio()
        {
            if(this.Any(x => x.Approved))
                return this.Where(x => x.Approved).Select(x => x.FillRatio).Sum();
            else
                return 0m;
        }

        public IOrder Parent { get; set; }

    }
}

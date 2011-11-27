using System;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class TransactionComponentCollection : TransientDomainCollection<ITransactionComponent>, ITransactionComponentCollection
    {
        public TransactionComponentCollection()
            : base() { }

        public TransactionComponentCollection(ITransaction parent)
            : base()
        {
            Parent = parent;
        }

        public ITransaction Parent { get; set; }

        public Money TotalValue
        {
            get 
            {
                if (this.GroupBy(x => x.ComponentValue.Underlying).Count() == 1)
                    return this.Select(c => c.ComponentValue).Sum();
                else
                    return BaseTotalValue;
            }
        }

        public Money BaseTotalValue
        {
            get { return this.Select(c => c.ComponentValue.BaseAmount).Sum(); }
        }

        public Money ReturnComponentValue(BookingComponentTypes type)
        {
            Money amount = null;
            if (this.Count > 0 && (this.Any(n => type.ContainsValue(n.BookingComponentType))))
                amount = this.Where(n => type.ContainsValue(n.BookingComponentType)).Select(c => c.ComponentValue).Sum();
            else if (Parent != null)
            {
                if (Parent.ValueSize != null)
                    amount = new Money(0m, Parent.TradedInstrument.CurrencyNominal);
                else
                    amount = new Money(0m, Parent.AccountA.BaseCurrency);
            }
            return amount;
        }

        public Money ReturnComponentValueInBaseCurrency(BookingComponentTypes type)
        {
            Money amount = null;
            if (this.Count > 0 && (this.Any(n => type.ContainsValue(n.BookingComponentType))))
                amount = this.Where(n => type.ContainsValue(n.BookingComponentType)).Select(c => c.ComponentValue.BaseAmount).Sum();
            else if (Parent != null)
                amount = new Money(0m, Parent.AccountA.BaseCurrency);
            return amount;
        }

        public Money ReturnComponentValue(BookingComponentTypes[] types)
        {
            Money amount = null;
            if (this.Count > 0 && (this.Any(n => n.BookingComponentType.IsValueWithin(types))))
                amount = this.Where(n => n.BookingComponentType.IsValueWithin(types)).Select(c => c.ComponentValue).Sum();
            else if (Parent != null)
            {
                if (Parent.ValueSize != null)
                    amount = new Money(0m, Parent.TradedInstrument.CurrencyNominal);
                else
                    amount = new Money(0m, Parent.AccountA.BaseCurrency);
            }
            return amount;
        }

        public Money ReturnComponentValueInBaseCurrency(BookingComponentTypes[] types)
        {
            Money amount = null;
            if (this.Count > 0 && (this.Any(n => n.BookingComponentType.IsValueWithin(types))))
                amount = this.Where(n => n.BookingComponentType.IsValueWithin(types)).Select(c => c.ComponentValue.BaseAmount).Sum();
            else if (Parent != null)
                amount = new Money(0m, Parent.AccountA.BaseCurrency);
            return amount;
        }

        /// <summary>
        /// Return total value of all components where the currency equals to the parameter
        /// </summary>
        public Money TotalValueComponentsInSpecifiedCurrency(ICurrency currency)
        {
            Money value = null;
            if (this.Count > 0 && (this.Any(n => n.ComponentValue.Underlying.Equals(currency))))
                value = this.Where(n => n.ComponentValue.Underlying.Equals(currency)).Select(c => c.ComponentValue).Sum();
            return value;
        }
    }
}

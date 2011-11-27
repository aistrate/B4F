using System;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class GeneralOperationsComponentCollection : TransientDomainCollection<IGeneralOperationsComponent>, IGeneralOperationsComponentCollection
    {
        public GeneralOperationsComponentCollection()
            : base() { }

        public GeneralOperationsComponentCollection(IGeneralOperationsBooking parent)
            : base()
        {
            Parent = parent;
        }

        public virtual IGeneralOperationsBooking Parent { get; set; }

        public virtual Money TotalAmount
        {
            get { return this.Select(x => x.ComponentValue).Sum(); }
        }

        public virtual Money TotalBaseAmount
        {
            get { return this.Select(x => x.ComponentValue.BaseAmount).Sum(); }
        }

        public Money ReturnComponentValue(BookingComponentTypes type)
        {
            Money amount = null;
            if (this.Count > 0 && (this.Any(n => type.ContainsValue(n.BookingComponentType))))
                amount = this.Where(n => type.ContainsValue(n.BookingComponentType)).Select(c => c.ComponentValue).Sum();
            return amount;
        }

        public Money ReturnComponentValueInBaseCurrency(BookingComponentTypes type)
        {
            Money amount = null;
            if (this.Count > 0 && (this.Any(n => type.ContainsValue(n.BookingComponentType))))
                amount = this.Where(n => type.ContainsValue(n.BookingComponentType)).Select(c => c.ComponentValue.BaseAmount).Sum();
            return amount;
        }
        
        public Money ReturnComponentValue(BookingComponentTypes[] types)
        {
            Money amount = null;
            if (this.Count > 0 && (this.Any(n => n.BookingComponentType.IsValueWithin(types))))
                amount = this.Where(n => n.BookingComponentType.IsValueWithin(types)).Select(c => c.ComponentValue).Sum();
            return amount;
        }

        public Money ReturnComponentValueInBaseCurrency(BookingComponentTypes[] types)
        {
            Money amount = null;
            if (this.Count > 0 && (this.Any(n => n.BookingComponentType.IsValueWithin(types))))
                amount = this.Where(n => n.BookingComponentType.IsValueWithin(types)).Select(c => c.ComponentValue.BaseAmount).Sum();
            return amount;
        }
    }
}

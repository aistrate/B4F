using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public class CashDividendCollection : TransientDomainCollection<ICashDividend>, ICashDividendCollection
    {
        public CashDividendCollection()
            : base() { }

        public CashDividendCollection(IDividendHistory parent)
            : base()
        {
            Parent = parent;
        }

        public void AddCashDividend(ICashDividend line)
        {

            line.DividendDetails = Parent;

            base.Add(line);
        }

        public IDividendHistory Parent { get; set; }

        public virtual InstrumentSize TotalUnits
        {
            get { return this.Select(x => x.UnitsInPossession).Sum(); }
        }

        public virtual Money TotalDividendAmount
        {
            get { return this.Select(x => x.DividendAmount).Sum(); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public class StockDividendCollection : TransientDomainCollection<ICorporateActionStockDividend>, IStockDividendCollection
    {
        public StockDividendCollection()
            : base() { }

        public StockDividendCollection(IDividendHistory parent)
            : base()
        {
            Parent = parent;
        }

        public void AddStockDividend(ICorporateActionStockDividend transaction)
        {

            transaction.CorporateActionDetails = Parent;
            base.Add(transaction);
        }

        public virtual IDividendHistory Parent { get; set; }


        public virtual InstrumentSize TotalUnits 
        { 
            get { return this.Select(x => x.PreviousSize).Sum(); }
        }

        public virtual Money TotalDividendAmount 
        {
            get { return this.Select(x => x.DividendAmount).Sum(); }
        }
    }
}
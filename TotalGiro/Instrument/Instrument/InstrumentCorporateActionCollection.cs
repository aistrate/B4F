using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// This class holds a collection of stock dividend instruments
    /// </summary>
    public class InstrumentCorporateActionCollection : TransientDomainCollection<IInstrumentCorporateAction>, IInstrumentCorporateActionCollection
    {
        public InstrumentCorporateActionCollection()
            : base() { }

        public InstrumentCorporateActionCollection(ISecurityInstrument parent)
            : base()
        {
            Parent = parent;
        }

        public ISecurityInstrument Parent { get; set; }

        public void AddCorporateAction(IInstrumentCorporateAction item)
        {
            if (Parent.SecCategory.Key == SecCategories.Bond)
                throw new ApplicationException("Bonds can not have stock dividend.");
            
            if (Count > 0)
            {
                if (this.Any(x => x.Isin.Trim().Equals(item.Isin.Trim(), StringComparison.OrdinalIgnoreCase)))
                    throw new ApplicationException("It is not possible to have two stock dividens with the same isin.");
            }
            if (item.Underlying.Key != Parent.Key)
                throw new ApplicationException("Not the same underlying.");
            base.Add(item);
        }

        public IStockDividend GetStockDividendByIsin(string isin)
        {
            return this.Find(x => x.IsStockDividend && x.Isin.Trim().Equals(isin.Trim(), StringComparison.OrdinalIgnoreCase)) as IStockDividend;
        }

        public IStockDividend GetLatestStockDividend()
        {
            return this.Where(x => x.IsStockDividend).OrderByDescending(x => x.CreationDate).FirstOrDefault() as IStockDividend;
        }
    }
}

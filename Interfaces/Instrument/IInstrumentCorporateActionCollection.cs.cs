using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    public interface IInstrumentCorporateActionCollection : IList<IInstrumentCorporateAction>
    {
        ISecurityInstrument Parent { get; set; }
        void AddCorporateAction(IInstrumentCorporateAction item);
        IStockDividend GetStockDividendByIsin(string isin);
        IStockDividend GetLatestStockDividend();
    }
}

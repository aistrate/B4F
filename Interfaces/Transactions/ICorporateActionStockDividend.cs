using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.CorporateAction;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface ICorporateActionStockDividend : ICorporateAction
    {
        IDividendHistory DividendDetails { get;  }
        Money DividendAmount { get; }
        Money TaxAmount { get; }
        bool IsGelicht { get; set; }
    }
}

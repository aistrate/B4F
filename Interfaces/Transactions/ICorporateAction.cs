using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments.History;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.CorporateAction;

namespace B4F.TotalGiro.Orders.Transactions
{

    /// <summary>
    /// The enum that lists all corporate action types
    /// </summary>
    public enum CorporateActionTypes
    {
        Conversion = 1,        
        BonusDistribution,
        StockDividend
    }


    public interface ICorporateAction : ITransaction
    {
        CorporateActionTypes CorporateActionType { get; set; }
        InstrumentSize PreviousSize { get; set; }
        ICorporateActionHistory CorporateActionDetails { get; set; }
    }
}

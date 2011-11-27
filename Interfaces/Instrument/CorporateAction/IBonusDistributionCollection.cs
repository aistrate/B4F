using System;
using B4F.TotalGiro.Orders.Transactions;
using System.Collections.Generic;
namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public interface IBonusDistributionCollection : IList<IBonusDistribution>
    {
        void AddBonusDistribution(B4F.TotalGiro.Orders.Transactions.IBonusDistribution entry);
        ICorporateActionBonusDistribution Parent { get; set; }
    }
}

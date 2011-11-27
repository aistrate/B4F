using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public class BonusDistributionCollection : TransientDomainCollection<IBonusDistribution>, IBonusDistributionCollection
    {
        public BonusDistributionCollection()
            : base() { }

        public BonusDistributionCollection(ICorporateActionBonusDistribution parent)
            : base()
        {
            Parent = parent;
        }

        public ICorporateActionBonusDistribution Parent
        {
            get { return (ICorporateActionBonusDistribution)parent; }
            set
            {
                parent = value;
                IsInitialized = true;
            }
        }

        public void AddBonusDistribution(IBonusDistribution entry)
        {
            //if (IsInitialized)
            //    entry.ParentDistribution = Parent;
            base.Add(entry);
        }

        private ICorporateActionBonusDistribution parent;

    }
}

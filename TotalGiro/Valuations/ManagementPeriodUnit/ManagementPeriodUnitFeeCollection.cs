using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees.FeeCalculations;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public class ManagementPeriodUnitFeeCollection : TransientDomainCollection<IManagementPeriodUnitFee>, IManagementPeriodUnitFeeCollection
    {
        public ManagementPeriodUnitFeeCollection()
            : base() { }

        public ManagementPeriodUnitFeeCollection(IManagementPeriodUnit parent)
            : base()
        {
            Parent = parent;
        }

        public IManagementPeriodUnit Parent { get; set; }

        public IManagementPeriodUnitFee AddFeeItem(FeeType feeType, Money amount, IFeeCalcVersion calcSource)
        {
            // Allow recalculation

            IManagementPeriodUnitFee item = null;
            item = GetItemByType(feeType.Key);

            if (item == null)
            {
                item = new ManagementPeriodUnitFee(Parent, feeType, amount, calcSource);
                Add(item);
            }
            else
                item.Edit(amount, calcSource);
            return item;
        }

        public IManagementPeriodUnitFee GetItemByType(FeeTypes feeType)
        {
            foreach (IManagementPeriodUnitFee item in this)
            {
                if (item.FeeType.Key == feeType)
                    return item;
            }
            return null;
        }

        public virtual Money TotalAmount 
        {
            get
            {
                Money total = null;
                foreach (IManagementPeriodUnitFee item in this)
                    total += item.Amount;
                return total;
            }
        }
    }
}

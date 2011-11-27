using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Valuations.AverageHoldings
{
    public class AverageHoldingFeeCollection : GenericCollection<IAverageHoldingFee>, IAverageHoldingFeeCollection
    {
        internal AverageHoldingFeeCollection(IAverageHolding parent, IList bagOfFees)
            : base(bagOfFees)
        {
            this.parent = parent;
        }

        public IAverageHoldingFee AddFeeItem(FeeType feeType, Money calculatedAmount, IManagementPeriodUnit unit, IFeeCalcVersion calcSource, decimal feePercentageUsed)
        {
            if (unit.ManagementFee != null)
                throw new ApplicationException("The fee can not be altered since it is already withdrawn");

            IAverageHoldingFee item = GetItemByType(feeType.Key);
            if (item == null)
            {
                item = new AverageHoldingFee(Parent, unit, feeType, calculatedAmount, calcSource, feePercentageUsed);
                Add(item);
            }
            else
                item.Edit(calculatedAmount, calcSource);
            return item;
        }

        public IAverageHoldingFee GetItemByType(FeeTypes feeType)
        {
            foreach (IAverageHoldingFee item in this)
            {
                if (item.FeeType.Key == feeType)
                    return item;
            }
            return null;
        }

        public IAverageHoldingFeeCollection GetItemsByType(FeeTypes feeType)
        {
            throw new ApplicationException("The method GetItemsByType is not supported");
        }

        public IAverageHolding Parent
        {
            get { return parent; }
        }

        public virtual Money TotalAmount
        {
            get
            {
                Money total = null;
                foreach (IAverageHoldingFee item in this)
                    total += item.Amount;
                return total;
            }
        }

        #region Override

        public override string ToString()
        {
            return String.Format("{0} items", Count.ToString());
        }

        #endregion

        #region Private Variables

        private IAverageHolding parent;

        #endregion    
    }
}

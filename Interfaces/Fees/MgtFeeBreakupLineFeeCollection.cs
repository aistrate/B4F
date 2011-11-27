using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Fees
{
    public class MgtFeeBreakupLineFeeCollection : GenericCollection<IAverageHoldingFee>, IAverageHoldingFeeCollection
    {
        internal MgtFeeBreakupLineFeeCollection(MgtFeeBreakupLine parent, IList bagOfFees)
            : base(bagOfFees)
        {
            this.parent = parent;
        }

        public FeeType[] GetFeeTypes()
        {
            FeeType[] feeTypes = null;
            if (Count > 0)
            {
                feeTypes = (from a in this
                                      group a by a.FeeType into g
                                      select g.Key).ToArray();
            }
            return feeTypes;
        }

        public IAverageHoldingFeeCollection GetItemsByType(FeeTypes feeType)
        {
            IList list = (from a in this
                    where a.FeeType.Key == feeType
                    select a).ToList();
            return new MgtFeeBreakupLineFeeCollection(Parent, list);
        }

        public IAverageHoldingFee GetItemByType(FeeTypes feeType)
        {
            throw new ApplicationException("The method GetItemByType is not supported");
        }

        public IAverageHoldingFee AddFeeItem(FeeType feeType, Money calculatedAmount, IManagementPeriodUnit unit, IFeeCalcVersion calcSource, decimal feePercentageUsed)
        {
            throw new ApplicationException("The method AddFeeItem is not supported");
        }

        public MgtFeeBreakupLine Parent
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

        public override void Add(IAverageHoldingFee item)
        {
            if (!Parent.MgtFeeType.Equals(item.FeeType) && Parent.MgtFeeType.Key != FeeTypes.SettleDifference)
                throw new ApplicationException("Feetypes don't match");
            Parent.Amount = Money.Add(Parent.Amount, item.Amount, true);
            base.Add(item);
        }
        
        public override string ToString()
        {
            return String.Format("{0} items", Count.ToString());
        }

        #endregion

        #region Private Variables

        private MgtFeeBreakupLine parent;

        #endregion
    }

}

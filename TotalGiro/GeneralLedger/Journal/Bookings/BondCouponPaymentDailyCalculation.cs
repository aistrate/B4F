using System;
using System.Linq;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using NHibernate.Collection.Generic;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class BondCouponPaymentDailyCalculation : IBondCouponPaymentDailyCalculation
    {
        protected BondCouponPaymentDailyCalculation() { }
        
        public BondCouponPaymentDailyCalculation(
            IBondCouponPayment parent, DateTime calcDate, DateTime settlementDate, 
            InstrumentSize positionSize, Money calculatedAccruedInterestUpToDate,
            IList<IBondCouponPaymentDailyCalculation> oldCalculations)
        {
            this.Parent = parent;
            this.Position = this.Parent.Position;
            this.CalcDate = calcDate;
            this.SettlementDate = settlementDate;
            this.PositionSize = positionSize;
            this.CalculatedAccruedInterestUpToDate = calculatedAccruedInterestUpToDate;
            this.OldCalculations = oldCalculations;
            CalculateDailyInterest();
        }
        
        public virtual int Key { get; set; }
        public virtual IBondCouponPayment Parent { get; set; }
        public virtual IFundPosition Position { get; set; }
        public virtual DateTime CalcDate
        {
            get
            {
                return this.calcDate.HasValue ? this.calcDate.Value : DateTime.MinValue;
            }
            set
            {
                this.calcDate = value;
            }
        }

        public virtual DateTime SettlementDate
        {
            get
            {
                return this.settlementDate.HasValue ? this.settlementDate.Value : DateTime.MinValue;
            }
            set
            {
                this.settlementDate = value;
            }
        }

        public virtual InstrumentSize PositionSize { get; set; }
        public virtual Money CalculatedAccruedInterestUpToDate { get; set; }
        public virtual Money DailyInterest { get; set; }

        public virtual IList<IBondCouponPaymentDailyCalculation> OldCalculations
        {
            get { return oldCalculations; }
            set { this.oldCalculations = value; }
        }

        public void CalculateDailyInterest()
        {
            Money cumulativeInterestUpToDate = null;
            if (Parent.DailyCalculations.Where(x => x.SettlementDate < this.SettlementDate).Count() > 0)
                cumulativeInterestUpToDate = Parent.DailyCalculations.Where(x => x.SettlementDate < this.SettlementDate).Select(x => x.DailyInterest).Sum();

            Money oldCalculatedAmount = null;
            if (oldCalculations != null && oldCalculations.Count > 0)
                oldCalculatedAmount = oldCalculations
                    .Where(x => x.CalcDate == this.CalcDate)
                    .Select(x => x.DailyInterest).Sum();

            DailyInterest = CalculatedAccruedInterestUpToDate - cumulativeInterestUpToDate - oldCalculatedAmount;
        }

        public void Edit(InstrumentSize positionSize, Money calculatedAccruedInterestUpToDate, IList<IBondCouponPaymentDailyCalculation> oldCalculations)
        {
            this.PositionSize = positionSize;
            this.CalculatedAccruedInterestUpToDate = calculatedAccruedInterestUpToDate;
            if (oldCalculations != null && oldCalculations.Count > 0)
            {
                foreach (IBondCouponPaymentDailyCalculation calc in oldCalculations)
                {
                    if (OldCalculations == null || OldCalculations.Any(x => x.Key == calc.Key))
                    {
                        if (OldCalculations == null)
                            OldCalculations = new List<IBondCouponPaymentDailyCalculation>();
                        OldCalculations.Add(calc);
                    }
                }
            }
            CalculateDailyInterest();
        }

        public override string ToString()
        {
            if (PositionSize != null && DailyInterest != null && Util.IsNotNullDate(SettlementDate))
                return string.Format("{0} {1} {2} {3}",
                    PositionSize.Underlying.DisplayIsin,
                    SettlementDate.ToShortDateString(),
                    PositionSize.Quantity,
                    DailyInterest.DisplayString);
            else
            return base.ToString();
        }

        #region Privates

        private DateTime? calcDate;
        private DateTime? settlementDate;
        private IList<IBondCouponPaymentDailyCalculation> oldCalculations;

        #endregion


    }
}

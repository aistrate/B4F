using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Accounts.ManagementPeriods;

namespace B4F.TotalGiro.Fees
{
    public class MgtFee
    {
        #region Constructors

        protected MgtFee() { }

        /// <summary>
        /// Constructor of MgtFee object
        /// </summary>
        /// <param name="startDate">The start date of the management fee</param>
        /// <param name="endDate">The end date of the management fee</param>
        public MgtFee(DateTime startDate, DateTime endDate) 
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
        }


        ///// <summary>
        ///// Constructor of MgtFee object
        ///// </summary>
        ///// <param name="feeType">The type of MgtFee</param>
        ///// <param name="amount">The (total) value calculated</param>
        ///// <param name="startDate">The start date of the management fee</param>
        ///// <param name="endDate">The end date of the management fee</param>
        ///// <param name="description">Description of the fee</param>
        //public MgtFee(FeeType feeType, Money amount, DateTime startDate, DateTime endDate) //, string description)
        //    : this(startDate, endDate)
        //{
        //    BreakupLines.Add(new MgtFeeBreakupLine(feeType)); //, amount)); 
        //}


        ///// <summary>
        ///// Constructor of MgtFee object
        ///// </summary>
        ///// <param name="detailsToCopy">The object we want to clone from</param>
        //public MgtFee(MgtFee detailsToCopy)
        //{
        //    if (detailsToCopy == null || detailsToCopy.BreakupLines == null || detailsToCopy.BreakupLines.Count == 0)
        //        throw new ApplicationException("It is not possible to clone the MgtFeedetails when they are null");

        //    foreach (MgtFeeBreakupLine line in detailsToCopy.BreakupLines)
        //    {
        //        BreakupLines.Add(new MgtFeeBreakupLine(line.FeeType, line.Amount));
        //    }
        //}

        #endregion Constructors

        #region Props

        /// <summary>
        /// the start date
        /// </summary>
        public virtual DateTime StartDate
        {
            get { return startDate; }
            internal set { startDate = value; }
        }

        /// <summary>
        /// The end date
        /// </summary>
        public virtual DateTime EndDate
        {
            get { return endDate; }
            internal set { endDate = value; }
        }

        /// <summary>
        /// The total value of the MgtFee calculation.
        /// </summary>
        public virtual Money Amount
        {
            get
            {
                Money amount = null;
                foreach (MgtFeeBreakupLine line in BreakupLines)
                {
                    amount += line.Amount;
                }
                return amount;
            }
        }

        /// <summary>
        /// The total tax value of the MgtFee calculation.
        /// </summary>
        public virtual Money Tax
        {
            get
            {
                Money tax = null;
                foreach (MgtFeeBreakupLine line in BreakupLines)
                {
                    tax += line.Tax;
                }
                return tax;
            }
        }

        /// <summary>
        /// The currency of MgtFee currency.
        /// </summary>
        public virtual ICurrency FeeCurrency
        {
            get
            {
                if (Amount != null)
                    return this.Amount.Underlying.ToCurrency;
                else
                    return null;
            }
        }

        /// <summary>
        /// A collection of MgtFeeBreakupLine objects belonging to this MgtFee Calculation.
        /// </summary>
        public virtual MgtFeeBreakupLineCollection BreakupLines
        {
            get
            {
                if (this.breakupLines == null)
                    this.breakupLines = new MgtFeeBreakupLineCollection(this, lines);
                return breakupLines;
            }
        }

        /// <summary>
        /// The parent that owns this MgtFee object
        /// </summary>
        //public virtual IObsoleteManagementFee Parent
        //{
        //    get { return parent; }
        //    set { parent = value; }
        //}

        #endregion

        #region Methods

        public void AddFee(IList<IAverageHoldingFee> feeItems)
        {
            foreach (IAverageHoldingFee feeItem in feeItems)
            {
                if (feeItem.FeeType.ManagementType == ManagementTypes.KickBack)
                    throw new ApplicationException("Kickback is not allowed in management fee");

                MgtFeeBreakupLine line = BreakupLines.GetItemByType(feeItem.FeeType);
                if (line == null)
                    BreakupLines.Add(new MgtFeeBreakupLine(feeItem.FeeType, feeItem));
                else
                    line.FeeItems.Add(feeItem);
            }
        }

        public void AddFee(IManagementPeriodUnitFeeCollection feeItems)
        {
            foreach (IManagementPeriodUnitFee feeItem in feeItems)
            {
                MgtFeeBreakupLine line = BreakupLines.GetItemByType(feeItem.FeeType);
                if (line == null)
                    BreakupLines.Add(new MgtFeeBreakupLine(feeItem.FeeType, feeItem.Amount));
                else
                    line.Amount += feeItem.Amount;
            }
        }

        #endregion

        #region Override

        public override string ToString()
        {
            if (Amount != null && BreakupLines != null && BreakupLines.Count > 0)
                return String.Format("MgtFee: {0} lines:{1}", Amount.ToString(), BreakupLines.Count.ToString());
            else
                return base.ToString();
        }

        #endregion

        #region MultiplyDivide

        /// <summary>
        /// This method multiplies an instance of a <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class</param>
        /// <param name="multiplier">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class</returns>
        public static MgtFee Multiply(MgtFee lhs, decimal multiplier)
        {
            MgtFee mgtFee = null;
            if (lhs != null && lhs.BreakupLines.Count > 0)
            {
                mgtFee = new MgtFee(lhs.StartDate, lhs.EndDate);
                foreach (MgtFeeBreakupLine line in lhs.BreakupLines)
                    mgtFee.BreakupLines.Add(new MgtFeeBreakupLine(line.MgtFeeType, line.Amount * multiplier)); 
            }
            //else
            //    throw new ApplicationException("It is not possible to multiply a null value");

            return mgtFee;
        }

        /// <summary>
        /// This method multiplies an instance of a <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class</param>
        /// <param name="multiplier">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class</returns>
        public static MgtFee operator *(MgtFee lhs, decimal multiplier)
        {
            return Multiply(lhs, multiplier);
        }

        /// <summary>
        /// This method divides an instance of a <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class</param>
        /// <param name="divider">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class</returns>
        public static MgtFee Divide(MgtFee lhs, decimal divider)
        {
            MgtFee mgtFee = null;
            if (lhs != null && lhs.BreakupLines.Count > 0)
            {
                mgtFee = new MgtFee(lhs.StartDate, lhs.EndDate);
                foreach (MgtFeeBreakupLine line in lhs.BreakupLines)
                    mgtFee.BreakupLines.Add(new MgtFeeBreakupLine(line.MgtFeeType, line.Amount / divider)); 
            }
            //else
            //    throw new ApplicationException("It is not possible to divide a null value");

            return mgtFee;
        }

        /// <summary>
        /// This method divides an instance of a <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class</param>
        /// <param name="divider">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Fees.MgtFee">MgtFee</see> class</returns>
        public static MgtFee operator /(MgtFee lhs, decimal divider)
        {
            return Divide(lhs, divider);
        }

        #endregion

        #region Private Variables

        private DateTime startDate;
        private DateTime endDate;
        //private IObsoleteManagementFee parent;
        private IList lines = new ArrayList();
        private MgtFeeBreakupLineCollection breakupLines;

        #endregion

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Valuations.AverageHoldings
{
    public class AverageHoldingFee : IAverageHoldingFee
    {
        /// <summary>
        /// Constructor of AverageHoldingFee object
        /// </summary>
        protected AverageHoldingFee()
        {
        }

        /// <summary>
        /// Constructor of AverageHoldingFee object
        /// </summary>
        /// <param name="parent">The holding this fee item belongs to</param>
        /// <param name="unit">The holding this fee item belongs to</param>
        /// <param name="feeType">The type of MgtFee</param>
        /// <param name="calculatedAmount">The (total) value calculated</param>
        /// <param name="calcSource">The source of the calculation</param>
        /// <param name="feePercentageUsed">The fee Percentage Used in the calculation</param>
        public AverageHoldingFee(IAverageHolding parent, IManagementPeriodUnit unit, FeeType feeType, Money calculatedAmount, IFeeCalcVersion calcSource, decimal feePercentageUsed)
            : this(parent, unit, feeType, calculatedAmount, null, calcSource, feePercentageUsed)
        {
        }

        /// <summary>
        /// Constructor of AverageHoldingFee object
        /// </summary>
        /// <param name="parent">The holding this fee item belongs to</param>
        /// <param name="unit">The holding this fee item belongs to</param>
        /// <param name="feeType">The type of MgtFee</param>
        /// <param name="calculatedAmount">The (total) value calculated</param>
        /// <param name="prevCalcAmount">The value already charged</param>
        /// <param name="calcSource">The source of the calculation</param>
        /// <param name="feePercentageUsed">The fee Percentage Used in the calculation</param>
        public AverageHoldingFee(IAverageHolding parent, IManagementPeriodUnit unit, FeeType feeType, Money calculatedAmount, Money prevCalcAmount, IFeeCalcVersion calcSource, decimal feePercentageUsed)
        {
            this.Parent = parent;
            this.Unit = unit;
            this.FeeType = feeType;
            this.Amount = calculatedAmount - prevCalcAmount;
            this.CalculatedAmount = calculatedAmount;
            this.FeePercentageUsed = feePercentageUsed;

            if (calcSource != null)
            {
                this.calcSource = calcSource;
                calcSourceKey = calcSource.Key;
            }
        }


        /// <summary>
        /// The ID
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// The average valuation this fee belongs to
        /// </summary>
        public virtual IAverageHolding Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }

        /// <summary>
        /// The unit this fee belongs to
        /// </summary>
        public virtual IManagementPeriodUnit Unit
        {
            get { return unit; }
            internal set { unit = value; }
        }

        /// <summary>
        /// The amount of the fee calculation for the average valuation without taking into regard the previous charged amount. (base currency)
        /// </summary>
        public virtual Money CalculatedAmount
        {
            get { return this.calculatedAmount; }
            set { this.calculatedAmount = value; }
        }

        /// <summary>
        /// The amount of the fee calculation for the average valuation, will be charged to the client. (base currency)
        /// </summary>
        public virtual Money Amount
        {
            get { return this.amount; }
            set { this.amount = value; }
        }

        /// <summary>
        /// The amount of any previous calculations
        /// </summary>
        public virtual Money PreviousCalculatedFeeAmount
        {
            get { return this.Parent.GetPreviousCalculatedFee(FeeType); }
        }

        /// <summary>
        /// The type of Fee.
        /// </summary>
        public virtual FeeType FeeType
        {
            get { return this.feeType; }
            internal set { this.feeType = value; }
        }

        /// <summary>
        /// The transaction that stornoed this Fee item.
        /// </summary>
        public virtual ITransaction StornoTransaction
        {
            get { return this.stornoTransaction; }
            internal set { this.stornoTransaction = value; }
        }

        /// <summary>
        /// This fee item is ignored since it is blow the threshhold
        /// </summary>
        public virtual bool IsIgnored
        {
            get { return isIgnored; }
            set { isIgnored = value; }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }

        public virtual bool IsEditted
        {
            get { return isEditted; }
            set { isEditted = value; }
        }

        public virtual int CalcSourceKey
        {
            get { return calcSourceKey; }
        }

        public virtual decimal FeePercentageUsed { get; set; }

        /// <summary>
        /// Gets or sets the fee calculation that will be used for orders matched by this fee rule.
        /// </summary>
        public virtual IFeeCalcVersion FeeCalcSource
        {
            get { return this.calcSource; }
        }

        public virtual string DisplayMessage
        {
            get 
            {
                string message = string.Empty;
                if (IsIgnored)
                    message += "This fee amount was ignored and the previous amount was used.";
                else if (StornoTransaction != null)
                    message += string.Format("This fee amount was stornoed by trade {0}.", StornoTransaction.Key);

                return message; 
            }
        }

        //public bool Deactivate()
        //{
        //    bool retVal = false;
        //    if (Parent.Transaction == null)
        //    {
        //        // allow Kickback to be overwritten
        //        if (FeeType.Key == FeeTypes.KickbackFee || FeeType.Key == FeeTypes.DiscountKickbackFee)
        //            return false;
        //        else
        //            throw new ApplicationException(string.Format("The holding {0} is not created properly, contact your system administrator.", Parent.Key.ToString()));
        //    }
        //    if (!(Parent.Transaction == null || (Parent.Transaction != null && Parent.Transaction.StornoTransaction != null)))
        //        throw new ApplicationException(string.Format("The holding {0} is already used in a previous management fee calculation.", Parent.Key.ToString()));
        //    else if (Parent.Transaction.StornoTransaction != null)
        //    {
        //        stornoTransaction = Parent.Transaction.StornoTransaction;
        //        retVal = true;
        //    }
        //    return retVal;
        //}

        public void Edit(Money calculatedAmount, IFeeCalcVersion calcSource)
        {
            this.Amount = calculatedAmount;
            this.CalculatedAmount = calculatedAmount;

            if (calcSource != null)
            {
                this.calcSource = calcSource;
                calcSourceKey = calcSource.Key;
            }
            IsEditted = true;
        }

        #region Override

        public override string ToString()
        {
            return String.Format("{0} {1}", FeeType.ToString(), Amount.ToString());
        }

        #endregion

        #region Private Variables

        private int key;
        private IAverageHolding parent;
        private IManagementPeriodUnit unit;
        private Money amount;
        private Money calculatedAmount;
        private FeeType feeType;
        private ITransaction stornoTransaction;
        private bool isIgnored;
        private int calcSourceKey;
        private IFeeCalcVersion calcSource;
        private DateTime creationDate = DateTime.Now;
        private bool isEditted;

        #endregion

    }
}

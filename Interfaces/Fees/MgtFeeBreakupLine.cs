using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.Fees
{
    public class MgtFeeBreakupLine
    {
        /// <summary>
        /// Constructor of MgtFeeBreakupLine object
        /// </summary>
        protected MgtFeeBreakupLine() { }

        ///// <summary>
        ///// Constructor of MgtFeeBreakupLine object
        ///// </summary>
        ///// <param name="feeType">The type of MgtFee</param>
        ///// <param name="amount">The (total) value calculated</param>
        //public MgtFeeBreakupLine(FeeTypes feeType, Money amount) 
        //{
        //    this.feeType = feeType;
        //    this.Amount = amount;
        //}

        /// <summary>
        /// Constructor of MgtFeeBreakupLine object
        /// </summary>
        /// <param name="mgtFeeType">The type of MgtFee</param>
        /// <param name="amount">The (total) value calculated</param>
        public MgtFeeBreakupLine(FeeType mgtFeeType, Money amount)
        {
            this.mgtFeeType = mgtFeeType;
            this.Amount = amount;
        }

        /// <summary>
        /// Constructor of MgtFeeBreakupLine object
        /// </summary>
        /// <param name="feeItem">A fee item that is generated for an averageholding</param>
        public MgtFeeBreakupLine(FeeType mgtFeeType, IAverageHoldingFee feeItem)
        {
            this.MgtFeeType = mgtFeeType;
            FeeItems.Add(feeItem);
        }

        /// <summary>
        /// The ID of the MgtFee breakup.
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            protected internal set { this.key = value; }
        }

        /// <summary>
        /// The total value of the MgtFee calculation.
        /// </summary>
        public virtual Money Amount
        {
            get { return this.amount; }
            set { this.amount = value; }
        }

        /// <summary>
        /// The Tax on the MgtFee Amount.
        /// </summary>
        public virtual Money Tax
        {
            get { return this.tax; }
            protected internal set { this.tax = value; }
        }

        /// <summary>
        /// The currency of MgtFee currency.
        /// </summary>
        public virtual ICurrency MgtFeeCurrency
        {
            get { return Amount.Underlying.ToCurrency; }
        }

        public virtual FeeType MgtFeeType
        {
            get { return this.mgtFeeType; }
            protected set { this.mgtFeeType = value; }
        }

       public virtual int FeeTypeKey
        {
            get { return MgtFeeType.key; }
        }

        public virtual string Description
        {
            get { return MgtFeeType.Description; }
        }

        public virtual MgtFee Parent
        {
            get { return parent; }
            protected internal set { this.parent = value; }
        }

        //public virtual IObsoleteManagementFee Transaction
        //{
        //    get 
        //    {
        //        if (this.transaction == null)
        //        {
        //            if (Parent != null && parent.Parent != null)
        //                this.transaction = parent.Parent;
        //        }
        //        return this.transaction; 
        //    }
        //    protected set { this.transaction = value; }
        //}

        /// <summary>
        /// The fees that belong to this item.
        /// </summary>
        public virtual IAverageHoldingFeeCollection FeeItems
        {
            get
            {
                if (feeItems == null)
                    this.feeItems = new MgtFeeBreakupLineFeeCollection(this, bagOfFees);
                return feeItems;
            }
        }

        public virtual void SetBTW(decimal percentage)
        {
            if (percentage < 0 || percentage > 1)
                throw new ApplicationException("The BTW Percentage can only be between 0 and 1");

            Tax = Money.Multiply(Amount, percentage);
        }

        #region Override

        public override string ToString()
        {
            return String.Format("{0} {1}", MgtFeeType.ToString(), Amount.ToString());
        }

        #endregion

        #region Private Variables

        private int key;
        private Money amount;
        private Money tax;
        private FeeType mgtFeeType;
        private MgtFee parent;
        //private IObsoleteManagementFee transaction;
        private IList bagOfFees = new ArrayList();
        private IAverageHoldingFeeCollection feeItems;

        #endregion

    }
}

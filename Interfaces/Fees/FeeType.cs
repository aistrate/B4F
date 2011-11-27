using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Fees
{
    ///// <summary>
    ///// This enumeration lists the possible feetypes for which fee calaculation can exist
    ///// </summary>
    [Flags()]
    public enum FeeTypes
    {
        /// <summary>
        /// The fee that is involved in managing the customers portfolio
        /// </summary>
        None = 0,
        ManagementFee = 1,
        AdministrationCosts = 2,
        FixedFee = 4,
        KickbackFee = 8,
        DiscountManagementFee = 16,
        DiscountKickbackFee = 32,
        SettleDifference = 64
    }

    /// <summary>
    /// This enumeration lists the possibilities where a calculation is based upon
    /// </summary>
    public enum FeeCalcBasis
    {
        Account,
        Instrument
    }

    public class FeeType
    {
        public virtual FeeTypes Key
        {
            get { return (FeeTypes)this.key; }
            internal set { this.key = (int)value; }
        }

        public virtual string Name
        {
            get { return name; }
            internal set { name = value; }
        }
        
        public virtual string Description
        {
            get { return description; }
            internal set { description = value; }
        }

        /// <summary>
        /// Indicates whether the fee calculation is based on either account level or instrument level.
        /// </summary>
        public virtual FeeCalcBasis CalcBasis
        {
            get { return this.calcBasis; }
            set { this.calcBasis = value; }
        }

        /// <summary>
        /// Indicates whether the fee calculation is relevant for management fee.
        /// </summary>
        public virtual bool IsRelevantForMgtFee
        {
            get { return this.isRelevantForMgtFee; }
            set { this.isRelevantForMgtFee = value; }
        }

        /// <summary>
        /// Is tax involved for this feetype.
        /// </summary>
        public virtual bool UseTax
        {
            get { return this.useTax; }
            set { this.useTax = value; }
        }

        /// <summary>
        /// Does this feetype need a calculation
        /// </summary>
        public virtual bool IsCalculation
        {
            get { return isCalculation; }
            set { isCalculation = value; }
        }

        /// <summary>
        /// Is this feetype a discount
        /// </summary>
        public virtual bool IsDiscount
        {
            get { return isDiscount; }
            set { isDiscount = value; }
        }

        /// <summary>
        /// The type that the discount should be given to
        /// </summary>
        public virtual FeeType DiscountOnFeeType
        {
            get { return discountOnFeeType; }
            set { discountOnFeeType = value; }
        }

        /// <summary>
        /// The sign that should be used for the fee type
        /// </summary>
        public virtual decimal FeeTypeSign
        {
            get
            {
                if (IsDiscount)
                    return 1M;
                else
                    return -1M;
            }
        }

        /// <summary>
        /// If there is a diff for previous periods, store it on this FeeType in the MgtFee
        /// </summary>
        public virtual FeeType SettleDifferenceOnFeeType
        {
            get { return settleDifferenceOnFeeType; }
            set { settleDifferenceOnFeeType = value; }
        }

        /// <summary>
        /// The type that should be used on the management fee
        /// </summary>
        public virtual FeeType MainFeeType
        {
            get 
            {
                if (DiscountOnFeeType != null)
                    return DiscountOnFeeType;
                else
                    return this;
            }
        }

        public virtual ManagementTypes ManagementType { get; set; }
        public virtual BookingComponentTypes BookingComponentType { get; set; }

        #region Override

        /// <summary>
        /// A string representation of the Fee type<b>Name</b>.
        /// </summary>
        public override string ToString()
        {
            return Description;
        }

        #endregion

        #region Private Variables

        internal int key;
        private string name;
        private string description;
        private FeeCalcBasis calcBasis;
        private bool isRelevantForMgtFee;
        private bool useTax;
        private bool isCalculation;
        private bool isDiscount;
        private FeeType discountOnFeeType;
        private FeeType settleDifferenceOnFeeType;

        #endregion
    }
}

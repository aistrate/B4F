using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Fees
{
    public enum CommissionBreakupTypes
    {
        Commission = 0,
        AdditionalCommission
    }
    
    public class CommissionBreakupLine
    {
        /// <summary>
        /// Constructor of CommValueDetails object
        /// </summary>
        protected CommissionBreakupLine()
        {
        }

        /// <summary>
        /// Constructor of CommissionBreakupLine object
        /// </summary>
        /// <param name="amount">The (total) value calculated</param>
        /// <param name="commissionType">The type of commission</param>
        /// <param name="commissionInfo">Info on how the commission is calculated</param>
        public CommissionBreakupLine(Money amount, CommissionBreakupTypes commissionType, string commissionInfo)
        {
            this.Amount = amount;
            this.CommissionType = commissionType;
            this.CommissionInfo = commissionInfo;
        }

        /// <summary>
        /// The ID of the commission breakup.
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            internal set { this.key = value; }
        }

        /// <summary>
        /// The total value of the commission calculation.
        /// </summary>
        public virtual Money Amount
        {
            get { return this.amount; }
            set { this.amount = value; }
        }

        /// <summary>
        /// The currency of commission currency.
        /// </summary>
        public virtual ICurrency CommCurrency
        {
            get { return Amount.Underlying.ToCurrency; }
        }

        /// <summary>
        /// The type of calculation (e.g. service charge or commission calc).
        /// </summary>
        public virtual CommissionBreakupTypes CommissionType
        {
            get { return this.commissionType; }
            internal set { this.commissionType = value; }
        }

        /// <summary>
        /// The currency of commission amount.
        /// </summary>
        public virtual string CommissionInfo
        {
            get { return this.commissionInfo; }
            internal set { this.commissionInfo = value; }
        }

        public Commission Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }

        public IOrder Order
        {
            get 
            {
                if (order == null)
                {
                    if (parent != null && parent.Parent != null && parent.Parent.Type == CommissionParentTypes.Order)
                        order = parent.Parent.Order;
                }
                return order;
            }
            protected set { order = value; }
        }

        //public ITransaction Transaction
        //{
        //    get 
        //    {
        //        if (transaction == null)
        //        {
        //            if (parent != null && parent.Parent != null && parent.Parent.Type == CommissionParentTypes.Transaction)
        //                transaction = parent.Parent.Transaction;
        //        }
        //        return transaction; 
        //    }
        //    protected set { transaction = value; }
        //}

        #region Override

        public override string ToString()
        {
            return String.Format("{0} {1}", CommissionType.ToString(), Amount.ToString());
        }

        #endregion

        #region Private Variables

        private int key;
        private Money amount;
        private CommissionBreakupTypes commissionType;
        private string commissionInfo;
        private Commission parent;
        private IOrder order;
        //private IObsoleteTransaction transaction;

        #endregion

    }
}

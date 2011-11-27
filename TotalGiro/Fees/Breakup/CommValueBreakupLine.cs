using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;


namespace B4F.TotalGiro.Fees.Calculations
{
    public class CommValueBreakupLine : ICommValueBreakupLine
    {
        /// <summary>
        /// Constructor of CommValueDetails object
        /// </summary>
        protected CommValueBreakupLine()
        {
        }

        ///// <summary>
        ///// Constructor of CommValueDetails object
        ///// </summary>
        //internal CommValueBreakupLine(CommValueDetails parentDetails) 
        //{
        //    this.Parent = parentDetails;
        //}

        /// <summary>
        /// Constructor of CommValueBreakupLine object
        /// </summary>
        /// <param name="parentDetails">The parent object for the detailed lines</param>
        /// <param name="calcValue">The (total) value calculated</param>
        /// <param name="calcType">The type of commission</param>
        /// <param name="calcInfo">Info on how the commission is calculated</param>
        public CommValueBreakupLine(CommValueDetails parentDetails, Money calcValue, CommValueBreakupType calcType, string calcInfo)
        {
            this.Parent = parentDetails;
            this.CalcValue = calcValue;
            this.CalcType = calcType;
            this.CalcInfo = calcInfo;
        }


        /// <summary>
        /// The ID of the commission calculation.
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            internal set { this.key = value; }
        }

        /// <summary>
        /// The total value of the commission calculation.
        /// </summary>
        public virtual Money CalcValue
        {
            get { return this.calcValue; }
            set { this.calcValue = value; }
        }

        /// <summary>
        /// The currency of commission currency.
        /// </summary>
        public virtual ICurrency CommCurrency
        {
            get { return CalcValue.Underlying.ToCurrency; }
        }

        /// <summary>
        /// The type of calculation (e.g. service charge or commission calc).
        /// </summary>
        public virtual CommValueBreakupType CalcType
        {
            get { return this.calcType; }
            internal set { this.calcType = value; }
        }

        /// <summary>
        /// The currency of commission amount.
        /// </summary>
        public virtual string CalcInfo
        {
            get { return this.calcInfo; }
            internal set { this.calcInfo = value; }
        }

        internal virtual CommValueDetails Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        #region Private Variables

        private Int32 key;
        private Money calcValue;
        private CommValueBreakupType calcType;
        private string calcInfo;
        private CommValueDetails parent;

        #endregion

    }
}

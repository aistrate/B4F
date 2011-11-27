using System;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Fees
{
    public class Commission
    {
        #region Constructors

        /// <summary>
        /// Constructor of Commission object
        /// </summary>
        public Commission() { }

        /// <summary>
        /// Constructor of Commission object
        /// </summary>
        /// <param name="amount">The (total) value calculated</param>
        /// <param name="commCommissionInfo">Info on how the commission is calculated</param>
        public Commission(Money amount, string commissionInfo)
            : this( CommissionBreakupTypes.Commission, amount, commissionInfo)
        {
        }

        /// <summary>
        /// Constructor of Commission object
        /// </summary>
        /// <param name="commissionType">The type of commission</param>
        /// <param name="amount">The (total) value calculated</param>
        /// <param name="commCommissionInfo">Info on how the commission is calculated</param>
        public Commission(CommissionBreakupTypes commissionType, Money amount, string commissionInfo)
        {
            BreakupLines.Add(new CommissionBreakupLine(amount, commissionType, commissionInfo));
            //AddLine(commissionType, amount, commissionInfo);
        }


        /// <summary>
        /// Constructor of Commission object
        /// </summary>
        /// <param name="detailsToCopy">The object we want to clone from</param>
        public Commission(Commission detailsToCopy)
        {
            if (detailsToCopy == null || detailsToCopy.BreakupLines == null || detailsToCopy.BreakupLines.Count == 0)
                throw new ApplicationException("It is not possible to clone the commissiondetails when they are null");

            foreach (CommissionBreakupLine line in detailsToCopy.BreakupLines)
            {
                BreakupLines.Add(new CommissionBreakupLine(line.Amount, line.CommissionType, line.CommissionInfo));
                //AddLine(line.CommissionType, line.Amount, line.CommissionInfo);
            }
        }

        /////// <summary>
        /////// Constructor of Commission object
        /////// </summary>
        /////// <param name="detailsToCopy">The object we want to clone from</param>
        /////// <param name="commCurrency">The new currency to convert to</param>
        /////// <param name="detailsToCopy">The ex rate to use</param>
        //public Commission(Commission detailsToCopy, ICurrency commCurrency, decimal exRate)
        //{
        //    Money commission;
        //    if (detailsToCopy == null || detailsToCopy.BreakupLines.Count == 0)
        //        throw new ApplicationException("It is not possible to clone the commissiondetails when they are null");

        //    foreach (CommissionBreakupLine line in detailsToCopy.BreakupLines)
        //    {
        //        if (line.CommCurrency.Equals(commCurrency))
        //            commission = line.Amount;
        //        else
        //            commission = line.Amount.Convert(exRate, commCurrency);

        //        BreakupLines.Add(new CommissionBreakupLine(commission, line.CommissionType, line.CommissionInfo));
        //    }
        //}

        #endregion Constructors

        #region Props


        /// <summary>
        /// The currency of commission currency.
        /// </summary>
        public virtual ICurrency CommCurrency
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
        /// The currency of commission amount.
        /// </summary>
        public virtual string CommissionInfo
        {
            get 
            {
                string info = string.Empty;
                foreach (CommissionBreakupLine line in BreakupLines)
                {
                    info += line.CommissionInfo;
                }
                return info; 
            }
        }

        /// <summary>
        /// The total value of the commission calculation.
        /// </summary>
        public virtual Money Amount
        {
            get { return this.amount; }
            internal set { this.amount = value; }
        }

        /// <summary>
        /// A collection of CommissionBreakupLine objects belonging to this Commission Calculation.
        /// </summary>
        public virtual CommissionBreakupLineCollection BreakupLines
        {
            get
            {
                if (this.breakupLines == null)
                    this.breakupLines = new CommissionBreakupLineCollection(this, lines);
                return breakupLines;
            }
        }

        /// <summary>
        /// The parent that owns this commission object
        /// </summary>
        public ICommissionParent Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        #endregion

        #region Override

        public override string ToString()
        {
            if (Amount != null && BreakupLines != null && BreakupLines.Count > 0)
                return String.Format("Commission: {0} lines:{1}", Amount.ToString(), BreakupLines.Count.ToString());
            else
                return base.ToString();
        }

        #endregion

        #region MultiplyDivide

        /// <summary>
        /// This method multiplies an instance of a <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class</param>
        /// <param name="multiplier">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class</returns>
        public static Commission Multiply(Commission lhs, decimal multiplier)
        {
            Commission commission = null;
            if (lhs != null && lhs.BreakupLines.Count > 0)
            {
                commission = new Commission(); 
                foreach (CommissionBreakupLine line in lhs.BreakupLines)
                    commission.BreakupLines.Add(new CommissionBreakupLine(line.Amount * multiplier, line.CommissionType, line.CommissionInfo));
            }
            //else
            //    throw new ApplicationException("It is not possible to multiply a null value");

            return commission;
        }

        /// <summary>
        /// This method multiplies an instance of a <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class</param>
        /// <param name="multiplier">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class</returns>
        public static Commission operator *(Commission lhs, decimal multiplier)
        {
            return Multiply(lhs, multiplier);
        }

        /// <summary>
        /// This method divides an instance of a <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class</param>
        /// <param name="divider">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class</returns>
        public static Commission Divide(Commission lhs, decimal divider)
        {
            Commission commission = null;
            if (lhs != null && lhs.BreakupLines.Count > 0)
            {
                commission = new Commission();
                foreach (CommissionBreakupLine line in lhs.BreakupLines)
                    commission.BreakupLines.Add(new CommissionBreakupLine(line.Amount / divider, line.CommissionType, line.CommissionInfo));
            }
            //else
            //    throw new ApplicationException("It is not possible to divide a null value");

            return commission;
        }

        /// <summary>
        /// This method divides an instance of a <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class with a specified number
        /// </summary>
        /// <param name="lhs">The instance of the <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class</param>
        /// <param name="divider">The number that is multiplied with</param>
        /// <returns>The result. A new instance of the <see cref="T:B4F.TotalGiro.Fees.Commission">Commission</see> class</returns>
        public static Commission operator /(Commission lhs, decimal divider)
        {
            return Divide(lhs, divider);
        }

        #endregion

        #region Private Variables

        private Money amount;
        private ICommissionParent parent;
        private IList lines = new ArrayList();
        private CommissionBreakupLineCollection breakupLines;

        #endregion

    }
}

using System;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Fees.Calculations
{

    /// <summary>
    /// Class for holding commissin calculation information
    /// </summary>
    public class CommValueDetails : ICommValueDetails
    {
        #region Constructors

        /// <summary>
        /// Constructor of CommValueDetails object
        /// </summary>
        internal CommValueDetails() { }

        /// <summary>
        /// Constructor of CommValueDetails object
        /// </summary>
        /// <param name="calcValue">The (total) value calculated</param>
        /// <param name="commCalcInfo">Info on how the commission is calculated</param>
        internal CommValueDetails(Money calcValue, string calcInfo)
            : this( CommValueBreakupType.Commission, calcValue, calcInfo)
        {
        }

        /// <summary>
        /// Constructor of CommValueDetails object
        /// </summary>
        /// <param name="calcType">The type of commission</param>
        /// <param name="calcValue">The (total) value calculated</param>
        /// <param name="commCalcInfo">Info on how the commission is calculated</param>
        internal CommValueDetails(CommValueBreakupType calcType, Money calcValue, string calcInfo)
        {
            AddLine(calcType, calcValue, calcInfo);
        }


        /// <summary>
        /// Constructor of CommValueDetails object
        /// </summary>
        /// <param name="detailsToCopy">The object we want to clone from</param>
        internal CommValueDetails(ICommValueDetails detailsToCopy)
        {
            if (detailsToCopy == null || detailsToCopy.CommLines.Count == 0)
                throw new ApplicationException("It is not possible to clone the commissiondetails when they are null");
            
            foreach (ICommValueBreakupLine line in detailsToCopy.CommLines)
            {
                AddLine(line.CalcType, line.CalcValue, line.CalcInfo);
            }
        }

        /// <summary>
        /// Constructor of CommValueDetails object
        /// </summary>
        /// <param name="detailsToCopy">The object we want to clone from</param>
        /// <param name="commCurrency">The new currency to convert to</param>
        /// <param name="detailsToCopy">The ex rate to use</param>
        internal CommValueDetails(ICommValueDetails detailsToCopy, ICurrency commCurrency, decimal exRate)
        {
            Money commission;
            if (detailsToCopy == null || detailsToCopy.CommLines.Count == 0)
                throw new ApplicationException("It is not possible to clone the commissiondetails when they are null");

            foreach (ICommValueBreakupLine line in detailsToCopy.CommLines)
            {
                if (line.CommCurrency.Equals(commCurrency))
                    commission = line.CalcValue;
                else
                    commission = line.CalcValue.Convert(exRate, commCurrency);

                AddLine(line.CalcType, commission, line.CalcInfo);
            }
        }

        #endregion Constructors

        #region Props

        /// <summary>
        /// The ID of the commission calculation.
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            internal set { this.key = value; }
        }


        /// <summary>
        /// The currency of commission currency.
        /// </summary>
        public virtual ICurrency CommCurrency
        {
            get { return this.CalcValue.Underlying.ToCurrency; }
        }


        /// <summary>
        /// The currency of commission amount.
        /// </summary>
        public virtual string CalcInfo
        {
            get 
            {
                string info = string.Empty;
                foreach (ICommValueBreakupLine line in CommLines)
                {
                    info += line.CalcInfo;
                }
                return info; 
            }
        }

        /// <summary>
        /// The total value of the commission calculation.
        /// </summary>
        public virtual Money CalcValue
        {
            get { return this.calcValue; }
            protected set { this.calcValue = value; }
        }

        /// <summary>
        /// A collection of CommValueBreakupLine objects belonging to this Commission Calculation.
        /// </summary>
        public virtual ICommValueBreakupLineCollection CommLines
        {
            get
            {
                if (this.calcBreakupLines == null)
                    this.calcBreakupLines = new CommValueBreakupLineCollection(this, lines);
                return calcBreakupLines;
            }
        }

        #endregion

        #region Methods

        public void AddLine(CommValueBreakupType calcType, Money calcValue)
        {
            this.AddLine(calcType, calcValue, "");
        }

        public void AddLine(CommValueBreakupType calcType, Money calcValue, string calcInfo)
        {
            CommLines.Add(new CommValueBreakupLine(this, calcValue, calcType, calcInfo));
            CalcValue += calcValue;
        }


        #endregion

        #region Override


        #endregion

        #region Private Variables

        private Int32 key;
        private ICurrency commCurrency;
        private Money calcValue;
        private IList lines = new ArrayList();
        private CommValueBreakupLineCollection calcBreakupLines;

        //private const string defaultMessage = "No Commission "

        #endregion

    }
}

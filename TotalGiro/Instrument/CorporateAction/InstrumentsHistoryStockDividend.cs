using System;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Instruments.History
{
    public class InstrumentsHistoryStockDividend : InstrumentHistory, IInstrumentsHistoryStockDividend
    {
        protected InstrumentsHistoryStockDividend() { }

        internal InstrumentsHistoryStockDividend(IInstrument instrument, DateTime changeDate,
            short basisNominator, short basisDenominator)
            : base(instrument, changeDate)
        {
            this.basisNominator = basisNominator;
            this.basisNominator = basisDenominator;
        }

        /// <summary>
        /// The number of old instruments involved in the change to the new instrument -> for the ratio
        /// </summary>
        public virtual short BasisNominator
        {
            get { return this.basisNominator; }
            set { this.basisNominator = value; }
        }

        /// <summary>
        /// The number of new instruments that evolved from the old instrument -> for the ratio
        /// </summary>
        public virtual short BasisDenominator
        {
            get { return this.basisDenominator; }
            set { this.basisDenominator = value; }
        }

        /// <summary>
        /// The type of corporate action
        /// </summary>
        public override CorporateActionTypes CorporateActionType
        {
            get { return CorporateActionTypes.StockDividend; }
        }

        #region Private Variables

        private short basisNominator;
        private short basisDenominator;

        #endregion
    
    }
}


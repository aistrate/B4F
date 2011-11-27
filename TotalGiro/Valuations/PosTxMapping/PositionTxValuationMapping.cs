using System;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;

namespace B4F.TotalGiro.Valuations.Mapping
{
    public class PositionTxValuationMapping : IPositionTxValuationMapping
    {
        protected PositionTxValuationMapping() { }

        internal PositionTxValuationMapping(IFundPositionTx positionTx, IValuationMutation mutation)
        {
            this.key = positionTx.Key;
            this.ValuationMutation = mutation;
            this.IsRelevant = positionTx.IsRelevant;
        }
        
        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        public IValuationMutation ValuationMutation
        {
            get { return valuationMutation; }
            set { valuationMutation = value; }
        }

        public bool IsRelevant
        {
            get { return this.isRelevant; }
            set { this.isRelevant = value; }
        }

        #region Privates

        private int key;
        private IValuationMutation valuationMutation;
        private bool isRelevant;

        #endregion
    }
}

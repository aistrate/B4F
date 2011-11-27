using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments.Classification;

namespace B4F.TotalGiro.Valuations
{
    public class PortfolioBreakUp
    {
        public PortfolioBreakUp(IList<IValuation> valuations, DateTime relevantDate)
        {
            this.relevantDate = relevantDate;
            foreach (IValuation val in valuations)
            {
                if (val.Date.Equals(relevantDate))
                {
                    IAssetClass asset = val.ValuationMutation.AssetClass;
                    if (asset != null)
                    {
                        if (!breakupDetails.ContainsKey(asset.Key))
                            breakupDetails.Add(new KeyValuePair<int, PortfolioBreakUpDetail>(asset.Key, new PortfolioBreakUpDetail(this, asset)));
                        breakupDetails[asset.Key].BreakUpValue += val.BaseMarketValue;
                    }
                    this.totalPortfolioValue += val.BaseMarketValue;
                }
            }
        }

        public virtual DateTime RelevantDate
        {
            get { return relevantDate; }
        }

        public virtual Money TotalPortfolioValue
        {
            get { return totalPortfolioValue; }
        }

        public ICollection<PortfolioBreakUpDetail> BreakUpDetails
        {
            get { return breakupDetails.Values; }
        }

        #region Privates

        private DateTime relevantDate;
        private Money totalPortfolioValue;
        private GenericDictionary<int, PortfolioBreakUpDetail> breakupDetails = new GenericDictionary<int, PortfolioBreakUpDetail>();

        #endregion

    }
    
    public class PortfolioBreakUpDetail
    {
        public PortfolioBreakUpDetail(PortfolioBreakUp parent, IAssetClass asset)
        {
            this.parent = parent;
            this.asset = asset;
        }

        public virtual IAssetClass AssetClass
        {
            get { return asset; }
        }

        public virtual Money BreakUpValue
        {
            get { return breakUpValue; }
            internal set { breakUpValue = value; }
        }

        public virtual decimal BreakUpPercentage
        {
            get { return Math.Round(BreakUpValue.Quantity / parent.TotalPortfolioValue.Quantity, 3); }
        }

        #region Privates

        private PortfolioBreakUp parent;
        private IAssetClass asset;
        private Money breakUpValue;

        #endregion
    }
}

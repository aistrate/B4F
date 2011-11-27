using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions.Exclusions
{
    /// <summary>
    /// This class holds collection of instruments that are excluded by the rebalance
    /// </summary>
    public class RebalanceExclusionCollection : TransientDomainCollection<IRebalanceExclusion>, IRebalanceExclusionCollection
    {
        public RebalanceExclusionCollection()
            : base() { }

        public RebalanceExclusionCollection(IRebalanceInstruction parent)
            : base()
        {
            Parent = parent;
        }

        #region Props

        /// <summary>
        /// Get/set associated Rebalance instruction
        /// </summary>
        public IRebalanceInstruction Parent { get; set; }

        public List<ITradeableInstrument> TradeableInstruments
        {
            get
            {
                List<ITradeableInstrument> instruments = null;
                if (Count > 0)
                {
                    instruments = (from a in this
                                   where a.ComponentType == ModelComponentType.Instrument
                                   select ((IRebalanceExcludedInstrument)a).Instrument).ToList();

                    var x = this.Where(c => c.ComponentType == ModelComponentType.Model)
                            .SelectMany(c => ((IRebalanceExcludedModel)c).Model.LatestVersion.ModelInstruments.Instruments)
                            .Where(u => u.IsTradeable && !u.IsCashManagementFund)
                            .Cast<ITradeableInstrument>();

                    instruments.AddRange(x);
                }
                return instruments;
            }
        }

        public List<IInstrument> Instruments
        {
            get
            {
                List<IInstrument> instruments = null;
                if (TradeableInstruments != null && TradeableInstruments.Count > 0)
                    instruments = TradeableInstruments.Cast<IInstrument>().ToList();
                return instruments;
            }
        }

        #endregion

        #region Methods

        public void AddExclusion(ITradeableInstrument instrument)
        {
            if (this.Where(u => u.ComponentType == ModelComponentType.Instrument && u.ComponentKey == instrument.Key).Count() == 0)
            {
                IRebalanceExclusion exclusion = new RebalanceExcludedInstrument(instrument);
                exclusion.Parent = Parent;
                Add(exclusion);
            }
        }

        public void AddExclusion(IPortfolioModel model)
        {
            if (this.Where(u => u.ComponentType == ModelComponentType.Model && u.ComponentKey == model.Key).Count() == 0)
            {
                IRebalanceExclusion exclusion = new RebalanceExcludedModel(model);
                exclusion.Parent = Parent;
                Add(exclusion);
            }
        }

        public bool RemoveExclusionAt(int index)
        {
            bool success = false;
            if (index > 0 && index <= Count)
            {
                IRebalanceExclusion exclusion = this[index];
                exclusion.Parent = null;
                success = Remove(exclusion);
            }
            return success;
        }

        #endregion
    }
}

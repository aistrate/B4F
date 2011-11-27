using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions.Exclusions
{
    public class RebalanceExcludedModel : RebalanceExclusion, IRebalanceExcludedModel
    {
        protected RebalanceExcludedModel() { }

        public RebalanceExcludedModel(IPortfolioModel model)
        {
            if (model == null)
                throw new ApplicationException("The model can not be null");
            this.Model = model;
        }
        
        /// <summary>
        /// Get/set the instrument
        /// </summary>
        public virtual IPortfolioModel Model { get; set; }

        public override int ComponentKey
        {
            get { return Model.Key; }
        }

        public override ModelComponentType ComponentType
        {
            get { return ModelComponentType.Model; }
        }

        public override string ComponentName
        {
            get { return Model.ModelName; }
        }
    }
}

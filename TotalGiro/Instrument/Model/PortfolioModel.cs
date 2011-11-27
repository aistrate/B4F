using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Fees.FeeRules;

namespace B4F.TotalGiro.Instruments
{
    public class PortfolioModel : ModelBase, IPortfolioModel
    {
        public PortfolioModel() { }

        public virtual IBenchMarkModel ModelBenchMark
        {
            get { return modelBenchMark; }
            set { modelBenchMark = value; }
        }

        public virtual IModelDetail Details
        {
            get { return details; }
            set { details = value; }
        }

        public virtual bool AllowExecOnlyCustomers
        {
            get { return (ExecutionOptions != ExecutionOnlyOptions.NotAllowed); }
        }

        public virtual ExecutionOnlyOptions ExecutionOptions { get; set; }
        public virtual ITradeableInstrument CashFundAlternative { get; set; }
        
        public virtual short MaxWithdrawalAmountPercentage 
        {
            get { return this.LatestVersion.MaxWithdrawalAmountPercentage; }
        }

        public override ModelType ModelType
        {
            get
            {
                return ModelType.PortfiolioModel;
            }
        }

        /// <summary>
        /// Fee rules attached to the Model
        /// </summary>
        public IFeeRuleCollection FeeRules
        {
            get
            {
                if (feeRules == null)
                    feeRules = new FeeRuleCollection(this, bagOfFeeRules);
                return feeRules;
            }
        }

        public virtual IModelPerformance ModelPerformances { get; set; }

        #region Privates
        
        private IBenchMarkModel modelBenchMark;
        private IModelDetail details;
        private bool allowExecOnlyCustomers;
        private IList bagOfFeeRules;
        private IFeeRuleCollection feeRules;

        private IList bagOfModelPerformances;
 

        #endregion
        
    }
}

//public IModelPerformanceCollection ModelPerformances
//{
//    get
//    {
//        if (modelperformances == null)
//            modelperformances = new ModelPerformanceCollection(this, bagOfModelPerformances);
//        return modelperformances;
//    }
//}
//private IModelPerformanceCollection modelperformances;
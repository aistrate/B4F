using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate;
using NHibernate.Criterion;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class represents specific allocation of model in time
    /// </summary>
    public class ModelVersion : IModelVersion
    {
        public ModelVersion() { }


        /// <summary>
        /// Get/set identifier
        /// </summary>
        public virtual Int32 Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual IModelBase ParentModel { get; set; }

        /// <summary>
        /// Get/set identifier
        /// </summary>
		public virtual Int32 VersionNumber
        {
            get { return versionNumber; }
            set { versionNumber = value; }
        }

        public virtual ILogin CreatedBy { get; set; }

        /// <summary>
        /// Get/set ModelComponents
        /// </summary>
		public virtual IModelComponentCollection ModelComponents
		{
            get
            {
                IModelComponentCollection components = (IModelComponentCollection)modelComponents.AsList();
                if (components.Parent == null) components.Parent = this;
                return components;
            }
		}

        public DateTime LatestVersionDate { get; set; }

        /// <summary>
        /// Get collection of ModelInstruments
        /// </summary>
		public virtual IModelInstrumentCollection ModelInstruments
        {
			get
			{
				if (this.modelInstruments == null)
                    this.modelInstruments = new ModelInstrumentCollection(this, this.ModelComponents);
				return modelInstruments;
			}
        }

        /// <summary>
        /// Calculate total of allocations
        /// </summary>
        /// <returns>Total amount</returns>
		public virtual decimal TotalAllocation()
		{
			decimal total = 0;

			foreach (ModelInstrument mi in ModelInstruments)
			{
				total += mi.Allocation;
			}
			return total;
		}

        /// <summary>
        /// If the model contains a cash management fund, it will be returned.
        /// </summary>
        /// <returns>The CashManagementFund</returns>
        public ICashManagementFund GetCashManagementFund()
        {
            ICashManagementFund fund = null;
            if (ModelInstruments != null)
                fund = ModelInstruments.GetCashFund();
            return fund;
        }

        public virtual bool ContainsCashManagementFund
		{
            get
            {
                return (GetCashManagementFund() != null);
            }
        }

        public ITradeableInstrument GetAlternativeCashFund()
        {
            ITradeableInstrument retVal = null;
            if (ModelInstruments.Count == 1 && ModelInstruments[0].Component.IsTradeable)
                retVal = (ITradeableInstrument)ModelInstruments[0].Component;
            else
            {
                if (retVal == null && ParentModel.ModelType == ModelType.PortfiolioModel)
                    retVal = ((IPortfolioModel)ParentModel).CashFundAlternative;
            }
            return retVal;
        }

        public ITradeableInstrument GetCashFundOrAlternative()
        {
            ITradeableInstrument retVal = GetCashManagementFund();
            if (retVal == null)
                retVal = GetAlternativeCashFund();
            return retVal;
        }

        public virtual short MaxWithdrawalAmountPercentage
        {
            get
            {
                decimal maxPerc = 0;
                if (ModelInstruments != null && ModelInstruments.Count > 0)
                {
                    foreach (ModelInstrument mi in ModelInstruments)
                    {
                        IAssetManagerInstrument ai = mi.AssetManagerInstrument;
                        if (ai != null)
                            maxPerc += (mi.Allocation * (decimal)ai.MaxWithdrawalAmountPercentage);
                    }
                }
                return Convert.ToInt16(maxPerc);
            }
        }

        #region Overrides

        /// <summary>
        /// Overridden composition of anme
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} v{1}", ParentModel.ModelName, VersionNumber);
        }

        #endregion

        #region Private Variables

        private Int32 key;
        private Int32 versionNumber;
        private IDomainCollection<IModelComponent> modelComponents = new ModelComponentCollection();
		private ModelInstrumentCollection modelInstruments;

		#endregion
		
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class holds allocation of a instrument of a model
    /// </summary>
	public class ModelInstrument : ModelComponent, IModelInstrument
    {
        #region constructor

        protected ModelInstrument() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.ModelInstrument">ModelInstrument</see> class.
        /// </summary>
        public ModelInstrument(IModelVersion parent, IInstrument component, decimal allocation)
            : base(allocation)
        {
            base.ParentVersion = parent;
            this.component = component;
        }

        #endregion

        #region Props

        /// <summary>
        /// Returns type of modelcomponent
        /// </summary>
        public override ModelComponentType ModelComponentType
        {
            get
            {
                return ModelComponentType.Instrument;
            }
        }

        /// <summary>
        /// Get/set the instrument
        /// </summary>
		public virtual IInstrument Component
        {
            get { return component; }
			set { component = (IInstrument)value; }
		}

        public virtual IAssetManagerInstrument AssetManagerInstrument
        {
            get
            {
                IAssetManagerInstrument ai = null;
                if (Component.IsTradeable && this.assetManager != null)
                    ai = this.assetManager.AssetManagerInstruments.GetItemByInstrument((ITradeableInstrument)Component);
                return ai;
            }
        }

        protected IAssetManager assetManager
        {
            get
            {
                IAssetManager company = null;
                if (base.ParentVersion != null && base.ParentVersion.ParentModel != null)
                    company = base.ParentVersion.ParentModel.AssetManager;
                return company;
            }
        }

        public override string ComponentName
        {
            get { return this.Component.Name; }
        }


        public override int ModelComponentKey
        {
            get { return this.Component.Key; }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Overridden composition of name
        /// </summary>
        /// <returns>Name with allocation</returns>
		public override string ToString()
        {
            return component.ToString() + " %" + (Allocation * 100).ToString();
        }

        /// <summary>
        /// Overridden hashcode composition
        /// </summary>
        /// <returns>Number</returns>
        public override int GetHashCode()
        {
            return this.Component.GetHashCode();
        }

        #endregion

        #region Private Variables

        private IInstrument component;
        //private IModelVersion parent;

		#endregion
    }
}

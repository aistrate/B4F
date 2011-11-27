using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Globalization;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class holds the allocation of a component of a model.
    /// A component is either an instrument or another model
    /// </summary>
    public abstract class ModelComponent : IModelComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public ModelComponent()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.ModelComponent">ModelComponent</see> class.
        /// </summary>
        public ModelComponent(decimal allocation)
        {
            this.allocation = allocation;
        }

        public IModelVersion ParentVersion { get; set; }

        /// <summary>
        /// Get/set allocation (percentages) of a model
        /// </summary>
        public virtual decimal Allocation
        {
            get { return allocation; }
            set { allocation = value; }
        }

        public abstract ModelComponentType ModelComponentType { get;}
        public abstract int ModelComponentKey { get; }
        public abstract string ComponentName { get; }


        /// <summary>
        /// Get/set identifier
        /// </summary>
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
		}


		#region Private Variables

		private Int32 key;
        private decimal allocation;

		#endregion



        #region IModelComponent Members


        public string DisplayAllocation
        {
            get { return this.Allocation.ToString("P5", CultureInfo.CreateSpecificCulture("nl-NL")); }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class holds allocation of a whole model as a component of a (another) model
    /// </summary>
    public class ModelModel : ModelComponent, IModelModel
    {
        #region Constructor

        public ModelModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.ModelModel">ModelModel</see> class.
        /// </summary>        
        public ModelModel(IModelVersion version, decimal allocation)
            : base(allocation)
        {
            this.version = version;
        }

        #endregion

        #region Props

        /// <summary>
        /// Get/set model
        /// </summary>
        public virtual IModelVersion Version 
        {
            get { return this.version; }
            set { this.version = value; }
        }

        /// <summary>
        /// Get/set model
        /// </summary>
        public virtual IModelBase Component
        {
            get { return Version.ParentModel; }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Get ModelComponentType (=Model)
        /// </summary>
        public override ModelComponentType ModelComponentType
        {
            get
            {
                return ModelComponentType.Model;
            }
        }

        /// <summary>
        /// Overridden composition of name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Version.ToString() + " %" + (Allocation * 100).ToString();
        }

        public override string ComponentName
        {
            get { return Version.ToString(); }
        }

        public override int ModelComponentKey
        {
            get { return this.Version.Key; }
        }

        #endregion

        #region Privates

        private IModelVersion version;

        #endregion
    }
}

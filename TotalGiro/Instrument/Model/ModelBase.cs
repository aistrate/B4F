using System;
using System.Collections;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Dal;
using NHibernate;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments
{

    /// <summary>
    /// Class representing modelportfolio
    /// </summary>
    public abstract class ModelBase : IModelBase
	{

		public ModelBase() { }        

        /// <summary>
        /// Get/set identifier
        /// </summary>
        public virtual Int32 Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Get/set name
        /// </summary>
		public virtual string ModelName
        {
            get { return modelName; }
            set { modelName = value; }
        }

        /// <summary>
        /// Get/set short name
        /// </summary>
        public virtual string ShortName
        {
            get 
            {
                if (string.IsNullOrEmpty(shortName))
                {
                    if (ModelName.Length > 15)
                        return ModelName.Substring(0,15); 
                    else
                        return ModelName;
                }
                else
                    return this.shortName;
            }
            set 
            { 
                if (ModelName != value)
                    this.shortName = value; 
            }
        }

        /// <summary>
        /// Get/set description
        /// </summary>
		public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        public virtual string ModelNotes { get; set; }

        /// <summary>
        /// Get/set activity flag
        /// </summary>
		public virtual bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public virtual bool IsSubModel { get; set; }
        public virtual string CreatedBy { get; set; }

        /// <summary>
        /// Get/set creation date
        /// </summary>
		public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }

        /// <summary>
        /// Get/set date last updated
        /// </summary>
		public virtual DateTime LastUpdated
        {
            get { return lastUpdated; }
        }

        /// <summary>
        /// Get/set collection of other versions of model
        /// </summary>
        public virtual IModelVersionCollection ModelVersions
        {
            get
            {
                IModelVersionCollection versions = (IModelVersionCollection)modelVersions.AsList();
                if (versions.Parent == null) versions.Parent = this;
                return versions;
            }
        }

        /// <summary>
        /// Get/set latest version of model
        /// </summary>
		public virtual IModelVersion LatestVersion
		{
			get { return latestVersion; }
			set { latestVersion = value; }
        }

        /// <summary>
        /// The assetmanager that invented and owns the model
        /// </summary>
        public IAssetManager AssetManager
        {
            get { return assetManager; }
            set { assetManager = value; }
        }        

        public virtual decimal TempBenchMark
        {
            get { return tempBenchMark; }
            set { tempBenchMark = value; }
        }

        public virtual decimal BenchMarkValue
        {
            get { return benchmarkValue; }
            set { benchmarkValue = value; }
        }

        public virtual decimal IBoxxTarget
        {
            get { return iboxxTarget; }
            set { iboxxTarget = value; }
        }

        public virtual decimal MSCIWorldTarget
        {
            get { return msciworldTarget; }
            set { msciworldTarget = value; }
        }

        public virtual decimal CompositeTarget
        {
            get { return compositeTarget; }
            set { compositeTarget = value; }
        }

        public virtual decimal ExpectedReturn { get; set; }

        public virtual decimal StandardDeviation { get; set; }

        public bool IsPublic { get; set; }

        /// <summary>
        /// Overridden composition of name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ModelName.ToString();
        }

		#region Equality


        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="lhs">First model</param>
        /// <param name="rhs">Second model</param>
        /// <returns>Flag</returns>
		public static bool operator ==(ModelBase lhs, IModelBase rhs)
		{
			if ((Object)lhs == null || (Object)rhs == null)
			{
				if ((Object)lhs == null && (Object)rhs == null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (lhs.Key == rhs.Key)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Overridden unequality operator
        /// </summary>
        /// <param name="lhs">First model</param>
        /// <param name="rhs">Second model</param>
        /// <returns>Flag</returns>
		public static bool operator !=(ModelBase lhs, IModelBase rhs)
		{
			if ((Object)lhs == null || (Object)rhs == null)
			{
				if ((Object)lhs == null && (Object)rhs == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				if (lhs.Key != rhs.Key)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Overridden equality method
        /// </summary>
        /// <param name="obj">Object to compare with</param>
        /// <returns>Flag</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is IModelBase))
			{
				return false;
			}
			return this == (IModelBase)obj;
		}

		#endregion

        /// <summary>
        /// Overridding Hashcode composition
        /// </summary>
        /// <returns>Hashcode</returns>
		public override int GetHashCode()
		{
			return this.Key.GetHashCode();
		}

		#region Private Variables

		private Int32 key;
		private string modelName;
        private string shortName;
		private string description;
		private bool isActive;
		private DateTime creationDate;
		private DateTime lastUpdated;
		private int versionID;
        private IDomainCollection<IModelVersion> modelVersions = new ModelVersionCollection();
		private IModelVersion latestVersion;
        private IAssetManager assetManager;
        private decimal tempBenchMark;
        private decimal benchmarkValue;
        private decimal iboxxTarget;
        private decimal msciworldTarget;
        private decimal compositeTarget;

        #endregion

        public abstract ModelType ModelType { get; } 


    }
}

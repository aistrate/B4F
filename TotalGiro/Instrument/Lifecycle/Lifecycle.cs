using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    public class Lifecycle : ILifecycle
    {
        protected Lifecycle() { }
        
        public Lifecycle(string name, IAssetManager assetManager)
        {
            if (string.IsNullOrEmpty(name))
                throw new ApplicationException("LifecycleName is mandatory");
            if (assetManager == null)
                throw new ApplicationException("AssetManager is mandatory");
            
            Name = name;
            AssetManager = assetManager;
            CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
            IsActive = true;
        }

        public virtual int Key { get; set; }
        public virtual string Name { get; set; }
        public virtual IAssetManager AssetManager { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual string CreatedBy { get; set; }

        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
        }

        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
            set { this.lastUpdated = value; }
        }

        public virtual ILifecycleLineCollection Lines
        {
            get
            {
                ILifecycleLineCollection lineColl = (ILifecycleLineCollection)lines.AsList();
                if (lineColl.Parent == null) lineColl.Parent = this;
                lineColl.ArrangeLines();
                return lineColl;
            }
        }

        public IPortfolioModel GetRelevantModel(DateTime birthdate)
        {
            int currentAge = Util.CalculateCurrentAge(birthdate);
            return GetRelevantModel(currentAge);
        }

        public IPortfolioModel GetRelevantModel(int currentAge)
        {
            ILifecycleLine line = Lines.Where(x => x.AgeFrom <= currentAge && x.AgeTo > currentAge).FirstOrDefault();
            // no match -> take last
            if (line == null)
                line = Lines[Lines.Count - 1];

            if (line == null)
                throw new ApplicationException(string.Format("Could not find a relevant lifecycle model for lifecycle {0}", Name));
            return line.Model;
        }

        #region Overrides

        /// <summary>
        /// Overridden composition of name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Privates

        private IDomainCollection<ILifecycleLine> lines;
        private DateTime creationDate = DateTime.Now;
        private DateTime lastUpdated;

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    public class LifecycleLine : ILifecycleLine
    {
        internal LifecycleLine()
        {
        }

        internal LifecycleLine(ILifecycle parent, int ageFrom, IPortfolioModel model)
        {
            if (ageFrom == 0)
                throw new ApplicationException("Age From is mandatory");
            if (model == null)
                throw new ApplicationException("Model is mandatory");

            Parent = parent;
            AgeFrom = ageFrom;
            Model = model;
            CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
        }

        public virtual int Key { get; set; }
        public virtual ILifecycle Parent { get; set; }
        public virtual short SerialNo { get; set; }
        public virtual int AgeFrom 
        {
            get { return this.ageFrom; }
            set 
            { 
                if (value < 12)
                    throw new ApplicationException(string.Format("At this age {0} people still go to school", value));
                
                // check if Age is not already in the collection
                if (Parent.Lines.Any(x => x.Key != Key && x.AgeFrom == value))
                    throw new ApplicationException(string.Format("There is already a line with the same age {0}", value));

                this.ageFrom = value; 
            } 
        }
        public virtual int AgeTo { get; set; }
        public IPortfolioModel Model { get; set; }
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


        #region Private Variables

        private DateTime creationDate = DateTime.Now;
        private DateTime lastUpdated;
        private int ageFrom;

        #endregion

    }
}

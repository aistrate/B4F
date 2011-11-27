using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.GeneralLedger.Journal.Maintenance
{
    public class BookYearClosure : IBookYearClosure
    {
        public BookYearClosure() 
        {
            this.clientClosures = new ClientBookYearClosureCollection(this);
        }

        public int Key { get; set; }
        public string CreatedBy { get; set; }
        public IGLBookYear BookYear { get; set; }
        public DateTime CreationDate { get { return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue; } }

        public virtual IClientBookYearClosureCollection ClientClosures
        {
            get
            {
                IClientBookYearClosureCollection temp = (IClientBookYearClosureCollection)clientClosures.AsList();
                if (temp.Parent == null) temp.Parent = this;
                return temp;
            }
        }

        #region privates

        private DateTime? creationDate;
        private IDomainCollection<IClientBookYearClosure> clientClosures;

        #endregion

    }
}

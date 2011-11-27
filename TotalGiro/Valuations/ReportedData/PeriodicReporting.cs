using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Valuations.ReportedData
{
    public class PeriodicReporting : IPeriodicReporting
    {
        public PeriodicReporting()
        {
            endTermValues = new EndTermValueCollection(this);
        }
        public PeriodicReporting(ReportingPeriodDetail reportingPeriod, DateTime endTermDate, string createdBy)
            : this()
        {
            this.ReportingPeriod = reportingPeriod;
            this.EndTermDate = endTermDate;
            this.CreatedBy = createdBy;
            this.CreationDate = DateTime.Now;
        }

        public virtual int Key { get; set; }
        public virtual ReportingPeriodDetail ReportingPeriod { get; set; }
        public virtual DateTime EndTermDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual EndTermType TermType { get{return this.ReportingPeriod.TermType;}  }
        public virtual int EndTermYear { get { return this.ReportingPeriod.EndTermYear; } }



        public virtual IEndTermValueCollection EndTermValues
        {
            get
            {
                IEndTermValueCollection values = (IEndTermValueCollection)endTermValues.AsList();
                if (values.Parent == null) values.Parent = this;
                return values;
            }
        }

        public virtual DateTime CreationDate
        {
            get
            {
                if (creationDate.HasValue)
                    return creationDate.Value;
                else
                    return DateTime.MinValue;
            }
            set
            {
                creationDate = value;
            }
        }

        public override string ToString()
        {
            return this.ReportingPeriod.ToString();
        }


        #region Privates


        private IDomainCollection<IEndTermValue> endTermValues;
        private DateTime? creationDate;

        #endregion

    }
}

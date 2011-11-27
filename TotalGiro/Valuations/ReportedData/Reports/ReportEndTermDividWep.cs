using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Valuations.ReportedData.Reports
{
    public class ReportEndTermDividWep : IReportEndTermDividWep
    {
        public ReportEndTermDividWep(ReportingPeriodDetail reportPeriod, IList<IEndTermValue> endTermValues)
        {
            this.records = new EndTermDividWepComparisonCollection(this);
            this.ReportPeriod = reportPeriod;
            this.EndtermValues = endTermValues;
            createList();
        }

        public void createList()
        {
            foreach (IEndTermValue etv in EndtermValues)
            {
                IEndTermDividWepComparison rec = new EndTermDividWepComparison(etv);
                this.Records.AddEndTermDividWepComparison(rec);
            }
        }

        public ReportingPeriodDetail ReportPeriod { get; set; }
        public IList<IEndTermValue> EndtermValues { get; set; }
        public virtual IEndTermDividWepComparisonCollection Records
        {
            get
            {
                IEndTermDividWepComparisonCollection values = (IEndTermDividWepComparisonCollection)records.AsList();
                if (values.Parent == null) values.Parent = this;
                return values;
            }
        }

        #region Privates

        private IDomainCollection<IEndTermDividWepComparison> records;

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Valuations.ReportedData
{
    public class ReportingPeriodDetail
    {
        public ReportingPeriodDetail() {}
        public ReportingPeriodDetail(EndTermType termType, int endTermYear)
        {
            this.TermType = termType;
            this.EndTermYear = endTermYear;
        }

        public EndTermType TermType { get; set; }
        public int EndTermYear { get; set; }

        public DateTime GetEndDate()
        {
            DateTime endDate = DateTime.MinValue;
            switch (TermType)
            {
                case EndTermType.FirstQtr:
                    endDate = new DateTime(EndTermYear, 3, 31);
                    break;
                case EndTermType.SecondQtr:
                    endDate = new DateTime(EndTermYear, 6, 30);
                    break;
                case EndTermType.ThirdQtr:
                    endDate = new DateTime(EndTermYear, 9, 30);
                    break;
                case EndTermType.FourthQtr:
                    endDate = new DateTime(EndTermYear, 12, 31);
                    break;
                case EndTermType.FullYear:
                    endDate = new DateTime(EndTermYear, 12, 31);
                    break;
                default:
                    endDate = new DateTime(EndTermYear, 12, 31);
                    break;
            }
            return endDate;
        }

        public ReportingPeriodDetail getNextReportingPeriodDetail()
        {
            ReportingPeriodDetail returnValue = null;
            switch (this.TermType)
            {
                case EndTermType.FirstQtr:
                    returnValue= new ReportingPeriodDetail(EndTermType.SecondQtr, this.EndTermYear);
                    break;
                case EndTermType.SecondQtr:
                    returnValue = new ReportingPeriodDetail(EndTermType.ThirdQtr, this.EndTermYear);
                    break;
                case EndTermType.ThirdQtr:
                    returnValue = new ReportingPeriodDetail(EndTermType.FourthQtr, this.EndTermYear);
                    break;
                case EndTermType.FourthQtr:
                    returnValue = new ReportingPeriodDetail(EndTermType.FirstQtr, this.EndTermYear + 1);
                    break;
                case EndTermType.FullYear:
                    returnValue = new ReportingPeriodDetail(EndTermType.FirstQtr, this.EndTermYear + 1);
                    break;
            }
            return returnValue;
        }

        public override bool Equals(object obj)
        {
            bool returnValue = false;
            if (obj is ReportingPeriodDetail)
            {
                ReportingPeriodDetail newobj = obj as ReportingPeriodDetail;
                returnValue = ((newobj.EndTermYear == this.EndTermYear) && (newobj.TermType == this.TermType));
            }
            return returnValue;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.TermType.ToString());
            sb.Append(" ");
            sb.Append(this.EndTermYear.ToString());
            return sb.ToString();
        }
    }
}

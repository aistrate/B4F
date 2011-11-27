using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Communicator.BelastingDienst;

namespace B4F.TotalGiro.Valuations.ReportedData.Reports
{
    public class EndTermDividWepComparison : IEndTermDividWepComparison
    {
        public EndTermDividWepComparison(IEndTermValue endTermValue)
        {
            this.EndTermValue = endTermValue;
            this.Account = endTermValue.Account;
            if (endTermValue.DividWepRecord != null)
            {
                this.DividWep = endTermValue.DividWepRecord;
                IncludedinDividWep = true;
            }
            else
                IncludedinDividWep = false;
        }

        public IAccountTypeInternal Account { get; set; }
        public IEndTermValue EndTermValue { get; set; }
        public IDividWepRecord DividWep { get; set; }
        public bool IncludedinDividWep { get; set; }
        public IReportEndTermDividWep Parent { get; set; }
        public string AccoutNumber { get { return this.Account.Number; } }
        public string AccoutShortName { get { return this.Account.ShortName; } }
        public decimal CashValue { get { return this.EndTermValue.CashValue.Quantity; } }
        public decimal FundValue { get { return this.EndTermValue.FundValue.Quantity; } }
        public Decimal FullValue { get { return (CashValue + FundValue); } }
        public decimal DividendValue { get { return this.EndTermValue.InternalDividend.Quantity; } }
        public decimal DividendTaxValue { get { return this.EndTermValue.InternalDividendTax.Quantity; } }
        public int WEP { get { return IncludedinDividWep ? DividWep.WepValue : 0; } }
        public decimal FullValueForDividWep { get { return IncludedinDividWep ? (FullValue) : 0; } }
        public decimal RoundingError { get { return IncludedinDividWep ? (FullValue - WEP) : 0; } }
        public decimal ValuesNotIncludedinWEP { get { return !IncludedinDividWep ? (FullValue ) : 0; } }






    }
}

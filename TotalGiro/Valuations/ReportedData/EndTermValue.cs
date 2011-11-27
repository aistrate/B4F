using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Communicator.BelastingDienst;

namespace B4F.TotalGiro.Valuations.ReportedData
{
    public class EndTermValue : IEndTermValue
    {
        public EndTermValue() { }


        public EndTermValue(IAccountTypeInternal Account, IPeriodicReporting reportingPeriod, Money ClosingValue, Money GreenFundValue, Money CultureFundValue)
            : this(Account, reportingPeriod)
        {
            this.ClosingValue = ClosingValue;
            this.GreenFundValue = GreenFundValue;
            this.CultureFundValue = CultureFundValue;
        }

        public EndTermValue(IAccountTypeInternal Account, IPeriodicReporting reportingPeriod)
        {
            this.Account = Account;
            this.ReportingPeriod = reportingPeriod;
            //this.EndTermDate = EndTermDate;
            //this.TermType = termType;
        }



        public int Key { get; set; }
        public DateTime EndTermDate { get { return this.ReportingPeriod.EndTermDate; } }
        public EndTermType TermType { get { return this.ReportingPeriod.TermType; } }
        public IPeriodicReporting ReportingPeriod { get; set; }
        public IAccountTypeInternal Account { get; set; }
        public Money CashValue { get; set; }
        public Money ClosingValue { get; set; }
        public Money CultureFundValue { get; set; }
        public Money ExternalDividend { get; set; }
        public Money ExternalDividendTax { get; set; }
        public Money FundValue { get; set; }
        public Money GreenFundValue { get; set; }
        public Money InternalDividend { get; set; }
        public Money InternalDividendTax { get; set; }
        public IDividWepRecord DividWepRecord { get; set; }







    }
}

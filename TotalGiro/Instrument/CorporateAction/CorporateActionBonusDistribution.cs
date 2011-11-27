using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;


namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public class CorporateActionBonusDistribution : CorporateActionHistory,   ICorporateActionBonusDistribution
    {
        protected CorporateActionBonusDistribution() :base() { }

        //internal CorporateActionBonusDistribution(IInstrument instrument, DateTime changeDate,
        //    InstrumentSize totalSizeDistributed)
        //    : base(instrument, changeDate)
        //{
        //    this.TotalSizeDistributed = totalSizeDistributed;
        //}

        public B4F.TotalGiro.Orders.Transactions.CorporateActionTypes CorporateActionType
        {
            get { return B4F.TotalGiro.Orders.Transactions.CorporateActionTypes.BonusDistribution; }
        }
        public InstrumentSize TotalSizeDistributed { get; set; }
        public IAccountTypeInternal CounterAccount { get; set; }
        public InstrumentSize TotalHoldingsAtDate { get; set; }
        public InstrumentSize SizeToDistribute { get; set; }
        public DateTime DistributionDate { get; set; }

        public override string DisplayString
        {
            get { return Description; }
        }

        public override string ToString()
        {
            return Description;
        }
        

        //public virtual IBonusDistributionCollection BonusDistributions
        //{
        //    get
        //    {
        //        return (IBonusDistributionCollection)CorporateActions;
        //    }
        //}
    }
}

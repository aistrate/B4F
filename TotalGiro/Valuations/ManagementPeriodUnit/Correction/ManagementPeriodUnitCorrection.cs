using System;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.ManagementPeriodUnits.Corrections
{
    public class ManagementPeriodUnitCorrection : IManagementPeriodUnitCorrection
    {
        public virtual int Key { get; set; }
        public virtual IManagementPeriodUnit Unit { get; protected set; }
        public virtual IAverageHolding AverageHolding { get; protected set; }
        //public virtual IObsoleteManagementFee Transaction { get; protected set; }
        public virtual bool Skip { get; set; }
        public virtual bool IsOpen 
        {
            get
            {
                return true;
                //return (Transaction == null && !Skip);
            }
        }

    }
}

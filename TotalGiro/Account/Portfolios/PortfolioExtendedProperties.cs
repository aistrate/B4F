using System;

namespace B4F.TotalGiro.Accounts.Portfolios
{
    public class PortfolioExtendedProperties
    {
        public virtual int Key { get; set; }
        public virtual DateTime LastTransactionDate { get; set; }
        //public virtual DateTime LastCashTransactionDate { get; set; }
    }
}

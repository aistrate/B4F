using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments.Nav;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Instruments
{
    public class VirtualFund : SecurityInstrument, IVirtualFund
    {
        protected VirtualFund()
        {
            initialize();
        }

        public VirtualFund(IVirtualFundHoldingsAccount holdingsAccount, IVirtualFundTradingAccount tradingAccount, IJournal journal)
            : this()
        {
            this.HoldingsAccount = holdingsAccount;
            this.TradingAccount = tradingAccount;
            this.JournalForFund = journal;
        }

        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.VirtualFund;
        }

        public override bool Transform(DateTime changeDate, decimal oldChildRatio, byte newParentRatio, bool isSpinOff,
                string instrumentName, string isin, DateTime issueDate)
        {
            VirtualFund newFund = new VirtualFund();
            return transform(newFund, changeDate, oldChildRatio, newParentRatio, isSpinOff, instrumentName, isin, issueDate);
        }

        public override bool Validate()
        {
            if (this.TradingAccount == null)
                throw new ApplicationException("The Trading account is mandatory.");
            if (this.HoldingsAccount == null)
                throw new ApplicationException("The Holding account is mandatory.");
            if (this.JournalForFund == null)
                throw new ApplicationException("The Journal is mandatory.");
            return base.validate();
        }

        public IVirtualFundHoldingsAccount HoldingsAccount { get; set; }
        public IVirtualFundTradingAccount TradingAccount { get; set; }

        public Money SettledCashPositionInBaseValue
        {
            get
            {
                return TradingAccount.Portfolio.PortfolioCashGL.SettledCashTotalInBaseValue;
            }
        }

        public Money UnSettledCashPositionInBaseValue
        {
            get
            {
                return TradingAccount.Portfolio.PortfolioCashGL.UnSettledCashTotalInBaseValue;
            }
        }

        public decimal TotalParticpations
        {
            get
            {
                if (this.LastNavCalculation != null)
                    return this.LastNavCalculation.TotalParticipationsAfterFill;
                else
                    return 0m;
            }
        }

        public Money LastNavPerUnit
        {
            get
            {
                if (this.LastNavCalculation != null)
                    return this.LastNavCalculation.NavPerUnit;
                else
                    return InitialNavPerUnit;
            }
        }


        public DateTime LastNavDate
        {
            get
            {
                if (this.LastNavCalculation != null)
                    return this.LastNavCalculation.ValuationDate;
                else
                    return new DateTime(1,1,1);
            }
        }

        public void AssignLastNavCalc()
        {
            if ((Calculations != null) && (Calculations.Count > 0))
            {
                LastNavCalculation = Calculations.OrderByDescending(c => c.ValuationDate).ElementAt(0);
            }
        }


        public IJournal JournalForFund { get; set; }
        public INavCalculation LastNavCalculation { get; set; }
        public Money InitialNavPerUnit { get; set; }
        public virtual INavCalculationCollection Calculations
        {
            get
            {
                INavCalculationCollection calc = (INavCalculationCollection)calculations.AsList();
                return calc;
            }

        }

        private IDomainCollection<INavCalculation> calculations;
    }
}

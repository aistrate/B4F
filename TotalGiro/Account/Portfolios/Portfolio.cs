using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts.Portfolios
{
    public class Portfolio : IPortfolio
    {
        public Portfolio()
        {
            glpositions = new CashPortfolio(this.ParentAccount);
            instrumentPositions = new FundPortfolio(this.ParentAccount);
        }

        public Portfolio(IAccountTypeInternal account) : this()
        {
            this.ParentAccount = account;
        }

        public IAccountTypeInternal ParentAccount{ get; set; }

        public Money TotalValue()
        {
            return PortfolioInstrument.TotalValueInBaseCurrency + PortfolioCashGL.SettledCashTotalInBaseValue;
        }

        /// <summary>
        /// The maximum amount that can be withdrawn from the portfolio by a withdrawal instructions
        /// </summary>
        /// <returns>An amount in base currency</returns>
        public Money MaxWithdrawalAmount()
        {
            Money maxWthDrwAmt = PortfolioCashGL.SettledCashTotalInBaseValue;
            maxWthDrwAmt += PortfolioInstrument.Select(x => x.MaxWithdrawalAmount).Sum();
            return maxWthDrwAmt;
        }

        public bool HasPositions
        {
            get
            {
                return (PortfolioInstrument.Count + PortfolioCashGL.Count) > 0;
            }
        }

        public virtual ICashPortfolio PortfolioCashGL
        {
            get
            {
                ICashPortfolio positions = (ICashPortfolio)glpositions.AsList();
                if (positions.ParentAccount == null) positions.ParentAccount = this.ParentAccount;
                return positions;
            }
        }

        public virtual IFundPortfolio PortfolioInstrument
        {
            get
            {
                IFundPortfolio pos = (IFundPortfolio)instrumentPositions.AsList();
                if (pos.ParentAccount == null) pos.ParentAccount = this.ParentAccount;
                return pos;
            }
        }

        internal PortfolioExtendedProperties ExtendedProps { get; set; }

        public virtual DateTime LastTransactionDate
        {
            get { return ExtendedProps != null ? ExtendedProps.LastTransactionDate : DateTime.MinValue; }
        }

        //public virtual DateTime LastCashTransactionDate
        //{
        //    get { return ExtendedProps != null ? ExtendedProps.LastCashTransactionDate : DateTime.MinValue; }
        //}
        
        public virtual List<ICommonPosition> AllPositions
        {
            get
            {
                return (from a in PortfolioInstrument.Cast<ICommonPosition>()
                        .Union(PortfolioCashGL.Cast<ICommonPosition>())
                         select a).ToList();
            }
        }

        public virtual List<ICommonPosition> AllActivePositions
        {
            get
            {
                return (from a in PortfolioInstrument.Cast<ICommonPosition>()
                        .Union(PortfolioCashGL.Cast<ICommonPosition>())
                        where a.Size.IsNotZero
                        select a).ToList();
            }
        }

        #region Privates

        private IDomainCollection<ICashPosition> glpositions;
        private IDomainCollection<IFundPosition> instrumentPositions;

        #endregion
    }
}

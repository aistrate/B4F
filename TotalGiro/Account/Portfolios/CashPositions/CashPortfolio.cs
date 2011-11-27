using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Collection.Generic;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{

        public class CashPortfolio : TransientDomainCollection<ICashPosition>, ICashPortfolio
    {
        public CashPortfolio()
            : base() { }

        public CashPortfolio(IAccountTypeInternal parentAccount)
            : base()
        {
            ParentAccount = parentAccount;
        }

        public IAccountTypeInternal ParentAccount { get; set; }

        public Money SettledCashTotalInBaseValue
        {
            get
            {
                Money returnValue;
                if (this.Count > 0)
                    returnValue = this.Select(x => x.SettledSize.CurrentBaseAmount).Sum();
                else
                    returnValue = new Money(0m, ParentAccount.BaseCurrency);
                return returnValue;
            }

        }

        public Money UnSettledCashTotalInBaseValue
        {
            get
            {
                Money returnValue;
                if (this.Count > 0)
                    returnValue = this.Select(x => x.UnSettledSize.CurrentBaseAmount).Sum();
                else
                    returnValue = new Money(0m, ParentAccount.BaseCurrency);
                return returnValue;
            }

        }

        public DateTime FirstCashTxDate
        {
            get
            {
                DateTime firstTxDate = DateTime.MinValue;
                if (this.Count > 0)
                    firstTxDate = (from m in this
                                   where m.OpenDate == this.Min(f => f.OpenDate)
                                   select m.OpenDate).ElementAt(0);
                return firstTxDate;
            }
        }

        public ICashSubPosition GetSettledBaseSubPosition()
        {
            ICashSubPosition subPos = null;
            ICashPosition pos = this.Find(x => x.PositionCurrency.IsBase);
            if (pos != null)
                subPos = pos.SettledPosition;
            return subPos;
        }

        public ICashSubPosition GetSubPosition(ICurrency positionCurrency, IGLAccount glAccount)
        {
            ICashPosition newPosition;
            
            newPosition = this.Find(x => ((x.Account.Key == this.ParentAccount.Key) && (x.PositionCurrency.Key == positionCurrency.Key)));
            if (newPosition == null)
            {
                newPosition = new CashPosition(this.ParentAccount, positionCurrency);
                Add(newPosition);
            }

            return newPosition.GetSubPosition(glAccount);
        }

        public ICashPosition GetPosition(ICurrency positionCurrency)
        {
            return this.Find(x => ((x.Account.Key == this.ParentAccount.Key) && (x.PositionCurrency.Key == positionCurrency.Key)));
        }
    }
}

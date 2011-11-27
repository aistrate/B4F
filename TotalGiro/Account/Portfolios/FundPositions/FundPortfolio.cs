using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public class FundPortfolio : TransientDomainCollection<IFundPosition>, IFundPortfolio
    {
        public FundPortfolio()
            : base() { }

        public FundPortfolio(IAccountTypeInternal parentAccount)
            : base()
        {
            ParentAccount = parentAccount;
        }

        public IFundPosition GetPosition(IInstrumentsWithPrices instrument)
        {
            return this.Find(x => ((x.Account.Key == this.ParentAccount.Key) && (x.InstrumentOfPosition.Key == instrument.Key)));
        }

        public IFundPositionTx CreatePositionTx(ITransaction transaction, TransactionSide txSide, PositionsTxValueTypes isCV)
        {
            IFundPositionTx positionTX = new FundPositionTx(transaction, txSide, isCV);
            IInstrumentsWithPrices instrument = (IInstrumentsWithPrices)positionTX.Instrument;
            IFundPosition newPosition = GetPosition(instrument);
            if (newPosition == null)
            {
                newPosition = new FundPosition(this.ParentAccount, instrument);
                Add(newPosition);
            }
            newPosition.PositionTransactions.AddPositionTX(positionTX);
            return positionTX;
        }

        public Money CashFundValueInBaseCurrency
        {
            get
            {
                Money ret; 
                if(this.Any(x => x.InstrumentOfPosition.SecCategory.Key == SecCategories.CashManagementFund))
                    return this.Where(x => x.InstrumentOfPosition.SecCategory.Key == SecCategories.CashManagementFund).Select(c => c.CurrentBaseValue).Sum();
                else
                    return new Money(0m, this.ParentAccount.BaseCurrency);

            }
        }

        public DateTime FirstTxDate
        {
            get
            {
                DateTime firstTxDate = DateTime.MinValue;
                if(this.Count > 0)
                    firstTxDate = (from m in this
                                  where m.OpenDate == this.Min(f => f.OpenDate)
                                  select m.OpenDate).ElementAt(0);
                return firstTxDate;
                                  
            }
        }

        public Money TotalValueInBaseCurrency
        {
            get
            {
                Money returnValue = null;
                if (this.Count > 0)
                    returnValue = this.Select(c => c.CurrentBaseValue).Sum();
                if (returnValue == null)
                    returnValue = new Money(0m, ParentAccount.BaseCurrency);
                return returnValue;
            }
        }

        public IAccountTypeInternal ParentAccount { get; set; }

        public IFundPortfolio NewCollection(Func<IFundPosition, bool> criteria)
        {
            FundPortfolio returnValue = new FundPortfolio(this.ParentAccount);
            returnValue.AddRange(this.Where(criteria));
            return returnValue;
        }

        public IFundPortfolio Exclude(IList<ITradeableInstrument> excludedInstruments)
        {
            if (excludedInstruments != null)
            {
                Func<IFundPosition, bool> predicate = x => x.InstrumentOfPosition.IsTradeable && !excludedInstruments.Contains((ITradeableInstrument)x.InstrumentOfPosition);
                return NewCollection(predicate);
            }
            else
                return this;
        }

        public IFundPortfolio ExcludeNonTradeableInstruments()
        {
            Func<IFundPosition, bool> predicate = x => x.InstrumentOfPosition.IsTradeable;
            return NewCollection(predicate);
        }
    }
}

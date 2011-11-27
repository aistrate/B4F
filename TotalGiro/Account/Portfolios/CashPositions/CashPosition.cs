using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Valuations;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class CashPosition : ICashPosition, ICommonPosition
    {
        #region Constructor

        public CashPosition() 
        {
            this.CreationDate = DateTime.Now;
            unSettledPositions = new CashSubPositionUnSettledCollection(this);

        }

        public CashPosition(IAccountTypeInternal account, ICurrency positionCurrency)
            : this()
        {
            this.Account = account;
            this.PositionCurrency = positionCurrency;
        }

        #endregion

        #region Methods

        public ICashSubPosition GetSubPosition(IGLAccount glAccount)
        {
            ICashSubPosition subPosition;
            CashPositionSettleStatus settleStatus = glAccount.IsSettledWithClient ? CashPositionSettleStatus.Settled : CashPositionSettleStatus.UnSettled;
            switch (settleStatus)
            {
                case CashPositionSettleStatus.Settled:
                    if (SettledPosition == null) SettledPosition = new CashSubPositionSettled(this);
                    subPosition = SettledPosition;
                    break;
                case CashPositionSettleStatus.UnSettled:
                    if (UnSettledPositions.GetSubPosition(glAccount.UnSettledType) == null) UnSettledPositions.AddSubPosition(new CashSubPositionUnSettled(this, glAccount.UnSettledType));
                    subPosition = UnSettledPositions.GetSubPosition(glAccount.UnSettledType);
                    break;
                default:
                    subPosition = SettledPosition;
                    break;
            }
            return subPosition;
        }

        private Money getPositionSize(ICashSubPosition position)
        {
            Money returnValue;
            if (position != null)
                returnValue = position.Size;
            else
                returnValue = new Money(0m, this.Account.BaseCurrency);
            return returnValue;
        }

        #endregion


        #region Props

        public virtual int Key { get; set; }
        public virtual IAccountTypeInternal Account { get; set; }
        public virtual ICurrency PositionCurrency { get; set; }
        public virtual DateTime CreationDate
        {
            get { return (creationDate.HasValue ? creationDate.Value : DateTime.MinValue); }
            set { creationDate = value; }
        }
        public virtual ICashSubPosition SettledPosition { get; set; }
        public virtual ICashSubPosition UnSettledPosition 
        {
            get { return UnSettledPositions.DefaultSubPosition; }
        }

        public virtual ICashSubPositionUnSettledCollection UnSettledPositions
        {
            get
            {
                ICashSubPositionUnSettledCollection positions = (ICashSubPositionUnSettledCollection)unSettledPositions.AsList();
                if (positions.ParentPosition == null) positions.ParentPosition = this;
                return positions;
            }
        }

        public virtual IExRate ExchangeRate
        {
            get { return PositionCurrency.ExchangeRate; }
        }

        public virtual Money SettledSize
        {
            get { return getPositionSize(this.SettledPosition); }
        }

        public virtual Money SettledSizeInBaseCurrency
        {
            get { return SettledSize.CurrentBaseAmount; }
        }

        public virtual Money UnSettledSize 
        {
            get { return getPositionSize(this.UnSettledPosition); }
        }

        public virtual Money UnSettledSizeInBaseCurrency
        {
            get { return UnSettledSize.CurrentBaseAmount; }
        }

        public virtual DateTime OpenDate
        {
            get { return (openDate.HasValue ? openDate.Value : DateTime.MinValue); }
            set { openDate = value; }
        }

        public virtual DateTime LastUpdated
        {
            get { return (lastUpdated.HasValue ? lastUpdated.Value : DateTime.MinValue); }
            set { lastUpdated = value; }
        }

        /// <summary>
        /// The last updated valuation mutations
        /// </summary>
        public virtual IValuationMutation LastMutation
        {
            get { return lastMutation; }
            set { lastMutation = value; }
        }

        /// <summary>
        /// The last updated valuation
        /// </summary>
        public virtual IValuation LastValuation
        {
            get { return lastValuation; }
            private set { lastValuation = value; }
        }

        #endregion

        #region overrides

        public override bool Equals(object obj)
        {
            bool answer = false;
            if (obj is ICashPosition)
            {
                ICashPosition nobj = (ICashPosition)obj;
                answer = ((nobj.Account.Key == this.Account.Key) && (nobj.PositionCurrency.Key == this.PositionCurrency.Key));
            }
            return answer;
        }

        public static bool operator ==(CashPosition lhs, ICashPosition rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(CashPosition lhs, ICashPosition rhs)
        {
            return !lhs.Equals(rhs);
        }

        #endregion

        #region Derived Properties

        public virtual IAssetClass AssetClass
        {
            get { return PositionCurrency.AssetClass; }
        }

        #endregion

        #region Privates

        private DateTime? openDate;
        private DateTime? lastUpdated;
        private DateTime? creationDate;
        private IValuationMutation lastMutation;
        private IValuation lastValuation;
        private IDomainCollection<ICashSubPositionUnSettled> unSettledPositions;

        #endregion

        #region ICommonPosition Members

        public InstrumentSize Size
        {
            get { return this.SettledSize; }
        }

        public IInstrument Instrument
        {
            get { return this.PositionCurrency; }
        }

        public Money CurrentValue
        {
            get { return this.SettledSize; }
        }

        public Money CurrentBaseValue
        {
            get { return this.SettledSizeInBaseCurrency; }
        }

        #endregion
    }
}

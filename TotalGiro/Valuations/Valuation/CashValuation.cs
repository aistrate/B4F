using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Valuations
{
    public class CashValuation : ICashValuation
    {
        /// <summary>
        /// The Key of the valuation. It is a description of the account + instrument + date
        /// </summary>
        public virtual string Key
        {
            get { return key; }
            protected set { key = value; }
        }

        /// <summary>
        /// The relevant account
        /// </summary>
        public virtual IAccountTypeInternal Account
        {
            get { return account; }
        }

        /// <summary>
        /// The relevant instrument
        /// </summary>
        public virtual IInstrument Instrument
        {
            get { return instrument; }
        }

        public virtual ValuationCashTypes ValuationCashType
        {
            get { return valuationCashType; }
        }

        /// <summary>
        /// The date of the valuation
        /// </summary>
        public virtual DateTime Date
        {
            get { return this.date; }
        }

        /// <summary>
        /// The amount that is relevant for the current date. In instrument currency.
        /// </summary>
        public virtual Money Amount
        {
            get { return amount; }
        }

        /// <summary>
        /// The amount that has been realised so far to this date. In instrument currency.
        /// </summary>
        public virtual Money AmountToDate
        {
            get { return amountToDate; }
        }

        /// <summary>
        /// The amount that has been realised so far to this date. In base currency.
        /// </summary>
        public virtual Money BaseAmountToDate
        {
            get { return baseAmountToDate; }
        }

        /// <summary>
        /// The market exchange rate
        /// </summary>
        public virtual decimal MarketRate
        {
            get { return marketRate; }
        }

        #region Overrids

        public override string ToString()
        {
            if (Account != null && Instrument != null)
                return Account.Number + '_' + Instrument.DisplayIsin + '_' + ValuationCashType.ToString() + '_' + Date.ToString("yyyy-MM-dd");
            else
                return base.ToString();
        }

        #endregion

        #region Privates

        private string key;
        private IAccountTypeInternal account;
        private IInstrument instrument;
        private ValuationCashTypes valuationCashType = ValuationCashTypes.None;
        private DateTime date;
        private Money amount;
        private Money amountToDate;
        private Money baseAmountToDate;
        private decimal marketRate;

        #endregion


    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Accounts.Positions;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Valuations
{
    public class ValuationMutation
    {
        #region Constructors

        internal ValuationMutation(ValuationKey key, DateTime mutationDate) 
        {
            this.Key = key;
            this.MutationDate = mutationDate;
        }


        internal ValuationMutation(ValuationKey key, DateTime mutationDate, ValuationMutation prevMutation)
            : this(key, mutationDate)
        {
            if (prevMutation != null)
            {
                this.Size = prevMutation.Size;
                this.BookPrice= prevMutation.BookPrice;
                this.BookValue= prevMutation.BookValue;
                this.RealisedAmountToDate = prevMutation.RealisedAmountToDate;
                this.DepositToDate = prevMutation.DepositToDate;
                this.WithDrawalToDate = prevMutation.WithDrawalToDate;
            }
        }

        #endregion

        #region Methods

        public void AddTx(IPositionTx posTx)
        {
            IsOpenClose isOpen = IsOpenClose.Close;
            InstrumentSize newSize = posTx.Size + Size;

            if (!posTx.IsCashPosition)
            {
                // Determine if it is opening or closing
                if (Size == null || Size.IsZero)
                    isOpen = IsOpenClose.Open;
                else if (newSize.Sign != Size.Sign && newSize.IsNotZero)
                    isOpen = IsOpenClose.Both;
                else
                {
                    if (posTx.Side == Side.Sell || posTx.Side == Side.XO)
                        isOpen = IsOpenClose.Close;
                    else
                        isOpen = IsOpenClose.Open;
                }

                // Realised Amount
                if (isOpen == IsOpenClose.Close)
                {
                    Money amount = (posTx.Price - BookPrice) * (posTx.Size * -1);
                    RealisedAmount += amount;
                    RealisedAmountToDate += amount;
                }
                else if (isOpen == IsOpenClose.Both)
                {
                    // Position has swapped -> the old position size has relised
                    Money amount = (posTx.Price - BookPrice) * Size;
                    RealisedAmount += amount;
                    RealisedAmountToDate += amount;
                }

                // Book Values
                if (isOpen == IsOpenClose.Open)
                {
                    if (BookPrice == null)
                        BookPrice = posTx.Price;
                    else
                    {
                        if (newSize.IsNotZero)
                            BookPrice = ((posTx.Price * posTx.Size) + (Size * BookPrice)) / newSize;
                    }
                    BookValue += (posTx.Value * -1);

                    //if (posTx.IsCashPosition)
                    //{
                    //    //if (posTx.Size.Sign)
                    //}
                }
                else if (isOpen == IsOpenClose.Both)
                {
                    BookPrice = posTx.Price;
                    BookValue = ((newSize * posTx.Price) * -1);
                }
                else // Close
                {
                    if (Size.IsNotZero && newSize.IsNotZero)
                        BookValue = BookValue / (Size.Quantity / newSize.Quantity);
                    else
                        BookValue = BookValue.Clone(0M);
                }
                BookChange += (posTx.Value * -1);
            }
            else
            {
                BookValue += posTx.Value;
                BookChange += posTx.Value;
            }
            Size = newSize;
        }

        #endregion

        public ValuationKey Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public DateTime MutationDate
        {
            get { return mutationDate; }
            set { mutationDate = value; }
        }

        #region Valuation Props

        public InstrumentSize Size
        {
            get { return this.size; }
            set {this.size = value; }
        }

        public Price BookPrice
        {
            get { return this.bookPrice; }
            set { this.bookPrice = value; }
        }

        public Money BookValue
        {
            get { return this.bookValue; }
            set { this.bookValue = value; }
        }

        public Money BookChange
        {
            get { return this.bookChange; }
            set { this.bookChange = value; }
        }
	
        public Money RealisedAmount
        {
            get { return this.realisedAmount; }
            set { this.realisedAmount = value; }
        }

        public Money RealisedAmountToDate
        {
            get { return this.realisedAmountToDate; }
            set { this.realisedAmountToDate = value; }
        }

        #endregion

        #region Deposit & WithDrawal

        public Money Deposit
        {
            get { return this.deposit; }
            set { this.deposit = value; }
        }

        public Money DepositToDate
        {
            get { return this.depositToDate; }
            set { this.depositToDate = value; }
        }

        public Money WithDrawal
        {
            get { return this.withDrawal; }
            set { this.withDrawal = value; }
        }

        public Money WithDrawalToDate
        {
            get { return this.withDrawalToDate; }
            set { this.withDrawalToDate = value; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (Key == null || Key.Instrument == null || Size == null || MutationDate == Util.NullDate)
                return base.ToString();
            else
                return MutationDate.ToShortDateString() + " " + Size.DisplayString + " " + Key.Instrument.DisplayName;
        }

        #endregion

        #region Privates

        private ValuationKey key;
        private DateTime mutationDate = Util.NullDate;
        private InstrumentSize size;
        private Price bookPrice;
        private Money bookValue;
        private Money bookChange;
        private Money realisedAmount;
        private Money realisedAmountToDate;
        private Money deposit;
        private Money depositToDate;
        private Money withDrawal;
        private Money withDrawalToDate;

        private enum IsOpenClose
        {
            Open,
            Close,
            Both
        }

        #endregion

    }
}

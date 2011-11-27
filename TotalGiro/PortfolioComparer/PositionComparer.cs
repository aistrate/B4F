using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.PortfolioComparer
{
    internal class PositionComparer
    {

        public PositionComparer(IInstrument instrument)
        {
            this.instrument = instrument;
        }

        public IInstrument Instrument
        {
            get { return instrument; }
            set { instrument = value; }
        }

        public bool InModel
        {
            get { return inModel; }
            set { inModel = value; }
        }

        public Money ActualPositionValue
        {
            get { return this.actualPositionValue; }
            set { this.actualPositionValue = value; }
        }

		public InstrumentSize ActualPositionSize
		{
			get { return actualPositionSize; }
			set { actualPositionSize = value; }
		}

        public Money ModelPositionValue
        {
            get { return this.modelPositionValue; }
            set { this.modelPositionValue = value; }
        }

		public Money OrderValue
		{
			get { return orderValue; }
			set { orderValue = value; }
		}

        public InstrumentSize OrderSize
        {
            get { return this.orderSize; }
            set { this.orderSize = value; }
        }

        public Money OpenOrderAmount
        {
            get { return openOrderAmount; }
            set { openOrderAmount = value; }
        }

        public Money ReservedCash
        {
            get { return reservedCash; }
            set { reservedCash = value; }
        }

        public string OrderIds
        {
            get { return orderIds; }
            set { orderIds = value; }
        }
	
        public Side Side
        {
            get 
            {
                if (OrderValue != null)
                    return (OrderValue.Sign ? Side.Buy : Side.Sell);
                else
                    return (OrderSize.Sign ? Side.Buy : Side.Sell); 
            }
        }

		public Price LastPrice
		{
			get { return this.lastPrice; }
			set { this.lastPrice = value; }
		}
		
		public bool IsClosure
        {
            get { return this.isClosure; }
            set { this.isClosure = value; }
        }

        public Money ExistingOrderAmount
        {
            get { return existingOrderAmount; }
            set { existingOrderAmount = value; }
        }

        public InstrumentSize ExistingOrderSize
        {
            get { return existingOrderSize; }
            set { existingOrderSize = value; }
        }

        public Money ReservedWithdrawalAmount
        {
            get { return reservedWithdrawalAmount; }
            set { reservedWithdrawalAmount = value; }
        }

		public override string ToString()
		{
			if (orderSize != null)
			{
				return orderSize.ToString();
			}
			else
			{
				return instrument.ToString();
			}
		}

		private IInstrument instrument;
		private bool inModel = false;
        private Money actualPositionValue;
		private InstrumentSize actualPositionSize;
        private Money modelPositionValue;
        private Money orderValue;
        private Money openOrderAmount;
        private Money reservedCash;
        private string orderIds = string.Empty;
        private InstrumentSize orderSize;
		private Price lastPrice;
		private bool isClosure;
        private Money existingOrderAmount;
        private InstrumentSize existingOrderSize;
        private Money reservedWithdrawalAmount;
    }
}

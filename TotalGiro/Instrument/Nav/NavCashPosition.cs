using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Instruments.Prices;

namespace B4F.TotalGiro.Instruments.Nav
{
    public class NavCashPosition : NavPosition, INavCashPosition
    {
        public NavCashPosition()
        {

        }

        public NavCashPosition(InstrumentSize Size, decimal ExchangeRateUsed,
            IHistoricalExRate ExchangeRateRecord)
            : base(Size, ExchangeRateUsed, ExchangeRateRecord)
        {

        }

        /// <summary>
        /// Is this a Security position
        /// </summary>
        public override bool IsSecurityPosition
        {
            get { return false; }
        }

        /// <summary>
        /// Is this a cash Position
        /// </summary>
        public override bool IsCashPosition
        {
            get { return true; }
        }

        /// <summary>
        /// The current value of the position.
        /// The last known price is used to calculate this value
        /// </summary>
        public override void setCurrentValue()
        {
            if ((Size != null) && (ClosingPriceUsed != null))
            {
                this.CurrentValue = Size.CalculateAmount(ClosingPriceUsed);
            }
        }

        /// <summary>
        /// Returns the current value in base currency.
        /// When the nominal currency of the instrument is not the base currency then it is converted using the latest exchange rate.
        /// </summary>
        public override void setCurrentBaseValue()
        {
            if (this.CurrentValue != null)
            {
                if (this.CurrentValue.Underlying.ToCurrency.IsBase)
                    this.CurrentBaseValue = this.CurrentValue;
                else
                    this.CurrentBaseValue = this.CurrentValue.CurrentBaseAmount;
            }

        }



    }
}

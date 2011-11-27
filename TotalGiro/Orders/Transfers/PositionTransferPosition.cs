using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Orders.Transfers
{
    public class PositionTransferPosition : IPositionTransferPosition
    {
        public PositionTransferPosition() { }
        
        public int Key { get; set; }
        public IPositionTransferPortfolio ParentPortfolio { get; set; }
        public InstrumentSize PositionSize { get; set; }
        public Price ActualPrice { get; set; }
        public Decimal ExchangeRate { get; set; }
        public Money ValueVV { get; set; }
        public Money ValueinEuro { get; set; }
        public Decimal PercentageOfPortfolio { get; set; }
        public Decimal FundPercentageOfPortfolio { get; set; }
        public bool IsFundPosition { get { return !this.InstrumentOfPosition.IsCash; } }
        public IInstrument InstrumentOfPosition
        {
            get
            {
                return this.PositionSize != null ? this.PositionSize.Underlying : null;
                 
            }
        }
        public string Isin
        {
            get
            {
                if (this.InstrumentOfPosition.IsTradeable)
                    return ((IInstrumentsWithPrices)this.InstrumentOfPosition).Isin;
                else
                    return "CASH";
            }
        }
        public bool IsEditable { get { return true; } }
        public bool IsDeletable { get { return !this.ParentPortfolio.ParentTransfer.Executed; } }        
        public string InstrumentDescription { get { return this.InstrumentName.PadRight(40,' ') + " - " + this.Isin; } }
        public string InstrumentName { get { return this.InstrumentOfPosition.Name; } }
        public Decimal Size { get { return this.PositionSize.Quantity; } }
        public DateTime CreationDate { get { return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue; } }
        public string PriceShortDisplayString { get { return (this.ActualPrice != null ? this.ActualPrice.ShortDisplayString : ""); } }

        public override int GetHashCode()
        {
            return this.InstrumentOfPosition.GetHashCode();
        }

        #region Privates

        private DateTime? creationDate;

        #endregion

    }
}

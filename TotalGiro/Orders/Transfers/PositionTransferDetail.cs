using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Orders.Transfers
{
    public class PositionTransferDetail : IPositionTransferDetail
    {
        public PositionTransferDetail()
        {
            this.transactions = new TransactionNTMCollection(this);
        }

        public PositionTransferDetail(IPositionTransferPosition oldPosition, TransferType typeOfTransfer,
            decimal transferAmount)
            : this()
        {
            this.TransferPrice = this.ActualPrice = oldPosition.ActualPrice;
            this.ExchangeRate = oldPosition.ExchangeRate;

            if (typeOfTransfer == TransferType.Full)
            {
                this.PositionSize = oldPosition.PositionSize;
                this.ValueVV = oldPosition.ValueVV;
                this.ValueinEuro = oldPosition.ValueinEuro;
            }
            else if (typeOfTransfer == TransferType.Amount)
            {
                this.ValueinEuro = oldPosition.ValueinEuro.Clone(transferAmount * oldPosition.PercentageOfPortfolio);
                this.ValueVV = (this.ValueinEuro / oldPosition.ExchangeRate);
                this.PositionSize = this.ValueVV.CalculateSize(this.TransferPrice);
            }
        }

        public int Key { get; set; }
        public IPositionTransfer ParentTransfer { get; set; }
        public InstrumentSize PositionSize { get; set; }
        public Price ActualPrice { get; set; }
        public Price TransferPrice { get; set; }
        public Decimal ExchangeRate { get; set; }
        public Money ValueVV { get; set; }
        public TransferDirection TxDirection { get; set; }

        public TransferStatus ParentStatus
        {
            get
            {
                return this.ParentTransfer.TransferStatus;
            }
        }

        public bool IsEditable
        {
            get
            {
                return !((this.ParentStatus == TransferStatus.Executed) || (this.ParentStatus == TransferStatus.Cancelled));
            }
        }


        public bool IsDeletable
        {
            get
            {
                return !((this.ParentStatus == TransferStatus.Executed) || (this.ParentStatus == TransferStatus.Cancelled));
            }
        }
        public string InstrumentDescription
        {
            get
            {
                return this.InstrumentName.PadRight(40, ' ') + " - " + this.Isin;
            }
        }
        public string InstrumentName
        {
            get
            {
                return this.InstrumentOfPosition != null ? this.InstrumentOfPosition.Name : null;
            }
        }
        public Decimal Size
        {
            get
            {
                return this.PositionSize != null ? this.PositionSize.Quantity : 0m;
            }
        }
        public DateTime CreationDate
        {
            get
            {
                return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue;
            }
        }
        public string TransferPriceShortDisplayString
        {
            get
            {
                return (this.TransferPrice != null ? this.TransferPrice.ShortDisplayString : "");
            }
        }
        public string ActualPriceShortDisplayString
        {
            get
            {
                return (this.ActualPrice != null ? this.ActualPrice.ShortDisplayString : "");
            }
        }
        public DateTime TransferDate
        {
            get
            {
                return this.ParentTransfer.TransferDate;
            }
        }

        public bool IsFundPosition
        {
            get
            {
                return this.InstrumentOfPosition != null ? this.InstrumentOfPosition.IsSecurity : false;
            }
        }

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
                if (InstrumentOfPosition != null)
                {
                    if (this.InstrumentOfPosition.IsTradeable)
                        return ((IInstrumentsWithPrices)this.InstrumentOfPosition).Isin;
                    else
                        return "CASH";
                }
                else return "";
            }
        }

        public Money ValueinEuro
        {
            get
            {
                return this.valueinEuro != null ? this.valueinEuro : new Money(0m, this.ParentTransfer.BaseCurrency);
            }
            set
            {
                this.valueinEuro = value;
            }
        }

        public virtual ITransactionNTMCollection Transactions
        {
            get
            {
                ITransactionNTMCollection temp = (ITransactionNTMCollection)transactions.AsList();
                if (temp.TransferDetail == null) temp.TransferDetail = this;
                return temp;
            }
        }

        #region Privates

        private DateTime? creationDate;
        private Money valueinEuro;
        private IDomainCollection<ITransactionNTM> transactions;

        #endregion
    }
}

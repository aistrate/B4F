using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public class OrderFillView
    {
        public OrderFillView()
        {
            ExchangeRate = 1M;
            SizeDecimals = 6;
            AmountDecimals = 2;
            TransactionDate = DateTime.Now;
            TransactionTime = DateTime.Today.AddHours(10);
            SettlementDate = DateTime.MinValue;
        }

        public OrderFillView(int orderId, decimal size, decimal amount, 
            decimal price, decimal exchangeRate, bool isSizeBased)
            : this()
        {
            this.OrderId = orderId;
            this.Size = size;
            this.Amount = amount;
            this.Price = price;
            this.ExchangeRate = exchangeRate;
            this.IsSizeBased = isSizeBased;
        }

        public OrderFillView(int orderId, decimal size, decimal amount,
            decimal price, decimal exchangeRate, bool isSizeBased,
            decimal serviceChargeAmount, decimal accruedInterest, int exchangeId)
            : this(orderId, size, amount, price, exchangeRate, isSizeBased)
        {
            this.ServiceChargeAmount = serviceChargeAmount;
            this.AccruedInterestAmount = accruedInterest;
            this.ExchangeId = exchangeId;
        }

        public OrderFillView(int orderId, decimal size, decimal amount, 
            decimal price, decimal exchangeRate, bool isSizeBased, 
            DateTime transactionDate, int counterpartyAccountId, int exchangeId)
            : this(orderId, size, amount, price, exchangeRate, isSizeBased)
        {
            this.TransactionDate = transactionDate;
            this.CounterpartyAccountId = counterpartyAccountId;
            this.ExchangeId = exchangeId;
        }

        public int OrderId { get; set; }
        public decimal Size { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public int CounterpartyAccountId { get; set; }
        public int ExchangeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime TransactionTime { get; set; }
        public DateTime SettlementDate { get; set; }
        public bool IsSizeBased { get; set; }
        public bool DoesSupportServiceCharge { get; set; }
        public decimal ServiceChargePercentage { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public int SizeDecimals { get; set; }
        public int AmountDecimals { get; set; }
        public decimal TickSize { get; set; }
        public string SizeSymbol { get; set; }
        public string AmountSymbol { get; set; }
        public string Warning { get; set; }
        public bool IsCompleteFill { get; set; }
        public decimal FillPercentage { get; set; }
        public string ServiceChargeDisplayInfo { get; set; }
        public decimal ExchangeRate { get; set; }
        public bool ShowExRate { get; set; }
        public bool IsBondOrder { get; set; }
        public bool ShowAccruedInterest { get; set; }
        public decimal AccruedInterestAmount { get; set; }

        public bool IsAmountBased
        {
            get { return !IsSizeBased; }
        }

        public string DisplaySize
        {
            get { return Util.FormatDecimal(Size, 2, SizeDecimals); }
        }

        public string DisplayAmount
        {
            get { return Util.FormatDecimal(Amount, AmountDecimals); }
        }

        public string DisplayAccruedInterestAmount
        {
            get { return Util.FormatDecimal(AccruedInterestAmount, AmountDecimals); }
        }

        public string PriceSymbol
        {
            get { return (SizeSymbol != null && SizeSymbol != string.Empty ? AmountSymbol + " / " + SizeSymbol : ""); }
        }

        public string DisplayFillPercentage
        {
            get 
            {
                decimal temp = Math.Truncate(FillPercentage * 1000000);
                temp /= 10000;
                return temp.ToString("0.000###") + "%"; 
            }
        }

        public string DisplayServiceChargePercentage
        {
            get { return (ServiceChargePercentage > 0 ? (ServiceChargePercentage * 100m).ToString("0.####") : ""); }
        }
        
        public string DisplayServiceChargeAmount
        {
            get { return (ServiceChargePercentage > 0 ? Util.FormatDecimal(ServiceChargeAmount, AmountDecimals) : ""); }
        }
    }
}

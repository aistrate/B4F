using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public class OrderEditView
    {
        public OrderEditView()
        {
        }

        public OrderEditView(int orderId, int numberofDecimals, int routeId)
        {
            this.OrderId = orderId;
            this.NumberOfDecimals = numberofDecimals;
            this.RouteId = routeId;
        }

        public OrderEditView(int orderId, string amountSymbol, decimal price,
            int numberofdecimals, bool isBondOrder, DateTime expectedSettlementDate)
        {                                 
            this.OrderId = orderId;
            this.AmountSymbol = amountSymbol;
            this.Price = price;
            this.NumberOfDecimals = numberofdecimals;
            this.IsBondOrder = isBondOrder;
            this.ExpectedSettlementDate = expectedSettlementDate;
        }

        public OrderEditView(int orderId, decimal exchangeRate, 
            decimal convertedAmount, decimal originalAmount, string amountSymbol)
        {                                 
            this.OrderId = orderId;
            this.AmountSymbol = amountSymbol;
            this.ExRate = exchangeRate;
            this.ConvertedAmount = convertedAmount;
            this.OriginalAmount = originalAmount;
        }

        public int OrderId { get; set; }
        public int NumberOfDecimals { get; set; }
        public int RouteId { get; set; }
        public string AmountSymbol { get; set; }
        public decimal Price { get; set; }
        public bool IsSizeBased { get; set; }
        public string Warning { get; set; }
        public decimal ExRate { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public bool IsBondOrder { get; set; }
        public DateTime ExpectedSettlementDate { get; set; }

        public string PriceSymbol
        {
            get { return AmountSymbol + "/inst."; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public class CurrencySubtotalRowView
    {
        private string currencyName;
        private int orderCount;
        private Money value;

        public CurrencySubtotalRowView(ICurrency currency)
        {
            this.currencyName = currency.Name;
            orderCount = 0;
            this.value = new Money(0m, currency);
        }

        public string CurrencyName
        {
            get { return this.currencyName; }
        }
        
        public int OrderCount
        {
            get { return this.orderCount; }
            set { this.orderCount = value; }
        }
        
        public Money Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}

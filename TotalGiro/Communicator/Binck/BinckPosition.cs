using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Communicator.Binck
{
    public class BinckPosition
    {
        public int Key { get; set; }
        public string AccountNumber { get; set; }
        public DateTime BalanceDate { get; set; }
        public string Symbol { get; set; }
        public string ISIN { get; set; }
        public string DerSymbol { get; set; }
        public DateTime DerExpiratie { get; set; }
        public Decimal DerStrikePrice { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
        public decimal PositionSize { get; set; }
        public decimal ExRate { get; set; }
        public bool BlockSize { get; set; }
        public decimal ValueRapCurr { get; set; }
        public decimal ValueOrgCurr { get; set; }
        public decimal StartvalOrgCurr { get; set; }
        public string StockType { get; set; }
        public string FondsCode { get; set; }
        public string Description { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records
{
    public class BinckRecord : ImportedRecord
    {

        public BinckRecord() { }
        public BinckRecord(string newLine)
        {
            ImportFormatter FormattedData = new ImportFormatter(newLine);

            this.AccountNumber = FormattedData[0].PadRight(7);
            this.BalanceDate = FormattedData.AssignDateValue(1);
            this.Symbol = FormattedData[2].PadRight(25);
            this.ISIN = FormattedData[3].PadRight(15);
            this.DerSymbol = FormattedData[4].PadRight(10);
            this.DerExpiratie = FormattedData[5].PadRight(15);
            this.DerStrikePrice = FormattedData[6].PadRight(5);
            this.Currency = FormattedData[7].PadRight(3);
            this.Price = FormattedData.AssignDecimalValue(8);
            this.PositionSize = FormattedData.AssignDecimalValue(9);
            this.ExRate = FormattedData.AssignDecimalValue(10);
            this.BlockSize = FormattedData.AssignBoolValue(11);
            this.ValueRapCurr = FormattedData.AssignDecimalValue(12);
            this.ValueOrgCurr = FormattedData.AssignDecimalValue(13);
            this.StartvalOrgCurr = FormattedData.AssignDecimalValue(14);
            this.StockType = FormattedData[15].PadRight(50);
            this.FondsCode = FormattedData[16].PadRight(8);
            this.Description = FormattedData[17].PadRight(75);
        }

        public string AccountNumber { get; set; }
        public DateTime BalanceDate { get; set; }
        public string Symbol { get; set; }
        public string ISIN { get; set; }
        public string DerSymbol { get; set; }
        public string DerExpiratie { get; set; }
        public string DerStrikePrice { get; set; }
        public string Currency { get; set; }
        public Decimal Price { get; set; }
        public Decimal PositionSize { get; set; }
        public decimal ExRate { get; set; }
        public bool BlockSize { get; set; }
        public Decimal ValueRapCurr { get; set; }
        public Decimal ValueOrgCurr { get; set; }
        public decimal StartvalOrgCurr { get; set; }
        public string StockType { get; set; }
        public string FondsCode { get; set; }
        public string Description { get; set; }
        public decimal AccruedInterest { get; set; }
        public ImportFormatter FormattedData { get; set; }
    }
}

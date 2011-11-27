using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public class CashDetailLine
    {
        //public CashDetailLine(string type, DateTime txDate, DateTime creationdate, string cashComponent, string ledgerCode, Money amount)
        //{
        //    this.Type = type;
        //    this.Transactiondate = txDate;
        //    this.CreationDate = creationdate;
        //    this.CashComponent = cashComponent;
        //    this.LedgerCode = ledgerCode;
        //    this.Amount = amount;
        //}

        public CashDetailLine(IJournalEntryLine line)
        {
            this.Type = line.GLAccount.Description;
            this.Transactiondate = line.Parent.TransactionDate;
            this.CreationDate = line.CreationDate;
            this.CashComponent = line.GLAccount.Description; ;
            this.LedgerCode = line.GLAccount.GLAccountNumber;
            this.Amount = line.Balance.Negate();
            this.Side = "";
            this.Fund = "";
        }

        public CashDetailLine(IJournalEntryLine line, bool IsFirstLine)
        {
            if (IsFirstLine)
            {
                ITransactionOrder tx = (ITransactionOrder)((ITransactionComponent)line.BookComponent.Parent).ParentTransaction;
                this.TradeID = tx.Key;
                this.Type = tx.TransactionTypeDisplay;
                if (tx.TxSide == B4F.TotalGiro.Orders.Side.Buy)
                    this.Side = "Aankoop";
                else
                    this.Side = "Verkoop";

                this.Size = tx.ValueSize.Quantity;
                this.Fund = tx.TradedInstrument.Name;
                this.Price = tx.Price;
                this.Transactiondate = line.Parent.TransactionDate;
                this.CreationDate = line.CreationDate;
            }

            this.CashComponent = line.GLAccount.Description; ;
            this.LedgerCode = line.GLAccount.GLAccountNumber;
            this.Amount = line.Balance.Negate();

        }


        public CashDetailLine() { }
        public int Key { get; set; }
        public int GroupKey { get; set; }
        public int GroupSubKey { get; set; }
        public int TradeID { get; set; }
        public string Type { get; set; }
        public string Side { get; set; }
        public Decimal Size { get; set; }
        public string Fund { get; set; }
        public Price Price { get; set; }
        public string PriceDisplay
        {
            get
            {
                if (Price != null)
                    return Price.DisplayString;
                else
                    return "";
            }
        }
        public DateTime Transactiondate { get; set; }
        public DateTime CreationDate { get; set; }
        public string CashComponent { get; set; }
        public string LedgerCode { get; set; }
        public Money Amount { get; set; }
        public string AmountDisplay { get { return Amount.DisplayString; } }
        public Money Saldo { get; set; }
    }
}

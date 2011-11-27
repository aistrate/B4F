using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal.Balances
{
    public abstract class BalanceRecord
    {
        public int Key { get; set; }
        public int LineNumber { get; set; }
        public Money Debit { get; set; }
        public Money Credit { get; set; }
        public DateTime TransactionDate { get; set; }
        public abstract string AccountNumber { get; }
        public abstract string FullDescription { get; }

        public string DebitDisplayString
        {
            get { return (Debit != null && Debit.Quantity != 0m ? Debit.ToString("{0:#,##0.00}") : ""); }
        }


        public string CreditDisplayString
        {
            get { return (Credit != null && Credit.Quantity != 0m ? Credit.ToString("{0:#,##0.00}") : ""); }
        }

        public Money Balance
        {
            get { return Credit - Debit; }
            set
            {
                if (value.IsGreaterThanZero)
                {
                    Debit = value.ZeroedAmount();
                    Credit = value;
                }
                else if (value.IsLessThanZero)
                {
                    Debit = value.Abs();
                    Credit = value.ZeroedAmount();
                }
                else
                    throw new ApplicationException("Cannot set Balance of Trial BalanceRecord Line to zero.");
            }
        }
    }
}

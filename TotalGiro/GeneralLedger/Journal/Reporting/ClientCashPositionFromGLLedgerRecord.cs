using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal.Reporting
{
    public class ClientCashPositionFromGLLedgerRecord : IClientCashPositionFromGLLedgerRecord
    {
        public int Key { get; set; }
        public IAccountTypeInternal InternalAccount { get; set; }
        public int LineNumber { get; set; }
        public DateTime BookDate { get; set; }
        public Money Debit { get; set; }
        public string DebitDisplayString
        {
            get { return (Debit != null && Debit.Quantity != 0m ? Debit.ToString("{0:#,##0.00}") : ""); }
        }
        public Money Credit { get; set; }
        public string CreditDisplayString
        {
            get { return (Credit != null && Credit.Quantity != 0m ? Credit.ToString("{0:#,##0.00}") : ""); }
        }

    }
}

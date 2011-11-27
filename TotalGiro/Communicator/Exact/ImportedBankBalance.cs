using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class ImportedBankBalance : IImportedBankBalance
    {
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual IJournal BankJournal { get; set; }

        public virtual DateTime BookBalanceDate
        {
            get { return bookBalanceDate; }
            set { bookBalanceDate = value; }
        }

        //public virtual string BookBalanceCurrCode
        //{
        //    get { return bookBalanceCurrCode; }
        //    set { bookBalanceCurrCode = value; }
        //}

        public virtual Money BookBalance
        {
            get { return bookBalance; }
            set { bookBalance = value; }
        }
        
        private int key;
        private string bankAccountNumber;
        private DateTime bookBalanceDate;
        //private string bookBalanceCurrCode;
        private Money bookBalance;
    }
}

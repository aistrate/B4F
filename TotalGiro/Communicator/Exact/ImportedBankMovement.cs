using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class ImportedBankMovement : IImportedBankMovement
    {
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual IJournal BankJournal { get; set; }

        public virtual DateTime BankStatementDate
        {
            get { return bankStatementDate; }
            set { bankStatementDate = value; }
        }

        public virtual DateTime CloseBalanceProcessDate
        {
            get { return closeBalanceProcessDate; }
            set { closeBalanceProcessDate = value; }
        }

        public virtual string CloseBalanceProcessTime
        {
            get { return closeBalanceProcessTime; }
            set { closeBalanceProcessTime = value; }
        }

        //public virtual string MovementCurrCode
        //{
        //    get { return movementCurrCode; }
        //    set { movementCurrCode = value; }
        //}

        public virtual Money MovementAmount
        {
            get { return movementAmount; }
            set { movementAmount = value; }
        }

        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        private int key;
        private string bankAccountNumber;
        private DateTime bankStatementDate;
        private DateTime closeBalanceProcessDate;
        private string closeBalanceProcessTime;
        //private string movementCurrCode;
        private Money movementAmount;
        private string description;
    }
}

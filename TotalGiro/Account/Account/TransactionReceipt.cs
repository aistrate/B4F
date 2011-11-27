using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Accounts
{
			
    /// <summary>
    /// Class to hold the fysical (pdf files) with transaction information that are sent to the client
    /// </summary>
    public class TransactionReceipt
    {

        public int TransactionReceiptId
        {
            get { return transactionreceiptid; }
            set { transactionreceiptid = value; }
        }

        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        public long FileSize
        {
            get { return filesize; }
            set { filesize = value; }
        }

        public Byte[] FileContent
        {
            get { return filecontent; }
            set { filecontent = value; }
        }

        public DateTime FileDate
        {
            get { return filedate; }
            set { filedate = value; }
        }

        public DateTime CreationDate
        {
            get { return creationdate; }
            set { creationdate = value; }
        }

        public DateTime ReceiptDate
        {
            get { return receiptdate; }
            set { receiptdate = value; }
        }

        public IAccountTypeInternal GiroAccount
        {
            get { return giroAccount; }
            set { giroAccount = value; }
        }

        #region PrivateVariables

        private int transactionreceiptid;
        private string filename;
        private long filesize;
        private Byte[] filecontent;
        private DateTime filedate;
        private DateTime creationdate;
        private DateTime receiptdate;
        private IAccountTypeInternal giroAccount;

        #endregion

    }
}

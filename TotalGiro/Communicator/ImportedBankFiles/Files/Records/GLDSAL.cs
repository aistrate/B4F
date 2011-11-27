using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records
{
    public class GLDSAL : ImportedRecord
    {
        protected GLDSAL() { }

        public GLDSAL(string ReadLine)
        {
            this.BankMessageNr = ReadLine.Substring(0, 16);
            this.BankAcctID = ReadLine.Substring(32, 10);
            this.BankStatementNr = ReadLine.Substring(42, 5);
            this.BankPageNr = ReadLine.Substring(47, 3);

            this.BookBalanceDate = convertToDate(ReadLine.Substring(122, 6));
            this.BookBalanceCurrCode = ReadLine.Substring(128, 3);
            this.BookBalance = convertToDecimal(ReadLine.Substring(121, 1), ReadLine.Substring(131, 17), 2);

            this.AvailBalanceDate = convertToDate(ReadLine.Substring(149, 6));
            this.AvailBalanceCurrCode = ReadLine.Substring(155, 3);
            this.AvailBalance = convertToDecimal(ReadLine.Substring(148, 1), ReadLine.Substring(158, 17), 2);

            this.ForBalanceDate = convertToDate(ReadLine.Substring(176, 6));
            this.ForBalanceCurrCode = ReadLine.Substring(182, 3);
            this.ForBalance = convertToDecimal(ReadLine.Substring(175, 1), ReadLine.Substring(185, 17), 2);
        }



        //public int Key
        //{
        //    get { return key; }
        //    set { key = value; }
        //}

        public string BankMessageNr
        {
            get { return bankMessageNr; }
            set { bankMessageNr = value; }
        }

        public string BankAcctID
        {
            get { return bankAcctID; }
            set { bankAcctID = value; }
        }

        public string BankStatementNr
        {
            get { return bankStatementNr; }
            set { bankStatementNr = value; }
        }

        public string BankPageNr
        {
            get { return bankPageNr; }
            set { bankPageNr = value; }
        }

        public DateTime BookBalanceDate
        {
            get { return bookBalanceDate; }
            set { bookBalanceDate = value; }
        }

        public string BookBalanceCurrCode
        {
            get { return bookBalanceCurrCode; }
            set { bookBalanceCurrCode = value; }
        }

        public decimal BookBalance
        {
            get { return bookBalance; }
            set { bookBalance = value; }
        }

        public DateTime AvailBalanceDate
        {
            get { return availBalanceDate; }
            set { availBalanceDate = value; }
        }

        public string AvailBalanceCurrCode
        {
            get { return availBalanceCurrCode; }
            set { availBalanceCurrCode = value; }
        }

        public decimal AvailBalance
        {
            get { return availBalance; }
            set { availBalance = value; }
        }

        public DateTime ForBalanceDate
        {
            get { return forBalanceDate; }
            set { forBalanceDate = value; }
        }

        public string ForBalanceCurrCode
        {
            get { return forBalanceCurrCode; }
            set { forBalanceCurrCode = value; }
        }

        public decimal ForBalance
        {
            get { return forBalance; }
            set { forBalance = value; }
        }

        private Decimal convertToDecimal(string sign, string balance, int noOfDecimals)
        {
            decimal result;
            decimal fraction;
            decimal divisor = Decimal.Parse((System.Math.Pow(10, noOfDecimals)).ToString());
            Decimal.TryParse(balance.Substring(0, (balance.Length - noOfDecimals)), out result);
            Decimal.TryParse(balance.Substring((balance.Length - noOfDecimals), noOfDecimals), out fraction);
            result += decimal.Divide(fraction, divisor);
            return sign == "1" ? result : -1 * result;

        }
        private DateTime convertToDate(string tryDate)
        {
            DateTime test;
            DateTime.TryParse(tryDate.Substring(2, 2) + "/" + tryDate.Substring(4, 2) + "/" + tryDate.Substring(0, 2), new CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out test);
            return test;
        }

        #region Private Variables

        //private int key;
        private string bankMessageNr;
        private string bankAcctID;
        private string bankStatementNr;
        private string bankPageNr;
        private DateTime bookBalanceDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private string bookBalanceCurrCode;
        private decimal bookBalance;
        private DateTime availBalanceDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private string availBalanceCurrCode;
        private decimal availBalance;
        private DateTime forBalanceDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private string forBalanceCurrCode;
        private decimal forBalance;


        #endregion



    }
}

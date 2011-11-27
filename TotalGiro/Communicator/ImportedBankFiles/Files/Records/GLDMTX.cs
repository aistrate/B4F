using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records
{
    public class GLDMTX : ImportedRecord
    {
        protected GLDMTX() { }

        public GLDMTX(string ReadLine)
        {
            this.ClientNumber = ReadLine.Substring(0, 5);
            this.BankAcctID = ReadLine.Substring(5, 35);
            this.BankStatementDate = convertToDate(ReadLine.Substring(40, 8));
            this.BankStatementNr = ReadLine.Substring(48, 5);
            this.BankMessageNr = ReadLine.Substring(53, 16);
            this.OpenBalanceDate = convertToDate(ReadLine.Substring(69, 8));
            this.OpenBalanceCurrCode = ReadLine.Substring(77, 3);
            this.OpenBalance = convertToDecimal(ReadLine.Substring(80, 1), ReadLine.Substring(81, 15), 2);
            this.ReferenceBank = ReadLine.Substring(96, 16);
            this.ReferenceClient = ReadLine.Substring(112, 16);
            this.SwiftMsgeRef = ReadLine.Substring(128, 4);
            this.MovementValueDate = convertToDate(ReadLine.Substring(132, 8));
            this.MovementEntryDate = convertToDate(ReadLine.Substring(140, 8));
            this.ReversalIndication = ReadLine.Substring(148, 1);
            this.MovementCurrCode = ReadLine.Substring(149, 3);
            this.MovementAmt = convertToDecimal(ReadLine.Substring(152, 1), ReadLine.Substring(153, 15), 2);
            this.CPAcctNr = ReadLine.Substring(168, 35);
            this.CPName = ReadLine.Substring(203, 35);
            this.CPAddress = ReadLine.Substring(238, 35);
            this.CPResidence = ReadLine.Substring(273, 35);
            this.CPCountry = ReadLine.Substring(308, 35);
            this.BankTransactionType = ReadLine.Substring(343, 4);
            this.BankTXTypeDesc = ReadLine.Substring(347, 50);
            this.BankTxTypeInformation = ReadLine.Substring(397, 390);
            this.CloseBalanceProcessDate = convertToDate(ReadLine.Substring(787, 8));
            this.CloseBalanceProcessTime = ReadLine.Substring(795, 7);
            this.CloseBalanceCurrCode = ReadLine.Substring(802, 3);
            this.CloseBalance = convertToDecimal(ReadLine.Substring(805, 1), ReadLine.Substring(806, 15), 2);
            this.TXReferenceNumber = ReadLine.Substring(821, 16);
            this.RelatedReference = ReadLine.Substring(837, 16);
            this.TXCodeSwift = ReadLine.Substring(853, 4);

        }

        //public int Key
        //{
        //    get { return key; }
        //    set { key = value; }
        //}

        public string ClientNumber
        {
            get { return clientNumber; }
            set { clientNumber = value; }
        }

        public string BankAcctID
        {
            get { return bankAcctID; }
            set { bankAcctID = value; }
        }

        public DateTime BankStatementDate
        {
            get { return bankStatementDate; }
            set { bankStatementDate = value; }
        }

        public string BankStatementNr
        {
            get { return bankStatementNr; }
            set { bankStatementNr = value; }
        }

        public string BankMessageNr
        {
            get { return bankMessageNr; }
            set { bankMessageNr = value; }
        }

        public DateTime OpenBalanceDate
        {
            get { return openBalanceDate; }
            set { openBalanceDate = value; }
        }

        public string OpenBalanceCurrCode
        {
            get { return openBalanceCurrCode; }
            set { openBalanceCurrCode = value; }
        }

        public decimal OpenBalance
        {
            get { return openBalance; }
            set { openBalance = value; }
        }

        public string ReferenceBank
        {
            get { return referenceBank; }
            set { referenceBank = value; }
        }

        public string ReferenceClient
        {
            get { return referenceClient; }
            set { referenceClient = value; }
        }

        public string SwiftMsgeRef
        {
            get { return swiftMsgeRef; }
            set { swiftMsgeRef = value; }
        }

        public DateTime MovementValueDate
        {
            get { return movementValueDate; }
            set { movementValueDate = value; }
        }

        public DateTime MovementEntryDate
        {
            get { return movementEntryDate; }
            set { movementEntryDate = value; }
        }

        public string ReversalIndication
        {
            get { return reversalIndication; }
            set { reversalIndication = value; }
        }

        public string MovementCurrCode
        {
            get { return movementCurrCode; }
            set { movementCurrCode = value; }
        }

        public decimal MovementAmt
        {
            get { return movementAmt; }
            set { movementAmt = value; }
        }

        public string CPAcctNr
        {
            get { return cPAcctNr; }
            set { cPAcctNr = value; }
        }

        public string CPName
        {
            get { return cPName; }
            set { cPName = value; }
        }

        public string CPAddress
        {
            get { return cPAddress; }
            set { cPAddress = value; }
        }

        public string CPResidence
        {
            get { return cPResidence; }
            set { cPResidence = value; }
        }

        public string CPCountry
        {
            get { return cPCountry; }
            set { cPCountry = value; }
        }

        public string BankTransactionType
        {
            get { return bankTransactionType; }
            set { bankTransactionType = value; }
        }

        public string BankTXTypeDesc
        {
            get { return bankTXTypeDesc; }
            set { bankTXTypeDesc = value; }
        }

        public string BankTxTypeInformation
        {
            get { return bankTxTypeInformation; }
            set { bankTxTypeInformation = value; }
        }

        public DateTime CloseBalanceProcessDate
        {
            get { return closeBalanceProcessDate; }
            set { closeBalanceProcessDate = value; }
        }

        public string CloseBalanceProcessTime
        {
            get { return closeBalanceProcessTime; }
            set { closeBalanceProcessTime = value; }
        }

        public string CloseBalanceCurrCode
        {
            get { return closeBalanceCurrCode; }
            set { closeBalanceCurrCode = value; }
        }

        public decimal CloseBalance
        {
            get { return closeBalance; }
            set { closeBalance = value; }
        }

        public string TXReferenceNumber
        {
            get { return tXReferenceNumber; }
            set { tXReferenceNumber = value; }
        }

        public string RelatedReference
        {
            get { return relatedReference; }
            set { relatedReference = value; }
        }

        public string TXCodeSwift
        {
            get { return tXCodeSwift; }
            set { tXCodeSwift = value; }
        }

        private DateTime convertToDate(string tryDate)
        {
            DateTime test;
            DateTime.TryParse(tryDate.Substring(4, 2) + "/" + tryDate.Substring(6, 2) + "/" + tryDate.Substring(0, 4), new CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out test);
            return test;
        }

        private Decimal convertToDecimal(string sign, string balance, int noOfDecimals)
        {
            decimal result;
            decimal fraction;
            decimal divisor = Decimal.Parse((System.Math.Pow(10, noOfDecimals)).ToString());
            Decimal.TryParse(balance.Substring(0, (balance.Length - noOfDecimals)), out result);
            Decimal.TryParse(balance.Substring((balance.Length - noOfDecimals), noOfDecimals), out fraction);
            result += decimal.Divide(fraction, divisor);
            return sign == "+" ? result : -1 * result;

        }

        #region Private Variables

        //private int key;	
        private string clientNumber;
        private string bankAcctID;
        private DateTime bankStatementDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private string bankStatementNr;
        private string bankMessageNr;
        private DateTime openBalanceDate;
        private string openBalanceCurrCode;
        private decimal openBalance;
        private string referenceBank;
        private string referenceClient;
        private string swiftMsgeRef;
        private DateTime movementValueDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private DateTime movementEntryDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private string reversalIndication;
        private string movementCurrCode;
        private decimal movementAmt;
        private string cPAcctNr;
        private string cPName;
        private string cPAddress;
        private string cPResidence;
        private string cPCountry;
        private string bankTransactionType;
        private string bankTXTypeDesc;
        private string bankTxTypeInformation;
        private DateTime closeBalanceProcessDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private string closeBalanceProcessTime;
        private string closeBalanceCurrCode;
        private decimal closeBalance;
        private string tXReferenceNumber;
        private string relatedReference;
        private string tXCodeSwift;

        #endregion

    }
}

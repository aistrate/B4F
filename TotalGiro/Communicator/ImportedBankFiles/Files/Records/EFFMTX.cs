using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records
{
    public class EFFMTX : ImportedRecord
    {
        protected EFFMTX() { }

        public EFFMTX(string ReadLine)
        {
            this.BankMsgeNr = ReadLine.Substring(0, 16);
            this.MsgeRefersTo = ReadLine.Substring(16, 16);
            this.PageNr = ReadLine.Substring(32, 5);
            this.FollowUpInd = Int16.Parse(ReadLine.Substring(37, 2));
            this.BankAcctID = ReadLine.Substring(39, 34);
            this.BankDetails01 = ReadLine.Substring(73, 35);
            this.BankDetails02 = ReadLine.Substring(108, 35);
            this.BankDetails03 = ReadLine.Substring(143, 35);
            this.BeginOfPeriod = convertToDate(ReadLine.Substring(178, 8));
            this.EndOfPeriod = convertToDate(ReadLine.Substring(186, 8));
            this.CreationDate = convertToDate(ReadLine.Substring(194, 8));
            this.BankSecCodeType = ReadLine.Substring(202, 4);
            this.SecuritiesCode = ReadLine.Substring(206, 12);
            this.SecDesc = ReadLine.Substring(218, 35);
            this.BankSecType = ReadLine.Substring(253, 1);
            this.InterestRate = convertToDecimal("+", ReadLine.Substring(254, 7), 5);
            this.StockExchCountryCode = ReadLine.Substring(261, 2);
            this.Quotation = ReadLine.Substring(263, 1);
            this.SecCurrCode = ReadLine.Substring(264, 3);
            this.ClassOfSec = ReadLine.Substring(267, 7);
            this.TypeofBalance = ReadLine.Substring(274, 3);
            this.BalanceCode = ReadLine.Substring(277, 2);
            this.CorrespondentNr = ReadLine.Substring(285, 4);
            this.CircuitCode = ReadLine.Substring(289, 2);
            this.OpenBalanceDate = convertToDate(ReadLine.Substring(291, 8));
            this.OpenBalance = convertToDecimal(ReadLine.Substring(299, 1), ReadLine.Substring(300, 14), Int16.Parse(ReadLine.Substring(314, 2)));
            this.Movement = convertToDecimal(ReadLine.Substring(316, 1), ReadLine.Substring(317, 14), Int16.Parse(ReadLine.Substring(331, 2)));
            this.BankTxType = ReadLine.Substring(333, 2);
            this.OurImportedRef = ReadLine.Substring(335, 16);
            this.BankReference = ReadLine.Substring(351, 16);
            this.SettlementDate = convertToDate(ReadLine.Substring(367, 8));
            this.CPDebit = ReadLine.Substring(375, 1);
            this.CPAcctNr = ReadLine.Substring(376, 34);
            this.TypeOfCode = ReadLine.Substring(410, 4);
            this.CPCode = ReadLine.Substring(414, 11);
            this.CPDetails01 = ReadLine.Substring(425, 35);
            this.CPDetails02 = ReadLine.Substring(460, 35);
            this.CPDetails03 = ReadLine.Substring(495, 35);
            this.SecPriceCurr = ReadLine.Substring(530, 3);
            this.SecPrice = convertToDecimal("+", ReadLine.Substring(533, 11), 5);
            this.AmountCurrCode = ReadLine.Substring(567, 3);
            this.Amount = convertToDecimal(ReadLine.Substring(566, 1), ReadLine.Substring(570, 15), 2);
            this.CLoseBalance = convertToDecimal(ReadLine.Substring(585, 1), ReadLine.Substring(586, 14), Int16.Parse(ReadLine.Substring(600, 2)));
            this.AdditionalInfo01 = ReadLine.Substring(602, 35);
            this.AdditionalInfo02 = ReadLine.Substring(637, 35);
            this.AdditionalInfo03 = ReadLine.Substring(672, 35);
            this.AdditionalInfo04 = ReadLine.Substring(707, 35);
            this.AdditionalInfo05 = ReadLine.Substring(742, 35);
            this.AdditionalInfo06 = ReadLine.Substring(777, 35);
            this.CounterParty = ReadLine.Substring(812, 35);
            this.TxDesc = ReadLine.Substring(847, 36);



        }



        public string BankMsgeNr
        {
            get { return bankMsgeNr; }
            set { bankMsgeNr = value; }
        }

        public string MsgeRefersTo
        {
            get { return msgeRefersTo; }
            set { msgeRefersTo = value; }
        }

        public string PageNr
        {
            get { return pageNr; }
            set { pageNr = value; }
        }

        public Int16 FollowUpInd
        {
            get { return followUpInd; }
            set { followUpInd = value; }
        }

        public string BankAcctID
        {
            get { return bankAcctID; }
            set { bankAcctID = value; }
        }

        public string BankDetails01
        {
            get { return bankDetails01; }
            set { bankDetails01 = value; }
        }

        public string BankDetails02
        {
            get { return bankDetails02; }
            set { bankDetails02 = value; }
        }

        public string BankDetails03
        {
            get { return bankDetails03; }
            set { bankDetails03 = value; }
        }

        public DateTime BeginOfPeriod
        {
            get { return beginOfPeriod; }
            set { beginOfPeriod = value; }
        }
        public DateTime EndOfPeriod
        {
            get { return endOfPeriod; }
            set { endOfPeriod = value; }
        }

        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        public string BankSecCodeType
        {
            get { return bankSecCodeType; }
            set { bankSecCodeType = value; }
        }

        public string SecuritiesCode
        {
            get { return securitiesCode; }
            set { securitiesCode = value; }
        }

        public string SecDesc
        {
            get { return secDesc; }
            set { secDesc = value; }
        }

        public string BankSecType
        {
            get { return bankSecType; }
            set { bankSecType = value; }
        }

        public decimal InterestRate
        {
            get { return interestRate; }
            set { interestRate = value; }
        }

        public string StockExchCountryCode
        {
            get { return stockExchCountryCode; }
            set { stockExchCountryCode = value; }
        }

        public string Quotation
        {
            get { return quotation; }
            set { quotation = value; }
        }

        public string SecCurrCode
        {
            get { return secCurrCode; }
            set { secCurrCode = value; }
        }

        public string ClassOfSec
        {
            get { return classOfSec; }
            set { classOfSec = value; }
        }

        public string TypeofBalance
        {
            get { return typeofBalance; }
            set { typeofBalance = value; }
        }

        public string BalanceCode
        {
            get { return balanceCode; }
            set { balanceCode = value; }
        }

        public string CorrespondentNr
        {
            get { return correspondentNr; }
            set { correspondentNr = value; }
        }

        public string CircuitCode
        {
            get { return circuitCode; }
            set { circuitCode = value; }
        }

        public DateTime OpenBalanceDate
        {
            get { return openBalanceDate; }
            set { openBalanceDate = value; }
        }

        public decimal OpenBalance
        {
            get { return openBalance; }
            set { openBalance = value; }
        }

        public decimal Movement
        {
            get { return movement; }
            set { movement = value; }
        }

        public string BankTxType
        {
            get { return bankTxType; }
            set { bankTxType = value; }
        }

        public string OurImportedRef
        {
            get { return ourImportedRef; }
            set { ourImportedRef = value; }
        }

        public string BankReference
        {
            get { return bankReference; }
            set { bankReference = value; }
        }

        public DateTime SettlementDate
        {
            get { return settlementDate; }
            set { settlementDate = value; }
        }

        public string CPDebit
        {
            get { return cPDebit; }
            set { cPDebit = value; }
        }

        public string CPAcctNr
        {
            get { return cPAcctNr; }
            set { cPAcctNr = value; }
        }

        public string TypeOfCode
        {
            get { return typeOfCode; }
            set { typeOfCode = value; }
        }

        public string CPCode
        {
            get { return cPCode; }
            set { cPCode = value; }
        }

        public string CPDetails01
        {
            get { return cPDetails01; }
            set { cPDetails01 = value; }
        }

        public string CPDetails02
        {
            get { return cPDetails02; }
            set { cPDetails02 = value; }
        }

        public string CPDetails03
        {
            get { return cPDetails03; }
            set { cPDetails03 = value; }
        }

        public string SecPriceCurr
        {
            get { return secPriceCurr; }
            set { secPriceCurr = value; }
        }

        public decimal SecPrice
        {
            get { return secPrice; }
            set { secPrice = value; }
        }

        public string AmountCurrCode
        {
            get { return amountCurrCode; }
            set { amountCurrCode = value; }
        }

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public decimal CLoseBalance
        {
            get { return cLoseBalance; }
            set { cLoseBalance = value; }
        }

        public string AdditionalInfo01
        {
            get { return additionalInfo01; }
            set { additionalInfo01 = value; }
        }

        public string AdditionalInfo02
        {
            get { return additionalInfo02; }
            set { additionalInfo02 = value; }
        }
        public string AdditionalInfo03
        {
            get { return additionalInfo03; }
            set { additionalInfo03 = value; }
        }

        public string AdditionalInfo04
        {
            get { return additionalInfo04; }
            set { additionalInfo04 = value; }
        }

        public string AdditionalInfo05
        {
            get { return additionalInfo05; }
            set { additionalInfo05 = value; }
        }

        public string AdditionalInfo06
        {
            get { return additionalInfo06; }
            set { additionalInfo06 = value; }
        }

        public string CounterParty
        {
            get { return counterParty; }
            set { counterParty = value; }
        }

        public string TxDesc
        {
            get { return txDesc; }
            set { txDesc = value; }
        }

        private DateTime convertToDate(string tryDate)
        {
            DateTime test;
            DateTime.TryParse(tryDate.Substring(4, 2) + "/" + tryDate.Substring(6, 2) + "/" + tryDate.Substring(0, 4), new CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out test);
            return test > nullDate ? test : nullDate;
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

        #region Private Parts

        private string bankMsgeNr;
        private string msgeRefersTo;
        private string pageNr;
        private Int16 followUpInd;
        private string bankAcctID;
        private string bankDetails01;
        private string bankDetails02;
        private string bankDetails03;
        private DateTime beginOfPeriod = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private DateTime endOfPeriod = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private DateTime creationDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private string bankSecCodeType;
        private string securitiesCode;
        private string secDesc;
        private string bankSecType;
        private decimal interestRate;
        private string stockExchCountryCode;
        private string quotation;
        private string secCurrCode;
        private string classOfSec;
        private string typeofBalance;
        private string balanceCode;
        private string correspondentNr;
        private string circuitCode;
        private DateTime openBalanceDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private decimal openBalance;
        private decimal movement;
        private string bankTxType;
        private string ourImportedRef;
        private string bankReference;
        private DateTime settlementDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private string cPDebit;
        private string cPAcctNr;
        private string typeOfCode;
        private string cPCode;
        private string cPDetails01;
        private string cPDetails02;
        private string cPDetails03;
        private string secPriceCurr;
        private decimal secPrice;
        private string amountCurrCode;
        private decimal amount;
        private decimal cLoseBalance;
        private string additionalInfo01;
        private string additionalInfo02;
        private string additionalInfo03;
        private string additionalInfo04;
        private string additionalInfo05;
        private string additionalInfo06;
        private string counterParty;
        private string txDesc;



        #endregion





    }
}

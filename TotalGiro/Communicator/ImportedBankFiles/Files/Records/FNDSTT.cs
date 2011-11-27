using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records
{
    public class FNDSTT : ImportedRecord
    {
        protected FNDSTT() { }

        public FNDSTT(string ReadLine)
        {
            this.BankSwiftRefNr = ReadLine.Substring(5, 16);
            this.BankAcctID = ReadLine.Substring(21, 10);
            this.BankBalanceDate = convertToDate(ReadLine.Substring(31, 6));
            this.CreationDate = convertToDate(ReadLine.Substring(37, 6));
            this.BankSecType = ReadLine.Substring(43, 7);
            this.NumberOfShares = convertToDecimal(ReadLine.Substring(278, 1), ReadLine.Substring(50, 15), 2);
            this.IsinCode = ReadLine.Substring(65, 12);
            this.VvdeNr = ReadLine.Substring(77, 6);
            this.BankSecurityName = ReadLine.Substring(83, 20);
            this.SecCurrCode = ReadLine.Substring(104, 3);
            this.LastMovementDate = convertToDate(ReadLine.Substring(165, 6));
            this.NameOfCustodian = ReadLine.Substring(212, 40);
            this.TypeOfInvestment = ReadLine.Substring(252, 1);
            this.StatementLineNr = Int16.Parse(ReadLine.Substring(253, 5));
            this.AbbreviatedNameOfCust = ReadLine.Substring(258, 20);
            this.SubCustodianCode = ReadLine.Substring(279, 4);
        }


        public string BankSwiftRefNr
        {
            get { return bankSwiftRefNr; }
            set { bankSwiftRefNr = value; }
        }

        public string BankAcctID
        {
            get { return bankAcctID; }
            set { bankAcctID = value; }
        }

        public DateTime BankBalanceDate
        {
            get { return bankBalanceDate; }
            set { bankBalanceDate = value; }
        }


        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }


        public string BankSecType
        {
            get { return bankSecType; }
            set { bankSecType = value; }
        }

        public decimal NumberOfShares
        {
            get { return numberOfShares; }
            set { numberOfShares = value; }
        }

        public string IsinCode
        {
            get { return isinCode; }
            set { isinCode = value; }
        }

        public string VvdeNr
        {
            get { return vvdeNr; }
            set { vvdeNr = value; }
        }

        public string BankSecurityName
        {
            get { return bankSecurityName; }
            set { bankSecurityName = value; }
        }

        public string SecCurrCode
        {
            get { return secCurrCode; }
            set { secCurrCode = value; }
        }

        public DateTime LastMovementDate
        {
            get { return lastMovementDate; }
            set { lastMovementDate = value; }
        }

        public string NameOfCustodian
        {
            get { return nameOfCustodian; }
            set { nameOfCustodian = value; }
        }

        public string TypeOfInvestment
        {
            get { return typeOfInvestment; }
            set { typeOfInvestment = value; }
        }

        public Int16 StatementLineNr
        {
            get { return statementLineNr; }
            set { statementLineNr = value; }
        }

        public string AbbreviatedNameOfCust
        {
            get { return abbreviatedNameOfCust; }
            set { abbreviatedNameOfCust = value; }
        }

        public string SubCustodianCode
        {
            get { return subCustodianCode; }
            set { subCustodianCode = value; }
        }

        public string BicCode
        {
            get { return bicCode; }
            set { bicCode = value; }
        }

        public string PlaceOfSafekeeping
        {
            get { return placeOfSafekeeping; }
            set { placeOfSafekeeping = value; }
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
            return test > nullDate ? test : nullDate;
        }

        #region Private Parts



        private string bankSwiftRefNr;
        private string bankAcctID;
        private DateTime bankBalanceDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private DateTime creationDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private string bankSecType;
        private decimal numberOfShares;
        private string isinCode;
        private string vvdeNr;
        private string bankSecurityName;
        private string secCurrCode;
        private DateTime lastMovementDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private string nameOfCustodian;
        private string typeOfInvestment;
        private Int16 statementLineNr;
        private string abbreviatedNameOfCust;
        private string subCustodianCode;
        private string bicCode;
        private string placeOfSafekeeping;


        #endregion
    }
}

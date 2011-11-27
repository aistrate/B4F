using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records
{
    public class FNDSTXRecord : ImportedRecord
    {
        protected FNDSTXRecord() { }

        public FNDSTXRecord(string ReadLine)
        {
            this.BankSwiftRefNr = ReadLine.Substring(0, 16);
            this.BankAcctID = ReadLine.Substring(16, 10);
            this.BankBalanceDate = convertToDate(ReadLine.Substring(26, 8));
            this.CreationDate = convertToDate(ReadLine.Substring(34, 8));
            this.BankSecType = ReadLine.Substring(42, 7);

            this.NumberOfShares = convertToDecimal("1", ReadLine.Substring(49, 21), 6);
            this.IsinCode = ReadLine.Substring(70, 12);
            this.VvdeNr = ReadLine.Substring(82, 6);
            this.BankSecurityName = ReadLine.Substring(88, 20);
            this.Quotation = ReadLine.Substring(108, 1);

            this.SecCurrCode = ReadLine.Substring(109, 3);
            this.SecuritiesPrice = convertToDecimal("1", ReadLine.Substring(112, 11), 5);
            this.ForexRate = convertToDecimal("1", ReadLine.Substring(123, 9), 6);
            this.EffectiveValueCurrencyCode = ReadLine.Substring(132, 3);
            this.EffectiveValue1 = ReadLine.Substring(135, 15);

            this.BalanceSpecificationCode = ReadLine.Substring(150, 3);
            this.PriceCreationDate = convertToDate(ReadLine.Substring(153, 8));
            this.InterestRate = convertToDecimal("1", ReadLine.Substring(161, 7), 5);
            this.CancellationIndication = ReadLine.Substring(168, 1);
            this.WithoutValueIndication = ReadLine.Substring(169, 1);

            this.LastMovementDate = convertToDate(ReadLine.Substring(170, 8));
            this.ForeignExchangeRateFactor = convertToInt(ReadLine.Substring(178, 5));
            this.PaymentPercentage = ReadLine.Substring(183, 3);
            this.PaidUpAmount = convertToDecimal("1", ReadLine.Substring(186, 15), 2);
            this.DistributionPercentage = ReadLine.Substring(201, 3);
            this.AmountOfDistribution = convertToDecimal("1", ReadLine.Substring(204, 15), 2);

            this.NameOfCustodian = ReadLine.Substring(219, 40);
            this.TypeOfInvestment = ReadLine.Substring(259, 1);
            this.AbbreviatedNameOfCustodian = ReadLine.Substring(260, 20);
            this.PositiveNegativeBalance = ReadLine.Substring(280, 1);
            this.SubCustodian = ReadLine.Substring(281, 4);

            this.ForeignExchangeRateEffectiveValue = convertToDecimal("1", ReadLine.Substring(285, 9), 6);
            this.ForeignExchangeRateFactorEffectiveValue = ReadLine.Substring(294, 5);
            this.AccruedInterest = ReadLine.Substring(299, 15);
            this.CouponDueDate = convertToDate(ReadLine.Substring(314, 8));
            this.NumberOfInterestDays = convertToInt(ReadLine.Substring(322, 3));

            this.UsanceDay = ReadLine.Substring(325, 3);
            this.UsanceYear = ReadLine.Substring(328, 3);
            this.FebCode = ReadLine.Substring(331, 7);
            this.AbbreviatedNameOfCountry = ReadLine.Substring(338, 20);
            this.TotalEffectiveValue = ReadLine.Substring(358, 17);

            this.IndicationDebitTotal = ReadLine.Substring(375, 1);
            this.BicCode = ReadLine.Substring(376, 11);
            this.PlaceOfSafekeeping = ReadLine.Substring(387, 4);

        }

        public string BankSwiftRefNr { get; set; }
        public string BankAcctID { get; set; }
        public DateTime BankBalanceDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string BankSecType { get; set; }

        public decimal NumberOfShares { get; set; }
        public string IsinCode { get; set; }
        public string VvdeNr { get; set; }
        public string BankSecurityName { get; set; }
        public string Quotation { get; set; }

        public string SecCurrCode { get; set; }
        public decimal SecuritiesPrice { get; set; }
        public decimal ForexRate { get; set; }
        public string EffectiveValueCurrencyCode { get; set; }
        public string EffectiveValue1 { get; set; }

        public string BalanceSpecificationCode { get; set; }
        public DateTime PriceCreationDate { get; set; }
        public decimal InterestRate { get; set; }
        public string CancellationIndication { get; set; }
        public string WithoutValueIndication { get; set; }

        public DateTime LastMovementDate { get; set; }
        public int ForeignExchangeRateFactor { get; set; }
        public string PaymentPercentage { get; set; }
        public decimal PaidUpAmount { get; set; }
        public string DistributionPercentage { get; set; }
        public decimal AmountOfDistribution { get; set; }

        public string NameOfCustodian { get; set; }
        public string TypeOfInvestment { get; set; }
        public string AbbreviatedNameOfCustodian { get; set; }
        public string PositiveNegativeBalance { get; set; }
        public string SubCustodian { get; set; }

        public decimal ForeignExchangeRateEffectiveValue { get; set; }
        public string ForeignExchangeRateFactorEffectiveValue { get; set; }
        public string AccruedInterest { get; set; }
        public DateTime CouponDueDate { get; set; }
        public int NumberOfInterestDays { get; set; }

        public string UsanceDay { get; set; }
        public string UsanceYear { get; set; }
        public string FebCode { get; set; }
        public string AbbreviatedNameOfCountry { get; set; }
        public string TotalEffectiveValue { get; set; }

        public string IndicationDebitTotal { get; set; }
        public string BicCode { get; set; }
        public string PlaceOfSafekeeping { get; set; }
        public int Key { get; set; }


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
            DateTime.TryParseExact(tryDate, "yyyyMMdd", new CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out test);
            return test > nullDate ? test : nullDate;
        }

        private int convertToInt(string tryInt)
        {
            int test;
            if (int.TryParse(tryInt, out test))
                return test;
            else
                return 0;

        }
    }
}

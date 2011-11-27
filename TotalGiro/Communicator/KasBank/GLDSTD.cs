using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.BackOffice.Orders;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Communicator.KasBank
{
    public class GLDSTD : IGLDSTD
    {
        public GLDSTD() { }
        public GLDSTD(IMoneyTransferOrder newOrder)
        {
            this.Reference = newOrder.Reference;
            this.CurrencyCode = newOrder.Amount.UnderlyingShortName;
            this.NarDebet1 = newOrder.NarDebet1;
            this.NarDebet2 = newOrder.NarDebet2;
            this.NarDebet3 = newOrder.NarDebet3;
            this.NarDebet4 = newOrder.NarDebet4;
            this.DebetAcctNr = newOrder.TransferorJournal.BankAccountNumber;
            this.NarBenef1 = Util.ConvertToAscii(newOrder.NarBenef1);
            this.NarBenef2 = Util.ConvertToAscii(newOrder.NarBenef2);
            this.NarBenef3 = Util.ConvertToAscii(newOrder.NarBenef3);
            this.NarBenef4 = Util.ConvertToAscii(newOrder.NarBenef4);
            this.SwiftCorrespondentBank = (newOrder.SwiftAddress != null ? newOrder.SwiftAddress : "");
            this.BenefBankAcctNr = newOrder.BenefBankAcctNr;
            this.GroundForPayment1 = newOrder.TransferDescription1;
            this.GroundForPayment2 = newOrder.TransferDescription2;
            this.GroundForPayment3 = Util.ConvertToAscii(newOrder.TransferDescription3);
            this.GroundForPayment4 = newOrder.TransferDescription4;

            switch (newOrder.CostIndication)
            {
                case IndicationOfCosts.Beneficiary:
                    this.IndicationOfCost = "B";
                    break;
                case IndicationOfCosts.Ours:
                    this.IndicationOfCost = "O";
                    break;
                case IndicationOfCosts.Shared:
                    this.IndicationOfCost = "S";
                    break;
                default:
                    this.IndicationOfCost = "O";
                    break;
            }

            this.Amount = Decimal.Multiply(newOrder.Amount.Quantity, 100m).ToString("###").PadLeft(17, '0');
            this.IndicationOfNonRes = "0";
            this.NatureOfCP = "0";
            this.ProcessDate = newOrder.ProcessDate.ToString("yyyyMMdd");
            this.CircuitCode = "B";
            this.OptionsContract = "N";
            

            //add the Money Order record to the GLDSTD
            OriginalMoneyOrder = newOrder;
            newOrder.GLDSTDRecord = this;
            newOrder.SetStatus(MoneyTransferOrderStati.FileCreated);

        }

        public virtual int Key { get; set; }
        public virtual IMoneyTransferOrder OriginalMoneyOrder { get; set; }
        public virtual IGLDSTDFile ParentFile { get; set; }
        public virtual DateTime CreationDate
        { get { return creationDate; } }
        private DateTime creationDate = DateTime.Now;

        /// <summary>
        /// Position 1 -> 1
        /// </summary>
        public virtual string PriorityCode
        {
            get { return priorityCode.PadLeft(1, '0'); }
            set { priorityCode = value; }
        }

        /// <summary>
        /// Position 2 -> 17
        /// </summary>
        public virtual string Reference
        {
            get { return reference.PadRight(16, ' '); }
            set { reference = value; }
        }

        /// <summary>
        /// Position 18 -> 41
        /// </summary>
        public virtual string Filler1
        {
            get { return filler1.PadRight(24, ' '); }
            set { filler1 = value; }
        }

        /// <summary>
        /// Position 42 -> 44
        /// </summary>
        public virtual string CurrencyCode
        {
            get { return currencyCode.PadRight(3, ' '); }
            set { currencyCode = value; }
        }

        /// <summary>
        /// Position 45 -> 66
        /// </summary>
        public virtual string Filler2
        {
            get { return filler2.PadRight(22, ' '); }
            set { filler2 = value; }
        }

        /// <summary>
        /// Position 67 -> 102
        /// </summary>
        public virtual string NarDebet1
        {
            get { return narDebet1.PadRight(35, ' '); }
            set { narDebet1 = value; }
        }

        /// <summary>
        /// Position 103 -> 137
        /// </summary>
        public virtual string NarDebet2
        {
            get { return narDebet2.PadRight(35, ' '); }
            set { narDebet2 = value; }
        }

        /// <summary>
        /// Position 138 -> 172
        /// </summary>
        public virtual string NarDebet3
        {
            get { return narDebet3.PadRight(35, ' '); }
            set { narDebet3 = value; }
        }

        /// <summary>
        /// Position 173 -> 206
        /// </summary>
        public virtual string NarDebet4
        {
            get { return narDebet4.PadRight(35, ' '); }
            set { narDebet4 = value; }
        }

        /// <summary>
        /// Position 207 -> 241
        /// </summary>
        public virtual string DebetAcctNr
        {
            get { return debetAcctNr.PadRight(35, ' '); }
            set { debetAcctNr = value; }
        }

        /// <summary>
        /// Position 242 -> 427
        /// </summary>
        public virtual string Filler3
        {
            get { return filler3.PadRight(186, ' '); }
            set { filler3 = value; }
        }

        /// <summary>
        /// Position 428 -> 462
        /// </summary>
        public virtual string NarCorrespondentBank1
        {
            get { return narCorrespondentBank1.PadRight(35, ' '); }
            set { narCorrespondentBank1 = value; }
        }

        /// <summary>
        /// Position 463 -> 497
        /// </summary>
        public virtual string NarCorrespondentBank2
        {
            get { return narCorrespondentBank2.PadRight(35, ' '); }
            set { narCorrespondentBank2 = value; }
        }

        /// <summary>
        /// Position 498 -> 532
        /// </summary>
        public virtual string NarCorrespondentBank3
        {
            get { return narCorrespondentBank3.PadRight(35, ' '); }
            set { narCorrespondentBank3 = value; }
        }

        /// <summary>
        /// Position 533 -> 567
        /// </summary>
        public virtual string NarCorrespondentBank4
        {
            get { return narCorrespondentBank4.PadRight(35, ' '); }
            set { narCorrespondentBank4 = value; }
        }

        /// <summary>
        /// Position 568 -> 578
        /// </summary>
        public virtual string SwiftCorrespondentBank
        {
            get { return swiftCorrespondentBank.PadRight(11, ' '); }
            set { swiftCorrespondentBank = value; }
        }

        /// <summary>
        /// Position 579 -> 613
        /// </summary>
        public virtual string Filler4
        {
            get { return filler4.PadRight(35, ' '); }
            set { filler4 = value; }
        }

        /// <summary>
        /// Position 614 -> 648
        /// </summary>
        public virtual string NarBenefBank1
        {
            get { return narBenefBank1.PadRight(35, ' '); }
            set { narBenefBank1 = value; }
        }

        /// <summary>
        /// Position 649 -> 683
        /// </summary>
        public virtual string NarBenefBank2
        {
            get { return narBenefBank2.PadRight(35, ' '); }
            set { narBenefBank2 = value; }
        }

        /// <summary>
        /// Position 684 -> 718
        /// </summary>
        public virtual string NarBenefBank3
        {
            get { return narBenefBank3.PadRight(35, ' '); }
            set { narBenefBank3 = value; }
        }

        /// <summary>
        /// Position 719 -> 753
        /// </summary>
        public virtual string NarBenefBank4
        {
            get { return narBenefBank4.PadRight(35, ' '); }
            set { narBenefBank4 = value; }
        }

        /// <summary>
        /// Position 754 -> 764
        /// </summary>
        public virtual string SwiftBenefBank
        {
            get { return swiftBenefBank.PadRight(11, ' '); }
            set { swiftBenefBank = value; }
        }

        /// <summary>
        /// Position 765 -> 799
        /// </summary>
        public virtual string BankBankAcctNr
        {
            get { return bankBankAcctNr.PadRight(35, ' '); }
            set { bankBankAcctNr = value; }
        }

        /// <summary>
        /// Position 800 -> 834
        /// </summary>
        public virtual string NarBenef1
        {
            get { return narBenef1.PadRight(35, ' '); }
            set { narBenef1 = value; }
        }

        /// <summary>
        /// Position 835 -> 869
        /// </summary>
        public virtual string NarBenef2
        {
            get { return narBenef2.PadRight(35, ' '); }
            set { narBenef2 = value; }
        }

        /// <summary>
        /// Position 870 -> 904
        /// </summary>
        public virtual string NarBenef3
        {
            get { return narBenef3.PadRight(35, ' '); }
            set { narBenef3 = value; }
        }

        /// <summary>
        /// Position 905 -> 939
        /// </summary>
        public virtual string NarBenef4
        {
            get { return narBenef4.PadRight(35, ' '); }
            set { narBenef4 = value; }
        }

        /// <summary>
        /// Position 940 -> 974
        /// </summary>
        public virtual string BenefBankAcctNr
        {
            get { return benefBankAcctNr.PadRight(35, ' '); }
            set { benefBankAcctNr = value; }
        }

        /// <summary>
        /// Position 975 -> 1009
        /// </summary>
        public virtual string GroundForPayment1
        {
            get { return groundForPayment1.PadRight(35, ' '); }
            set { groundForPayment1 = value; }
        }

        /// <summary>
        /// Position 1010 -> 1044
        /// </summary>
        public virtual string GroundForPayment2
        {
            get { return groundForPayment2.PadRight(35, ' '); }
            set { groundForPayment2 = value; }
        }

        /// <summary>
        /// Position 1045 -> 1079
        /// </summary>
        public virtual string GroundForPayment3
        {
            get { return groundForPayment3.PadRight(35, ' '); }
            set { groundForPayment3 = value; }
        }

        /// <summary>
        /// Position 1080 -> 1114
        /// </summary>
        public virtual string GroundForPayment4
        {
            get { return groundForPayment4.PadRight(35, ' '); }
            set { groundForPayment4 = value; }
        }


        /// <summary>
        /// Position 1115 -> 1115
        /// </summary>
        public virtual string IndicationOfCost
        {
            get { return indicationOfCosts.PadRight(1, ' '); }
            set { indicationOfCosts = value; }
        }

        /// <summary>
        /// Position 1116 -> 1325
        /// </summary>
        public virtual string Filler5
        {
            get { return filler5.PadRight(210, ' '); }
            set { filler5 = value; }
        }

        /// <summary>
        /// Position 1326 -> 1342
        /// </summary>
        public virtual string Amount
        {
            get { return amount.PadLeft(17, '0'); }
            set { amount = value; }
        }

        /// <summary>
        /// Position 1343 -> 1343
        /// </summary>
        public virtual string IndicationOfNonRes
        {
            get { return indicationOfNonRes.PadLeft(1, '0'); }
            set { indicationOfNonRes = value; }
        }

        /// <summary>
        /// Position 1344 -> 1344
        /// </summary>
        public virtual string NatureOfCP
        {
            get { return natureOfCP.PadLeft(1, '0'); }
            set { natureOfCP = value; }
        }

        public virtual string ProcessDate
        {
            get { return processDate.PadLeft(8, '0'); }
            set { processDate = value; }
        }


        /// <summary>
        /// Position 1353 -> 1353
        /// </summary>
        public virtual string Filler6
        {
            get { return filler6.PadRight(1, ' '); }
            set { filler6 = value; }
        }

        /// <summary>
        /// Position 1354 -> 1354
        /// </summary>
        public virtual string CircuitCode
        {
            get { return circuitCode.PadRight(1, 'B'); }
            set { circuitCode = value; }
        }

        /// <summary>
        /// Position 1355 -> 1358
        /// </summary>
        public virtual string Filler7
        {
            get { return filler7.PadRight(4, ' '); }
            set { filler7 = value; }
        }

        /// <summary>
        /// Position 1359 -> 1374
        /// </summary> 
        public virtual string TestKey
        {
            get { return testKey.PadRight(16, ' '); }
            set { testKey = value; }
        }

        /// <summary>
        /// Position 1375 -> 1379
        /// </summary> 
        public virtual string Filler8
        {
            get { return filler8.PadRight(5, ' '); }
            set { filler8 = value; }
        }

        /// <summary>
        /// Position 1380 -> 1380
        /// </summary> 
        public virtual string OptionsContract
        {
            get { return optionsContract.PadRight(1, 'N'); }
            set { optionsContract = value; }
        }

        /// <summary>
        /// Position 1381 -> 1460
        /// </summary> 
        public virtual string TextOnForex
        {
            get { return textOnForex.PadRight(80, ' '); }
            set { textOnForex = value; }
        }

        /// <summary>
        /// Position 1461 -> 1464
        /// </summary> 
        public virtual string CodeOnForex
        {
            get { return codeOnForex.PadLeft(4, '0'); }
            set { codeOnForex = value; }
        }

        /// <summary>
        /// Position 1465 -> 1466
        /// </summary> 
        public virtual string CountryCodeForex
        {
            get { return countryCodeForex.PadRight(2, ' '); }
            set { countryCodeForex = value; }
        }

        /// <summary>
        /// Position 1467 -> 2000
        /// </summary> 
        public virtual string Filler9
        {
            get { return filler9.PadRight(534, ' '); }
            set { filler9 = value; }
        }



        public override string ToString()
        {
            StringBuilder oneLine = new StringBuilder();
            oneLine = oneLine.Append(this.PriorityCode);
            oneLine = oneLine.Append(this.Reference);
            oneLine = oneLine.Append(this.Filler1);
            oneLine = oneLine.Append(this.CurrencyCode);
            oneLine = oneLine.Append(this.Filler2);
            oneLine = oneLine.Append(this.NarDebet1);
            oneLine = oneLine.Append(this.NarDebet2);
            oneLine = oneLine.Append(this.NarDebet3);
            oneLine = oneLine.Append(this.NarDebet4);
            oneLine = oneLine.Append(this.DebetAcctNr);
            oneLine = oneLine.Append(this.Filler3);
            oneLine = oneLine.Append(this.NarCorrespondentBank1);
            oneLine = oneLine.Append(this.NarCorrespondentBank2);
            oneLine = oneLine.Append(this.NarCorrespondentBank3);
            oneLine = oneLine.Append(this.NarCorrespondentBank4);
            oneLine = oneLine.Append(this.SwiftCorrespondentBank);
            oneLine = oneLine.Append(this.Filler4);
            oneLine = oneLine.Append(this.NarBenefBank1);
            oneLine = oneLine.Append(this.NarBenefBank2);
            oneLine = oneLine.Append(this.NarBenefBank3);
            oneLine = oneLine.Append(this.NarBenefBank4);
            oneLine = oneLine.Append(this.SwiftBenefBank);
            oneLine = oneLine.Append(this.BankBankAcctNr);
            oneLine = oneLine.Append(this.NarBenef1);
            oneLine = oneLine.Append(this.NarBenef2);
            oneLine = oneLine.Append(this.NarBenef3);
            oneLine = oneLine.Append(this.NarBenef4);
            oneLine = oneLine.Append(this.BenefBankAcctNr);
            oneLine = oneLine.Append(this.GroundForPayment1);
            oneLine = oneLine.Append(this.GroundForPayment2);
            oneLine = oneLine.Append(this.GroundForPayment3);
            oneLine = oneLine.Append(this.GroundForPayment4);
            oneLine = oneLine.Append(this.IndicationOfCost);
            oneLine = oneLine.Append(this.Filler5);
            oneLine = oneLine.Append(this.Amount);
            oneLine = oneLine.Append(this.IndicationOfNonRes);
            oneLine = oneLine.Append(this.NatureOfCP);
            oneLine = oneLine.Append(this.ProcessDate);
            oneLine = oneLine.Append(this.Filler6);
            oneLine = oneLine.Append(this.CircuitCode);
            oneLine = oneLine.Append(this.Filler7);
            oneLine = oneLine.Append(this.TestKey);
            oneLine = oneLine.Append(this.Filler8);
            oneLine = oneLine.Append(this.OptionsContract);
            oneLine = oneLine.Append(this.TextOnForex);
            oneLine = oneLine.Append(this.CodeOnForex);
            oneLine = oneLine.Append(this.CountryCodeForex);
            oneLine = oneLine.Append(this.Filler9);
            return oneLine.ToString().PadRight(2000, ' ');
        }




        private string priorityCode = "";
        private string reference = "";
        private string filler1 = "";
        private string currencyCode = "";
        private string filler2 = "";
        private string narDebet1 = "";
        private string narDebet2 = "";
        private string narDebet3 = "";
        private string narDebet4 = "";
        private string debetAcctNr = "";
        private string filler3 = "";
        private string narCorrespondentBank1 = "";
        private string narCorrespondentBank2 = "";
        private string narCorrespondentBank3 = "";
        private string narCorrespondentBank4 = "";
        private string swiftCorrespondentBank = "";
        private string filler4 = "";
        private string narBenefBank1 = "";
        private string narBenefBank2 = "";
        private string narBenefBank3 = "";
        private string narBenefBank4 = "";
        private string swiftBenefBank = "";
        private string bankBankAcctNr = "";
        private string narBenef1 = "";
        private string narBenef2 = "";
        private string narBenef3 = "";
        private string narBenef4 = "";
        private string benefBankAcctNr = "";
        private string groundForPayment1 = "";
        private string groundForPayment2 = "";
        private string groundForPayment3 = "";
        private string groundForPayment4 = "";
        private string indicationOfCosts = "";
        private string filler5 = "";
        private string amount = "";
        private string indicationOfNonRes = "";
        private string natureOfCP = "";
        private string processDate = "";
        private string filler6 = "";
        private string circuitCode = "";
        private string filler7 = "";
        private string testKey = "";
        private string filler8 = "";
        private string optionsContract = "";
        private string textOnForex = "";
        private string codeOnForex = "";
        private string countryCodeForex = "";
        private string filler9 = "";

    }
}

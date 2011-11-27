using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Communicator.BelastingDienst
{
    public class DividWepFile : IDividWepFile
    {
        public DividWepFile()
        {
            records = new DividWepRecordCollection(this);

        }

        public DividWepFile(int financialYear, string pathName) :this()
        {
            this.FinancialYear = financialYear;
            this.SluitRecordType = "9";
            this.CodeFinance = "870";
            this.CreationDate = DateTime.Now;
            this.Path = pathName;
            this.FileName   = createFileName(financialYear);
        }

        private string createFileName(int financialYear)
        {
            StringBuilder sb = new StringBuilder(this.Path);
            sb.Append(@"DIVIDWEP_");
            sb.Append(financialYear.ToString());
            sb.Append("_");
            sb.Append(DateTime.Now.ToString("yyyyMMdd_HHmm"));
            return sb.ToString();
        }


        public virtual bool CreateOutputFile()
        {
            this.FileName = createFileName(this.FinancialYear );
            StreamWriter writer = new StreamWriter(this.FileName, false);

            writer.Write(this.InstelRecord);


            foreach (DividWepRecord dwr in this.Records)
            {
                writer.Write(dwr.SingleRecord);
            }
            writer.Write(this.SluitRecord);

            writer.Close();
            return true;


        }

        public virtual bool CreateCloseRecord()
        {
            //int noOfRecords = 0;
            int totalWep = 0;
            int totalDividend = 0;
            int totalTax = 0;
            foreach (DividWepRecord dwr in this.Records)
            {
                totalWep += dwr.WepValue;
                totalDividend += dwr.DivrentebedragValue;
                totalTax += dwr.BedragbronbelastingValue;
                //noOfRecords++;
            }

            this.TotalWep = totalWep;
            this.TotalDividend = totalDividend;
            this.TotalTax = totalTax;
            this.NoOfRecords = records.Count;
            this.SluitRecord = this.SluitRecordType.PadLeft(1, '0')
                + this.NoOfRecords.ToString().PadLeft(6, '0')
                + PadValueFields(this.TotalWep, 20)
                + PadValueFields(this.TotalDividend, 20)
                + PadValueFields(this.TotalTax, 20)
                + this.CodeFinance.ToString().PadLeft(3, '0');
            return true;
        }


        public virtual int Key { get; set; }
        public virtual string FileName { get; set; }
        public virtual string Path { get; set; }
        public virtual int NoOfRecords { get; set; }
        public virtual int TotalDividend { get; set; }
        public virtual int TotalTax { get; set; }
        public virtual string CodeFinance { get; set; }
        public virtual int TotalWep { get; set; }
        public virtual string SluitRecordType { get; set; }
        public int FinancialYear { get; set; }

        public virtual string InstelRecord
        {
            get { return instelRecord.PadRight(256, ' '); }
            set { instelRecord = value; }
        }

        public virtual string SluitRecord
        {
            get { return sluitRecord.PadRight(256, ' '); }
            set { sluitRecord = value; }
        }

        private string PadValueFields(int fieldValue, Int16 TotalLength)
        {
            StringBuilder retunValue;
            bool IsPositive = (fieldValue >= 0);

            int tempfieldValue = Math.Abs(fieldValue);

            int lastPlace = tempfieldValue % 10;
            retunValue = new StringBuilder().Append((tempfieldValue.ToString().Substring(0, tempfieldValue.ToString().Length - 1)));
            retunValue.Append(SpecialChar(lastPlace, IsPositive));

            return retunValue.ToString().PadLeft(TotalLength, '0');
        }

        private string SpecialChar(int lastChar, bool IsPositive)
        {
            string returnValue = "{";

            switch (IsPositive)
            {
                case true:

                    switch (lastChar)
                    {
                        case 0:
                            returnValue = "{";
                            break;
                        case 1:
                            returnValue = "A";
                            break;
                        case 2:
                            returnValue = "B";
                            break;
                        case 3:
                            returnValue = "C";
                            break;
                        case 4:
                            returnValue = "D";
                            break;
                        case 5:
                            returnValue = "E";
                            break;
                        case 6:
                            returnValue = "F";
                            break;
                        case 7:
                            returnValue = "G";
                            break;
                        case 8:
                            returnValue = "H";
                            break;
                        case 9:
                            returnValue = "I";
                            break;
                        default:
                            break;
                    }

                    break;

                case false:
                    switch (lastChar)
                    {
                        case 0:
                            returnValue = "}";
                            break;
                        case 1:
                            returnValue = "J";
                            break;
                        case 2:
                            returnValue = "K";
                            break;
                        case 3:
                            returnValue = "L";
                            break;
                        case 4:
                            returnValue = "M";
                            break;
                        case 5:
                            returnValue = "N";
                            break;
                        case 6:
                            returnValue = "O";
                            break;
                        case 7:
                            returnValue = "P";
                            break;
                        case 8:
                            returnValue = "Q";
                            break;
                        case 9:
                            returnValue = "R";
                            break;
                        default:
                            break;
                    }
                    break;

            }

            return returnValue;
        }

        public virtual IDividWepRecordCollection Records
        {
            get
            {
                IDividWepRecordCollection rec = (IDividWepRecordCollection)records.AsList();
                if (rec.ParentFile == null) rec.ParentFile = this;
                return rec;
            }
        }

        public virtual DateTime CreationDate
        {
            get
            {
                return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue;
            }

            set
            {
                this.creationDate = value;
            }
        }

        private IDomainCollection<IDividWepRecord> records;
        private string instelRecord = "0DIVIDWEPST ISB EFFECTENGIRO   HERENGR 199-201       1016 BE  AMSTERDAM    60870";
        private string sluitRecord;
        private DateTime? creationDate;
    }


}

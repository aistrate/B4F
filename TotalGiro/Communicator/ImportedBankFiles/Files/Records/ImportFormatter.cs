using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records
{
    public class ImportFormatter : List<string>
    {
        public ImportFormatter(string newLine)
        {
            sortArray(newLine);
        }

        private void sortArray(string newline)
        {
            StringBuilder sb = new StringBuilder();
            bool ignoreFlag = false;

            for (int i = 0; i < newline.Length; i++)
            {
                switch (newline[i])
                {
                    case '"':
                        ignoreFlag = !ignoreFlag;
                        break;
                    case ',':
                        if (!ignoreFlag)
                        {
                            this.Add(sb.ToString());
                            sb.Remove(0, sb.Length);
                        }
                        else
                            sb.Append(newline[i]);
                        break;
                    default:
                        sb.Append(newline[i]);
                        break;
                }
            }
            this.Add(sb.ToString());

        }

        public DateTime AssignDateValue(int index)
        {
            DateTime returnValue = DateTime.MinValue;
            DateTime.TryParse(this[index], out returnValue);
            return returnValue;
        }

        public DateTime AssignDateValue(int index, CultureInfo culture, DateTimeStyles styles)
        {
            DateTime returnValue = DateTime.MinValue;
            DateTime.TryParse(this[index], culture, styles, out returnValue);
            return returnValue;
        }

        public Decimal AssignDecimalValue(int index)
        {
            Decimal returnValue = 0;
            Decimal.TryParse(this[index], out returnValue);
            return returnValue;
        }


        public bool AssignBoolValue(int index)
        {
            return ((this[index] != null) && (this[index] == "1"));
        }
    }
}

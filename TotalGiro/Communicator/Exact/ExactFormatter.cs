using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Communicator.Exact.Formatters;

namespace B4F.TotalGiro.Communicator.Exact
{
    internal class ExactFormatter
    {
        private const int NR_COLUMNS = 40;

        static ExactFormatter()
        {
            // start at index 1
            formatters[1] = new NumericFormatter(3);        // Ledger  |  Subledger
            formatters[2] = new AlphanumFormatter(1);       // Ledger  |  Subledger
            formatters[3] = new AlphanumFormatter(3);       // Ledger  |  Subledger
            formatters[4] = new AlphanumFormatter(3);       // Ledger  |  Subledger   (not used)
            formatters[5] = new AlphanumFormatter(2);       // Ledger  |  Subledger   (not used)
            formatters[6] = new AlphanumFormatter(8);       // Ledger  |  Subledger   (not used?)
            formatters[7] = new AlphanumFormatter(60);      // Ledger  |  Subledger
            formatters[8] = new DateFormatter();            // Ledger  |  Subledger
            formatters[9] = new AlphanumFormatter(9);       //   --    |  Subledger
            formatters[10] = new AlphanumFormatter(6);      // Ledger  |  Subledger
            formatters[11] = new AlphanumFormatter(6);      // Ledger  |  Subledger
            formatters[12] = new AlphanumFormatter(8);      //   --    |  Subledger
            formatters[13] = new NumericFormatter(8, 2);    // Ledger  |  Subledger
            formatters[14] = new AlphanumFormatter(1);      // Ledger  |     --
            formatters[15] = new AlphanumFormatter(3);      // Ledger  |  Subledger
            formatters[16] = new NumericFormatter(5, 6);    // Ledger  |  Subledger
            formatters[17] = new AlphanumFormatter(1);      // Ledger  |     --
            formatters[18] = new NumericFormatter(8, 2);    // Ledger  |     --
            formatters[19] = new DateFormatter();           // Ledger  |     --
            formatters[20] = new DateFormatter();           // Ledger  |     --
            formatters[21] = new AlphanumFormatter(3);      //   --    |  Subledger
            formatters[22] = new NumericFormatter(8, 2);    //   --    |  Subledger
            formatters[23] = new AlphanumFormatter(2);      // Ledger  |     --       (not used)
            formatters[24] = new AlphanumFormatter(20);     // Ledger  |     --
            formatters[25] = new AlphanumFormatter(1);      // Ledger  |     --
            formatters[26] = new NumericFormatter(8, 2);    // Ledger  |     --
            formatters[27] = new AlphanumFormatter(4);      //   --    |  Subledger
            formatters[28] = new AlphanumFormatter(4);      //   --    |  Subledger
            formatters[29] = new NumericFormatter(8, 2);    //   --    |  Subledger
            formatters[30] = new EmptyFormatter();          //   --    |     --
            formatters[31] = new EmptyFormatter();          //   --    |     --
            formatters[32] = new AlphanumFormatter(1);      // Ledger  |  Subledger
            // the rest (up to 40) is empty
        }

        public ExactFormatter(IExactFormatterSource dataSource)
        {
            this.dataSource = dataSource;
        }

        public string FormatLine()
        {
            ExactFieldCollection fields = dataSource.GetFields();
            StringBuilder line = new StringBuilder();

            for (int i = 1; i <= NumberOfColumns; i++)
            {
                try
                {
                    string fieldValue = "";

                    if (formatters[i] != null && fields[i] != null)
                        fieldValue = formatters[i].Format(fields[i]);

                    if (i > 1)
                        line.Append(',');

                    line.Append(fieldValue);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Format("Error formatting column {0}.", i), ex);
                }
            }

            return line.ToString();
        }

        public int NumberOfColumns { get { return NR_COLUMNS; } }

        private IExactFormatterSource dataSource;
        private static ObjectFormatter[] formatters = new ObjectFormatter[NR_COLUMNS + 1];
    }
}

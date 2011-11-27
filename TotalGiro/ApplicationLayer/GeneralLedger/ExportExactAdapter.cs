using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Communicator.Exact;
using System.Collections;
using System.Linq;
using System.IO;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal;
using System.Data;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class ExportExactAdapter
    {
        public static void ExportToExact(BatchExecutionResults results, DateTime dateUntil)
        {
            createExportEntries(dateUntil);
            IDalSession session = NHSessionFactory.CreateSession();            
            try
            {
                string exportFilePath = Utility.GetPathFromConfigFile("ExactExportFilePath");

                IList<ILedgerType> ledgerGroups = LedgerEntryMapper.GetLedgerEntryGroupings(session, dateUntil);
                foreach (ILedgerType grouping in ledgerGroups)
                {
                    //ILedgerType ledgerType = (ILedgerType)grouping;
                    exportGrouping(results, session, grouping, dateUntil, exportFilePath);
                }
            }
            finally
            {
                session.Close();
            }
        }

        private static void exportGrouping(BatchExecutionResults results, IDalSession session, ILedgerType ledgerType, DateTime dateUntil,
                                           string exportFilePath)
        {
            try
            {
                string fileName = string.Format("{0}_{1:yyyyMMdd}", ledgerType.Type, dateUntil);
                string fileExt = "csv";

                IList ledgerEntries = LedgerEntryMapper.GetLedgerEntries(session, ledgerType, dateUntil);
                int fileOrdinal = ExportedLedgerFileMapper.GetNextOrdinal(session, fileName);

                if ((ledgerEntries != null) && (ledgerEntries.Count > 0))
                {
                    ExportedLedgerFile exportedLedgerFile = new ExportedLedgerFile(fileName, fileExt, exportFilePath, fileOrdinal++);
                    FileStream fs = new FileStream(exportedLedgerFile.FullPathName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);

                    foreach (ILedgerEntry ledgerEntry in ledgerEntries)
                    {
                        string formattedEntry = formatLedgerEntry(results, ledgerEntry);

                        if (formattedEntry.Length > 0)
                        {
                            sw.Write(formattedEntry);

                            ledgerEntry.ExportedLedgerFile = exportedLedgerFile;
                            exportedLedgerFile.LedgerEntries.AddLedgerEntry(ledgerEntry);
                        }
                    }

                    sw.Close();
                    fs.Close();
                    ExportedLedgerFileMapper.Update(session, exportedLedgerFile);
                    results.MarkSuccess();
                }
            }
            catch (Exception ex)
            {
                results.MarkError(
                    new ApplicationException(string.Format("Error exporting for ledger type '{0}', currency '{1}' and transaction date {2:d}.",
                                                           ledgerType.Type, dateUntil), ex));
            }
        }

        private static string formatLedgerEntry(BatchExecutionResults results, ILedgerEntry ledgerEntry)
        {
            try
            {
                string formatted = "";

                formatted += ledgerEntry.FormatLine() + Environment.NewLine;
                foreach (var subledgerEntry in ledgerEntry.SubledgerEntries
                        .Where(x => ((x.LineNumber > 0) && (x.Amount != 0m)))
                        .OrderBy(y => y.LineNumber))
                {
                    formatted += subledgerEntry.FormatLine() + Environment.NewLine;
                }

                return formatted;
            }
            catch (Exception ex)
            {
                results.MarkError(
                    new ApplicationException(string.Format("Error exporting ledger entry {0} or one of its sub-ledger entries.",
                                                           ledgerEntry.Key), ex));
                return "";
            }
        }


        private static void createExportEntries(DateTime dateUntil)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IList<ExactEntryGrouping> entries = SubledgerEntryMapper.GetJournalEntryGroupsToExport(session, dateUntil);

                foreach (ExactEntryGrouping grouping in entries)
                {
                    CreateLedgerEntry(session, grouping, dateUntil);
                }
            }
            finally
            {
                session.Close();
            }
        }

        private static void CreateLedgerEntry(IDalSession session, ExactEntryGrouping grouping, DateTime dateUntil)
        {
            IExactJournal exactJournal = ExactJournalMapper.GetExactJournal(session, grouping.Key);
            ILedgerType ledgerType = exactJournal.LedgerType;
            string nextBookingNumber = LedgerEntryMapper.GetNextLedgerEntryNumber(session, exactJournal.JournalNumber);

            ILedgerEntry newEntry = new LedgerEntry(ledgerType, exactJournal.JournalNumber, nextBookingNumber, grouping.TransactionDate, 0m, "B", false);

            IList<IJournalEntryLine> groupOfLines;

            groupOfLines = SubledgerEntryMapper.GetJournalEntriesToExport(session, grouping);

            CreateSubLedgerEntries(session, newEntry, groupOfLines);

            LedgerEntryMapper.Update(session, newEntry);
        }

        private static void CreateSubLedgerEntries(IDalSession session, ILedgerEntry newEntry, IList<IJournalEntryLine> groupOfLines)
        {
            var summary = from s in groupOfLines                //.Where(n => n.GLAccount.ExactAccount != null)
                          group s by
                          new
                          {
                              account = s.GLAccount.ExactAccount,
                              currency = s.Balance.UnderlyingShortName
                          } into accountGroups
                          select new
                          {
                              exactAccount = accountGroups.Key.account,
                              currency = accountGroups.Key.currency,
                              records = accountGroups,
                              total = (from l in accountGroups select (l.Debit - l.Credit)).Sum()
                          };

            //bool bankFlag = newEntry.LedgerType.JournalType == JournalTypes.BankStatement;
            int lineNumer = 1;
            foreach (var d in summary.OrderBy(a => a.exactAccount).ThenBy(b => b.currency))
            {
                Decimal quantity =  d.total.Quantity;
                ISubledgerEntry newSubEntry = new SubledgerEntry("", d.exactAccount, quantity, d.currency, false, d.total.XRate);
                if (newSubEntry.Amount != 0m) newSubEntry.LineNumber = lineNumer++;
                foreach (var f in d.records)
                {
                    f.SubledgerEntry = newSubEntry;
                }
                newEntry.SubledgerEntries.AddSubLedgerEntry(newSubEntry);
            }
        }

        public static void DeleteExportedFile(int fileID)
        {

            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IExportedLedgerFile file = ExportedLedgerFileMapper.GetExportedFile(session, fileID);
                foreach (ILedgerEntry ledger in file.LedgerEntries)
                {
                    DeleteLedgerEntry(ledger);
                    ledger.ExportedLedgerFile = null;
                    session.Update(ledger);
                }
            }
            finally
            {
                session.Close();
            }
        }

        public static void DeleteLedgerEntry(ILedgerEntry ledger)
        {

            foreach (ISubledgerEntry subLedger in ledger.SubledgerEntries)
            {
                foreach (IJournalEntryLine line in subLedger.JournalEntryLines)
                {
                    line.SubledgerEntry = null;
                }
            }
        }

        public static DataSet GetExportedFiles()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return ExportedLedgerFileMapper.GetExportedLedgerFiles(session)
                    .Select(f => new
                    {
                        f.Key,
                        f.Name,
                        f.FullPathName,
                        f.CreationDate
                    }).ToDataSet();
            }
        }

        public static DataSet GetLedgerEntriesinFile(int fileID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return LedgerEntryMapper.GetLedgerEntriesinFile(session, fileID)
                    .Select(f => new
                    {
                        f.Key,
                        f.BookingNumber,
                        f.Journal,
                        f.LedgerType,
                        f.ValueDate
                    }).ToDataSet();
            }
        }
    }
}

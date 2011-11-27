using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal;
using System.Collections;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class MemorialBookingsAdapter
    {
        public static DataSet GetMemorialBookings(
            int journalId, DateTime transactionDateFrom, DateTime transactionDateTo, string journalEntryNumber, JournalEntryStati statuses)
        {
            //@"Key, JournalEntryNumber, Status, DisplayStatus, Journal.JournalNumber, TransactionDate, Description"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IJournal journal = JournalMapper.GetJournal(session, journalId);
                return JournalEntryMapper.GetJournalEntries<IMemorialBooking>(session, JournalTypes.Memorial, 
                    journal, transactionDateFrom, transactionDateTo, journalEntryNumber, statuses, true)
                    .Select(c => new
                    {
                        c.Key,
                        c.JournalEntryNumber,
                        c.Status,
                        c.DisplayStatus,
                        Journal_JournalNumber =
                            c.Journal.JournalNumber,
                        c.TransactionDate,
                        c.Description
                    })
                    .ToDataSet();
            }
        }

        public static int OpenMemorialBookingsCount(int journalId)
        {
            int count = 0;
            IDalSession session = NHSessionFactory.CreateSession();
            IJournal journal = JournalMapper.GetJournal(session, journalId);
            IList<IJournalEntry> bookings = JournalEntryMapper.GetJournalEntries(session, JournalTypes.Memorial, journal, JournalEntryStati.New | JournalEntryStati.Open);
            if (bookings != null)
                count = bookings.Count;
            return count;
        }

        public static object CreateMemorialBooking(int journalId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IJournal journal = JournalMapper.GetJournal(session, journalId);

                string nextJournalEntryNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, journal);

                IMemorialBooking newMemorialBooking = new MemorialBooking(journal, nextJournalEntryNumber);

                JournalEntryMapper.Insert(session, newMemorialBooking);

                return newMemorialBooking.Key;
            }
            finally
            {
                session.Close();
            }
            throw new Exception("The method or operation is not implemented.");
        }

        private static bool CreateManagementFeeBookingsforDate(IDalSession session, IList listofFees,
            IMemorialBooking feeBookings, IGLAccount clientTaxAccount, IGLAccount clientFeeAccount,
            IGLAccount clientFixedCostsAccount, IGLAccount taxAccount)
        {
            //foreach (IManagementFee imf in listofFees)
            //{
            //    foreach (MgtFeeBreakupLine line in imf.FeeDetails.BreakupLines)
            //    {
            //        if (line.Amount.IsNotZero)
            //        {
            //            IAccountTypeInternal account = line.Transaction.AccountA;
            //            IJournalEntryLine newLineClient = new JournalEntryLine();
            //            IJournalEntryLine newLineCP = new JournalEntryLine();
            //            IJournalEntryLine newLineClientTax = new JournalEntryLine();
            //            IJournalEntryLine newLineCPTax = new JournalEntryLine();

            //            switch (line.MgtFeeType.Key)
            //            {
            //                case FeeTypes.AdministrationCosts:
            //                case FeeTypes.FixedFee:
            //                    newLineClient.GLAccount = clientFixedCostsAccount;
            //                    newLineCP.GLAccount = account.AccountOwner.ManagementFeeFixedCostsAccount;
            //                    break;
            //                default:
            //                    newLineClient.GLAccount = clientFeeAccount;
            //                    newLineCP.GLAccount = account.AccountOwner.ManagementFeeIncomeAccount;
            //                    break;
            //            }

            //            newLineClientTax.GLAccount = clientTaxAccount;
            //            newLineCPTax.GLAccount = taxAccount;

            //            newLineClient.Balance = line.Amount;
            //            newLineCP.Balance = line.Amount.Negate();
            //            if ((line.Tax != null) && (line.Tax.IsNotZero))
            //            {
            //                newLineClientTax.Balance = line.Tax;
            //                newLineCPTax.Balance = line.Tax.Negate();
            //            }

            //            newLineClient.GiroAccount = account;
            //            newLineClientTax.GiroAccount = account;

            //            newLineClient.TotalGiroTrade = line.Transaction;
            //            newLineCP.TotalGiroTrade = line.Transaction;
            //            newLineClientTax.TotalGiroTrade = line.Transaction;
            //            newLineCPTax.TotalGiroTrade = line.Transaction;

            //            feeBookings.Lines.Add(newLineClient);
            //            feeBookings.Lines.Add(newLineCP);
            //            feeBookings.Lines.Add(newLineClientTax);
            //            feeBookings.Lines.Add(newLineCPTax);

            //        }
            //    }
            //}

            return true;
        }
    }
}

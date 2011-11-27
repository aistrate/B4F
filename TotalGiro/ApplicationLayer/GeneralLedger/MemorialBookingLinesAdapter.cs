using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal;
using System.Collections;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.ApplicationLayer.BackOffice;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class MemorialBookingLinesAdapter
    {
        public static DataSet GetMemorialBookingDetails(int journalEntryId)
        {
            // @"Key, JournalEntryNumber, Status, DisplayStatus, Journal.Key, Journal.FullDescription, Journal.ManagementCompany.Key, 
            //  Journal.Currency.Key, Journal.Currency.IsBase, ExchangeRate, TransactionDate, Description");
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return JournalEntryMapper.GetJournalEntries(session, journalEntryId)
                    .Cast<IMemorialBooking>()
                    .Select(c => new
                    {
                        c.Key,
                        c.JournalEntryNumber,
                        c.Status,
                        c.DisplayStatus,
                        Journal_Key = 
                            c.Journal.Key,
                        Journal_FullDescription =
                            c.Journal.FullDescription,
                        Journal_ManagementCompany_Key =
                            c.Journal.ManagementCompany.Key,
                        Journal_Currency_Key =
                            c.Journal.Currency.Key,
                        Journal_Currency_IsBase = 
                        c.Journal.Currency.IsBase,
                        c.ContainsForeignCashLines,
                        c.ExchangeRate, 
                        c.TransactionDate,
                        c.Description,
                        ShowManualAllowedGLAccountsOnly =
                            c.Journal.ShowManualAllowedGLAccountsOnly

                    })
                    .ToDataSet();
            }
        }

        private static IMemorialBooking getMemorialBooking(IDalSession session, int journalEntryId)
        {
            IMemorialBooking memorialBooking = (IMemorialBooking)JournalEntryMapper.GetJournalEntry(session, journalEntryId);
            if (memorialBooking == null)
                throw new ApplicationException(string.Format("Memorial Booking with ID '{0}' could not be found.", journalEntryId));
            return memorialBooking;
        }

        public static IList GetMemorialBookingEditView(int journalEntryId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ArrayList editViews = new ArrayList();

            try
            {
                IMemorialBooking memorialBooking = getMemorialBooking(session, journalEntryId);
                if (memorialBooking != null)
                {
                    if (memorialBooking.Status != JournalEntryStati.New)
                        throw new ApplicationException("Transaction Date cannot be edited because the Memorial Booking has been already booked.");

                    MemorialBookingEditView memorialBookingEditView = new MemorialBookingEditView(journalEntryId);
                    memorialBookingEditView.TransactionDate = memorialBooking.TransactionDate;
                    memorialBookingEditView.Description = memorialBooking.Description;
                    memorialBookingEditView.ExRate = memorialBooking.ExchangeRate;

                    editViews.Add(memorialBookingEditView);
                }
            }
            finally
            {
                session.Close();
            }

            return editViews;
        }

        public static void UpdateMemorialBooking(MemorialBookingEditView memorialBookingEditView)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IMemorialBooking memorialBooking = getMemorialBooking(session, memorialBookingEditView.JournalEntryId);

                if (memorialBooking.Status != JournalEntryStati.New)
                    throw new ApplicationException("Memorial Booking cannot be updated because it has been already booked.");

                memorialBooking.TransactionDate = memorialBookingEditView.TransactionDate;
                memorialBooking.Description = memorialBookingEditView.Description.Trim();
                
                JournalEntryMapper.Update(session, memorialBooking);
            }
            finally
            {
                session.Close();
            }
        }

        public static BookingResults BookMemorialBooking(int journalEntryId, bool forceIfUnchanged)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IMemorialBooking memorialBooking = getMemorialBooking(session, journalEntryId);

                if (memorialBooking.TransactionDate == DateTime.MinValue)
                    return BookingResults.TransactionDateNeeded;
                else
                {
                    doBook(session, memorialBooking);
                    return BookingResults.OK;
                }
            }
            finally
            {
                session.Close();
            }
        }

        private static void doBook(IDalSession session, IMemorialBooking memorialBooking)
        {
            if (memorialBooking.Status == JournalEntryStati.Booked)
                throw new ApplicationException("This Memorial Booking has been already booked.");

            if (memorialBooking.Balance.IsNotZero)
                throw new ApplicationException(
                    string.Format("Cannot book Memorial Booking because its Total Balance is not zero ({0:#,##0.00}).",
                                  memorialBooking.Balance));

            foreach (IJournalEntryLine line in memorialBooking.Lines)
                if (line.Status == JournalEntryLineStati.New)
                {
                    if (line.GLAccount == null)
                        throw new ApplicationException(string.Format("Memorial Booking Line number {0} does not have a GL Account.", line.LineNumber));

                    if (!line.IsStorno)
                    {
                        if (line.GLAccount.RequiresGiroAccount && line.GiroAccount == null)
                            throw new ApplicationException(
                                string.Format("Memorial Booking Line number {0} is required by its GL Account ('{1}') to have a non-empty Giro Account.",
                                              line.LineNumber, line.GLAccount.FullDescription));
                        else if (!line.GLAccount.RequiresGiroAccount && line.GiroAccount != null)
                            throw new ApplicationException(
                                string.Format("Memorial Booking Line number {0} is prohibited by its GL Account ('{1}') to have a Giro Account.",
                                              line.LineNumber, line.GLAccount.FullDescription));
                    }
                }

            JournalEntryLinesAdapter.BookJournalEntry(session, memorialBooking);
        }
    }
}

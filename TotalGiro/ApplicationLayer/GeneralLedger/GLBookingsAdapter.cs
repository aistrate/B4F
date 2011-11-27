using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class GLBookingsAdapter
    {
        public static DataSet GetBookingDetails(int bookingId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IGeneralOperationsBooking booking = GeneralOperationsBookingMapper.GetBooking(session, bookingId);
                List<IGeneralOperationsBooking> bookings = new List<IGeneralOperationsBooking>();
                bookings.Add(booking);
                return bookings
                .Select(c => new
                {
                    c.Key,
                    AccountName =
                        c.Account.DisplayNumberWithName,
                    BookDate =
                        c.BookDate,
                    ClassName =
                        c.GetType().Name,
                    AuditLogKey =
                        c.GeneralOpsJournalEntry.Key,
                    Description =
                        c.Description,
                    TotalAmount =
                        c.TotalAmount.DisplayString,
                    c.TaxPercentage,
                    c.IsStorno,
                    StornoBookingID =
                        c.StornoBooking != null ? c.StornoBooking.Key.ToString() : "",
                    c.CreationDate,
                    BookNotaID = 
                        c.BookNota != null ? c.BookNota.Key.ToString() : "",
                    JournalEntryID =
                        c.GeneralOpsJournalEntry.Key,
                    AuditLogClass =
                        c.GeneralOpsJournalEntry.JournalEntryType.ToString()
                })
                .ToDataSet();
            }
        }
    }
}

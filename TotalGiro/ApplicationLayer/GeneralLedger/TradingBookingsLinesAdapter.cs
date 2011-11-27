using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal;
using System.Collections;
using System.Data;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public class TradingBookingsLinesAdapter
    {
        public static DataSet GetJournalEntryLines(int journalEntryId)
        {
            // Debit.DisplayString and Credit.DisplayString need to be in the list because otherwise when Debit.ToString("...") is called,
            // a "Session already closed" error is thrown by NHibernate
            //    @"Key, LineNumber, Status, GLAccountId, GLAccount.FullDescription, BalanceCurrencyId, GiroAccount.ShortName,
            //      Debit, DebitDisplayString, DebitQuantity, Credit, CreditDisplayString, CreditQuantity, GiroAccount.Number, 
            //      Description, TxComponent.Key, TotalGiroOrderID, TotalGiroTXType");

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return getJournalEntry(session, journalEntryId).Lines
                            .Select(c => new
                            {
                                c.Key,
                                c.LineNumber,
                                c.Status,
                                GLAccountId =
                                    c.GLAccount.Key,
                                GLAccount_FullDescription =
                                    c.GLAccount.FullDescription,
                                BalanceCurrencyId =
                                    (c.Balance != null ? c.Balance.Underlying.Key : 0M),
                                GiroAccount_Key =
                                    (c.GiroAccount != null ? c.GiroAccount.Key : 0),
                                GiroAccount_ShortName =
                                    (c.GiroAccount != null ? c.GiroAccount.ShortName : ""),
                                GiroAccount_Number =
                                    (c.GiroAccount != null ? c.GiroAccount.Number : ""),
                                c.Debit, 
                                DebitDisplayString =
                                    c.Debit.DisplayString, 
                                DebitQuantity =
                                    c.Debit.Quantity, 
                                c.Credit, 
                                CreditDisplayString =
                                    c.Credit.DisplayString, 
                                CreditQuantity =
                                    c.Credit.Quantity,
                                c.ExchangeRate,
                                c.Description,
                                TxComponent_Key =
                                    (c.BookComponent != null ? c.BookComponent.Key : 0), 
                                TotalGiroOrderID =
                                    (c.BookComponent != null && 
                                    c.BookComponent.Parent.BookingComponentParentType == BookingComponentParentTypes.Transaction &&
                                    ((ITransactionComponent)c.BookComponent.Parent).ParentTransaction.TransactionType == TransactionTypes.Allocation  ? 
                                    ((ITransactionOrder)((ITransactionComponent)c.BookComponent.Parent).ParentTransaction).Order.Key : 0),
                                TotalGiroTXType =
                                    (c.BookComponent != null && c.BookComponent.Parent.BookingComponentParentType == BookingComponentParentTypes.Transaction ? ((ITransactionComponent)c.BookComponent.Parent).ParentTransaction.TradeType.Description : "Unknown")
                            })
                            .ToDataSet();
            }
        }

        private static IJournalEntry getJournalEntry(IDalSession session, int journalEntryId)
        {
            IJournalEntry journalEntry = JournalEntryMapper.GetJournalEntry(session, journalEntryId);
            if (journalEntry == null)
                throw new ApplicationException(string.Format("Journal Entry with ID '{0}' could not be found.", journalEntryId));
            return journalEntry;
        }

        public static DataSet GetTradingBookingsDetails(int journalEntryId)
        {
            //@"Key, JournalEntryNumber, Status, DisplayStatus, Journal.Key, Journal.FullDescription,  
            //  TransactionDate, TradedInstrument, TradeSizeDisplay, TradePrice.DisplayString, CounterParty");
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return JournalEntryMapper.GetJournalEntries(session, journalEntryId)
                        .Cast <ITradingJournalEntry>()
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
                            c.TransactionDate,
                            c.TradedInstrument,
                            c.TradeSizeDisplay,
                            TradePrice_DisplayString =
                                c.TradePrice.DisplayString,
                            c.CounterParty,
                            c.ExchangeRate,
                            Journal_Currency_IsBase = 
                                c.Journal.Currency.IsBase
                        })
                        .ToDataSet();
            }
        }

        public static DataSet GetLineSummary(int journalEntryId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                List<IJournalEntryLine> lines = getJournalEntry(session, journalEntryId).Lines.ToList();

                var summary = from s in lines
                              group new { Debit1 = s.Debit, Credit1 = s.Credit } by
                              new { account = s.GLAccount, giroacct = s.GiroAccount }
                                  into g
                                  let firstchoice = new
                                  {
                                      account = g.Key.account,
                                      giroacct = g.Key.giroacct,
                                      Debit = (from pair in g select pair.Debit1).Sum(),
                                      Credit = (from pair in g select pair.Credit1).Sum()
                                  }
                                  where (firstchoice.Credit - firstchoice.Debit).IsNotZero
                                  select firstchoice;

                //@"account.GLAccountNumber, giroacct.ShortName,  account.FullDescription, giroacct.Number, Debit.DisplayString, Credit.DisplayString");
                return summary
                    .Select(c => new
                    {
                        account_GLAccountNumber =
                            c.account.GLAccountNumber, 
                        giroacct_ShortName =
                            c.giroacct.ShortName,
                        account_FullDescription =
                            c.account.FullDescription, 
                        giroacct_Number =
                            c.giroacct.Number,
                        Debit_DisplayString =
                            c.Debit.DisplayString, 
                        Credit_DisplayString =
                            c.Credit.DisplayString
                    })
                    .ToDataSet();
            }
        }
    }
}

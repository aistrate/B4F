using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NHibernate.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.History;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.MIS.Positions;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions
{
    public static class InstrumentConversionAdapter
    {
        public static DataSet GetInstrumentConversions(string isin, 
            string instrumentName, int currencyNominalId, int secCategoryId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentHistoryMapper.GetInstrumentConversions(session, isin, instrumentName, (SecCategories)secCategoryId, currencyNominalId)
                    .Select(d => new
                    {
                        d.Key,
                        InstrumentName = d.Instrument.DisplayNameWithIsin,
                        NewInstrumentName = d.NewInstrument.DisplayNameWithIsin,
                        Ratio = string.Format("{0} : {1}", d.OldChildRatio.ToString("0.####"), d.NewParentRatio.ToString("0.####")),
                        d.Description,
                        d.ChangeDate,
                        d.ExecutionDate,
                        d.CreationDate
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetTradeableInstruments(int instrumentConversionID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<ITradeableInstrument> list = null;
                if (instrumentConversionID != 0)
                {
                    IInstrumentsHistoryConversion conversion = InstrumentHistoryMapper.GetInstrumentConversion(session, instrumentConversionID);
                    list = new List<ITradeableInstrument>();
                    list.Add((ITradeableInstrument)conversion.Instrument);
                }
                else
                    list = InstrumentMapper.GetTradeableInstruments(session);
                return list
                .Select(p => new
                {
                    Key = p.Key,
                    Description = p.DisplayNameWithIsin
                })
                .OrderBy(o => o.Description)
                .ToDataSet().AddEmptyFirstRow();
            }
        }

        public static DataSet GetParentInstruments(int convertedInstrumentId, int instrumentConversionID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<ITradeableInstrument> list = null;
                if (instrumentConversionID != 0)
                {
                    IInstrumentsHistoryConversion conversion = InstrumentHistoryMapper.GetInstrumentConversion(session, instrumentConversionID);
                    list = new List<ITradeableInstrument>();
                    list.Add((ITradeableInstrument)conversion.NewInstrument);
                }
                else
                    list = InstrumentMapper.GetTradeableInstruments(session);

                return list
                .Where(x => x.Key != convertedInstrumentId)
                .Select(p => new
                {
                    Key = p.Key,
                    Description = p.DisplayNameWithIsin
                })
                .OrderBy(o => o.Description)
                .ToDataSet().AddEmptyFirstRow();
            }
        }

        public static InstrumentConversionDetails GetInstrumentConversionDetails(int instrumentConversionId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IInstrumentsHistoryConversion conversion = InstrumentHistoryMapper.GetInstrumentConversion(session, instrumentConversionId);
                return new InstrumentConversionDetails(conversion);
            }
        }

        public static DataSet GetInstrumentConversionTxDetails(int instrumentConversionId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return (session.Session.Linq<InstrumentConversion>()
                        .Where(x => x.InstrumentTransformation.Key == instrumentConversionId))
                        .ToList()
                        .Select(p => new
                        {
                            Key = p.Key,
                            AccountID = p.AccountA.Key,
                            AccountNumber = p.AccountA.Number,
                            ValueSize = (p.ValueSize * -1M).ToString(),
                            ValueSizeQuantity = p.ValueSize.Quantity * -1M,
                            ConvertedInstrumentSize = p.ConvertedInstrumentSize.ToString(),
                            ConvertedInstrumentSizeQuantity = p.ConvertedInstrumentSize.Quantity,
                            p.TransactionDate,
                            p.CreatedBy,
                            p.CreationDate,
                            p.Description,
                            p.Approved
                        })
                        .OrderBy(o => o.Description)
                        .ToDataSet();
            }
        }

        public static int CreateOrSaveInstrumentConversion(InstrumentConversionDetails convData)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ITradeableInstrument instrument = (ITradeableInstrument)InstrumentMapper.GetTradeableInstrument(session, convData.InstrumentID);
                ITradeableInstrument newInstrument = (ITradeableInstrument)InstrumentMapper.GetTradeableInstrument(session, convData.NewInstrumentID);
                IInstrumentsHistoryConversion conversion = null;
                if (convData.Key == 0)
                    conversion = new InstrumentsHistoryConversion(instrument, newInstrument,
                        convData.ChangeDate, convData.OldChildRatio,
                        convData.NewParentRatio, convData.IsSpinOff);
                else
                    conversion = InstrumentHistoryMapper.GetInstrumentConversion(session, convData.Key);

                conversion.ChangeDate = convData.ChangeDate;
                conversion.ExecutionDate = convData.ExecutionDate;
                conversion.IsSpinOff = convData.IsSpinOff;
                conversion.NewParentRatio = convData.NewParentRatio;
                conversion.OldChildRatio = convData.OldChildRatio;

                instrument.ParentInstrument = newInstrument;

                session.BeginTransaction();
                session.Update(instrument);
                session.InsertOrUpdate(conversion);
                session.CommitTransaction();

                return conversion.Key;
            }
        }

        public static bool InitialiseConversion(int instrumentConversionId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                bool success = false;
                int successCount = 0;
                IInstrumentsHistoryConversion conversion = InstrumentHistoryMapper.GetInstrumentConversion(session, instrumentConversionId);
                if (!conversion.IsInitialised)
                {
                    if (conversion.ChangeDate > DateTime.Today)
                        throw new ApplicationException("The change date is in the future.");

                    int journalId = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get(@"DefaultTransactiesEUR")));
                    IJournal journal = JournalMapper.GetJournal(session, journalId);
                    IList<IHistoricalPosition> accountsWithPositionByDate = B4F.TotalGiro.MIS.StoredPositions.StoredPositionTransactionMapper.GetAccountsWithPositionByDate(session, conversion.ChangeDate, conversion.Instrument.Key);

                    if (accountsWithPositionByDate != null && accountsWithPositionByDate.Count > 0)
                    {
                        DateTime transactionDate = conversion.ChangeDate;
                        decimal exRate = ((ITradeableInstrument)conversion.Instrument).CurrencyNominal.ExchangeRate.Rate;

                        foreach (IHistoricalPosition hp in accountsWithPositionByDate)
                        {
                            if (!conversion.Conversions.Any(x => x.AccountA.Key == hp.Account.Key))
                            {
                                IDalSession session2 = NHSessionFactory.CreateSession();
                                IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session2, hp.Account.Key);
                                InstrumentSize size = hp.ValueSize * -1M;
                                InstrumentSize newSize = new InstrumentSize(hp.ValueSize.Quantity * conversion.ConversionRate, conversion.NewInstrument);

                                if (size.IsNotZero)
                                {
                                    ITradingJournalEntry tradingJournalEntry = getNewBooking(session2, journal, transactionDate);
                                    IInstrumentConversion instConv = new InstrumentConversion(
                                        account, account.DefaultAccountforTransfer,
                                        size, newSize, exRate, conversion, tradingJournalEntry);
                                    if (session2.Insert(instConv))
                                        successCount++;
                                }
                                else
                                    successCount++;
                            }
                            else
                                successCount++;
                        }
                    }
                    if (accountsWithPositionByDate.Count == successCount)
                    {
                        conversion.IsInitialised = true;
                        session.InsertOrUpdate(conversion);
                        success = true;
                    }
                }
                return success;
            }
        }

        public static bool ExecuteConversion(int instrumentConversionId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                bool success = false;
                int todoCount = 0;
                int successCount = 0;
                IInstrumentsHistoryConversion conversion = InstrumentHistoryMapper.GetInstrumentConversion(session, instrumentConversionId);

                IInternalEmployeeLogin employee = LoginMapper.GetCurrentEmployee(session);
                todoCount = conversion.Conversions.Where(x => !x.Approved).Count();
                foreach (int instConvId in conversion.Conversions.Where(x => !x.Approved).Select(x => x.Key))
                {
                    IDalSession session2 = NHSessionFactory.CreateSession();
                    IInstrumentConversion instConv = (IInstrumentConversion)TransactionMapper.GetTransaction(session2, instConvId);
                    instConv.Approve(employee);
                    if (TransactionMapper.Update(session2, instConv))
                        successCount++;
                }

                if (todoCount == successCount)
                {
                    conversion.IsExecuted = true;
                    conversion.ExecutionDate = DateTime.Today;

                    session.Update(conversion);
                    deactivateInstrument(conversion.Instrument.Key, conversion.ChangeDate);
                    success = true;
                }
                return success;
            }
        }

        private static void deactivateInstrument(int instrumentId, DateTime changeDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                SecurityInstrument instrument = (SecurityInstrument)InstrumentMapper.GetTradeableInstrument(session, instrumentId);
                instrument.InActiveDate = changeDate;
                instrument.IsActive = false;
                session.Update(instrument);
            }
        }

        private static ITradingJournalEntry getNewBooking(IDalSession session, IJournal journal, DateTime transactionDate)
        {
            string nextJournalEntryNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, journal);
            ITradingJournalEntry newTradingJournalEntry = new TradingJournalEntry(journal, nextJournalEntryNumber, transactionDate);
            return newTradingJournalEntry;
        }


        public class InstrumentConversionDetails
        {
            public InstrumentConversionDetails() { }
            public InstrumentConversionDetails(IInstrumentsHistoryConversion conv)
            {
                if (conv != null)
                {
                    this.Key = conv.Key;
                    this.Instrument = (ITradeableInstrument)conv.Instrument;
                    this.InstrumentID = conv.Instrument.Key;
                    this.NewInstrument = (ITradeableInstrument)conv.NewInstrument;
                    this.NewInstrumentID = conv.NewInstrument.Key;
                    this.OldChildRatio = conv.OldChildRatio;
                    this.NewParentRatio = conv.NewParentRatio;
                    this.IsSpinOff = conv.IsSpinOff;
                    this.ChangeDate = conv.ChangeDate;
                    this.ExecutionDate = conv.ExecutionDate;
                    this.IsExecuted = conv.IsExecuted;
                    this.IsInitialised = conv.IsInitialised;

                    if (conv.IsInitialised && conv.Conversions != null && conv.Conversions.Count > 0)
                    {
                        InstrumentSize size  = conv.Conversions.TotalOriginalSize();
                        this.TotalOriginalSize = size != null ? size.Quantity * -1M : 0M;
                        InstrumentSize newsize = conv.Conversions.TotalConvertedSize();
                        this.TotalConvertedSize = newsize != null ? newsize.Quantity : 0M;
                    }
                }
            }
            public int Key;
            public ITradeableInstrument Instrument;
            public int InstrumentID;
            public ITradeableInstrument NewInstrument;
            public int NewInstrumentID;
            public decimal OldChildRatio;
            public byte NewParentRatio;
            public bool IsSpinOff;
            public DateTime ChangeDate;
            public DateTime ExecutionDate;
            public bool IsInitialised;
            public bool IsExecuted;
            public decimal TotalOriginalSize;
            public decimal TotalConvertedSize;
        }
    }
}

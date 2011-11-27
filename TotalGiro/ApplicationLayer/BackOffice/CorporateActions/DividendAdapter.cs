using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.CorporateAction;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.MIS.Positions;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Fees;

namespace B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions
{
    public class DividendAdapter
    {
        public static DataSet GetFilteredInstruments(
            string isin, string instrumentName, int currencyNominalId, 
            int secCategoryId, ActivityReturnFilter activityFilter)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = InstrumentMapper.GetTradeableInstruments(
                    session, SecCategoryFilterOptions.Securities,
                    isin, instrumentName, (SecCategories)secCategoryId, 0, 
                    currencyNominalId, false, activityFilter, "")
                    .Select(c => new
                    {
                        c.Key,
                        Description = c.DisplayIsinWithName
                    })
                    .ToDataSet();
                Utility.AddEmptyFirstRow(ds.Tables[0]);
                return ds;
            }
        }
        
        public static DataSet GetDividendHistories(int instrumentKey, DateTime startDate, DateTime endDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return DividendHistoryMapper.GetDividendHistoryList(session, instrumentKey, startDate, endDate)
                    .Select(d => new
                    {
                        d.Key,
                        InstrumentName = d.Instrument.Name,
                        d.ExDividendDate,
                        d.SettlementDate,
                        d.UnitPrice,
                        UnitPriceDisplay = d.UnitPrice.DisplayQuantity,
                        d.DividendType,
                        d.DisplayStatus

                    })
                    .ToDataSet();
            }

        }

        public static int InitialiseDividend(int dividendKey)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                int successCount = 0;
                IDividendHistory history = DividendHistoryMapper.GetDividendHistory(session, dividendKey);
                if (!history.IsInitialised)
                {
                    if (history.ExDividendDate > DateTime.Today)
                        throw new ApplicationException("The exdividend date is in the future.");
                    
                    int journalId = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get(@"DefaultDividendJournal")));
                    IJournal journal = JournalMapper.GetJournal(session, journalId);
                    IList<IHistoricalPosition> accountsWithPositionByDate = B4F.TotalGiro.MIS.StoredPositions.StoredPositionTransactionMapper.GetAccountsWithPositionByDate(session, history.ExDividendDate, history.Instrument.Key);

                    if (accountsWithPositionByDate != null && accountsWithPositionByDate.Count > 0)
                    {
                        if (history.NeedsStockDividend)
                        {
                            DateTime transactionDate = history.ExDividendDate;
                            IStockDividend instDiv = null;
                            if (!string.IsNullOrEmpty(history.StockDivIsin))
                                instDiv = history.Instrument.CorporateActionInstruments.GetStockDividendByIsin(history.StockDivIsin);
                            else
                                instDiv = history.Instrument.CorporateActionInstruments.GetLatestStockDividend();
                            if (instDiv == null)
                            {
                                instDiv = new StockDividend(history.Instrument, history.StockDivIsin);
                                if (!instDiv.Validate())
                                    throw new ApplicationException("Could not create stock dividend");
                                InstrumentMapper.Update(session, instDiv);
                            }
                            history.StockDividend = instDiv;
                            Price price = updateStockDividendPrices(session, instDiv, history);

                            foreach (IHistoricalPosition hp in accountsWithPositionByDate)
                            {
                                if (!history.StockDividends.Any(x => x.AccountA.Key == hp.Account.Key))
                                {
                                    IDalSession session2 = NHSessionFactory.CreateSession();
                                    IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session2, hp.Account.Key);
                                    InstrumentSize size = new InstrumentSize(hp.ValueSize.Quantity, instDiv);
                                    if (history.DividendType == DividendTypes.Scrip)
                                        size = size * (history.ScripRatio / 100M);

                                    if ((size * price).IsNotZero)
                                    {
                                        ITradingJournalEntry dividendBooking = getNewStockDividendBooking(session2, journal, transactionDate);
                                        ICorporateActionStockDividend stockDiv = new CorporateActionStockDividend(
                                            account, account.DefaultAccountforTransfer,
                                            size, price,
                                            instDiv.CurrencyNominal.ExchangeRate.Rate, transactionDate,
                                            history, hp.ValueSize, dividendBooking);
                                        if (session2.Insert(stockDiv))
                                            successCount++;
                                    }
                                    else
                                        successCount++;
                                }
                                else
                                    successCount++;
                            }
                        }
                        else
                        {
                            DateTime transactionDate = history.SettlementDate;
                            IGLLookupRecords lookups = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.CashDividend);
                            foreach (IHistoricalPosition hp in accountsWithPositionByDate)
                            {
                                if (!history.CashDividends.Any(x => x.Account.Key == hp.Account.Key))
                                {
                                    IDalSession session2 = NHSessionFactory.CreateSession();
                                    IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session2, hp.Account.Key);
                                    IMemorialBooking dividendBooking = getNewCashDividendBooking(session2, journal, transactionDate);
                                    ICashDividend newBooking = new CashDividend(account, dividendBooking, history.Description, history.TaxPercentage, history, hp.ValueSize, lookups);
                                    if (newBooking.TotalAmount.IsNotZero)
                                    {
                                        if (session2.Insert(newBooking))
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
                            history.IsInitialised = true;
                            session.InsertOrUpdate(history);
                        }
                    }
                }
                return dividendKey;
            }
        }

        protected static Price updateStockDividendPrices(IDalSession session, IStockDividend instrument, IDividendHistory history)
        {
            if (instrument == null)
                throw new ApplicationException("Stuff");
            
            Price price = new Price(history.UnitPrice.Quantity, instrument.CurrencyNominal, instrument);
            IList<IHistoricalPrice> historicalPrices = HistoricalPriceMapper.GetHistoricalPrices(session, instrument.Key, history.ExDividendDate, history.SettlementDate);
            DateTime priceDate = history.ExDividendDate;

            while (priceDate <= history.SettlementDate)
            {
                IHistoricalPrice historicalPrice = historicalPrices.Where(x => x.Date == priceDate).FirstOrDefault();
                if (historicalPrice == null)
                {
                    historicalPrice = new HistoricalPrice(price, priceDate);
                    instrument.HistoricalPrices.AddHistoricalPrice(historicalPrice);
                    InstrumentMapper.Update(session, instrument);
                }
                else
                {
                    if (historicalPrice.Price != price)
                    {
                        historicalPrice.Price = price;
                        HistoricalPriceMapper.Update(session, historicalPrice);
                    }
                }

                priceDate = priceDate.AddDays(1);
            }
            return price;
        }

        public static int ExecuteDividend(int dividendKey)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                int todoCount = -1;
                int successCount = 0;
                IDividendHistory history = DividendHistoryMapper.GetDividendHistory(session, dividendKey);
                if (history.ExDividendDate > DateTime.Today)
                    throw new ApplicationException("The exdividend date is in the future.");
                
                if (history.NeedsStockDividend)
                {
                    IInternalEmployeeLogin employee = LoginMapper.GetCurrentEmployee(session);
                    todoCount = history.StockDividends.Where(x => !x.Approved).Count();
                    foreach (int stockDivId in history.StockDividends.Where(x => !x.Approved).Select(x => x.Key))
                    {
                        IDalSession session2 = NHSessionFactory.CreateSession();
                        ICorporateActionStockDividend stockDiv = (ICorporateActionStockDividend)TransactionMapper.GetTransaction(session2, stockDivId);
                        stockDiv.Approve(employee);
                        if (TransactionMapper.Update(session2, stockDiv))
                            successCount++;
                    }
                }
                else
                {
                    IFeeFactory feeFactory = FeeFactory.GetInstance(session, FeeFactoryInstanceTypes.Commission);
                    todoCount = history.CashDividends.Where(x => x.GeneralOpsJournalEntry.Status != JournalEntryStati.Booked).Count();
                    foreach (int bookingId in history.CashDividends.Where(x => x.GeneralOpsJournalEntry.Status != JournalEntryStati.Booked).Select(x => x.Key))
                    {
                        IDalSession session2 = NHSessionFactory.CreateSession();
                        ICashDividend booking = (ICashDividend)GeneralOperationsBookingMapper.GetBooking(session2, bookingId);
                        executeBooking(booking, feeFactory);
                        if (session2.Update(booking))
                            successCount++;
                    }
                }

                if (todoCount == successCount)
                {
                    history.IsExecuted = true;
                    session.Update(history);
                }

                return dividendKey;
            }
        }

        private static void executeBooking(ICashDividend booking, IFeeFactory feeFactory)
        {
            ITradeableInstrument instrument;
            if (booking.NeedToCreateCashInitiatedOrder(out instrument))
            {
                Money divAmount = booking.Components.TotalAmount;
                if (instrument != null)
                {
                    OrderAmountBased order = new OrderAmountBased(booking.Account, divAmount, instrument, true, feeFactory, true);
                    order.OrderInfo = booking.Description;
                    booking.CashInitiatedOrder = order;
                }
                else
                {
                    // Sell from the biggest position
                    IFundPosition pos = booking.Account.Portfolio.PortfolioInstrument.Where(x => x.Size.IsGreaterThanZero).OrderByDescending(x => x.CurrentValue).FirstOrDefault();
                    if (pos != null && (pos.CurrentBaseValue + divAmount).IsGreaterThanOrEqualToZero)
                    {
                        OrderAmountBased order = new OrderAmountBased(booking.Account, divAmount, pos.Instrument, true, feeFactory, true);
                        order.OrderInfo = booking.Description;
                        booking.CashInitiatedOrder = order;
                    }
                }
            }
            booking.Execute();
        }

        private static ITradingJournalEntry getNewStockDividendBooking(IDalSession session, IJournal journal, DateTime transactionDate)
        {
            string nextJournalEntryNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, journal);
            ITradingJournalEntry newTradingJournalEntry = new TradingJournalEntry(journal, nextJournalEntryNumber, transactionDate);
            return newTradingJournalEntry;
        }

        private static IMemorialBooking getNewCashDividendBooking(IDalSession session, IJournal journal, DateTime transactionDate)
        {
            string nextJournalEntryNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, journal);
            IMemorialBooking newMemorialBooking = new MemorialBooking(journal, nextJournalEntryNumber);
            newMemorialBooking.TransactionDate = transactionDate;
            JournalEntryMapper.Insert(session, newMemorialBooking);
            return newMemorialBooking;
        }

        public static int LichtenDividend(int dividendKey)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                int successCount = 0;
                IDividendHistory history = DividendHistoryMapper.GetDividendHistory(session, dividendKey);
                int todoCount = -1;
                if (history.NeedsStockDividend && !history.IsGelicht)
                {
                    if (history.SettlementDate > DateTime.Today)
                        throw new ApplicationException("The settlement date is in the future.");

                    todoCount = history.StockDividends.Where(x => !x.IsGelicht).Count();
                    //IList<IHistoricalPosition> stockDivPositions = B4F.TotalGiro.MIS.StoredPositions.StoredPositionTransactionMapper.GetAccountsWithPositionByDate(session, history.SettlementDate, history.StockDividend.Key);

                    if (todoCount > 0)
                    {
                        IInternalEmployeeLogin employee = LoginMapper.GetCurrentEmployee(session);
                        int journalId = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get(@"DefaultDividendJournal")));
                        IJournal journal = JournalMapper.GetJournal(session, journalId);
                        IGLLookupRecords lookups = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.CashDividend);
                        DateTime transactionDate = history.SettlementDate;
                        IStockDividend instDiv = history.StockDividend;
                        Price price = instDiv.CurrentPrice.Get(e => e.Price);
                        IFeeFactory feeFactory = null;

                        foreach (int stDivId in history.StockDividends.Where(x => !x.IsGelicht).Select(x => x.Key))
                        {
                            IDalSession session2 = NHSessionFactory.CreateSession();
                            ICorporateActionStockDividend stDiv = (ICorporateActionStockDividend)TransactionMapper.GetTransaction(session2, stDivId);
                            ITradingJournalEntry tradingJournalEntry = getNewStockDividendBooking(session2, journal, transactionDate);
                            IAccountTypeCustomer account = (IAccountTypeCustomer)stDiv.AccountA;
                            ICorporateActionExecution corpaExec = new CorporateActionExecution(
                                account, account.DefaultAccountforTransfer,
                                new InstrumentSize(stDiv.ValueSize.Quantity * -1M, instDiv), price,
                                instDiv.CurrencyNominal.ExchangeRate.Rate, transactionDate,
                                history, tradingJournalEntry, "Lichten van " + instDiv.DisplayIsinWithName );
                            
                            if (history.DividendType == DividendTypes.Cash)
                            {
                                if (feeFactory == null)
                                    feeFactory = FeeFactory.GetInstance(session, FeeFactoryInstanceTypes.Commission);
                                IMemorialBooking dividendBooking = getNewCashDividendBooking(session2, journal, transactionDate);
                                ICashDividend newBooking = new CashDividend(account, 
                                    dividendBooking, history.Description, history.TaxPercentage,
                                    history, stDiv.ValueSize, lookups);
                                executeBooking(newBooking, feeFactory);

                                corpaExec.CounterBooking = newBooking;
                            }
                            else
                            {
                                InstrumentSize size = new InstrumentSize(stDiv.ValueSize.Quantity, instDiv.Underlying);
                                ITransactionNTM ntm = new TransactionNTM(account, account.DefaultAccountforTransfer,
                                        size, instDiv.Underlying.CurrentPrice.Get(e => e.Price), instDiv.CurrencyNominal.ExchangeRate.Rate, 
                                        transactionDate, transactionDate, 0M, size.Sign ? Side.XI : Side.XO,
                                        tradingJournalEntry, null, null, "Stock dividend " + instDiv.Underlying.DisplayIsinWithName);
                                ntm.Approve(employee);

                                corpaExec.CounterTransaction = ntm;
                            }
                            corpaExec.Approve(employee);
                            stDiv.IsGelicht = true;
                            session2.BeginTransaction();
                            if (session2.Insert(corpaExec) && session2.Update(stDiv))
                            {
                                if (session2.CommitTransaction())
                                    successCount++;
                            }
                        }
                    }
                    if (todoCount == successCount)
                    {
                        history.IsGelicht = true;
                        session.InsertOrUpdate(history);
                    }
                }
                return dividendKey;
            }
        }

        public static DividendHistoryDetails GetDividendDetails(int detailsID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IDividendHistory history = DividendHistoryMapper.GetDividendHistory(session, detailsID);
                return new DividendHistoryDetails(history);
            }
        }

        public static DataSet GetAccountDividendDetails(int instrumentHistoryID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                if (instrumentHistoryID != 0)
                {
                    IList<ICashDividend> cashdividends = DividendHistoryMapper.GetCashDividendDetails(session, instrumentHistoryID);
                    if (cashdividends != null && cashdividends.Count > 0)
                    {
                        ds = cashdividends.Select(d => new
                        {
                            d.Key,
                            AccountID = d.Account.Key,
                            AccountNumber = d.Account.Number,
                            UnitsInPossession = d.UnitsInPossession.DisplayString,
                            NumberOfUnits = d.UnitsInPossession.Quantity,
                            DividendAmount = d.DividendAmount,
                            TaxAmount = d.TaxAmount,
                            IsStockDiv = false
                        })
                        .ToDataSet();
                    }
                    else
                    {
                        IList<ICorporateActionStockDividend> stockdividends = DividendHistoryMapper.GetStockDividendDetails(session, instrumentHistoryID);
                        if (stockdividends != null && stockdividends.Count > 0)
                        {
                            ds = stockdividends.Select(d => new
                            {
                                d.Key,
                                AccountID = d.AccountA.Key,
                                AccountNumber = d.AccountA.Number,
                                UnitsInPossession = d.PreviousSize.DisplayString,
                                NumberOfUnits = d.PreviousSize.Quantity,
                                DividendAmount = d.DividendAmount,
                                TaxAmount = d.TaxAmount,
                                IsStockDiv = true
                            })
                            .ToDataSet();
                        }
                    }
                }
                return ds;
            }
        }

        public static DataSet GetTradeableInstruments()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return Instruments.InstrumentMapper.GetTradeableInstrumentsForDropDownList(session)
                .Select(p => new
                {
                    Key = p.Key,
                    Description = p.Value
                })
                .OrderBy(o => o.Description)
                .ToDataSet().AddEmptyFirstRow();
            }
        }

        public static string GetStockDividendIsin(int instrumentId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ISecurityInstrument fund = (ISecurityInstrument)InstrumentMapper.GetTradeableInstrument(session, instrumentId);
                return fund.CorporateActionInstruments.GetLatestStockDividend().GetS(e => e.DisplayIsin);
            }
        }

        public static int CreateOrSaveDividendHistory(DividendHistoryDetails newDividend)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ISecurityInstrument fund = (ISecurityInstrument)InstrumentMapper.GetTradeableInstrument(session, newDividend.FundID);
                IDividendHistory history = null;
                if (newDividend.Key == 0)
                    history = new DividendHistory(fund);
                else
                    history = DividendHistoryMapper.GetDividendHistory(session, newDividend.Key);

                history.DividendType = newDividend.DividendType;
                history.ExDividendDate = newDividend.ExDividendDate;
                history.SettlementDate = newDividend.SettlementDate;
                history.Description = newDividend.ExtDescription;
                history.StockDivIsin = newDividend.StockDivIsin;
                history.TotalNumberOfUnits = newDividend.TotalDividendDeposited;

                history.TypeOfDividendTax = (DividendTaxStyle)newDividend.DividendTaxStyle;
                history.TaxPercentage = ((newDividend.TaxPercentage != 0) && history.TypeOfDividendTax == DividendTaxStyle.Gross) ? newDividend.TaxPercentage : 0;
                history.UnitPrice = new Price(newDividend.UnitPrice, fund.CurrencyNominal, fund);
                history.ScripRatio = newDividend.ScripRatio;

                session.InsertOrUpdate(history);
                return history.Key;
            }
        }

        public static DataSet GetDividendTaxStyles()
        {
            return (Enum.GetValues(typeof(DividendTaxStyle))).Cast<DividendTaxStyle>()
                .Select(e => new
                {
                    Key = (int)e,
                    Description = e.ToString()
                })
                .ToDataSet();
        }


        public class DividendHistoryDetails
        {
            public DividendHistoryDetails() { }
            public DividendHistoryDetails(IDividendHistory history)
            {
                if (history != null)
                {
                    this.IsStockDiv = history.NeedsStockDividend;
                    this.Key = history.Key;
                    this.DividendType = history.DividendType;
                    this.Fund = history.Instrument;
                    this.FundID = history.Instrument.Key;
                    this.ExDividendDate = history.ExDividendDate;
                    this.SettlementDate = history.SettlementDate;
                    this.UnitPrice = history.UnitPrice != null ? history.UnitPrice.Quantity : 0M;
                    this.ScripRatio = history.ScripRatio;
                    this.ExtDescription = history.Description;
                    this.IsExecuted = history.IsExecuted;
                    this.IsInitialised = history.IsInitialised;
                    this.IsGelicht = history.IsGelicht;
                    this.DividendTaxStyle = (int)history.TypeOfDividendTax;
                    this.TaxPercentage = history.TaxPercentage;
                    this.StockDivIsin = history.StockDivIsin;
                    this.TotalDividendDeposited = history.TotalNumberOfUnits;
                    this.TotalUnitsInPossession = this.IsStockDiv ? history.StockDividends.Get(e => e.TotalUnits).GetS(e => e.DisplayString) : history.CashDividends.Get(e => e.TotalUnits).GetS(e => e.DisplayString);
                    this.TotalDividendAmount = this.IsStockDiv ? history.StockDividends.Get(e => e.TotalDividendAmount).GetS(e => e.DisplayString) : history.CashDividends.Get(e => e.TotalDividendAmount).GetS(e => e.DisplayString);
                }
            }
            public int Key;
            public DividendTypes DividendType;
            public ITradeableInstrument Fund;
            public int FundID;
            public DateTime ExDividendDate;
            public DateTime SettlementDate;
            public Decimal UnitPrice;
            public Decimal ScripRatio;
            public string ExtDescription;
            public bool IsStockDiv;
            public bool IsInitialised;
            public bool IsExecuted;
            public bool IsGelicht;
            public int DividendTaxStyle;
            public Decimal TaxPercentage;
            public string StockDivIsin;
            public decimal TotalDividendDeposited;
            public string TotalUnitsInPossession;
            public string TotalDividendAmount;

        }

    }
}

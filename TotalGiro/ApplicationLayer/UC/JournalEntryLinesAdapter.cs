using System;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments.ExRates;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public static class JournalEntryLinesAdapter
    {
        public static DataSet GetJournalEntryLines(int journalEntryId, bool isInsert, int stornoedLineId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IJournalEntry journalEntry = getJournalEntry(session, journalEntryId);
                IJournalEntryLineCollection lines = journalEntry.Lines;

                if (isInsert)
                {
                    int nextLineNumber = journalEntry.Lines.GetNextLineNumber();

                    if (stornoedLineId == 0)
                        lines.AddJournalEntryLine(new JournalEntryLine(nextLineNumber));
                    else
                    {
                        IJournalEntryLine stornoedLine = getLineById(journalEntry, stornoedLineId, JournalEntryLineStati.New, "stornoe");
                        lines.AddJournalEntryLine(stornoedLine.CreateStorno(nextLineNumber++));
                        lines.AddJournalEntryLine(stornoedLine.Clone(nextLineNumber));
                    }
                }


                return lines
                    .Select(c => new
                    {
                        c.Key,
                        c.LineNumber,
                        c.Status,
                        GLAccountId =
                            (c.GLAccount != null ? c.GLAccount.Key : 0),
                        GLAccount_FullDescription =
                            (c.GLAccount != null ? c.GLAccount.FullDescription : ""),
                        c.GLAccountIsFixed,
                        IsRelevantForDepositFee =
                            (c.GLAccount != null && c.GiroAccount != null ? c.GLAccount.CashTransferType == CashTransferTypes.Deposit : false),
                        BalanceCurrencyId =
                            (c.Balance != null ? c.Balance.Underlying.Key : 0M),
                        Currency =
                            (c.Currency != null ? c.Currency.Symbol : ""),
                        c.ExchangeRate,
                        c.Debit,
                        DebitDisplayString =
                            c.Debit != null ? c.Debit.DisplayString : "",
                        DebitQuantity =
                            c.Debit != null ? c.Debit.Quantity : 0M,
                        c.Credit,
                        CreditDisplayString =
                            c.Credit != null ? c.Credit.DisplayString : "",
                        CreditQuantity = 
                            c.Credit != null ? c.Credit.Quantity : 0M,
                        GiroAccount_Key =
                            c.GiroAccount != null ? c.GiroAccount.Key : 0,
                        GiroAccount_Number =
                            c.GiroAccount != null ? c.GiroAccount.Number : "",
                        GiroAccount_ShortName =
                            c.GiroAccount != null ? c.GiroAccount.ShortName : "",
                        c.Description,
                        c.OriginalDescription,
                        c.IsStornoable,
                        c.IsEditable,
                        c.IsDeletable,
                        c.IsAllowedToAddTransferFee,
                        IsStornoed = 
                            c.Storno != null || (c.Status == JournalEntryLineStati.Booked && c.StornoedLine != null)
                    })
                    .ToDataSet();

            }
            finally
            {
                session.Close();
            }
        }

        public static void UpdateJournalEntryLine(JournalEntryLineEditView lineEditView)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            
            try
            {
                IJournalEntry journalEntry = getJournalEntry(session, lineEditView.JournalEntryId);
                IJournalEntryLine updatingLine = null;

                if (lineEditView.Key == 0)
                    updatingLine = new JournalEntryLine();
                else
                {
                    updatingLine = getLineById(journalEntry, lineEditView.Key, JournalEntryLineStati.Booked, "update");
                    if (!updatingLine.IsEditable)
                        throw new ApplicationException(string.Format("Journal Entry Line number {0} is not editable.", updatingLine.LineNumber));
                }

                assignProperties(session, updatingLine, lineEditView, journalEntry.Journal.Currency);

                if (lineEditView.Key == 0)
                {
                    if (lineEditView.StornoedLineId != 0)
                    {
                        IJournalEntryLine stornoedLine = getLineById(journalEntry, lineEditView.StornoedLineId, JournalEntryLineStati.New, "stornoe");
                        journalEntry.Lines.AddJournalEntryLine(stornoedLine.CreateStorno());
                        updatingLine.OriginalDescription = stornoedLine.OriginalDescription;
                    }

                    journalEntry.Lines.AddJournalEntryLine(updatingLine);
                }

                session.BeginTransaction();

                JournalEntryMapper.Update(session, journalEntry);

                AdjustFixedAccountLine(session, journalEntry);

                session.CommitTransaction();
            }
            finally
            {
                session.Close();
            }
        }

        private static void assignProperties(IDalSession session, IJournalEntryLine updatingLine, JournalEntryLineEditView lineEditView,
                                             ICurrency defaultCurrency)
        {
            if (updatingLine.GLAccount == null || lineEditView.GLAccountId != updatingLine.GLAccount.Key)
            {
                IGLAccount glAccount = GLAccountMapper.GetGLAccount(session, lineEditView.GLAccountId);
                if (glAccount != null)
                    updatingLine.GLAccount = glAccount;
                else
                    throw new ApplicationException("Journal Entry Line must have a valid GLAccount.");
            }

            ICurrency currency = null;
            if (lineEditView.CurrencyId != 0)
            {
                if (updatingLine.Balance == null || lineEditView.CurrencyId != updatingLine.Balance.Underlying.Key)
                {
                    currency = InstrumentMapper.GetCurrency(session, lineEditView.CurrencyId);
                    if (currency == null)
                        throw new ApplicationException("Journal Entry Line must have a valid Currency.");
                }
                else
                    currency = (ICurrency)updatingLine.Balance.Underlying;
            }
            else
                currency = defaultCurrency;

            if ((lineEditView.DebitQuantity != 0m && lineEditView.CreditQuantity == 0m) || 
                (lineEditView.DebitQuantity == 0m && lineEditView.CreditQuantity != 0m))
                updatingLine.Balance = new Money(lineEditView.DebitQuantity - lineEditView.CreditQuantity, currency, currency.BaseCurrency, lineEditView.ExchangeRate);
            else
                throw new ApplicationException("Either Debit or Credit (but not both) on Journal Entry Line must be non-zero.");

            if (updatingLine.GLAccount.RequiresGiroAccount)
            {
                if (lineEditView.GiroAccountNumber == string.Empty)
                    throw new ApplicationException(
                        string.Format("Journal Entry Line is required by its GL Account ('{0}') to have a non-empty Giro Account.",
                                      updatingLine.GLAccount.FullDescription));

                if (updatingLine.GiroAccount == null || lineEditView.GiroAccountNumber != updatingLine.GiroAccount.Number)
                {
                    IAccountTypeCustomer giroAccount = AccountMapper.GetAccountByNumber(session, lineEditView.GiroAccountNumber) as IAccountTypeCustomer;
                    if (giroAccount != null)
                        updatingLine.GiroAccount = giroAccount;
                    else
                        throw new ApplicationException("Journal Entry Line must have a valid Giro Account (Customer or Nostro).");
                }
            }
            else
            {
                if (lineEditView.GiroAccountNumber != string.Empty)
                    throw new ApplicationException(
                        string.Format("Journal Entry Line is prohibited by its GL Account ('{0}') to have a Giro Account.",
                                      updatingLine.GLAccount.FullDescription));

                updatingLine.GiroAccount = null;
            }

            updatingLine.Description = lineEditView.Description;
        }

        public static void DeleteJournalEntryLine(int journalEntryId, int journalEntryLineId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IJournalEntry journalEntry = getJournalEntry(session, journalEntryId);
                IJournalEntryLine deletingLine = getLineById(journalEntry, journalEntryLineId, JournalEntryLineStati.Booked, "delete");

                if (deletingLine.IsDeletable)
                {
                    session.BeginTransaction();
                    deleteLine(session, deletingLine);
                    AdjustFixedAccountLine(session, journalEntry);
                    session.CommitTransaction();
                }
                else
                    throw new ApplicationException(string.Format("Journal Entry Line number {0} is not deletable.", deletingLine.LineNumber));
            }
            finally
            {
                session.Close();
            }
        }

        public static void AdjustFixedAccountLine(IDalSession session, IJournalEntry journalEntry)
        {
            try
            {
                if (journalEntry.Status == JournalEntryStati.New && journalEntry.Journal.FixedGLAccount != null)
                {
                    IJournalEntryLine fixedAccountLine = journalEntry.Lines.FixedAccountLine;
                    Money balance = journalEntry.Balance;

                    if (balance.IsNotZero)
                    {
                        if (fixedAccountLine == null)
                        {
                            fixedAccountLine = new JournalEntryLine();
                            fixedAccountLine.GLAccount = journalEntry.Journal.FixedGLAccount;

                            journalEntry.Lines.AddJournalEntryLine(fixedAccountLine);
                            fixedAccountLine.LineNumber = 0;
                        }

                        fixedAccountLine.Balance = balance.Negate();

                        JournalEntryMapper.Update(session, journalEntry);
                    }
                    else if (fixedAccountLine != null)
                        deleteLine(session, fixedAccountLine);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Could not adjust Fixed-Account Line.", ex);
            }
        }

        public static DataSet GetGLAccounts()
        {
            return GetGLAccounts(false, false);
        }

        public static DataSet GetGLAccounts(bool showAllowedManualOnly, bool isLineInsert)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = GLAccountMapper.GetGLAccounts(session, false, showAllowedManualOnly && isLineInsert)
                    .Select(c => new
                    {
                        c.Key,
                        c.FullDescription
                    })
                    .ToDataSet();
                Utility.AddEmptyFirstRow(ds);
                return ds;
            }
        }

        public static DataSet GetActiveCurrencies()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentMapper.GetCurrencies(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.Symbol
                    })
                    .ToDataSet();
            }
        }

        public static bool GetDefaultCurrencyFromGLAccount(int glAccountId, DateTime transactionDate, out int defaulCurrencyId, out decimal exRate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                bool success = false;
                defaulCurrencyId = 0;
                exRate = 1M;
                IGLAccount glacc = GLAccountMapper.GetGLAccount(session, glAccountId);
                if (glacc != null && glacc.DefaultCurrency != null)
                {
                    defaulCurrencyId = glacc.DefaultCurrency.Key;
                    IHistoricalExRate rate = HistoricalExRateMapper.GetHistoricalExRate(session, glacc.DefaultCurrency, transactionDate);
                    if (rate != null)
                        exRate = rate.Rate;
                    else if (glacc.DefaultCurrency.ExchangeRate != null)
                        exRate = glacc.DefaultCurrency.ExchangeRate.Rate;
                    success = true;
                }
                return success;
            }
        }

        public static decimal GetExChangeRate(int currencyId, DateTime transactionDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICurrency currency = InstrumentMapper.GetCurrency(session, currencyId);
                IHistoricalExRate rate = HistoricalExRateMapper.GetHistoricalExRate(session, currency, transactionDate);
                if (rate != null)
                    return rate.Rate;
                else
                    return currency.ExchangeRate.Rate;
            }
        }

        private static void deleteLine(IDalSession session, IJournalEntryLine line)
        {
            IJournalEntry journalEntry = line.Parent;
            int removedLineNumber = line.LineNumber;
            bool glAccountIsFixed = line.GLAccountIsFixed;

            journalEntry.Lines.Remove(line);
            session.Delete(line);

            if (!glAccountIsFixed)
                journalEntry.Lines.ShiftLineNumbers(removedLineNumber, -1);

            JournalEntryMapper.Update(session, journalEntry);
        }

        private static IJournalEntry getJournalEntry(IDalSession session, int journalEntryId)
        {
            IJournalEntry journalEntry = JournalEntryMapper.GetJournalEntry(session, journalEntryId);
            if (journalEntry == null)
                throw new ApplicationException(string.Format("Journal Entry with ID '{0}' could not be found.", journalEntryId));
            return journalEntry;
        }

        private static IJournalEntryLine getLineById(IJournalEntry journalEntry, int journalEntryLineId, JournalEntryLineStati illegalStatus,
                                                     string action)
        {
            IJournalEntryLine line = journalEntry.Lines.GetLineById(journalEntryLineId);

            if (line == null)
                throw new ApplicationException(string.Format("Journal Entry Line with ID '{0}' could not be found.", journalEntryLineId));
            else if (line.Status == illegalStatus)
                throw new ApplicationException(string.Format(
                    "Journal Entry Line number '{0}' cannot be {1}d because its status is {2}.",
                    line.LineNumber, action, line.Status));
            else if (line.GLAccount != null && line.GLAccount.IsFixed)
                throw new ApplicationException(string.Format(
                    "Journal Entry Line number '{0}' cannot be {1}d because its GL Account ('{2}') is a fixed account.",
                    line.LineNumber, action, line.GLAccount.FullDescription));

            return line;
        }

        // Not used (yet)
        public static DataSet GetInternalAccounts(int assetManagerId, string accountNumber, string accountName)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IAssetManager assetManager = null;
            if (assetManagerId > 0)
                assetManager = (IAssetManager)ManagementCompanyMapper.GetAssetManager(session, assetManagerId);

            DataSet ds = AccountMapper.GetInternalAccounts(session, assetManager, accountNumber, accountName)
                .Select(c => new
                {
                    c.Key,
                    c.Number,
                    c.DisplayNumberWithName
                })
                .ToDataSet();
            session.Close();

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static void BookJournalEntry(IDalSession session, IJournalEntry journalEntry)
        {
            journalEntry.BookLines();
            journalEntry.Status = JournalEntryStati.Booked;

            session.BeginTransaction();
            JournalEntryMapper.Update(session, journalEntry);
            session.CommitTransaction();
        }

        public static bool AddTransferFee(int journalEntryLineID, decimal tranferFeeQuantity, string description)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                bool success = false;
                IJournalEntryLine line = JournalEntryMapper.GetJournalEntryLine(session, journalEntryLineID);
                if (!line.IsAllowedToAddTransferFee)
                    throw new ApplicationException(string.Format("Journal Entry Line number {0} is not suitable for adding transfer fee.", line.LineNumber));
                
                if (line.BookComponent != null && line.BookComponent.Parent != null &&
                    line.BookComponent.Parent.BookingComponentParentType == BookingComponentParentTypes.CashTransfer)
                {
                    ICashTransfer mutation = ((ICashTransferComponent)line.BookComponent.Parent).ParentBooking as ICashTransfer;
                    if (mutation != null)
                    {
                        IGLLookupRecords lookups = GlLookupRecordMapper.GetGLLookupRecords(session);
                        Money tranferFee = new Money(tranferFeeQuantity, line.Currency).Negate();
                        if (mutation.AddTransferFee(tranferFee, lookups, description))
                            success = session.Update(mutation);
                    }
                }
                return success;
            }
        }

    }
}

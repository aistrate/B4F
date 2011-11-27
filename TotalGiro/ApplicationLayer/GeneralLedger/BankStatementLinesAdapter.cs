using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.BackOffice;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Communicator.Exact;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public enum BookingResults
    {
        OK = 0,
        ClosingBalanceNeeded = 1,
        TransactionDateNeeded = 2,
        UnchangedBalance = 3
    }

    public enum ScreenModeState
    {
        Main = 0,
        EditJournalEntry = 1,
        Question = 2,
        EditLine = 3,
        AddTransferFee = 4
    }
    
    public static class BankStatementLinesAdapter
    {
        public static DataSet GetBankStatementDetails(int journalEntryId)
        {
            //  @"Key, JournalEntryNumber, Status, DisplayStatus, Journal.Key, Journal.FullDescription, Journal.ManagementCompany.Key, 
            //  TransactionDate, OpenAmount, StartingBalanceDate, StartingBalance, ClosingBalance, HasClosingBalance, Lines.Count");
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return JournalEntryMapper.GetJournalEntries(session, journalEntryId)
                    .Cast<IBankStatement>()
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
                        Journal_Currency_IsBase =
                            c.Journal.Currency.IsBase,
                        c.ContainsForeignCashLines,
                        c.ExchangeRate,
                        c.TransactionDate,
                        c.OpenAmount,
                        c.StartingBalanceDate,
                        c.StartingBalance,
                        c.ClosingBalance,
                        c.HasClosingBalance,
                        Lines_Count =
                            c.Lines.Count,
                        ShowManualAllowedGLAccountsOnly =
                            c.Journal.ShowManualAllowedGLAccountsOnly
                    })
                    .ToDataSet();
            }
        }

        private static IBankStatement getBankStatement(IDalSession session, int journalEntryId)
        {
            IBankStatement bankStatement = (IBankStatement)JournalEntryMapper.GetJournalEntry(session, journalEntryId);
            if (bankStatement == null)
                throw new ApplicationException(string.Format("Bank Statement with ID '{0}' could not be found.", journalEntryId));
            return bankStatement;
        }

        public static IList GetBankStatementEditView(int journalEntryId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ArrayList editViews = new ArrayList();

            try
            {
                IBankStatement bankStatement = getBankStatement(session, journalEntryId);
                if (bankStatement != null)
                {
                    if (bankStatement.Status != JournalEntryStati.New)
                        throw new ApplicationException("Closing Balance cannot be edited because the Bank Statement has been already booked.");

                    Money closingBalance = (bankStatement.HasClosingBalance ? 
                                                    bankStatement.ClosingBalance : 
                                                    bankStatement.StartingBalance - bankStatement.Balance);
                    bool canChangeTransactionDate = (bankStatement.PrevBankStatement == null && bankStatement.Status == JournalEntryStati.New);

                    BankStatementEditView bankStatementEditView = new BankStatementEditView(journalEntryId, closingBalance, canChangeTransactionDate);

                    if (canChangeTransactionDate)
                        bankStatementEditView.TransactionDate = bankStatement.TransactionDate;

                    editViews.Add(bankStatementEditView);
                }
            }
            finally
            {
                session.Close();
            }

            return editViews;
        }

        public static void UpdateBankStatement(BankStatementEditView bankStatementEditView)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IBankStatement bankStatement = getBankStatement(session, bankStatementEditView.JournalEntryId);

                if (bankStatement.Status != JournalEntryStati.New)
                    throw new ApplicationException("Bank Statement cannot be updated because it has been already booked.");
                
                if (bankStatementEditView.CanChangeTransactionDate)
                    bankStatement.TransactionDate = bankStatementEditView.TransactionDate;
                bankStatement.ClosingBalance = new Money(bankStatementEditView.ClosingBalanceQuantity, bankStatement.Journal.Currency);
                bankStatement.HasClosingBalance = true;

                JournalEntryMapper.Update(session, bankStatement);
            }
            finally
            {
                session.Close();
            }
        }

        public static string ImportBankStatementLines(int journalEntryId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IBankStatement bankStatement = getBankStatement(session, journalEntryId);

                if (bankStatement.TransactionDate == DateTime.MinValue)
                    throw new ApplicationException("Cannot import because Bank Statement does not have a Transaction Date.");

                if (bankStatement.Status != JournalEntryStati.New)
                    throw new ApplicationException("Cannot import because Bank Statement has been already booked.");

                string balanceImportMessage = importBankBalance(session, bankStatement);

                string movementsImportMessage = importBankMovements(session, bankStatement);

                session.BeginTransaction();
                JournalEntryMapper.Update(session, bankStatement);
                JournalEntryLinesAdapter.AdjustFixedAccountLine(session, bankStatement);
                session.CommitTransaction();

                return balanceImportMessage + "<br />" + movementsImportMessage;
            }
            finally
            {
                session.Close();
            }
        }

        private static string importBankBalance(IDalSession session, IBankStatement bankStatement)
        {
            if (bankStatement.HasClosingBalance)
                return "Balance was not imported because it is already set.";
            else
            {
                IImportedBankBalance importedBankBalance =
                    ImportedBankBalanceMapper.GetImportedBankBalance(session, bankStatement.Journal, bankStatement.TransactionDate);

                if (importedBankBalance != null)
                {
                    if (!importedBankBalance.BookBalance.EqualCurrency(bankStatement.Journal.Currency))
                        throw new ApplicationException(
                            string.Format("Imported balance currency and Journal currency are different ('{0}' and '{1}' respectively).",
                                          importedBankBalance.BookBalance.Underlying.ToCurrency, bankStatement.Journal.Currency.Symbol));

                    bankStatement.ClosingBalance = importedBankBalance.BookBalance;
                    bankStatement.HasClosingBalance = true;
                    bankStatement.ImportedBankBalance = importedBankBalance;

                    return "Successfully imported Balance.";
                }
                else
                    return "Could not find Balance to import.";
            }
        }

        private static string importBankMovements(IDalSession session, IBankStatement bankStatement)
        {
            if (bankStatement.Lines.Count > 0)
                return "No Lines were imported because Lines already exist.";

            IList importedBankMovements =
                ImportedBankMovementMapper.GetImportedBankMovements(session, bankStatement.Journal, bankStatement.TransactionDate);

            IGLAccount depositGLAccount = null;
            IGLAccount withdrawalGLAccount = null;

            if (importedBankMovements.Count > 0)
            {
                string accountNumberPrefixes = GetAccountNumberPrefixes(session);

                foreach (IImportedBankMovement importedBankMovement in importedBankMovements)
                {
                    if (!importedBankMovement.MovementAmount.EqualCurrency(bankStatement.Journal.Currency))
                        throw new ApplicationException(
                            string.Format("Imported movement currency and Journal currency are different ('{0}' and '{1}' respectively).",
                                          importedBankMovement.MovementAmount.Underlying.ToCurrency, bankStatement.Journal.Currency));

                    IJournalEntryLine journalEntryLine = new JournalEntryLine();

                    string accountName = FindAccountName(importedBankMovement.Description, accountNumberPrefixes);
                    if (accountName != string.Empty)
                    {
                        ICustomerAccount giroAccount = AccountMapper.GetAccountByNumber(session, accountName) as ICustomerAccount;
                        if (giroAccount != null)
                            journalEntryLine.GiroAccount = giroAccount;

                        if (importedBankMovement.MovementAmount.IsGreaterThanZero)
                        {
                            if (depositGLAccount == null)
                                depositGLAccount = GLAccountMapper.GetDepositGLAccount(session);
                            journalEntryLine.GLAccount = depositGLAccount;
                        }
                        else if (importedBankMovement.MovementAmount.IsLessThanZero)
                        {
                            if (withdrawalGLAccount == null)
                                withdrawalGLAccount = GLAccountMapper.GetWithdrawalGLAccount(session);
                            journalEntryLine.GLAccount = withdrawalGLAccount;
                        }
                    }

                    journalEntryLine.Balance = importedBankMovement.MovementAmount.Negate();
                    journalEntryLine.OriginalDescription = importedBankMovement.Description.TrimEnd();
                    journalEntryLine.ImportedBankMovement = importedBankMovement;

                    bankStatement.Lines.AddJournalEntryLine(journalEntryLine);
                }

                return string.Format("Successfully imported {0} Line{1}.", bankStatement.Lines.Count, (bankStatement.Lines.Count > 1 ? "s" : ""));
            }

            return "Could not find Lines to import.";
        }

        public static string GetAccountNumberPrefixes(IDalSession session)
        {
            string prefixes = "";

            foreach (string prefix in AccountMapper.GetAccountNumberPrefixes(session))
            {
                Match match = Regex.Match(prefix, @"^([A-Z]{2}([A-Z]{2})?)\d*$", RegexOptions.IgnoreCase);
                if (match.Success)
                    prefixes += match.Groups[1].Value.ToUpper() + "|";
            }

            if (prefixes == "")
                throw new ApplicationException("Could not find any valid account number prefixes.");
            else
                prefixes += "EVGL";     // frequent misspelling of 'EGVL'
            
            return prefixes;
        }

        public static string FindAccountName(string text, string accountNumberPrefixes)
        {
            if (text != null && text != string.Empty)
            {
                string pattern = "(" + accountNumberPrefixes + @")(([.*]|\s)*(O|\d)){5}\d{0,3}(?!\d)";
                Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    return match.Value.ToUpper().Replace(" ", "").Replace(".", "").Replace("*", "").Replace("O", "0");
                {
                    string secondPattern = @"^(\d{5,8})\b.*(EGVLO?)\s*$";
                    match = Regex.Match(text, secondPattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string accountNumber = match.Groups[2].Value.ToUpper().Replace("O", "0") + match.Groups[1].Value;
                        if (accountNumber.Length == 10)
                            return accountNumber;
                    }
                }
            }

            return "";
        }

        // This is for testing only (Test/TestRegex.aspx)
        public static DataSet GetMovements()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            
            Hashtable parameters = new Hashtable();
            string hql = "FROM ImportedBankMovement";
            IList movements = session.GetListByHQL(hql, parameters);

            string accountNumberPrefixes = GetAccountNumberPrefixes(session);

            session.Close();

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ds.Tables.Add(dt);
            dt.Columns.Add("Key", typeof(int));
            dt.Columns.Add("DateString", typeof(string));
            dt.Columns.Add("ExtractedAccountNumber", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            
            //string movementIdList = ", 28276, 25361, 25840, 29038, 21166, 21755, 22124, 25008, 25138, 25349, 33904, 35087, 27794, 29715, 17574, 17558, 17857, 24592, 27188,";
            foreach (IImportedBankMovement movement in movements)
            {
                string extractedAccountNumber = FindAccountName(movement.Description, accountNumberPrefixes);
                //if (movement.Description.Trim() != string.Empty && extractedAccountNumber == string.Empty)
                if (extractedAccountNumber != string.Empty)
                //if (movementIdList.IndexOf(", " + movement.Key.ToString() + ",") >= 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["Key"] = movement.Key;
                    dr["DateString"] = movement.BankStatementDate.ToString("yyyy-MM-dd");
                    dr["Description"] = movement.Description;
                    dr["ExtractedAccountNumber"] = extractedAccountNumber;
                    dt.Rows.Add(dr);
                }
            }

            return ds;
        }

        public static BookingResults BookBankStatement(int journalEntryId, bool forceIfUnchanged)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IBankStatement bankStatement = getBankStatement(session, journalEntryId);

                if (bankStatement.TransactionDate == DateTime.MinValue || !bankStatement.HasClosingBalance)
                    return BookingResults.ClosingBalanceNeeded;
                else if (bankStatement.Status == JournalEntryStati.New &&
                         !forceIfUnchanged && bankStatement.ClosingBalance == bankStatement.StartingBalance && bankStatement.OpenAmount.IsZero)
                    return BookingResults.UnchangedBalance;
                else
                {
                    doBook(session, bankStatement);
                    return BookingResults.OK;
                }
            }
            finally
            {
                session.Close();
            }
        }

        private static void doBook(IDalSession session, IBankStatement bankStatement)
        {
            if (bankStatement.Status == JournalEntryStati.Booked)
                throw new ApplicationException("This Bank Statement has been already booked.");

            if (bankStatement.OpenAmount.IsNotZero)
                throw new ApplicationException(
                    string.Format("Cannot book Bank Statement because its Open Amount is different from zero ({0:#,##0.00}).", 
                                  bankStatement.OpenAmount));

            if (bankStatement.TotalBalance.IsNotZero)
                throw new ApplicationException(
                    string.Format("Cannot book Bank Statement because its Total Balance (including Fixed Account) is different from zero ({0:#,##0.00}).",
                                  bankStatement.TotalBalance));

            foreach (IJournalEntryLine line in bankStatement.Lines)
                if (line.Status == JournalEntryLineStati.New)
                {
                    if (line.GLAccount == null)
                        throw new ApplicationException(string.Format("Bank Statement Line number {0} does not have a GL Account.", line.LineNumber));

                    if (!line.IsStorno)
                    {
                        if (line.GLAccount.RequiresGiroAccount && line.GiroAccount == null)
                            throw new ApplicationException(
                                string.Format("Bank Statement Line number {0} is required by its GL Account ('{1}') to have a non-empty Giro Account.",
                                              line.LineNumber, line.GLAccount.FullDescription));
                        else if (!line.GLAccount.RequiresGiroAccount && line.GiroAccount != null)
                            throw new ApplicationException(
                                string.Format("Bank Statement Line number {0} is prohibited by its GL Account ('{1}') to have a Giro Account.",
                                              line.LineNumber, line.GLAccount.FullDescription));
                    }
                }

            JournalEntryLinesAdapter.BookJournalEntry(session, bankStatement);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Jobs;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using log4net;

namespace B4F.TotalGiro.Valuations
{
    #region Enum

    public enum ValuationEngineJobs
    {
        ValuationMutations,
        DailyValuations,
        AverageHoldings,
        Fees
    }

    #endregion
    
    public class ValuationEngine: AgentWorker
    {
        #region Valuation Mutation Stuff

        public int RunValuationMutations(IDalSessionFactory factory, BackgroundWorker worker, DoWorkEventArgs e)
        {
            int counter = 0;
            int successCount = 0;
            int errCount = 0;
            Hashtable errors = new Hashtable();
            DateTime maxDate = MaxValuationDate;
            try
            {
                IDalSession session = factory.CreateSession();
                string result;
                int progress = 0;

                // set session timeout 
                session.TimeOut = 0;
                session.LockMode = LockModes.None;

                if (RunSP_ResetValuations)
                {
                    // Run TG_ResetValuations
                    ResetValuations(session, null);
                }

                // Set MaxValuationMutationDate
                SetValuationMutationValidityDate(session, maxDate);

                IList<int> accountKeys = getAccountKeysForRunValuationMutations(session, maxDate);
                session.Close();

                if (accountKeys != null && accountKeys.Count > 0)
                {
                    foreach (int accountID in accountKeys)
                    {
                        session = factory.CreateSession();
                        Hashtable parameters2 = new Hashtable();
                        parameters2.Add("accountID", accountID);
                        IList accounts = session.GetTypedListByNamedQuery<IAccountTypeCustomer>(
                            "B4F.TotalGiro.Valuations.GetAccountForRunValuationMutations",
                            parameters2);
                        
                        if (accounts != null && accounts.Count == 1)
                        {
                            string message;
                            IAccountTypeCustomer account = (IAccountTypeCustomer)accounts[0];

                            bool success = runValuationMutationsForAccount(session, account, maxDate, out message);
                            session.Close();
                            if (success)
                                successCount++;
                            else
                            {
                                if (errors.ContainsKey(message))
                                    errors[message] = ((int)errors[message]) + 1;
                                else
                                    errors.Add(message, 1);
                                errCount++;
                            }
                            counter++;
                        }

                        if (worker != null)
                        {
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                                result = string.Format("The Valuation Job was cancelled, {0} accounts were valuated successfully {1}", successCount.ToString(), getFailureMessage(errors, errCount));
                                e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, successCount.ToString(), result);
                                return successCount;
                            }
                            else if (errCount > 99)
                            {
                                e.Cancel = true;
                                result = string.Format("The Valuation Job was cancelled, to many failures, {0} accounts were valuated successfully {1}", successCount.ToString(), getFailureMessage(errors, errCount));
                                e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, successCount.ToString(), result);
                                return successCount;
                            }
                            else
                            {
                                if (progress != ((counter * 100) / accountKeys.Count) && progress < 99)
                                {
                                    progress = ((counter * 100) / accountKeys.Count);
                                    worker.ReportProgress(progress);
                                }
                            }
                        }
                    }
                }
                result = string.Format("{0} accounts were valuated successfully {1}", successCount.ToString(), getFailureMessage(errors, errCount));
                if (e != null)
                    e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Ok, successCount.ToString(), result);
            }
            catch (Exception ex)
            {
                successCount = 0;
                if (e != null)
                    e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Exception, "An error occured during the valuation job", "", ex);
                else
                    throw new ApplicationException(Util.GetMessageFromException(ex));
            }
            finally
            {
                if (e != null && worker != null)
                {
                    e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Ok, successCount.ToString(), string.Format("Successfully valuated {0} accounts.", successCount));
                    worker.ReportProgress(100);
                }
            }
            return successCount;
        }

        private IList<int> getAccountKeysForRunValuationMutations(IDalSession session, DateTime maxDate)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("maxDate", maxDate);
            IList<int> accountKeysPos = session.GetTypedListByNamedQuery<int>(
                "B4F.TotalGiro.Valuations.GetAccountKeysForRunValuationMutationsFromPositions",
                parameters);

            parameters.Add("settledFlag", CashPositionSettleStatus.Settled);
            parameters.Add("unSettledFlag", CashPositionSettleStatus.UnSettled);
            parameters.Add("statusBooked", JournalEntryLineStati.Booked);
            IList<int> accountKeysJournal = session.GetTypedListByNamedQuery<int>(
                "B4F.TotalGiro.Valuations.GetAccountKeysForRunValuationMutationsFromJournal",
                parameters);

            return accountKeysJournal.Union(accountKeysPos).OrderBy(x => x).ToList();
        }

        private string getFailureMessage(Hashtable errors, int errCount)
        {
            string message = "";
            if (errors != null && errors.Count > 0)
            {
                foreach (DictionaryEntry entry in errors)
                {
                    message += string.Format(@"{0} the following error was recorded: /n {1}", entry.Value, entry.Key);
                }
                message = string.Format(@"{0} accounts did not valuate correctly /n", errCount.ToString()) + message;
            }
            return message;
        }

        public void RunValuationMutationsForAccount(IDalSession session, IAccountTypeCustomer account, bool runSP)
        {
            DateTime maxDate = MaxValuationDate;
            string message;

            // Run TG_ResetValuations for account
            if (runSP)
                ResetValuations(session, account);

            bool success = runValuationMutationsForAccount(session, account, maxDate, out message);
            if (!success)
                throw new ApplicationException(message);
        }

        private bool runValuationMutationsForAccount(IDalSession session, IAccountTypeCustomer account, DateTime maxDate, out string message)
        {
            bool success = false;
            message = "";
            try
            {
                IList<IFundPositionTx> posTxs;
                IList<IJournalEntryLine> lines;
                List<IValuationMutation> mutationsToSave = null;
                IValuationCashMutation[] cashMutationsToSave = null;

                if (getFundPositionTxData(session, account, maxDate, out posTxs))
                {
                    // Calculate the valuations
                    runValuationMutationJobForAccount(session, posTxs, account, maxDate, ref mutationsToSave);
                }
                if (getJournalEntryLineData(session, account, maxDate, out lines))
                {
                    // Calculate the valuations
                    runValuationMutationJobForAccount(session, lines, account, maxDate, ref mutationsToSave);
                    // Calculate the cash valuations
                    runCashValuationMutationJobForAccount(session, lines, account, maxDate, out cashMutationsToSave);
                }
                // save data
                saveMutations(session, ref mutationsToSave, ref cashMutationsToSave);
                
                // Get the ValuationMutationValidityDate of the mutations
                DateTime validityDate = DateTime.MinValue;
                foreach (ICommonPosition pos in account.Portfolio.AllPositions)
                {
                    if (pos.LastMutation != null && pos.LastMutation.Size.IsZero)
                    {
                        if (validityDate < pos.LastMutation.Date)
                            validityDate = pos.LastMutation.Date;
                    }
                    else
                    {
                        // Not all positions are closed -> take maxDate
                        validityDate = maxDate;
                        break;
                    }
                }

                //// save the Validity Date
                setAccountValidityDate(session, account, validityDate);
                success = true;
            }
            catch (Exception ex)
            {
                message = Util.GetMessageFromException(ex);
                string logMessage = string.Format("Error in runValuationMutationsForAccount -> account: {0}; {1}", account.Key, message);
                log.Error(logMessage);
            }
            return success;
        }

        public void ResetValuations(IDalSession session)
        {
            ResetValuations(session, null);
        }

        public void ResetValuations(IDalSession session, IAccountTypeCustomer account)
        {
            // Run TG_ResetValuations
            //raiseProgressEvent(string.Format("Run TG_ResetValuations for {0}", (account != null ? account.DisplayNumberWithName: "all accounts")));

            SqlCommand command = new SqlCommand("TG_ResetValuations", (SqlConnection)session.Connection);
            SqlParameter param1 = new SqlParameter("endDate", MaxValuationDate);
            command.Parameters.Add(param1);
            SqlParameter param2 = new SqlParameter("AccountID", (account != null ? account.Key : 0));
            command.Parameters.Add(param2);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            int i = command.ExecuteNonQuery();
        }

        public void SetValuationMutationValidityDate(IDalSession session, DateTime maxDate)
        {
            // Run TG_SetValuationMutationValidityDate
            SqlCommand command = new SqlCommand("TG_SetValuationMutationValidityDate", (SqlConnection)session.Connection);
            SqlParameter param = new SqlParameter("valuationdate", maxDate);
            command.Parameters.Add(param);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            int i = command.ExecuteNonQuery();
        }

        private bool getFundPositionTxData(IDalSession session, IAccountTypeCustomer account, DateTime maxDate, out IList<IFundPositionTx> posTxs)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("accountID",account.Key);
            parameters.Add("maxDate",maxDate);

            posTxs = session.GetTypedListByNamedQuery<IFundPositionTx>(
                "B4F.TotalGiro.Valuations.GetFundPositionTxData",
                parameters);
            return (posTxs != null && posTxs.Count > 0);
        }

        private bool getJournalEntryLineData(IDalSession session, IAccountTypeCustomer account, DateTime maxDate, out IList<IJournalEntryLine> lines)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("accountID", account.Key);
            parameters.Add("maxDate", maxDate);
            parameters.Add("settledFlag", CashPositionSettleStatus.Settled);
            parameters.Add("unSettledFlag", CashPositionSettleStatus.UnSettled);
            parameters.Add("statusBooked", JournalEntryLineStati.Booked);

            lines = session.GetTypedListByNamedQuery<IJournalEntryLine>(
                "B4F.TotalGiro.Valuations.GetJournalEntryLineData",
                parameters);
            return (lines != null && lines.Count > 0);
        }

        private void runValuationMutationJobForAccount(IDalSession session, IList<IFundPositionTx> posTxs, IAccountTypeCustomer account, DateTime maxDate, ref List<IValuationMutation> mutationsToSave)
        {
            ////raiseProgressEvent(string.Format("Create valuations for {0}", account.DisplayNumberWithName));
            IDictionary<string, IValuationMutation> mutations = new Dictionary<string, IValuationMutation>();
            ISecurityValuationMutation mutation;
            IList<IFundPositionTx> notRelevantPositionTxs = new List<IFundPositionTx>();

            if (posTxs != null && posTxs.Count > 0)
            {
                foreach (IFundPositionTx posTx in posTxs)
                {
                    if (posTx.TransactionDate <= maxDate && (Util.IsNullDate(account.ValuationsEndDate) || posTx.TransactionDate <= account.ValuationsEndDate))
                    {
                        if (posTx.IsRelevant)
                        {
                            if (posTx.ParentPosition.LastMutation == null)
                            {
                                IList<IFundPosition> convertedPositions = null;
                                if (posTx.IsConversion)
                                {
                                    // Check whether the Instrument has changed
                                    convertedPositions = getPositionsConvertedInstrument(posTx.Instrument, account.Portfolio.PortfolioInstrument);
                                }

                                if (convertedPositions != null && convertedPositions.Count > 0)
                                    mutation = new SecurityValuationMutation(convertedPositions, posTx, posTx.TransactionDate);
                                // Create Complete New Valuation
                                else
                                    mutation = new SecurityValuationMutation(posTx);
                            }
                            else
                            {
                                // check that Date of LastMutation is not after the TxDate
                                if (posTx.ParentPosition.LastMutation.Date > posTx.TransactionDate)
                                    throw new ApplicationException(string.Format("The date of the LastMutation can not be greater than the TxDate for Account: {0}, Instrument: {1}, Date: {2}", posTx.Account.Key, posTx.Instrument.Key, posTx.TransactionDate));
                                
                                // Is the TxDate the same
                                if (posTx.ParentPosition.LastMutation.Date.Equals(posTx.TransactionDate))
                                    mutation = (ISecurityValuationMutation)posTx.ParentPosition.LastMutation;
                                // else Create New Valuation from Previous Valuation
                                else
                                    mutation = new SecurityValuationMutation(posTx.TransactionDate, (ISecurityValuationMutation)posTx.ParentPosition.LastMutation);
                            }
                            mutation.AddTx(posTx);

                            account.Portfolio.PortfolioInstrument.GetPosition(posTx.Instrument).LastMutation = mutation;
                            if (!mutations.Keys.Contains(mutation.GetUniqueCode))
                                mutations.Add(mutation.GetUniqueCode, mutation);
                        }
                        else
                            notRelevantPositionTxs.Add(posTx);
                    }
                }
                if (mutations != null && mutations.Count > 0)
                {
                    mutationsToSave = mutations.Values.ToList();
                    if (notRelevantPositionTxs.Count > 0)
                        addNotRelevantPosTxToMutations(ref mutationsToSave, notRelevantPositionTxs);

                    // Validate mutations
                    foreach (IValuationMutation mut in mutationsToSave)
                        mut.Validate();
                }
            }
        }

        private void runValuationMutationJobForAccount(IDalSession session, IList<IJournalEntryLine> lines, IAccountTypeCustomer account, DateTime maxDate, ref List<IValuationMutation> mutationsToSave)
        {
            ////raiseProgressEvent(string.Format("Create valuations for {0}", account.DisplayNumberWithName));
            IDictionary<string, IValuationMutation> mutations = new Dictionary<string, IValuationMutation>();
            IMonetaryValuationMutation mutation;
            IList<IJournalEntryLine> notRelevantLines = new List<IJournalEntryLine>();

            if (lines != null && lines.Count > 0)
            {
                foreach (IJournalEntryLine line in lines)
                {
                    if (line.BookDate <= maxDate && (Util.IsNullDate(account.ValuationsEndDate) || line.BookDate <= account.ValuationsEndDate))
                    {
                        if (line.IsRelevant && line.ParentSubPosition.SettledFlag == CashPositionSettleStatus.Settled)
                        {
                            if (line.ParentSubPosition.ParentPosition.LastMutation == null)
                                mutation = new MonetaryValuationMutation(line);
                            else
                            {
                                // check that Date of LastMutation is not after the TxDate
                                if (line.ParentSubPosition.ParentPosition.LastMutation.Date > line.BookDate)
                                    throw new ApplicationException(string.Format("The date of the LastMutation can not be greater than the TxDate for Account: {0}, Instrument: {1}, Date: {2}", account.Key, line.Currency.Key, line.BookDate));
                                
                                // Is the TxDate the same
                                if (line.ParentSubPosition.ParentPosition.LastMutation.Date.Equals(line.BookDate))
                                    mutation = (IMonetaryValuationMutation)line.ParentSubPosition.ParentPosition.LastMutation;
                                // else Create New Valuation from Previous Valuation
                                else
                                    mutation = new MonetaryValuationMutation(line.BookDate, (IMonetaryValuationMutation)line.ParentSubPosition.ParentPosition.LastMutation);
                            }
                            mutation.AddLine(line);

                            account.Portfolio.PortfolioCashGL.GetPosition(line.Currency).LastMutation = mutation;
                            if (!mutations.Keys.Contains(mutation.GetUniqueCode))
                                mutations.Add(mutation.GetUniqueCode, mutation);
                        }
                        else
                            notRelevantLines.Add(line);
                    }
                }
                if (mutations != null && mutations.Count > 0)
                {
                    if (mutationsToSave == null)
                        mutationsToSave = new List<IValuationMutation>(mutations.Count);
                    mutationsToSave.AddRange(mutations.Values);
                    if (notRelevantLines.Count > 0)
                        addNotRelevantLinesToMutations(ref mutationsToSave, notRelevantLines);

                    // Validate mutations
                    foreach (IValuationMutation mut in mutationsToSave)
                        mut.Validate();
                }
                else if (notRelevantLines.Count > 0)
                {
                    IValuationMutation mut = lines[0].ParentSubPosition.ParentPosition.LastMutation;
                    if (mut != null)
                    {
                        mutationsToSave = new List<IValuationMutation>(1);
                        mutationsToSave.Add(mut);
                        addNotRelevantLinesToMutations(ref mutationsToSave, notRelevantLines);
                    }
                }
            }
        }

        private void runCashValuationMutationJobForAccount(IDalSession session, IList<IJournalEntryLine> lines, IAccountTypeCustomer account, DateTime maxDate, out IValuationCashMutation[] cashMutationsToSave)
        {
            //raiseProgressEvent(string.Format("Create Cash valuations for {0}", account.DisplayNumberWithName));
            cashMutationsToSave = null;
            IDictionary<string, IValuationCashMutation> mutations = new Dictionary<string, IValuationCashMutation>();
            IValuationCashMutation mutation;
            IList<IJournalEntryLine> notRelevantLines = new List<IJournalEntryLine>();

            if (lines != null && lines.Count > 0)
            {
                foreach (IJournalEntryLine line in lines.Where(u => u.GLAccount.ValuationCashType != ValuationCashTypes.None))
                {
                    if (line.BookDate <= maxDate && (Util.IsNullDate(account.ValuationsEndDate) || line.BookDate <= account.ValuationsEndDate))
                    {
                        if (line.IsRelevant)
                        {
                            ValuationCashMutationKey key = new ValuationCashMutationKey((IAccountTypeCustomer)line.GiroAccount, line.BookingRelatedInstrument, line.GLAccount.ValuationCashType);

                            // search key in account
                            // It's possible because of storno's or earlier tx that there is no LastCashMutation
                            // In this case create new CashMutation
                            if (account.LastValuationCashMutations.Count > 0 && account.LastValuationCashMutations.ContainsKey(key) && account.LastValuationCashMutations[key].LastCashMutation != null)
                            {
                                ILastValuationCashMutationHolder holder = account.LastValuationCashMutations[key];

                                // If pos Date is earlier than last recorded date -> skip it
                                if (holder.LastCashMutation.Date <= line.BookDate)
                                {
                                    // Is the TxDate the same
                                    if (holder.LastCashMutation.Date.Equals(line.BookDate))
                                    {
                                        // check whether line not already stored
                                        if (!holder.LastCashMutation.ContainsLine(line))
                                            mutation = holder.LastCashMutation;
                                        else
                                            mutation = null;
                                    }
                                    else
                                    {
                                        mutation = new ValuationCashMutation(line.BookDate, holder.LastCashMutation);
                                    }
                                }
                                else
                                    mutation = null;
                            }
                            else
                            {
                                // Create Complete New Cash Valuation
                                mutation = new ValuationCashMutation(line);
                            }

                            if (mutation != null)
                            {
                                mutation.AddLine(line);

                                if (account.LastValuationCashMutations.ContainsKey(key))
                                    account.LastValuationCashMutations[key].LastCashMutation = mutation;
                                else
                                    account.LastValuationCashMutations.Add(key, mutation);

                                if (!mutations.Keys.Contains(mutation.GetUniqueCode))
                                    mutations.Add(mutation.GetUniqueCode, mutation);
                            }
                        }
                        else
                            notRelevantLines.Add(line);
                    }
                }
                if (mutations != null && mutations.Count > 0)
                {
                    cashMutationsToSave = new IValuationCashMutation[mutations.Count];
                    mutations.Values.CopyTo(cashMutationsToSave, 0);

                    if (notRelevantLines.Count > 0)
                        addNotRelevantLinesToCashMutations(ref cashMutationsToSave, notRelevantLines);

                    // Validate mutations
                    foreach (IValuationCashMutation mut in cashMutationsToSave)
                        mut.Validate();
                }
            }
        }

        #region NotRelevantPosTx

        private void addNotRelevantPosTxToMutations(ref List<IValuationMutation> mutationsToSave, IList<IFundPositionTx> notRelevantPositionTxs)
        {
            if (mutationsToSave != null && mutationsToSave.Count > 0 && notRelevantPositionTxs.Count > 0)
            {
                foreach (IFundPositionTx posTx in notRelevantPositionTxs)
                {
                    ISecurityValuationMutation mutation = (ISecurityValuationMutation)getNearestMutation(ref mutationsToSave, posTx);
                    if (mutation != null)
                        mutation.AddNotRelevantPositionTx(posTx);
                }
            }
        }

        private void addNotRelevantLinesToMutations(ref List<IValuationMutation> mutationsToSave, IList<IJournalEntryLine> notRelevantLines)
        {
            if (mutationsToSave != null && mutationsToSave.Count > 0 && notRelevantLines.Count > 0)
            {
                foreach (IJournalEntryLine line in notRelevantLines)
                {
                    IMonetaryValuationMutation mutation = (IMonetaryValuationMutation)getNearestMutation(ref mutationsToSave, line);
                    if (mutation != null)
                        mutation.AddNotRelevantLine(line);
                }
            }
        }

        private void addNotRelevantLinesToCashMutations(ref IValuationCashMutation[] cashMutationsToSave, IList<IJournalEntryLine> notRelevantLines)
        {
            if (cashMutationsToSave != null && cashMutationsToSave.Length > 0 && notRelevantLines.Count > 0)
            {
                foreach (IJournalEntryLine line in notRelevantLines)
                {
                    IValuationCashMutation mutation = getNearestMutation(ref cashMutationsToSave, line, 3);
                    if (mutation == null)
                        mutation = getNearestMutation(ref cashMutationsToSave, line, 2);
                    if (mutation == null)
                        mutation = getNearestMutation(ref cashMutationsToSave, line, 1);
                    if (mutation == null)
                        mutation = cashMutationsToSave[0];
                    if (mutation != null)
                        mutation.AddNotRelevantLine(line);
                }
            }
        }

        private ISecurityValuationMutation getNearestMutation(ref List<IValuationMutation> mutationsToSave, IFundPositionTx posTx)
        {
            ISecurityValuationMutation mut = null;
            TimeSpan days = TimeSpan.MaxValue;

            foreach (IValuationMutation mutation in mutationsToSave)
            {
                if (posTx.Instrument.Equals(mutation.Instrument))
                {
                    //if (mut != null && ((TimeSpan)(mutation.Date - posTx.TransactionDate)).Days > 0)
                    //    return mut;

                    if (mut == null || (days < (mutation.Date - posTx.TransactionDate)))
                    {
                        mut = (ISecurityValuationMutation)mutation;
                        days = mut.Date - posTx.TransactionDate;
                    }

                    if (days.Days >= 0)
                        return mut;
                }
            }
            if (mut == null)
            {
                // if no match, just take the first the first one
                mut = (ISecurityValuationMutation)mutationsToSave[0];
            }
            return mut;
        }

        private IMonetaryValuationMutation getNearestMutation(ref List<IValuationMutation> mutationsToSave, IJournalEntryLine line)
        {
            IMonetaryValuationMutation mut = null;
            TimeSpan days = TimeSpan.MaxValue;

            foreach (IValuationMutation mutation in mutationsToSave)
            {
                if (line.Currency.Equals(mutation.Instrument))
                {
                    //if (mut != null && ((TimeSpan)(mutation.Date - line.TransactionDate)).Days > 0)
                    //    return mut;

                    if (mut == null || (days < (mutation.Date - line.BookDate)))
                    {
                        mut = (IMonetaryValuationMutation)mutation;
                        days = mut.Date - line.BookDate;
                    }

                    if (days.Days >= 0)
                        return mut;
                }
            }
            if (mut == null)
            {
                // if no match, just take the first the first one
                mut = (IMonetaryValuationMutation)mutationsToSave[0];
            }
            return mut;
        }

        private IValuationCashMutation getNearestMutation(ref IValuationCashMutation[] mutationsToSave, IJournalEntryLine line, int matchlevel)
        {
            IValuationCashMutation mut = null;
            TimeSpan days = TimeSpan.MaxValue;
            string lineKey;
            string mutKey;
            switch (matchlevel)
            {
                case 1:
                    lineKey = string.Format("{0}",
                        line.Currency.Key.ToString());
                    break;
                case 2:
                    lineKey = string.Format("{0}-{1}",
                        line.Currency.Key.ToString(),
                        (line.BookingRelatedInstrument != null ? line.BookingRelatedInstrument.Key.ToString() : "0"));
                    break;
                default: // 3
                    lineKey = string.Format("{0}-{1}-{2}",
                        line.Currency.Key.ToString(),
                        (line.BookingRelatedInstrument != null ? line.BookingRelatedInstrument.Key.ToString() : "0"),
                        line.GLAccount.ValuationCashType.ToString());
                    break;
            }


            foreach (IValuationCashMutation mutation in mutationsToSave)
            {
                switch (matchlevel)
                {
                    case 1:
                        mutKey = string.Format("{0}",
                            mutation.Amount.Underlying.Key.ToString());
                        break;
                    case 2:
                        mutKey = string.Format("{0}-{1}",
                            mutation.Amount.Underlying.Key.ToString(),
                            (mutation.Instrument != null ? mutation.Instrument.Key.ToString() : "0"));
                        break;
                    default: // 3
                        mutKey = string.Format("{0}-{1}-{2}",
                            mutation.Amount.Underlying.Key.ToString(),
                            (mutation.Instrument != null ? mutation.Instrument.Key.ToString() : "0"),
                            mutation.ValuationCashType.ToString());
                        break;
                }

                if (lineKey.Equals(mutKey))
                {
                    //if (mut != null && ((TimeSpan)(mutation.Date - posTx.TransactionDate)).Days > 0)
                    //    return mut;

                    if (mut == null || (days < (mutation.Date - line.BookDate)))
                    {
                        mut = mutation;
                        days = mut.Date - line.BookDate;
                    }

                    if (days.Days >= 0)
                        return mut;
                }
            }
            return mut;
        }

        #endregion

        private void saveMutations(IDalSession session, ref List<IValuationMutation> mutationsToSave, ref IValuationCashMutation[] cashMutationsToSave)
        {
            if (mutationsToSave != null && mutationsToSave.Count > 0 && cashMutationsToSave != null && cashMutationsToSave.Length > 0)
            {
                object[] mutations = new object[mutationsToSave.Count + cashMutationsToSave.Length];
                mutationsToSave.ToArray().CopyTo(mutations, 0);
                cashMutationsToSave.CopyTo(mutations, mutationsToSave.Count);
                session.Insert(mutations);
            }
            else if (mutationsToSave != null && mutationsToSave.Count > 0)
            {
                session.Insert(mutationsToSave);
            }
            else if (cashMutationsToSave != null && cashMutationsToSave.Length > 0)
            {
                session.Insert(cashMutationsToSave);
            }
        }

        private void setAccountValidityDate(IDalSession session, IAccountTypeCustomer account, DateTime maxValuationDate)
        {
            account.ValuationMutationValidityDate = maxValuationDate;
            session.Update(account);
        }

        private IList<IFundPosition> getPositionsConvertedInstrument(IInstrument instrument, IFundPortfolio portfolio)
        {
            IList<IFundPosition> positions = null;
            if (portfolio != null)
            {
                foreach (IFundPosition pos in portfolio)
                {
                    if (pos.InstrumentOfPosition.ParentInstrument != null && pos.InstrumentOfPosition.ParentInstrument.Equals(instrument))
                    {
                        if (positions == null)
                            positions = new List<IFundPosition>();
                        positions.Add(pos);
                    }
                }
            }
            return positions;
        }

        private Price findRelevantPrice(ref IList prices, IInstrument instrument, DateTime date)
        {
            foreach (IPriceDetail histPrice in prices)
            {
                if (histPrice.Date.Equals(date) && histPrice.Price.Instrument.Equals(instrument))
                    return histPrice.Price;
            }
            // Nothing found -> return zero price for Tradeable instrument
            // return 1 for currencies
            if (instrument.IsTradeable)
                return new Price(0M, ((ITradeableInstrument)instrument).CurrencyNominal, instrument);
            else
                return new Price(1M, (ICurrency)instrument, instrument);
        }

        #endregion

        #region Daily Valuation Stuff

        public int RunDailyValuations(IDalSessionFactory factory, BackgroundWorker worker, DoWorkEventArgs e)
        {
            int successCount = 0;

            try
            {
                IDalSession session = factory.CreateSession();
                valuationRunner = new ValuationRunner(MaxValuationDate, ConcurrentStoredProceduresCount, session);
                valuationRunner.HasFinished += new EventHandler(valuationRunner_HasFinished);
                valuationRunner.ReportProgress += new ValuationRunner.ReportProgressEventHandler(valuationRunner_ReportProgress);
                valuationRunner.Worker = worker;
                valuationRunner.WorkerEvent = e;
                valuationRunner.Run();

                while (valuationRunner != null && valuationRunner.IsRunning)
                {
                    Thread.Sleep(2000);
                }

                if (valuationRunner != null)
                {
                    Debug.Print(string.Format("{0} - The end", DateTime.Now));
                    successCount = valuationRunner.TotalNrAccountsProcessed;
                    valuationRunner.Dispose();
                    valuationRunner = null;
                }
            }
            catch (Exception ex)
            {
                e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Exception, "An error occured during the daily valuation job", "", ex);
                worker.ReportProgress(100);
                Debug.Print("Error in RunDailyValuations");
            }
            //finally
            //{
            //    worker.ReportProgress(100);
            //}
            return successCount;
        }

        private void valuationRunner_ReportProgress(object sender, ReportProgressEventArgs e)
        {
            if (valuationRunner != null && valuationRunner.Worker != null)
            {
                int successCount = valuationRunner.TotalNrAccountsProcessed;
                if (valuationRunner.Worker.CancellationPending)
                {
                    valuationRunner.Stop();
                    valuationRunner.WorkerEvent.Cancel = true;
                    string result = string.Format("The Daily Valuation Job was cancelled, {0} accounts were valuated successfully", successCount);
                    valuationRunner.WorkerEvent.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, successCount.ToString(), result);
                }
                else if (e.ErrCount > 99)
                {
                    valuationRunner.Stop();
                    valuationRunner.WorkerEvent.Cancel = true;
                    string result = string.Format("The Daily Valuation Job was cancelled, to many failures, {0} accounts were valuated successfully", successCount);
                    valuationRunner.WorkerEvent.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, successCount.ToString(), result);
                }
                else
                    valuationRunner.Worker.ReportProgress(e.Progress);
            }
        }

        private void valuationRunner_HasFinished(object sender, EventArgs e)
        {
            if (valuationRunner != null)
            {
                if (valuationRunner.WorkerEvent != null)
                {
                    string errResult = "";
                    if (valuationRunner.ErrorCount > 0)
                        errResult = string.Format(", {0} accounts failed to valuate successfully", valuationRunner.ErrorCount);
                    string result = string.Format("{0} accounts were valuated (daily) successfully{1}", valuationRunner.TotalNrAccountsProcessed, errResult);
                    valuationRunner.WorkerEvent.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Ok, valuationRunner.TotalNrAccountsProcessed.ToString(), result);
                }
                
                if (valuationRunner.Worker != null)
                {
                    valuationRunner.Worker.ReportProgress(100);
                    Debug.Print("Finished 100%");
                }
            }
        }

        public bool RunDailyValuationsForAccount(IDalSession session, IAccountTypeCustomer account, DateTime lastValuationDate, out string message)
        {
            bool success = false;
            message = "";
            try
            {
                SqlCommand command = new SqlCommand("TG_CreateValuationsForAccount", (SqlConnection)session.Connection);
                SqlParameter param1 = new SqlParameter("AccountID", account.Key);
                command.Parameters.Add(param1);
                SqlParameter param2 = new SqlParameter("lastValuationDate", (Util.IsNullDate(lastValuationDate) ? null : lastValuationDate.ToString("yyyy-MM-dd")));
                command.Parameters.Add(param2);
                SqlParameter param3 = new SqlParameter("success", success);
                param3.SqlDbType = SqlDbType.Bit;
                param3.Direction = ParameterDirection.Output;
                command.Parameters.Add(param3);
                SqlParameter param4 = new SqlParameter("errMessage", message);
                param4.SqlDbType = SqlDbType.VarChar;
                param4.Size = 2000;
                param4.Direction = ParameterDirection.Output;
                command.Parameters.Add(param4);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.ExecuteNonQuery();

                if (!((System.Data.SqlTypes.SqlString)param4.SqlValue).IsNull)
                    message = param4.SqlValue.ToString();
                success = ((System.Data.SqlTypes.SqlBoolean)param3.SqlValue).IsTrue;
            }
            catch (Exception ex)
            {
                message = Util.GetMessageFromException(ex);
                string logMessage = string.Format("Error in RunDailyValuationsForAccount -> account: {0}; {1}", account.Key, message);
                log.Error(logMessage);
            }
            return success;
        }

        #endregion

        #region Avg Holding Stuff

//        public int RunAverageHoldings(IDalSessionFactory factory, BackgroundWorker worker, DoWorkEventArgs e)
//        {
//            int counter = 0;
//            int successCount = 0;
//            int errCount = 0;
//            Hashtable errors = new Hashtable();
//            DateTime lastAvgHldDate;
//            string hqlAccounts;

//            // get the date until we do the avg holding calculations
//            if (Util.IsNotNullDate(LastAvgHoldingDate) || Util.IsLastDayOfQuarter(MaxValuationDate))
//            {
//                if (Util.IsNotNullDate(LastAvgHoldingDate))
//                    lastAvgHldDate = LastAvgHoldingDate;
//                else if (Util.IsLastDayOfQuarter(MaxValuationDate))
//                    lastAvgHldDate = MaxValuationDate;
//                else
//                    throw new ApplicationException("Errorin RunAverageHoldings");

//                hqlAccounts = @"select A.Key 
//                    from AccountTypeCustomer A 
//                    where A.Key in (
//                        select A.Key
//                        from Valuation V
//                        left join V.AverageHolding AH
//                        inner join V.Account A, AccountTypeCustomer AC 
//                        where A.Key = AC.Key
//                        and V.Date >= AC.ManagementStartDate
//                        and (V.Date <= :lastAvgHldDate)
//                        and (AC.ManagementEndDate = :nullDate or V.Date <= AC.ManagementEndDate)
//                        and (AH is null or AH.IsDirty = 1))
//                    order by A.Key";
//            }
//            else
//            {
//                lastAvgHldDate = MaxValuationDate;
//                hqlAccounts = @"select A.Key 
//                    from AccountTypeCustomer A 
//                    where A.Key in (
//                        select A.Key
//                        from Valuation V
//                        left join V.AverageHolding AH
//                        inner join V.Account A, AccountTypeCustomer AC 
//                        where A.Key = AC.Key
//                        and V.Date between AC.ManagementStartDate and AC.ManagementEndDate
//                        and (AC.ManagementEndDate > :nullDate and AC.ManagementEndDate <= :lastAvgHldDate)
//                        and (AH is null or AH.IsDirty = 1))
//                    order by A.Key";
//            }

//            try
//            {
//                IDalSession session = factory.CreateSession();
//                string result;
//                int progress = 0;

//                // set session timeout 
//                session.TimeOut = 0;
//                session.LockMode = LockModes.None;

//                Hashtable parameters = new Hashtable();
//                parameters.Add("lastAvgHldDate", lastAvgHldDate);
//                parameters.Add("nullDate", DateTime.NullValue);

//                IList accountKeys = session.GetListByHQL(hqlAccounts, parameters);
//                session.Close();
//                if (accountKeys != null && accountKeys.Count > 0)
//                {
//                    foreach (int accountID in accountKeys)
//                    {
//                        session = factory.CreateSession();
//                        string hqlAccount = "from AccountTypeCustomer A where A.Key = :accountID";
//                        Hashtable parameters2 = new Hashtable();
//                        parameters2.Add("accountID", accountID);
//                        IList accounts = session.GetListByHQL(hqlAccount, parameters2);
//                        if (accounts != null && accounts.Count == 1)
//                        {
//                            string message;
//                            IAccountTypeCustomer account = (IAccountTypeCustomer)accounts[0];
//                            DateTime date;
//                            if (Util.IsNotNullDate(account.ManagementEndDate) && account.ManagementEndDate < lastAvgHldDate)
//                                date = account.ManagementEndDate;
//                            else
//                                date = lastAvgHldDate;

//                            bool success = RunAverageHoldingsForAccount(session, account, date, out message);
//                            session.Close();
//                            if (success)
//                                successCount++;
//                            else
//                            {
//                                if (errors.ContainsKey(message))
//                                    errors[message] = ((int)errors[message]) + 1;
//                                else
//                                    errors.Add(message, 1);
//                                errCount++;
//                            }
//                            counter++;
//                        }

//                        if (worker.CancellationPending)
//                        {
//                            e.Cancel = true;
//                            result = string.Format("The Average Holding Job was cancelled, {0} accounts were valuated successfully {1}", successCount.ToString(), getFailureMessage(errors, errCount));
//                            e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, successCount.ToString(), result);
//                            return successCount;
//                        }
//                        else if (errCount > 99)
//                        {
//                            e.Cancel = true;
//                            result = string.Format("The Average Holding Job was cancelled, to many failures, {0} accounts were valuated successfully {1}", successCount.ToString(), getFailureMessage(errors, errCount));
//                            e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, successCount.ToString(), result);
//                            return successCount;
//                        }
//                        else
//                        {
//                            if (progress != ((counter * 100) / accountKeys.Count) && progress < 99)
//                            {
//                                progress = ((counter * 100) / accountKeys.Count);
//                                worker.ReportProgress(progress);
//                            }
//                        }
//                    }
//                }
//                result = string.Format("{0} accounts were averaged successfully {1}", successCount.ToString(), getFailureMessage(errors, errCount));
//                e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Ok, successCount.ToString(), result);
//            }
//            catch (Exception ex)
//            {
//                e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Exception, "An error occured during the Average Holding job", "", ex);
//                successCount = 0;
//            }
//            finally
//            {
//                worker.ReportProgress(100);
//            }
//            return successCount;
//        }

        public bool RunAverageHoldingsForAccount(IDalSession session, IAccountTypeCustomer account, DateTime lastAvgHoldingDate, out string message)
        {
            bool success = false;
            message = "";
            try
            {
                SqlCommand command = new SqlCommand("TG_CreateAverageHoldingsForAccount", (SqlConnection)session.Connection);
                SqlParameter param1 = new SqlParameter("AccountID", account.Key);
                command.Parameters.Add(param1);
                SqlParameter param2 = new SqlParameter("endDate", lastAvgHoldingDate);
                command.Parameters.Add(param2);
                SqlParameter param3 = new SqlParameter("success", success);
                param3.SqlDbType = SqlDbType.Bit;
                param3.Direction = ParameterDirection.Output;
                command.Parameters.Add(param3);
                SqlParameter param4 = new SqlParameter("errMessage", message);
                param4.Direction = ParameterDirection.Output;
                command.Parameters.Add(param4);
                param4.SqlDbType = SqlDbType.VarChar;
                param4.Size = 2000;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.ExecuteNonQuery();

                if (!((System.Data.SqlTypes.SqlString)param4.SqlValue).IsNull)
                    message = param4.SqlValue.ToString();
                success = ((System.Data.SqlTypes.SqlBoolean)param3.SqlValue).IsTrue;
            }
            catch (Exception ex)
            {
                message = Util.GetMessageFromException(ex);
                string logMessage = string.Format("Error in RunAverageHoldingsForAccount -> account: {0}; {1}", account.Key, message);
                log.Error(logMessage);
            }
            return success;
        }

        public int RunAverageHoldings(IDalSessionFactory factory, BackgroundWorker worker, DoWorkEventArgs e)
        {
            int successCount = 0;

            try
            {
                IDalSession session = factory.CreateSession();
                bool success;
                averageHoldingRunner = new AverageHoldingRunner(MaxValuationDate, ConcurrentStoredProceduresCount, session, out success);
                if (success)
                {
                    averageHoldingRunner.HasFinished += new EventHandler(averageHoldingRunner_HasFinished);
                    averageHoldingRunner.ReportProgress += new AverageHoldingRunner.ReportProgressEventHandler(averageHoldingRunner_ReportProgress);
                    averageHoldingRunner.Worker = worker;
                    averageHoldingRunner.WorkerEvent = e;
                    averageHoldingRunner.Run();

                    while (averageHoldingRunner != null && averageHoldingRunner.IsRunning)
                    {
                        Thread.Sleep(2000);
                    }

                    if (averageHoldingRunner != null)
                    {
                        Debug.Print(string.Format("{0} - The end", DateTime.Now));
                        if (averageHoldingRunner != null)
                        {
                            successCount = averageHoldingRunner.TotalNrAccountsProcessed;
                            averageHoldingRunner.Dispose();
                            averageHoldingRunner = null;
                        }
                    }
                }
                else
                    worker.ReportProgress(100);
            }
            catch (Exception ex)
            {
                e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Exception, "An error occured during the daily valuation job", "", ex);
                Debug.Print("Error in RunDailyValuations");
            }
            //finally
            //{
            //    worker.ReportProgress(100);
            //}
            return successCount;
        }

        private void averageHoldingRunner_ReportProgress(object sender, ReportProgressEventArgs e)
        {
            if (averageHoldingRunner != null && averageHoldingRunner.Worker != null)
            {
                int successCount = averageHoldingRunner.TotalNrAccountsProcessed;
                if (averageHoldingRunner.Worker.CancellationPending)
                {
                    averageHoldingRunner.Stop();
                    averageHoldingRunner.WorkerEvent.Cancel = true;
                    string result = string.Format("The Average Holding Job was cancelled, {0} accounts were valuated successfully", successCount);
                    averageHoldingRunner.WorkerEvent.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, successCount.ToString(), result);
                }
                else if (e.ErrCount > 99)
                {
                    averageHoldingRunner.Stop();
                    averageHoldingRunner.WorkerEvent.Cancel = true;
                    string result = string.Format("The Average Holding Job was cancelled, to many failures, {0} accounts created average holdings successfully", successCount);
                    averageHoldingRunner.WorkerEvent.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, successCount.ToString(), result);
                }
                else
                    averageHoldingRunner.Worker.ReportProgress(e.Progress);
            }
        }

        private void averageHoldingRunner_HasFinished(object sender, EventArgs e)
        {
            if (averageHoldingRunner != null)
            {
                if (averageHoldingRunner.WorkerEvent != null)
                {
                    string errResult = "";
                    if (averageHoldingRunner.ErrorCount > 0)
                        errResult = string.Format(", {0} accounts failed to create avg holdings successfully", averageHoldingRunner.ErrorCount);
                    string result = string.Format("{0} accounts were create avg holdings successfully{1}", averageHoldingRunner.TotalNrAccountsProcessed, errResult);
                    averageHoldingRunner.WorkerEvent.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Ok, averageHoldingRunner.TotalNrAccountsProcessed.ToString(), result);
                }

                if (averageHoldingRunner.Worker != null)
                {
                    averageHoldingRunner.Worker.ReportProgress(100);
                    Debug.Print("Finished 100%");
                }
            }
        }


        #endregion

        #region Fees Stuff

        public int RunCalculateFees(IDalSessionFactory factory, BackgroundWorker worker, DoWorkEventArgs e)
        {
            int counter = 0;
            int successCount = 0;
            int errCount = 0;
            Hashtable errors = new Hashtable();
            try
            {
                IDalSession session = factory.CreateSession();
                FeeFactory feeFactory = null;
                string result;
                int progress = 0;

                // set session timeout 
                session.TimeOut = 0;
                session.LockMode = LockModes.None;

                IList<int> unitKeys = session.GetTypedListByNamedQuery<int>(
                    "B4F.TotalGiro.Valuations.GetNotCalculatedManagementPeriodUnits");
                session.Close();

                if (unitKeys != null && unitKeys.Count > 0)
                {
                    feeFactory = FeeFactory.GetInstance(factory.CreateSession(), FeeFactoryInstanceTypes.Fee, true);
                    foreach (int unitID in unitKeys)
                    {
                        session = factory.CreateSession();
                        string hqlUnit = "from ManagementPeriodUnit U where U.Key = :unitID";
                        Hashtable parameters = new Hashtable();
                        parameters.Add("unitID", unitID);
                        IList<ManagementPeriodUnit> units = session.GetTypedListByHQL<ManagementPeriodUnit>(hqlUnit, parameters);
                        if (units != null && units.Count == 1)
                        {
                            string message;
                            IManagementPeriodUnit unit = units[0];

                            bool success = runCalculateFeesForUnit(session, feeFactory, unit, out message);
                            session.Close();
                            if (success)
                                successCount++;
                            else
                            {
                                if (errors.ContainsKey(message))
                                    errors[message] = ((int)errors[message]) + 1;
                                else
                                    errors.Add(message, 1);
                                errCount++;
                            }
                            counter++;
                        }

                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            result = string.Format("The Fee Calculation Job was cancelled, {0} units were calculated successfully {1}", successCount.ToString(), getFailureMessage(errors, errCount));
                            e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, successCount.ToString(), result);
                            return successCount;
                        }
                        else if (errCount > 99)
                        {
                            e.Cancel = true;
                            result = string.Format("The Fee Calculation Job was cancelled, to many failures, {0} units were calculated successfully {1}", successCount.ToString(), getFailureMessage(errors, errCount));
                            e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Cancelled, successCount.ToString(), result);
                            return successCount;
                        }
                        else
                        {
                            if (progress != ((counter * 100) / unitKeys.Count) && progress < 99)
                            {
                                progress = ((counter * 100) / unitKeys.Count);
                                worker.ReportProgress(progress);
                            }
                        }
                    }
                }
                result = string.Format("{0} units were calculated successfully {1}", successCount.ToString(), getFailureMessage(errors, errCount));
                e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Ok, successCount.ToString(), result);
            }
            catch (Exception ex)
            {
                e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Exception, "An error occured during the average holdings fee job", "", ex);
                successCount = 0;
            }
            finally
            {
                worker.ReportProgress(100);
            }
            return successCount;
        }

        public void RunCalculateFeesForUnit(IDalSession session, IManagementPeriodUnit unit)
        {
            string message;
            FeeFactory feeFactory = FeeFactory.GetInstance(session, FeeFactoryInstanceTypes.Fee);
            bool success = runCalculateFeesForUnit(session, feeFactory, unit, out message);
            if (!success)
                throw new ApplicationException(message);
        }

        private bool runCalculateFeesForUnit(IDalSession session, FeeFactory feeFactory, IManagementPeriodUnit unit, out string message)
        {
            bool success = false;
            message = "";
            try
            {
                switch (unit.ManagementPeriod.ManagementType)
                {
                    case B4F.TotalGiro.Accounts.ManagementPeriods.ManagementTypes.ManagementFee:
                        unit.Success = feeFactory.CalculateFeePerUnit(session, unit);
                        break;
                    case B4F.TotalGiro.Accounts.ManagementPeriods.ManagementTypes.KickBack:
                        unit.Success = calculateKickBackOnUnit(session, unit, out message);
                        break;
                }
                if (!string.IsNullOrEmpty(message))
                    unit.Message = message;
                success = session.Update(unit);
            }
            catch (Exception ex)
            {
                message = Util.GetMessageFromException(ex);
                string logMessage = string.Format("Error in runCalculateFeesForUnit -> unit: {0}; {1}", unit.Key, message);
                log.Error(logMessage);
            }
            return success;
        }

        private bool calculateKickBackOnUnit(IDalSession session, IManagementPeriodUnit unit, out string message)
        {
            if (this.feeTypeKickBack == null)
                this.feeTypeKickBack = (FeeType)session.GetObjectInstance(typeof(FeeType), (int)FeeTypes.KickbackFee);
            return FeeFactory.CalculateKickBackOnUnit(session, unit, this.feeTypeKickBack, out message);
        }

        #endregion

        #region AgentWorker

        public override WorkerResult Run(IDalSessionFactory factory, BackgroundWorker worker, DoWorkEventArgs e)
        {
            int i = 0;
            switch (Job)
            {
                case ValuationEngineJobs.ValuationMutations:
                    i = RunValuationMutations(factory, worker, e);
                    break;
                case ValuationEngineJobs.DailyValuations:
                    i = RunDailyValuations(factory, worker, e);
                    break;
                case ValuationEngineJobs.AverageHoldings:
                    i = RunAverageHoldings(factory, worker, e);
                    break;
                case ValuationEngineJobs.Fees:
                    i = RunCalculateFees(factory, worker, e);
                    break;
            }
            return (WorkerResult)e.Result;
        }

        public ValuationEngineJobs Job
        {
            get { return job; }
            set { job = value; }
        }

        public bool RunSP_ResetValuations
        {
            get { return runSP; }
            set { runSP = value; }
        }

        public short DaysDelay
        {
            get { return daysDelay; }
            set { daysDelay = value; }
        }

        public short ConcurrentStoredProceduresCount
        {
            get { return concurrentStoredProceduresCount; }
            set { concurrentStoredProceduresCount = value; }
        }

        public DateTime MaxValuationDate
        {
            get { return DateTime.Now.Date.AddDays(Math.Abs(DaysDelay) * -1); }
        }

        public DateTime LastAvgHoldingDate
        {
            get { return lastAvgHoldingDate; }
            set 
            {
                if (value.Equals(Util.GetLastDayOfMonth(value)))
                    lastAvgHoldingDate = value;
                else
                    lastAvgHoldingDate = Util.GetLastDayOfMonth(value.AddMonths(-1));
            }
        }

        #region Privates

        private ValuationEngineJobs job = ValuationEngineJobs.ValuationMutations;
        private ValuationRunner valuationRunner;
        private AverageHoldingRunner averageHoldingRunner;
        private string startCharacter = string.Empty;
        private string endCharacter = string.Empty;
        private bool runSP;
        private short daysDelay = 7;
        private short concurrentStoredProceduresCount = 4;
        private DateTime lastAvgHoldingDate = DateTime.MinValue;

        #endregion

        #endregion

        #region Privates

        private FeeType feeTypeKickBack;
        private static readonly ILog log = LogManager.GetLogger("System");

        #endregion

    }
}

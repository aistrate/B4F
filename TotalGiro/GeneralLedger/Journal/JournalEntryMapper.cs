using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using NHibernate.Criterion;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public static class JournalEntryMapper
    {
        public static IList<T> GetJournalEntries<T>(IDalSession session, JournalTypes journalType, IJournal journal, DateTime transactionDate, JournalEntryStati statuses)
            where T : IJournalEntry
        {
            return GetJournalEntries<T>(session, journalType, journal, transactionDate, DateTime.MinValue, null, statuses, false);
        }

        public static IList<T> GetJournalEntries<T>(IDalSession session, JournalTypes journalType, IJournal journal, DateTime transactionDateFrom,
                                              DateTime transactionDateTo, string journalEntryNumber, JournalEntryStati statuses)
            where T : IJournalEntry
        {
            return GetJournalEntries<T>(session, journalType, journal, transactionDateFrom, transactionDateTo, journalEntryNumber, statuses, false);
        }

        public static IList<T> GetJournalEntries<T>(IDalSession session, JournalTypes journalType, IJournal journal, DateTime transactionDateFrom,
            DateTime transactionDateTo, string journalEntryNumber, JournalEntryStati statuses,
            bool showNullTransactionDates)
            where T : IJournalEntry
        {
            Hashtable parameters = new Hashtable();
            string hql = string.Format("FROM {0} S WHERE ((1 = 1", getType(journalType).Name);


            if (transactionDateFrom != DateTime.MinValue)
            {
                parameters.Add("TransactionDateFrom", transactionDateFrom);
                hql += " AND S.TransactionDate >= :TransactionDateFrom";
            }
            if (transactionDateTo != DateTime.MinValue)
            {
                parameters.Add("TransactionDateTo", transactionDateTo);
                hql += " AND S.TransactionDate <= :TransactionDateTo";
            }
            hql += ")";

            if (showNullTransactionDates)
                hql += " OR S.TransactionDate IS NULL";

            hql += ")";

            if (journal != null)
            {
                parameters.Add("Journal", journal);
                hql += " AND S.Journal = :Journal";
            }

            if (journalEntryNumber != null)
            {
                parameters.Add("JournalEntryNumber", journalEntryNumber);
                hql += " AND S.JournalEntryNumber = :JournalEntryNumber";
            }

            bool showOnlyLegalStatuses = (statuses != JournalEntryStati.None);
            string statusList = "";
            foreach (JournalEntryStati status in Enum.GetValues(typeof(JournalEntryStati)))
                if (status != JournalEntryStati.None &&
                    (showOnlyLegalStatuses && ((statuses & status) == status) || !showOnlyLegalStatuses))
                    statusList += (statusList != string.Empty ? ", " : "") + ((int)status).ToString();

            hql += string.Format(" AND S.Status {0} IN ({1})", (showOnlyLegalStatuses ? "" : "NOT"), statusList);

            return session.GetTypedListByHQL<T>(hql, parameters);
        }

        public static IList<IJournalEntry> GetJournalEntries(IDalSession session, int journalEntryId)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", journalEntryId));
            return session.GetTypedList<JournalEntry, IJournalEntry>(expressions);
        }

        public static IList<IJournalEntry> GetJournalEntries(IDalSession session, JournalTypes journalType, IJournal journal, JournalEntryStati status)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            Type type = null;
            switch (journalType)
            {
                case JournalTypes.BankStatement:
                    type = typeof(BankStatement);
                    break;
                case JournalTypes.Memorial:
                    type = typeof(MemorialBooking);
                    break;
                case JournalTypes.ClientTransaction:
                    type = typeof(TradingJournalEntry);
                    break;
            }
            expressions.Add(Expression.Eq("Journal", journal));
            expressions.Add(Expression.In("Status", status.ToEnumArray()));
            return NHSession.ToList<IJournalEntry>(session.GetList(type, expressions));
        }

        public static IList<IJournalEntryLine> GetJournalEntriesfromTransaction(IDalSession session, int totalGiroTradeID)
        {
            Hashtable parameters = new Hashtable();

            string hql = @"from JournalEntryLine J
                        where J.TotalGiroTradeDetails.Key = :totalGiroTradeID";

            parameters.Add("totalGiroTradeID", totalGiroTradeID);
            return session.GetTypedListByHQL<IJournalEntryLine>(hql, parameters);
        }

        public static IList<IJournalEntryLine> GetClientJournalEntryLines(
            IDalSession session, IAccountTypeCustomer account, DateTime beginDate, DateTime endDate, bool displayAll)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("baseCurrencyId", account.AccountOwner.StichtingDetails.BaseCurrency.Key);
            parameters.Add("accountId", account.Key);
            parameters.Add("statusBooked", (int)JournalEntryLineStati.Booked);
            parameters.Add("beginDate", beginDate);
            parameters.Add("endDate", endDate);
            if (!displayAll)
                parameters.Add("hideStornos", 1);

            return session.GetTypedListByNamedQuery<IJournalEntryLine>(
                "B4F.TotalGiro.GeneralLedger.Journal.GetClientJournalEntryLines",
                parameters);
        }

        public static IJournalEntryLine GetJournalEntryLine(IDalSession session, int journalEntryLineId)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", journalEntryLineId));
            IList lines = session.GetList(typeof(JournalEntryLine), expressions);
            return (lines != null && lines.Count == 1 ? (IJournalEntryLine)lines[0] : null);
        }

        public static IJournalEntry GetJournalEntry(IDalSession session, int journalEntryId)
        {
            IList<IJournalEntry> journalEntries = GetJournalEntries(session, journalEntryId);
            return (journalEntries != null && journalEntries.Count == 1 ? journalEntries[0] : null);
        }

        public static string GetNextJournalEntryNumber(IDalSession session, IJournal journal)
        {
            StringBuilder prefix = new StringBuilder();
            switch (journal.JournalType)
            {
                case JournalTypes.BankStatement:
                    prefix.Append("B-");
                    break;
                case JournalTypes.Memorial:
                    prefix.Append("M-");
                    break;
                case JournalTypes.ClientTransaction:
                    prefix.Append("T-");
                    break;
                default:
                    prefix.Append("B-");
                    break;
            }
            prefix.Append(journal.JournalNumber.PadLeft(4, '0')).Append("-");
            prefix.Append(DateTime.Now.Year.ToString()).Append("-");


            string sqlQuery = "SELECT TOP 1 {E.*} FROM GLJournalEntries E WHERE left(E.JournalEntryNumber,12) = '" + prefix + "' ORDER BY E.JournalEntryNumber DESC";
            IList journalEntries = session.GetListbySQLQuery(sqlQuery, "E", typeof(JournalEntry));

            if (journalEntries.Count > 0)
            {
                prefix.Append((int.Parse((((IJournalEntry)journalEntries[0]).JournalEntryNumber).Substring(12, 8)) + 1).ToString("00000000"));

            }
            else
                prefix.Append("00000001");

            return prefix.ToString();
        }

        //public static string GetNextJournalEntryNumber(IDalSession session, GLTransactionStages GLTransactionStage)
        //{
        //    int GLTransactionStageID = (int)GLTransactionStage;
        //    string prefix;
        //    string sqlQuery = "SELECT TOP 1 {E.*} FROM GLJournalEntries E WHERE E.JournalEntryTypeID in (3,4,5) ORDER BY E.JournalEntryNumber DESC";
        //    IList journalEntries = session.GetListbySQLQuery(sqlQuery, "E", typeof(JournalEntry));

        //    switch (GLTransactionStage)
        //    {
        //        case GLTransactionStages.Unsettled:
        //            prefix = "U";
        //            break;
        //        case GLTransactionStages.ClientSettled:
        //            prefix = "C";
        //            break;
        //        case GLTransactionStages.ExternalSettled:
        //            prefix = "E";
        //            break;
        //        case GLTransactionStages.Storno:
        //            prefix = "S";
        //            break;
        //        default:
        //            prefix = "U";
        //            break;
        //    }


        //    if (journalEntries.Count > 0)
        //        return prefix + (int.Parse(((IJournalEntry)journalEntries[0]).JournalEntryNumber.Substring(1)) + 1).ToString("0000000000");
        //    else
        //        return prefix + "0000000001";
        //}

        public static void Insert(IDalSession session, IJournalEntry obj)
        {
            Update(session, obj);
            //return blnSuccess;
            //session.Insert(obj); ;
        }

        public static ITradingJournalEntry GetNewJournalEntryFromOld(IDalSession session, IJournalEntry oldEntry, DateTime transactionDate)
        {
            IJournal oldJournal = oldEntry.Journal;
            string nextJournalNumber = GetNextJournalEntryNumber(session, oldJournal);
            return new TradingJournalEntry(oldJournal, nextJournalNumber, transactionDate);
        }

        public static void Update(IDalSession session, IJournalEntry obj)
        {
            //bool blnSuccess = session.InsertOrUpdate(obj);
            //if (blnSuccess && obj.JournalEntryNumber == null)
            //{
            //    obj.SetJournalEntryNumber();
            //    blnSuccess = session.InsertOrUpdate(obj);
            //}
            session.InsertOrUpdate(obj); ;
        }

        public static IJournalEntry GetLastJournalEntry(IDalSession session, IJournal journal)
        {
            string queryName;
            Hashtable parameters = new Hashtable();

            if (journal != null)
            {
                queryName = "B4F.TotalGiro.GeneralLedger.Journal.GetLastJournalEntriesWithJournal";
                parameters.Add("journalId", journal.Key);
            }
            else
                queryName = "B4F.TotalGiro.GeneralLedger.Journal.GetLastJournalEntriesWithUnKnownJournal";

            IList<IJournalEntry> journalEntries = session.GetTypedListByNamedQuery<IJournalEntry>(
                queryName, parameters);

            return (journalEntries.Count > 0 ? journalEntries[0] : null);
        }

        public static DateTime GetLastJournalEntryDate(IDalSession session, IJournal journal)
        {
            if (journal == null)
                throw new ApplicationException("Which journal do you mean?");

            Hashtable parameters = new Hashtable();
            parameters.Add("journalId", journal.Key);

            IList<DateTime> dates = session.GetTypedListByNamedQuery<DateTime>(
                "B4F.TotalGiro.GeneralLedger.Journal.GetLastJournalEntryDate", 
                parameters);

            return (dates.Count > 0 ? dates[0] : DateTime.MinValue);
        }

        private static Type getType(JournalTypes journalType)
        {
            Type type;
            switch (journalType)
            {
                case JournalTypes.BankStatement:
                    type = typeof(BankStatement);
                    break;
                case JournalTypes.Memorial:
                    type = typeof(MemorialBooking);
                    break;
                default:
                    type = typeof(JournalEntry);
                    break;
            }
            return type;
        }

        public static IList GetClientTransactionsToSettle(IDalSession session)
        {
            Hashtable parameters = new Hashtable();
            JournalTypes journalType = JournalTypes.ClientTransaction;

            string query = @"B4F.TotalGiro.GeneralLedger.Journal.GetClientTransactionsToSettle";

            parameters.Add("journalType", journalType);
            return session.GetListByNamedQuery(query, parameters);
        }

        public static IList GetTrialBalance(IDalSession session, DateTime transactiondate)
        {
            Hashtable parameters = new Hashtable();
            string hql = @"";


            parameters.Add("transactiondate", transactiondate);
            return session.GetListByHQL(hql, parameters);
        }

//        public static IList GetUnsettledExternalSettlements(IDalSession session)
//        {
//            Hashtable parameters = new Hashtable();

//            JournalTypes journalType = JournalTypes.BankStatement;

//            string hql = @"SELECT JEL.Key
//                        from JournalEntryLine JEL
//                        left join JEL.Parent JE
//                        left join JE.Journal J
//                        left join JEL.GLAccount G
//                        where J.JournalType = :journalType
//                        and G.GLAccountNumber = '1670'
//                        and JEL.Key not in (Select ES.BankSettlement.Key from ExternalSettlement ES)
//                        Order by JEL.Key";

//            parameters.Add("journalType", journalType);
//            return session.GetListByHQL(hql, parameters);
//        }

        public static IList GetStornosToProcess(IDalSession session)
        {
            Hashtable parameters = new Hashtable();

            JournalTypes journalType = JournalTypes.ClientTransaction;

            string query = @"B4F.TotalGiro.GeneralLedger.Journal.GetStornosToProcess";

            parameters.Add("journalType", journalType);
            return session.GetListByNamedQuery(query, parameters);

        }

        public static IList<IJournalEntryLine> GetTradeToSettleExternally(IDalSession session, Money Amount, DateTime TransactionDate)
        {
            Hashtable parameters = new Hashtable();

            JournalTypes journalType = JournalTypes.ClientTransaction;

            //Change to adjust for data
            DateTime newDate = TransactionDate.AddDays(7);

            string query = @"B4F.TotalGiro.GeneralLedger.Journal.GetTradeToSettleExternally";

            parameters.Add("journalType", journalType);
            parameters.Add("transactionDate", newDate);
            parameters.Add("underlying", Amount.Underlying);
            parameters.Add("lower", Decimal.Add(Amount.Abs().Quantity, -0.05m));
            parameters.Add("upper", Decimal.Add(Amount.Abs().Quantity, 0.05m));

            return session.GetTypedListByNamedQuery<IJournalEntryLine>(query, parameters);
        }

        public static IList<IJournalEntryLine> GetUnmatchedBankSettlements(IDalSession session, DateTime startDate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable();

            parameters.Add("journalType", JournalTypes.BankStatement);
            if (Util.IsNotNullDate(startDate))
                parameters.Add("startDate", startDate);
            if (Util.IsNotNullDate(endDate))
                parameters.Add("endDate", endDate);
            return session.GetTypedListByNamedQuery<IJournalEntryLine>(
                "B4F.TotalGiro.GeneralLedger.Journal.GetUnmatchedBankSettlements", 
                parameters);
        }

        //public static IList<int> GetUnmatchedBankSettlementKeys(IDalSession session)
        //{
        //    Hashtable parameters = new Hashtable(1);

        //    parameters.Add("journalType", JournalTypes.BankStatement);
        //    return session.GetTypedListByNamedQuery<int>(
        //        "B4F.TotalGiro.GeneralLedger.Journal.GetUnmatchedBankSettlementKeys", 
        //        parameters);
        //}

        public static IList<IJournalEntryLine> GetUnmatchedBankSettlements(IDalSession session, int[] ids)
        {
            Hashtable parameters = new Hashtable();
            Hashtable parametersList = new Hashtable();

            parameters.Add("journalType", JournalTypes.BankStatement);
            parametersList.Add("ids", ids);
            return session.GetTypedListByNamedQuery<IJournalEntryLine>(
                "B4F.TotalGiro.GeneralLedger.Journal.GetUnmatchedBankSettlementsByIds", 
                parameters, parametersList);

        }

        public static IList<IOrderExecution> GetUnsettledExternalTrades(IDalSession session, DateTime startDate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable();

            if (Util.IsNotNullDate(startDate))
                parameters.Add("startDate", startDate);
            if (Util.IsNotNullDate(endDate))
                parameters.Add("endDate", endDate);

            return session.GetTypedListByNamedQuery<IOrderExecution>(
                "B4F.TotalGiro.Orders.Transactions.Transaction.GetUnsettledExecutions",
                parameters);
        }

        public static IList<IOrderExecution> GetUnsettledExternalTrades(IDalSession session, int[] ids)
        {
            Hashtable parameters = new Hashtable();
            Hashtable parametersList = new Hashtable();

            parametersList.Add("ids", ids);

            return session.GetTypedListByNamedQuery<IOrderExecution>(
                "B4F.TotalGiro.Orders.Transactions.Transaction.GetUnsettledExecutionsById",
                parameters, parametersList);
        }

        public static IList<IJournalEntryLine> GetUnProcessedCashTransfers(IDalSession session, IAccountTypeCustomer account)
        {
            Hashtable parameters = new Hashtable(3);
            parameters.Add("accountID", account.Key);
            parameters.Add("baseCurrency", account.AccountOwner.StichtingDetails.BaseCurrency.Key);
            parameters.Add("statusBooked", JournalEntryStati.Booked);
            return session.GetTypedListByNamedQuery<IJournalEntryLine>(
                "B4F.TotalGiro.GeneralLedger.Journal.UnProcessedCashTransfers",
                parameters);
        }

        public static IList<ITradingJournalEntry> GetUnsettledExternalTradeEntriesByID(IDalSession session, int[] ids)
        {
            Hashtable parameters = new Hashtable();
            Hashtable parametersList = new Hashtable();

            parametersList.Add("ids", ids);

            return session.GetTypedListByNamedQuery<ITradingJournalEntry>(
                "B4F.TotalGiro.GeneralLedger.Journal.GetUnsettledExternalTradeEntriesByID", 
                parameters, parametersList);
        }

        public static IList GetCashDetailGroupings(IDalSession session, int subPositionKey)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("SubPositionKey", subPositionKey);
            return session.GetListByNamedQuery("B4F.TotalGiro.GeneralLedger.Journal.GetCashDetailGroupings", parameters);
        }

        public static IList MigrateExternalSettlements(IDalSession session)
        {
            string query = "B4F.TotalGiro.GeneralLedger.Journal.MigrateExternalSettlements";

            return session.GetListByNamedQuery(query);
        }

        public static IList<int> GetAccountKeysToCloseInBookYear(IDalSession session, IGLBookYear bookYear)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("bookYear", bookYear);
            return session.GetTypedListByNamedQuery<int>(
                "B4F.TotalGiro.GeneralLedger.Journal.GetAccountKeysToCloseInBookYear", parameters);

        }

        public static IList<IJournalEntryLine> GetBookingsToCloseForFinancialYear(IDalSession session, DateTime dateUntil, int accountKey)
        {
            DateTime startOfNextYear = dateUntil.AddDays(1);
            Hashtable parameters = new Hashtable(3);
            parameters.Add("dateUntil", dateUntil);
            parameters.Add("startOfNextYear", startOfNextYear);
            parameters.Add("accountKey", accountKey);
            return session.GetTypedListByNamedQuery<IJournalEntryLine>(
                "B4F.TotalGiro.GeneralLedger.Journal.GetBookingsToCloseForFinancialYear", parameters);

        }

        public static IList<IJournalEntryLine> GetSettledBookingsForAccountUntilDate(IDalSession session, DateTime dateUntil, int accountKey)
        {
            Hashtable parameters = new Hashtable(2);
            parameters.Add("dateUntil", dateUntil);
            parameters.Add("accountKey", accountKey);
            return session.GetTypedListByNamedQuery<IJournalEntryLine>(
                "B4F.TotalGiro.GeneralLedger.Journal.GetSettledBookingsForAccountUntilDate", parameters);

        }

        public static IList<IJournalEntryLine> GetDividendBookingsForPeriod(IDalSession session, DateTime startDate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable(2);
            parameters.Add("startDate", startDate);
            parameters.Add("endDate", endDate);
            return session.GetTypedListByNamedQuery<IJournalEntryLine>(
                "B4F.TotalGiro.GeneralLedger.Journal.GetDividendBookingsForPeriod", parameters);

        }


        

    }
}

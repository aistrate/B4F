using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Utils;
using NHibernate.Criterion;
using NHibernate.Linq;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Valuations.ReportedData;

namespace B4F.TotalGiro.Accounts
{
    #region Enums

    /// <summary>
    /// Used to determine which positions to retrieve
    /// </summary>
    public enum PositionsView
    {
        /// <summary>
        /// Retrieves all positions
        /// </summary>
        All,
        /// <summary>
        /// Retrieves all positions where the size is not zero
        /// </summary>
        NotZero,
        /// <summary>
        /// Retrieves all positions where the size is zero
        /// </summary>
        Zero
    }

    /// <summary>
    /// Used to determine which accounts to retrieve
    /// </summary>
    public enum AccountTypeReturnClass
    {
        /// <summary>
        /// Retrieves all accounts
        /// </summary>
        All,
        /// <summary>
        /// Retrieves all internal accounts
        /// </summary>
        Internal,
        /// <summary>
        /// Retrieves all external accounts 
        /// </summary>
        External
    }

    #endregion

    /// <summary>
    /// This class is used to instantiate Account and Position objects. 
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class AccountMapper
    {
        /// <summary>
        /// This method retrieves an account instance via its bank account number.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="number">The account number to be found.</param>
        /// <returns>A specific instance of a account class</returns>
        public static IAccount GetAccountByNumber(IDalSession session, string number)
        {
            //var query = from account in session.Session.Linq<Account>()
            //            where account.Number == number
            //            select account;
            //return query.First();

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Number", number));
            IList result = session.GetList(typeof(Account), expressions);
            if ((result != null) && (result.Count > 0))
                return (IAccount)result[0];
            else
                return null;
        }

        public static IList<ICustomerAccount> GetAccountsWithCashNoOrders(IDalSession session)
        {
            string hqlMain =        @"from CustomerAccount CA
                                    Where CA.Key in (
                                    Select distinct A.Key
                                    from CashPosition CP
                                    Join CP.SettledPosition SP
                                    Join CP.Account A
                                    where SP.Size.Quantity <> 0.00)";
            IList<ICustomerAccount> main = session.GetTypedListByHQL<CustomerAccount, ICustomerAccount>(hqlMain);

            string hqlOrders = @"Select distinct A.Key
                                from Order O
                                join O.Account A
                                where O.Status in (Select OS.key from OrderStatus OS where OS.IsOpen = 1)
                                Order by A.Key";
            IList<int> orders = session.GetTypedListByHQL<int>(hqlOrders);

            return main
                        .GroupJoin(orders,
                        m => m.Key,
                        o => o,
                        (m, o) => new { accounts = m, orders = o })
                        .Where(o => o.orders.Count() == 0)
                        .OrderBy(a => a.accounts.Number)
                        .Select(a => a.accounts)
                        .ToList();
        }

        /// <summary>
        /// This method retrieves an account instance by its unique identifier. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="id">The unique identifier of the account</param>
        /// <returns>An Instance of an account class</returns>
        public static IAccount GetAccount(IDalSession session, int id)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", id));
            IList accounts = session.GetList(typeof(Account), expressions);
            if (accounts != null && accounts.Count == 1)
                return (IAccount)accounts[0];
            else
                return null;
        }

        /// <summary>
        /// This method retrieves a list of account instances that comply to a specific role and to a certain owner. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="type">The role of the account</param>
        /// <param name="accountOwner">The owner of the account. In the case of a customer the owner is an asset manager. In the case of an asset manager the owner is the stichting</param>
        /// <returns>A list of account instances</returns>
        public static IList<T> GetAccounts<T>(IDalSession session, AccountTypes type, IManagementCompany accountOwner)
            where T : IAccount
        {
            List<ICriterion> expressions = new List<ICriterion>();
            if (accountOwner == null)
            {
                accountOwner = LoginMapper.GetCurrentManagmentCompany(session);
            }
            expressions.Add(Expression.Eq("AccountOwner.Key", accountOwner.Key));
            return NHSession.ToList<T>(session.GetList(getType(type), expressions));
        }

        /// <summary>
        /// This method retrieves a list of account instances that comply to a specific role and that meet the TotalGiro's user security setting. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="type">The role of the account</param>
        /// <param name="option">The security option of the user of the system</param>
        /// <returns>A list of account instances</returns>
        public static IList<T> GetAccounts<T>(IDalSession session, AccountTypes type, SecurityInfoOptions option)
            where T : IAccount
        {
            List<ICriterion> expressions = new List<ICriterion>();
            LoginMapper.GetSecurityInfo(session, expressions, option);
            return NHSession.ToList<T>(session.GetList(getType(type), expressions));
        }

        /// <summary>
        /// This method retrieves a list of customeraccount instances that meet the passed in arguments. 
        /// When an argument is ommitted it is not used to filter the accounts
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="assetManager">The asset manager the customer belongs to</param>
        /// <param name="accountNumber">The account's number of the account</param>
        /// <param name="accountName">The name of the account</param>
        /// <returns>A list of AccountTypeInternal (customer) instances</returns>
        public static IList<IAccountTypeCustomer> GetCustomerAccounts(IDalSession session, int assetManagerId, string accountNumber, string accountName)
        {
            return GetCustomerAccounts(session, assetManagerId, 0, accountNumber, accountName, false);
        }

        public static IList<IAccountTypeCustomer> GetCustomerAccounts(IDalSession session, int assetManagerId, int modelId, string accountNumber,
            string accountName, bool accountsWithModelOnly)
        {
            return GetCustomerAccounts(session, assetManagerId, modelId, accountNumber, accountName, false, true);
        }

        public static IList<IAccountTypeCustomer> GetCustomerAccounts(IDalSession session, int assetManagerId, int modelId, string accountNumber,
                                                string accountName, bool accountsWithModelOnly, bool retrieveNostroAccounts)
        {
            return GetCustomerAccounts(session, assetManagerId, 0, 0, 0, modelId, accountNumber, accountName, accountsWithModelOnly,
                                       retrieveNostroAccounts, true, false, 0, true, false);
        }

        /// <summary>
        /// This method retrieves a list of customeraccount instances that meet the passed in arguments. 
        /// When an argument is ommitted it is not used to filter the accounts
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="assetManagerId">The asset manager the customer belongs to</param>
        /// <param name="remisierId">The remisier the customer belongs to</param>
        /// <param name="remisierEmployeeId">The remisier employee the customer belongs to</param>
        /// <param name="lifecycleId">The account's model portfolio</param>
        /// <param name="modelId">The account's model portfolio</param>
        /// <param name="accountNumber">The account's number of the account</param>
        /// <param name="accountName">The name of the account</param>
        /// <param name="accountsWithModelOnly">Only return accounts that have a model attached</param>
        /// <returns>A list of AccountTypeInternal (customer) instances</returns>
        public static IList<IAccountTypeCustomer> GetCustomerAccounts(IDalSession session,
            int assetManagerId, int remisierId, int remisierEmployeeId,
            int lifecycleId, int modelId, string accountNumber,
            string accountName, bool accountsWithModelOnly, bool retrieveNostroAccounts, bool showActive, bool showInactive, int selectedYear,
            bool showTradeable, bool showNonTradeable)
        {
            Hashtable parameters = new Hashtable();
            Hashtable parameterLists = new Hashtable();

            if (assetManagerId != 0)
                parameters.Add("assetManagerId", assetManagerId);
            else
            {
                ILogin login = LoginMapper.GetCurrentLogin(session);
                if (login != null)
                {
                    if (login is AssetManagerEmployeeLogin)
                        parameters.Add("assetManagerId", ((AssetManagerEmployeeLogin)login).Employer.Key);
                    else if (login is CustomerLogin)
                        throw new System.Security.Authentication.AuthenticationException("The no filter option is only available for stichting employees");
                }
                else
                    throw new System.Security.Authentication.AuthenticationException("You are not a registered user");
            }

            if (remisierId != 0)
                parameters.Add("remisierId", remisierId);
            if (remisierEmployeeId != 0)
                parameters.Add("remisierEmployeeId", remisierEmployeeId);
            if (lifecycleId != 0)
                parameters.Add("lifecycleId", lifecycleId);
            if (modelId != 0)
                parameters.Add("modelId", modelId);
            if (accountNumber != null && accountNumber.Length > 0)
                parameters.Add("accountNumber", Util.PrepareNamedParameterWithWildcard(accountNumber, MatchModes.Anywhere));
            if (accountName != null && accountName.Length > 0)
                parameters.Add("accountName", Util.PrepareNamedParameterWithWildcard(accountName, MatchModes.Anywhere));
            if (accountsWithModelOnly)
                parameters.Add("accountsWithModelOnly", 1);
            if (showTradeable || showNonTradeable)
            {
                ArrayList tradeabilityOptions = new ArrayList();
                if (showTradeable)
                    tradeabilityOptions.Add(Tradeability.Tradeable);
                if (showNonTradeable)
                    tradeabilityOptions.Add(Tradeability.NonTradeable);
                parameterLists.Add("tradeabilityOptions", tradeabilityOptions);
            }
            if (showActive || showInactive)
            {
                ArrayList statuses = new ArrayList();
                if (showActive)
                    statuses.Add(AccountStati.Active);
                if (showInactive)
                    statuses.Add(AccountStati.Inactive);
                parameterLists.Add("statuses", statuses);
            }
            if (selectedYear > 0)
            {
                DateTime selectedStartDate = new DateTime(selectedYear, 01, 01);
                DateTime selectedEndDate = new DateTime(selectedYear, 12, 31);
                parameters.Add("selectedStartDate", selectedStartDate);
                parameters.Add("selectedEndDate", selectedEndDate);
            }
            if (!(retrieveNostroAccounts && remisierId == 0))
                parameters.Add("accountTypeCustomer", AccountTypes.Customer);

            return session.GetTypedListByNamedQuery<IAccountTypeCustomer>(
                "B4F.TotalGiro.Accounts.Account.GetCustomerAccounts",
                parameters, parameterLists);
        }

        /// <summary>
        /// This method retrieves a list of internal account instances that comply to a specific role and belong to a specific account manager. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="type">The role of the account</param>
        /// <param name="assetManager">The asset manager the customer belongs to</param>
        /// <returns>A list of AccountTypeInternal instances</returns>
        public static IList<T> GetInternalAccounts<T>(IDalSession session, AccountTypes type, IAssetManager assetManager)
            where T : IAccount
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("AccountOwner.Key", assetManager.Key));
            return NHSession.ToList<T>(session.GetList(getType(type), expressions));
        }


        public static IList<IAccountTypeInternal> GetInternalAccounts(IDalSession session, IAssetManager assetManager, string accountNumber, string accountName)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();
            if (assetManager != null)
                expressions.Add(Expression.Eq("AccountOwner.Key", assetManager.Key));
            if (accountNumber != null && accountNumber.Length > 0)
                expressions.Add(Expression.Like("Number", accountNumber, MatchMode.Anywhere));
            if (accountName != null && accountName.Length > 0)
                expressions.Add(Expression.Like("ShortName", accountName, MatchMode.Anywhere));
            LoginMapper.GetSecurityInfo(session, expressions, SecurityInfoOptions.NoFilter);
            orderings.Add(Order.Asc("Number"));

            // Last two null parameters are necessary, otherwise sorting doesn't work
            return session.GetTypedList<AccountTypeInternal, IAccountTypeInternal>(expressions, orderings);
        }

        /// <summary>
        /// This method retrieves a list of external account instances. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.</param>
        /// <returns>A list of AccountTypeInternal instances.</returns>
        public static IList<IAccountTypeExternal> GetExternalAccounts(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();
            orderings.Add(Order.Asc("Number"));
            // Last two null parameters are necessary, otherwise sorting doesn't work
            return session.GetTypedList<AccountTypeExternal, IAccountTypeExternal>(expressions, orderings);
        }

        /// <summary>
        /// This method retrieves a list of account instances that comply to a specific role. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="type">The role of the account</param>
        /// <returns>A list of account instances</returns>
        public static IList<T> GetAccounts<T>(IDalSession session, AccountTypes type)
            where T : IAccount
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();
            orderings.Add(Order.Asc("Number"));

            // Last two null parameters are necessary, otherwise sorting doesn't work
            return NHSession.ToList<T>(session.GetList(getType(type), expressions, orderings));
        }

        /// <summary>
        /// This method retrieves a list of account instances of a certain type. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="type">The type of accounts that should be returned</param>
        /// <returns>A list of account instances</returns>
        public static IList<T> GetAccounts<T>(IDalSession session, AccountTypeReturnClass type)
            where T : IAccount
        {
            Type typeAcc;

            switch (type)
            {
                case AccountTypeReturnClass.Internal:
                    typeAcc = typeof(AccountTypeInternal);
                    break;
                case AccountTypeReturnClass.External:
                    typeAcc = typeof(AccountTypeExternal);
                    break;
                default:
                    typeAcc = typeof(Account);
                    break;
            }
            List<Order> orderings = new List<Order>();
            orderings.Add(Order.Asc("Number"));
            return NHSession.ToList<T>(session.GetList(typeAcc, null, orderings));
        }

        /// <summary>
        /// This method retrieves a list of account instances that are uniquely identified by one of the unique identifiers in the passed-in array. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="accountIDs">An array of account unique identifiers</param>
        /// <returns>A list of account instances</returns>
        public static IList<T> GetAccounts<T>(IDalSession session, int[] accountIDs)
            where T : IAccount
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("Key", accountIDs));
            return NHSession.ToList<T>(session.GetTypedList<Account>(expressions));
        }

        /// <summary>
        /// This method retrieves a list of account instances that belong to a specific account manager. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="assetManager">The asset manager the account belongs to</param>
        /// <returns>A list of account instances</returns>
        public static IList<T> GetAccounts<T>(IDalSession session, IAssetManager assetManager)
            where T : IAccount
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("VermogensBeheer.Key", assetManager.Key));
            return NHSession.ToList<T>(session.GetTypedList<Account>(expressions));
        }

        /// <summary>
        /// This method retrieves a list of account instances that are attached to a modelportfolio. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <returns>A list of account instances</returns>
        public static IList<IAccount> GetManagedAccounts(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Sql("this_.ModelID IS NOT NULL"));
            return session.GetTypedList<Account, IAccount>(expressions);
        }

        public static IList GetAccountNumberPrefixes(IDalSession session)
        {
            string sqlQuery = @"SELECT DISTINCT SUBSTRING(a.AccountNumber, 1, 4) AS Prefix 
                                FROM vweCustomerAccounts A
                                ORDER BY SUBSTRING(a.AccountNumber, 1, 4)";

            DataSet ds = session.GetDataFromQuery(sqlQuery);

            ArrayList prefixList = new ArrayList();

            foreach (DataRow dr in ds.Tables[0].Rows)
                prefixList.Add(dr["Prefix"]);

            return prefixList;
        }

        public static List<ICashMutationView> GetCashPositionTransactions(
            IDalSession session, ICashSubPosition position, DateTime beginDate, DateTime endDate, bool displayAll)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("subPosId", position.Key);
            parameters.Add("beginDate", beginDate);
            parameters.Add("endDate", endDate);
            parameters.Add("settledFlag", position.SettledFlag);
            if (!displayAll)
                parameters.Add("hideStornos", 1);

            IList<IBookingComponentParent> bookings = session.GetTypedListByNamedQuery<IBookingComponentParent>(
                "B4F.TotalGiro.GeneralLedger.Journal.Bookings.GetBookingComponentParents",
                parameters);

            return GetCashPositionTransactionsFromBookings(bookings, position);
        }

        public static List<ICashMutationView> GetCashPositionTransactions(
            IDalSession session, IAccountTypeCustomer account, DateTime beginDate, DateTime endDate, bool includeStornos)
        {
            return GetCashPositionTransactions(session, account, 0, beginDate, endDate, includeStornos);
        }

        public static List<ICashMutationView> GetCashPositionTransactions(
            IDalSession session, IAccountTypeCustomer account, int currencyId, DateTime beginDate, DateTime endDate, bool includeStornos)
        {
            Hashtable parameters = new Hashtable();
            if (currencyId != 0)
                parameters.Add("baseCurrencyId", currencyId);
            parameters.Add("accountId", account.Key);
            parameters.Add("statusBooked", (int)JournalEntryLineStati.Booked);
            parameters.Add("beginDate", beginDate);
            parameters.Add("endDate", endDate);
            parameters.Add("settledFlag", (int)CashPositionSettleStatus.Settled);
            if (!includeStornos)
                parameters.Add("hideStornos", 1);

            IList<IBookingComponentParent> bookings = session.GetTypedListByNamedQuery<IBookingComponentParent>(
                "B4F.TotalGiro.GeneralLedger.Journal.Bookings.GetBookingComponentParents",
                parameters);
            return GetCashPositionTransactionsFromBookings(bookings, true);
        }

        public static List<ICashMutationView> GetCashPositionTransactionsFromBookings(
            IList<IBookingComponentParent> bookings, bool isSettled)
        {
            return getCashPositionTransactionsFromBookings(bookings, null, isSettled);
        }

        public static List<ICashMutationView> GetCashPositionTransactionsFromBookings(
            IList<IBookingComponentParent> bookings, ICashSubPosition position)
        {
            return getCashPositionTransactionsFromBookings(bookings, position, position.SettledFlag == CashPositionSettleStatus.Settled);
        }

        private static List<ICashMutationView> getCashPositionTransactionsFromBookings(
            IList<IBookingComponentParent> bookings, ICashSubPosition position, bool isSettled)
        {
            List<ICashMutationView> result = new List<ICashMutationView>();

            IList<ITransaction> transactions = bookings
                                                .Where(x => x.BookingComponentParentType == BookingComponentParentTypes.Transaction)
                                                .Cast<ITransactionComponent>()
                                                .GroupBy(y => y.ParentTransaction.Key)
                                                .Select(d => d.First().ParentTransaction).ToList();
            foreach (B4F.TotalGiro.Orders.Transactions.ITransaction trade in transactions)
                result.Add(new CashMutationViewTX(trade, position));

            IList<IGeneralOperationsBooking> gops = bookings
                                                    .Where(x => x.BookingComponentParentType != BookingComponentParentTypes.Transaction)
                                                    .Cast<IGeneralOperationsComponent>()
                                                    .GroupBy(y => y.ParentBooking.Key)
                                                    .Select(d => d.First().ParentBooking).ToList();
            foreach (IGeneralOperationsBooking booking in gops)
                result.Add(new CashMutationViewGop(booking, position, isSettled));
            return result;
        }

        public static IList<ICustomerAccount> GetCustomerAccountsbyCreationDate(IDalSession session, DateTime creationDate)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Ge("creationDate", creationDate.AddDays(-7)));
            return session.GetTypedList<CustomerAccount, ICustomerAccount>(expressions);

        }

        public static IList<IAccountTypeCustomer> GetAccountsforVirtualFund(IDalSession session, int virtualFundID)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("fundID", virtualFundID);
            return session.GetTypedListByNamedQuery<IAccountTypeCustomer>(
                "B4F.TotalGiro.Accounts.Account.GetAccountsforVirtualFund",
                parameters);
        }

        public static IList<int> GetAccountKeystoCloseFinancialYear(IDalSession session, int year)
        {
            Hashtable parameters = new Hashtable(4);

            DateTime startDate = new DateTime(year, 1, 1);
            parameters.Add("startDate", startDate);

            DateTime endDate = new DateTime(year, 12, 31);
            parameters.Add("endDate", endDate);

            DateTime alreadyBooked = new DateTime(year, 12, 31).AddDays(1);
            parameters.Add("alreadyBooked", alreadyBooked);

            parameters.Add("year", year);

            return session.GetTypedListByNamedQuery<int>(
                "B4F.TotalGiro.Accounts.Account.GetAccountKeystoCloseFinancialYear",
                parameters);
        }




        public static IList<int> GetAccountKeysActiveinFinancialYear(IDalSession session, int year)
        {
            

            DateTime lastFinancialYearDay = new DateTime((year - 1), 12, 31);
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = new DateTime(year, 12, 31);

            Hashtable parameters1 = new Hashtable(1);
            parameters1.Add("lastFinancialYearDay", lastFinancialYearDay);
            IList<int> accountKeysWithStartingBalance = session.GetTypedListByNamedQuery<int>(
                                "B4F.TotalGiro.Accounts.Account.GetAccountKeysWithStartingBalance",
                                parameters1);

            Hashtable parameters2 = new Hashtable(2);
            parameters2.Add("startDate", startDate);            
            parameters2.Add("endDate", endDate);
            IList<int> accountKeyswithTransactionsinPeriod = session.GetTypedListByNamedQuery<int>(
                                "B4F.TotalGiro.Accounts.Account.GetAccountKeyswithTransactionsinPeriod",
                                parameters2);

            IList<int> accountKeyswithGLbookingsinPeriod = session.GetTypedListByNamedQuery<int>(
                                "B4F.TotalGiro.Accounts.Account.GetAccountKeyswithGLbookingsinPeriod",
                                parameters2);

            var relevantKeys = accountKeysWithStartingBalance
                    .Union(accountKeyswithTransactionsinPeriod)
                    .Union(accountKeyswithGLbookingsinPeriod);

            IList<int> CustomerKeys = session.GetTypedListByNamedQuery<int>(
                    "B4F.TotalGiro.Accounts.Account.GetCustomerAccountKeys");

            return CustomerKeys
                        .Join(relevantKeys, c => c, r => r, (c, r) => c).ToList();
        }

        public static IList<int> GetCustomerAccountKeysActiveforDividWEP(IDalSession session, ReportingPeriodDetail period)
        {
            Hashtable parameters = new Hashtable(2);

            parameters.Add("termType", period.TermType);
            parameters.Add("endTermYear", period.EndTermYear);


            return session.GetTypedListByNamedQuery<int>(
            "B4F.TotalGiro.Accounts.Account.GetCustomerAccountKeysActiveforDividWEP",
            parameters);

        }





        #region AccountStatuses

        public static List<AccountStatus> GetAccountStatuses(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            return session.GetTypedList<AccountStatus>(expressions);
        }

        public static IList<AccountTradeability> GetAccountTradeabilityStatuses(IDalSession session)
        {
            return session.GetTypedList<AccountTradeability>();
        }

        public static List<AccountStati> GetOpenAccountStati(IDalSession session)
        {
            return GetAccountStatuses(session).Where(status => status.IsOpen && Enum.IsDefined(typeof(AccountStati), status.Key))
                                              .Select(status => (AccountStati)status.Key).ToList();
        }

        public static AccountStatus GetAccountStatus(IDalSession session, AccountStati key)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("key", (int)key));
            IList statuses = session.GetList(typeof(AccountStatus), expressions);
            if (statuses != null && statuses.Count > 0)
                return (AccountStatus)statuses[0];
            else
                return null;
        }

        public static bool AccountStatusIsOpen(IDalSession session, AccountStati key)
        {
            AccountStatus status = GetAccountStatus(session, key);
            return (status != null ? status.IsOpen : true);
        }

        #endregion


        #region Positions

        /// <summary>
        /// This method retrieves a specific Fund Position instance that is uniquely identified by the passed in argument. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="positionId">The unique identifier of the position</param>
        /// <returns>An instance of a specific Position class</returns>
        public static IFundPosition GetFundPosition(IDalSession session, int positionId)
        {
            IFundPosition pos = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", positionId));
            IList<IFundPosition> positions = session.GetTypedList<FundPosition, IFundPosition>(expressions);
            if (positions != null && positions.Count == 1)
                pos = positions[0];
            return pos;
        }

        /// <summary>
        /// This method retrieves a specific Fund Position instance that is uniquely identified by the passed in argument. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="positionIds">The unique identifiers of the positions</param>
        /// <returns>An instance of a specific Position class</returns>
        public static IList<IFundPosition> GetFundPositions(IDalSession session, int[] positionIds)
        {
            IFundPosition pos = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("Key", positionIds));
            return session.GetTypedList<FundPosition, IFundPosition>(expressions);
        }

        /// <summary>
        /// This method retrieves a specific Cash Position instance that is uniquely identified by the passed in argument. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="positionId">The unique identifier of the position</param>
        /// <returns>An instance of a specific Position class</returns>
        public static ICashPosition GetCashPosition(IDalSession session, int positionId)
        {
            ICashPosition pos = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", positionId));
            IList<ICashPosition> positions = session.GetTypedList<CashPosition, ICashPosition>(expressions);
            if (positions != null && positions.Count == 1)
                pos = positions[0];
            return pos;
        }
        public static IAccountHolder GetAccountHolder(IDalSession session, int ahKey)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", ahKey));
            IList collAH = session.GetList(typeof(AccountHolder), expressions);
            if (collAH != null && collAH.Count == 1)
                return (IAccountHolder)collAH[0];
            else
                return null;
        }

        #endregion



        #region ModelHistory

        public static IModelHistory GetHistoricalModel(IDalSession session, IAccountTypeCustomer account, DateTime date)
        {
            IModelHistory model = null;
            Hashtable parameters = new Hashtable(2);
            parameters.Add("accountId", account.Key);
            parameters.Add("date", date);

            IList<IModelHistory> result = session.GetTypedListByNamedQuery<IModelHistory>(
                "B4F.TotalGiro.Accounts.ModelHistory.GetHistoricalModels",
                parameters);
            if (result != null && result.Count > 0)
                model = result[0];
            return model;
        }

        #endregion

        #region Helpers

        private static System.Type getType(AccountTypes accountType)
        {
            System.Type type;
            switch (accountType)
            {
                case AccountTypes.Commission:
                    type = typeof(CommissionAccount);
                    break;
                case AccountTypes.Crumble:
                    type = typeof(CrumbleAccount);
                    break;
                case AccountTypes.Custody:
                    type = typeof(CustodyAccount);
                    break;
                case AccountTypes.Nostro:
                    type = typeof(NostroAccount);
                    break;
                case AccountTypes.Trading:
                    type = typeof(TradingAccount);
                    break;
                case AccountTypes.Counterparty:
                    type = typeof(CounterPartyAccount);
                    break;
                default:
                    type = typeof(CustomerAccount);
                    break;
            }
            return type;
        }

        #endregion

        #region CRUD

        /// <summary>
        /// This method is used to insert or update the account
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">The account that is inserted or updated</param>
        /// <returns>Returns True when succesfull</returns>
        public static bool Update(IDalSession session, IAccount obj)
        {
            return session.InsertOrUpdate(obj);
        }

        /// <summary>
        /// This method is used to insert the account
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">The account that is inserted or updated</param>
        /// <returns>Returns True when succesfull</returns>
        public static void Insert(IDalSession session, IAccount obj)
        {
            session.InsertOrUpdate(obj);
        }

        /// <summary>
        /// This method is used to insert or update the list of accounts
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">The list of accounts that is inserted or updated</param>
        /// <returns>Returns True when succesfull</returns>
        public static bool UpDateList(IDalSession session, IList list)
        {
            return session.Update(list);
        }

        #endregion

    }
}

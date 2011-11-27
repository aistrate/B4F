using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Instructions
{
    public static class InstructionEntryAdapter
    {
        public static DataSet GetCustomerAccounts(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
                                                  int maximumRows, int pageIndex, string sortColumn)
        {
            const string propertyList =
                "Key, ShortName, Number, ModelPortfolio.ModelName, CurrentRebalanceDate, LastRebalanceDate, ActiveAccountInstructions.Count, CounterAccount.Key";

            string bareSortColumn = sortColumn.Split(' ')[0];
            bool ascending = !(sortColumn.Split(' ').Length == 2 && sortColumn.Split(' ')[1] == "DESC");

            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;

            IList allAccounts;
            if (isHqlSortingNeeded(bareSortColumn))
            {
                allAccounts = GetCustomerAccountsList(session,
                                    assetManagerId, modelPortfolioId, accountNumber, accountName, null, bareSortColumn, ascending, true);
                ds = DataSetBuilder.CreateDataSetFromHibernateList(allAccounts, "Key");
            }
            else
            {
                allAccounts = GetCustomerAccountsList(session,
                                    assetManagerId, modelPortfolioId, accountNumber, accountName, null, bareSortColumn, ascending, false);
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(allAccounts, "Key, " + bareSortColumn.Replace('_', '.'));
                Util.SortDataTable(ds.Tables[0], sortColumn);

                session.Close();
                session = NHSessionFactory.CreateSession();
            }
            
            int[] accountIds = Util.GetPageKeys(ds.Tables[0], maximumRows, pageIndex, "Key");
            IList pageAccounts = GetCustomerAccountsList(session, 0, 0, null, null, accountIds, bareSortColumn, ascending, false);
            DataSetBuilder.MergeDataTableWithBusinessObjectList(ds.Tables[0], pageAccounts, "Key", propertyList);

            session.Close();
            return ds;
        }

        private static bool isHqlSortingNeeded(string sortColumn)
        {
            string[] hqlSortColumns = new string[] { "KEY", "NUMBER", "SHORTNAME", "MODELPORTFOLIO_MODELNAME" };
            sortColumn = sortColumn.ToUpper();
            foreach (string col in hqlSortColumns)
                if (col == sortColumn)
                    return true;
            return false;
        }

        public static IList GetCustomerAccountsList(IDalSession session, int assetManagerId, int modelPortfolioId, string accountNumber,
                                                    string accountName, int[] accountIds, string sortColumn, bool ascending, bool keysOnly)
        {
            string where = "";

            if (assetManagerId > 0)
                where += string.Format(" and A.AccountOwner = {0} ", assetManagerId);
            if (modelPortfolioId > 0)
                where += string.Format(" and M.Key = {0}", modelPortfolioId);
            if (accountNumber != null && accountNumber.Length > 0)
                where += string.Format(" and A.Number LIKE '%{0}%'", accountNumber);
            if (accountName != null && accountName.Length > 0)
                where += string.Format(" and A.ShortName LIKE '%{0}%'", accountName);
            if (accountIds != null)
                where += string.Format(" and A.Key IN ({0})",
                    (accountIds.Length == 0 ? "0" : string.Join(", ", Array.ConvertAll<int, string>(accountIds, id => id.ToString()))));

            string orderBy = "order by A.Key", contactsJoin = "";

            if (keysOnly && sortColumn != "")
            {
                string ascendingStr = (ascending ? "ASC" : "DESC");
                sortColumn = sortColumn.ToUpper();

                string sortProperty = "";
                switch (sortColumn)
                {
                    case "KEY":
                        sortProperty = "A.Key";
                        break;
                    case "NUMBER":
                        sortProperty = "A.Number";
                        break;
                    case "SHORTNAME":
                        sortProperty = "CN.Name";
                        contactsJoin = @"left join A.bagOfAccountHolders AH
                                         left join AH.Contact C
                                         left join C.CurrentNAW CN";
                        where += " and AH.IsPrimaryAccountHolder = true";
                        break;
                    case "MODELPORTFOLIO_MODELNAME":
                        sortProperty = "M.ModelName";
                        break;
                }

                if (sortProperty != "")
                    orderBy = string.Format("order by {0} {1}", sortProperty, ascendingStr);
            }
            
            string hql = string.Format(@"{0}from CustomerAccount A
                                         left join {1} A.ModelPortfolio M {2}
                                         where M is not null 
                                         and A.Status = :status
                                         and A.TradeableStatus = :tradeableStatus
                                         and IsNull(A.IsExecOnlyCustomer, 0) = 0 {3} {4}",
                                       (keysOnly ? "select A.Key " : ""),
                                       (keysOnly ? "" : "fetch"),
                                        contactsJoin, where, orderBy);

            Hashtable parameters = new Hashtable();
            parameters.Add("status", (int)AccountStati.Active);
            parameters.Add("tradeableStatus", (int)Tradeability.Tradeable);
            return session.GetListByHQL(hql, parameters);
        }

        public static DataSet GetPublicCounterAccounts(object accountids)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<ICounterAccount> list = null;
                DataSet ds = null;

                if (accountids != null)
                {
                    int[] ids = (int[])accountids;
                    if (ids != null && ids.Length == 1)
                    {
                        IAccount account = AccountMapper.GetAccount(session, ids[0]);
                        list = CounterAccountMapper.GetCounterAccounts(session, account, true);
                    }
                    else
                        list = CounterAccountMapper.GetPublicCounterAccounts(session);

                    ds = list
                        .Select(c => new
                        {
                            c.Key,
                            c.DisplayName
                        })
                        .ToDataSet();
                }
                else
                    ds = new DataSet();

                Utility.AddEmptyFirstRow(ds, "DisplayName", "Default Counter Account");
                return ds;
            }
        }

        public static DataSet GetAccountActiveInstructions(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable(1);
                parameters.Add("accountId", accountId);
                IList list = session.GetListByNamedQuery(
                    "B4F.TotalGiro.Accounts.Instructions.AccountActiveInstructions",
                    parameters);
                return DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    list,
                    "Key, Account.Number, Account.ShortName, InstructionType, DisplayStatus, CreationDate, Message, OrdersGenerated");
            }
        }

        public static DataSet GetOrderActionTypes()
        {
            DataSet ds = Util.GetDataSetFromEnum(typeof(OrderActionTypes));
            DataTable dt = ds.Tables[0];
            for (int i = dt.Rows.Count; i > 0; i--)
            {
                DataRow row = dt.Rows[i - 1];
                if (row[0].Equals((int)OrderActionTypes.SingleOrder) || row[0].Equals((int)OrderActionTypes.NoAction))
                    dt.Rows.Remove(row);
            }
            Util.SortDataTable(ds.Tables[0], "Description");
            return ds;
        }

        public static DataSet GetInstruments(string filter, string propertyList, int[] excludedKeys)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable();
                Hashtable parameterLists = new Hashtable();
                DataSet ds = null;

                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (!company.IsStichting)
                    parameters.Add("managementCompanyID", company.Key);

                if (!string.IsNullOrEmpty(filter))
                {
                    if (Util.IsNumeric(filter) || (filter.Length > 2 && Util.IsNumeric(filter.Substring(2))))
                        parameters.Add("isin", Util.PrepareNamedParameterWithWildcard(filter, MatchModes.Anywhere));
                    else
                        parameters.Add("name", Util.PrepareNamedParameterWithWildcard(filter, MatchModes.Anywhere));
                }
                if (excludedKeys != null && excludedKeys.Length > 0)
                    parameterLists.Add("excludedKeys", excludedKeys);

                List<ITradeableInstrument> instruments = session.GetTypedListByNamedQuery<ITradeableInstrument>(
                    "B4F.TotalGiro.ApplicationLayer.Instructions.InstrumentsToExclude",
                    parameters, parameterLists);

                return DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    instruments,
                    propertyList);
            }
        }

        public static DataSet GetModels(string filter, string propertyList, int[] excludedKeys)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable();
                Hashtable parameterLists = new Hashtable();
                DataSet ds = null;

                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (!company.IsStichting)
                    parameters.Add("managementCompanyID", company.Key);

                if (!string.IsNullOrEmpty(filter))
                    parameters.Add("name", Util.PrepareNamedParameterWithWildcard(filter, MatchModes.Anywhere));
                if (excludedKeys != null && excludedKeys.Length > 0)
                    parameterLists.Add("excludedKeys", excludedKeys);

                List<IPortfolioModel> models = session.GetTypedListByNamedQuery<IPortfolioModel>(
                    "B4F.TotalGiro.ApplicationLayer.Instructions.ModelsToExclude",
                    parameters, parameterLists);

                return DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    models,
                    propertyList);
            }
        }

        public static bool CreateRebalanceInstructions(BatchExecutionResults results, int[] accountIds, OrderActionTypes orderActionType, bool noCharges, DateTime execDate, List<RebalanceExclusionDetails> exclusions)
        {
            bool retVal = false;

            if (accountIds == null || accountIds.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<IAccountTypeCustomer> accounts = AccountMapper.GetAccounts<IAccountTypeCustomer>(session, accountIds);
                if (accounts != null && accounts.Count > 0)
                {
                    IList saveAccounts = new ArrayList();
                    foreach (IAccountTypeCustomer account in accounts)
                    {
                        try
                        {
                            IRebalanceInstruction instruction = (IRebalanceInstruction)account.CreateInstruction(InstructionTypes.Rebalance, orderActionType, execDate, noCharges);
                            if (instruction != null)
                            {
                                if (exclusions != null && exclusions.Count > 0)
                                {
                                    foreach (RebalanceExclusionDetails exclusion in exclusions)
                                    {
                                        switch (exclusion.ComponentType)
                                        {
                                            case ModelComponentType.Model:
                                                if (exclusion.Model == null)
                                                    exclusion.Model = ModelMapper.GetModel(session, exclusion.ComponentKey);
                                                instruction.ExcludedComponents.AddExclusion(exclusion.Model);
                                                break;
                                            case ModelComponentType.Instrument:
                                                if (exclusion.Instrument == null)
                                                    exclusion.Instrument = InstrumentMapper.GetTradeableInstrument(session, exclusion.ComponentKey);
                                                instruction.ExcludedComponents.AddExclusion(exclusion.Instrument);
                                                break;
                                        }
                                    }
                                }
                                saveAccounts.Add(account);
                                results.MarkSuccess();
                            }
                        }
                        catch (Exception ex)
                        {
                            results.MarkError(
                                new ApplicationException(string.Format("Error creating rebalance instruction  for {0}.", account.DisplayNumberWithName), ex));
                        }
                    }
                    retVal = AccountMapper.UpDateList(session, saveAccounts);
                }
            }
            return retVal;
        }

        public static bool CreateWithdrawalInstructions(BatchExecutionResults results, int[] accountIds, DateTime executionDate, DateTime withdrawalDate, decimal withdrawalAmount, int? counterAccountID, string transferDescription, bool noCharges)
        {
            bool retVal = false;
            if (accountIds == null || accountIds.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                if (withdrawalAmount == 0)
                    throw new ApplicationException("The amount can not be zero");

                ICounterAccount counterAcc = null;
                if (counterAccountID.HasValue && counterAccountID.Value != 0)
                {
                    counterAcc = CounterAccountMapper.GetCounterAccount(session, counterAccountID.Value);
                    if (counterAcc == null)
                        throw new ApplicationException("Counter Account can not be found.");
                }

                ICurrency baseCurrency = LoginMapper.GetCurrentManagmentCompany(session).BaseCurrency;
                Money amount = new Money(withdrawalAmount, baseCurrency).Negate();

                IList<IAccountTypeCustomer> accounts = AccountMapper.GetAccounts<IAccountTypeCustomer>(session, accountIds);
                if (accounts != null && accounts.Count > 0)
                {
                    IList saveAccounts = new ArrayList();
                    foreach (IAccountTypeCustomer account in accounts)
                    {
                        try
                        {
                            if (account.CreateWithdrawalInstruction(executionDate, withdrawalDate, amount, counterAcc, null, transferDescription, noCharges) != null)
                            {
                                saveAccounts.Add(account);
                                results.MarkSuccess();
                            }
                        }
                        catch (Exception ex)
                        {
                            results.MarkError(
                                new ApplicationException(string.Format("Error creating withdrawal instruction  for {0}.", account.DisplayNumberWithName), ex));
                        }
                    }
                    retVal = AccountMapper.UpDateList(session, saveAccounts);
                }
            }
            return retVal;
        }

        public static bool CreateDepartureInstructions(BatchExecutionResults results, int[] accountIds, DateTime executionDate, int? counterAccountID, string transferDescription, bool noCharges)
        {
            bool retVal = false;
            if (accountIds == null || accountIds.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICounterAccount counterAcc = null;
                if (counterAccountID.HasValue && counterAccountID.Value != 0)
                    counterAcc = CounterAccountMapper.GetCounterAccount(session, counterAccountID.Value);

                IList<IAccountTypeCustomer> accounts = AccountMapper.GetAccounts<IAccountTypeCustomer>(session, accountIds);
                if (accounts != null && accounts.Count > 0)
                {
                    IList saveAccounts = new ArrayList();
                    foreach (IAccountTypeCustomer account in accounts)
                    {
                        try
                        {
                            int? withdrawalCount = account.ActiveWithdrawalInstructions.GetV(e => e.Count);
                            if (withdrawalCount.HasValue && withdrawalCount.Value > 0)
                                throw new ApplicationException(string.Format("{0} withdrawal instructions exist and need to be cancelled before the clients portfolio can be liquidated.", withdrawalCount.Value));
                            
                            IClientDepartureInstruction instruction = account.CreateDepartureInstruction(executionDate, counterAcc, transferDescription, noCharges);
                            if (instruction != null)
                            {
                                saveAccounts.Add(account);
                                results.MarkSuccess();
                                if (instruction.CounterAccount == null)
                                    results.MarkWarning(string.Format("Account {0} does not have a default counter account. Please look up it's counter account otherwise the client can not be paid.", instruction.Account.DisplayNumberWithName));
                            }
                        }
                        catch (Exception ex)
                        {
                            results.MarkError(
                                new ApplicationException(string.Format("Error creating departure instruction  for {0}.", account.DisplayNumberWithName), ex));
                        }
                    }
                    retVal = AccountMapper.UpDateList(session, saveAccounts);
                }
            }
            return retVal;
        }

        #region Display Results

        public static string FormatErrorsForCreateInstructions(BatchExecutionResults results, string instructionType)
        {
            const int MAX_ERRORS_DISPLAYED = 25;
            const int MAX_WARNINGS_DISPLAYED = 25;

            string message = "<br/>";

            if (results.SuccessCount == 0 && results.ErrorCount == 0)
                message += string.Format("No new {0} instructions were created", instructionType);
            else
            {
                if (results.SuccessCount > 0)
                    message += string.Format("{0} {1} instructions were successfully created.<br/><br/><br/>", results.SuccessCount, instructionType);

                if (results.ErrorCount > 0)
                {
                    string tooManyErrorsMessage = (results.ErrorCount > MAX_ERRORS_DISPLAYED ?
                                                        string.Format(" (only the first {0} are shown)", MAX_ERRORS_DISPLAYED) : "");

                    message += string.Format("{0} errors occured while creating {1} instructions{2}:<br/><br/><br/>",
                                             results.ErrorCount, instructionType, tooManyErrorsMessage);

                    int errors = 0;
                    foreach (Exception ex in results.Errors)
                    {
                        if (++errors > MAX_ERRORS_DISPLAYED)
                            break;
                        message += Utility.GetCompleteExceptionMessage(ex) + "<br/>";
                    }
                }

                if (results.WarningCount > 0)
                {
                    string tooManyWarningsMessage = (results.WarningCount > MAX_WARNINGS_DISPLAYED ?
                                                        string.Format(" (only the first {0} warnings are shown)", MAX_ERRORS_DISPLAYED) : "");

                    message += string.Format("{0} warnings occured while creating {1} instructions{2}:<br/><br/><br/>",
                                             results.WarningCount, instructionType, tooManyWarningsMessage);

                    int warnings = 0;
                    foreach (string warning in results.Warnings)
                    {
                        if (++warnings > MAX_WARNINGS_DISPLAYED)
                            break;
                        message += warning + "<br/>";
                    }
                }
            }

            return message;
        }

        #endregion

    }
}

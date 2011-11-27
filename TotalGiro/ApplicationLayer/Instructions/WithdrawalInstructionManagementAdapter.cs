using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.BackOffice.Orders;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Instructions
{
    [Flags()]
    public enum WithdrawalInstructionsTypeReturnEnum
    {
        All = -1,
        AdHoc = 0,
        Periodic = 1
    }
    
    public static class WithdrawalInstructionManagementAdapter
    {
        public static DataSet GetInstructions(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
            DateTime dateFrom, DateTime dateTo, ActivityReturnFilter activeStatus, WithdrawalInstructionsTypeReturnEnum returnType,
            int maximumRows, int pageIndex, string sortColumn)
        {
            const string propertyList =
                "Key, Account.Key, Account.Number, Account.ShortName, Status, DisplayStatus, Message, Warning, WithdrawalDate, LatestPossibleFreeUpCashDate, Amount.Quantity, Amount.DisplayString, DisplayRegularity, HasOrders, MoneyTransferOrder.Key, IsEditable, IsActive, CreationDate, DoNotChargeCommission, IsCancellable";

            string bareSortColumn = sortColumn.Split(' ')[0];
            bool ascending = !(sortColumn.Split(' ').Length == 2 && sortColumn.Split(' ')[1] == "DESC");

            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;

            IList allInstructions;
            if (isHqlSortingNeeded(bareSortColumn))
            {
                allInstructions = GetInstructionsList(session,
                                    assetManagerId, modelPortfolioId, accountNumber, accountName, dateFrom, dateTo, activeStatus, returnType, null, bareSortColumn, ascending, true);
                ds = DataSetBuilder.CreateDataSetFromHibernateList(allInstructions, "Key, IsActive");
            }
            else
            {
                allInstructions = GetInstructionsList(session,
                                    assetManagerId, modelPortfolioId, accountNumber, accountName, dateFrom, dateTo, activeStatus, returnType, null, bareSortColumn, ascending, false);
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(allInstructions, "Key, " + bareSortColumn.Replace('_', '.'));
                Util.SortDataTable(ds.Tables[0], sortColumn);

                session.Close();
                session = NHSessionFactory.CreateSession();
            }

            int[] instructionIds = Util.GetPageKeys(ds.Tables[0], maximumRows, pageIndex, "Key");
            IList pageInstructions = GetInstructionsList(session, 0, 0, null, null, DateTime.MinValue, DateTime.MinValue, ActivityReturnFilter.All, WithdrawalInstructionsTypeReturnEnum.All, instructionIds, bareSortColumn, ascending, false);
            DataSetBuilder.MergeDataTableWithBusinessObjectList(ds.Tables[0], pageInstructions, "Key", propertyList);

            session.Close();
            return ds;
        }

        private static bool isHqlSortingNeeded(string sortColumn)
        {
            string[] hqlSortColumns = new string[] { "KEY", "ACCOUNT_NUMBER", "ACCOUNT_SHORTNAME", "STATUS", "MESSAGE", "AMOUNT_QUANTITY", "WITHDRAWALDATE", "EXECUTIONDATE", "CREATIONDATE", "ISACTIVE", "DONOTCHARGECOMMISSION", "REGULARITY" };
            sortColumn = sortColumn.ToUpper();
            foreach (string col in hqlSortColumns)
                if (col == sortColumn)
                    return true;
            return false;
        }

        public static IList GetInstructionsList(IDalSession session, int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
            DateTime dateFrom, DateTime dateTo, ActivityReturnFilter activeStatus, WithdrawalInstructionsTypeReturnEnum returnType,
            int[] instructionIds, string sortColumn, bool ascending, bool keysOnly)
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
            if (dateFrom != DateTime.MinValue || dateTo != DateTime.MinValue)
            {
                if (dateFrom != DateTime.MinValue && dateTo != DateTime.MinValue)
                    where += string.Format(" and I.WithdrawalDate between '{0}' and '{1}'", dateFrom.ToString("yyyy-MM-dd"), dateTo.ToString("yyyy-MM-dd"));
                if (dateFrom != DateTime.MinValue)
                    where += string.Format(" and I.WithdrawalDate >= '{0}'", dateFrom.ToString("yyyy-MM-dd"));
                if (dateTo != DateTime.MinValue)
                    where += string.Format(" and I.WithdrawalDate <= '{0}'", dateTo.ToString("yyyy-MM-dd"));
            }
            if (activeStatus == ActivityReturnFilter.Active)
                where += string.Format(" and (I.IsActive = 1 or (I.IsActive = 0 and I.CloseDate >= '{0}'))", DateTime.Today.ToString("yyyy-MM-dd"));
            else if (activeStatus == ActivityReturnFilter.InActive)
                where += " and (I.IsActive = 0)";
            if (returnType != WithdrawalInstructionsTypeReturnEnum.All)
                where += string.Format(" and I.IsPeriodic = {0}", (returnType == WithdrawalInstructionsTypeReturnEnum.Periodic ? 1 : 0));
            if (instructionIds != null)
                where += string.Format(" and I.Key IN ({0})",
                    (instructionIds.Length == 0 ? "0" : string.Join(", ", Array.ConvertAll<int, string>(instructionIds, id => id.ToString()))));

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
                    case "ACCOUNT_NUMBER":
                        sortProperty = "A.Number";
                        break;
                    case "ACCOUNT_SHORTNAME":
                        sortProperty = "CN.Name";
                        contactsJoin = @"left join A.bagOfAccountHolders AH
                                         left join AH.Contact C
                                         left join C.CurrentNAW CN";
                        where += " and AH.IsPrimaryAccountHolder = true";
                        break;
                    case "STATUS":
                        sortProperty = "I.Status";
                        break;
                    case "DONOTCHARGECOMMISSION":
                        sortProperty = "I.DoNotChargeCommission";
                        break;
                    case "MESSAGE":
                        sortProperty = "I.Message";
                        break;
                    case "WITHDRAWALDATE":
                        sortProperty = "I.WithdrawalDate";
                        break;
                    case "EXECUTIONDATE":
                        sortProperty = "I.ExecutionDate";
                        break;
                    case "CREATIONDATE":
                        sortProperty = "I.CreationDate";
                        break;

                    case "AMOUNT_QUANTITY":
                        sortProperty = "I.Amount.Quantity";
                        break;
                    case "ISACTIVE":
                        sortProperty = "I.IsActive";
                        break;
                    case "REGULARITY":
                        sortProperty = "R.Regularity";
                        break;
                }

                if (sortProperty != "")
                    orderBy = string.Format("order by {0} {1}", sortProperty, ascendingStr);
            }

            string hql = string.Format(@"{0}from CashWithdrawalInstruction I
                                        left join {1} I.Account A 
                                        left join {1} A.ModelPortfolio M 
                                        left join {1} I.Rule R {2}
                                        where (I.ExecutionDate <= '{3}') {4} {5}",
                                       (keysOnly ? "select I.Key, I.IsActive " : ""),
                                       (keysOnly ? "" : "fetch"),
                                        contactsJoin, DateTime.Today.ToString("yyyy-MM-dd"), where, orderBy);

            return session.GetListByHQL(hql, null);
        }


        public static IList GetInstructionEditData(int instructionId)
        {
            ArrayList list = new ArrayList();

            IDalSession session = NHSessionFactory.CreateSession();
            ICashWithdrawalInstruction instruction = (ICashWithdrawalInstruction)InstructionMapper.GetInstruction(session, instructionId);
            InstructionEditView iev = new InstructionEditView(instruction.Key,
                                                instruction.WithdrawalDate,
                                                instruction.ExecutionDate,
                                                Math.Abs(instruction.Amount.Quantity),
                                                (instruction.CounterAccount != null ? instruction.CounterAccount.Key : int.MinValue),
                                                instruction.IsEditable,
                                                instruction.TransferDescription,
                                                instruction.DoNotChargeCommission);
            list.Add(iev);
            session.Close();

            return list;
        }

        public static DataSet GetOrdersByInstruction(int instructionId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IInstruction instruction = InstructionMapper.GetInstruction(session, instructionId);
                return OrderMapper.GetOrders(session, instruction, SecurityInfoOptions.NoFilter)
                    .Select(c => new
                    {
                        c.Key,
                        Account_Number = c.Account.Number,
                        Account_ShortName = c.Account.ShortName,
                        RequestedInstrument_DisplayName = c.RequestedInstrument.DisplayName, 
                        c.Side, 
                        c.Value, 
                        Value_DisplayString = c.Value.DisplayString,
                        c.Commission, 
                        Commission_DisplayString = c.Commission.DisplayString, 
                        c.DisplayIsSizeBased, 
                        c.Status, 
                        c.DisplayStatus, 
                        c.TopParentDisplayStatus, 
                        c.OrderID, 
                        c.CreationDate
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetMoneyTransferOrder(int instructionId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;
            ICashWithdrawalInstruction instruction = (ICashWithdrawalInstruction)InstructionMapper.GetInstruction(session, instructionId);
            if (instruction != null && instruction.MoneyTransferOrder != null)
            {
                IList<IMoneyTransferOrder> list = new List<IMoneyTransferOrder>();
                list.Add(instruction.MoneyTransferOrder);


                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                        list.ToList(),
                        "Key, Reference, BenefBankAcctNr, NarBenef1, TransferDescription1, Amount.DisplayString, Amount, ProcessDate, TransfereeAccount.Number, Status, DisplayStatus, IsEditable");
            }
            session.Close();
            return ds;
        }

        public static DataSet GetCounterAccounts(int instructionId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IInstruction instruction = InstructionMapper.GetInstruction(session, instructionId);
                DataSet ds = CounterAccountMapper.GetCounterAccounts(session, instruction.Account, true)
                    .Select(c => new
                    {
                        c.Key,
                        c.DisplayName
                    })
                    .ToDataSet();
                Utility.AddEmptyFirstRow(ds, "DisplayName", "Default Counter Account");
                return ds;
            }
        }

        public static void EditInstruction(int instructionId, DateTime withdrawalDate, DateTime executionDate, decimal withdrawalAmount, int? counterAccountID, string transferDescription, bool doNotChargeCommission)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ICashWithdrawalInstruction instruction = (ICashWithdrawalInstruction)InstructionMapper.GetInstruction(session, instructionId);
            if (instruction.IsEditable)
            {
                Money amount = new Money(withdrawalAmount, instruction.Amount.Underlying.ToCurrency).Negate();
                ICounterAccount counterAcc = null;
                if (counterAccountID.HasValue && counterAccountID.Value != 0)
                {
                    counterAcc = CounterAccountMapper.GetCounterAccount(session, counterAccountID.Value);
                    if (counterAcc == null)
                        throw new ApplicationException("Counter Account can not be found.");
                }

                if (instruction.Edit(withdrawalDate, executionDate, amount, counterAcc, transferDescription, doNotChargeCommission))
                    InstructionMapper.Update(session, instruction);
            }
            else
                throw new ApplicationException("The instruction is no longer editable");

            session.Close();
        }

        public static int ProcessInstructions(int[] instructionIds)
        {
            if (instructionIds == null || instructionIds.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
            
            IDalSession session = NHSessionFactory.CreateSession();

            // Initialize the Engine
            InstructionEngine engine = new InstructionEngine();
            int instructionsUpdated = 0;

            IList<IInstruction> instructions = InstructionMapper.GetInstructions(session, instructionIds);
            if (instructions != null && instructions.Count > 0)
            {
                session.BeginTransaction();
                foreach (IInstruction instruction in instructions)
                {
                    if (engine.ProcessInstruction(instruction))
                    {
                        InstructionMapper.Update(session, instruction);
                        instructionsUpdated++;
                        if (instruction.UpdateableOrders != null && instruction.UpdateableOrders.Count > 0)
                            OrderMapper.Insert(session, instruction.UpdateableOrders);
                    }
                }
                session.CommitTransaction();
            }
            session.Close();

            return instructionsUpdated;
        }

        public static int CancelInstructions(int[] instructionIds)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            int instructionsCancelled = 0;

            // Initialize the Engine
            //IFeeFactory fees = FeeFactory.GetInstance(session);
            InstructionEngine engine = new InstructionEngine();

            IList<IInstruction> instructions = InstructionMapper.GetInstructions(session, instructionIds);
            if (instructions != null && instructions.Count > 0)
            {
                session.BeginTransaction();
                foreach (IInstruction instruction in instructions)
                {
                    if (engine.CancelInstruction(instruction))
                    {
                        InstructionMapper.Update(session, instruction);
                        instructionsCancelled++;
                        if (instruction.UpdateableOrders != null && instruction.UpdateableOrders.Count > 0)
                            OrderMapper.Update(session, instruction.UpdateableOrders);
                    }
                }
                session.CommitTransaction();
            }
            session.Close();

            return instructionsCancelled;
        }
    }
}

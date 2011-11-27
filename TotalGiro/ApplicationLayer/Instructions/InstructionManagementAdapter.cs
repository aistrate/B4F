using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Accounts.Instructions.Exclusions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Instructions
{
    public static class InstructionManagementAdapter
    {
        public static DataSet GetInstructions(
            int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
            DateTime dateFrom, DateTime dateTo, ActivityReturnFilter activeStatus, 
            int maximumRows, int pageIndex, string sortColumn)
        {
            const string propertyList =
                "Key, Account.Key, Account.Number, Account.ShortName, Status, DisplayStatus, InstructionType, Message, Warning, ExecutionDate, CreationDate, HasOrders, IsEditable, IsActive, DoNotChargeCommission";

            string bareSortColumn = sortColumn.Split(' ')[0];
            bool ascending = !(sortColumn.Split(' ').Length == 2 && sortColumn.Split(' ')[1] == "DESC");

            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;

            IList allInstructions;
            if (isHqlSortingNeeded(bareSortColumn))
            {
                allInstructions = GetInstructionsList(session,
                                    assetManagerId, modelPortfolioId, accountNumber, accountName,
                                    dateFrom, dateTo, activeStatus, null, bareSortColumn, ascending, true);
                ds = DataSetBuilder.CreateDataSetFromHibernateList(allInstructions, "Key, IsActive");
            }
            else
            {
                allInstructions = GetInstructionsList(session,
                                    assetManagerId, modelPortfolioId, accountNumber, accountName,
                                    dateFrom, dateTo, activeStatus, null, bareSortColumn, ascending, false);
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(allInstructions, "Key, " + bareSortColumn.Replace('_', '.'));
                Util.SortDataTable(ds.Tables[0], sortColumn);

                session.Close();
                session = NHSessionFactory.CreateSession();
            }

            int[] instructionIds = Util.GetPageKeys(ds.Tables[0], maximumRows, pageIndex, "Key");
            IList pageInstructions = GetInstructionsList(session, 
                                        0, 0, null, null, DateTime.MinValue, DateTime.MinValue, 
                                        ActivityReturnFilter.All, instructionIds, bareSortColumn, ascending, false);
            DataSetBuilder.MergeDataTableWithBusinessObjectList(ds.Tables[0], pageInstructions, "Key", propertyList);

            session.Close();
            return ds;
        }

        private static bool isHqlSortingNeeded(string sortColumn)
        {
            string[] hqlSortColumns = new string[] { "KEY", "ACCOUNT_NUMBER", "ACCOUNT_SHORTNAME", "STATUS", "MESSAGE", "EXECUTIONDATE", "CREATIONDATE", "DONOTCHARGECOMMISSION" };
            sortColumn = sortColumn.ToUpper();
            foreach (string col in hqlSortColumns)
                if (col == sortColumn)
                    return true;
            return false;
        }

        public static IList GetInstructionsList(
            IDalSession session, int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
            DateTime dateFrom, DateTime dateTo, ActivityReturnFilter activeStatus, 
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
                    where += string.Format(" and I.ExecutionDate between '{0}' and '{1}'", dateFrom.ToString("yyyy-MM-dd"), dateTo.ToString("yyyy-MM-dd"));
                if (dateFrom != DateTime.MinValue)
                    where += string.Format(" and I.ExecutionDate >= '{0}'", dateFrom.ToString("yyyy-MM-dd"));
                if (dateTo != DateTime.MinValue)
                    where += string.Format(" and I.ExecutionDate <= '{0}'", dateTo.ToString("yyyy-MM-dd"));
            }
            if (activeStatus == ActivityReturnFilter.Active)
                where += string.Format(" and (I.IsActive = 1 or (I.IsActive = 0 and I.CloseDate >= '{0}'))", DateTime.Today.ToString("yyyy-MM-dd"));
            else if (activeStatus == ActivityReturnFilter.InActive)
                where += " and (I.IsActive = 0)";
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
                    case "EXECUTIONDATE":
                        sortProperty = "I.ExecutionDate";
                        break;
                    case "CREATIONDATE":
                        sortProperty = "I.CreationDate";
                        break;
                }

                if (sortProperty != "")
                    orderBy = string.Format("order by {0} {1}", sortProperty, ascendingStr);
            }

            string hql = string.Format(@"{0}from InstructionTypeRebalance I
                                        left join {1} I.Account A 
                                        left join {1} A.ModelPortfolio M {2}
                                        where (I.ExecutionDate <= '{3}') {4} {5}",
                                       (keysOnly ? "select I.Key, I.IsActive " : ""),
                                       (keysOnly ? "" : "fetch"),
                                        contactsJoin, DateTime.Today.ToString("yyyy-MM-dd"), where, orderBy);

            return session.GetListByHQL(hql, null);
        }

        public static DataSet GetInstrumentsFromInstructions(object instructions)
        {
            //"Key, DisplayName, Isin, SecCategory.Name, DisplayCurrentPrice, DisplayCurrentPriceDate"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                if (instructions == null)
                    return null;

                int[] instructionIds = (int[])instructions;
                Hashtable paramList = new Hashtable();
                paramList.Add("keys", instructionIds);
                return session.GetTypedListByNamedQuery<ITradeableInstrument>(
                    "B4F.TotalGiro.ApplicationLayer.Instructions.InstrumentsFromInstructions", 
                    new Hashtable() ,paramList)
                .Select(c => new
                {
                    c.Key,
                    c.DisplayName,
                    c.Isin,
                    SecCategoryName =
                        c.SecCategory.Name,
                    c.DisplayCurrentPrice,
                    c.DisplayCurrentPriceDate
                })
                .ToDataSet();
            }
        }

        public static IList GetInstructionEditData(int Key)
        {
            ArrayList list = new ArrayList();

            IDalSession session = NHSessionFactory.CreateSession();
            IInstructionTypeRebalance instruction = (IInstructionTypeRebalance)InstructionMapper.GetInstruction(session, Key);
            InstructionEditView iev = new InstructionEditView(instruction.Key,
                                                (int)instruction.OrderActionType,
                                                instruction.ExecutionDate,
                                                instruction.DoNotChargeCommission,
                                                instruction.IsEditable);
            if (instruction.InstructionType == InstructionTypes.Rebalance)
                iev.AddExclusions(((IRebalanceInstruction)instruction).ExcludedComponents);

            list.Add(iev);
            session.Close();

            return list;
        }

        public static DataSet GetOrdersByInstruction(int instructionId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IInstruction instruction = InstructionMapper.GetInstruction(session, instructionId);
                return OrderMapper.GetOrders(session, instruction)
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

        public static bool GetToleranceParameters(out decimal minValue, out int pricingType)
        {
            minValue = 0;
            pricingType = (int)PricingTypes.Direct;
            IDalSession session = NHSessionFactory.CreateSession();
            InstructionEngineParameters tolParams = InstructionEngineParametersMapper.GetParameters(session);
            if (tolParams != null)
            {
                switch (tolParams.PricingType)
                {
                    case PricingTypes.Direct:
                        minValue = tolParams.MinimumRebalanceAmount.Quantity;
                        break;
                    case PricingTypes.Percentage:
                        minValue = tolParams.MinimumRebalancePercentage;
                        break;
                }
                pricingType = (int)tolParams.PricingType;
            }
            session.Close();
            return true;
        }

        public static DataSet GetPricingTypes()
        {
            DataSet ds = Utils.Util.GetDataSetFromEnum(typeof(PricingTypes));
            ds.Tables[0].Rows[(int)PricingTypes.Direct - 1][1] = "EUR";
            ds.Tables[0].Rows[(int)PricingTypes.Percentage - 1][1] = "%";
            return ds;
        }

        public static void EditInstruction(int instructionId, int orderActionTypeID, DateTime executionDate, bool doNotChargeCommission, List<RebalanceExclusionDetails> exclusions)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IInstructionTypeRebalance instruction = (IInstructionTypeRebalance)InstructionMapper.GetInstruction(session, instructionId);
            if (instruction.IsEditable)
            {
                if (instruction.Edit((OrderActionTypes)orderActionTypeID, executionDate, doNotChargeCommission))
                {
                    // check exclusions
                    if (instruction.InstructionType == InstructionTypes.Rebalance)
                    {
                        IRebalanceInstruction rb = (IRebalanceInstruction)instruction;
                        if (rb.ExcludedComponents.Count > 0 || exclusions.Count > 0)
                        {
                            // check for to be deleted components
                            if (rb.ExcludedComponents.Count > 0)
                            {
                                for (int i = rb.ExcludedComponents.Count; i > 0; i--)
                                {
                                    IRebalanceExclusion x = (IRebalanceExclusion)rb.ExcludedComponents[i - 1];
                                    if (exclusions.Where(u => u.ComponentType == x.ComponentType && u.ComponentKey == x.ComponentKey).Count() == 0)
                                        rb.ExcludedComponents.RemoveExclusionAt(i-1);
                                }
                            }

                            // check for to be added components
                            if (exclusions.Count > 0)
                            {
                                foreach (RebalanceExclusionDetails exclusion in exclusions)
                                {
                                    if (rb.ExcludedComponents.Where(u => u.ComponentType == exclusion.ComponentType && u.ComponentKey == exclusion.ComponentKey).Count() == 0)
                                    {
                                        switch (exclusion.ComponentType)
                                        {
                                            case ModelComponentType.Model:
                                                rb.ExcludedComponents.AddExclusion(ModelMapper.GetModel(session, exclusion.ComponentKey));
                                                break;
                                            case ModelComponentType.Instrument:
                                                rb.ExcludedComponents.AddExclusion(InstrumentMapper.GetTradeableInstrument(session, exclusion.ComponentKey));
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    InstructionMapper.Update(session, instruction);
                }
            }
            else
                throw new ApplicationException("The instruction is no longer editable");

            session.Close();
        }

        public static int ProcessInstructions(int[] instructionIds, int pricingType, decimal minimumQty)
        {
            if (instructionIds == null || instructionIds.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            IDalSession session = NHSessionFactory.CreateSession();

            // Initialize the Engine
            ICurrency underlying = InstrumentMapper.GetBaseCurrency(session);
            InstructionEngineParameters engineParams = new InstructionEngineParameters((PricingTypes)pricingType, minimumQty, underlying);
            InstructionEngine engine = new InstructionEngine(engineParams);
            int instructionsUpdated = 0;

            if (instructionIds != null && instructionIds.Length > 0)
            {
                for (int i = 0; i < instructionIds.Length; i++)
                {
                    // Get new session
                    session = NHSessionFactory.CreateSession();
                    IInstruction instruction = InstructionMapper.GetInstruction(session, instructionIds[i]);

                    // Get UnProcessed CashTransfers
                    IRebalanceInstruction ri = null;
                    if (instruction.InstructionType == InstructionTypes.Rebalance && instruction.Status < 3)
                    {
                        ri = (IRebalanceInstruction)instruction;
                        if (ri.NeedsToProcessCashTransfers)
                            ri.CashTransfers.AddTransfers(JournalEntryMapper.GetUnProcessedCashTransfers(session, instruction.Account));
                    }
                    
                    if (engine.ProcessInstruction(instruction))
                    {
                        session.BeginTransaction();
                        InstructionMapper.Update(session, instruction);
                        instructionsUpdated++;
                        if (instruction.UpdateableOrders != null && instruction.UpdateableOrders.Count > 0)
                            OrderMapper.Insert(session, instruction.UpdateableOrders);
                        session.CommitTransaction();
                    }
                }
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

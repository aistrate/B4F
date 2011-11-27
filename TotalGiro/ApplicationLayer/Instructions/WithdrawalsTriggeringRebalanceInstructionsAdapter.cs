using System;
using System.Collections;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Instructions
{
    public static class WithdrawalsTriggeringRebalanceInstructionsAdapter
    {
        public static DataSet GetAccountsWithCashWithdrawals(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName, string filterOption)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (company != null)
            {
                string where = "";
                if (assetManagerId > 0)
                    where += string.Format(" and A.AccountOwner.Key = {0}", assetManagerId);
                if (modelPortfolioId > 0)
                    where += string.Format(" and A.ModelPortfolio.Key = {0}", modelPortfolioId);
                if (accountNumber != null && accountNumber.Length > 0)
                    where += string.Format(" and A.Number like '%{0}%'", accountNumber);
                if (accountName != null && accountName.Length > 0)
                    where += string.Format(" and A.ShortName like '%{0}%'", accountName);


                string hql = string.Format(@"from CustomerAccount A
                    left join fetch A.hibBagOfPositions
                    where A.Key in (
                    select W.Account.Key
                    from CashWithdrawalInstruction W
                    where W.IsActive = 1)
                    and A.Key not in (
                    select I.Account.Key from RebalanceInstruction I
                    where I.IsActive = 1 and I.Status <= {1}) {0}", where, ((int)RebalanceInstructionStati.CashFund).ToString());

                IList list = DataSetBuilder.GetDistinctList(session.GetListByHQL(hql));

                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    list,
                    @"Key, Number, ShortName, ActiveWithdrawalInstructions.TotalAmount, ActiveWithdrawalInstructions.TotalAmount.Quantity, 
                    ActiveWithdrawalInstructions.Count, ActiveWithdrawalInstructions.FirstWithdrawalDate, TotalCash, TotalCashFund, 
                    TotalBothCash, TotalBothCash.Quantity");

                addColumnToDataSet(ds, "NeedsRebalance", typeof(bool), "(ActiveWithdrawalInstructions_TotalAmount_Quantity + TotalBothCash_Quantity) < 0");

                if (ds.Tables[0].Rows.Count > 0 && filterOption != "ALL")
                {
                    Util.FilterDataTable(ds.Tables[0], string.Format("NeedsRebalance = {0}", (filterOption == "NEED" ? "1" : "0")), null);
                }
            }
            session.Close();
            return ds;
        }

        private static void addColumnToDataSet(DataSet ds, string columnName, Type dataType, string expr)
        {
            DataColumn col = new DataColumn(columnName, dataType, expr, MappingType.Attribute);
            col.AutoIncrement = false;
            col.ReadOnly = true;
            ds.Tables[0].Columns.Add(col);
        }

        public static DataSet GetInstructions(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAccount account = AccountMapper.GetAccount(session, accountId);
                return InstructionMapper.GetInstructions<ICashWithdrawalInstruction>(
                    session, account, InstructionReturnClass.CashWithdrawal, true)
                    .Select(c => new
                    {
                        c.Key,
                        c.DisplayStatus,
                        c.Message,
                        c.CreationDate,
                        c.WithdrawalDate,
                        Amount_DisplayString =
                            c.Amount != null ? c.Amount.DisplayString : "",
                        c.DisplayRegularity
                    })
                    .ToDataSet();
            }
        }
    }
}

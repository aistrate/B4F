using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Fees.CommCalculations;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission
{
    public static class RuleOverviewAdapter
    {
        public static DataSet GetCommissionRules()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                // Key, CommRuleName, CommCalculation.Name
                return CommRuleMapper.GetCommissionRules(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.CommRuleName, 
                        CommCalculation_Name = c.CommCalculation != null ? c.CommCalculation.Name : ""
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetAccountCommissionRules(int accountID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                CommRuleMapper.GetAccountCommissionRules(session, accountID),
                "Key, CommRuleName, CommCalculation.Name, StartDate, EndDate");
            session.Close();
            return ds;
        }

        public static DataSet GetModelCommissionRules(int modelID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                CommRuleMapper.GetModelCommissionRules(session, modelID),
                "Key, CommRuleName, CommCalculation.Name, StartDate, EndDate");
            session.Close();
            return ds;
        }

        public static DataSet GetCustomerAccounts(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
                                          string propertyList)
        {
            DataSet ds = AccountFinderAdapter.GetCustomerAccounts(assetManagerId, modelPortfolioId, accountNumber, accountName, false, true, true, 0, true, true, propertyList);
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetTradeableInstruments(string isin, string instrumentName,
            SecCategories secCategoryId, int exchangeId, int currencyNominalId)
        {
            string hqlWhere = "or I.Key in (select C.Instrument.Key from CommRule C where C.AssetManager.Key = :managementCompanyID and C.Instrument is not null)";
            string propertyList = "Key, DisplayIsinWithName";

            DataSet ds = InstrumentFinderAdapter.GetTradeableInstruments(
                SecCategoryFilterOptions.All, isin, instrumentName, secCategoryId, 
                exchangeId, currencyNominalId, ActivityReturnFilter.All, true, hqlWhere, propertyList);
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static void DeleteCommissionRule(int ruleId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ICommRule rule = CommRuleMapper.GetCommissionRule(session, ruleId);
            CommRuleMapper.Delete(session, (CommRule)rule);
            session.Close();
        }

        public static DataSet GetCommissionRules(
            string commrulename, int commRuleTypeId, int modelId, int accountId, int instrumentId, int buySell, 
            SecCategories secCategoryId, int exchangeId, DateTime startdate, DateTime enddate,
            int commcalcId, int orderactiontype, int additionalCalcId, Boolean applytoallaccounts)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                //"Key, CommRuleName, CommCalculation.Name"
                return CommRuleMapper.GetCommissionRules(session, commrulename, commRuleTypeId, 
                    modelId, accountId, instrumentId, buySell, secCategoryId, exchangeId,
                    startdate, enddate, commcalcId, orderactiontype, additionalCalcId,
                    applytoallaccounts)
                    .Select(c => new
                    {
                        c.Key,
                        c.CommRuleName,
                        CommCalculation_Name = c.CommCalculation != null ? c.CommCalculation.Name : "",
                        c.StartDate,
                        c.EndDate,
                        c.DisplayRule,
                        IsAccountActive = c.Account != null ? c.Account.Status == AccountStati.Active : true
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetBuySellOptions()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(CommRuleBuySell), SortingDirection.Descending);
            Utility.AddEmptyFirstRow(ds.Tables[0], "Key");
            return ds;
        }

        public static DataSet GetOrderActionTypeOptions()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(OrderActionTypes), SortingDirection.Ascending);
            Utility.AddEmptyFirstRow(ds.Tables[0], "Key");
            return ds;
        }
    }
}

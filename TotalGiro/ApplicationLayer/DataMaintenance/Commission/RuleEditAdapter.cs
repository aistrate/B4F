using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees.CommCalculations;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission
{
    // helper class
    public class CommRuleDetails
    {
        public CommRuleDetails()
        {
            CommCalculation = int.MinValue;
            AdditionalCalculation = int.MinValue;
            Account = int.MinValue;
            Model= int.MinValue;
            Exchange= int.MinValue;
            Instrument= int.MinValue;
            RuleSecCategory= int.MinValue;
            OrderActionType= int.MinValue;
            BuySell = ((int)CommRuleBuySell.Both);
            OpenClose = ((int)CommRuleOpenClose.Both);
            CommRuleType = ((int)CommRuleTypes.Specific);
            OriginalOrderType = ((int)BaseOrderTypes.Both);

        }
        
        public string CommRuleName;
        public int CommCalculation, AdditionalCalculation, Account, Model, Exchange, Instrument, RuleSecCategory, OrderActionType,
            BuySell = ((int)CommRuleBuySell.Both), OpenClose = ((int)CommRuleOpenClose.Both), CommRuleType = ((int)CommRuleTypes.Specific), OriginalOrderType = ((int)BaseOrderTypes.Both);
        public bool ApplyToAll, HasEmployerRelation;
        public DateTime StartDate, EndDate;
    }

    public static class RuleEditAdapter
    {
        public static DataSet GetInternalAccounts()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);

                if (company != null && !company.IsStichting)
                {
                    ds = AccountMapper.GetInternalAccounts<ICustomerAccount>(session, AccountTypes.Customer, (IAssetManager)company)
                        .Select(c => new
                        {
                            c.Key,
                            c.ShortName,
                            c.Number
                        })
                        .ToDataSet();
                    Utility.AddEmptyFirstRow(ds.Tables[0]);
                }
                return ds;
            }
        }

        public static DataSet GetExchanges()
        {
            return B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter.GetExchanges();
        }

        public static DataSet GetInstruments()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = InstrumentMapper.GetInstruments(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.Name
                    })
                    .ToDataSet();
                Utility.AddEmptyFirstRow(ds);
                return ds;
            }
        }

        public static DataSet GetSecCategories()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = SecCategoryMapper.GetSecCategories(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.Description
                    })
                    .ToDataSet();

                Utility.AddEmptyFirstRow(ds.Tables[0]);
                return ds;
            }
        }

        public static DataSet GetModelPortfolios()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                ModelMapper.GetModelsSorted(session, true, false), "Key, ModelName");
            session.Close();

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetAccountTypes()
        {
            DataSet dsTypes = new DataSet("AccountTypes");
            DataTable dtTypes = new DataTable("AccountTypes");
            dsTypes.Tables.Add(dtTypes);
            dtTypes.Columns.Add("Key", typeof(int));
            dtTypes.Columns.Add("Description", typeof(string));

            foreach (int i in Enum.GetValues(typeof(AccountTypes)))
            {
                DataRow row = dtTypes.NewRow();
                row[0] = i;
                row[1] = ((AccountTypes)i).ToString();
                dtTypes.Rows.Add(row);
            }

            return dsTypes;
        }

        public static DataSet GetCommissionCalculations()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = CommCalcMapper.GetCommissionCalculations(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.Name
                    })
                    .ToDataSet();
                Utility.AddEmptyFirstRow(ds);
                return ds;
            }
        }

        public static DataSet GetCommRuleTypes()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(CommRuleTypes), SortingDirection.Ascending);
            //Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetBuySellOptions()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(CommRuleBuySell), SortingDirection.Descending);
            return ds;
        }

        public static DataSet GetOpenCloseOptions()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(CommRuleOpenClose), SortingDirection.Descending);
            return ds;
        }

        public static DataSet GetBaseOrderTypes()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(BaseOrderTypes));
            return ds;
        }

        public static DataSet GetOrderActionTypeOptions()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(OrderActionTypes), SortingDirection.Ascending);
            //Utility.AddEmptyFirstRow(ds.Tables[0], "Key", 0);
            return ds;
        }

        public static CommRuleDetails GetCommRuleDetails(int ruleId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ICommRule rule = CommRuleMapper.GetCommissionRule(session, ruleId);
            CommRuleDetails ruleDetails = new CommRuleDetails();

            ruleDetails.CommRuleName = rule.CommRuleName.Trim();
            ruleDetails.CommCalculation = rule.CommCalculation.Key;
            ruleDetails.CommRuleType = ((int)rule.CommRuleType);

            if (rule.AdditionalCalculation != null)
                ruleDetails.AdditionalCalculation = rule.AdditionalCalculation.Key;
            if (rule.StartDate != null)
                ruleDetails.StartDate = rule.StartDate;
            if (rule.EndDate != null)
                ruleDetails.EndDate = rule.EndDate;
            if (rule.ModelPortfolio != null)
                ruleDetails.Model = rule.ModelPortfolio.Key;
            if (rule.Account != null)
                ruleDetails.Account = rule.Account.Key;
            if (rule.Exchange != null)
                ruleDetails.Exchange = rule.Exchange.Key;
            if (rule.Instrument != null)
                ruleDetails.Instrument = rule.Instrument.Key;
            if (rule.RuleSecCategory != null)
                ruleDetails.RuleSecCategory = (int)rule.RuleSecCategory.Key;
            ruleDetails.OrderActionType = (int)rule.ActionType;
            ruleDetails.BuySell = (int)rule.BuySell;
            ruleDetails.OriginalOrderType = (int)rule.OriginalOrderType;
            ruleDetails.ApplyToAll = rule.ApplyToAllAccounts;
            ruleDetails.HasEmployerRelation = rule.HasEmployerRelation;

            session.Close();

            return ruleDetails;
        }

        public static void SaveCommissionRule(int ruleId, CommRuleDetails ruleDetails)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IAccount account = (ruleDetails.Account != 0 && ruleDetails.Account != int.MinValue ? 
                AccountMapper.GetAccount(session, ruleDetails.Account) : null);
            IExchange exchange = (ruleDetails.Exchange != 0 && ruleDetails.Exchange != int.MinValue ? 
                ExchangeMapper.GetExchange(session, ruleDetails.Exchange) : null);
            IPortfolioModel model = (ruleDetails.Model != 0 && ruleDetails.Model != int.MinValue ? 
                (IPortfolioModel)ModelMapper.GetModel(session, ruleDetails.Model) : null);
            IInstrument instrument = (ruleDetails.Instrument != 0 && ruleDetails.Instrument != int.MinValue ?
                InstrumentMapper.GetInstrument(session, ruleDetails.Instrument) : null);
            ISecCategory secCategory = (ruleDetails.RuleSecCategory != 0 && ruleDetails.RuleSecCategory != int.MinValue ?
                SecCategoryMapper.GetSecCategory(session, (SecCategories)ruleDetails.RuleSecCategory) : null);

            CommRuleBuySell buySell = (CommRuleBuySell)ruleDetails.BuySell;

            CommRuleOpenClose openClose = (CommRuleOpenClose)ruleDetails.OpenClose;

            BaseOrderTypes originalOrderType = (BaseOrderTypes)ruleDetails.OriginalOrderType;

            CommRuleTypes commruleType = (CommRuleTypes)ruleDetails.CommRuleType;

            OrderActionTypes orderActionType = (OrderActionTypes)ruleDetails.OrderActionType;

            CommCalc commissionCalculation = CommCalcMapper.GetCommissionCalculation(session, ruleDetails.CommCalculation);

            CommCalc additionalCalculation = ruleDetails.AdditionalCalculation != 0 && ruleDetails.AdditionalCalculation != int.MinValue ? 
                CommCalcMapper.GetCommissionCalculation(session, ruleDetails.AdditionalCalculation) : null;

            CommRule specificRule = null;
            if (ruleId != 0)
                specificRule = (CommRule)CommRuleMapper.GetCommissionRule(session, ruleId);
            else
                specificRule = new CommRule();

            specificRule.CommRuleName = ruleDetails.CommRuleName;
            specificRule.CommRuleType = commruleType;
            specificRule.AccountType = AccountTypes.Customer;
            specificRule.StartDate = ruleDetails.StartDate;
            specificRule.EndDate = ruleDetails.EndDate;
            specificRule.Account = account;
            specificRule.ModelPortfolio = model;
            specificRule.Instrument = instrument;
            specificRule.Exchange = exchange;
            specificRule.RuleSecCategory = secCategory;
            specificRule.BuySell = buySell;
            specificRule.OpenClose = openClose;
            specificRule.OriginalOrderType = originalOrderType;
            specificRule.CommCalculation = commissionCalculation;
            specificRule.AdditionalCalculation = additionalCalculation;
            specificRule.ActionType = orderActionType;
            specificRule.ApplyToAllAccounts = ruleDetails.ApplyToAll;
            specificRule.HasEmployerRelation = ruleDetails.HasEmployerRelation;

            InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            if (emp != null)
                specificRule.AssetManager =  (IAssetManager)emp.Employer;

            CommRuleMapper.InsertOrUpdate(session, specificRule);

            session.Close();
        }
    }
}

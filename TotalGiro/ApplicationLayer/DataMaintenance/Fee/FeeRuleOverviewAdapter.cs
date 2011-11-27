using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.Fees.FeeRules;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee
{
    public static class FeeRuleOverviewAdapter
    {
        public static DataSet GetFeeRules(int feeCalcId, int modelId, 
            int accountId, int startPeriod, int endPeriod,
            bool isDefault, bool hasEmployerRelation, 
            bool executionOnly, bool sendByPost)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return FeeRuleMapper.GetFeeRules(session, feeCalcId, modelId,
                    accountId, startPeriod, endPeriod, isDefault, hasEmployerRelation,
                    executionOnly, sendByPost)
                    .Select(c => new
                        {
                            c.Key,
                            FeeCalculation_Key = c.FeeCalculation.Key,
                            FeeCalculation_Name = c.FeeCalculation.Name,
                            c.IsDefault,
                            c.DisplayRule,
                            c.StartPeriod,
                            c.EndPeriod
                        })
                        .ToDataSet();
            }
        }

        public static bool CreateDefaultFeeRule(int feeCalculationId,
            bool executionOnly, bool hasEmployerRelation, bool sendByPost, int startPeriod, int endPeriod)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IFeeCalc calc = FeeCalcMapper.GetFeeCalculation(session, feeCalculationId);

            FeeRule rule = new FeeRule(calc, null, null, true, executionOnly, hasEmployerRelation, sendByPost, startPeriod);
            if (endPeriod > 0)
                rule.EndPeriod = endPeriod;
            bool success = FeeRuleMapper.Insert(session, rule);
            session.Close();
            return success;
        }

        public static void UpdateFeeRule(int feeCalculationId, int startPeriod, int endPeriod, int original_Key)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IFeeRule rule = FeeRuleMapper.GetFeeRule(session, original_Key);
                if (rule != null)
                {
                    IFeeCalc calc = FeeCalcMapper.GetFeeCalculation(session, feeCalculationId);
                    rule.FeeCalculation = calc;
                    rule.StartPeriod = startPeriod;
                    rule.EndPeriod = endPeriod;
                    session.Update(rule);
                }
            }
        }

        public static DataSet GetActiveFeeCalculations()
        {
            DataSet ds = null;
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (company != null)
                {
                    Hashtable parameters = new Hashtable();
                    if (!company.IsStichting)
                        parameters.Add("managementCompanyID", company.Key);
                    IList<IFeeCalc> list = session.GetTypedListByNamedQuery<IFeeCalc>(
                        "B4F.TotalGiro.Fees.FeeCalculations.ActiveFeeCalculations",
                        parameters);

                    ds = list
                        .Select(c => new
                        {
                            c.Key,
                            c.Name
                        })
                        .ToDataSet();
                    Utility.AddEmptyFirstRow(ds);
                }
            }
            return ds;
        }

    }
}

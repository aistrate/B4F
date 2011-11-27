using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Accounts.Withdrawals;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.ApplicationLayer.Instructions
{
    public static class CreatePeriodicWithdrawalInstructionsAdapter
    {
        public static DataSet GetWithdrawalRules(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable();

                if (assetManagerId > 0)
                    parameters.Add("assetManagerId", assetManagerId);
                if (modelPortfolioId > 0)
                    parameters.Add("modelPortfolioId", modelPortfolioId);
                if (accountNumber != null && accountNumber.Length > 0)
                    parameters.Add("accountNumber", Util.PrepareNamedParameterWithWildcard(accountNumber));
                if (accountName != null && accountName.Length > 0)
                    parameters.Add("accountName", Util.PrepareNamedParameterWithWildcard(accountName));
                
                IList<IWithdrawalRule> rules = session.GetTypedListByNamedQuery<IWithdrawalRule>(
                    "B4F.TotalGiro.Accounts.Withdrawals.GetWithdrawalRules",
                    parameters);

                return rules
                    .Select(c => new
                    {
                        c.Key,
                        AccountID = c.Account.Key,
                        Account_Number = c.Account.Number,
                        Account_ShortName = c.Account.ShortName,
                        Account_ModelPortfolioName = c.Account.ModelPortfolioName,
                        Amount_Quantity = c.Amount.Quantity,
                        Regularity_Description = c.Regularity.Description,
                        c.FirstDateWithdrawal,
                        c.NextWithdrawalDate1,
                        c.MaxWithdrawalDate,
                        c.DoNotChargeCommission,
                        c.IsInValid
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetWithdrawalInstructions(int withdrawalRuleID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstructionMapper.GetWithdrawalInstructions(session, withdrawalRuleID)
                    .Select(c => new
                    {
                        c.Key,
                        c.DisplayStatus,
                        c.Message,
                        c.WithdrawalDate,
                        Amount_DisplayString =
                            c.Amount != null ? c.Amount.DisplayString : "",
                        c.DisplayRegularity,
                        c.IsActive,
                        c.CreationDate
                    })
                    .ToDataSet();
            }
        }

        public static DateTime GetMaximumWithdrawalCreationDate(int withdrawalRuleID)
        {
            DateTime retDate = DateTime.MinValue;
            IDalSession session = NHSessionFactory.CreateSession();
            IWithdrawalRule rule = WithdrawalRuleMapper.GetWithdrawalRule(session, withdrawalRuleID);
            if (rule != null)
                retDate = rule.GetMaxWithdrawalDate();
            return retDate;
        }

        public static bool CreatePeriodicWithdrawals(BatchExecutionResults results)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable();

                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (company != null && !company.IsStichting)
                    parameters.Add("assetManagerId", company.Key);

                int[] keys = session.GetTypedListByNamedQuery<int>(
                    "B4F.TotalGiro.Accounts.Withdrawals.GetWithdrawalRuleKeys", parameters)
                    .ToArray();

                if (keys.Count() > 0)
                    return CreatePeriodicWithdrawals(results, keys, DateTime.MinValue);
                else
                    return false;
            }
        }

        public static bool CreatePeriodicWithdrawals(BatchExecutionResults results, int key, DateTime endDate)
        {
            int[] keys = { key };
            return CreatePeriodicWithdrawals(results, keys, endDate);
        }

        private static bool CreatePeriodicWithdrawals(BatchExecutionResults results, int[] keys, DateTime endDate)
        {
            ICustomerAccount account = null;
            IDalSession session = null;

            if (keys != null && keys.Length > 0)
            {
                for (int j = 0; j < keys.Length; j++)
                {
                    int ruleKey = keys[j];
                    int itemsCreated = 0;
                    try
                    {
                        session = NHSessionFactory.CreateSession();

                        Hashtable parameters = new Hashtable(1);
                        parameters.Add("withdrawalRuleId", ruleKey);

                        IList<IWithdrawalRule> rules = session.GetTypedListByNamedQuery<IWithdrawalRule>(
                            "B4F.TotalGiro.Accounts.Withdrawals.GetWithdrawalRule",
                            parameters);
                        
                        if (rules != null && rules.Count == 1)
                        {
                            IWithdrawalRule rule = rules[0];
                            account = rule.Account;

                            if (!account.IsDeparting)
                            {
                                DateTime maxDate;

                                // if not passed in -> get the max date
                                if (endDate == DateTime.MinValue)
                                    maxDate = rule.GetMaxWithdrawalDate();
                                else
                                {
                                    if (Util.IsNotNullDate(rule.EndDateWithdrawal) && endDate > rule.EndDateWithdrawal)
                                        maxDate = rule.EndDateWithdrawal;
                                    else
                                        maxDate = endDate;
                                }
                                bool isDirty = false;
                                int start = (rule.LastWithdrawalDate >= DateTime.Today ? 0 : 1);

                                // hardcoded: Never look further then 12 months
                                for (int i = start; i < (12 + start); i++)
                                {
                                    DateTime nextDate = rule.GetSpecificDate(i);
                                    if (Util.IsNullDate(nextDate) || nextDate > maxDate)
                                        break;

                                    if (!rule.WithdrawalInstructions.Contains(nextDate))
                                    {
                                        // periodic withdrawal is always without commission
                                        account.CreateWithdrawalInstruction(DateTime.Today, nextDate, rule.Amount.Negate(), null, rule, null, rule.DoNotChargeCommission);
                                        isDirty = true;
                                        itemsCreated++;
                                    }
                                }
                                if (isDirty)
                                {
                                    AccountMapper.Update(session, account);
                                    results.MarkSuccess(itemsCreated);
                                }
                            }
                            else
                                results.MarkError(
                                    new ApplicationException(string.Format("Did not create periodic Withdrawal Instruction  for {0} since the account is departing.", (account != null ? account.DisplayNumberWithName : ruleKey.ToString()))));
                        }
                    }
                    catch (Exception ex)
                    {
                        results.MarkError(
                            new ApplicationException(string.Format("Error creating periodic Withdrawal Instruction  for {0}.", (account != null ? account.DisplayNumberWithName : ruleKey.ToString())), ex));
                    }
                }
            }
            
            if (session != null)
                session.Close();
            return true;
        }

        #region Display Results

        public static string FormatErrorsForCreatePeriodicWithdrawals(BatchExecutionResults results)
        {
            const int MAX_ERRORS_DISPLAYED = 25;

            string message = "<br/>";

            if (results.SuccessCount == 0 && results.ErrorCount == 0)
                message += "No new periodic withdrawal instructions need to be created";
            else
            {
                if (results.SuccessCount > 0)
                    message += string.Format("{0} periodic withdrawal instructions were successfully created.<br/><br/><br/>", results.SuccessCount);

                if (results.ErrorCount > 0)
                {
                    string tooManyErrorsMessage = (results.ErrorCount > MAX_ERRORS_DISPLAYED ?
                                                        string.Format(" (only the first {0} are shown)", MAX_ERRORS_DISPLAYED) : "");

                    message += string.Format("{0} errors occured while creating periodic withdrawal instructions{1}:<br/><br/><br/>",
                                             results.ErrorCount, tooManyErrorsMessage);

                    int errors = 0;
                    foreach (Exception ex in results.Errors)
                    {
                        if (++errors > MAX_ERRORS_DISPLAYED)
                            break;
                        message += Utility.GetCompleteExceptionMessage(ex) + "<br/>";
                    }
                }
            }

            return message;
        }

        #endregion
    
    }
}

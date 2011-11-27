using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts.Withdrawals;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using Microsoft.SqlServer.Server;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public class WithdrawalRuleDetails
    {
        public string RegularityLabel, AccountNumber, PandhouderPermission, TransferDescription;
        public int RegularityValue, AccountID, CounterAccountID;
        public DateTime FirstDateWithdrawal, EndDateWithdrawal;
        public decimal Amount;
        public bool IsActive, DoNotChargeCommission;
        public bool? PandhouderPermissionValue;
    }
    

    public class WithdrawalRuleEditAdapter
    {

        public static DataSet GetRegularities()
        {
            return Util.GetDataSetFromEnum(typeof(Regularities));
        }

        public static WithdrawalRuleDetails GetWithdrawalRule(int ruleID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IWithdrawalRule rule = WithdrawalRuleMapper.GetWithdrawalRule(session, ruleID);
            WithdrawalRuleDetails ruleDetails = new WithdrawalRuleDetails();
            ruleDetails.RegularityLabel = rule.Regularity.Description;
            ruleDetails.RegularityValue = (int)rule.Regularity.Key;
            if (rule.Account != null)
                ruleDetails.AccountNumber = rule.Account.Number;
            if (rule.CounterAccount != null)
                ruleDetails.CounterAccountID = rule.CounterAccount.Key;
            else
                ruleDetails.CounterAccountID = int.MinValue;
            if (Util.IsNotNullDate(rule.FirstDateWithdrawal))
                ruleDetails.FirstDateWithdrawal = rule.FirstDateWithdrawal;
            else
                ruleDetails.FirstDateWithdrawal = DateTime.MinValue;
            if (Util.IsNotNullDate(rule.EndDateWithdrawal))
                ruleDetails.EndDateWithdrawal = rule.EndDateWithdrawal;
            else
                ruleDetails.EndDateWithdrawal = DateTime.MinValue;
            ruleDetails.Amount = rule.Amount.Quantity;
            ruleDetails.PandhouderPermission = rule.PandhouderPermission.ToString();
            ruleDetails.IsActive = rule.IsActive;
            ruleDetails.DoNotChargeCommission = rule.DoNotChargeCommission;
            ruleDetails.TransferDescription = rule.TransferDescription;
            session.Close();
            return ruleDetails;
        }

        public static DataSet GetWithdrawalRules(int accountID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return WithdrawalRuleMapper.GetWithdrawalRules(session, accountID)
                .Select(c => new
                {
                    c.Key, 
                    Amount_Quantity = c.Amount != null ? c.Amount.Quantity : 0M,
                    Regularity_Description = c.Regularity.GetS(e => e.Description), 
                    c.FirstDateWithdrawal, 
                    c.EndDateWithdrawal, 
                    CounterAccount_Number = c.CounterAccount.GetS(e => e.Number), 
                    c.DoNotChargeCommission, 
                    c.IsActive
                })
                .ToDataSet();
            }
        }

        public static DataSet GetCounterAccounts(int accountId, int contactID)
        {
            DataSet ds = AccountEditAdapter.GetAttachedCounterAccounts(accountId, contactID);
            if (ds != null)
            {
                ds.Tables[0].Rows[0][1] = "Default";
            }
            return ds;
        }

        public static string GetAccountNumber(int accountID)
        {
            string strAccount = "";
            IDalSession session = NHSessionFactory.CreateSession();
            ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, accountID);
            if (acc != null)
                strAccount = acc.Number;
            session.Close();
            return strAccount;
        }

        public static bool GetDefaultCommissionNotCharged()
        {
            bool notCharged = false;
            IDalSession session = NHSessionFactory.CreateSession();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (company != null && company.CompanyType == ManagementCompanyType.AssetManager)
                notCharged = ((IAssetManager)company).DoNotChargeCommissionWithdrawals;
            session.Close();
            return notCharged;
        }

        public static void SaveWithdrawalRule(ref int ruleID, WithdrawalRuleDetails ruleDetails)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IWithdrawalRule rule = null;

            ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, ruleDetails.AccountID);
            ICounterAccount counterAcc = CounterAccountMapper.GetCounterAccount(session, ruleDetails.CounterAccountID);
            if (acc == null)
                throw new ApplicationException("Account can not be found.");

            if (ruleID != 0)
            {
                // Edit existing Rule
                rule = WithdrawalRuleMapper.GetWithdrawalRule(session, ruleID);

                if (rule == null)
                    throw new ApplicationException("Rule can not be found.");

                checkDeactivateRule(rule, ruleDetails.EndDateWithdrawal, ruleDetails.IsActive);

                rule.CounterAccount = counterAcc;
                rule.TransferDescription = ruleDetails.TransferDescription;
                rule.DoNotChargeCommission = ruleDetails.DoNotChargeCommission;

                WithdrawalRuleMapper.Update(session, rule);
            }
            else
            {
                Money amount = null;
                WithdrawalRuleRegularity regularity = null;

                if (!(acc.Status == AccountStati.Active && acc.TradeableStatus == Tradeability.Tradeable))
                    throw new ApplicationException("It is not possible a withdrawal rule for an account that is not active or tradeable.");

                if (ruleDetails.Amount != 0)
                    amount = new Money(ruleDetails.Amount, acc.AccountOwner.StichtingDetails.BaseCurrency);
                else
                    throw new ApplicationException("Set Amount.");
                regularity = WithdrawalRuleMapper.GetWithdrawalRuleRegularity(session, (Regularities)ruleDetails.RegularityValue);

                // Create new Rule
                rule = new WithdrawalRule(amount, regularity, ruleDetails.FirstDateWithdrawal, counterAcc);
                rule.DoNotChargeCommission = ruleDetails.DoNotChargeCommission;
                if (Util.IsNotNullDate(ruleDetails.EndDateWithdrawal))
                    rule.EndDateWithdrawal = ruleDetails.EndDateWithdrawal;

                if (acc.PandHouder != null)
                {
                    if (ruleDetails.PandhouderPermissionValue.HasValue && ruleDetails.PandhouderPermissionValue.Value)
                        rule.PandhouderPermission = PandhouderPermissions.Yes;
                    else
                        throw new ApplicationException("Pandhouder Permission is needed.");
                }
                rule.TransferDescription = ruleDetails.TransferDescription;

                acc.WithdrawalRules.Add(rule);
                if (rule.Validate())
                    AccountMapper.Update(session, acc);
            }
            ruleID = rule.Key;
            session.Close();
        }

        public static void DeleteWithDrawalRule(int ruleID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IWithdrawalRule rule = WithdrawalRuleMapper.GetWithdrawalRule(session, ruleID);
            if (rule != null)
            {
                if (checkDeactivateRule(rule, DateTime.Today, false))
                    WithdrawalRuleMapper.Update(session, rule);
            }
            session.Close();
        }

        private static bool checkDeactivateRule(IWithdrawalRule rule, DateTime endDateWithdrawal, bool isActive)
        {
            if (Util.IsNotNullDate(endDateWithdrawal) && Util.IsNullDate(rule.EndDateWithdrawal) ||
               (!isActive && rule.IsActive))
            {
                if (!isActive && rule.IsActive)
                {
                    if (Util.IsNullDate(endDateWithdrawal))
                        endDateWithdrawal = DateTime.Today;
                    if (rule.FirstDateWithdrawal > endDateWithdrawal)
                        endDateWithdrawal = rule.FirstDateWithdrawal;
                    rule.EndDateWithdrawal = endDateWithdrawal;
                }
                rule.IsActive = isActive;
                int instructionsToBeCancelled = 0;
                foreach (ICashWithdrawalInstruction instruction in rule.WithdrawalInstructions)
                {
                    if (instruction.IsActive && instruction.WithdrawalDate >= rule.EndDateWithdrawal)
                        instructionsToBeCancelled++;
                }
                if (instructionsToBeCancelled > 0)
                    throw new ApplicationException(string.Format("The end date/active flag of the rule can not be set since {0} withdrawal instructions exist with a withdrawal date after {1}. Please cancel the instruction(s) in 'Withdrawal Instruction Management'.", instructionsToBeCancelled, rule.EndDateWithdrawal.ToString("yyyy-MM-dd")));
            }
            return true;
        }

        public static void CheckWithdrawalRuleConditions(
                                                            int accountID,
                                                            ref bool blnTegenrekening,
                                                            ref bool blnValidTegenrekening,
                                                            ref bool blnVerpand
                                                        )
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, accountID);

            if (acc != null)
            {
                if (acc.CounterAccount != null)
                    blnTegenrekening = true;

                if (acc.CounterAccount != null && acc.CounterAccount.IsValid)
                    blnValidTegenrekening = true;

                if (acc.PandHouder != null)
                    blnVerpand = true;
            }
            session.Close();
        }

    }
}

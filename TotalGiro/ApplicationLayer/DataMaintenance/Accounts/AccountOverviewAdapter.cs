using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class AccountOverviewAdapter
    {
        public static DataSet GetCustomerAccounts(int assetManagerId, int remisierId, int remisierEmployeeId,
            int lifecycleId, int modelPortfolioId, string accountNumber, string accountName,
            bool showActive, bool showInactive, bool showTradeable, bool showNonTradeable)
        {
            //DataSet ds = AccountFinderAdapter.GetCustomerAccounts(
            //   assetManagerId, remisierId, remisierEmployeeId, modelPortfolioId, accountNumber, accountName, false, showActive, showInactive, 0, showTradeable, showNonTradeable,
            //   "Key, ShortName, Number, RemisierEmployee.Remisier.Name , RemisierEmployee.Employee.FullName, ModelPortfolioName, Status, AccountType, CreationDate");
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = AccountMapper.GetCustomerAccounts(session, assetManagerId, remisierId, remisierEmployeeId,
                    lifecycleId, modelPortfolioId, accountNumber, accountName, false, false, showActive,
                    showInactive, 0, showTradeable, showNonTradeable)
                    .Select(c => new
                    {
                        c.Key,
                        c.ShortName,
                        c.Number,
                        c.CreationDate,
                        c.ModelPortfolioName,
                        c.Status,
                        c.AccountType,
                        RemisierEmployee_Remisier_Name =
                            (c.AccountType == AccountTypes.Customer &&
                            ((ICustomerAccount)c).RemisierEmployee != null &&
                            ((ICustomerAccount)c).RemisierEmployee.Remisier != null ? ((ICustomerAccount)c).RemisierEmployee.Remisier.Name : ""),
                        RemisierEmployee_Employee_FullName =
                            (c.AccountType == AccountTypes.Customer &&
                            ((ICustomerAccount)c).RemisierEmployee != null &&
                            ((ICustomerAccount)c).RemisierEmployee.Employee != null ? ((ICustomerAccount)c).RemisierEmployee.Employee.FullName : "")
                    })
                    .ToDataSet();


                DataSet dsStatuses = AccountEditAdapter.GetAccountStatuses();
                Hashtable ht = new Hashtable();
                foreach (DataRow dr in dsStatuses.Tables[0].Rows)
                    ht[(AccountStati)dr["Key"]] = (bool)dr["IsOpen"];

                DataTable dt = ds.Tables[0];
                dt.Columns.Add("IsOpen", typeof(bool));
                foreach (DataRow dr in dt.Rows)
                    dr["IsOpen"] = ht[(AccountStati)dr["Status"]];

                return ds;
            }
        }

        public static DataSet GetCustomerAccountsToday()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<AccountStatus> stati = AccountMapper.GetAccountStatuses(session);
                return AccountMapper.GetCustomerAccountsbyCreationDate(session, DateTime.Now)
                    .Select(c => new
                    {
                        c.Key,
                        c.ShortName,
                        c.Number,
                        c.CreationDate,
                        c.ModelPortfolioName,
                        c.Status,
                        c.AccountType,
                        IsOpen =
                            stati.Where(x => x.Key.Equals(c.Status)).First().IsOpen
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetAccountFamilies(int accountID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                IAssetManager company = (IAssetManager)LoginMapper.GetCurrentManagmentCompany(session);
                if ((company == null || company.IsStichting) && accountID != 0)
                {
                    ICustomerAccount account = AccountMapper.GetAccount(session, accountID) as ICustomerAccount;
                    if (account != null)
                        company = account.AccountOwner as IAssetManager;
                }
                ds = B4F.TotalGiro.Accounts.AccountFamilyMapper.GetAccountFamilies(session, company)
                    .Select(c => new
                    {
                        c.Key,
                        c.AccountPrefix
                    })
                    .ToDataSet();

                Utility.AddEmptyFirstRow(ds);
                return ds;
            }
        }

        public static DataSet GetLifecycles(int assetManagerId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = LifecycleMapper.GetLifecycles(session, assetManagerId)
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

        public static int GetRelevantLifecycleModelID(int accountId, int lifecycleId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                int modelId = int.MinValue;
                IAccountTypeCustomer cust = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
                ILifecycle lc = LifecycleMapper.GetLifecycle(session, lifecycleId);
                IAccountHolder ah = cust.PrimaryAccountHolder ?? cust.AccountHolders.FirstOrDefault();
                CheckLifecycleForAccount(cust, ah);
                if (lc != null && ah != null)
                {
                    int? modelKey;
                    if (!(cust.IsDeparting || cust.IsUnderRebalance))
                        modelKey = lc.GetRelevantModel(ah.Contact.GetBirthFounding).GetV(x => x.Key);
                    else
                        modelKey = cust.Get(x => x.ModelPortfolio).GetV(x => x.Key);
                    if (modelKey.HasValue)
                        modelId = modelKey.Value;
                }
                return modelId;
            }
        }

        public static void CheckLifecycleForAccount(IAccountTypeCustomer cust, IAccountHolder ah)
        {
            if (ah == null)
                throw new ApplicationException(string.Format("It is not possible to set a lifecycle on account {0} since there is no primary account holder.", cust.DisplayNumberWithName));

            DateTime birthdate = ah.Contact.GetBirthFounding;
            if (Util.IsNullDate(birthdate))
                throw new ApplicationException(string.Format("It is not possible to set a lifecycle on account {0} since the primary account holder has no birthdate.", cust.DisplayNumberWithName));
        }


        public static DataSet ImportAccountsFromEffectenGiro()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList IAFE = B4F.TotalGiro.ApplicationLayer.DataMaintenance.Accounts.ImportAccountsAdapter.ImportAccountsFromEffectenGiro();
                return AccountMapper.GetCustomerAccountsbyCreationDate(session, DateTime.Now)
                    .Select(c => new
                    {
                        c.Key,
                        c.ShortName,
                        c.Number,
                        AccountOwner_CompanyName = c.AccountOwner.CompanyName,
                        c.ModelPortfolioName,
                        c.Status,
                        c.AccountType
                    })
                    .ToDataSet();
            }
        }

        public static ICustomerAccount CreateMinimalAccount(int familyID, string accountShortName)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DateTime creationDate = session.GetServerTime();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            IAccountFamily family;
            string newAccoutNumber = GetNewAccountNumber(familyID, out family);

            ICustomerAccount newBie = new CustomerAccount(newAccoutNumber, accountShortName, company, creationDate);
            newBie.Family = family;
            newBie.UseManagementFee = true;

            B4F.TotalGiro.Accounts.AccountMapper.Update(session, newBie);
            session.Close();
            return newBie;
        }

        public static string GetNewAccountNumber(int familyID, out IAccountFamily family)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            family = AccountFamilyMapper.GetAccountFamily(session, familyID);

            string newAccountNumber = AccountFamilyMapper.GetNewAccountNumber(session, family);
            return newAccountNumber;
        }

        public static bool HasEffectenGiroRights()
        {
            int currentManagmentCompanyId = GetCurrentManagmentCompanyId();
            //fudge .. only Vierlander has rights at moment.
            return currentManagmentCompanyId == 10;
        }

        public static bool ShowLifecycle()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (company.CompanyType == ManagementCompanyType.AssetManager)
                    return ((IAssetManager)company).SupportLifecycles;
                else
                    return false;
            }
            finally
            {
                session.Close();
            }
        }

        public static int GetCurrentManagmentCompanyId()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                return company.Key;
            }
            finally
            {
                session.Close();
            }
        }

    }
}

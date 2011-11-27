using System.Data;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public static class AccountFinderAdapter
    {
        public enum AccountGuiStatus
        {
            Active = 1,
            Inactive = 2,
            All = 3
        }

        public enum AccountGuiTradeability
        {
            Tradeable = 1,
            NonTradeable = 2,
            All = 3
        }

        public static bool IsLoggedInAsStichting()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                bool result = LoginMapper.IsLoggedInAsStichting(session);
                return result;
            }
        }

        public static bool IsLoggedInAsAssetManager()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                bool result = LoginMapper.IsLoggedInAsAssetManager(session);
                return result;
            }
        }

        public static bool IsLoggedInAsCompliance()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                bool result = LoginMapper.IsLoggedInAsCompliance(session);
                return result;
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

        public static string GetCurrentManagmentCompanyName()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                return company.CompanyName;
            }
            finally
            {
                session.Close();
            }
        }

        public static DataSet GetAssetManagers()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;

                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (company.CompanyType == ManagementCompanyType.EffectenGiro)
                {
                    IEffectenGiro stichting = (IEffectenGiro)company;
                    ds = ((IEffectenGiro)company).AssetManagers
                        .OrderBy(x => x.CompanyName)
                        .Where(x => x.IsActive)
                        .Select(c => new
                        {
                            c.Key,
                            c.CompanyName
                        })
                        .ToDataSet();
                    Utility.AddEmptyFirstRow(ds.Tables[0]);
                }
                return ds;
            }
        }

        public static DataSet GetRemisiers()
        {
            return GetRemisiers(0);
        }

        public static DataSet GetRemisiers(int assetManagerId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                IAssetManager assetManager = null;

                if (assetManagerId > 0)
                    assetManager = (IAssetManager)ManagementCompanyMapper.GetAssetManager(session, assetManagerId);

                IList<IRemisier> list = RemisierMapper.GetRemisiers(session, assetManager);
                if (list != null)
                {
                    ds = list
                    .Select(c => new
                    {
                        c.Key,
                        c.DisplayNameAndRefNumber
                    })
                    .ToDataSet();

                    Utility.AddEmptyFirstRow(ds.Tables[0]);
                }
                return ds;
            }
        }

        public static DataSet GetRemisierEmployees(int remisierId, int assetManagerId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;

                if (remisierId > 0)
                {
                    IList<IRemisierEmployee> list = RemisierEmployeeMapper.GetRemisierEmployees(session, remisierId);
                    if (list != null)
                    {
                        ds = list
                            .Select(c => new
                            {
                                c.Key,
                                Employee_FullNameLastNameFirst =
                                    c.Employee.FullNameLastNameFirst,
                                c.IsDefault
                            })
                            .ToDataSet();
                    }
                    if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        Util.SortDataTable(ds.Tables[0], "IsDefault DESC, Employee_FullNameLastNameFirst ASC");
                }
                else
                {
                    ds = new DataSet();
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Key", typeof(int));
                    dt.Columns.Add("Employee_FullNameLastNameFirst", typeof(string));
                    ds.Tables.Add(dt);
                }
                Utility.AddEmptyFirstRow(ds.Tables[0]);
                session.Close();
                return ds;
            }
        }

        public static DataSet GetLifecycles(int assetManagerId, ActivityReturnFilter activityFilter)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                if (activityFilter != ActivityReturnFilter.Active)
                    activityFilter = ActivityReturnFilter.All;

                DataSet ds = LifecycleMapper.GetLifecycles(session, assetManagerId, activityFilter)
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

        public static DataSet GetModelPortfolios()
        {
            return GetModelPortfolios(0, ActivityReturnFilter.Active);
        }

        public static DataSet GetModelPortfolios(int assetManagerId)
        {
            return GetModelPortfolios(assetManagerId, ActivityReturnFilter.Active);
        }

        public static DataSet GetModelPortfolios(int assetManagerId, ActivityReturnFilter activityFilter)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                IAssetManager assetManager = null;

                if (activityFilter != ActivityReturnFilter.Active)
                    activityFilter = ActivityReturnFilter.All;

                if (assetManagerId > 0)
                    assetManager = (IAssetManager)ManagementCompanyMapper.GetAssetManager(session, assetManagerId);

                IList<IPortfolioModel> list = ModelMapper.GetModelsSorted(session, assetManager, true, false, activityFilter);
                if (list != null)
                {
                    ds = list
                        .Select(c => new
                        {
                            c.Key,
                            c.ModelName
                        })
                        .ToDataSet();

                    Utility.AddEmptyFirstRow(ds);
                }
                return ds;
            }
        }

        public static DataSet GetCustomerAccounts(
            int assetManagerId, int modelPortfolioId, string accountNumber, 
            string accountName, bool retrieveNostroAccounts)
        {
            DataSet ds = GetCustomerAccounts(assetManagerId, modelPortfolioId, accountNumber, accountName, retrieveNostroAccounts,
                                             true, true, 0, true, true,
                                             "Key, Number, DisplayNumberWithName");

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetCustomerAccounts(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
                                          bool retrieveNostroAccounts, int virtualFundID)
        {
            DataSet ds;
            if (virtualFundID == 0)
                ds = GetCustomerAccounts(assetManagerId, modelPortfolioId, accountNumber, accountName, retrieveNostroAccounts,
                                             true, true, 0, true, true,
                                             "Key, Number, DisplayNumberWithName");
            else
                ds = GetAccountsforVirtualFund(virtualFundID);

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetCustomerAccounts(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
                                                  bool showActive, bool showInactive, int year,
                                                    bool showTradeable, bool showNonTradeable)
        {
            DataSet ds = GetCustomerAccounts(assetManagerId, modelPortfolioId, accountNumber, accountName, false,
                                             showActive, showInactive, year, showTradeable, showNonTradeable,  
                                             "Key, Number, DisplayNumberWithName");

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetCustomerAccounts(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
                          bool retrieveNostroAccounts, bool showActive, bool showInactive, int year,
                            bool showTradeable, bool showNonTradeable, string propertyList)
        {
            return GetCustomerAccounts(assetManagerId, 0, 0, 0, modelPortfolioId, accountNumber, accountName, retrieveNostroAccounts, 
                showActive, showInactive, year, showTradeable, showNonTradeable, propertyList);
        }

        public static DataSet GetCustomerAccounts(int assetManagerId, int remisierId, int remisierEmployeeId, int lifecycleId, int modelPortfolioId, 
                                                  string accountNumber, string accountName, bool retrieveNostroAccounts, 
                                                  bool showActive, bool showInactive, int year, bool showTradeable, bool showNonTradeable, 
                                                  string propertyList)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return AccountMapper.GetCustomerAccounts(session, assetManagerId, remisierId, remisierEmployeeId,
                    lifecycleId, modelPortfolioId, accountNumber, accountName, false, retrieveNostroAccounts, showActive,
                    showInactive, year, showTradeable, showNonTradeable)
                    .ToDataSet(propertyList);
            }
        }

        public static DataSet GetAccountsforVirtualFund(int virtualFundID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return AccountMapper.GetAccountsforVirtualFund(session, virtualFundID)
                    .Select(c => new
                    {
                        c.Key,
                        c.Number,
                        c.DisplayNumberWithName
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetAccountStatuses()
        {
            DataSet ReturnValue = new DataSet();
            DataTable Statuses = new DataTable();
            ReturnValue.Tables.Add(Statuses);

            DataColumn dc1 = new DataColumn("ID", System.Type.GetType("System.Int32"));
            Statuses.Columns.Add(dc1);

            DataColumn dc2 = new DataColumn("Status", System.Type.GetType("System.String"));
            Statuses.Columns.Add(dc2);

            DataRow dr1 = Statuses.NewRow();
            dr1["ID"] = 1;
            dr1["Status"] = "Active";
            Statuses.Rows.Add(dr1);

            DataRow dr2 = Statuses.NewRow();
            dr2["ID"] = 2;
            dr2["Status"] = "Inactive";
            Statuses.Rows.Add(dr2);

            DataRow dr3 = Statuses.NewRow();
            dr3["ID"] = 3;
            dr3["Status"] = "All";
            Statuses.Rows.Add(dr3);

            return ReturnValue;

        }

        public static DataSet GetAccountTradeability()
        {
            DataSet ReturnValue = new DataSet();
            DataTable Statuses = new DataTable();
            ReturnValue.Tables.Add(Statuses);

            DataColumn dc1 = new DataColumn("ID", System.Type.GetType("System.Int32"));
            Statuses.Columns.Add(dc1);

            DataColumn dc2 = new DataColumn("Tradeability", System.Type.GetType("System.String"));
            Statuses.Columns.Add(dc2);

            DataRow dr1 = Statuses.NewRow();
            dr1["ID"] = (int) AccountGuiTradeability.Tradeable;
            dr1["Tradeability"] = "Tradeable";
            Statuses.Rows.Add(dr1);

            DataRow dr2 = Statuses.NewRow();
            dr2["ID"] = (int) AccountGuiTradeability.NonTradeable;
            dr2["Tradeability"] = "NON-Tradeable";
            Statuses.Rows.Add(dr2);

            DataRow dr3 = Statuses.NewRow();
            dr3["ID"] = (int)AccountGuiTradeability.All;
            dr3["Tradeability"] = "All";
            Statuses.Rows.Add(dr3);

            return ReturnValue;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public static class AccountValuesAdapter
    {
        public static DataSet GetCustomerAccounts(int assetManagerId, int remisierId, int remisierEmployeeId,
            int lifecycleId, int modelPortfolioId, string accountNumber, string accountName,
            bool showActive, bool showInactive, bool showTradeable, bool showNonTradeable)
        {
            return AccountFinderAdapter.GetCustomerAccounts(assetManagerId, remisierId, remisierEmployeeId, lifecycleId,
                modelPortfolioId, accountNumber, accountName, false, showActive, showInactive, 0, 
                showTradeable, showNonTradeable,
                "Key, ShortName, Number, AccountOwner.CompanyName, ModelPortfolioName, TotalAll");
        }

        public static void GetCountTotals(int assetManagerId, int remisierId, int remisierEmployeeId, 
            int lifecycleId, int modelPortfolioId, string accountNumber, string accountName,
            bool showActive, bool showInactive, bool showTradeable, bool showNonTradeable,
            out int count, out Money totalAmount)
        {
            DataSet ds = AccountFinderAdapter.GetCustomerAccounts(assetManagerId, remisierId, remisierEmployeeId, lifecycleId,
                modelPortfolioId, accountNumber, accountName, false, showActive, showInactive, 0,
                showTradeable, showNonTradeable,
                "Key, TotalAll.Quantity");
            
            count = ds.Tables[0].Rows.Count;

            decimal total = 0;
            foreach (DataRow row in ds.Tables[0].Rows)
                total += (decimal)row[1];
            
            IDalSession session = NHSessionFactory.CreateSession();
            ICurrency baseCurrency = LoginMapper.GetCurrentManagmentCompany(session).BaseCurrency;
            totalAmount = new Money(total, baseCurrency);
            session.Close();
        }
    }
}

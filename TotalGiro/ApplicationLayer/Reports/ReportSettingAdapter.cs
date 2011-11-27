using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Reports;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.Reports
{
    public static class ReportSettingAdapter
    {
        public static DataSet GetCustomerAccounts(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
                                          string propertyList)
        {
            return GetReportAccountSettings(assetManagerId, modelPortfolioId, accountNumber, accountName, propertyList);
        }

        public static DataSet GetReportAccountSettings(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
                                  string propertyList)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IAssetManager assetManager = null;
            IPortfolioModel model = null;

            int[] accountIds = new int[3] { 680, 681, 682 };

            if (assetManagerId > 0)
                assetManager = (IAssetManager)ManagementCompanyMapper.GetAssetManager(session, assetManagerId);

            if (modelPortfolioId > 0)
                model = (IPortfolioModel)ModelMapper.GetModel(session, modelPortfolioId);


            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                ReportSettingMapper.GetReportAccountSettings(accountIds), propertyList);


            session.Close();
            return ds;
        }
    }
}

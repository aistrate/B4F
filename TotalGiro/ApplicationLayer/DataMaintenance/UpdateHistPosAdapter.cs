using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using B4F.TotalGiro.Accounts.Positions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;


namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class UpdateHistPosAdapter
    {

        public static DataSet GetAccounts(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName)
        {
            DataSet ds = null;
            ds = DataSetBuilder.CreateDataSetFromHibernateList(
                            HistoricalPositionMapper.GetMaxHistPositions(accountNumber, accountName, assetManagerId),
                            "Key, ShortName, Number, AccountOwner, AccountOwnerID, Date");
            return ds;
        }

        public static void CreateHisPosbySingleAccountNR(string accountNR)
        {
            IDalSession session;
            session = NHSessionFactory.CreateSession();

            Hashtable parameters = new Hashtable();
            parameters.Add("AccountNumber", accountNR);
            session.ExecuteStoredProcedure("EXEC [dbo].[TG_CreateHisPosbySingleAccountNR] @AccountNUmber = :AccountNumber", parameters);
            session.Close();
        }
    }




}

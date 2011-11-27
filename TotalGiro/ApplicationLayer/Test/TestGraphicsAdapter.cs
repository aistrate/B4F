using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Positions;

namespace B4F.TotalGiro.ApplicationLayer.Test
{
    public static class TestGraphicsAdapter
    {
        public static DataSet GetHistPositions()
        {
            int accountID = 386;
            IDalSession session = NHSessionFactory.CreateSession();
            IAccountTypeInternal Account = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountID);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                HistoricalPositionMapper.GetHistPositions(session, Account),
                "Key, Value.Quantity, Value.Underlying.DisplayName");

            session.Close();

            return ds;
        }
    }
}

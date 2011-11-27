using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using System.Data;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public static class CounterAccountFinderAdapter
    {
        public static DataSet GetCounterAccounts(int assetManagerId, string counterAccountNumber, string counterAccountName, string contactName,
                                                  bool isPublic, string propertyList)
        {
            return GetCounterAccounts(assetManagerId, counterAccountNumber, counterAccountName, contactName,
                                       string.Empty, true, false, isPublic, propertyList);
        }

        public static DataSet GetCounterAccounts(int assetManagerId, string counterAccountNumber, string counterAccountName, string contactName, string accountNumber,
                                  bool showActive, bool showInactive, bool isPublic, string propertyList)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAssetManager assetManager = null;
                if (assetManagerId > 0)
                    assetManager = (IAssetManager)ManagementCompanyMapper.GetAssetManager(session, assetManagerId);

                return CounterAccountMapper.GetCounterAccounts(
                    session, assetManager, counterAccountNumber, counterAccountName, 
                    contactName, accountNumber, showActive, showInactive, isPublic)
                    .ToDataSet(propertyList);
            }
        }
    }
}

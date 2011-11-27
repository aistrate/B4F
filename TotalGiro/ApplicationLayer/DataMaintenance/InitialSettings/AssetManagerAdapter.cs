using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.InitialSettings
{
    public static class AssetManagerAdapter
    {
        public static DataSet GetAssetManagers()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return ManagementCompanyMapper.GetAssetManagers(session).Select(a => new
                {
                    a.Key,
                    a.CompanyName
                }).ToDataSet();
            }
        }

        public static int SaveManagerDetails(AssetManagerRecordDetails details)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAssetManager manager = null;

                if (details.Key == 0)
                {
                    manager = new AssetManager();
                    IEffectenGiro stichting = ManagementCompanyMapper.GetEffectenGiroCompany(session);

                    string tShortName = details.Initials + @" Trading Account";
                    string tNumber = details.Initials + @"_Trading";
                    ITradingAccount newTradingAccount = new TradingAccount(tNumber, tShortName, stichting);
                    manager.TradingAccount = newTradingAccount;

                    string nShortName = details.Initials + @" Nostro Account";
                    string nNumber = details.Initials + @"_Nostro";
                    INostroAccount newNostroAccount = new NostroAccount(nNumber, nShortName, manager);
                    manager.OwnAccount = newNostroAccount;                    

                    manager.StichtingDetails = stichting;
                }
                else
                    manager = ManagementCompanyMapper.GetAssetManager(session, details.Key);

                manager.CompanyName = details.Name;
                manager.Initials = details.Initials;
                manager.IsActive = details.IsActive;
                manager.SupportLifecycles = details.SupportLifecycles;

                session.InsertOrUpdate(manager);
                return manager.Key;
            }
        }

        public static AssetManagerRecordDetails GetAssetManagerDetails(int assetManagerID)
        {
            AssetManagerRecordDetails returnValue = new AssetManagerRecordDetails(assetManagerID);
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAssetManager details = ManagementCompanyMapper.GetAssetManager(session, assetManagerID);
                returnValue.Name = details.CompanyName;
                if (details.TradingAccount != null) returnValue.TradingAccount = details.TradingAccount.ShortName;
                if (details.OwnAccount != null) returnValue.NostroAccount = details.OwnAccount.ShortName;
                if (details.Initials != null) returnValue.Initials = details.Initials;
                returnValue.IsActive = details.IsActive;
                returnValue.SupportLifecycles = details.SupportLifecycles;
            }
            return returnValue;

        }

        public class AssetManagerRecordDetails
        {
            public AssetManagerRecordDetails(int key)
            {
                this.Key = key;
            }
            public int Key;
            public string Name;
            public string TradingAccount;
            public string NostroAccount;
            public string Initials;
            public bool IsActive;
            public bool SupportLifecycles;
        }
    }
}
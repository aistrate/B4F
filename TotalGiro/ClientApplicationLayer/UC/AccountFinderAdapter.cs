using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.ClientApplicationLayer.UC
{
    public static class AccountFinderAdapter
    {
        public static KeyValuePair<int, string> GetCurrentAssetManagerInfo()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAssetManager assetManager = SecurityLayerAdapter.GetCurrentAssetManager(session);
                return new KeyValuePair<int, string>(assetManager.Key, assetManager.CompanyName);
            }
        }

        public static DataSet GetAssetManagers()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetOwnedAssetManagers(session)
                                           .Select(am => new { am.Key, am.CompanyName })
                                           .ToDataSet()
                                           .AddEmptyFirstRow();
            }
        }

        public static KeyValuePair<int, string> GetCurrentRemisierInfo()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IRemisier remisier = SecurityLayerAdapter.GetCurrentRemisier(session);
                return new KeyValuePair<int, string>(remisier.Key, remisier.Name);
            }
        }

        public static DataSet GetRemisiers(int assetManagerId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IEnumerable<IRemisier> remisiers = new List<IRemisier>();

                if (assetManagerId != int.MinValue)
                    remisiers = SecurityLayerAdapter.GetOwnedRemisiers(session, assetManagerId);

                return remisiers.Select(r => new { r.Key, r.DisplayNameAndRefNumber })
                                .ToDataSet()
                                .AddEmptyFirstRow();
            }
        }

        public static KeyValuePair<int, string> GetCurrentRemisierEmployeeInfo()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IRemisierEmployee remisierEmployee = SecurityLayerAdapter.GetCurrentRemisierEmployee(session);
                return new KeyValuePair<int, string>(remisierEmployee.Key, remisierEmployee.Employee.FullName);
            }
        }

        public static DataSet GetRemisierEmployees(int remisierId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IEnumerable<IRemisierEmployee> remisierEmployees = new List<IRemisierEmployee>();

                if (remisierId != int.MinValue)
                    remisierEmployees = SecurityLayerAdapter.GetOwnedRemisierEmployees(session, remisierId)
                                                            .OrderByDescending(re => re.IsDefault)
                                                            .ThenBy(re => re.Employee.FullNameLastNameFirst);

                return remisierEmployees.Select(re => new { re.Key, re.Employee.FullNameLastNameFirst })
                                        .ToDataSet()
                                        .AddEmptyFirstRow();
            }
        }

        public static DataSet GetModelPortfolios(int assetManagerId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IEnumerable<IPortfolioModel> models = new List<IPortfolioModel>();

                if (assetManagerId != int.MinValue)
                    models = SecurityLayerAdapter.GetOwnedModels(session, assetManagerId);

                return models.Select(m => new { m.Key, m.ModelName })
                             .ToDataSet()
                             .AddEmptyFirstRow();
            }
        }
    }
}
